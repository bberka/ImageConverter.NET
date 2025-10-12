using ImageMagick;

namespace ImageConverter.NET.Lib;

public interface IImageConverterNet
{
    ImageConversionResult ConvertFromDirectory(
        string input,
        string output,
        string outFormat,
        bool includeSubdirectories = true,
        bool overwrite = false,
        int? newWidth = null,
        int? newHeight = null,
        int? maxParallelism = null,
        string? summaryPath = null);

    ImageConversionResult ResizeFromDirectory(
        string input,
        string output,
        int width,
        int height,
        bool includeSubdirectories = true,
        bool overwrite = false,
        int? maxParallelism = null,
        string? summaryPath = null);

    IEnumerable<string> GetSupportedFormats();
}