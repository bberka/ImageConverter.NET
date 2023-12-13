using ImageConverter.NET.Lib.Logger;

namespace ImageConverter.NET.Lib;

public static class FileManager
{
  private static string GetInputFolderDefault() {
    var inputFolder = Path.Combine(Directory.GetCurrentDirectory(), "input");
    if (!Directory.Exists(inputFolder))
      throw new Exception("Input folder does not exists");
    return inputFolder;
  }

  private static string GetOutputFolderDefault() {
    var outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "output");
    if (!Directory.Exists(outputFolder))
      Directory.CreateDirectory(outputFolder);
    return outputFolder;
  }

  public static string GetOutputFolderConsoleInput() {
    Console.Write(@"Enter nothing to use default output folder
Enter output folder: ");
    var outputFolder = Console.ReadLine();
    if (string.IsNullOrEmpty(outputFolder)) {
      outputFolder = GetOutputFolderDefault();
    }
    else {
      if (!Directory.Exists(outputFolder))
        throw new Exception("Output folder does not exists");
    }

    return outputFolder;
  }

  public static string GetInputFolderConsoleInput() {
    Console.Write(@"Enter nothing to use default input folder
Enter input folder: ");
    var inputFolder = Console.ReadLine();
    if (string.IsNullOrEmpty(inputFolder)) {
      inputFolder = GetInputFolderDefault();
    }
    else {
      if (!Directory.Exists(inputFolder))
        throw new Exception("Input folder does not exists");
    }

    return inputFolder;
  }

  public static List<string> FilterFiles(string path, string extension) {
    const string start = "*.";
    var search = start + extension;
    var files = Directory.GetFiles(path, search).ToList();
    foreach (var dir in Directory.GetDirectories(path)) {
      var rec = FilterFiles(dir, search);
      files.AddRange(rec);
    }

    return files;
  }
}