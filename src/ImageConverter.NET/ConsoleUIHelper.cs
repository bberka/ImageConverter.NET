using ImageConverter.NET.Lib;
using ImageConverter.NET.Lib.Logger;

namespace ImageConverter.NET;

public static class ConsoleUIHelper
{
  public static Version Version => typeof(ImageConversionManager).Assembly.GetName().Version;
  public  static string VersionText => $"ImageConverter.NET v{Version.Major}.{Version.Minor}.{Version.Build}.{Version.Revision}";
  public static void WriteVersion() {
    ConsoleLogger.Info(VersionText);
    ConsoleLogger.Log();
  }
  public static void PrintSelected(SupportedConversion conversion) {
    ConsoleLogger.Info("Selected: " + conversion.UIName);
  }

  public static void PrintUsage() {
    Console.Clear();
    WriteVersion();
    var converters = ImageConversionManager.SupportedConversions.ToList();
    foreach (var item in converters) ConsoleLogger.Log($"{item.Id}. {item.UIName}");
    ConsoleLogger.Log("0. Exit");
  }

  public static uint GetChoice() {
 
    uint? choice = null;
    while (choice == null) {
      PrintUsage();
      Console.WriteLine();
      Console.Write("Enter your choice: ");
      var choiceStr = Console.ReadLine();
      if (!uint.TryParse(choiceStr, out var choiceInt)) continue;
      choice = choiceInt;
    }

    return choice.Value;
  }

  public static void Exit() {
    ConsoleLogger.Info("Exiting");
    Environment.Exit(0);
  }


  public static void ClearConsole() {
    Console.Clear();
  }
}