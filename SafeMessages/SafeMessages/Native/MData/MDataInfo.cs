using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Native.App;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.MData {
  internal static class MDataInfo {
    private static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();

    public static Task<List<byte>> MDataInfoDecryptAsync(ulong mDataInfoH, List<byte> cipherText) {
      var tcs = new TaskCompletionSource<List<byte>>();
      var cipherPtr = cipherText.ToIntPtr();
      var cipherLen = (IntPtr)cipherText.Count;

      MDataInfoDecryptCb callback = null;
      callback = (pVoid, result, plainText, len) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        var byteList = plainText.ToList<byte>(len);
        tcs.SetResult(byteList);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.MDataInfoDecrypt(Session.AppPtr, mDataInfoH, cipherPtr, cipherLen, Session.UserData, callback);
      Marshal.FreeHGlobal(cipherPtr);

      return tcs.Task;
    }

    public static Task<ulong> MDataInfoDeserialiseAsync(List<byte> serialisedData) {
      var tcs = new TaskCompletionSource<ulong>();
      MDataInfoDeserialiseCb callback = null;
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

      var serialisedDataPtr = serialisedData.ToIntPtr();

      NativeBindings.MDataInfoDeserialise(Session.AppPtr, serialisedDataPtr, (IntPtr)serialisedData.Count, Session.UserData, callback);

      Marshal.FreeHGlobal(serialisedDataPtr);

      return tcs.Task;
    }

    public static Task<List<byte>> MDataInfoEncryptEntryKeyAsync(ulong infoH, List<byte> inputBytes) {
      var tcs = new TaskCompletionSource<List<byte>>();
      var inputBytesPtr = inputBytes.ToIntPtr();
      MDataInfoEncryptEntryKeyCb callback = null;
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
      NativeBindings.MDataInfoEncryptEntryKey(Session.AppPtr, infoH, inputBytesPtr, (IntPtr)inputBytes.Count, Session.UserData, callback);
      Marshal.FreeHGlobal(inputBytesPtr);

      return tcs.Task;
    }

    public static Task<List<byte>> MDataInfoEncryptEntryValueAsync(ulong infoH, List<byte> inputBytes) {
      var tcs = new TaskCompletionSource<List<byte>>();
      var inputBytesPtr = inputBytes.ToIntPtr();
      MDataInfoEncryptEntryValueCb callback = null;
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
      NativeBindings.MDataInfoEncryptEntryValue(Session.AppPtr, infoH, inputBytesPtr, (IntPtr)inputBytes.Count, Session.UserData, callback);
      Marshal.FreeHGlobal(inputBytesPtr);

      return tcs.Task;
    }

    public static Task MDataInfoFreeAsync(ulong mDataInfoH) {
      var tcs = new TaskCompletionSource<object>();

      MDataInfoFreeCb callback = null;
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
      NativeBindings.MDataInfoFree(Session.AppPtr, mDataInfoH, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<ulong> MDataInfoNewPublicAsync(List<byte> xorName, ulong typeTag) {
      var tcs = new TaskCompletionSource<ulong>();

      MDataInfoNewPublicCb callback = null;
      callback = (pVoid, result, pubMDataInfoH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(pubMDataInfoH);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);

      var xorNamePtr = xorName.ToIntPtr();

      NativeBindings.MDataInfoNewPublic(Session.AppPtr, xorNamePtr, typeTag, Session.UserData, callback);

      Marshal.FreeHGlobal(xorNamePtr);

      return tcs.Task;
    }

    public static Task<ulong> MDataInfoRandomPrivateAsync(ulong typeTag) {
      var tcs = new TaskCompletionSource<ulong>();

      MDataInfoRandomPrivateCb callback = null;
      callback = (pVoid, result, privateMDataInfoH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(privateMDataInfoH);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.MDataInfoRandomPrivate(Session.AppPtr, typeTag, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<ulong> MDataInfoRandomPublicAsync(ulong typeTag) {
      var tcs = new TaskCompletionSource<ulong>();

      MDataInfoRandomPublicCb callback = null;
      callback = (pVoid, result, pubMDataInfoH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(pubMDataInfoH);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.MDataInfoRandomPublic(Session.AppPtr, typeTag, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<List<byte>> MDataInfoSerialiseAsync(ulong mdataInfoH) {
      var tcs = new TaskCompletionSource<List<byte>>();
      MDataInfoSerialiseCb callback = null;
      callback = (pVoid, result, bytesPtr, len) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(bytesPtr.ToList<byte>(len));
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.MDataInfoSerialise(Session.AppPtr, mdataInfoH, Session.UserData, callback);

      return tcs.Task;
    }
  }
}
