using ImageConverter.NET.Lib.Logger;
using ImageMagick;

namespace ImageConverter.NET.Lib;

public static class ImageConversionManager
{
  public static readonly IReadOnlyCollection<SupportedConversion> SupportedConversions = new List<SupportedConversion> {
    //DONT PUT 0 AS ID, IT WILL BE USED AS EXIT VALUE
    new(1, MagickFormat.Dds, MagickFormat.Png, "Convert DDS to PNG"),
    new(2, MagickFormat.Png, MagickFormat.Dds, "Convert PNG to DDS"),
    new(3, MagickFormat.WebP, MagickFormat.Png, "Convert WEBP to PNG"),
    new(4, MagickFormat.WebP, MagickFormat.Dds, "Convert WEBP to DDS")
  };

  public static SupportedConversion GetSupportedConversion(uint id) {
    return SupportedConversions.FirstOrDefault(x => x.Id == id) ?? throw new ArgumentException("Invalid id");
  }

  public static void Convert(string imageFilePath, string outputFilePath, MagickFormat inputFormat, MagickFormat outputFormat) {
    try {
      var inputFormatString = inputFormat.ToString().ToLower();
      var outputFormatString = outputFormat.ToString().ToLower();
      FileManager.CheckFunctionParams(imageFilePath, outputFilePath, inputFormatString, outputFormatString);
      using var image = new MagickImage(imageFilePath);
      image.Write(outputFilePath, outputFormat);
    }
    catch (Exception ex) {
      ConsoleLogger.Error($"Exception occurred while conversion: {ex.Message}");
    }
  }
}