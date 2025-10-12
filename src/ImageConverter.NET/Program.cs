using Cocona;
using ImageConverter.NET;
using ImageConverter.NET.Lib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var logLevel = ParseLogLevel(args);
var fileLogPath = GetFileLogPath(args);

var logConfig = new LoggerConfiguration()
    .MinimumLevel.Is(logLevel)
    .WriteTo.Console(
        outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        theme: AnsiConsoleTheme.Code
    );

if (!string.IsNullOrEmpty(fileLogPath))
{
    if (!Path.IsPathRooted(fileLogPath))
        fileLogPath = Path.Combine(Directory.GetCurrentDirectory(), fileLogPath);

    logConfig = logConfig.WriteTo.File(
        fileLogPath,
        fileSizeLimitBytes: 10 * 1024 * 1024, // 10 MB
        rollingInterval: RollingInterval.Day,
        outputTemplate:
        "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
    );
}

Log.Logger = logConfig.CreateLogger();

Log.Information("Starting ImageConverter.NET with log level {Level}", logLevel);

var builder = CoconaApp.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);
builder.Services.AddLogging();

builder.Services.AddScoped<IImageConverterNet, ImageConverterNet>();

var app = builder.Build();

app.AddCommands<ImageConverterNetCli>();

app.Run();

return;

// ---- Helpers ----

static LogEventLevel ParseLogLevel(string[] args)
{
    var index = Array.FindIndex(args, a => a.Equals("--log-level", StringComparison.OrdinalIgnoreCase));
    if (index < 0 || index + 1 >= args.Length)
        return LogEventLevel.Information; // Default
    return Enum.TryParse<LogEventLevel>(args[index + 1], true, out var level)
        ? level
        : LogEventLevel.Information;
}

static string? GetFileLogPath(string[] args)
{
    var index = Array.FindIndex(args, a => a.Equals("--file-log", StringComparison.OrdinalIgnoreCase));
    if (index < 0 || index + 1 >= args.Length)
        return null;
    return args[index + 1];
}