using Cocona;
using ImageConverter.NET;
using ImageConverter.NET.Lib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var logLevel = ParseLogLevel(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(logLevel)
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        theme: AnsiConsoleTheme.Code
    )
    .CreateLogger();

var builder = CoconaApp.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);
builder.Services.AddLogging();

builder.Services.AddScoped<IImageConverterNet, ImageConverterNet>();

var app = builder.Build();

CoconaApp.Run<CoconaImageConverterApp>(args);

app.Run();
return;

static LogEventLevel ParseLogLevel(string[] args)
{
    var index = Array.FindIndex(args, a => a.Equals("--log-level", StringComparison.OrdinalIgnoreCase));
    if (index < 0 || index + 1 >= args.Length) return LogEventLevel.Information; // Default
    return Enum.TryParse<LogEventLevel>(args[index + 1], true, out var level) ? level : LogEventLevel.Information; // Default
}