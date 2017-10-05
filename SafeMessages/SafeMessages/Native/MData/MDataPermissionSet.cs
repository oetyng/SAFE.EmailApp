using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Native.App;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.MData {
  internal static class MDataPermissionSet {
    private static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();

    public static Task AllowAsync(NativeHandle permissionSetH, MDataAction allowAction) {
      var tcs = new TaskCompletionSource<object>();
      MDataPermissionSetAllowCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      NativeBindings.MDataPermissionSetAllow(Session.AppPtr, permissionSetH, allowAction, callback);

      return tcs.Task;
    }

    public static Task FreeAsync(ulong permissionSetH) {
      var tcs = new TaskCompletionSource<object>();
      MDataPermissionSetFreeCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      NativeBindings.MDataPermissionSetFree(Session.AppPtr, permissionSetH, callback);

      return tcs.Task;
    }

    public static Task<NativeHandle> NewAsync() {
      var tcs = new TaskCompletionSource<NativeHandle>();

      MDataPermissionSetNewCb callback = (_, result, permissionSetH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(permissionSetH, FreeAsync));
      };

      NativeBindings.MDataPermissionSetNew(Session.AppPtr, callback);

      return tcs.Task;
    }
  }
}
