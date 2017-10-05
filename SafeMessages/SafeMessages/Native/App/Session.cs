using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Services;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeMessages.Native.App {
  internal static class Session {
    private static IntPtr _appPtr;
    private static volatile bool _isDisconnected;
    public static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();
    private static readonly NetObsCb NetObs;
    public static bool IsDisconnected { get => _isDisconnected; private set => _isDisconnected = value; }

    public static IntPtr AppPtr {
      private set {
        if (_appPtr == value) {
          return;
        }

        if (_appPtr != IntPtr.Zero) {
          NativeBindings.FreeApp(_appPtr);
        }

        _appPtr = value;
      }
      get {
        if (_appPtr == IntPtr.Zero) {
          throw new ArgumentNullException(nameof(AppPtr));
        }
        return _appPtr;
      }
    }

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

          AppRegisteredCb callback = (_, result, appPtr) => {
            if (result.ErrorCode != 0) {
              tcs.SetException(result.ToException());
              return;
            }

            AppPtr = appPtr;
            IsDisconnected = false;
            tcs.SetResult(true);
          };

          NativeBindings.AppRegistered(appId, authGrantedFfiPtr, NetObs, callback);
          Marshal.FreeHGlobal(authGrantedFfi.BootStrapConfigPtr);
          Marshal.FreeHGlobal(authGrantedFfiPtr);

          return tcs.Task;
        });
    }

    public static Task<DecodeIpcResult> DecodeIpcMessageAsync(string encodedReq) {
      return Task.Run(
        () => {
          var tcs = new TaskCompletionSource<DecodeIpcResult>();

          DecodeAuthCb authCb = (_, id, authGrantedFfiPtr) => {
            var authGrantedFfi = Marshal.PtrToStructure<AuthGrantedFfi>(authGrantedFfiPtr);
            var authGranted = new AuthGranted {
              AppKeys = authGrantedFfi.AppKeys,
              AccessContainer = authGrantedFfi.AccessContainer,
              BootStrapConfig = authGrantedFfi.BootStrapConfigPtr.ToList<byte>(authGrantedFfi.BootStrapConfigLen)
            };

            tcs.SetResult(new DecodeIpcResult {AuthGranted = authGranted});
          };
          DecodeUnregCb unregCb = (_, id, config, size) => { tcs.SetResult(new DecodeIpcResult {UnRegAppInfo = (config, size)}); };
          DecodeContCb contCb = (_, id) => { tcs.SetResult(new DecodeIpcResult {ContReqId = id}); };
          DecodeShareMDataCb shareMDataCb = (_, id) => { tcs.SetResult(new DecodeIpcResult {ShareMData = id}); };
          DecodeRevokedCb revokedCb = _ => { tcs.SetResult(new DecodeIpcResult {Revoked = true}); };
          DecodeErrorCb errorCb = (_, result) => { tcs.SetException(result.ToException()); };

          NativeBindings.DecodeIpcMessage(encodedReq, authCb, unregCb, contCb, shareMDataCb, revokedCb, errorCb);

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
          EncodeAuthReqCb callback = (_, result, id, req) => {
            if (result.ErrorCode != 0) {
              tcs.SetException(result.ToException());
              return;
            }

            tcs.SetResult(req);
          };

          NativeBindings.EncodeAuthReq(authReqFfiPtr, callback);
          Marshal.FreeHGlobal(authReqFfi.ContainersArrayPtr);
          Marshal.FreeHGlobal(authReqFfiPtr);

          return tcs.Task;
        });
    }

    public static void FreeApp() {
      IsDisconnected = false;
      AppPtr = IntPtr.Zero;
    }

    public static Task<bool> InitLoggingAsync() {
      return Task.Run(
        () => {
          var tcs = new TaskCompletionSource<bool>();
          var fileOps = DependencyService.Get<IFileOps>();

          InitLoggingCb cb2 = (_, result) => {
            if (result.ErrorCode != 0) {
              tcs.SetException(result.ToException());
              return;
            }

            tcs.SetResult(true);
          };

          AppSetAdditionalSearchPathCb cb1 = async (_, result) => {
            if (result.ErrorCode != 0) {
              tcs.SetException(result.ToException());
              return;
            }

            var fileList = new List<(string, string)> {("log.toml", "log.toml")};

            await fileOps.TransferAssetsAsync(fileList);
            Debug.WriteLine("Assets Transferred");

            NativeBindings.AppInitLogging(null, cb2);
          };

          NativeBindings.AppSetAdditionalSearchPath(fileOps.ConfigFilesPath, cb1);
          return tcs.Task;
        });
    }

    /// <summary>
    ///   Network State Callback
    /// </summary>
    /// <param name="self">Self Ptr</param>
    /// <param name="result">Event Result</param>
    /// <param name="eventType">0 : Connected. -1 : Disconnected</param>
    private static void OnNetworkObserverCb(IntPtr self, FfiResult result, int eventType) {
      Debug.WriteLine("Network Observer Fired");
      if (result.ErrorCode != 0 || eventType != -1) {
        return;
      }

      IsDisconnected = true;
      Device.BeginInvokeOnMainThread(
        async () => {
          AppPtr = IntPtr.Zero;
          if (SafeMessages.App.IsBackgrounded) {
            return;
          }
          var appService = DependencyService.Get<AppService>();
          await appService.CheckAndReconnect();
        });
    }
  }
}
