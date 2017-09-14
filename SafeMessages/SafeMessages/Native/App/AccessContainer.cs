using System.Threading.Tasks;
using CommonUtils;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.App {
  internal static class AccessContainer {
    private static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();

    public static Task<ulong> GetContainerMDataInfoAsync(string appId) {
      var tcs = new TaskCompletionSource<ulong>();

      AccessContainerGetContainerMDataInfoCb callback = null;
      callback = (pVoid, result, mdataInfoH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(mdataInfoH);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.AccessContainerGetContainerMDataInfo(Session.AppPtr, appId, Session.UserData, callback);

      return tcs.Task;
    }
  }
}
