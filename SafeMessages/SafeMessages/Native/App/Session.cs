using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommonUtils;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.App {
  internal static class Session {
    public const ulong UserAnyoneSignPk = 0;
    public static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();
    private static readonly NetObsCallback NetObs;
    public static readonly IntPtr UserData = IntPtr.Zero;

    public static IntPtr AppPtr { private set; get; }

    static Session() {
      AppPtr = IntPtr.Zero;
      NetObs = OnNetworkObserverCb;
    }

    public static Task<bool> AppRegisteredAsync(string appId, AuthGranted authGranted) {
      return Task.Run(
        () => {
          var tcs = new TaskCompletionSource<bool>();
          var authGrantedFfi = new AuthGrantedFfi {
            AccessContainer = authGranted.AccessContainer,
            AppKeys = authGranted.AppKeys,
            BootStrapConfigPtr = authGranted.BootStrapConfig.ToIntPtr(),
            BootStrapConfigLen = (IntPtr)authGranted.BootStrapConfig.Count
          };
          var authGrantedFfiPtr = CommonUtils.Helpers.StructToPtr(authGrantedFfi);

          AppRegisteredCallback callback = null;
          callback = (ptr, result, appPtr) => {
            if (result.ErrorCode != 0) {
              tcs.SetException(result.ToException());
              CallbackManager.Unregister(callback);
              return;
            }

            AppPtr = appPtr;
            tcs.SetResult(true);
            CallbackManager.Unregister(callback);
          };

          CallbackManager.Register(callback);
          NativeBindings.AppRegistered(appId, authGrantedFfiPtr, IntPtr.Zero, UserData, NetObs, callback);
          Marshal.FreeHGlobal(authGrantedFfi.BootStrapConfigPtr);
          Marshal.FreeHGlobal(authGrantedFfiPtr);

          return tcs.Task;
        });
    }

    public static Task<DecodeIpcResult> DecodeIpcMessageAsync(string encodedReq) {
      return Task.Run(
        () => {
          var tcs = new TaskCompletionSource<DecodeIpcResult>();
          DecodeAuthCb authCb = null;
          DecodeUnregCb unregCb = null;
          DecodeContCb contCb = null;
          DecodeShareMDataCb shareMDataCb = null;
          DecodeRevokedCb revokedCb = null;
          DecodeErrorCb errorCb = null;
          var callbacks = new List<object>();

          authCb = (self, id, authGrantedFfiPtr) => {
            var authGrantedFfi = (AuthGrantedFfi)Marshal.PtrToStructure(authGrantedFfiPtr, typeof(AuthGrantedFfi));
            var authGranted = new AuthGranted {
              AppKeys = authGrantedFfi.AppKeys,
              AccessContainer = authGrantedFfi.AccessContainer,
              BootStrapConfig = authGrantedFfi.BootStrapConfigPtr.ToList<byte>(authGrantedFfi.BootStrapConfigLen)
            };

            tcs.SetResult(new DecodeIpcResult {AuthGranted = authGranted});
            CallbackManager.Unregister(callbacks);
          };
          unregCb = (self, id, config, size) => {
            tcs.SetResult(new DecodeIpcResult {UnRegAppInfo = Tuple.Create(config, size)});
            CallbackManager.Unregister(callbacks);
          };
          contCb = (self, id) => {
            tcs.SetResult(new DecodeIpcResult {ContReqId = id});
            CallbackManager.Unregister(callbacks);
          };
          shareMDataCb = (self, id) => {
            tcs.SetResult(new DecodeIpcResult {ShareMData = id});
            CallbackManager.Unregister(callbacks);
          };
          revokedCb = self => {
            tcs.SetResult(new DecodeIpcResult {Revoked = true});
            CallbackManager.Unregister(callbacks);
          };
          errorCb = (self, result) => {
            tcs.SetException(result.ToException());
            CallbackManager.Unregister(callbacks);
          };

          callbacks.AddRange(new List<object> {authCb, unregCb, contCb, shareMDataCb, revokedCb, errorCb});
          CallbackManager.Register(callbacks);
          NativeBindings.DecodeIpcMessage(encodedReq, UserData, authCb, unregCb, contCb, shareMDataCb, revokedCb, errorCb);

          return tcs.Task;
        });
    }

    public static Task<string> EncodeAuthReqAsync(AuthReq authReq) {
      return Task.Run(
        () => {
          var tcs = new TaskCompletionSource<string>();
          var authReqFfi = new AuthReqFfi {
            AppContainer = authReq.AppContainer,
            AppExchangeInfo = authReq.AppExchangeInfo,
            ContainersLen = (IntPtr)authReq.Containers.Count,
            ContainersArrayPtr = authReq.Containers.ToIntPtr()
          };
          var authReqFfiPtr = CommonUtils.Helpers.StructToPtr(authReqFfi);
          EncodeAuthReqCb callback = null;
          callback = (ptr, result, id, req) => {
            if (result.ErrorCode != 0) {
              tcs.SetException(result.ToException());
              CallbackManager.Unregister(callback);
              return;
            }

            tcs.SetResult(req);
            CallbackManager.Unregister(callback);
          };

          CallbackManager.Register(callback);
          NativeBindings.EncodeAuthReq(authReqFfiPtr, UserData, callback);
          Marshal.FreeHGlobal(authReqFfi.ContainersArrayPtr);
          Marshal.FreeHGlobal(authReqFfiPtr);

          return tcs.Task;
        });
    }

    public static void FreeApp() {
      if (AppPtr == IntPtr.Zero) {
        return;
      }
      NativeBindings.FreeApp(AppPtr);
      AppPtr = IntPtr.Zero;
    }

    public static Task<bool> InitLoggingAsync() {
      return Task.Run(
        () => {
          var tcs = new TaskCompletionSource<bool>();
          InitLoggingCb cb2 = null;
          cb2 = (ptr, result) => {
            if (result.ErrorCode != 0) {
              tcs.SetException(result.ToException());
              CallbackManager.Unregister(cb2);
              return;
            }

            tcs.SetResult(true);
            CallbackManager.Unregister(cb2);
          };
          CallbackManager.Register(cb2);

          AppOutputLogPathCallback cb1 = null;
          cb1 = async (ptr, result, path) => {
            if (result.ErrorCode != 0) {
              tcs.SetException(result.ToException());
              CallbackManager.Unregister(cb1);
              CallbackManager.Unregister(cb2);
              return;
            }

            var appPath = Path.GetDirectoryName(path);
            var fileList = new List<Tuple<string, string>> {Tuple.Create("log.toml", Path.Combine(appPath, "log.toml"))};

            var fileOps = DependencyService.Get<IFileOps>();
            await fileOps.TransferAssetsAsync(fileList);

            Debug.WriteLine("Assets Transferred");
            NativeBindings.AppInitLogging(null, UserData, cb2);
            CallbackManager.Unregister(cb1);
          };

          CallbackManager.Register(cb1);
          NativeBindings.AppOutputLogPath("test_file", UserData, cb1);
          return tcs.Task;
        });
    }

    private static void OnNetworkObserverCb(IntPtr self, FfiResult result, int eventType) {
      Debug.WriteLine("Network Observer Fired");
    }
  }
}
