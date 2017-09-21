using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Native.App;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.MData {
  internal static class MDataPermissions {
    private static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();

    public static Task MDataPermissionSetAllowAsync(ulong permissionSetH, MDataAction allowAction) {
      var tcs = new TaskCompletionSource<object>();
      MDataPermissionSetAllowCb callback = null;
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
      NativeBindings.MDataPermissionSetAllow(Session.AppPtr, permissionSetH, allowAction, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task MDataPermissionSetFreeAsync(ulong permissionSetH) {
      var tcs = new TaskCompletionSource<object>();
      MDataPermissionSetFreeCb callback = null;
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
      NativeBindings.MDataPermissionSetFree(Session.AppPtr, permissionSetH, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<ulong> MDataPermissionSetNewAsync() {
      var tcs = new TaskCompletionSource<ulong>();

      MDataPermissionSetNewCb callback = null;
      callback = (pVoid, result, permissionSetH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(permissionSetH);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.MDataPermissionSetNew(Session.AppPtr, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task MDataPermissionsFreeAsync(ulong permissionsH) {
      var tcs = new TaskCompletionSource<object>();
      MDataPermissionsFreeCb callback = null;
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
      NativeBindings.MDataPermissionsFree(Session.AppPtr, permissionsH, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task MDataPermissionsInsertAsync(ulong permissionsH, ulong forUserH, ulong permissionSetH) {
      var tcs = new TaskCompletionSource<object>();

      MDataPermissionsInsertCb callback = null;
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
      NativeBindings.MDataPermissionsInsert(Session.AppPtr, permissionsH, forUserH, permissionSetH, Session.UserData, callback);

      return tcs.Task;
    }

    public static Task<ulong> MDataPermissionsNewAsync() {
      var tcs = new TaskCompletionSource<ulong>();

      MDataPermissionsNewCb callback = null;
      callback = (pVoid, result, permissionsH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(permissionsH);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.MDataPermissionsNew(Session.AppPtr, Session.UserData, callback);

      return tcs.Task;
    }
  }
}
