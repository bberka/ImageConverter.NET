using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using ImageMagick;
using Microsoft.Extensions.Logging;

namespace ImageConverter.NET.Lib;

public class ImageConverterNet(ILogger<ImageConverterNet> logger) : IImageConverterNet
{
    public ImageConversionResult ConvertFromDirectory(
        string input,
        string output,
        string outFormat,
        bool includeSubdirectories = true,
        bool overwrite = false,
        int? newWidth = null,
        int? newHeight = null,
        int? maxParallelism = null,
        string? summaryPath = null)
    {
        var format = Util.GetFormatEnum(outFormat);
        return ConvertInternal(
            input,
            output,
            i => Util.MakeOutputFilePath(i, input, output, format),
            f => ConvertSingle(f.input, f.output, format, overwrite, newWidth, newHeight),
            includeSubdirectories,
            overwrite,
            maxParallelism,
            summaryPath);
    }

    public ImageConversionResult ResizeFromDirectory(
        string input,
        string output,
        int width,
        int height,
        bool includeSubdirectories = true,
        bool overwrite = false,
        int? maxParallelism = null,
        string? summaryPath = null)
    {
        return ConvertInternal(
            input,
            output,
            i => Util.MakeOutputFilePath(i, input, output, Util.GetFileFormat(i)),
            f => ResizeSingle(f.input, f.output, width, height, overwrite),
            includeSubdirectories,
            overwrite,
            maxParallelism,
            summaryPath);
    }

    private ImageConversionResult ConvertInternal(
        string input,
        string output,
        Func<string, string> outputPathFactory,
        Action<(string input, string output)> processImage,
        bool includeSubdirectories,
        bool overwrite,
        int? maxParallelism,
        string? summaryPath)
    {
        input = string.IsNullOrEmpty(input)
            ? Util.GetInputFolderDefault()
            : input;
        output = string.IsNullOrEmpty(output)
            ? Util.GetOutputFolderDefault()
            : output;

        if (!Directory.Exists(input))
            throw new Exception("Input directory does not exist.");

        if (!Directory.Exists(output))
            Directory.CreateDirectory(output);

        var imageFiles = Util.GetSupportedFormatImageFiles(input, includeSubdirectories);
        if (imageFiles.Count == 0)
            throw new Exception("No files found in input directory.");

        logger.LogInformation("{Count} files found in {Input}", imageFiles.Count, input);

        var result = new ImageConversionResult { TotalFiles = imageFiles.Count };
        var sw = Stopwatch.StartNew();

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = maxParallelism ?? Environment.ProcessorCount
        };

        var success = 0;
        var failed = 0;

        Parallel.ForEach(imageFiles, options, imageFile =>
        {
            try
            {
                var outFile = outputPathFactory(imageFile);
                var outFolder = Path.GetDirectoryName(outFile);
                if (!string.IsNullOrEmpty(outFolder) && !Directory.Exists(outFolder))
                    Directory.CreateDirectory(outFolder);

                processImage((imageFile, outFile));
                logger.LogDebug("Processed {File}", Util.MakeRelativePath(imageFile, input));
                Interlocked.Increment(ref success);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref failed);
                lock (result.Errors)
                {
                    result.Errors.Add(new ImageProcessError
                    {
                        File = Util.MakeRelativePath(imageFile, input),
                        Message = ex.Message
                    });
                }

                logger.LogError(ex, "Error processing {File}", Util.MakeRelativePath(imageFile, input));
            }
        });

        sw.Stop();
        result.ElapsedSeconds = sw.Elapsed.TotalSeconds;
        result.Succeeded = success;
        result.Failed = failed;

        logger.LogInformation("Completed: {Summary}", result);

        if (!string.IsNullOrEmpty(summaryPath))
            WriteSummaryFile(summaryPath, result);

        return result;
    }

    private static void WriteSummaryFile(string path, ImageConversionResult result)
    {
        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(path, json);
    }

    private static void ConvertSingle(
        string inputFile,
        string outputFile,
        MagickFormat outputFormat,
        bool overwrite,
        int? newWidth,
        int? newHeight)
    {
        if (File.Exists(outputFile))
        {
            if (!overwrite)
                throw new IOException("Output file exists.");
            File.Delete(outputFile);
        }

        using var image = new MagickImage(inputFile);
        if (newWidth.HasValue && newHeight.HasValue)
            image.Resize(newWidth.Value, newHeight.Value);
        image.Write(outputFile, outputFormat);
    }

    private static void ResizeSingle(
        string inputFile,
        string outputFile,
        int width,
        int height,
        bool overwrite)
    {
        if (File.Exists(outputFile))
        {
            if (!overwrite)
                throw new IOException("Output file exists.");
            File.Delete(outputFile);
        }

        using var image = new MagickImage(inputFile);
        image.Resize(width, height);
        image.Write(outputFile);
    }

    public IEnumerable<string> GetSupportedFormats() =>
        Enum.GetNames<MagickFormat>().Select(x => x.ToLower(new CultureInfo("en-US")));
}