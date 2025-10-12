using Cocona;
using ImageConverter.NET.Lib;
using Microsoft.Extensions.Logging;

namespace ImageConverter.NET;

public class ImageConverterNetCli(
    ILogger<ImageConverterNetCli> logger,
    IImageConverterNet imageConverterNet
)
{
    [Command(
        "convert-image",
        Description = "Converts images in the given input directory to the specified format."
    )]
    public void ConvertImage(
        [Option("input", Description = "Path to the input directory.")]
        string input,
        [Option("output", Description = "Path to the output directory.")]
        string output,
        [Option("convert-to", Description = "Target format (e.g., 'png').")]
        string convertTo,
        [Option("include-subdirectories", Description = "Include subdirectories.")]
        bool includeSubdirectories = true,
        [Option("overwrite", Description = "Overwrite existing files.")]
        bool overwrite = false,
        [Option("new-width", Description = "Target width.")]
        int? newWidth = null,
        [Option("new-height", Description = "Target height.")]
        int? newHeight = null,
        [Option("max-parallelism", Description = "Maximum number of parallel threads.")]
        int? maxParallelism = null,
        [Option("summary", Description = "Path to output summary JSON file.")]
        string? summaryPath = null
    )
    {
        try
        {
            logger.LogInformation(
                "Converting images from {Input} to {Output} as {Format}",
                input,
                output,
                convertTo
            );

            var result = imageConverterNet.ConvertFromDirectory(
                input,
                output,
                convertTo,
                includeSubdirectories,
                overwrite,
                newWidth,
                newHeight,
                maxParallelism,
                summaryPath
            );

            logger.LogInformation("Conversion completed: {Summary}", result.ToString());
            if (summaryPath is not null)
                logger.LogInformation("Summary exported to {Path}", summaryPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error converting images");
        }
    }

    [Command(
        "resize-image",
        Description = "Resizes images in the given input directory to the specified dimensions."
    )]
    public void ResizeImage(
        [Option("input", Description = "Path to the input directory.")]
        string input,
        [Option("output", Description = "Path to the output directory.")]
        string output,
        [Option("width", Description = "Target width.")]
        int width,
        [Option("height", Description = "Target height.")]
        int height,
        [Option("include-subdirectories", Description = "Include subdirectories.")]
        bool includeSubdirectories = true,
        [Option("overwrite", Description = "Overwrite existing files.")]
        bool overwrite = false,
        [Option("max-parallelism", Description = "Maximum number of parallel threads.")]
        int? maxParallelism = null,
        [Option("summary", Description = "Path to output summary JSON file.")]
        string? summaryPath = null
    )
    {
        try
        {
            logger.LogInformation(
                "Resizing images from {Input} to {Output} with size {Width}x{Height}",
                input,
                output,
                width,
                height
            );

            var result = imageConverterNet.ResizeFromDirectory(
                input,
                output,
                width,
                height,
                includeSubdirectories,
                overwrite,
                maxParallelism,
                summaryPath
            );

            logger.LogInformation("Resize completed: {Summary}", result.ToString());
            if (summaryPath is not null)
                logger.LogInformation("Summary exported to {Path}", summaryPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error resizing images");
        }
    }

    [Command("list-formats", Description = "Lists supported image formats.")]
    public void ListFormats()
    {
        try
        {
            var formats = imageConverterNet.GetSupportedFormats();
            logger.LogInformation("Supported formats:");
            foreach (var format in formats)
                logger.LogInformation("- {Format}", format);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing formats");
        }
    }
}