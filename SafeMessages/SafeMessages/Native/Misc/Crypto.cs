using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Native.App;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.Misc {
  internal static class Crypto {
    private const int KeyLen = 32;
    private static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();

    public static Task<NativeHandle> AppPubSignKeyAsync() {
      var tcs = new TaskCompletionSource<NativeHandle>();
      AppPubSignKeyCb callback = (_, result, appPubSignKeyH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(appPubSignKeyH, SignKeyFreeAsync));
      };

      NativeBindings.AppPubSignKey(Session.AppPtr, callback);

      return tcs.Task;
    }

    public static Task<List<byte>> DecryptSealedBoxAsync(List<byte> cipherText, NativeHandle pkHandle, NativeHandle skHandle) {
      var tcs = new TaskCompletionSource<List<byte>>();
      var cipherPtr = cipherText.ToIntPtr();
      DecryptSealedBoxCb callback = (_, result, dataPtr, dataLen) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        var data = dataPtr.ToList<byte>(dataLen);

        tcs.SetResult(data);
      };

      NativeBindings.DecryptSealedBox(Session.AppPtr, cipherPtr, (IntPtr)cipherText.Count, pkHandle, skHandle, callback);
      Marshal.FreeHGlobal(cipherPtr);

      return tcs.Task;
    }

    public static Task<(NativeHandle, NativeHandle)> EncGenerateKeyPairAsync() {
      var tcs = new TaskCompletionSource<(NativeHandle, NativeHandle)>();
      EncGenerateKeyPairCb callback = (_, result, encPubKeyH, encSecKeyH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult((new NativeHandle(encPubKeyH, EncPubKeyFreeAsync), new NativeHandle(encSecKeyH, EncSecretKeyFreeAsync)));
      };

      NativeBindings.EncGenerateKeyPair(Session.AppPtr, callback);

      return tcs.Task;
    }

    public static Task EncPubKeyFreeAsync(ulong encPubKeyH) {
      var tcs = new TaskCompletionSource<object>();
      EncPubKeyFreeCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      NativeBindings.EncPubKeyFree(Session.AppPtr, encPubKeyH, callback);

      return tcs.Task;
    }

    public static Task<List<byte>> EncPubKeyGetAsync(NativeHandle encPubKeyH) {
      var tcs = new TaskCompletionSource<List<byte>>();
      EncPubKeyGetCb callback = (_, result, encPubKeyPtr) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(encPubKeyPtr.ToList<byte>((IntPtr)KeyLen));
      };

      NativeBindings.EncPubKeyGet(Session.AppPtr, encPubKeyH, callback);

      return tcs.Task;
    }

    public static Task<NativeHandle> EncPubKeyNewAsync(List<byte> asymPublicKeyBytes) {
      var tcs = new TaskCompletionSource<NativeHandle>();
      var asymPublicKeyPtr = asymPublicKeyBytes.ToIntPtr();
      EncPubKeyNewCb callback = (self, result, encryptPubKeyHandle) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(encryptPubKeyHandle, EncPubKeyFreeAsync));
      };

      NativeBindings.EncPubKeyNew(Session.AppPtr, asymPublicKeyPtr, callback);
      Marshal.FreeHGlobal(asymPublicKeyPtr);

      return tcs.Task;
    }

    public static Task<List<byte>> EncryptSealedBoxAsync(List<byte> inputData, NativeHandle pkHandle) {
      var tcs = new TaskCompletionSource<List<byte>>();
      var inputDataPtr = inputData.ToIntPtr();
      EncryptSealedBoxCb callback = (_, result, dataPtr, dataLen) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }
        var data = dataPtr.ToList<byte>(dataLen);
        tcs.SetResult(data);
      };

      NativeBindings.EncryptSealedBox(Session.AppPtr, inputDataPtr, (IntPtr)inputData.Count, pkHandle, callback);
      Marshal.FreeHGlobal(inputDataPtr);

      return tcs.Task;
    }

    public static Task EncSecretKeyFreeAsync(ulong encSecKeyH) {
      var tcs = new TaskCompletionSource<object>();
      EncSecretKeyFreeCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      NativeBindings.EncSecretKeyFree(Session.AppPtr, encSecKeyH, callback);

      return tcs.Task;
    }

    public static Task<List<byte>> EncSecretKeyGetAsync(NativeHandle encSecKeyH) {
      var tcs = new TaskCompletionSource<List<byte>>();
      EncSecretKeyGetCb callback = (_, result, encSecKeyPtr) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(encSecKeyPtr.ToList<byte>((IntPtr)KeyLen));
      };

      NativeBindings.EncSecretKeyGet(Session.AppPtr, encSecKeyH, callback);

      return tcs.Task;
    }

    public static Task<NativeHandle> EncSecretKeyNewAsync(List<byte> asymSecKeyBytes) {
      var tcs = new TaskCompletionSource<NativeHandle>();
      var asymSecKeyPtr = asymSecKeyBytes.ToIntPtr();
      EncSecretKeyNewCb callback = (_, result, encSecKeyHandle) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(encSecKeyHandle, EncSecretKeyFreeAsync));
      };

      NativeBindings.EncSecretKeyNew(Session.AppPtr, asymSecKeyPtr, callback);

      return tcs.Task;
    }

    public static Task SignKeyFreeAsync(ulong signKeyHandle) {
      var tcs = new TaskCompletionSource<object>();
      SignKeyFreeCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      NativeBindings.SignKeyFree(Session.AppPtr, signKeyHandle, callback);

      return tcs.Task;
    }
  }
}
