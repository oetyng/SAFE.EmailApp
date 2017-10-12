#if __MOBILE__

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Native;
using Xamarin.Forms;
#if __IOS__
using ObjCRuntime;

#endif

[assembly: Dependency(typeof(NativeBindings))]

namespace SafeAuthenticator.Native {
  public class NativeBindings : INativeBindings {
    #region AuthAccountInfo

    public void AuthAccountInfo(IntPtr authPtr, AuthAccountInfoCb callback) {
      AuthAccountInfoNative(authPtr, callback.ToHandlePtr(), OnAuthAccountInfoCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "auth_account_info")]
#elif __ANDROID__
    [DllImport("safe_authenticator", EntryPoint = "auth_account_info")]
#endif
    public static extern void AuthAccountInfoNative(IntPtr authPtr, IntPtr self, AuthAccountInfoCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(AuthAccountInfoCb))]
#endif
    private static void OnAuthAccountInfoCb(IntPtr self, FfiResult result, IntPtr accountInfoPtr) {
      self.HandlePtrToType<AuthAccountInfoCb>()(IntPtr.Zero, result, accountInfoPtr);
    }

    #endregion

    #region AuthDecodeIpcMsg

    public void AuthDecodeIpcMsg(
      IntPtr authPtr,
      string encodedString,
      AppAuthReqCb appAuthCb,
      AppContReqCb appContCb,
      AppUnregAppReqCb appUnregCb,
      AppShareMDataReqCb appShareMDataCb,
      AppReqOnErrorCb appReqOnErrorCb) {
      var cbs = new List<object> {appAuthCb, appContCb, appUnregCb, appShareMDataCb, appReqOnErrorCb};
      AuthDecodeIpcMsgNative(
        authPtr,
        encodedString,
        cbs.ToHandlePtr(),
        OnAppAuthReqCb,
        OnAppContReqCb,
        OnAppUnregAppReqCb,
        OnAppShareMDataReqCb,
        OnAppReqOnErrorCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "auth_decode_ipc_msg")]
#elif __ANDROID__
    [DllImport("safe_authenticator", EntryPoint = "auth_decode_ipc_msg")]
#endif
    public static extern void AuthDecodeIpcMsgNative(
      IntPtr authPtr,
      string encodedString,
      IntPtr self,
      AppAuthReqCb appAuthCb,
      AppContReqCb appContCb,
      AppUnregAppReqCb appUnregCb,
      AppShareMDataReqCb appShareMDataCb,
      AppReqOnErrorCb appReqOnErrorCb);

#if __IOS__
    [MonoPInvokeCallback(typeof(AppAuthReqCb))]
#endif
    private static void OnAppAuthReqCb(IntPtr self, uint reqId, IntPtr authReq) {
      var cb = (AppAuthReqCb)self.HandlePtrToType<List<object>>()[0];
      cb(IntPtr.Zero, reqId, authReq);
    }

#if __IOS__
    [MonoPInvokeCallback(typeof(AppContReqCb))]
#endif
    private static void OnAppContReqCb(IntPtr self, uint reqId, IntPtr ffiContainersReq) {
      var cb = (AppContReqCb)self.HandlePtrToType<List<object>>()[1];
      cb(IntPtr.Zero, reqId, ffiContainersReq);
    }

#if __IOS__
    [MonoPInvokeCallback(typeof(AppUnregAppReqCb))]
#endif
    private static void OnAppUnregAppReqCb(IntPtr self, uint reqId) {
      var cb = (AppUnregAppReqCb)self.HandlePtrToType<List<object>>()[2];
      cb(IntPtr.Zero, reqId);
    }

#if __IOS__
    [MonoPInvokeCallback(typeof(AppShareMDataReqCb))]
#endif
    private static void OnAppShareMDataReqCb(IntPtr self, uint reqId, IntPtr ffiShareMDataReq, IntPtr ffiUserMetaData) {
      var cb = (AppShareMDataReqCb)self.HandlePtrToType<List<object>>()[3];
      cb(IntPtr.Zero, reqId, ffiShareMDataReq, ffiUserMetaData);
    }

#if __IOS__
    [MonoPInvokeCallback(typeof(AppReqOnErrorCb))]
#endif
    private static void OnAppReqOnErrorCb(IntPtr self, FfiResult result, string encodedString) {
      var cb = (AppReqOnErrorCb)self.HandlePtrToType<List<object>>()[4];
      cb(IntPtr.Zero, result, encodedString);
    }

    #endregion

    #region AuthExeFileStem

    public void AuthExeFileStem(AuthExeFileStemCb callback) {
      AuthExeFileStemNative(callback.ToHandlePtr(), OnAuthExeFileStemCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "auth_exe_file_stem")]
#elif __ANDROID__
    [DllImport("safe_authenticator", EntryPoint = "auth_exe_file_stem")]
#endif
    public static extern void AuthExeFileStemNative(IntPtr self, AuthExeFileStemCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(AuthExeFileStemCb))]
#endif
    private static void OnAuthExeFileStemCb(IntPtr self, FfiResult result, string exeFileStem) {
      self.HandlePtrToType<AuthExeFileStemCb>()(IntPtr.Zero, result, exeFileStem);
    }

    #endregion

    #region AuthInitLogging

    public void AuthInitLogging(string fileName, InitLoggingCb callback) {
      AuthInitLoggingNative(fileName, callback.ToHandlePtr(), OnInitLoggingCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "auth_init_logging")]
#elif __ANDROID__
    [DllImport("safe_authenticator", EntryPoint = "auth_init_logging")]
#endif
    public static extern void AuthInitLoggingNative(string fileName, IntPtr userDataPtr, InitLoggingCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(InitLoggingCb))]
#endif
    private static void OnInitLoggingCb(IntPtr self, FfiResult result) {
      self.HandlePtrToType<InitLoggingCb>()(IntPtr.Zero, result);
    }

    #endregion

    #region AuthOutputLogPath

    public void AuthOutputLogPath(string fileName, AuthLogPathCb callback) {
      AuthOutputLogPathNative(fileName, callback.ToHandlePtr(), OnAuthLogPathCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "auth_output_log_path")]
#elif __ANDROID__
    [DllImport("safe_authenticator", EntryPoint = "auth_output_log_path")]
#endif
    public static extern void AuthOutputLogPathNative(string fileName, IntPtr userDataPtr, AuthLogPathCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(AuthLogPathCb))]
#endif
    private static void OnAuthLogPathCb(IntPtr self, FfiResult result, string path) {
      self.HandlePtrToType<AuthLogPathCb>()(IntPtr.Zero, result, path);
    }

    #endregion

    #region AuthRegisteredApps

    public void AuthRegisteredApps(IntPtr authPtr, AuthRegisteredAppsCb callback) {
      AuthRegisteredAppsNative(authPtr, callback.ToHandlePtr(), OnAuthRegisteredAppsCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "auth_registered_apps")]
#elif __ANDROID__
    [DllImport("safe_authenticator", EntryPoint = "auth_registered_apps")]
#endif
    public static extern void AuthRegisteredAppsNative(IntPtr authPtr, IntPtr self, AuthRegisteredAppsCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(AuthRegisteredAppsCb))]
#endif
    private static void OnAuthRegisteredAppsCb(IntPtr self, FfiResult result, IntPtr registeredAppFfiPtr, IntPtr len) {
      self.HandlePtrToType<AuthRegisteredAppsCb>()(IntPtr.Zero, result, registeredAppFfiPtr, len);
    }

    #endregion

    #region AuthSetAdditionalSearchPath

    public void AuthSetAdditionalSearchPath(string path, AuthSetAdditionalSearchPathCb callback) {
      AuthSetAdditionalSearchPathNative(path, callback.ToHandlePtr(), OAuthSetAdditionalSearchPathCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "auth_set_additional_search_path")]
#elif __ANDROID__
    [DllImport("safe_authenticator", EntryPoint = "auth_set_additional_search_path")]
#endif
    public static extern void AuthSetAdditionalSearchPathNative(string path, IntPtr self, AuthSetAdditionalSearchPathCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(AuthSetAdditionalSearchPathCb))]
#endif
    private static void OAuthSetAdditionalSearchPathCb(IntPtr self, FfiResult result) {
      self.HandlePtrToType<AuthSetAdditionalSearchPathCb>()(IntPtr.Zero, result);
    }

    #endregion

    #region Create Account

    public void CreateAccount(string location, string password, string invitation, NetObsCb netobs, CreateAccountCb createAcctCb) {
      CreateAccountNative(location, password, invitation, netobs.ToHandlePtr(), createAcctCb.ToHandlePtr(), OnNetObsCb, OnCreateAccountCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "create_acc")]
#elif __ANDROID__
    [DllImport("safe_authenticator", EntryPoint = "create_acc")]
#endif
    public static extern void CreateAccountNative(
      string location,
      string password,
      string invitation,
      IntPtr netCbPtr,
      IntPtr userDataPtr,
      NetObsCb netobs,
      CreateAccountCb createAcctCb);

#if __IOS__
    [MonoPInvokeCallback(typeof(CreateAccountCb))]
#endif
    private static void OnCreateAccountCb(IntPtr self, FfiResult result, IntPtr authPtr) {
      self.HandlePtrToType<CreateAccountCb>()(IntPtr.Zero, result, authPtr);
    }

#if __IOS__
    [MonoPInvokeCallback(typeof(NetObsCb))]
#endif
    private static void OnNetObsCb(IntPtr self, int errorCode, int eventType) {
      self.HandlePtrToType<NetObsCb>()(IntPtr.Zero, errorCode, eventType);
    }

    #endregion

    #region EncodeAuthRsp

    public void EncodeAuthRsp(IntPtr authPtr, IntPtr authReq, uint reqId, bool isGranted, EncodeAuthRspCb callback) {
      EncodeAuthRspCallbackNative(authPtr, authReq, reqId, isGranted, callback.ToHandlePtr(), OnEncodeAuthRspCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "encode_auth_resp")]
#elif __ANDROID__
    [DllImport("safe_authenticator", EntryPoint = "encode_auth_resp")]
#endif
    public static extern void EncodeAuthRspCallbackNative(
      IntPtr authPtr,
      IntPtr authReq,
      uint reqId,
      [MarshalAs(UnmanagedType.U1)] bool isGranted,
      IntPtr self,
      EncodeAuthRspCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(EncodeAuthRspCb))]
#endif
    private static void OnEncodeAuthRspCb(IntPtr self, FfiResult result, string encodedRsp) {
      self.HandlePtrToType<EncodeAuthRspCb>()(IntPtr.Zero, result, encodedRsp);
    }

    #endregion

    #region FreeAuthNative

    public void FreeAuth(IntPtr authPtr) {
      FreeAuthNative(authPtr);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "auth_free")]
#elif __ANDROID__
    [DllImport("safe_authenticator", EntryPoint = "auth_free")]
#endif
    public static extern void FreeAuthNative(IntPtr authPtr);

    #endregion

    #region Login

    public void Login(string location, string password, NetObsCb netobs, LoginCb loginAcctCb) {
      LoginNative(location, password, loginAcctCb.ToHandlePtr(), netobs.ToHandlePtr(), OnNetObsCb, OnLoginCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "login")]
#elif __ANDROID__
    [DllImport("safe_authenticator", EntryPoint = "login")]
#endif
    public static extern void LoginNative(
      string location,
      string password,
      IntPtr userDataPtr,
      IntPtr netObsPtr,
      NetObsCb netobs,
      LoginCb loginAcctCb);

#if __IOS__
    [MonoPInvokeCallback(typeof(LoginCb))]
#endif
    private static void OnLoginCb(IntPtr self, FfiResult result, IntPtr authPtr) {
      self.HandlePtrToType<LoginCb>()(IntPtr.Zero, result, authPtr);
    }

    #endregion
  }
}
#endif
