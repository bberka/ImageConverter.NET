namespace ImageConverter.NET.Lib;

public class ImageFile
{
  public ImageFile(string filePath) {
    FilePath = filePath;
    Extension = Path.GetExtension(filePath).Replace(".", "");
    FileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
  }

  public string FilePath { get; }

  public string Extension { get; }


  public string FileNameWithoutExtension { get; }

  public string GetOutputFilePath(string inputFolder, string outputFolder, SupportedFormat outputFormat) {
    var outputFilePath = FilePath.Replace("." + Extension, "." + outputFormat.FormatString).Replace(inputFolder, outputFolder);
    return outputFilePath;
  }

  public string GetRelativePath(string inputFolder) {
    var path = FilePath.Replace(inputFolder, "");
    //remove last / 
    if (path.StartsWith("\\") || path.StartsWith("/")) path = path[1..];
    return path;
  }
}