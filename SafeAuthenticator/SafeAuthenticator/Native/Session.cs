using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommonUtils;
using SafeAuthenticator.Models;
using Xamarin.Forms;

// ReSharper disable ConvertToLocalFunction

namespace SafeAuthenticator.Native {
  internal static class Session {
    private static IntPtr _authPtr;
    public static readonly INativeBindings NativeBindings = DependencyService.Get<INativeBindings>();
    private static readonly NetObsCb NetObs;
    public static readonly IntPtr UserData = IntPtr.Zero;

    public static IntPtr AuthPtr {
      private set => _authPtr = value;
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
      AuthAccountInfoCb callback = null;
      callback = (self, result, accountInfoPtr) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        var acctInfo = (AccountInfo)Marshal.PtrToStructure(accountInfoPtr, typeof(AccountInfo));
        tcs.SetResult(acctInfo);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.AuthAccountInfo(AuthPtr, UserData, callback);

      return tcs.Task;
    }

    public static Task<DecodeIpcResult> AuthDecodeIpcMsgAsync(string encodedReq) {
      return Task.Run(
        () => {
          var tcs = new TaskCompletionSource<DecodeIpcResult>();
          AppAuthReqCb authCb = null;
          AppContReqCb contCb = null;
          AppUnregAppReqCb unregCb = null;

          AppShareMDataReqCb shareMDataCb = null;
          AppReqOnErrorCb errorCb = null;
          var callbacks = new List<object>();

          authCb = (self, id, authReqFfiPtr) => {
            var authReqFfi = (AuthReqFfi)Marshal.PtrToStructure(authReqFfiPtr, typeof(AuthReqFfi));
            var authReq = new AuthReq {
              AppContainer = authReqFfi.AppContainer,
              AppExchangeInfo = authReqFfi.AppExchangeInfo,
              Containers = authReqFfi.ContainersArrayPtr.ToList<ContainerPermissions>(authReqFfi.ContainersLen)
            };

            tcs.SetResult(new DecodeIpcResult {AuthReq = authReq});
            CallbackManager.Unregister(callbacks);
          };
          contCb = (self, id, contReq) => {
            tcs.SetResult(new DecodeIpcResult {ContReq = contReq});
            CallbackManager.Unregister(callbacks);
          };
          unregCb = (self, reqId) => {
            tcs.SetResult(new DecodeIpcResult {UnRegAppReq = reqId});
            CallbackManager.Unregister(callbacks);
          };
          shareMDataCb = (self, reqId, shareMDataReq, userMetaData) => {
            tcs.SetResult(new DecodeIpcResult {ShareMDataReq = Tuple.Create(shareMDataReq, userMetaData)});
            CallbackManager.Unregister(callbacks);
          };
          errorCb = (self, result, origReq) => {
            tcs.SetException(new Exception(result.Description));
            CallbackManager.Unregister(callbacks);
          };

          callbacks.AddRange(new List<object> {authCb, unregCb, contCb, shareMDataCb, errorCb});
          CallbackManager.Register(callbacks);
          NativeBindings.AuthDecodeIpcMsg(AuthPtr, encodedReq, UserData, authCb, contCb, unregCb, shareMDataCb, errorCb);

          return tcs.Task;
        });
    }

    public static Task<List<RegisteredApp>> AuthRegisteredAppsAsync() {
      var tcs = new TaskCompletionSource<List<RegisteredApp>>();
      AuthRegisteredAppsCb callback = null;
      callback = (self, result, regAppsPtr, regAppsLen) => {
        if (result.ErrorCode != 0) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        var regAppsFfiList = regAppsPtr.ToList<RegisteredAppFfi>(regAppsLen);
        var regApps = regAppsFfiList.Select(
          x => new RegisteredApp(x.AppExchangeInfo, x.ContainersArrayPtr.ToList<ContainerPermissions>(x.ContainersLen))).ToList();
        tcs.SetResult(regApps);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.AuthRegisteredApps(AuthPtr, UserData, callback);

      return tcs.Task;
    }

    public static Task CreateAccountAsync(string location, string password, string invitation) {
      return Task.Run(
        () => {
          var tcs = new TaskCompletionSource<object>();
          CreateAccountCb callback = null;
          callback = (self, result, authPtr) => {
            if (result.ErrorCode != 0) {
              tcs.SetException(result.ToException());
              CallbackManager.Unregister(callback);
              return;
            }

            AuthPtr = authPtr;
            tcs.SetResult(null);
            CallbackManager.Unregister(callback);
          };

          CallbackManager.Register(callback);
          NativeBindings.CreateAccount(location, password, invitation, IntPtr.Zero, UserData, NetObs, callback);

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

      var authReqFfiPtr = CommonUtils.Helpers.StructToPtr(authReqFfi);

      EncodeAuthRspCb callback = null;
      callback = (self, result, encodedRsp) => {
        // -200 user did not grant access
        if (result.ErrorCode != 0 && result.ErrorCode != -200) {
          tcs.SetException(result.ToException());
          CallbackManager.Unregister(callback);
          return;
        }

        tcs.SetResult(encodedRsp);
        CallbackManager.Unregister(callback);
      };

      CallbackManager.Register(callback);
      NativeBindings.EncodeAuthRsp(AuthPtr, authReqFfiPtr, 0, isGranted, UserData, callback);
      Marshal.FreeHGlobal(authReqFfi.ContainersArrayPtr);
      Marshal.FreeHGlobal(authReqFfiPtr);

      return tcs.Task;
    }

    public static void FreeAuth() {
      if (AuthPtr == IntPtr.Zero) {
        return;
      }
      NativeBindings.FreeAuth(AuthPtr);
      AuthPtr = IntPtr.Zero;
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

          AuthLogPathCb cb1 = null;
          cb1 = async (ptr, result, path) => {
            if (result.ErrorCode != 0) {
              tcs.SetException(result.ToException());
              CallbackManager.Unregister(cb1);
              CallbackManager.Unregister(cb2);
              return;
            }

            var appPath = Path.GetDirectoryName(path);
            var fileList = new List<Tuple<string, string>> {
              Tuple.Create(
                "crust.config",
                Path.Combine(appPath, $"{Path.GetFileName(appPath).Replace(".app", "")}.crust.config")),
              Tuple.Create("log.toml", Path.Combine(appPath, Path.Combine(appPath, "log.toml")))
            };

            var fileOps = DependencyService.Get<IFileOps>();
            await fileOps.TransferAssetsAsync(fileList);

            Debug.WriteLine("Assets Transferred");
            NativeBindings.AuthInitLogging(null, UserData, cb2);
            CallbackManager.Unregister(cb1);
          };

          CallbackManager.Register(cb1);
          NativeBindings.AuthOutputLogPath("test_file", UserData, cb1);
          return tcs.Task;
        });
    }

    public static Task LoginAsync(string location, string password) {
      return Task.Run(
        () => {
          var tcs = new TaskCompletionSource<object>();
          LoginCb callback = null;
          callback = (self, result, authPtr) => {
            if (result.ErrorCode != 0) {
              tcs.SetException(result.ToException());
              CallbackManager.Unregister(callback);
              return;
            }

            AuthPtr = authPtr;
            tcs.SetResult(null);
            CallbackManager.Unregister(callback);
          };

          CallbackManager.Register(callback);
          NativeBindings.Login(location, password, IntPtr.Zero, UserData, NetObs, callback);

          return tcs.Task;
        });
    }

    private static void OnNetworkObserverCb(IntPtr self, int result, int eventType) {
      Debug.WriteLine("Network Observer Fired");
    }
  }
}
