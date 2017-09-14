using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Native.App;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.Misc {
  internal static class NativeUtils {
    private static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();

    public static Task<List<byte>> Sha3HashAsync(List<byte> source) {
      var tcs = new TaskCompletionSource<List<byte>>();
      var sourcePtr = source.ToIntPtr();
      Sha3HashCb callback = null;
      callback = (pVoid, result, digestPtr, digestLen) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(digestPtr.ToList<byte>(digestLen));
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.Sha3Hash(sourcePtr, (IntPtr)source.Count, Session.UserData, callback);
      Marshal.FreeHGlobal(sourcePtr);

      return tcs.Task;
    }
  }
}
