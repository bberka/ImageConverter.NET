namespace ImageConverter.NET.Lib.Logger;

/// <summary>
///   Basic console logger for heavy api request logging.
///   File logging with heavy api request sometime creating errors.
///   With this you only log to console but if error happens you log with EasLog or some other library to a file.
/// </summary>
public static class ConsoleLogger
{
  private const ConsoleColor FatalColor = ConsoleColor.Magenta;
  private const ConsoleColor ErrorColor = ConsoleColor.Red;
  private const ConsoleColor BaseColor = ConsoleColor.White;
  private const ConsoleColor WarningColor = ConsoleColor.Yellow;
  private const ConsoleColor InfoColor = ConsoleColor.Green;
  private const ConsoleColor DebugColor = ConsoleColor.Blue;
  private const ConsoleColor TraceColor = ConsoleColor.Cyan;

  public static void Log(string message) {
    Console.WriteLine(message);
  }

  /// <summary>
  ///   Writes log to console with given color. Creates new line.
  /// </summary>
  /// <param name="message"></param>
  /// <param name="color"></param>
  public static void Log(string message, ConsoleColor color) {
    Console.ForegroundColor = color;
    Console.WriteLine(message);
    Console.ResetColor();
  }

  public static void Log(string message, ConsoleColor color, bool newLine) {
    Console.ForegroundColor = color;
    if (newLine)
      Console.WriteLine(message);
    else
      Console.Write(message);
    Console.ResetColor();
  }

  public static void Log(LogLevel level, string message) {
    switch (level) {
      case LogLevel.Error:
        Log(message, ErrorColor);
        break;
      case LogLevel.Warning:
        Log(message, WarningColor);
        break;
      case LogLevel.Information:
        Log(message, InfoColor);
        break;
      case LogLevel.Debug:
        Log(message, DebugColor);
        break;
      case LogLevel.Trace:
        Log(message, TraceColor);
        break;
      case LogLevel.Fatal:
        Log(message, FatalColor);
        break;
      default:
        Log(message, BaseColor);
        break;
    }
  }


  public static void Error(string message) {
    Log(LogLevel.Error, message);
  }

  public static void Fatal(string message) {
    Log(LogLevel.Fatal, message);
  }

  public static void Warn(string message) {
    Log(LogLevel.Warning, message);
  }

  public static void Info(string message) {
    Log(LogLevel.Information, message);
  }

  public static void Debug(string message) {
    Log(LogLevel.Debug, message);
  }

  public static void Trace(string message) {
    Log(LogLevel.Trace, message);
  }
}