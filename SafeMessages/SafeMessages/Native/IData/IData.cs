using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Native.App;
using Xamarin.Forms;

// ReSharper disable InconsistentNaming

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.IData {
  internal static class IData {
    private static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();

    public static Task<List<byte>> IDataCloseSelfEncryptorAsync(ulong seH, ulong cipherOptH) {
      var tcs = new TaskCompletionSource<List<byte>>();
      IDataCloseSelfEncryptorCb callback = null;
      callback = (self, result, xorNamePtr) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        var xorNameList = xorNamePtr.ToList<byte>((IntPtr)32);
        tcs.SetResult(xorNameList);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.IDataCloseSelfEncryptor(Session.AppPtr, seH, cipherOptH, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<ulong> IDataFetchSelfEncryptorAsync(List<byte> xorName) {
      var tcs = new TaskCompletionSource<ulong>();
      var xorNamePtr = xorName.ToIntPtr();
      IDataFetchSelfEncryptorCb callback = null;
      callback = (self, result, sEReaderHandle) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(sEReaderHandle);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.IDataFetchSelfEncryptor(Session.AppPtr, xorNamePtr, Session.UserData, callback);
      Marshal.FreeHGlobal(xorNamePtr);

      return tcs.Task;
    }

    public static Task<ulong> IDataNewSelfEncryptorAsync() {
      var tcs = new TaskCompletionSource<ulong>();
      IDataNewSelfEncryptorCb callback = null;
      callback = (self, result, sEWriterHandle) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(sEWriterHandle);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.IDataNewSelfEncryptor(Session.AppPtr, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<List<byte>> IDataReadFromSelfEncryptorAsync(ulong seHandle, ulong fromPos, ulong len) {
      var tcs = new TaskCompletionSource<List<byte>>();
      IDataReadFromSelfEncryptorCb callback = null;
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
      NativeBindings.IDataReadFromSelfEncryptor(Session.AppPtr, seHandle, fromPos, len, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task IDataSelfEncryptorReaderFreeAsync(ulong sEReaderHandle) {
      var tcs = new TaskCompletionSource<object>();
      IDataSelfEncryptorReaderFreeCb callback = null;
      callback = (self, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(null);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.IDataSelfEncryptorReaderFree(Session.AppPtr, sEReaderHandle, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task IDataSelfEncryptorWriterFreeAsync(ulong sEWriterHandle) {
      var tcs = new TaskCompletionSource<object>();
      IDataSelfEncryptorWriterFreeCb callback = null;
      callback = (self, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(null);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.IDataSelfEncryptorWriterFree(Session.AppPtr, sEWriterHandle, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<ulong> IDataSizeAsync(ulong seHandle) {
      var tcs = new TaskCompletionSource<ulong>();
      IDataSizeCb callback = null;
      callback = (self, result, len) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(len);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.IDataSize(Session.AppPtr, seHandle, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<object> IDataWriteToSelfEncryptorAsync(ulong seHandle, List<byte> data) {
      var tcs = new TaskCompletionSource<object>();
      var dataPtr = data.ToIntPtr();
      IDataWriteToSelfEncryptorCb callback = null;
      callback = (self, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(null);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.IDataWriteToSelfEncryptor(Session.AppPtr, seHandle, dataPtr, (IntPtr)data.Count, Session.UserData, callback);
      Marshal.FreeHGlobal(dataPtr);

      return tcs.Task;
    }
  }
}
