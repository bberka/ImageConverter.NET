using ImageConverter.NET.Lib.Abstract;
using ImageConverter.NET.Lib.Data;
using SixLabors.ImageSharp.Advanced;

namespace ImageConverter.NET.Lib.Converter;

public sealed class PngToDdsConverter : IImageConverter
{
  public void ConvertImage(string imageFilePath, string outputFilePath) {
    if (!File.Exists(imageFilePath))
      throw new Exception("Input file does not exists");
    if (File.Exists(outputFilePath))
      throw new Exception("Output file already exists");
    if (!imageFilePath.EndsWith(".png"))
      throw new Exception("Input file path must end with png: " + imageFilePath);
    if (!outputFilePath.EndsWith(".dds"))
      throw new Exception("Output file path must end with dds: " + outputFilePath);
    var dds = new PngImage(imageFilePath);
    dds.Save(outputFilePath);
  }
}