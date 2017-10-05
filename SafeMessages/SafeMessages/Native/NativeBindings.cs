#if __MOBILE__

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CommonUtils;
using SafeMessages.Native;
using Xamarin.Forms;
#if __IOS__
using ObjCRuntime;

#endif

[assembly: Dependency(typeof(NativeBindings))]

namespace SafeMessages.Native {
  public class NativeBindings : INativeBindings {
    #region AccessContainerGetContainerMDataInfo

    public void AccessContainerGetContainerMDataInfo(IntPtr appPtr, string name, AccessContainerGetContainerMDataInfoCb callback) {
      AccessContainerGetContainerMDataInfoNative(appPtr, name, callback.ToHandlePtr(), OnAccessContainerGetContainerMDataInfoCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "access_container_get_container_mdata_info")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "access_container_get_container_mdata_info")]
#endif
    public static extern void AccessContainerGetContainerMDataInfoNative(
      IntPtr appPtr,
      string name,
      IntPtr self,
      AccessContainerGetContainerMDataInfoCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(AccessContainerGetContainerMDataInfoCb))]
#endif
    private static void OnAccessContainerGetContainerMDataInfoCb(IntPtr self, FfiResult result, ulong mDataInfoHandle) {
      self.HandlePtrToType<AccessContainerGetContainerMDataInfoCb>()(IntPtr.Zero, result, mDataInfoHandle);
    }

    #endregion

    #region AppExeFileStem

    public void AppExeFileStem(AppExeFileStemCb callback) {
      AppExeFileStemNative(callback.ToHandlePtr(), OnAppExeFileStemCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "app_exe_file_stem")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "app_exe_file_stem")]
#endif
    public static extern void AppExeFileStemNative(IntPtr self, AppExeFileStemCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(AppExeFileStemCb))]
#endif
    private static void OnAppExeFileStemCb(IntPtr self, FfiResult result, string exeFileStem) {
      self.HandlePtrToType<AppExeFileStemCb>()(IntPtr.Zero, result, exeFileStem);
    }

    #endregion

    #region AppInitLogging

    public void AppInitLogging(string fileName, InitLoggingCb callback) {
      AppInitLoggingNative(fileName, callback.ToHandlePtr(), OnInitLoggingCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "app_init_logging")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "app_init_logging")]
#endif
    public static extern void AppInitLoggingNative(string fileName, IntPtr userDataPtr, InitLoggingCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(InitLoggingCb))]
#endif
    private static void OnInitLoggingCb(IntPtr self, FfiResult result) {
      self.HandlePtrToType<InitLoggingCb>()(IntPtr.Zero, result);
    }

    #endregion

    #region AppOutputLogPath

    public void AppOutputLogPath(string fileName, AppOutputLogPathCallback callback) {
      AppOutputLogPathNative(fileName, callback.ToHandlePtr(), OnAppOutputLogPathCallback);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "app_output_log_path")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "app_output_log_path")]
#endif
    public static extern void AppOutputLogPathNative(string fileName, IntPtr userDataPtr, AppOutputLogPathCallback callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(AppOutputLogPathCallback))]
#endif
    private static void OnAppOutputLogPathCallback(IntPtr self, FfiResult result, string path) {
      self.HandlePtrToType<AppOutputLogPathCallback>()(IntPtr.Zero, result, path);
    }

    #endregion

    #region AppPubSignKey

    public void AppPubSignKey(IntPtr appPtr, AppPubSignKeyCb callback) {
      AppPubSignKeyNative(appPtr, callback.ToHandlePtr(), OnAppPubSignKeyCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "app_pub_sign_key")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "app_pub_sign_key")]
#endif
    public static extern void AppPubSignKeyNative(IntPtr appPtr, IntPtr self, AppPubSignKeyCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(AppPubSignKeyCb))]
#endif
    private static void OnAppPubSignKeyCb(IntPtr self, FfiResult result, ulong signKeyHandle) {
      self.HandlePtrToType<AppPubSignKeyCb>()(IntPtr.Zero, result, signKeyHandle);
    }

    #endregion

    #region AppRegistered

    public void AppRegistered(string appId, IntPtr ffiAuthGrantedPtr, NetObsCb netObsCb, AppRegisteredCb appRegCb) {
      AppRegisteredNative(appId, ffiAuthGrantedPtr, netObsCb.ToHandlePtr(), appRegCb.ToHandlePtr(), OnNetObsCb, OnAppRegisteredCb);
    }
#if __IOS__
    [DllImport("__Internal", EntryPoint = "app_registered")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "app_registered")]
#endif
    public static extern void AppRegisteredNative(
      string appId,
      IntPtr ffiAuthGrantedPtr,
      IntPtr networkUserData,
      IntPtr userData,
      NetObsCb netObsCb,
      AppRegisteredCb appRegCb);

#if __IOS__
    [MonoPInvokeCallback(typeof(AppRegisteredCb))]
#endif
    private static void OnAppRegisteredCb(IntPtr self, FfiResult result, IntPtr appPtr) {
      self.HandlePtrToType<AppRegisteredCb>()(IntPtr.Zero, result, appPtr);
    }

#if __IOS__
    [MonoPInvokeCallback(typeof(NetObsCb))]
#endif
    private static void OnNetObsCb(IntPtr self, FfiResult result, int eventType) {
      self.HandlePtrToType<NetObsCb>()(IntPtr.Zero, result, eventType);
    }

    #endregion

    #region AppSetAdditionalSearchPath

    public void AppSetAdditionalSearchPath(string path, AppSetAdditionalSearchPathCb callback) {
      AppSetAdditionalSearchPathNative(path, callback.ToHandlePtr(), OnAppSetAdditionalSearchPathCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "app_set_additional_search_path")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "app_set_additional_search_path")]
#endif
    public static extern void AppSetAdditionalSearchPathNative(string path, IntPtr self, AppSetAdditionalSearchPathCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(AppSetAdditionalSearchPathCb))]
#endif
    private static void OnAppSetAdditionalSearchPathCb(IntPtr self, FfiResult result) {
      self.HandlePtrToType<AppSetAdditionalSearchPathCb>()(IntPtr.Zero, result);
    }

    #endregion

    #region CipherOptFree

    public void CipherOptFree(IntPtr appPtr, ulong cipherOptHandle, CipherOptFreeCb callback) {
      CipherOptFreeNative(appPtr, cipherOptHandle, callback.ToHandlePtr(), OnCipherOptFreeCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "cipher_opt_free")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "cipher_opt_free")]
#endif
    public static extern void CipherOptFreeNative(IntPtr appPtr, ulong cipherOptHandle, IntPtr self, CipherOptFreeCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(CipherOptFreeCb))]
#endif
    private static void OnCipherOptFreeCb(IntPtr self, FfiResult result) {
      self.HandlePtrToType<CipherOptFreeCb>()(IntPtr.Zero, result);
    }

    #endregion

    #region CipherOptNewPlaintext

    public void CipherOptNewPlaintext(IntPtr appPtr, CipherOptNewPlaintextCb callback) {
      CipherOptNewPlaintextNative(appPtr, callback.ToHandlePtr(), OnCipherOptNewPlaintextCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "cipher_opt_new_plaintext")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "cipher_opt_new_plaintext")]
#endif
    public static extern void CipherOptNewPlaintextNative(IntPtr appPtr, IntPtr self, CipherOptNewPlaintextCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(CipherOptNewPlaintextCb))]
#endif
    private static void OnCipherOptNewPlaintextCb(IntPtr self, FfiResult result, ulong cipherOptHandle) {
      self.HandlePtrToType<CipherOptNewPlaintextCb>()(IntPtr.Zero, result, cipherOptHandle);
    }

    #endregion

    #region DecodeIpcMessage

    public void DecodeIpcMessage(
      string encodedReq,
      DecodeAuthCb authCb,
      DecodeUnregCb unregCb,
      DecodeContCb contCb,
      DecodeShareMDataCb shareMDataCb,
      DecodeRevokedCb revokedCb,
      DecodeErrorCb errorCb) {
      var cbs = new List<object> {authCb, unregCb, contCb, shareMDataCb, revokedCb, errorCb};
      DecodeIpcMessageNative(
        encodedReq,
        cbs.ToHandlePtr(),
        OnDecodeAuthCb,
        OnDecodeUnregCb,
        OnDecodeContCb,
        OnDecodeShareMDataCb,
        OnDecodeRevokedCb,
        OnDecodeErrorCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "decode_ipc_msg")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "decode_ipc_msg")]
#endif
    public static extern void DecodeIpcMessageNative(
      string encodedReq,
      IntPtr self,
      DecodeAuthCb authCb,
      DecodeUnregCb unregCb,
      DecodeContCb contCb,
      DecodeShareMDataCb shareMDataCb,
      DecodeRevokedCb revokedCb,
      DecodeErrorCb errorCb);

#if __IOS__
    [MonoPInvokeCallback(typeof(DecodeAuthCb))]
#endif
    private static void OnDecodeAuthCb(IntPtr self, uint reqId, IntPtr authGrantedFfiPtr) {
      var cb = (DecodeAuthCb)self.HandlePtrToType<List<object>>()[0];
      cb(IntPtr.Zero, reqId, authGrantedFfiPtr);
    }

#if __IOS__
    [MonoPInvokeCallback(typeof(DecodeUnregCb))]
#endif
    private static void OnDecodeUnregCb(IntPtr self, uint reqId, IntPtr bsConfig, IntPtr bsSize) {
      var cb = (DecodeUnregCb)self.HandlePtrToType<List<object>>()[1];
      cb(IntPtr.Zero, reqId, bsConfig, bsSize);
    }

#if __IOS__
    [MonoPInvokeCallback(typeof(DecodeContCb))]
#endif
    private static void OnDecodeContCb(IntPtr self, uint reqId) {
      var cb = (DecodeContCb)self.HandlePtrToType<List<object>>()[2];
      cb(IntPtr.Zero, reqId);
    }

#if __IOS__
    [MonoPInvokeCallback(typeof(DecodeShareMDataCb))]
#endif
    private static void OnDecodeShareMDataCb(IntPtr self, uint reqId) {
      var cb = (DecodeShareMDataCb)self.HandlePtrToType<List<object>>()[3];
      cb(IntPtr.Zero, reqId);
    }

#if __IOS__
    [MonoPInvokeCallback(typeof(DecodeRevokedCb))]
#endif
    private static void OnDecodeRevokedCb(IntPtr self) {
      var cb = (DecodeRevokedCb)self.HandlePtrToType<List<object>>()[4];
      cb(IntPtr.Zero);
    }

#if __IOS__
    [MonoPInvokeCallback(typeof(DecodeErrorCb))]
#endif
    private static void OnDecodeErrorCb(IntPtr self, FfiResult result) {
      var cb = (DecodeErrorCb)self.HandlePtrToType<List<object>>()[5];
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region DecryptSealedBox

    public void DecryptSealedBox(IntPtr appPtr, IntPtr data, IntPtr len, ulong pkHandle, ulong skHandle, DecryptSealedBoxCb callback) {
      DecryptSealedBoxNative(appPtr, data, len, pkHandle, skHandle, callback.ToHandlePtr(), OnDecryptSealedBoxCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "decrypt_sealed_box")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "decrypt_sealed_box")]
#endif
    public static extern void DecryptSealedBoxNative(
      IntPtr appPtr,
      IntPtr data,
      IntPtr len,
      ulong pkHandle,
      ulong skHandle,
      IntPtr self,
      DecryptSealedBoxCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(DecryptSealedBoxCb))]
#endif
    private static void OnDecryptSealedBoxCb(IntPtr self, FfiResult result, IntPtr data, IntPtr dataLen) {
      var cb = self.HandlePtrToType<DecryptSealedBoxCb>();
      cb(IntPtr.Zero, result, data, dataLen);
    }

    #endregion

    #region EncGenerateKeyPair

    public void EncGenerateKeyPair(IntPtr appPtr, EncGenerateKeyPairCb callback) {
      EncGenerateKeyPairNative(appPtr, callback.ToHandlePtr(), OnEncGenerateKeyPairCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "enc_generate_key_pair")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "enc_generate_key_pair")]
#endif
    public static extern void EncGenerateKeyPairNative(IntPtr appPtr, IntPtr self, EncGenerateKeyPairCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(EncGenerateKeyPairCb))]
#endif
    private static void OnEncGenerateKeyPairCb(IntPtr self, FfiResult result, ulong encPubKeyHandle, ulong encSecKeyHandle) {
      var cb = self.HandlePtrToType<EncGenerateKeyPairCb>();
      cb(IntPtr.Zero, result, encPubKeyHandle, encSecKeyHandle);
    }

    #endregion

    #region EncodeAuthReq

    public void EncodeAuthReq(IntPtr authReq, EncodeAuthReqCb callback) {
      EncodeAuthReqNative(authReq, callback.ToHandlePtr(), OnEncodeAuthReqCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "encode_auth_req")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "encode_auth_req")]
#endif
    public static extern void EncodeAuthReqNative(IntPtr authReq, IntPtr userDataPtr, EncodeAuthReqCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(EncodeAuthReqCb))]
#endif
    private static void OnEncodeAuthReqCb(IntPtr self, FfiResult result, uint requestId, string encodedReq) {
      var cb = self.HandlePtrToType<EncodeAuthReqCb>();
      cb(IntPtr.Zero, result, requestId, encodedReq);
    }

    #endregion

    #region EncPubKeyFree

    public void EncPubKeyFree(IntPtr appPtr, ulong encryptPubKeyHandle, EncPubKeyFreeCb callback) {
      EncPubKeyFreeNative(appPtr, encryptPubKeyHandle, callback.ToHandlePtr(), OnEncPubKeyFreeCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "enc_pub_key_free")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "enc_pub_key_free")]
#endif
    public static extern void EncPubKeyFreeNative(IntPtr appPtr, ulong encryptPubKeyHandle, IntPtr self, EncPubKeyFreeCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(EncPubKeyFreeCb))]
#endif
    private static void OnEncPubKeyFreeCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<EncPubKeyFreeCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region EncPubKeyGet

    public void EncPubKeyGet(IntPtr appPtr, ulong encryptPubKeyHandle, EncPubKeyGetCb callback) {
      EncPubKeyGetNative(appPtr, encryptPubKeyHandle, callback.ToHandlePtr(), OnEncPubKeyGetCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "enc_pub_key_get")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "enc_pub_key_get")]
#endif
    public static extern void EncPubKeyGetNative(IntPtr appPtr, ulong encryptPubKeyHandle, IntPtr self, EncPubKeyGetCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(EncPubKeyGetCb))]
#endif
    private static void OnEncPubKeyGetCb(IntPtr self, FfiResult result, IntPtr asymPublicKey) {
      var cb = self.HandlePtrToType<EncPubKeyGetCb>();
      cb(IntPtr.Zero, result, asymPublicKey);
    }

    #endregion

    #region EncPubKeyNew

    public void EncPubKeyNew(IntPtr appPtr, IntPtr asymPublicKey, EncPubKeyNewCb callback) {
      EncPubKeyNewNative(appPtr, asymPublicKey, callback.ToHandlePtr(), OnEncPubKeyNewCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "enc_pub_key_new")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "enc_pub_key_new")]
#endif
    public static extern void EncPubKeyNewNative(IntPtr appPtr, IntPtr asymPublicKey, IntPtr self, EncPubKeyNewCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(EncPubKeyNewCb))]
#endif
    private static void OnEncPubKeyNewCb(IntPtr self, FfiResult result, ulong encryptPubKeyHandle) {
      var cb = self.HandlePtrToType<EncPubKeyNewCb>();
      cb(IntPtr.Zero, result, encryptPubKeyHandle);
    }

    #endregion

    #region EncryptSealedBox

    public void EncryptSealedBox(IntPtr appPtr, IntPtr data, IntPtr len, ulong pkHandle, EncryptSealedBoxCb callback) {
      EncryptSealedBoxNative(appPtr, data, len, pkHandle, callback.ToHandlePtr(), OnEncryptSealedBoxCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "encrypt_sealed_box")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "encrypt_sealed_box")]
#endif
    public static extern void EncryptSealedBoxNative(
      IntPtr appPtr,
      IntPtr data,
      IntPtr len,
      ulong pkHandle,
      IntPtr self,
      EncryptSealedBoxCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(EncryptSealedBoxCb))]
#endif
    private static void OnEncryptSealedBoxCb(IntPtr self, FfiResult result, IntPtr data, IntPtr dataLen) {
      var cb = self.HandlePtrToType<EncryptSealedBoxCb>();
      cb(IntPtr.Zero, result, data, dataLen);
    }

    #endregion

    #region EncSecretKeyFree

    public void EncSecretKeyFree(IntPtr appPtr, ulong encryptSecKeyHandle, EncSecretKeyFreeCb callback) {
      EncSecretKeyFreeNative(appPtr, encryptSecKeyHandle, callback.ToHandlePtr(), OnEncSecretKeyFreeCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "enc_secret_key_free")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "enc_secret_key_free")]
#endif
    public static extern void EncSecretKeyFreeNative(IntPtr appPtr, ulong encryptSecKeyHandle, IntPtr self, EncSecretKeyFreeCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(EncSecretKeyFreeCb))]
#endif
    private static void OnEncSecretKeyFreeCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<EncSecretKeyFreeCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region EncSecretKeyGet

    public void EncSecretKeyGet(IntPtr appPtr, ulong encryptSecKeyHandle, EncSecretKeyGetCb callback) {
      EncSecretKeyGetNative(appPtr, encryptSecKeyHandle, callback.ToHandlePtr(), OnEncSecretKeyGetCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "enc_secret_key_get")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "enc_secret_key_get")]
#endif
    public static extern void EncSecretKeyGetNative(IntPtr appPtr, ulong encryptSecKeyHandle, IntPtr self, EncSecretKeyGetCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(EncSecretKeyGetCb))]
#endif
    private static void OnEncSecretKeyGetCb(IntPtr self, FfiResult result, IntPtr asymSecretKey) {
      var cb = self.HandlePtrToType<EncSecretKeyGetCb>();
      cb(IntPtr.Zero, result, asymSecretKey);
    }

    #endregion

    #region EncSecretKeyNew

    public void EncSecretKeyNew(IntPtr appPtr, IntPtr asymSecretKey, EncSecretKeyNewCb callback) {
      EncSecretKeyNewNative(appPtr, asymSecretKey, callback.ToHandlePtr(), OnEncSecretKeyNewCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "enc_secret_key_new")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "enc_secret_key_new")]
#endif
    public static extern void EncSecretKeyNewNative(IntPtr appPtr, IntPtr asymSecretKey, IntPtr self, EncSecretKeyNewCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(EncSecretKeyNewCb))]
#endif
    private static void OnEncSecretKeyNewCb(IntPtr self, FfiResult result, ulong encryptSecKeyHandle) {
      var cb = self.HandlePtrToType<EncSecretKeyNewCb>();
      cb(IntPtr.Zero, result, encryptSecKeyHandle);
    }

    #endregion

    #region FreeAppNative

    public void FreeApp(IntPtr appPtr) {
      FreeAppNative(appPtr);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "app_free")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "app_free")]
#endif
    public static extern void FreeAppNative(IntPtr appPtr);

    #endregion

    #region IDataCloseSelfEncryptor

    public void IDataCloseSelfEncryptor(IntPtr appPtr, ulong seHandle, ulong cipherOptHandle, IDataCloseSelfEncryptorCb callback) {
      IDataCloseSelfEncryptorNative(appPtr, seHandle, cipherOptHandle, callback.ToHandlePtr(), OnIDataCloseSelfEncryptorCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "idata_close_self_encryptor")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "idata_close_self_encryptor")]
#endif
    public static extern void IDataCloseSelfEncryptorNative(
      IntPtr appPtr,
      ulong seHandle,
      ulong cipherOptHandle,
      IntPtr self,
      IDataCloseSelfEncryptorCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(IDataCloseSelfEncryptorCb))]
#endif
    private static void OnIDataCloseSelfEncryptorCb(IntPtr self, FfiResult result, IntPtr xorNameArr) {
      var cb = self.HandlePtrToType<IDataCloseSelfEncryptorCb>();
      cb(IntPtr.Zero, result, xorNameArr);
    }

    #endregion

    #region IDataFetchSelfEncryptor

    public void IDataFetchSelfEncryptor(IntPtr appPtr, IntPtr xorNameArr, IDataFetchSelfEncryptorCb callback) {
      IDataFetchSelfEncryptorNative(appPtr, xorNameArr, callback.ToHandlePtr(), OnIDataFetchSelfEncryptorCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "idata_fetch_self_encryptor")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "idata_fetch_self_encryptor")]
#endif
    public static extern void IDataFetchSelfEncryptorNative(
      IntPtr appPtr,
      IntPtr xorNameArr,
      IntPtr self,
      IDataFetchSelfEncryptorCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(IDataFetchSelfEncryptorCb))]
#endif
    private static void OnIDataFetchSelfEncryptorCb(IntPtr self, FfiResult result, ulong sEReaderHandle) {
      var cb = self.HandlePtrToType<IDataFetchSelfEncryptorCb>();
      cb(IntPtr.Zero, result, sEReaderHandle);
    }

    #endregion

    #region IDataNewSelfEncryptor

    public void IDataNewSelfEncryptor(IntPtr appPtr, IDataNewSelfEncryptorCb callback) {
      IDataNewSelfEncryptorNative(appPtr, callback.ToHandlePtr(), OnIDataNewSelfEncryptorCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "idata_new_self_encryptor")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "idata_new_self_encryptor")]
#endif
    public static extern void IDataNewSelfEncryptorNative(IntPtr appPtr, IntPtr self, IDataNewSelfEncryptorCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(IDataNewSelfEncryptorCb))]
#endif
    private static void OnIDataNewSelfEncryptorCb(IntPtr self, FfiResult result, ulong sEWriterHandle) {
      var cb = self.HandlePtrToType<IDataNewSelfEncryptorCb>();
      cb(IntPtr.Zero, result, sEWriterHandle);
    }

    #endregion

    #region IDataReadFromSelfEncryptor

    public void IDataReadFromSelfEncryptor(IntPtr appPtr, ulong seHandle, ulong fromPos, ulong len, IDataReadFromSelfEncryptorCb callback) {
      IDataReadFromSelfEncryptorNative(appPtr, seHandle, fromPos, len, callback.ToHandlePtr(), OnIDataReadFromSelfEncryptorCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "idata_read_from_self_encryptor")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "idata_read_from_self_encryptor")]
#endif
    public static extern void IDataReadFromSelfEncryptorNative(
      IntPtr appPtr,
      ulong seHandle,
      ulong fromPos,
      ulong len,
      IntPtr self,
      IDataReadFromSelfEncryptorCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(IDataReadFromSelfEncryptorCb))]
#endif
    private static void OnIDataReadFromSelfEncryptorCb(IntPtr self, FfiResult result, IntPtr data, IntPtr dataLen) {
      var cb = self.HandlePtrToType<IDataReadFromSelfEncryptorCb>();
      cb(IntPtr.Zero, result, data, dataLen);
    }

    #endregion

    #region IDataSelfEncryptorReaderFree

    public void IDataSelfEncryptorReaderFree(IntPtr appPtr, ulong sEReaderHandle, IDataSelfEncryptorReaderFreeCb callback) {
      IDataSelfEncryptorReaderFreeNative(appPtr, sEReaderHandle, callback.ToHandlePtr(), OnIDataSelfEncryptorReaderFreeCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "idata_self_encryptor_reader_free")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "idata_self_encryptor_reader_free")]
#endif
    public static extern void IDataSelfEncryptorReaderFreeNative(
      IntPtr appPtr,
      ulong sEReaderHandle,
      IntPtr self,
      IDataSelfEncryptorReaderFreeCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(IDataSelfEncryptorReaderFreeCb))]
#endif
    private static void OnIDataSelfEncryptorReaderFreeCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<IDataSelfEncryptorReaderFreeCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region IDataSelfEncryptorWriterFree

    public void IDataSelfEncryptorWriterFree(IntPtr appPtr, ulong sEWriterHandle, IDataSelfEncryptorWriterFreeCb callback) {
      IDataSelfEncryptorWriterFreeNative(appPtr, sEWriterHandle, callback.ToHandlePtr(), OnIDataSelfEncryptorWriterFreeCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "idata_self_encryptor_writer_free")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "idata_self_encryptor_writer_free")]
#endif
    public static extern void IDataSelfEncryptorWriterFreeNative(
      IntPtr appPtr,
      ulong sEWriterHandle,
      IntPtr self,
      IDataSelfEncryptorWriterFreeCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(IDataSelfEncryptorWriterFreeCb))]
#endif
    private static void OnIDataSelfEncryptorWriterFreeCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<IDataSelfEncryptorWriterFreeCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region IDataSize

    public void IDataSize(IntPtr appPtr, ulong seHandle, IDataSizeCb callback) {
      IDataSizeNative(appPtr, seHandle, callback.ToHandlePtr(), OnIDataSizeCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "idata_size")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "idata_size")]
#endif
    public static extern void IDataSizeNative(IntPtr appPtr, ulong seHandle, IntPtr self, IDataSizeCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(IDataSizeCb))]
#endif
    private static void OnIDataSizeCb(IntPtr self, FfiResult result, ulong len) {
      var cb = self.HandlePtrToType<IDataSizeCb>();
      cb(IntPtr.Zero, result, len);
    }

    #endregion

    #region IDataWriteToSelfEncryptor

    public void IDataWriteToSelfEncryptor(IntPtr appPtr, ulong seHandle, IntPtr data, IntPtr size, IDataWriteToSelfEncryptorCb callback) {
      IDataWriteToSelfEncryptorNative(appPtr, seHandle, data, size, callback.ToHandlePtr(), OnIDataWriteToSelfEncryptorCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "idata_write_to_self_encryptor")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "idata_write_to_self_encryptor")]
#endif
    public static extern void IDataWriteToSelfEncryptorNative(
      IntPtr appPtr,
      ulong seHandle,
      IntPtr data,
      IntPtr size,
      IntPtr self,
      IDataWriteToSelfEncryptorCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(IDataWriteToSelfEncryptorCb))]
#endif
    private static void OnIDataWriteToSelfEncryptorCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<IDataWriteToSelfEncryptorCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region MDataEntriesForEach

    public void MDataEntriesForEach(
      IntPtr appPtr,
      ulong entriesHandle,
      MDataEntriesForEachCb forEachCallback,
      MDataEntriesForEachResCb resultCallback) {
      var cbs = new List<object> {forEachCallback, resultCallback};
      MDataEntriesForEachNative(appPtr, entriesHandle, cbs.ToHandlePtr(), OnMDataEntriesForEachCb, OnMDataEntriesForEachResCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_entries_for_each")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_entries_for_each")]
#endif
    public static extern void MDataEntriesForEachNative(
      IntPtr appPtr,
      ulong entriesHandle,
      IntPtr self,
      MDataEntriesForEachCb forEachCallback,
      MDataEntriesForEachResCb resultCallback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataEntriesForEachCb))]
#endif
    private static void OnMDataEntriesForEachCb(
      IntPtr self,
      IntPtr entryKey,
      IntPtr entryKeyLen,
      IntPtr entryVal,
      IntPtr entryValLen,
      ulong entryVersion) {
      var cb = (MDataEntriesForEachCb)self.HandlePtrToType<List<object>>(false)[0];
      cb(IntPtr.Zero, entryKey, entryKeyLen, entryVal, entryValLen, entryVersion);
    }

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataEntriesForEachResCb))]
#endif
    private static void OnMDataEntriesForEachResCb(IntPtr self, FfiResult result) {
      var cb = (MDataEntriesForEachResCb)self.HandlePtrToType<List<object>>()[1];
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region MDataEntriesFree

    public void MDataEntriesFree(IntPtr appPtr, ulong entriesHandle, MDataEntriesFreeCb callback) {
      MDataEntriesFreeNative(appPtr, entriesHandle, callback.ToHandlePtr(), OnMDataEntriesFreeCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_entries_free")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_entries_free")]
#endif
    public static extern void MDataEntriesFreeNative(IntPtr appPtr, ulong entriesHandle, IntPtr self, MDataEntriesFreeCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataEntriesFreeCb))]
#endif
    private static void OnMDataEntriesFreeCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<MDataEntriesFreeCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region MDataEntriesInsert

    public void MDataEntriesInsert(
      IntPtr appPtr,
      ulong entriesHandle,
      IntPtr keyPtr,
      IntPtr keyLen,
      IntPtr valuePtr,
      IntPtr valueLen,
      MDataEntriesInsertCb callback) {
      MDataEntriesInsertNative(appPtr, entriesHandle, keyPtr, keyLen, valuePtr, valueLen, callback.ToHandlePtr(), OnMDataEntriesInsertCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_entries_insert")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_entries_insert")]
#endif
    public static extern void MDataEntriesInsertNative(
      IntPtr appPtr,
      ulong entriesHandle,
      IntPtr keyPtr,
      IntPtr keyLen,
      IntPtr valuePtr,
      IntPtr valueLen,
      IntPtr self,
      MDataEntriesInsertCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataEntriesInsertCb))]
#endif
    private static void OnMDataEntriesInsertCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<MDataEntriesInsertCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region MDataEntriesLen

    public void MDataEntriesLen(IntPtr appPtr, ulong entriesHandle, MDataEntriesLenCb callback) {
      MDataEntriesLenNative(appPtr, entriesHandle, callback.ToHandlePtr(), OnMDataEntriesLenCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_entries_len")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_entries_len")]
#endif
    public static extern void MDataEntriesLenNative(IntPtr appPtr, ulong entriesHandle, IntPtr self, MDataEntriesLenCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataEntriesLenCb))]
#endif
    private static void OnMDataEntriesLenCb(IntPtr self, ulong len) {
      var cb = self.HandlePtrToType<MDataEntriesLenCb>();
      cb(IntPtr.Zero, len);
    }

    #endregion

    #region MDataEntriesNew

    public void MDataEntriesNew(IntPtr appPtr, MDataEntriesNewCb callback) {
      MDataEntriesNewNative(appPtr, callback.ToHandlePtr(), OnMDataEntriesNewCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_entries_new")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_entries_new")]
#endif
    public static extern void MDataEntriesNewNative(IntPtr appPtr, IntPtr self, MDataEntriesNewCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataEntriesNewCb))]
#endif
    private static void OnMDataEntriesNewCb(IntPtr self, FfiResult result, ulong mDataEntriesHandle) {
      var cb = self.HandlePtrToType<MDataEntriesNewCb>();
      cb(IntPtr.Zero, result, mDataEntriesHandle);
    }

    #endregion

    #region MDataEntryActionsFree

    public void MDataEntryActionsFree(IntPtr appPtr, ulong actionsHandle, MDataEntryActionsFreeCb callback) {
      MDataEntryActionsFreeNative(appPtr, actionsHandle, callback.ToHandlePtr(), OnMDataEntryActionsFreeCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_entry_actions_free")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_entry_actions_free")]
#endif
    public static extern void MDataEntryActionsFreeNative(
      IntPtr appPtr,
      ulong actionsHandle,
      IntPtr self,
      MDataEntryActionsFreeCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataEntryActionsFreeCb))]
#endif
    private static void OnMDataEntryActionsFreeCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<MDataEntryActionsFreeCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region MDataEntryActionsInsert

    public void MDataEntryActionsInsert(
      IntPtr appPtr,
      ulong actionsHandle,
      IntPtr keyPtr,
      IntPtr keyLen,
      IntPtr valuePtr,
      IntPtr valueLen,
      MDataEntryActionsInsertCb callback) {
      MDataEntryActionsInsertNative(
        appPtr,
        actionsHandle,
        keyPtr,
        keyLen,
        valuePtr,
        valueLen,
        callback.ToHandlePtr(),
        OnMDataEntryActionsInsertCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_entry_actions_insert")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_entry_actions_insert")]
#endif
    public static extern void MDataEntryActionsInsertNative(
      IntPtr appPtr,
      ulong actionsHandle,
      IntPtr keyPtr,
      IntPtr keyLen,
      IntPtr valuePtr,
      IntPtr valueLen,
      IntPtr self,
      MDataEntryActionsInsertCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataEntryActionsInsertCb))]
#endif
    private static void OnMDataEntryActionsInsertCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<MDataEntryActionsInsertCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region MDataEntryActionsNew

    public void MDataEntryActionsNew(IntPtr appPtr, MDataEntryActionsNewCb callback) {
      MDataEntryActionsNewNative(appPtr, callback.ToHandlePtr(), OnMDataEntryActionsNewCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_entry_actions_new")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_entry_actions_new")]
#endif
    public static extern void MDataEntryActionsNewNative(IntPtr appPtr, IntPtr self, MDataEntryActionsNewCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataEntryActionsNewCb))]
#endif
    private static void OnMDataEntryActionsNewCb(IntPtr self, FfiResult result, ulong mDataEntryActionsHandle) {
      var cb = self.HandlePtrToType<MDataEntryActionsNewCb>();
      cb(IntPtr.Zero, result, mDataEntryActionsHandle);
    }

    #endregion

    #region MDataGetValue

    public void MDataGetValue(IntPtr appPtr, ulong infoHandle, IntPtr keyPtr, IntPtr keyLen, MDataGetValueCb callback) {
      MDataGetValueNative(appPtr, infoHandle, keyPtr, keyLen, callback.ToHandlePtr(), OnMDataGetValueCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_get_value")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_get_value")]
#endif
    public static extern void MDataGetValueNative(
      IntPtr appPtr,
      ulong infoHandle,
      IntPtr keyPtr,
      IntPtr keyLen,
      IntPtr self,
      MDataGetValueCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataGetValueCb))]
#endif
    private static void OnMDataGetValueCb(IntPtr self, FfiResult result, IntPtr data, IntPtr dataLen, ulong entryVersion) {
      var cb = self.HandlePtrToType<MDataGetValueCb>();
      cb(IntPtr.Zero, result, data, dataLen, entryVersion);
    }

    #endregion

    #region MDataInfoDecrypt

    public void MDataInfoDecrypt(IntPtr appPtr, ulong mDataInfoH, IntPtr cipherText, IntPtr cipherLen, MDataInfoDecryptCb callback) {
      MDataInfoDecryptNative(appPtr, mDataInfoH, cipherText, cipherLen, callback.ToHandlePtr(), OnMDataInfoDecryptCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_info_decrypt")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_info_decrypt")]
#endif
    public static extern void MDataInfoDecryptNative(
      IntPtr appPtr,
      ulong mDataInfoH,
      IntPtr cipherText,
      IntPtr cipherLen,
      IntPtr self,
      MDataInfoDecryptCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataInfoDecryptCb))]
#endif
    private static void OnMDataInfoDecryptCb(IntPtr self, FfiResult result, IntPtr plainText, IntPtr len) {
      var cb = self.HandlePtrToType<MDataInfoDecryptCb>();
      cb(IntPtr.Zero, result, plainText, len);
    }

    #endregion

    #region MDataInfoDeserialise

    public void MDataInfoDeserialise(IntPtr appPtr, IntPtr ptr, IntPtr len, MDataInfoDeserialiseCb callback) {
      MDataInfoDeserialiseNative(appPtr, ptr, len, callback.ToHandlePtr(), OnMDataInfoDeserialiseCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_info_deserialise")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_info_deserialise")]
#endif
    public static extern void MDataInfoDeserialiseNative(
      IntPtr appPtr,
      IntPtr ptr,
      IntPtr len,
      IntPtr self,
      MDataInfoDeserialiseCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataInfoDeserialiseCb))]
#endif
    private static void OnMDataInfoDeserialiseCb(IntPtr self, FfiResult result, ulong mDataInfoHandle) {
      var cb = self.HandlePtrToType<MDataInfoDeserialiseCb>();
      cb(IntPtr.Zero, result, mDataInfoHandle);
    }

    #endregion

    #region MDataInfoEncryptEntryKey

    public void MDataInfoEncryptEntryKey(
      IntPtr appPtr,
      ulong infoH,
      IntPtr inputPtr,
      IntPtr inputLen,
      MDataInfoEncryptEntryKeyCb callback) {
      MDataInfoEncryptEntryKeyNative(appPtr, infoH, inputPtr, inputLen, callback.ToHandlePtr(), OnMDataInfoEncryptEntryKeyCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_info_encrypt_entry_key")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_info_encrypt_entry_key")]
#endif
    public static extern void MDataInfoEncryptEntryKeyNative(
      IntPtr appPtr,
      ulong infoH,
      IntPtr inputPtr,
      IntPtr inputLen,
      IntPtr self,
      MDataInfoEncryptEntryKeyCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataInfoEncryptEntryKeyCb))]
#endif
    private static void OnMDataInfoEncryptEntryKeyCb(IntPtr self, FfiResult result, IntPtr dataPtr, IntPtr dataLen) {
      var cb = self.HandlePtrToType<MDataInfoEncryptEntryKeyCb>();
      cb(IntPtr.Zero, result, dataPtr, dataLen);
    }

    #endregion

    #region MDataInfoEncryptEntryValue

    public void MDataInfoEncryptEntryValue(
      IntPtr appPtr,
      ulong infoH,
      IntPtr inputPtr,
      IntPtr inputLen,
      MDataInfoEncryptEntryValueCb callback) {
      MDataInfoEncryptEntryValueNative(appPtr, infoH, inputPtr, inputLen, callback.ToHandlePtr(), OnMDataInfoEncryptEntryValueCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_info_encrypt_entry_value")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_info_encrypt_entry_value")]
#endif
    public static extern void MDataInfoEncryptEntryValueNative(
      IntPtr appPtr,
      ulong infoH,
      IntPtr inputPtr,
      IntPtr inputLen,
      IntPtr self,
      MDataInfoEncryptEntryValueCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataInfoEncryptEntryValueCb))]
#endif
    private static void OnMDataInfoEncryptEntryValueCb(IntPtr self, FfiResult result, IntPtr dataPtr, IntPtr dataLen) {
      var cb = self.HandlePtrToType<MDataInfoEncryptEntryValueCb>();
      cb(IntPtr.Zero, result, dataPtr, dataLen);
    }

    #endregion

    #region MDataInfoFree

    public void MDataInfoFree(IntPtr appPtr, ulong infoHandle, MDataInfoFreeCb callback) {
      MDataInfoFreeNative(appPtr, infoHandle, callback.ToHandlePtr(), OnMDataInfoFreeCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_info_free")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_info_free")]
#endif
    public static extern void MDataInfoFreeNative(IntPtr appPtr, ulong infoHandle, IntPtr self, MDataInfoFreeCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataInfoFreeCb))]
#endif
    private static void OnMDataInfoFreeCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<MDataInfoFreeCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region MDataInfoNewPublic

    public void MDataInfoNewPublic(IntPtr appPtr, IntPtr xorNameArr, ulong typeTag, MDataInfoNewPublicCb callback) {
      MDataInfoNewPublicNative(appPtr, xorNameArr, typeTag, callback.ToHandlePtr(), OnMDataInfoNewPublicCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_info_new_public")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_info_new_public")]
#endif
    public static extern void MDataInfoNewPublicNative(
      IntPtr appPtr,
      IntPtr xorNameArr,
      ulong typeTag,
      IntPtr self,
      MDataInfoNewPublicCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataInfoNewPublicCb))]
#endif
    private static void OnMDataInfoNewPublicCb(IntPtr self, FfiResult result, ulong mDataInfoHandle) {
      var cb = self.HandlePtrToType<MDataInfoNewPublicCb>();
      cb(IntPtr.Zero, result, mDataInfoHandle);
    }

    #endregion

    #region MDataInfoRandomPrivate

    public void MDataInfoRandomPrivate(IntPtr appPtr, ulong typeTag, MDataInfoRandomPrivateCb callback) {
      MDataInfoRandomPrivateNative(appPtr, typeTag, callback.ToHandlePtr(), OnMDataInfoRandomPrivateCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_info_random_private")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_info_random_private")]
#endif
    public static extern void MDataInfoRandomPrivateNative(IntPtr appPtr, ulong typeTag, IntPtr self, MDataInfoRandomPrivateCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataInfoRandomPrivateCb))]
#endif
    private static void OnMDataInfoRandomPrivateCb(IntPtr self, FfiResult result, ulong mDataInfoHandle) {
      var cb = self.HandlePtrToType<MDataInfoRandomPrivateCb>();
      cb(IntPtr.Zero, result, mDataInfoHandle);
    }

    #endregion

    #region MDataInfoRandomPublic

    public void MDataInfoRandomPublic(IntPtr appPtr, ulong typeTag, MDataInfoRandomPublicCb callback) {
      MDataInfoRandomPublicNative(appPtr, typeTag, callback.ToHandlePtr(), OnMDataInfoRandomPublicCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_info_random_public")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_info_random_public")]
#endif
    public static extern void MDataInfoRandomPublicNative(IntPtr appPtr, ulong typeTag, IntPtr self, MDataInfoRandomPublicCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataInfoRandomPublicCb))]
#endif
    private static void OnMDataInfoRandomPublicCb(IntPtr self, FfiResult result, ulong mDataInfoHandle) {
      var cb = self.HandlePtrToType<MDataInfoRandomPublicCb>();
      cb(IntPtr.Zero, result, mDataInfoHandle);
    }

    #endregion

    #region MDataInfoSerialise

    public void MDataInfoSerialise(IntPtr appPtr, ulong infoHandle, MDataInfoSerialiseCb callback) {
      MDataInfoSerialiseNative(appPtr, infoHandle, callback.ToHandlePtr(), OnMDataInfoSerialiseCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_info_serialise")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_info_serialise")]
#endif
    public static extern void MDataInfoSerialiseNative(IntPtr appPtr, ulong infoHandle, IntPtr self, MDataInfoSerialiseCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataInfoSerialiseCb))]
#endif
    private static void OnMDataInfoSerialiseCb(IntPtr self, FfiResult result, IntPtr data, IntPtr dataLen) {
      var cb = self.HandlePtrToType<MDataInfoSerialiseCb>();
      cb(IntPtr.Zero, result, data, dataLen);
    }

    #endregion

    #region MDataKeysForEach

    public void MDataKeysForEach(IntPtr appPtr, ulong keysHandle, MDataKeysForEachCb forEachCb, MDataKeysForEachResCb resCb) {
      var cbs = new List<object> {forEachCb, resCb};
      var a = cbs.ToHandlePtr();
      Debug.WriteLine(a);
      MDataKeysForEachNative(appPtr, keysHandle, a, OnMDataKeysForEachCb, OnMDataKeysForEachResCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_keys_for_each")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_keys_for_each")]
#endif
    public static extern void MDataKeysForEachNative(
      IntPtr appPtr,
      ulong keysHandle,
      IntPtr self,
      MDataKeysForEachCb forEachCb,
      MDataKeysForEachResCb resCb);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataKeysForEachCb))]
#endif
    private static void OnMDataKeysForEachCb(IntPtr self, IntPtr bytePtr, IntPtr byteLen) {
      var cb = (MDataKeysForEachCb)self.HandlePtrToType<List<object>>(false)[0];
      cb(IntPtr.Zero, bytePtr, byteLen);
    }

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataKeysForEachResCb))]
#endif
    private static void OnMDataKeysForEachResCb(IntPtr self, FfiResult result) {
      var cb = (MDataKeysForEachResCb)self.HandlePtrToType<List<object>>()[1];
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region MDataKeysFree

    public void MDataKeysFree(IntPtr appPtr, ulong keysHandle, MDataKeysFreeCb callback) {
      MDataKeysFreeNative(appPtr, keysHandle, callback.ToHandlePtr(), OnMDataKeysFreeCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_keys_free")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_keys_free")]
#endif
    public static extern void MDataKeysFreeNative(IntPtr appPtr, ulong keysHandle, IntPtr self, MDataKeysFreeCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataKeysFreeCb))]
#endif
    private static void OnMDataKeysFreeCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<MDataKeysFreeCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region MDataKeysLen

    public void MDataKeysLen(IntPtr appPtr, ulong keysHandle, MDataKeysLenCb callback) {
      MDataKeysLenNative(appPtr, keysHandle, callback.ToHandlePtr(), OnMDataKeysLenCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_keys_len")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_keys_len")]
#endif
    public static extern void MDataKeysLenNative(IntPtr appPtr, ulong keysHandle, IntPtr self, MDataKeysLenCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataKeysLenCb))]
#endif
    private static void OnMDataKeysLenCb(IntPtr self, FfiResult result, IntPtr keysLen) {
      var cb = self.HandlePtrToType<MDataKeysLenCb>();
      cb(IntPtr.Zero, result, keysLen);
    }

    #endregion

    #region MDataListEntries

    public void MDataListEntries(IntPtr appPtr, ulong infoHandle, MDataListEntriesCb callback) {
      MDataListEntriesNative(appPtr, infoHandle, callback.ToHandlePtr(), OnMDataListEntriesCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_list_entries")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_list_entries")]
#endif
    public static extern void MDataListEntriesNative(IntPtr appPtr, ulong infoHandle, IntPtr self, MDataListEntriesCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataListEntriesCb))]
#endif
    private static void OnMDataListEntriesCb(IntPtr self, FfiResult result, ulong mDataEntriesHandle) {
      var cb = self.HandlePtrToType<MDataListEntriesCb>();
      cb(IntPtr.Zero, result, mDataEntriesHandle);
    }

    #endregion

    #region MDataListKeys

    public void MDataListKeys(IntPtr appPtr, ulong infoHandle, MDataListKeysCb callback) {
      MDataListKeysNative(appPtr, infoHandle, callback.ToHandlePtr(), OnMDataListKeysCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_list_keys")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_list_keys")]
#endif
    public static extern void MDataListKeysNative(IntPtr appPtr, ulong infoHandle, IntPtr self, MDataListKeysCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataListKeysCb))]
#endif
    private static void OnMDataListKeysCb(IntPtr self, FfiResult result, ulong keysHandle) {
      var cb = self.HandlePtrToType<MDataListKeysCb>();
      cb(IntPtr.Zero, result, keysHandle);
    }

    #endregion

    #region MDataMutateEntries

    public void MDataMutateEntries(IntPtr appPtr, ulong infoHandle, ulong actionsHandle, MDataMutateEntriesCb callback) {
      MDataMutateEntriesNative(appPtr, infoHandle, actionsHandle, callback.ToHandlePtr(), OnMDataMutateEntriesCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_mutate_entries")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_mutate_entries")]
#endif
    public static extern void MDataMutateEntriesNative(
      IntPtr appPtr,
      ulong infoHandle,
      ulong actionsHandle,
      IntPtr self,
      MDataMutateEntriesCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataMutateEntriesCb))]
#endif
    private static void OnMDataMutateEntriesCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<MDataMutateEntriesCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region MDataPermissionSetAllow

    public void MDataPermissionSetAllow(IntPtr appPtr, ulong setHandle, MDataAction action, MDataPermissionSetAllowCb callback) {
      MDataPermissionSetAllowNative(appPtr, setHandle, action, callback.ToHandlePtr(), OnMDataPermissionSetAllowCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_permission_set_allow")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_permission_set_allow")]
#endif
    public static extern void MDataPermissionSetAllowNative(
      IntPtr appPtr,
      ulong setHandle,
      MDataAction action,
      IntPtr self,
      MDataPermissionSetAllowCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataPermissionSetAllowCb))]
#endif
    private static void OnMDataPermissionSetAllowCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<MDataPermissionSetAllowCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region MDataPermissionSetFree

    public void MDataPermissionSetFree(IntPtr appPtr, ulong setHandle, MDataPermissionSetFreeCb callback) {
      MDataPermissionSetFreeNative(appPtr, setHandle, callback.ToHandlePtr(), OnMDataPermissionSetFreeCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_permission_set_free")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_permission_set_free")]
#endif
    public static extern void MDataPermissionSetFreeNative(IntPtr appPtr, ulong setHandle, IntPtr self, MDataPermissionSetFreeCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataPermissionSetFreeCb))]
#endif
    private static void OnMDataPermissionSetFreeCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<MDataPermissionSetFreeCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region MDataPermissionSetNew

    public void MDataPermissionSetNew(IntPtr appPtr, MDataPermissionSetNewCb callback) {
      MDataPermissionSetNewNative(appPtr, callback.ToHandlePtr(), OnMDataPermissionSetNewCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_permission_set_new")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_permission_set_new")]
#endif
    public static extern void MDataPermissionSetNewNative(IntPtr appPtr, IntPtr self, MDataPermissionSetNewCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataPermissionSetNewCb))]
#endif
    private static void OnMDataPermissionSetNewCb(IntPtr self, FfiResult result, ulong mDataPermissionSetHandle) {
      var cb = self.HandlePtrToType<MDataPermissionSetNewCb>();
      cb(IntPtr.Zero, result, mDataPermissionSetHandle);
    }

    #endregion

    #region MDataPermissionsFree

    public void MDataPermissionsFree(IntPtr appPtr, ulong permissionsHandle, MDataPermissionsFreeCb callback) {
      MDataPermissionsFreeNative(appPtr, permissionsHandle, callback.ToHandlePtr(), OnMDataPermissionsFreeCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_permissions_free")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_permissions_free")]
#endif
    public static extern void MDataPermissionsFreeNative(
      IntPtr appPtr,
      ulong permissionsHandle,
      IntPtr self,
      MDataPermissionsFreeCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataPermissionsFreeCb))]
#endif
    private static void OnMDataPermissionsFreeCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<MDataPermissionsFreeCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region MDataPermissionsInsert

    public void MDataPermissionsInsert(
      IntPtr appPtr,
      ulong permissionsHandle,
      ulong userHandle,
      ulong permissionSetHandle,
      MDataPermissionsInsertCb callback) {
      MDataPermissionsInsertNative(
        appPtr,
        permissionsHandle,
        userHandle,
        permissionSetHandle,
        callback.ToHandlePtr(),
        OnMDataPermissionsInsertCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_permissions_insert")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_permissions_insert")]
#endif
    public static extern void MDataPermissionsInsertNative(
      IntPtr appPtr,
      ulong permissionsHandle,
      ulong userHandle,
      ulong permissionSetHandle,
      IntPtr self,
      MDataPermissionsInsertCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataPermissionsInsertCb))]
#endif
    private static void OnMDataPermissionsInsertCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<MDataPermissionsInsertCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region MDataPermissionsNew

    public void MDataPermissionsNew(IntPtr appPtr, MDataPermissionsNewCb callback) {
      MDataPermissionsNewNative(appPtr, callback.ToHandlePtr(), OnMDataPermissionsNewCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_permissions_new")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_permissions_new")]
#endif
    public static extern void MDataPermissionsNewNative(IntPtr appPtr, IntPtr self, MDataPermissionsNewCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataPermissionsNewCb))]
#endif
    private static void OnMDataPermissionsNewCb(IntPtr self, FfiResult result, ulong mDataPermissionsHandle) {
      var cb = self.HandlePtrToType<MDataPermissionsNewCb>();
      cb(IntPtr.Zero, result, mDataPermissionsHandle);
    }

    #endregion

    #region MDataPut

    public void MDataPut(IntPtr appPtr, ulong infoHandle, ulong permissionsHandle, ulong entriesHandle, MDataPutCb callback) {
      MDataPutNative(appPtr, infoHandle, permissionsHandle, entriesHandle, callback.ToHandlePtr(), OnMDataPutCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "mdata_put")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "mdata_put")]
#endif
    public static extern void MDataPutNative(
      IntPtr appPtr,
      ulong infoHandle,
      ulong permissionsHandle,
      ulong entriesHandle,
      IntPtr self,
      MDataPutCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(MDataPutCb))]
#endif
    private static void OnMDataPutCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<MDataPutCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    #region Sha3Hash

    public void Sha3Hash(IntPtr data, IntPtr len, Sha3HashCb callback) {
      Sha3HashNative(data, len, callback.ToHandlePtr(), OnSha3HashCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "sha3_hash")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "sha3_hash")]
#endif
    public static extern void Sha3HashNative(IntPtr data, IntPtr len, IntPtr self, Sha3HashCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(Sha3HashCb))]
#endif
    private static void OnSha3HashCb(IntPtr self, FfiResult result, IntPtr digest, IntPtr len) {
      var cb = self.HandlePtrToType<Sha3HashCb>();
      cb(IntPtr.Zero, result, digest, len);
    }

    #endregion

    #region SignKeyFree

    public void SignKeyFree(IntPtr appPtr, ulong signKeyHandle, SignKeyFreeCb callback) {
      SignKeyFreeNative(appPtr, signKeyHandle, callback.ToHandlePtr(), OnSignKeyFreeCb);
    }

#if __IOS__
    [DllImport("__Internal", EntryPoint = "sign_key_free")]
#elif __ANDROID__
    [DllImport("safe_app", EntryPoint = "sign_key_free")]
#endif
    public static extern void SignKeyFreeNative(IntPtr appPtr, ulong signKeyHandle, IntPtr self, SignKeyFreeCb callback);

#if __IOS__
    [MonoPInvokeCallback(typeof(SignKeyFreeCb))]
#endif
    private static void OnSignKeyFreeCb(IntPtr self, FfiResult result) {
      var cb = self.HandlePtrToType<SignKeyFreeCb>();
      cb(IntPtr.Zero, result);
    }

    #endregion

    // ReSharper disable InconsistentNaming

    // ReSharper enable InconsistentNaming
  }
}

#endif
