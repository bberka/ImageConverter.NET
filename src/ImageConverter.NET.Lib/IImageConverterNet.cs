using ImageMagick;

namespace ImageConverter.NET.Lib;

public interface IImageConverterNet
{
    // --- Single-file operations ---
    void Convert(
        string inputFilePath,
        string outputFilePath,
        string outputFormat,
        bool overwrite = false,
        uint? newWidth = null,
        uint? newHeight = null);

    void Resize(
        string inputFilePath,
        string outputFilePath,
        uint width,
        uint height,
        bool overwrite = false);

    // --- Directory operations ---
    ImageConversionResult ConvertFromDirectory(
        string input,
        string output,
        string outFormat,
        bool includeSubdirectories = true,
        bool overwrite = false,
        uint? newWidth = null,
        uint? newHeight = null,
        int? maxParallelism = null,
        string? summaryPath = null);

    ImageConversionResult ResizeFromDirectory(
        string input,
        string output,
        uint width,
        uint height,
        bool includeSubdirectories = true,
        bool overwrite = false,
        int? maxParallelism = null,
        string? summaryPath = null);

    // --- Supported Formats ---
    IEnumerable<string> GetSupportedFormats();
}