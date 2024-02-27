using Cocona;
using ImageConverter.NET.Lib;
using ImageConverter.NET.Lib.Logger;

namespace ImageConverter.NET;

public class CoconaImageConverterApp
{
  [Command("convert-image",
           Description = "Converts an images in given input directory to the specified format.")]
  public void ConvertImage(
    [Option("input", Description = "Path to the input directory.")]
    string input,
    [Option("output", Description = "Path to the output directory.")]
    string output,
    [Option("convert-to", Description = "Target format (e.g., 'png').")]
    string convertTo,
    [Option("include-subdirectories", Description = "Include subdirectories.")]
    bool includeSubdirectories = true,
    [Option("overwrite", Description = "Overwrite existing files.")]
    bool overwrite = false,
    [Option("new-width", Description = "Target width.")]
    int? newWidth = null,
    [Option("new-height", Description = "Target height.")]
    int? newHeight = null) {
    try {
  
      ImageConversionManager.ConvertFromDirectory(input,
                                                  output,
                                                  convertTo,
                                                  includeSubdirectories,
                                                  overwrite,
                                                  newWidth,
                                                  newHeight);
    }
    catch (Exception ex) {
      ConsoleLogger.Error($"Error converting image: {ex.Message}");
    }
  }

  [Command("resize-image",
           Description = "Resizes an image in the given input directory to the specified dimensions.")]
  public void ResizeImage(
    [Option("input", Description = "Path to the input directory.")]
    string input,
    [Option("output", Description = "Path to the output directory.")]
    string output,
    [Option("width", Description = "Target width.")]
    int width,
    [Option("height", Description = "Target height.")]
    int height,
    [Option("include-subdirectories", Description = "Include subdirectories.")]
    bool includeSubdirectories = true,
    [Option("overwrite", Description = "Overwrite existing files.")]
    bool overwrite = false) {
    try {
      ImageConversionManager.ResizeFromDirectory(input, output, width, height, includeSubdirectories, overwrite);
    }
    catch (Exception ex) {
      ConsoleLogger.Error($"Error resizing image: {ex.Message}");
    }
  }

  [Command("list-formats",
           Description = "Lists supported image formats.")]
  public void ListFormats() {
    var formats = ImageConversionManager.GetSupportedFormats();
    ConsoleLogger.Info("Supported formats:");
    foreach (var format in formats) ConsoleLogger.Info(format);
  }
}