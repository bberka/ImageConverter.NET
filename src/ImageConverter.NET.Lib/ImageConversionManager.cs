using ImageMagick;

namespace ImageConverter.NET.Lib;

public static class ImageConversionManager
{
  public static IReadOnlyCollection<SupportedFormat> Formats => new List<SupportedFormat> {
    new(1, MagickFormat.Png),
    new(2, MagickFormat.Jpeg),
    new(3, MagickFormat.Jpg),
    new(4, MagickFormat.Bmp),
    new(5, MagickFormat.WebP),
    new(6, MagickFormat.Dds)
  };


  public static bool IsSupportedFormat(string format) {
    return Formats.Any(x => x.FormatString.Equals(format, StringComparison.OrdinalIgnoreCase));
  }

  public static bool IsSupportedFormat(int id) {
    return Formats.Any(x => x.Id == id);
  }

  public static bool IsSupportedFormat(MagickFormat format) {
    return Formats.Any(x => x.Format == format);
  }

  public static SupportedFormat? GetSupportedConversion(int id) {
    return Formats.FirstOrDefault(x => x.Id == id);
  }

  public static SupportedFormat? GetSupportedConversion(string format) {
    return Formats.FirstOrDefault(x => x.FormatString.Equals(format, StringComparison.OrdinalIgnoreCase));
  }

  public static SupportedFormat GetSupportedConversionEnsureNotNull(string format) {
    var formatData = GetSupportedConversion(format);
    if (formatData is null) throw new Exception($"Format {format} is not supported");
    return formatData.Value;
  }


  public static void Convert(string imageFilePath, string outputFilePath, SupportedFormat inputFormat, SupportedFormat outputFormat) {
    FileManager.CheckFunctionParams(imageFilePath, outputFilePath, inputFormat.FormatString, outputFormat.FormatString);
    using var image = new MagickImage(imageFilePath, new MagickReadSettings {
      Format = inputFormat.Format
    });
    image.Write(outputFilePath, outputFormat.Format);
  }
}