using ImageConverter.NET.Lib;
using ImageConverter.NET.Lib.Logger;

Console.Title = VersionManager.VersionText;
ConsoleLogger.Info(VersionManager.VersionText);
Console.Write("""
              Enter nothing to use default input folder
              Input folder:
              """);
var inputFolder = Console.ReadLine();
inputFolder = string.IsNullOrEmpty(inputFolder)
                ? FileManager.GetInputFolderDefault()
                : inputFolder;
if (!Directory.Exists(inputFolder)) {
  ConsoleLogger.Error("Input folder does not exists or is not accessible: " + inputFolder);
  Console.ReadLine();
  return;
}

Console.Write("""
              Enter nothing to use default output folder
              Output folder:
              """);
var outputFolder = Console.ReadLine();
outputFolder = string.IsNullOrEmpty(outputFolder)
                 ? FileManager.GetOutputFolderDefault()
                 : outputFolder;
if (!Directory.Exists(outputFolder))
  Directory.CreateDirectory(outputFolder);

foreach (var item in ImageConversionManager.Formats) ConsoleLogger.Log($"{item.Id}. {item.FormatString}");
Console.Write("Output format: ");
var choiceStr = Console.ReadLine();
if (!int.TryParse(choiceStr, out var choiceInt)) {
  ConsoleLogger.Error("Invalid choice");
  Console.ReadLine();

  return;
}

var choice = ImageConversionManager.GetSupportedConversion(choiceInt);
if (choice is null) {
  ConsoleLogger.Error("Invalid choice");
  Console.ReadLine();
  return;
}

Console.Clear();
ConsoleLogger.Info(VersionManager.VersionText);
ConsoleLogger.Info("Input Folder: " + inputFolder);
ConsoleLogger.Info("Output Folder: " + outputFolder);
ConsoleLogger.Info("Output Format: " + choice.Value.FormatString);

try {
  var imageFiles = FileManager.GetSupportedFormatImageFiles(inputFolder);
  if (imageFiles.Count == 0) throw new Exception("No files found in input directory");
  var convertedFileCount = 0;
  ConsoleLogger.Info($"{imageFiles.Count} files found in input directory");
  foreach (var imageFile in imageFiles)
    try {
      var outputFilePath = imageFile.GetOutputFilePath("input", "output", choice.Value);
      var inputFormat = ImageConversionManager.GetSupportedConversionEnsureNotNull(imageFile.Extension);
      ImageConversionManager.Convert(imageFile.FilePath, outputFilePath, inputFormat, choice.Value);
      ConsoleLogger.Info($"Converted {imageFile.GetRelativePath(inputFolder)} to {choice.Value.FormatString}");
      convertedFileCount++;
    }
    catch (Exception ex) {
      ConsoleLogger.Error($"Error occurred while converting file: {imageFile.GetRelativePath(inputFolder)} \n\tError: {ex.Message}");
    }

  ConsoleLogger.Info($"Converted {convertedFileCount} number of files");
}
catch (Exception e) {
  ConsoleLogger.Error($"Exception occurred: {e.Message}");
}

Console.ReadLine();