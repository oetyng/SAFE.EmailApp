using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Native.App;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

// ReSharper disable InconsistentNaming

namespace SafeMessages.Native.IData {
  internal static class IData {
    private static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();

    public static Task<List<byte>> CloseSelfEncryptorAsync(ulong seH, NativeHandle cipherOptH) {
      var tcs = new TaskCompletionSource<List<byte>>();
      IDataCloseSelfEncryptorCb callback = (_, result, xorNamePtr) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        var xorNameList = xorNamePtr.ToList<byte>((IntPtr)32);
        tcs.SetResult(xorNameList);
      };

      NativeBindings.IDataCloseSelfEncryptor(Session.AppPtr, seH, cipherOptH, callback);

      return tcs.Task;
    }

    public static Task<NativeHandle> FetchSelfEncryptorAsync(List<byte> xorName) {
      var tcs = new TaskCompletionSource<NativeHandle>();
      var xorNamePtr = xorName.ToIntPtr();
      IDataFetchSelfEncryptorCb callback = (_, result, sEReaderHandle) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(sEReaderHandle, SelfEncryptorReaderFreeAsync));
      };

      NativeBindings.IDataFetchSelfEncryptor(Session.AppPtr, xorNamePtr, callback);
      Marshal.FreeHGlobal(xorNamePtr);

      return tcs.Task;
    }

    public static Task<NativeHandle> NewSelfEncryptorAsync() {
      var tcs = new TaskCompletionSource<NativeHandle>();
      IDataNewSelfEncryptorCb callback = (_, result, sEWriterHandle) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(sEWriterHandle, null));
      };

      NativeBindings.IDataNewSelfEncryptor(Session.AppPtr, callback);

      return tcs.Task;
    }

    public static Task<List<byte>> ReadFromSelfEncryptorAsync(NativeHandle seHandle, ulong fromPos, ulong len) {
      var tcs = new TaskCompletionSource<List<byte>>();
      IDataReadFromSelfEncryptorCb callback = (_, result, dataPtr, dataLen) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }
        var data = dataPtr.ToList<byte>(dataLen);
        tcs.SetResult(data);
      };

      NativeBindings.IDataReadFromSelfEncryptor(Session.AppPtr, seHandle, fromPos, len, callback);

      return tcs.Task;
    }

    public static Task SelfEncryptorReaderFreeAsync(ulong sEReaderHandle) {
      var tcs = new TaskCompletionSource<object>();
      IDataSelfEncryptorReaderFreeCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      NativeBindings.IDataSelfEncryptorReaderFree(Session.AppPtr, sEReaderHandle, callback);

      return tcs.Task;
    }

    public static Task SelfEncryptorWriterFreeAsync(ulong sEWriterHandle) {
      var tcs = new TaskCompletionSource<object>();
      IDataSelfEncryptorWriterFreeCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      NativeBindings.IDataSelfEncryptorWriterFree(Session.AppPtr, sEWriterHandle, callback);

      return tcs.Task;
    }

    public static Task<ulong> SizeAsync(NativeHandle seHandle) {
      var tcs = new TaskCompletionSource<ulong>();
      IDataSizeCb callback = (_, result, len) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(len);
      };

      NativeBindings.IDataSize(Session.AppPtr, seHandle, callback);

      return tcs.Task;
    }

    public static Task<object> WriteToSelfEncryptorAsync(NativeHandle seHandle, List<byte> data) {
      var tcs = new TaskCompletionSource<object>();
      var dataPtr = data.ToIntPtr();
      IDataWriteToSelfEncryptorCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      NativeBindings.IDataWriteToSelfEncryptor(Session.AppPtr, seHandle, dataPtr, (IntPtr)data.Count, callback);
      Marshal.FreeHGlobal(dataPtr);

      return tcs.Task;
    }
  }
}
