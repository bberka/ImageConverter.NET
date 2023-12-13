namespace ImageConverter.NET.Lib.Abstract;

public interface IImageConverter
{
  void ConvertImage(string imageFilePath, string outputFilePath);
}