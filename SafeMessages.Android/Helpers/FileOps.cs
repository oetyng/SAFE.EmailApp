using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using SafeMessages.Droid.Helpers;
using SafeMessages.Helpers;

[assembly: Dependency(typeof(FileOps))]

namespace SafeMessages.Droid.Helpers
{
  public class FileOps : IFileOps {
    public string ConfigFilesPath {
      get {
        string path;
        // Personal -> /data/data/@PACKAGE_NAME@/files
        path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        return path;
      }
    }

    public async Task TransferAssetsAsync(List<(string, string)> fileList) {
      foreach (var tuple in fileList) {
        using (var reader = new StreamReader(Android.App.Application.Context.Assets.Open(tuple.Item1))) {
          using (var writer = new StreamWriter(Path.Combine(ConfigFilesPath, tuple.Item2))) {
            await writer.WriteAsync(await reader.ReadToEndAsync());
            writer.Close();
          }
          reader.Close();
        }
      }
    }
  }
}
