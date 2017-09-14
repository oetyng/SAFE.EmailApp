using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Native.App;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.Misc {
  internal static class Crypto {
    private const int KeyLen = 32;
    private static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();

    public static Task<ulong> AppPubSignKeyAsync() {
      var tcs = new TaskCompletionSource<ulong>();
      AppPubSignKeyCb callback = null;
      callback = (pVoid, result, appPubSignKeyH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(appPubSignKeyH);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.AppPubSignKey(Session.AppPtr, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<List<byte>> DecryptSealedBoxAsync(List<byte> cipherText, ulong pkHandle, ulong skHandle) {
      var tcs = new TaskCompletionSource<List<byte>>();
      var cipherPtr = cipherText.ToIntPtr();
      DecryptSealedBoxCb callback = null;
      callback = (self, result, dataPtr, dataLen) => {
        // Marshal.FreeHGlobal(cipherPtr); // TODO: Temp soln
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        var data = dataPtr.ToList<byte>(dataLen);

        tcs.SetResult(data);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.DecryptSealedBox(Session.AppPtr, cipherPtr, (IntPtr)cipherText.Count, pkHandle, skHandle, Session.UserData, callback);
      // Marshal.FreeHGlobal(cipherPtr);

      return tcs.Task;
    }

    public static Task<Tuple<ulong, ulong>> EncGenerateKeyPairAsync() {
      var tcs = new TaskCompletionSource<Tuple<ulong, ulong>>();
      EncGenerateKeyPairCb callback = null;
      callback = (pVoid, result, encPubKeyH, encSecKeyH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(Tuple.Create(encPubKeyH, encSecKeyH));
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.EncGenerateKeyPair(Session.AppPtr, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task EncPubKeyFreeAsync(ulong encPubKeyH) {
      var tcs = new TaskCompletionSource<object>();
      EncPubKeyFreeCb callback = null;
      callback = (pVoid, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(null);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.EncPubKeyFree(Session.AppPtr, encPubKeyH, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<List<byte>> EncPubKeyGetAsync(ulong encPubKeyH) {
      var tcs = new TaskCompletionSource<List<byte>>();
      EncPubKeyGetCb callback = null;
      callback = (pVoid, result, encPubKeyPtr) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(encPubKeyPtr.ToList<byte>((IntPtr)KeyLen));
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.EncPubKeyGet(Session.AppPtr, encPubKeyH, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<ulong> EncPubKeyNewAsync(List<byte> asymPublicKeyBytes) {
      var tcs = new TaskCompletionSource<ulong>();
      var asymPublicKeyPtr = asymPublicKeyBytes.ToIntPtr();
      EncPubKeyNewCb callback = null;
      callback = (self, result, encryptPubKeyHandle) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(encryptPubKeyHandle);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.EncPubKeyNew(Session.AppPtr, asymPublicKeyPtr, Session.UserData, callback);
      // Marshal.FreeHGlobal(asymPublicKeyPtr);

      return tcs.Task;
    }

    public static Task<List<byte>> EncryptSealedBoxAsync(List<byte> inputData, ulong pkHandle) {
      var tcs = new TaskCompletionSource<List<byte>>();
      var inputDataPtr = inputData.ToIntPtr();
      EncryptSealedBoxCb callback = null;
      callback = (self, result, dataPtr, dataLen) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }
        var data = dataPtr.ToList<byte>(dataLen);
        tcs.SetResult(data);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.EncryptSealedBox(Session.AppPtr, inputDataPtr, (IntPtr)inputData.Count, pkHandle, Session.UserData, callback);
      // Marshal.FreeHGlobal(inputDataPtr);

      return tcs.Task;
    }

    public static Task EncSecretKeyFreeAsync(ulong encSecKeyH) {
      var tcs = new TaskCompletionSource<object>();
      EncSecretKeyFreeCb callback = null;
      callback = (pVoid, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(null);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.EncSecretKeyFree(Session.AppPtr, encSecKeyH, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<List<byte>> EncSecretKeyGetAsync(ulong encSecKeyH) {
      var tcs = new TaskCompletionSource<List<byte>>();
      EncSecretKeyGetCb callback = null;
      callback = (pVoid, result, encSecKeyPtr) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(encSecKeyPtr.ToList<byte>((IntPtr)KeyLen));
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.EncSecretKeyGet(Session.AppPtr, encSecKeyH, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<ulong> EncSecretKeyNewAsync(List<byte> asymSecKeyBytes) {
      var tcs = new TaskCompletionSource<ulong>();
      var asymSecKeyPtr = asymSecKeyBytes.ToIntPtr();
      EncSecretKeyNewCb callback = null;
      callback = (self, result, encSecKeyHandle) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(encSecKeyHandle);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.EncSecretKeyNew(Session.AppPtr, asymSecKeyPtr, Session.UserData, callback);

      return tcs.Task;
    }
  }
}
