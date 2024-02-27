using Cocona;
using ImageConverter.NET;
using ImageConverter.NET.Lib;
using ImageConverter.NET.Lib.Logger;

Console.Title = VersionManager.VersionText;
ConsoleLogger.Info(VersionManager.VersionText);

CoconaApp.Run<CoconaImageConverterApp>(args);

Console.WriteLine("Press any key to exit...");
Console.ReadKey();