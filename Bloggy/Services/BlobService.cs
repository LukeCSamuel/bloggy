using Azure.Storage.Blobs;

namespace Bloggy.Services
{
  public class BlobService
  {
    readonly BlobContainerClient _client;
    string Root { get; }
    string DeliveryHost { get; }

    public BlobService(EnvService env)
    {
      Root = env.AppSettings.Storage.Root;
      DeliveryHost = env.AppSettings.Storage.DeliveryHost;

      var containerName = env.AppSettings.Storage.ContainerName;
      var serviceClient = new BlobServiceClient(env.BlobStorageConnectionString);
      _client = serviceClient.GetBlobContainerClient(containerName);
    }

    public async Task<string> UploadImageAsync (string name, Stream image)
    {
      var path = $"{Root}/img/up/{name}";

      var blob = _client.GetBlobClient(path);
      await blob.UploadAsync(image, false);

      return $"{DeliveryHost}/{path}";
    }
  }
}