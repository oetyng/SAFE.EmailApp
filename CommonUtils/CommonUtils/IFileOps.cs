using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonUtils {
  public interface IFileOps {
    string ConfigFilesPath { get; }
    Task TransferAssetsAsync(List<Tuple<string, string>> fileList);
  }
}
