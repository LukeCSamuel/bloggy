using System.Text.Json;

namespace Bloggy.Services
{
  public class EnvService
  {
    public AppEnvironment AppEnvironment { get; }
    public string CosmosConnectionString { get; }
    public string JwtKey { get; }

    public EnvService ()
    {
      var appEnv = Environment.GetEnvironmentVariable("APP_ENVIRONMENT");
      AppEnvironment = appEnv is "development" ? AppEnvironment.Development : AppEnvironment.Production;

      CosmosConnectionString = Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING")
        ?? throw new Exception("A CosmosDB connection string should be provided in the 'COSMOS_CONNECTION_STRING' environment variable.");

      JwtKey = Environment.GetEnvironmentVariable("AUTHENTICATION_JWT_KEY")
        ?? throw new Exception("A JWT encryption key is needed to provide authentication.");
    }
  }

  public class AppSettings
  {
    public required string CosmosDatabaseName { get; set; }
    public required string CosmosContainerName { get; set; }
  }

  public enum AppEnvironment
  {
    Development,
    Production
  }
}