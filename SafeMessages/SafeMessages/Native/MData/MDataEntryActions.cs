using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Native.App;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.MData {
  internal static class MDataEntryActions {
    private static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();

    public static Task FreeAsync(ulong entryActionsH) {
      var tcs = new TaskCompletionSource<object>();
      MDataEntryActionsFreeCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      NativeBindings.MDataEntryActionsFree(Session.AppPtr, entryActionsH, callback);

      return tcs.Task;
    }

    public static Task InsertAsync(NativeHandle entryActionsH, List<byte> entKey, List<byte> entVal) {
      var tcs = new TaskCompletionSource<object>();

      MDataEntryActionsInsertCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      var entKeyPtr = entKey.ToIntPtr();
      var entValPtr = entVal.ToIntPtr();

      NativeBindings.MDataEntryActionsInsert(
        Session.AppPtr,
        entryActionsH,
        entKeyPtr,
        (IntPtr)entKey.Count,
        entValPtr,
        (IntPtr)entVal.Count,
        callback);

      Marshal.FreeHGlobal(entKeyPtr);
      Marshal.FreeHGlobal(entValPtr);

      return tcs.Task;
    }

    public static Task<NativeHandle> NewAsync() {
      var tcs = new TaskCompletionSource<NativeHandle>();

      MDataEntryActionsNewCb callback = (_, result, entryActionsH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(entryActionsH, FreeAsync));
      };

      NativeBindings.MDataEntryActionsNew(Session.AppPtr, callback);

      return tcs.Task;
    }
  }
}
