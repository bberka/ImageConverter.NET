using Pfim;

namespace ImageConverter.NET.Lib.Data;

public class DdsImage //Forked from https://github.com/andburn/dds-reader
{
  private readonly IImage _image;

  public DdsImage(string file) {
    _image = Pfimage.FromFile(file);
    Process();
  }

  public DdsImage(Stream stream) {
    if (stream == null)
      throw new Exception("DDSImage ctor: Stream is null");

    _image = Dds.Create(stream, new PfimConfig());
    Process();
  }

  public DdsImage(byte[] data) {
    if (data == null || data.Length <= 0)
      throw new Exception("DDSImage ctor: no data");

    _image = Dds.Create(data, new PfimConfig());
    Process();
  }

  public byte[] Data {
    get {
      if (_image != null)
        return _image.Data;
      return new byte[0];
    }
  }

  public void Save(string file) {
    if (_image.Format == ImageFormat.Rgba32)
      Save<Bgra32>(file);
    else if (_image.Format == ImageFormat.Rgb24)
      Save<Bgr24>(file);
    else
      throw new Exception("Unsupported pixel format (" + _image.Format + ")");
  }

  private void Process() {
    if (_image == null)
      throw new Exception("DDSImage image creation failed");

    if (_image.Compressed)
      _image.Decompress();
  }

  private void Save<T>(string file) where T : unmanaged, IPixel<T> {
    var image = Image.LoadPixelData<T>(_image.Data, _image.Width, _image.Height);
    image.Save(file);
  }
}