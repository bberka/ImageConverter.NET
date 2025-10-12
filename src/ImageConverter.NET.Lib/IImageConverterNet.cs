using ImageMagick;

namespace ImageConverter.NET.Lib;

public interface IImageConverterNet
{
    void Convert(
        string imageFilePath,
        string outputFilePath,
        MagickFormat outputFormat,
        bool overwrite = false,
        int? newWidth = null,
        int? newHeight = null);

    void Convert(
        string imageFilePath,
        string outputFilePath,
        string outputFormat,
        bool overwrite = false,
        int? newWidth = null,
        int? newHeight = null);

    void ConvertFromDirectory(
        string input,
        string output,
        string outFormat,
        bool includeSubdirectories = true,
        bool overwrite = false,
        int? newWidth = null,
        int? newHeight = null);

    void ConvertFromDirectory(
        string input,
        string output,
        MagickFormat outFormat,
        bool includeSubdirectories = true,
        bool overwrite = false,
        int? newWidth = null,
        int? newHeight = null);

    void ResizeFromDirectory(
        string input,
        string output,
        int width,
        int height,
        bool includeSubdirectories = true,
        bool overwrite = false);

    IEnumerable<string> GetSupportedFormats();
}