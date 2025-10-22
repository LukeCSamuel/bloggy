using Microsoft.Azure.Cosmos;
using Bloggy.Models;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Bloggy.Services
{
  public class CosmosService
  {
    readonly CosmosClient _client;
    string DatabaseName { get; }
    string ContainerName { get; }
    Task ServiceReady { get; }

    public CosmosService(EnvService env)
    {

      DatabaseName = env.AppSettings.Cosmos.Database;
      ContainerName = env.AppSettings.Cosmos.ContainerName;

      CosmosClientOptions? options = null;
      if (env.AppEnvironment is AppEnvironment.Development)
      {
        // Disable TLS/SSL in development environment because the emulated DB has an untrusted cert
        options = new()
        {
          HttpClientFactory = () => new HttpClient(new HttpClientHandler()
          {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
          }),
          ConnectionMode = ConnectionMode.Gateway,
        };
      }

      _client = new CosmosClient(env.CosmosConnectionString, options);
      ServiceReady = InitializeDatabase();
    }

    public async Task<T?> GetByIdAsync<T>(string id) where T : CosmosModel
    {
      var container = await GetContainerAsync();
      var queryable = container.GetItemLinqQueryable<T>();
      var type = typeof(T).Name;
      using FeedIterator<T> feed = queryable
        .Where(item => item.id == id && item.type == type)
        .ToFeedIterator();

      var results = await ExtractResults(feed);
      return results.SingleOrDefault();
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>(Expression<Func<T, bool>>? predicate = null) where T : CosmosModel
    {
      var container = await GetContainerAsync();
      var queryable = container.GetItemLinqQueryable<T>();
      var type = typeof(T).Name;

      var query = queryable.Where(item => item.type == type);
      if (predicate is not null)
      {
        query = query.Where(predicate);
      }

      using FeedIterator<T> feed = query.ToFeedIterator();

      return await ExtractResults(feed);
    }

    public async Task UpdateAsync<T>(T model, ClaimsPrincipal? user = null, bool force = false) where T : CosmosModel
    {
      // Check if the model requires authorization before proceeding
      if (!force && model is AuthorizedCosmosModel withOwner)
      {
        await AuthorizeAsync(withOwner, user);
      }

      // Make the update
      var container = await GetContainerAsync();
      await container.UpsertItemAsync(model);
    }

    public Task AuthorizeAsync<T>(T model, ClaimsPrincipal? user) where T : AuthorizedCosmosModel
    {
      return AuthorizeAsync(model.id, user, model);
    }

    public async Task AuthorizeAsync<T>(string entityId, ClaimsPrincipal? user, T? model = null) where T : AuthorizedCosmosModel
    {
      if (user is null)
      {
        throw new AuthorizationRequiredException();
      }

      // Check owner of model in database
      var existing = await GetByIdAsync<T>(entityId);
      var ownerId = existing is AuthorizedCosmosModel owned ? owned.ownerId : model?.ownerId;
      var actorId = AuthService.GetUserId(user);
      if (ownerId is not null && actorId != ownerId)
      {
        throw new AuthorizationFailedException();
      }
    }

    private async Task<IEnumerable<T>> ExtractResults<T>(FeedIterator<T> feed)
    {
      var result = new List<T>();
      while (feed.HasMoreResults)
      {
        var response = await feed.ReadNextAsync();
        foreach (var item in response)
        {
          result.Add(item);
        }
      }
      return result;
    }

    private async Task InitializeDatabase()
    {
      // This is primarily for local development to initialize the emulator
      var res = await _client.CreateDatabaseIfNotExistsAsync(DatabaseName, throughput: 400);
      await res.Database.CreateContainerIfNotExistsAsync(ContainerName, partitionKeyPath: "/id");
    }

    private async Task<Container> GetContainerAsync()
    {
      await ServiceReady;
      return _client.GetDatabase(DatabaseName).GetContainer(ContainerName);
    }
  }

  public class AuthorizationRequiredException() : Exception("Authorization is required") { }

  public class AuthorizationFailedException() : Exception("The user is not authorized to complete this action") { }
}