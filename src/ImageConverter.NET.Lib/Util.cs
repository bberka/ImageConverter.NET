using System.Globalization;
using ImageMagick;

namespace ImageConverter.NET.Lib;

internal static class Util
{
    private static string EnsureFolder(string folderName)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    public static string GetInputFolderDefault() => EnsureFolder("input");
    public static string GetOutputFolderDefault() => EnsureFolder("output");

    // --- Format parsing ---
    public static MagickFormat GetFormatEnum(string format)
    {
        if (string.IsNullOrWhiteSpace(format))
            throw new ArgumentException("Format string cannot be null or empty.", nameof(format));

        var cleaned = format.Trim().TrimStart('.');
        return Enum.Parse<MagickFormat>(cleaned, ignoreCase: true);
    }

    public static MagickFormat GetFileFormat(string filePath)
    {
        var ext = Path.GetExtension(filePath);
        if (string.IsNullOrWhiteSpace(ext))
            throw new ArgumentException("File has no extension.", nameof(filePath));

        var cleaned = ext.Trim('.').Trim();
        return Enum.Parse<MagickFormat>(cleaned, ignoreCase: true);
    }

    // --- Path builders ---
    public static string MakeOutputFilePath(
        string inputFilePath, string inputFolder, string outputFolder, MagickFormat outputFormat)
    {
        var fileName = Path.GetFileNameWithoutExtension(inputFilePath);
        var relPath = Path.GetRelativePath(inputFolder, Path.GetDirectoryName(inputFilePath) ?? inputFolder);

        var finalFolder = Path.Combine(outputFolder, relPath);
        if (!Directory.Exists(finalFolder))
            Directory.CreateDirectory(finalFolder);

        var newFileName = $"{fileName}.{outputFormat.ToString().ToLower(CultureInfo.InvariantCulture)}";
        return Path.Combine(finalFolder, newFileName);
    }

    public static string MakeRelativePath(string filePath, string inputFolder)
    {
        var relative = Path.GetRelativePath(inputFolder, filePath);
        return relative.Replace('\\', '/');
    }

    // --- Helpers ---
    public static bool IsSupportedFormat(string filePath)
    {
        var ext = Path.GetExtension(filePath).Trim('.').Trim();
        return Enum.TryParse<MagickFormat>(ext, true, out _);
    }

    public static List<string> GetSupportedFormatImageFiles(string path, bool includeSubDirectories = true)
    {
        var option = includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        return Directory.GetFiles(path, "*.*", option)
            .Where(IsSupportedFormat)
            .ToList();
    }
}