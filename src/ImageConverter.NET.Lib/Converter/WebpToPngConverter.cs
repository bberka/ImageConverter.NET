using ImageConverter.NET.Lib.Abstract;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;

namespace ImageConverter.NET.Lib.Converter;

public sealed class WebpToPngConverter : IImageConverter
{
  public void ConvertImage(string imageFilePath, string outputFilePath) {
    if (!File.Exists(imageFilePath))
      throw new Exception("Input file does not exists");
    if (File.Exists(outputFilePath))
      throw new Exception("Output file already exists");
    if (!imageFilePath.EndsWith(".webp"))
      throw new Exception("Input file path must end with webp: " + imageFilePath);
    if (!outputFilePath.EndsWith(".png"))
      throw new Exception("Output file path must end with png: " + outputFilePath);
    using var input = File.OpenRead(imageFilePath);
    using var output = File.OpenWrite(outputFilePath);
    // Decode the WebP image
    using var image = Image.Load(input);
    // Encode the image as PNG
    image.Save(output, new PngEncoder());
  }
}