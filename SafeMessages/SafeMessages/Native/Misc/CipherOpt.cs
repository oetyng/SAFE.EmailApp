using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Native.App;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.Misc {
  internal static class CipherOpt {
    private static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();

    public static Task CipherOptFreeAsync(ulong cipherOptHandle) {
      var tcs = new TaskCompletionSource<object>();
      CipherOptFreeCb callback = null;
      callback = (self, result) => {
        if (result.ErrorCode != 0) {
          CallbackManager.Unregister(callback);
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.CipherOptFree(Session.AppPtr, cipherOptHandle, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<ulong> CipherOptNewPlaintextAsync() {
      var tcs = new TaskCompletionSource<ulong>();
      CipherOptNewPlaintextCb callback = null;
      callback = (self, result, cipherOptHandle) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(cipherOptHandle);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.CipherOptNewPlaintext(Session.AppPtr, Session.UserData, callback);

      return tcs.Task;
    }
  }
}
