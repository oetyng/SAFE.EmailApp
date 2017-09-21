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

    public static Task<List<Tuple<List<byte>, List<byte>, ulong>>> MDataEntriesForEachAsync(ulong entH) {
      var tcs = new TaskCompletionSource<List<Tuple<List<byte>, List<byte>, ulong>>>();
      var entries = new List<Tuple<List<byte>, List<byte>, ulong>>();

      MDataEntriesForEachCb forEachCb = null;
      forEachCb = (pVoid, entryKeyPtr, entryKeyLen, entryValPtr, entryValLen, entryVersion) => {
        var entryKey = entryKeyPtr.ToList<byte>(entryKeyLen);
        var entryVal = entryValPtr.ToList<byte>(entryValLen);
        entries.Add(Tuple.Create(entryKey, entryVal, entryVersion));
      };
      CallbackManager.Register(forEachCb);

      MDataEntriesForEachResCb forEachResCb = null;
      forEachResCb = (ptr, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(forEachCb);
          CallbackManager.Unregister(forEachResCb);
          return;
        }

        tcs.SetResult(entries);
        CallbackManager.Unregister(forEachCb);
        CallbackManager.Unregister(forEachResCb);
      };

      CallbackManager.Register(forEachResCb);
      NativeBindings.MDataEntriesForEach(Session.AppPtr, entH, Session.UserData, forEachCb, forEachResCb);

      return tcs.Task;
    }

    public static Task MDataEntriesFreeAsync(ulong entriesH) {
      var tcs = new TaskCompletionSource<object>();
      MDataEntriesFreeCb callback = null;
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
      NativeBindings.MDataEntriesFree(Session.AppPtr, entriesH, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task MDataEntriesInsertAsync(ulong entriesH, List<byte> entKey, List<byte> entVal) {
      var tcs = new TaskCompletionSource<object>();

      MDataEntriesInsertCb callback = null;
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

      var entKeyPtr = entKey.ToIntPtr();
      var entValPtr = entVal.ToIntPtr();

      NativeBindings.MDataEntriesInsert(
        Session.AppPtr,
        entriesH,
        entKeyPtr,
        (IntPtr)entKey.Count,
        entValPtr,
        (IntPtr)entVal.Count,
        Session.UserData,
        callback);

      Marshal.FreeHGlobal(entKeyPtr);
      Marshal.FreeHGlobal(entValPtr);

      return tcs.Task;
    }

    public static Task<ulong> MDataEntriesLenAsync(ulong entriesHandle) {
      var tcs = new TaskCompletionSource<ulong>();
      MDataEntriesLenCb callback = null;
      callback = (self, len) => {
        // TODO: no result?

        tcs.SetResult(len);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.MDataEntriesLen(Session.AppPtr, entriesHandle, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<ulong> MDataEntriesNewAsync() {
      var tcs = new TaskCompletionSource<ulong>();

      MDataEntriesNewCb callback = null;
      callback = (pVoid, result, entriesH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(entriesH);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.MDataEntriesNew(Session.AppPtr, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task MDataEntryActionsFreeAsync(ulong entryActionsH) {
      var tcs = new TaskCompletionSource<object>();
      MDataEntryActionsFreeCb callback = null;
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
      NativeBindings.MDataEntryActionsFree(Session.AppPtr, entryActionsH, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task MDataEntryActionsInsertAsync(ulong entryActionsH, List<byte> entKey, List<byte> entVal) {
      var tcs = new TaskCompletionSource<object>();

      MDataEntryActionsInsertCb callback = null;
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

      var entKeyPtr = entKey.ToIntPtr();
      var entValPtr = entVal.ToIntPtr();

      NativeBindings.MDataEntryActionsInsert(
        Session.AppPtr,
        entryActionsH,
        entKeyPtr,
        (IntPtr)entKey.Count,
        entValPtr,
        (IntPtr)entVal.Count,
        Session.UserData,
        callback);

      Marshal.FreeHGlobal(entKeyPtr);
      Marshal.FreeHGlobal(entValPtr);

      return tcs.Task;
    }

    public static Task<ulong> MDataEntryActionsNewAsync() {
      var tcs = new TaskCompletionSource<ulong>();

      MDataEntryActionsNewCb callback = null;
      callback = (pVoid, result, entryActionsH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(entryActionsH);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.MDataEntryActionsNew(Session.AppPtr, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<List<List<byte>>> MDataKeysForEachAsync(ulong entKeysH) {
      var tcs = new TaskCompletionSource<List<List<byte>>>();
      var keys = new List<List<byte>>();
      MDataKeysForEachCb forEachCb = null;
      forEachCb = (pVoid, bytePtr, len) => {
        var key = bytePtr.ToList<byte>(len);
        keys.Add(key);
      };
      CallbackManager.Register(forEachCb);

      MDataKeysForEachResCb forEachResCb = null;
      forEachResCb = (ptr, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(forEachCb);
          CallbackManager.Unregister(forEachResCb);
          return;
        }

        tcs.SetResult(keys);
        CallbackManager.Unregister(forEachCb);
        CallbackManager.Unregister(forEachResCb);
      };

      CallbackManager.Register(forEachResCb);
      NativeBindings.MDataKeysForEach(Session.AppPtr, entKeysH, Session.UserData, forEachCb, forEachResCb);

      return tcs.Task;
    }

    public static Task MDataKeysFreeAsync(ulong entKeysH) {
      var tcs = new TaskCompletionSource<object>();
      MDataKeysFreeCb callback = null;
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
      NativeBindings.MDataKeysFree(Session.AppPtr, entKeysH, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<IntPtr> MDataKeysLenAsync(ulong mDataInfoH) {
      var tcs = new TaskCompletionSource<IntPtr>();
      MDataKeysLenCb callback = null;
      callback = (pVoid, result, len) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(len);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.MDataKeysLen(Session.AppPtr, mDataInfoH, Session.UserData, callback);

      return tcs.Task;
    }
  }
}
