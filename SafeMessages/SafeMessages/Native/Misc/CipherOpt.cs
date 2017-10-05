using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Native.App;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.Misc {
  internal static class CipherOpt {
    private static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();

    public static Task FreeAsync(ulong cipherOptHandle) {
      var tcs = new TaskCompletionSource<object>();
      CipherOptFreeCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      NativeBindings.CipherOptFree(Session.AppPtr, cipherOptHandle, callback);

      return tcs.Task;
    }

    public static Task<NativeHandle> NewPlaintextAsync() {
      var tcs = new TaskCompletionSource<NativeHandle>();
      CipherOptNewPlaintextCb callback = (_, result, cipherOptHandle) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(cipherOptHandle, FreeAsync));
      };

      NativeBindings.CipherOptNewPlaintext(Session.AppPtr, callback);

      return tcs.Task;
    }
  }
}
