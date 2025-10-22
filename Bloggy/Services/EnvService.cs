using System.Text.Json;

namespace Bloggy.Services
{
  public class EnvService
  {
    public AppEnvironment AppEnvironment { get; }
    public string CosmosConnectionString { get; }
    public string BlobStorageConnectionString { get; }
    public string JwtKey { get; }
    public AppSettings AppSettings { get; }

    public EnvService()
    {
      var appEnv = Environment.GetEnvironmentVariable("APP_ENVIRONMENT");
      AppEnvironment = appEnv is "development" ? AppEnvironment.Development : AppEnvironment.Production;

      CosmosConnectionString = Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING")
        ?? throw new Exception("A CosmosDB connection string should be provided in the 'COSMOS_CONNECTION_STRING' environment variable.");

      BlobStorageConnectionString = Environment.GetEnvironmentVariable("BLOB_STORAGE_CONNECTION_STRING")
        ?? throw new Exception("A blob storage connection string should be provided in the 'BLOB_STORAGE_CONNECTION_STRING' environment variable.");

      JwtKey = Environment.GetEnvironmentVariable("AUTHENTICATION_JWT_KEY")
        ?? throw new Exception("A JWT encryption key should be provided in the 'AUTHENTICATION_JWT_KEY' environment variable.");

      var settingsFile = AppEnvironment is AppEnvironment.Development ? "appSettings.development.json" : "appSettings.json";
      AppSettings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(settingsFile))
        ?? new AppSettings()
        {
          Jwt = new() { Issuer = "https://halloween.caswell.dev" },
          Cosmos = new()
          {
            Database = "caswell",
            ContainerName = "bloggy",
          },
          Storage = new()
          {
            DeliveryHost = "https://s.caswell.dev",
            Root = "bloggy",
            ContainerName = "$web",
          },
        };
    }
  }

  public class AppSettings
  {
    public required JwtAppSettings Jwt { get; set; }
    public required CosmosAppSettings Cosmos { get; set; }
    public required StorageAppSettings Storage { get; set; }
  }

  public class JwtAppSettings
  {
    public required string Issuer { get; set; }
  }

  public class CosmosAppSettings
  {
    public required string Database { get; set; }
    public required string ContainerName { get; set; }
  }
  
  public class StorageAppSettings
  {
    public required string DeliveryHost { get; set; }
    public required string Root { get; set; }
    public required string ContainerName { get; set; }
  }

  public enum AppEnvironment
  {
    Development,
    Production
  }
}