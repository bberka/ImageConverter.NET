namespace ImageConverter.NET.Lib;

public class ImageConversionResult
{
    public int TotalFiles { get; init; }
    public int Succeeded { get; set; }
    public int Failed { get; set; }
    public double ElapsedSeconds { get; set; }
    public List<ImageProcessError> Errors { get; } = [];

    public override string ToString() =>
        $"Processed {TotalFiles} files. " +
        $"Succeeded: {Succeeded}, Failed: {Failed}, Duration: {ElapsedSeconds:F2}s";
}