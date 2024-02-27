namespace ImageConverter.NET.Lib;

public static class VersionManager
{
  public static Version Version => typeof(ImageConversionManager).Assembly.GetName().Version ?? new Version(0, 0);
  public static string VersionText => $"ImageConverter.NET v{Version.Major}.{Version.Minor}";
}