using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Native.MData;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.App {
  internal static class AccessContainer {
    private static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();

    public static Task<NativeHandle> GetMDataInfoAsync(string containerId) {
      var tcs = new TaskCompletionSource<NativeHandle>();

      AccessContainerGetContainerMDataInfoCb callback = (_, result, mdataInfoH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(mdataInfoH, MDataInfo.FreeAsync));
      };

      NativeBindings.AccessContainerGetContainerMDataInfo(Session.AppPtr, containerId, callback);

      return tcs.Task;
    }
  }
}
