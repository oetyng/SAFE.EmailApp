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

    public static Task<List<byte>> DecryptAsync(NativeHandle mDataInfoH, List<byte> cipherText) {
      var tcs = new TaskCompletionSource<List<byte>>();
      var cipherPtr = cipherText.ToIntPtr();
      var cipherLen = (IntPtr)cipherText.Count;

      MDataInfoDecryptCb callback = (_, result, plainText, len) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        var byteList = plainText.ToList<byte>(len);
        tcs.SetResult(byteList);
      };

      NativeBindings.MDataInfoDecrypt(Session.AppPtr, mDataInfoH, cipherPtr, cipherLen, callback);
      Marshal.FreeHGlobal(cipherPtr);

      return tcs.Task;
    }

    public static Task<NativeHandle> DeserialiseAsync(List<byte> serialisedData) {
      var tcs = new TaskCompletionSource<NativeHandle>();
      MDataInfoDeserialiseCb callback = (_, result, mdataInfoH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(mdataInfoH, FreeAsync));
      };

      var serialisedDataPtr = serialisedData.ToIntPtr();
      NativeBindings.MDataInfoDeserialise(Session.AppPtr, serialisedDataPtr, (IntPtr)serialisedData.Count, callback);

      Marshal.FreeHGlobal(serialisedDataPtr);

      return tcs.Task;
    }

    public static Task<List<byte>> EncryptEntryKeyAsync(NativeHandle infoH, List<byte> inputBytes) {
      var tcs = new TaskCompletionSource<List<byte>>();
      var inputBytesPtr = inputBytes.ToIntPtr();
      MDataInfoEncryptEntryKeyCb callback = (_, result, dataPtr, dataLen) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }
        var data = dataPtr.ToList<byte>(dataLen);
        tcs.SetResult(data);
      };

      NativeBindings.MDataInfoEncryptEntryKey(Session.AppPtr, infoH, inputBytesPtr, (IntPtr)inputBytes.Count, callback);
      Marshal.FreeHGlobal(inputBytesPtr);

      return tcs.Task;
    }

    public static Task<List<byte>> EncryptEntryValueAsync(NativeHandle infoH, List<byte> inputBytes) {
      var tcs = new TaskCompletionSource<List<byte>>();
      var inputBytesPtr = inputBytes.ToIntPtr();
      MDataInfoEncryptEntryValueCb callback = (_, result, dataPtr, dataLen) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }
        var data = dataPtr.ToList<byte>(dataLen);
        tcs.SetResult(data);
      };

      NativeBindings.MDataInfoEncryptEntryValue(Session.AppPtr, infoH, inputBytesPtr, (IntPtr)inputBytes.Count, callback);
      Marshal.FreeHGlobal(inputBytesPtr);

      return tcs.Task;
    }

    public static Task FreeAsync(ulong mDataInfoH) {
      var tcs = new TaskCompletionSource<object>();

      MDataInfoFreeCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      NativeBindings.MDataInfoFree(Session.AppPtr, mDataInfoH, callback);

      return tcs.Task;
    }

    public static Task<NativeHandle> NewPublicAsync(List<byte> xorName, ulong typeTag) {
      var tcs = new TaskCompletionSource<NativeHandle>();

      MDataInfoNewPublicCb callback = (_, result, pubMDataInfoH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(pubMDataInfoH, FreeAsync));
      };

      var xorNamePtr = xorName.ToIntPtr();
      NativeBindings.MDataInfoNewPublic(Session.AppPtr, xorNamePtr, typeTag, callback);
      Marshal.FreeHGlobal(xorNamePtr);

      return tcs.Task;
    }

    public static Task<NativeHandle> RandomPrivateAsync(ulong typeTag) {
      var tcs = new TaskCompletionSource<NativeHandle>();

      MDataInfoRandomPrivateCb callback = (_, result, privateMDataInfoH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(privateMDataInfoH, FreeAsync));
      };

      NativeBindings.MDataInfoRandomPrivate(Session.AppPtr, typeTag, callback);

      return tcs.Task;
    }

    public static Task<NativeHandle> RandomPublicAsync(ulong typeTag) {
      var tcs = new TaskCompletionSource<NativeHandle>();

      MDataInfoRandomPublicCb callback = (_, result, pubMDataInfoH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(pubMDataInfoH, FreeAsync));
      };

      NativeBindings.MDataInfoRandomPublic(Session.AppPtr, typeTag, callback);

      return tcs.Task;
    }

    public static Task<List<byte>> SerialiseAsync(NativeHandle mdataInfoH) {
      var tcs = new TaskCompletionSource<List<byte>>();
      MDataInfoSerialiseCb callback = (_, result, bytesPtr, len) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(bytesPtr.ToList<byte>(len));
      };

      NativeBindings.MDataInfoSerialise(Session.AppPtr, mdataInfoH, callback);

      return tcs.Task;
    }
  }
}
