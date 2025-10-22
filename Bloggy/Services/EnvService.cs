using System.Text.Json;

namespace Bloggy.Services
{
  public class EnvService
  {
    public AppEnvironment AppEnvironment { get; }
    public string CosmosConnectionString { get; }
    public string BlobStorageConnectionString { get; }
    public string JwtKey { get; }

    public EnvService ()
    {
      var appEnv = Environment.GetEnvironmentVariable("APP_ENVIRONMENT");
      AppEnvironment = appEnv is "development" ? AppEnvironment.Development : AppEnvironment.Production;

      CosmosConnectionString = Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING")
        ?? throw new Exception("A CosmosDB connection string should be provided in the 'COSMOS_CONNECTION_STRING' environment variable.");

      BlobStorageConnectionString = Environment.GetEnvironmentVariable("BLOB_STORAGE_CONNECTION_STRING")
        ?? throw new Exception("A blob storage connection string should be provided in the 'BLOB_STORAGE_CONNECTION_STRING' environment variable.");

      JwtKey = Environment.GetEnvironmentVariable("AUTHENTICATION_JWT_KEY")
        ?? throw new Exception("A JWT encryption key should be provided in the 'AUTHENTICATION_JWT_KEY' environment variable.");
    }
  }

  public enum AppEnvironment
  {
    Development,
    Production
  }
}