using ImageMagick;

namespace ImageConverter.NET.Lib;

public readonly struct SupportedFormat
{
  public SupportedFormat(int id, MagickFormat format) {
    Id = id;
    Format = format;
  }

  public string FormatString => Format.ToString().ToLower();
  public MagickFormat Format { get; }
  public int Id { get; }
}