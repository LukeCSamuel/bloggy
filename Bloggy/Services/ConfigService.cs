using System.Text.Json;

namespace Bloggy.Services
{
  internal class ConfigService
  {
    public AppEnvironment AppEnvironment { get; }
    public string CosmosConnectionString { get; }
    public AppSettings AppSettings { get; }

    public ConfigService ()
    {
      var appEnv = Environment.GetEnvironmentVariable("APP_ENVIRONMENT");
      AppEnvironment = appEnv is "development" ? AppEnvironment.Development : AppEnvironment.Production;

      CosmosConnectionString = Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING")
        ?? throw new Exception("A CosmosDB connection string should be provided in the 'COSMOS_CONNECTION_STRING' environment variable.");

      var settingsFile = AppEnvironment is AppEnvironment.Development ? "appSettings.development.json" : "appSettings.json";
      AppSettings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(settingsFile))
        ?? new AppSettings()
        {
          CosmosDatabaseName = "caswell",
          CosmosContainerName = "bloggy",
        };
    }

  }

  public class AppSettings
  {
    public required string CosmosDatabaseName { get; set; }
    public required string CosmosContainerName { get; set; }
  }

  internal enum AppEnvironment
  {
    Development,
    Production
  }

  static class ConfigServiceExtensions
  {
    public static IServiceCollection AddConfigService(this IServiceCollection services)
    {
      var config = new ConfigService();
      return services
        .AddSingleton(config);
    }
  }
}