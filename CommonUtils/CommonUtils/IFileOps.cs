using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonUtils {
  public interface IFileOps {
    Task TransferAssetsAsync(List<Tuple<string, string>> fileList);
  }
}
