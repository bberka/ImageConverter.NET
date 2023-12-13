using ImageConverter.NET.Lib.Data;
using ImageConverter.NET.Lib.Logger;

namespace ImageConverter.NET.Lib.Manager;

public static class DdsManager
{
  public static void SaveDDS(string ddsfile, string output) {

  }

  public static void SaveAll(string folderpath, string folderoutput) {
    if (!Directory.Exists(folderpath) || !Directory.Exists(folderoutput)) return;
    var srcext = "dds";
    var outext = "png";
    var files = FileManager.FilterFiles(folderpath,srcext);
    Parallel.ForEach(files, file => {
      try {
        var parsed = file.Replace(folderpath, "");
        var filename = Path.GetFileNameWithoutExtension(file);
        if (file.EndsWith(outext)) {
          var parentDir = parsed.Replace(filename + outext, "");
          var dirToCreate = folderoutput + parentDir;
          if (!Directory.Exists(dirToCreate)) Directory.CreateDirectory(dirToCreate);
          var outputPath = dirToCreate + "\\" + filename + srcext;
          File.Copy(file, outputPath, true);
        }
        else if (file.EndsWith(srcext)) {
          var parentDir = parsed.Replace(filename + srcext, "");
          var dirToCreate = folderoutput + parentDir;
          if (!Directory.Exists(dirToCreate)) Directory.CreateDirectory(dirToCreate);
          var outputPath = dirToCreate + "\\" + filename + outext;
          var dds = new DdsImage(file);
          dds.Save(outputPath);
        }
        else {
          ConsoleLogger.Error($"File not supported: {file}");
        }
      }
      catch (Exception ex) {
        ConsoleLogger.Error($"Exception occurred: {ex.Message}");
      }
    });
  }


}