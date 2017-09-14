using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Native.App;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.MData {
  internal static class MData {
    private static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();

    public static Task<Tuple<List<byte>, ulong>> MDataGetValueAsync(ulong infoHandle, List<byte> key) {
      var tcs = new TaskCompletionSource<Tuple<List<byte>, ulong>>();
      var keyPtr = key.ToIntPtr();
      MDataGetValueCb callback = null;
      callback = (self, result, dataPtr, dataLen, entryVersion) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        var data = dataPtr.ToList<byte>(dataLen);
        tcs.SetResult(Tuple.Create(data, entryVersion));
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.MDataGetValue(Session.AppPtr, infoHandle, keyPtr, (IntPtr)key.Count, Session.UserData, callback);
      Marshal.FreeHGlobal(keyPtr);

      return tcs.Task;
    }

    public static Task<ulong> MDataListEntriesAsync(ulong infoHandle) {
      var tcs = new TaskCompletionSource<ulong>();
      MDataListEntriesCb callback = null;
      callback = (self, result, mDataEntriesHandle) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(mDataEntriesHandle);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.MDataListEntries(Session.AppPtr, infoHandle, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<ulong> MDataListKeysAsync(ulong mDataInfoH) {
      var tcs = new TaskCompletionSource<ulong>();
      MDataListKeysCb callback = null;
      callback = (pVoid, result, mDataEntKeysH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(mDataEntKeysH);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.MDataListKeys(Session.AppPtr, mDataInfoH, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task MDataMutateEntriesAsync(ulong mDataInfoH, ulong entryActionsH) {
      var tcs = new TaskCompletionSource<object>();
      MDataMutateEntriesCb callback = null;
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
      NativeBindings.MDataMutateEntries(Session.AppPtr, mDataInfoH, entryActionsH, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task MDataPutAsync(ulong mDataInfoH, ulong permissionsH, ulong entriesH) {
      var tcs = new TaskCompletionSource<object>();
      MDataPutCb callback = null;
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
      NativeBindings.MDataPut(Session.AppPtr, mDataInfoH, permissionsH, entriesH, Session.UserData, callback);

      return tcs.Task;
    }
  }
}
