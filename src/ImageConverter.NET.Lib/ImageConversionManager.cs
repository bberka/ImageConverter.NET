using System.Globalization;
using ImageConverter.NET.Lib.Logger;
using ImageMagick;

namespace ImageConverter.NET.Lib;

public static class ImageConversionManager
{
  public static void Convert(string imageFilePath,
                             string outputFilePath,
                             MagickFormat outputFormat,
                             bool overwrite = false,
                             int? newWidth = null,
                             int? newHeight = null) {
    if (!File.Exists(imageFilePath))
      throw new Exception("Input file does not exists");
    if (File.Exists(outputFilePath)) {
      if (!overwrite)
        throw new Exception("Output file already exists");
      File.Delete(outputFilePath);
    }

    var inputFormat = Util.GetFormatEnum(Path.GetExtension(imageFilePath).Trim('.'));

    using var image = new MagickImage(imageFilePath,
                                      new MagickReadSettings {
                                        Format = inputFormat
                                      });
    if (newWidth.HasValue && newHeight.HasValue)
      image.Resize(newWidth.Value, newHeight.Value);
    image.Write(outputFilePath, outputFormat);
  }

  public static void Convert(string imageFilePath,
                             string outputFilePath,
                             string outputFormat,
                             bool overwrite = false,
                             int? newWidth = null,
                             int? newHeight = null) {
    Convert(imageFilePath,
            outputFilePath,
            Util.GetFormatEnum(outputFormat),
            overwrite,
            newWidth,
            newHeight);
  }

  public static void ConvertFromDirectory(string input,
                                          string output,
                                          string outFormat,
                                          bool includeSubdirectories = true,
                                          bool overwrite = false,
                                          int? newWidth = null,
                                          int? newHeight = null) {
    ConvertFromDirectory(input,
                         output,
                         Util.GetFormatEnum(outFormat),
                         includeSubdirectories,
                         overwrite,
                         newWidth,
                         newHeight);
  }

  public static void ConvertFromDirectory(string input,
                                          string output,
                                          MagickFormat outFormat,
                                          bool includeSubdirectories = true,
                                          bool overwrite = false,
                                          int? newWidth = null,
                                          int? newHeight = null) {
    input = string.IsNullOrEmpty(input)
              ? Util.GetInputFolderDefault()
              : input;
    output = string.IsNullOrEmpty(output)
               ? Util.GetOutputFolderDefault()
               : output;
    if (!Directory.Exists(input))
      throw new Exception("Input directory does not exists");
    if (!Directory.Exists(output))
      Directory.CreateDirectory(output);
    var imageFiles = Util.GetSupportedFormatImageFiles(input, includeSubdirectories);
    if (imageFiles.Count == 0) throw new Exception("No files found in input directory");
    ConsoleLogger.Info($"{imageFiles.Count} files found in input directory");
    var convertedFileCount = 0;
    foreach (var imageFile in imageFiles)
      try {
        var outputFilePath = Util.MakeOutputFilePath(imageFile, input, output, outFormat);
        var outFolder = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrEmpty(outFolder) && !Directory.Exists(outFolder)) Directory.CreateDirectory(outFolder);
        Convert(imageFile,
                outputFilePath,
                outFormat,
                overwrite,
                newWidth,
                newHeight);
        ConsoleLogger.Info($"Converted {Util.MakeRelativePath(imageFile, input)} to {outFormat.ToString().ToLower(new CultureInfo("en-US"))}");
        convertedFileCount++;
      }
      catch (Exception ex) {
        ConsoleLogger.Error($"Error occurred while converting file: {Util.MakeRelativePath(imageFile, input)} \n\tError: {ex.Message}");
      }

    ConsoleLogger.Info($"Converted {convertedFileCount} number of files");
  }

  public static void ResizeFromDirectory(string input, string output, int width, int height, bool includeSubdirectories = true, bool overwrite = false) {
    input = string.IsNullOrEmpty(input)
              ? Util.GetInputFolderDefault()
              : input;
    output = string.IsNullOrEmpty(output)
               ? Util.GetOutputFolderDefault()
               : output;
    if (!Directory.Exists(input))
      throw new Exception("Input directory does not exists");
    if (!Directory.Exists(output))
      Directory.CreateDirectory(output);
    var imageFiles = Util.GetSupportedFormatImageFiles(input, includeSubdirectories);
    if (imageFiles.Count == 0) throw new Exception("No files found in input directory");
    ConsoleLogger.Info($"{imageFiles.Count} files found in input directory");
    var resizedFileCount = 0;
    foreach (var imageFile in imageFiles)
      try {
        var outputFilePath = Util.MakeOutputFilePath(imageFile, input, output, Util.GetFileFormat(imageFile));
        if (File.Exists(outputFilePath)) {
          if (!overwrite) {
            ConsoleLogger.Error($"Output file already exists: {Util.MakeRelativePath(imageFile, input)}");
            continue;
          }
          File.Delete(outputFilePath);
        }
        var outFolder = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrEmpty(outFolder) && !Directory.Exists(outFolder)) Directory.CreateDirectory(outFolder);
        using var image = new MagickImage(imageFile);
        image.Resize(width, height);
        image.Write(outputFilePath);
        ConsoleLogger.Info($"Resized {Util.MakeRelativePath(imageFile, input)} to {width}x{height}");
        resizedFileCount++;
      }
      catch (Exception ex) {
        ConsoleLogger.Error($"Error occurred while resizing file: {Util.MakeRelativePath(imageFile, input)} \n\tError: {ex.Message}");
      }

    ConsoleLogger.Info($"Resized {resizedFileCount} number of files");
  }

  public static IEnumerable<string> GetSupportedFormats() {
    return Enum.GetNames<MagickFormat>().Select(x => x.ToLower(new CultureInfo("en-US")));
  }
}