
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;

namespace webapi.Services
{
    public class ImageService
    {
        public async Task CompressAndSaveImageAsync(IFormFile imageFile, string outputPath)
        {
            using var imageStream = imageFile.OpenReadStream();
            using var image = Image.Load(imageStream);

            var encoder = new WebpEncoder();

            await Task.Run(() => image.Save(outputPath, encoder));
        }
    }
}
