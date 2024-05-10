
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;

namespace webapi.Services
{
    public class ImageService
    {
        public async Task CompressAndSaveImageAsync(IFormFile imageFile, string outputPath, int quality = 75)
        {
            using var imageStream = imageFile.OpenReadStream();
            using var image = Image.Load(imageStream);

            // var encoder = new JpegEncoder
            // {
            //     Quality = quality,
            //             // CompressionLevel = PngCompressionLevel.BestCompression,
            //             // BitDepth = PngBitDepth.Bit1,
            // };

            var encoder = new WebpEncoder();

            await Task.Run(() => image.Save(outputPath, encoder));
        }
    }
}
