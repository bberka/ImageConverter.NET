using ImageConverter.NET.Lib;
using ImageConverter.NET.Lib.Logger;

Console.Title = VersionManager.VersionText;
ConsoleLogger.Info(VersionManager.VersionText);

//OUT NAME
Console.WriteLine("Enter out folder name for conversion: ");
var folderNameForConversion = Console.ReadLine();
if (string.IsNullOrEmpty(folderNameForConversion)) {
  ConsoleLogger.Error("Invalid out folder name");
  Console.ReadLine();
  return;
}

//FORMAT
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


var folderName = folderNameForConversion + "_" + choiceInt + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
if (!Directory.Exists(folderName)) {
  Directory.CreateDirectory(folderName);
}
var inputFolder = Path.Combine(Directory.GetCurrentDirectory(), "input");
Console.Clear();
ConsoleLogger.Info(VersionManager.VersionText);
ConsoleLogger.Info("Input Folder: " + inputFolder);
ConsoleLogger.Info("Out Folder Name For Conversion: " + folderNameForConversion);
ConsoleLogger.Info("Output Format: " + choice.Value.FormatString);

try {
  var newInputFolderPath = Path.Combine(folderName, "input");
  Directory.CreateDirectory(newInputFolderPath);
  var newOutputFolderPath = Path.Combine(folderName, "output");
  Directory.CreateDirectory(newOutputFolderPath);
  var imageFiles = FileManager.GetSupportedFormatImageFiles(inputFolder);
  if (imageFiles.Count == 0) throw new Exception("No files found in input directory");
  var convertedFileCount = 0;
  ConsoleLogger.Info($"{imageFiles.Count} files found in input directory");
  foreach (var imageFile in imageFiles)
    try {
      var outputFilePath = imageFile.GetOutputFilePath(inputFolder, newOutputFolderPath, choice.Value);
      var inputFormat = ImageConversionManager.GetSupportedConversionEnsureNotNull(imageFile.Extension);
      ImageConversionManager.Convert(imageFile.FilePath, outputFilePath, inputFormat, choice.Value);
      ConsoleLogger.Info($"Converted {imageFile.GetRelativePath(inputFolder)} to {choice.Value.FormatString}");
      convertedFileCount++;
      var originalInputFilePath = imageFile.FilePath;
      var newInputFilePath = imageFile.FilePath.Replace(inputFolder, newInputFolderPath);
      File.Move(originalInputFilePath,newInputFilePath,true);
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