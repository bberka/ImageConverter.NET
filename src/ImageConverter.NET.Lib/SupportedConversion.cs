using ImageMagick;

namespace ImageConverter.NET.Lib;

public class SupportedConversion
{
  public SupportedConversion(uint id, MagickFormat inputFormat, MagickFormat outputFormat, string uiName) {
    InputFormat = inputFormat;
    OutputFormat = outputFormat;
    UIName = uiName;
    Id = id;
  }

  public string InputFormatString => InputFormat.ToString().ToLower();
  public string OutputFormatString => OutputFormat.ToString().ToLower();
  public MagickFormat InputFormat { get; }
  public MagickFormat OutputFormat { get; }
  public string UIName { get; }
  public uint Id { get; }
}