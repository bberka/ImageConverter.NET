using ImageConverter.NET.Lib;
using ImageConverter.NET.Lib.Logger;

namespace ImageConverter.NET;

public static class ConsoleUIHelper
{
  public static Version Version => typeof(ImageConversionManager).Assembly.GetName().Version;
  public static string VersionText => $"ImageConverter.NET v{Version.Major}.{Version.Minor}.{Version.Build}";


  private static void PrintFormats(int? inputId = null) {
    var formats = ImageConversionManager.Formats.Where(x => x.Id != inputId).ToList();
    foreach (var item in formats) ConsoleLogger.Log($"{item.Id}. {item.FormatString}");
  }

  public static SupportedFormat GetInputFormat() {
    SupportedFormat? choice = null;
    while (choice == null) {
      PrintFormats();
      Console.Write("Enter input format: ");
      var choiceStr = Console.ReadLine();
      if (!int.TryParse(choiceStr, out var choiceInt)) continue;
      choice = ImageConversionManager.GetSupportedConversion(choiceInt);
      if (choice is null) continue;
    }

    return choice.Value;
  }

  public static SupportedFormat GetOutputFormat(int exceptId) {
    SupportedFormat? choice = null;
    while (choice == null) {
      PrintFormats(exceptId);
      Console.Write("Enter output format: ");
      var choiceStr = Console.ReadLine();
      if (!int.TryParse(choiceStr, out var choiceInt)) continue;
      if (exceptId == choiceInt) continue;
      choice = ImageConversionManager.GetSupportedConversion(choiceInt);
      if (choice is null) continue;
    }

    return choice.Value;
  }

  public static void Exit() {
    ConsoleLogger.Info("Exiting");
    Environment.Exit(0);
  }
}