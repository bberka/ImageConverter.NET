using ImageConverter.NET;
using ImageConverter.NET.Lib;
using ImageConverter.NET.Lib.Logger;

var choice = ConsoleUIHelper.GetChoice();
if (choice == 0) ConsoleUIHelper.Exit();
var selectedConversionType = ImageConversionManager.GetSupportedConversion(choice);
if (selectedConversionType is null) ConsoleUIHelper.Exit();
try {
  ConsoleUIHelper.ClearConsole();
  ConsoleUIHelper.PrintSelected(selectedConversionType);
  var input = FileManager.GetInputFolderConsoleInput();
  ConsoleUIHelper.ClearConsole();
  ConsoleUIHelper.PrintSelected(selectedConversionType);
  ConsoleLogger.Info("Input folder:" + input);
  var output = FileManager.GetOutputFolderConsoleInput();
  ConsoleUIHelper.ClearConsole();
  ConsoleUIHelper.PrintSelected(selectedConversionType);
  ConsoleLogger.Info("Input folder: " + input);
  ConsoleLogger.Info("Output folder: " + output);
  var files = FileManager.FilterFiles(input, selectedConversionType.InputFormatString);
  if (files.Count == 0) throw new Exception($"No {selectedConversionType.InputFormatString} files found in input directory");
  foreach (var file in files) ImageConversionManager.Convert(file, file.Replace("." + selectedConversionType.InputFormatString, "." + selectedConversionType.OutputFormatString).Replace(input, output), selectedConversionType.InputFormat, selectedConversionType.OutputFormat);
 ConsoleLogger.Info($"Converted {files.Count} number of files" );
}
catch (Exception e) {
  ConsoleLogger.Error($"Exception occurred: {e.Message}");
}

Console.ReadLine();