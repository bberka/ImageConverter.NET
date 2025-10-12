using System.Globalization;
using ImageMagick;
using Microsoft.Extensions.Logging;

namespace ImageConverter.NET.Lib;

public class ImageConverterNet(ILogger<ImageConverterNet> logger) : IImageConverterNet
{
    public void Convert(
        string imageFilePath,
        string outputFilePath,
        MagickFormat outputFormat,
        bool overwrite = false,
        int? newWidth = null,
        int? newHeight = null)
    {
        ValidateDimensions(newWidth, newHeight);

        if (!File.Exists(imageFilePath))
            throw new Exception("Input file does not exist");

        if (File.Exists(outputFilePath))
        {
            if (!overwrite)
                throw new Exception("Output file already exists");
            File.Delete(outputFilePath);
        }

        var inputFormat =
            Util.GetFormatEnum(Path.GetExtension(imageFilePath).Trim('.'));

        using var image = new MagickImage(
            imageFilePath,
            new MagickReadSettings { Format = inputFormat });

        if (newWidth.HasValue && newHeight.HasValue)
            image.Resize(newWidth.Value, newHeight.Value);

        image.Write(outputFilePath, outputFormat);
    }

    public void Convert(
        string imageFilePath,
        string outputFilePath,
        string outputFormat,
        bool overwrite = false,
        int? newWidth = null,
        int? newHeight = null)
    {
        Convert(
            imageFilePath,
            outputFilePath,
            Util.GetFormatEnum(outputFormat),
            overwrite,
            newWidth,
            newHeight);
    }

    public void ConvertFromDirectory(
        string input,
        string output,
        string outFormat,
        bool includeSubdirectories = true,
        bool overwrite = false,
        int? newWidth = null,
        int? newHeight = null)
    {
        ConvertFromDirectory(
            input,
            output,
            Util.GetFormatEnum(outFormat),
            includeSubdirectories,
            overwrite,
            newWidth,
            newHeight);
    }

    public void ConvertFromDirectory(
        string input,
        string output,
        MagickFormat outFormat,
        bool includeSubdirectories = true,
        bool overwrite = false,
        int? newWidth = null,
        int? newHeight = null)
    {
        input = string.IsNullOrEmpty(input)
            ? Util.GetInputFolderDefault()
            : input;
        output = string.IsNullOrEmpty(output)
            ? Util.GetOutputFolderDefault()
            : output;

        if (!Directory.Exists(input))
            throw new Exception("Input directory does not exist");

        if (!Directory.Exists(output))
            Directory.CreateDirectory(output);

        var imageFiles = Util.GetSupportedFormatImageFiles(input, includeSubdirectories);
        if (imageFiles.Count == 0)
            throw new Exception("No files found in input directory");

        logger.LogInformation("{Count} files found in input directory", imageFiles.Count);

        var convertedFileCount = 0;
        Parallel.ForEach(imageFiles, imageFile =>
        {
            try
            {
                var outputFilePath = Util.MakeOutputFilePath(
                    imageFile,
                    input,
                    output,
                    outFormat);

                var outFolder = Path.GetDirectoryName(outputFilePath);
                if (!string.IsNullOrEmpty(outFolder) && !Directory.Exists(outFolder))
                    Directory.CreateDirectory(outFolder);

                Convert(
                    imageFile,
                    outputFilePath,
                    outFormat,
                    overwrite,
                    newWidth,
                    newHeight);

                logger.LogInformation(
                    "Converted {File} to {Format}",
                    Util.MakeRelativePath(imageFile, input),
                    outFormat.ToString().ToLower(new CultureInfo("en-US")));

                Interlocked.Increment(ref convertedFileCount);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error converting {File}",
                    Util.MakeRelativePath(imageFile, input));
            }
        });

        logger.LogInformation("Converted {Count} files", convertedFileCount);
    }

    public void ResizeFromDirectory(
        string input,
        string output,
        int width,
        int height,
        bool includeSubdirectories = true,
        bool overwrite = false)
    {
        input = string.IsNullOrEmpty(input)
            ? Util.GetInputFolderDefault()
            : input;
        output = string.IsNullOrEmpty(output)
            ? Util.GetOutputFolderDefault()
            : output;

        if (!Directory.Exists(input))
            throw new Exception("Input directory does not exist");

        if (!Directory.Exists(output))
            Directory.CreateDirectory(output);

        var imageFiles = Util.GetSupportedFormatImageFiles(input, includeSubdirectories);
        if (imageFiles.Count == 0)
            throw new Exception("No files found in input directory");

        logger.LogInformation("{Count} files found in input directory", imageFiles.Count);

        var resizedFileCount = 0;
        Parallel.ForEach(imageFiles, imageFile =>
        {
            try
            {
                var outputFilePath = Util.MakeOutputFilePath(
                    imageFile,
                    input,
                    output,
                    Util.GetFileFormat(imageFile));

                if (File.Exists(outputFilePath))
                {
                    if (!overwrite)
                    {
                        logger.LogWarning(
                            "Output file already exists: {File}",
                            Util.MakeRelativePath(imageFile, input));
                        return;
                    }

                    File.Delete(outputFilePath);
                }

                var outFolder = Path.GetDirectoryName(outputFilePath);
                if (!string.IsNullOrEmpty(outFolder) && !Directory.Exists(outFolder))
                    Directory.CreateDirectory(outFolder);

                using var image = new MagickImage(imageFile);
                image.Resize(width, height);
                image.Write(outputFilePath);

                logger.LogInformation(
                    "Resized {File} to {Width}x{Height}",
                    Util.MakeRelativePath(imageFile, input),
                    width,
                    height);

                Interlocked.Increment(ref resizedFileCount);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error resizing {File}",
                    Util.MakeRelativePath(imageFile, input));
            }
        });

        logger.LogInformation("Resized {Count} files", resizedFileCount);
    }

    public IEnumerable<string> GetSupportedFormats()
    {
        return Enum.GetNames<MagickFormat>()
            .Select(x => x.ToLower(new CultureInfo("en-US")));
    }

    private static void ValidateDimensions(int? newWidth, int? newHeight)
    {
        if (newWidth.HasValue && newHeight.HasValue &&
            (newWidth.Value <= 0 || newHeight.Value <= 0))
            throw new Exception("Invalid dimensions");

        if (newWidth.HasValue && !newHeight.HasValue)
            throw new Exception("Height must be specified when width is provided");

        if (newHeight.HasValue && !newWidth.HasValue)
            throw new Exception("Width must be specified when height is provided");
    }
}