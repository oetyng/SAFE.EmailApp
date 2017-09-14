
#if __MOBILE__
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommonUtils;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileOps))]

namespace CommonUtils {
  public class FileOps : IFileOps {
    public async Task TransferAssetsAsync(List<Tuple<string, string>> fileList) {
      foreach (var tuple in fileList) {
#if __IOS__
        if (tuple.Item1 == "log.toml") {
          continue;
        }
        using (var reader = new StreamReader(Path.Combine(".", tuple.Item1))) {
#elif __ANDROID__
      using (var reader = new StreamReader(Forms.Context.Assets.Open(tuple.Item1))) {
#endif
          using (var writer = new StreamWriter(tuple.Item2)) {
            await writer.WriteAsync(await reader.ReadToEndAsync());
            writer.Close();
          }
          reader.Close();
        }
      }
    }
  }
}
#endif
