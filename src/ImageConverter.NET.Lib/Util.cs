using System.Globalization;
using ImageMagick;

namespace ImageConverter.NET.Lib;

internal static class Util
{
  public static string GetInputFolderDefault() {
    var inputFolder = Path.Combine(Directory.GetCurrentDirectory(), "input");
    if (!Directory.Exists(inputFolder))
      Directory.CreateDirectory(inputFolder);
    return inputFolder;
  }

  public static string GetOutputFolderDefault() {
    var outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "output");
    if (!Directory.Exists(outputFolder))
      Directory.CreateDirectory(outputFolder);
    return outputFolder;
  }

  public static MagickFormat GetFormatEnum(string format) {
    return Enum.Parse<MagickFormat>(format, true);
  }


  public static string MakeOutputFilePath(string filePath, string inputFolder, string outputFolder, MagickFormat outputFormat) {
    var outputFilePath = filePath.Replace("." + Path.GetExtension(filePath).Trim('.'), "." + outputFormat.ToString().ToLower(new CultureInfo("en-US"))).Replace(inputFolder, outputFolder);
    return outputFilePath;
  }

  public static string MakeRelativePath(string filePath, string inputFolder) {
    var path = filePath.Replace(inputFolder, "");
    if (path.StartsWith("\\") || path.StartsWith("/")) path = path[1..];
    return path;
  }

  public static MagickFormat GetFileFormat(string input) {
    return Enum.Parse<MagickFormat>(Path.GetExtension(input).Trim('.'));
  }

  public static bool IsSupportedFormat(string filePath) {
    //is defined in enums and is supported by ImageMagick
    return Enum.TryParse<MagickFormat>(Path.GetExtension(filePath).Trim('.'), true, out _);
  }

  public static List<string> GetSupportedFormatImageFiles(string path, bool includeSubDirectories = true) {
    if (includeSubDirectories)
      return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                      .Where(IsSupportedFormat)
                      .ToList();
    return Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(IsSupportedFormat)
                    .ToList();
  }
}