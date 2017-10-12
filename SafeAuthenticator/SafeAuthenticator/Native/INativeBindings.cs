using System;

namespace SafeAuthenticator.Native {
  #region Native Delegates

  public delegate void AuthSetAdditionalSearchPathCb(IntPtr self, FfiResult result);

  public delegate void AuthExeFileStemCb(IntPtr self, FfiResult result, string exeFileStem);

  public delegate void EncodeAuthRspCb(IntPtr self, FfiResult result, string encodedRsp);

  public delegate void NetObsCb(IntPtr self, int errorCode, int eventType);

  public delegate void CreateAccountCb(IntPtr self, FfiResult result, IntPtr authPtr);

  public delegate void AppAuthReqCb(IntPtr self, uint reqId, IntPtr authReq);

  public delegate void AppContReqCb(IntPtr self, uint reqId, IntPtr ffiContainersReq);

  public delegate void AppUnregAppReqCb(IntPtr self, uint reqId);

  public delegate void AppShareMDataReqCb(IntPtr self, uint reqId, IntPtr ffiShareMDataReq, IntPtr ffiUserMetaData);

  public delegate void AppReqOnErrorCb(IntPtr self, FfiResult result, string encodedString);

  public delegate void AuthLogPathCb(IntPtr self, FfiResult result, string path);

  public delegate void InitLoggingCb(IntPtr self, FfiResult result);

  public delegate void LoginCb(IntPtr self, FfiResult result, IntPtr authPtr);

  public delegate void AuthRegisteredAppsCb(IntPtr self, FfiResult result, IntPtr registeredAppFfiPtr, IntPtr len);

  public delegate void AuthAccountInfoCb(IntPtr self, FfiResult result, IntPtr accountInfoPtr);

  #endregion

  public interface INativeBindings {
    void AuthAccountInfo(IntPtr authPtr, AuthAccountInfoCb callback);

    void AuthDecodeIpcMsg(
      IntPtr authPtr,
      string encodedString,
      AppAuthReqCb appAuthCb,
      AppContReqCb appContCb,
      AppUnregAppReqCb appUnregCb,
      AppShareMDataReqCb appShareMDataCb,
      AppReqOnErrorCb appReqOnErrorCb);

    void AuthExeFileStem(AuthExeFileStemCb callback);
    void AuthInitLogging(string fileName, InitLoggingCb callback);
    void AuthOutputLogPath(string fileName, AuthLogPathCb callback);

    void AuthRegisteredApps(IntPtr authPtr, AuthRegisteredAppsCb callback);
    void AuthSetAdditionalSearchPath(string path, AuthSetAdditionalSearchPathCb callback);

    void CreateAccount(string location, string password, string invitation, NetObsCb netobs, CreateAccountCb createAcctCb);

    void EncodeAuthRsp(IntPtr authPtr, IntPtr authReq, uint reqId, bool isGranted, EncodeAuthRspCb callback);

    void FreeAuth(IntPtr authPtr);

    void Login(string location, string password, NetObsCb netobs, LoginCb loginAcctCb);
  }
}
