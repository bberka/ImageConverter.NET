// See https://aka.ms/new-console-template for more information

using ImageConverter.NET.Lib;
using ImageConverter.NET.Lib.Abstract;
using ImageConverter.NET.Lib.Converter;
using ImageConverter.NET.Lib.Logger;


const string Message =
  @"1. Convert DDS to PNG
2. Convert PNG to DDS
3. Convert WEBP to PNG
0. Exit

Enter your choice: ";

Console.Write(Message);
var choice = Console.ReadLine();
Console.Clear();
var strChoice = choice;
switch (choice) {
  case "1":
    strChoice = "Convert DDS to PNG";
    break;
  case "2":
    strChoice = "Convert PNG to DDS";
    break;
  case "3":
    strChoice = "Convert WEBP to PNG";
    break;
  case "0":
    strChoice = "Exit";
    PrintSelected();
    Exit();
    break;
  default:
    strChoice = "Invalid choice";
    PrintSelected();
    Exit();
    break;
}

try {
  ClearConsole();
  PrintSelected();
  var input = FileManager.GetInputFolderConsoleInput();
  ClearConsole();
  PrintSelected();
  ConsoleLogger.Info("Input folder:" + input);
  var output = FileManager.GetOutputFolderConsoleInput();
  ClearConsole();
  PrintSelected();
  ConsoleLogger.Info("Input folder: " + input);
  ConsoleLogger.Info("Output folder: " + output);
  
  switch (choice) {
    case "1":
      var ddsFiles = FileManager.FilterFiles(input, "dds");
      if (ddsFiles.Count == 0) throw new Exception("No DDS files found in current directory");
      foreach (var ddsFile in ddsFiles) {
        IImageConverter dds = new DdsToPngConverter();
        dds.ConvertImage(ddsFile, ddsFile.Replace(".dds", ".png").Replace(input, output));
      }

      ConsoleLogger.Info("Done");
      break;
    case "2":
      var pngFiles = FileManager.FilterFiles(input, "png");
      if (pngFiles.Count == 0) throw new Exception("No PNG files found in current directory");
      foreach (var pngFile in pngFiles) {
        IImageConverter png = new PngToDdsConverter();
        png.ConvertImage(pngFile, pngFile.Replace(".png", ".dds").Replace(input, output));
      }

      ConsoleLogger.Info("Done");
      break;
    case "3":
      var webpFiles = FileManager.FilterFiles(input, "webp");
      if (webpFiles.Count == 0) throw new Exception("No WEBP files found in current directory");
      foreach (var webpFile in webpFiles) {
        IImageConverter webp = new WebpToPngConverter();
        webp.ConvertImage(webpFile, webpFile.Replace(".webp", ".png").Replace(input, output));
      }

      ConsoleLogger.Info("Done");
      break;
  }
}
catch (Exception e) {
  ConsoleLogger.Error($"Exception occurred: {e.Message}");
}

Console.ReadLine();
return;


void Exit() {
  ConsoleLogger.Info("Exiting");
  Environment.Exit(0);
}

void PrintSelected() {
  ConsoleLogger.Info("Selected: " + strChoice);
}

void ClearConsole() {
  Console.Clear();
}