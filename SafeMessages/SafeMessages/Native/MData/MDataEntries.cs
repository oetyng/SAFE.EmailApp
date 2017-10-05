using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Native.App;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.MData {
  internal static class MDataEntries {
    private static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();

    public static Task<List<(List<byte>, List<byte>, ulong)>> ForEachAsync(NativeHandle entH) {
      var tcs = new TaskCompletionSource<List<(List<byte>, List<byte>, ulong)>>();
      var entries = new List<(List<byte>, List<byte>, ulong)>();

      MDataEntriesForEachCb forEachCb = (_, entryKeyPtr, entryKeyLen, entryValPtr, entryValLen, entryVersion) => {
        var entryKey = entryKeyPtr.ToList<byte>(entryKeyLen);
        var entryVal = entryValPtr.ToList<byte>(entryValLen);
        entries.Add((entryKey, entryVal, entryVersion));
      };

      MDataEntriesForEachResCb forEachResCb = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(entries);
      };

      NativeBindings.MDataEntriesForEach(Session.AppPtr, entH, forEachCb, forEachResCb);

      return tcs.Task;
    }

    public static Task FreeAsync(ulong entriesH) {
      var tcs = new TaskCompletionSource<object>();
      MDataEntriesFreeCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      NativeBindings.MDataEntriesFree(Session.AppPtr, entriesH, callback);

      return tcs.Task;
    }

    public static Task InsertAsync(NativeHandle entriesH, List<byte> entKey, List<byte> entVal) {
      var tcs = new TaskCompletionSource<object>();

      MDataEntriesInsertCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      var entKeyPtr = entKey.ToIntPtr();
      var entValPtr = entVal.ToIntPtr();

      NativeBindings.MDataEntriesInsert(
        Session.AppPtr,
        entriesH,
        entKeyPtr,
        (IntPtr)entKey.Count,
        entValPtr,
        (IntPtr)entVal.Count,
        callback);

      Marshal.FreeHGlobal(entKeyPtr);
      Marshal.FreeHGlobal(entValPtr);

      return tcs.Task;
    }

    public static Task<ulong> LenAsync(NativeHandle entriesHandle) {
      var tcs = new TaskCompletionSource<ulong>();
      MDataEntriesLenCb callback = (_, len) => {
        // TODO: no result?

        tcs.SetResult(len);
      };

      NativeBindings.MDataEntriesLen(Session.AppPtr, entriesHandle, callback);

      return tcs.Task;
    }

    public static Task<NativeHandle> NewAsync() {
      var tcs = new TaskCompletionSource<NativeHandle>();

      MDataEntriesNewCb callback = (_, result, entriesH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(entriesH, FreeAsync));
      };

      NativeBindings.MDataEntriesNew(Session.AppPtr, callback);

      return tcs.Task;
    }
  }
}
