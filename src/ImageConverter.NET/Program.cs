using ImageConverter.NET;
using ImageConverter.NET.Lib;
using ImageConverter.NET.Lib.Logger;

Console.Title = ConsoleUIHelper.VersionText;

var inputFormat = ConsoleUIHelper.GetInputFormat();
Console.Clear();
ConsoleLogger.Info("Input Format: " + inputFormat.FormatString);
Console.WriteLine();
var outputFormat = ConsoleUIHelper.GetOutputFormat(inputFormat.Id);
Console.Clear();
ConsoleLogger.Info("Input Format: " + inputFormat.FormatString);
ConsoleLogger.Info("Output Format: " + outputFormat.FormatString);
Console.WriteLine();
var inputDirectory = FileManager.GetInputFolderConsoleInput();
Console.Clear();
ConsoleLogger.Info("Input Format: " + inputFormat.FormatString);
ConsoleLogger.Info("Output Format: " + outputFormat.FormatString);
ConsoleLogger.Info("Input Directory: " + inputDirectory);
Console.WriteLine();
var outputDirectory = FileManager.GetOutputFolderConsoleInput();
Console.Clear();
ConsoleLogger.Info("Input Format: " + inputFormat.FormatString);
ConsoleLogger.Info("Output Format: " + outputFormat.FormatString);
ConsoleLogger.Info("Input Directory: " + inputDirectory);
ConsoleLogger.Info("Output Directory: " + outputDirectory);
Console.WriteLine();

try {
  var files = FileManager.FilterFiles(inputDirectory, inputFormat.FormatString);
  if (files.Count == 0) throw new Exception($"No {inputFormat.FormatString} files found in input directory");
  foreach (var file in files) {
    var outputFilePath = file.Replace("." + inputFormat.FormatString, "." + outputFormat.FormatString).Replace(inputDirectory, outputDirectory);
    ImageConversionManager.Convert(file, outputFilePath, inputFormat, outputFormat);
  }

  ConsoleLogger.Info($"Converted {files.Count} number of files");
}
catch (Exception e) {
  ConsoleLogger.Error($"Exception occurred: {e.Message}");
}

Console.ReadLine();