using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommonUtils;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using SafeAuthenticator.Services;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeAuthenticator.Native {
  internal static class Session {
    private static IntPtr _authPtr;
    private static volatile bool _isDisconnected;
    public static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();
    private static readonly NetObsCb NetObs;
    public static readonly IntPtr UserData = IntPtr.Zero;
    public static bool IsDisconnected { get => _isDisconnected; private set => _isDisconnected = value; }

    public static IntPtr AuthPtr {
      private set {
        if (_authPtr == value) {
          return;
        }

        if (_authPtr != IntPtr.Zero) {
          NativeBindings.FreeAuth(_authPtr);
        }

        _authPtr = value;
      }
      get {
        if (_authPtr == IntPtr.Zero) {
          throw new ArgumentNullException(nameof(AuthPtr));
        }
        return _authPtr;
      }
    }

    static Session() {
      AuthPtr = IntPtr.Zero;
      NetObs = OnNetworkObserverCb;
    }

    public static Task<AccountInfo> AuthAccountInfoAsync() {
      var tcs = new TaskCompletionSource<AccountInfo>();
      AuthAccountInfoCb callback = (_, result, accountInfoPtr) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        var acctInfo = Marshal.PtrToStructure<AccountInfo>(accountInfoPtr);
        tcs.SetResult(acctInfo);
      };

      NativeBindings.AuthAccountInfo(AuthPtr, callback);

      return tcs.Task;
    }

    public static Task<DecodeIpcResult> AuthDecodeIpcMsgAsync(string encodedReq) {
      return Task.Run(
        () => {
          var tcs = new TaskCompletionSource<DecodeIpcResult>();
          AppAuthReqCb authCb = (_, id, authReqFfiPtr) => {
            var authReqFfi = Marshal.PtrToStructure<AuthReqFfi>(authReqFfiPtr);
            var authReq = new AuthReq {
              AppContainer = authReqFfi.AppContainer,
              AppExchangeInfo = authReqFfi.AppExchangeInfo,
              Containers = authReqFfi.ContainersArrayPtr.ToList<ContainerPermissions>(authReqFfi.ContainersLen)
            };

            tcs.SetResult(new DecodeIpcResult {AuthReq = authReq});
          };
          AppContReqCb contCb = (_, id, contReq) => { tcs.SetResult(new DecodeIpcResult {ContReq = contReq}); };
          AppUnregAppReqCb unregCb = (_, reqId) => { tcs.SetResult(new DecodeIpcResult {UnRegAppReq = reqId}); };
          AppShareMDataReqCb shareMDataCb = (_, reqId, shareMDataReq, userMetaData) => {
            tcs.SetResult(new DecodeIpcResult {ShareMDataReq = (shareMDataReq, userMetaData)});
          };
          AppReqOnErrorCb errorCb = (_, result, origReq) => { tcs.SetException(new Exception(result.Description)); };

          NativeBindings.AuthDecodeIpcMsg(AuthPtr, encodedReq, authCb, contCb, unregCb, shareMDataCb, errorCb);

          return tcs.Task;
        });
    }

    public static Task<List<RegisteredApp>> AuthRegisteredAppsAsync() {
      var tcs = new TaskCompletionSource<List<RegisteredApp>>();
      AuthRegisteredAppsCb callback = (_, result, regAppsPtr, regAppsLen) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          return;
        }

        var regAppsFfiList = regAppsPtr.ToList<RegisteredAppFfi>(regAppsLen);
        var regApps = regAppsFfiList.Select(
          x => new RegisteredApp(x.AppExchangeInfo, x.ContainersArrayPtr.ToList<ContainerPermissions>(x.ContainersLen))).ToList();
        tcs.SetResult(regApps);
      };

      NativeBindings.AuthRegisteredApps(AuthPtr, callback);

      return tcs.Task;
    }

    public static Task CreateAccountAsync(string location, string password, string invitation) {
      return Task.Run(
        () => {
          var tcs = new TaskCompletionSource<object>();
          CreateAccountCb callback = (_, result, authPtr) => {
            if (result.ErrorCode != 0) {
              tcs.SetException(result.ToException());
              return;
            }

            AuthPtr = authPtr;
            IsDisconnected = false;
            tcs.SetResult(null);
          };

          NativeBindings.CreateAccount(location, password, invitation, NetObs, callback);

          return tcs.Task;
        });
    }

    public static Task<string> EncodeAuthRspAsync(AuthReq authReq, bool isGranted) {
      var tcs = new TaskCompletionSource<string>();
      var authReqFfi = new AuthReqFfi {
        AppContainer = authReq.AppContainer,
        AppExchangeInfo = authReq.AppExchangeInfo,
        ContainersLen = (IntPtr)authReq.Containers.Count,
        ContainersArrayPtr = authReq.Containers.ToIntPtr()
      };

      var authReqFfiPtr = Helpers.Helpers.StructToPtr(authReqFfi);
      EncodeAuthRspCb callback = (_, result, encodedRsp) => {
        // -200 user did not grant access
        if (result.ErrorCode != 0 && result.ErrorCode != -200) {
          tcs.SetException(result.ToException());
          return;
        }

        tcs.SetResult(encodedRsp);
      };

      NativeBindings.EncodeAuthRsp(AuthPtr, authReqFfiPtr, 0, isGranted, callback);
      Marshal.FreeHGlobal(authReqFfi.ContainersArrayPtr);
      Marshal.FreeHGlobal(authReqFfiPtr);

      return tcs.Task;
    }

    public static void FreeAuth() {
      IsDisconnected = false;
      AuthPtr = IntPtr.Zero;
    }

    public static Task<bool> InitLoggingAsync() {
      return Task.Run(
        () => {
          var tcs = new TaskCompletionSource<bool>();
          InitLoggingCb cb3 = (_, result) => {
            if (result.ErrorCode != 0) {
              tcs.SetException(result.ToException());
              return;
            }

            tcs.SetResult(true);
          };

          AuthSetAdditionalSearchPathCb cb2 = (_, result) => {
            if (result.ErrorCode != 0) {
              tcs.SetException(result.ToException());
              return;
            }

            NativeBindings.AuthInitLogging(null, cb3);
          };

          AuthExeFileStemCb cb1 = async (_, result, appName) => {
            if (result.ErrorCode != 0) {
              tcs.SetException(result.ToException());
              return;
            }

            var fileList = new List<(string, string)> {("crust.config", $"{appName}.crust.config"), ("log.toml", "log.toml")};

            var fileOps = DependencyService.Get<IFileOps>();
            await fileOps.TransferAssetsAsync(fileList);

            Debug.WriteLine($"Assets Transferred - {appName}");
            NativeBindings.AuthSetAdditionalSearchPath(fileOps.ConfigFilesPath, cb2);
          };

          NativeBindings.AuthExeFileStem(cb1);

          return tcs.Task;
        });
    }

    public static Task LoginAsync(string location, string password) {
      return Task.Run(
        () => {
          var tcs = new TaskCompletionSource<object>();
          LoginCb callback = (_, result, authPtr) => {
            if (result.ErrorCode != 0) {
              tcs.SetException(result.ToException());
              return;
            }

            AuthPtr = authPtr;
            IsDisconnected = false;
            tcs.SetResult(null);
          };

          NativeBindings.Login(location, password, NetObs, callback);

          return tcs.Task;
        });
    }

    /// <summary>
    ///   Network State Callback
    /// </summary>
    /// <param name="self">Self Ptr</param>
    /// <param name="result">Event Result</param>
    /// <param name="eventType">0 : Connected. -1 : Disconnected</param>
    private static void OnNetworkObserverCb(IntPtr self, int result, int eventType) {
      Debug.WriteLine("Network Observer Fired");

      if (result != 0 || eventType != -1) {
        return;
      }

      IsDisconnected = true;
      Device.BeginInvokeOnMainThread(
        async () => {
          AuthPtr = IntPtr.Zero;
          if (App.IsBackgrounded) {
            return;
          }
          var authService = DependencyService.Get<AuthService>();
          await authService.CheckAndReconnect();
        });
    }
  }
}
