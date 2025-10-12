using System.Text.Json;
using ImageConverter.NET.Lib;
using ImageMagick;
using Microsoft.Extensions.Logging.Abstractions;

namespace ImageConverter.NET.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
public class ImageConverterNetTests
{
    private IImageConverterNet _converter = null!;
    private string _inputDir = null!;
    private string _outputDir = null!;

    [SetUp]
    public void SetUp()
    {
        _converter = new ImageConverterNet(new NullLogger<ImageConverterNet>());

        _inputDir = Path.Combine(Path.GetTempPath(), $"imgcvt_in_{Guid.NewGuid()}");
        _outputDir = Path.Combine(Path.GetTempPath(), $"imgcvt_out_{Guid.NewGuid()}");

        Directory.CreateDirectory(_inputDir);
        Directory.CreateDirectory(_outputDir);

        // Creates a few sample images
        CreateSampleImage(Path.Combine(_inputDir, "red.jpg"), MagickColors.Red);
        CreateSampleImage(Path.Combine(_inputDir, "blue.png"), MagickColors.Blue);
    }

    [TearDown]
    public void CleanUp()
    {
        if (Directory.Exists(_inputDir))
            Directory.Delete(_inputDir, true);
        if (Directory.Exists(_outputDir))
            Directory.Delete(_outputDir, true);
    }

    // --- Helpers ---
    private static void CreateSampleImage(string path, MagickColor color)
    {
        using var img = new MagickImage(color, 64, 64);
        img.Write(path);
    }

    private static T ReadJson<T>(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json)!;
    }

    // --- TESTS ---

    [Test]
    public void Convert_SingleFile_CreatesOutput()
    {
        var input = Path.Combine(_inputDir, "red.jpg");
        var output = Path.Combine(_outputDir, "red_converted.png");

        _converter.Convert(input, output, "png", overwrite: true, newWidth: 32, newHeight: 32);

        Assert.That(File.Exists(output), Is.True, "Output image should exist.");
        using var img = new MagickImage(output);
        Assert.That(img.Width, Is.EqualTo(32));
        Assert.That(img.Height, Is.EqualTo(32));
    }

    [Test]
    public void Resize_SingleFile_CreatesResizedOutput()
    {
        var input = Path.Combine(_inputDir, "blue.png");
        var output = Path.Combine(_outputDir, "blue_resized.png");

        _converter.Resize(input, output, 40, 40, overwrite: true);

        Assert.That(File.Exists(output), Is.True);
        using var img = new MagickImage(output);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(img.Width, Is.EqualTo(40));
            Assert.That(img.Height, Is.EqualTo(40));
        }
    }

    [Test]
    public void ConvertFromDirectory_AllFilesProcessed_AndSummaryCreated()
    {
        var summaryPath = Path.Combine(_outputDir, "convert_summary.json");

        var result = _converter.ConvertFromDirectory(
            _inputDir, _outputDir, "bmp",
            includeSubdirectories: false,
            overwrite: true,
            newWidth: 16,
            newHeight: 16,
            maxParallelism: 2,
            summaryPath: summaryPath
        );

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.TotalFiles, Is.EqualTo(2));
            Assert.That(result.Succeeded, Is.EqualTo(2));
            Assert.That(result.Failed, Is.Zero);
            Assert.That(File.Exists(summaryPath), Is.True);
        }

        var summary = ReadJson<ImageConversionResult>(summaryPath);
        Assert.That(summary.Succeeded, Is.EqualTo(2));

        var bmpFiles = Directory.GetFiles(_outputDir, "*.bmp", SearchOption.AllDirectories);
        Assert.That(bmpFiles.Length, Is.EqualTo(2));
    }

    [Test]
    public void ResizeFromDirectory_AllFilesProcessed_AndSummaryCreated()
    {
        var summaryPath = Path.Combine(_outputDir, "resize_summary.json");

        var result = _converter.ResizeFromDirectory(
            _inputDir, _outputDir, 20, 20,
            includeSubdirectories: false,
            overwrite: true,
            maxParallelism: 1,
            summaryPath: summaryPath
        );

        // Validation
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.TotalFiles, Is.EqualTo(2));
            Assert.That(result.Succeeded, Is.GreaterThanOrEqualTo(1), "At least one should succeed.");
            Assert.That(File.Exists(summaryPath), Is.True);
        }

        var summary = ReadJson<ImageConversionResult>(summaryPath);
        Assert.That(summary.Succeeded, Is.Not.Zero);

        var outputs = Directory.GetFiles(_outputDir, "*.*", SearchOption.AllDirectories);
        Assert.That(outputs, Is.Not.Empty);
    }

    [Test]
    public void ConvertFromDirectory_ShouldRecordFailed_WhenOutputFileExists_AndOverwriteFalse()
    {
        // Arrange
        var destFile = Path.Combine(_outputDir, "red.png");
        CreateSampleImage(destFile, MagickColors.Green); // pre-existing file

        var summaryPath = Path.Combine(_outputDir, "fail_summary.json");

        var result = _converter.ConvertFromDirectory(
            _inputDir,
            _outputDir,
            "png",
            includeSubdirectories: false,
            overwrite: false,
            summaryPath: summaryPath
        );

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.TotalFiles, Is.EqualTo(2));
            Assert.That(result.Failed, Is.GreaterThan(0));
            Assert.That(result.Errors.Count, Is.GreaterThan(0));
        }
    }

    [Test]
    public void ResizeFromDirectory_ShouldHandleInvalidInputDirectory()
    {
        var invalidDir = Path.Combine(Path.GetTempPath(), "missing_dir_" + Guid.NewGuid());
        Assert.That(() => { _converter.ResizeFromDirectory(invalidDir, _outputDir, 30, 30); }, Throws.TypeOf<DirectoryNotFoundException>());
    }
}