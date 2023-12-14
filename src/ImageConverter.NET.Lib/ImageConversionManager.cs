using ImageConverter.NET.Lib.Logger;
using ImageMagick;

namespace ImageConverter.NET.Lib;

public static class ImageConversionManager
{
  public static IReadOnlyCollection<SupportedFormat> Formats => new List<SupportedFormat>() {
    new (1,MagickFormat.Png),
    new (2,MagickFormat.Jpeg),
    new (3,MagickFormat.Jpg),
    new (4,MagickFormat.Bmp),
    new (5,MagickFormat.WebP),
    new (6,MagickFormat.Dds),
  };
    

  public static SupportedFormat GetSupportedConversion(int id) {
    return Formats.SingleOrDefault(x => x.Id == id);
  }

  public static void Convert(string imageFilePath, string outputFilePath, SupportedFormat inputFormat, SupportedFormat outputFormat) {
    try {
      FileManager.CheckFunctionParams(imageFilePath, outputFilePath, inputFormat.FormatString, outputFormat.FormatString);
      using var image = new MagickImage(imageFilePath, new MagickReadSettings {
        Format = inputFormat.Format
      });
      image.Write(outputFilePath, outputFormat.Format);
    }
    catch (Exception ex) {
      ConsoleLogger.Error($"Exception occurred while conversion: {ex.Message}");
    }
  }
}