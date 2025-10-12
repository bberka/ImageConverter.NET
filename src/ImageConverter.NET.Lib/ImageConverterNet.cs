using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using ImageMagick;
using Microsoft.Extensions.Logging;

namespace ImageConverter.NET.Lib;

public class ImageConverterNet(ILogger<ImageConverterNet> logger) : IImageConverterNet
{
    // === Single-file convert ===
    public void Convert(
        string inputFilePath,
        string outputFilePath,
        string outputFormat,
        bool overwrite = false,
        uint? newWidth = null,
        uint? newHeight = null)
    {
        if (!File.Exists(inputFilePath))
            throw new FileNotFoundException("Input file does not exist", inputFilePath);

        var format = Util.GetFormatEnum(outputFormat);
        DoConvert(inputFilePath, outputFilePath, format, overwrite, newWidth, newHeight);

        logger.LogInformation(
            "Converted {File} to format {Format}",
            Path.GetFileName(inputFilePath),
            outputFormat);
    }

    // === Single-file resize ===
    public void Resize(string inputFilePath, string outputFilePath,
        uint width, uint height, bool overwrite = false)
    {
        if (!File.Exists(inputFilePath))
            throw new FileNotFoundException("Input file does not exist", inputFilePath);

        DoResize(inputFilePath, outputFilePath, width, height, overwrite);
        logger.LogInformation(
            "Resized {File} to {Width}x{Height}",
            Path.GetFileName(inputFilePath), width, height);
    }

    // === Directory convert ===
    public ImageConversionResult ConvertFromDirectory(
        string input, string output, string outFormat,
        bool includeSubdirectories = true, bool overwrite = false,
        uint? newWidth = null, uint? newHeight = null,
        int? maxParallelism = null, string? summaryPath = null)
    {
        var format = Util.GetFormatEnum(outFormat);

        return ProcessDirectory(
            input, output,
            i => Util.MakeOutputFilePath(i, input, output, format),
            f => DoConvert(f.input, f.output, format, overwrite, newWidth, newHeight),
            includeSubdirectories, maxParallelism, summaryPath);
    }

    // === Directory resize ===
    public ImageConversionResult ResizeFromDirectory(
        string input, string output,
        uint width, uint height,
        bool includeSubdirectories = true, bool overwrite = false,
        int? maxParallelism = null, string? summaryPath = null)
    {
        return ProcessDirectory(
            input, output,
            i => Util.MakeOutputFilePath(i, input, output, Util.GetFileFormat(i)),
            f => DoResize(f.input, f.output, width, height, overwrite),
            includeSubdirectories, maxParallelism, summaryPath);
    }

    // === Internal folder processor (shared) ===
    private ImageConversionResult ProcessDirectory(
        string input, string output,
        Func<string, string> outPathFactory,
        Action<(string input, string output)> action,
        bool includeSubdirectories,
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
            throw new DirectoryNotFoundException("Input directory does not exist.");
        if (!Directory.Exists(output))
            Directory.CreateDirectory(output);

        var files = Util.GetSupportedFormatImageFiles(input, includeSubdirectories);
        if (files.Count == 0)
            throw new Exception("No supported files in input directory.");

        logger.LogInformation("Processing {Count} files from {Input}", files.Count, input);

        var result = new ImageConversionResult { TotalFiles = files.Count };
        var sw = Stopwatch.StartNew();

        var success = 0;
        var failed = 0;

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = maxParallelism ?? Environment.ProcessorCount
        };

        Parallel.ForEach(files, options, file =>
        {
            try
            {
                var outputPath = outPathFactory(file);
                var dir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                action((file, outputPath));
                Interlocked.Increment(ref success);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref failed);

                lock (result.Errors)
                {
                    result.Errors.Add(new ImageProcessError
                    {
                        File = Util.MakeRelativePath(file, input),
                        Message = ex.Message
                    });
                }

                logger.LogError(ex, "Error processing {File}", file);
            }
        });

        sw.Stop();
        result.ElapsedSeconds = sw.Elapsed.TotalSeconds;
        result.Succeeded = success;
        result.Failed = failed;

        logger.LogInformation("Completed: {Succeeded} succeeded, {Failed} failed in {Elapsed:F2}s",
            success, failed, result.ElapsedSeconds);

        if (!string.IsNullOrEmpty(summaryPath))
            SaveSummary(summaryPath, result);

        return result;
    }

    // === Core single-file conversion ===
    private static void DoConvert(
        string inputFile, string outputFile,
        MagickFormat format, bool overwrite,
        uint? newWidth, uint? newHeight)
    {
        if (File.Exists(outputFile))
        {
            if (!overwrite)
                throw new IOException($"Output already exists: {outputFile}");
            File.Delete(outputFile);
        }

        using var image = new MagickImage(inputFile);

        if (newWidth.HasValue && newHeight.HasValue)
            image.Resize(newWidth.Value, newHeight.Value);

        image.Write(outputFile, format);
    }

    // === Core single-file resizing ===
    private static void DoResize(
        string inputFile, string outputFile,
        uint width, uint height, bool overwrite)
    {
        if (File.Exists(outputFile))
        {
            if (!overwrite)
                throw new IOException($"Output already exists: {outputFile}");
            File.Delete(outputFile);
        }

        using var image = new MagickImage(inputFile);
        image.Resize(width, height);
        image.Write(outputFile);
    }

    // === Summary JSON ===
    private static void SaveSummary(string path, ImageConversionResult result)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(path, json);
    }

    // === Supported formats ===
    public IEnumerable<string> GetSupportedFormats() =>
        Enum.GetNames<MagickFormat>()
            .Select(x => x.ToLower(CultureInfo.InvariantCulture));
}