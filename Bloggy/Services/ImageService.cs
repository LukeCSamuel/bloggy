using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Bloggy.Services
{
  public class ImageService(BlobService blob)
  {
    public async Task<string> UploadImageAsync(ImageUploadOptions uploadOptions)
    {
      using var memStream = new MemoryStream();
      await uploadOptions.Raw.CopyToAsync(memStream);

      if (memStream.Length == 0)
      {
        throw new EmptyRequestBodyException();
      }

      memStream.Position = 0;
      using var image = Image.Load(memStream);

      // Scale image to desired size
      image.Mutate(x => x.Resize(new ResizeOptions
      {
        Mode = ResizeMode.Max,
        Size = new Size(uploadOptions.Width, uploadOptions.Height)
      }));

      // Encode to webp into a memory stream
      var memOut = new MemoryStream();
      await image.SaveAsWebpAsync(memOut);
      memOut.Position = 0;

      return await blob.UploadImageAsync($"{uploadOptions.Name}.webp", memOut);
    }
  }

  public record ImageUploadOptions
  {
    public required Stream Raw { get; init; }
    public required string Name { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
  }

  public class EmptyRequestBodyException() : Exception("Empty request body");
}