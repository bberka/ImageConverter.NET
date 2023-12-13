using ImageConverter.NET.Lib.Abstract;
using ImageConverter.NET.Lib.Data;

namespace ImageConverter.NET.Lib.Converter;

public sealed class DdsToPngConverter : IImageConverter
{
  public void ConvertImage(string imageFilePath, string outputFilePath) {
    if (!File.Exists(imageFilePath))
      throw new Exception("Input file does not exists");
    if (File.Exists(outputFilePath))
      throw new Exception("Output file already exists");
    if (!imageFilePath.EndsWith(".dds"))
      throw new Exception("Input file path must end with dds: " + imageFilePath);
    if (!outputFilePath.EndsWith(".png"))
      throw new Exception("Output file path must end with png: " + outputFilePath);
    var dds = new DdsImage(imageFilePath);
    dds.Save(outputFilePath);
  }
}