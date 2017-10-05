using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Native.App;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.MData {
  internal static class MDataPermissions {
    private static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();

    public static Task FreeAsync(ulong permissionsH) {
      var tcs = new TaskCompletionSource<object>();
      MDataPermissionsFreeCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      NativeBindings.MDataPermissionsFree(Session.AppPtr, permissionsH, callback);

      return tcs.Task;
    }

    public static Task InsertAsync(NativeHandle permissionsH, NativeHandle forUserH, NativeHandle permissionSetH) {
      var tcs = new TaskCompletionSource<object>();

      MDataPermissionsInsertCb callback = (_, result) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(null);
      };

      NativeBindings.MDataPermissionsInsert(Session.AppPtr, permissionsH, forUserH, permissionSetH, callback);

      return tcs.Task;
    }

    public static Task<NativeHandle> NewAsync() {
      var tcs = new TaskCompletionSource<NativeHandle>();

      MDataPermissionsNewCb callback = (_, result, permissionsH) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(new NativeHandle(permissionsH, FreeAsync));
      };

      NativeBindings.MDataPermissionsNew(Session.AppPtr, callback);

      return tcs.Task;
    }
  }
}
