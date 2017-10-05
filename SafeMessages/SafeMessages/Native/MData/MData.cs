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

    public static Task<(List<byte>, ulong)> GetValueAsync(NativeHandle infoHandle, List<byte> key) {
      var tcs = new TaskCompletionSource<(List<byte>, ulong)>();
      var keyPtr = key.ToIntPtr();
      MDataGetValueCb callback = (_, result, dataPtr, dataLen, entryVersion) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        var data = dataPtr.ToList<byte>(dataLen);
        tcs.SetResult((data, entryVersion));
      };

      NativeBindings.MDataGetValue(Session.AppPtr, infoHandle, keyPtr, (IntPtr)key.Count, callback);
      Marshal.FreeHGlobal(keyPtr);

      return tcs.Task;
    }

    public static Task<NativeHandle> ListEntriesAsync(NativeHandle infoHandle) {
      var tcs = new TaskCompletionSource<NativeHandle>();
      MDataListEntriesCb callback = (_, result, mDataEntriesHandle) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(mDataEntriesHandle, MDataEntries.FreeAsync));
      };

      NativeBindings.MDataListEntries(Session.AppPtr, infoHandle, callback);

      return tcs.Task;
    }

    public static Task<NativeHandle> ListKeysAsync(NativeHandle mDataInfoH) {
      var tcs = new TaskCompletionSource<NativeHandle>();
      MDataListKeysCb callback = (_, result, mDataEntKeysH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(mDataEntKeysH, MDataKeys.FreeAsync));
      };

      NativeBindings.MDataListKeys(Session.AppPtr, mDataInfoH, callback);

      return tcs.Task;
    }

    public static Task MutateEntriesAsync(NativeHandle mDataInfoH, NativeHandle entryActionsH) {
      var tcs = new TaskCompletionSource<object>();
      MDataMutateEntriesCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      NativeBindings.MDataMutateEntries(Session.AppPtr, mDataInfoH, entryActionsH, callback);

      return tcs.Task;
    }

    public static Task PutAsync(NativeHandle mDataInfoH, NativeHandle permissionsH, NativeHandle entriesH) {
      var tcs = new TaskCompletionSource<object>();
      MDataPutCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      NativeBindings.MDataPut(Session.AppPtr, mDataInfoH, permissionsH, entriesH, callback);

      return tcs.Task;
    }
  }
}
