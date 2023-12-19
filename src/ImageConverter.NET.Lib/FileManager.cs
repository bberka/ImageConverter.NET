namespace ImageConverter.NET.Lib;

public static class FileManager
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


  public static List<ImageFile> GetSupportedFormatImageFiles(string path, bool includeSubDirectories = true) {
    if (includeSubDirectories)
      return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                      .Where(x => ImageConversionManager.Formats.Any(y => x.EndsWith(y.FormatString, StringComparison.OrdinalIgnoreCase)))
                      .Select(x => new ImageFile(x))
                      .Where(x => ImageConversionManager.IsSupportedFormat(x.Extension))
                      .ToList();
    return Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(x => ImageConversionManager.Formats.Any(y => x.EndsWith(y.FormatString, StringComparison.OrdinalIgnoreCase)))
                    .Select(x => new ImageFile(x))
                    .Where(x => ImageConversionManager.IsSupportedFormat(x.Extension))
                    .ToList();
  }

  internal static void CheckFunctionParams(string imageFilePath, string outputFilePath, string inputExtension, string outputExtension) {
    if (!File.Exists(imageFilePath))
      throw new Exception("Input file does not exists");
    if (File.Exists(outputFilePath))
      throw new Exception("Output file already exists");
    if (!imageFilePath.EndsWith(inputExtension, StringComparison.OrdinalIgnoreCase))
      throw new Exception("Input file path must end with " + inputExtension + ": " + imageFilePath);
    if (!outputFilePath.EndsWith(outputExtension, StringComparison.OrdinalIgnoreCase))
      throw new Exception("Output file path must end with " + outputExtension + ": " + outputFilePath);
  }
}