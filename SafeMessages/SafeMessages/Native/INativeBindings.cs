using System;
using CommonUtils;

namespace SafeMessages.Native {
  #region Native Delegates

  public delegate void InitLoggingCb(IntPtr self, FfiResult result);

  public delegate void AppExeFileStemCb(IntPtr self, FfiResult result, string exeFileStem);

  public delegate void AppSetAdditionalSearchPathCb(IntPtr self, FfiResult result);

  public delegate void AppOutputLogPathCallback(IntPtr self, FfiResult result, string path);

  public delegate void MDataInfoDecryptCb(IntPtr self, FfiResult result, IntPtr plainText, IntPtr len);

  public delegate void NetworkObserverCb(IntPtr self, int errorCode, int eventType);

  public delegate void EncodeAuthReqCb(IntPtr self, FfiResult result, uint requestId, string encodedReq);

  public delegate void AccessContainerGetContainerMDataInfoCb(IntPtr self, FfiResult result, ulong mDataInfoHandle);

  public delegate void AppPubSignKeyCb(IntPtr self, FfiResult result, ulong signKeyHandle);

  public delegate void CipherOptFreeCb(IntPtr self, FfiResult result);

  public delegate void CipherOptNewPlaintextCb(IntPtr self, FfiResult result, ulong cipherOptHandle);

  public delegate void DecryptSealedBoxCb(IntPtr self, FfiResult result, IntPtr data, IntPtr dataLen);

  public delegate void EncGenerateKeyPairCb(IntPtr self, FfiResult result, ulong encPubKeyHandle, ulong encSecKeyHandle);

  public delegate void EncPubKeyFreeCb(IntPtr self, FfiResult result);

  public delegate void EncPubKeyGetCb(IntPtr self, FfiResult result, IntPtr asymPublicKey);

  public delegate void EncPubKeyNewCb(IntPtr self, FfiResult result, ulong encryptPubKeyHandle);

  public delegate void EncryptSealedBoxCb(IntPtr self, FfiResult result, IntPtr data, IntPtr dataLen);

  public delegate void EncSecretKeyFreeCb(IntPtr self, FfiResult result);

  public delegate void EncSecretKeyGetCb(IntPtr self, FfiResult result, IntPtr asymSecretKey);

  public delegate void EncSecretKeyNewCb(IntPtr self, FfiResult result, ulong encryptSecKeyHandle);

  // ReSharper disable InconsistentNaming - IData
  public delegate void IDataCloseSelfEncryptorCb(IntPtr self, FfiResult result, IntPtr xorNameArr);

  public delegate void IDataFetchSelfEncryptorCb(IntPtr self, FfiResult result, ulong sEReaderHandle);

  public delegate void IDataNewSelfEncryptorCb(IntPtr self, FfiResult result, ulong sEWriterHandle);

  public delegate void IDataReadFromSelfEncryptorCb(IntPtr self, FfiResult result, IntPtr data, IntPtr dataLen);

  public delegate void IDataSelfEncryptorReaderFreeCb(IntPtr self, FfiResult result);

  public delegate void IDataSelfEncryptorWriterFreeCb(IntPtr self, FfiResult result);

  public delegate void IDataSizeCb(IntPtr self, FfiResult result, ulong len);

  public delegate void IDataWriteToSelfEncryptorCb(IntPtr self, FfiResult result);
  // ReSharper restore InconsistentNaming

  public delegate void MDataEntriesForEachCb(
    IntPtr self,
    IntPtr entryKey,
    IntPtr entryKeyLen,
    IntPtr entryVal,
    IntPtr entryValLen,
    ulong entryVersion);

  public delegate void MDataEntriesForEachResCb(IntPtr self, FfiResult result);

  public delegate void MDataEntriesFreeCb(IntPtr self, FfiResult result);

  public delegate void MDataEntriesInsertCb(IntPtr self, FfiResult result);

  public delegate void MDataEntriesLenCb(IntPtr self, ulong len);

  public delegate void MDataEntriesNewCb(IntPtr self, FfiResult result, ulong mDataEntriesHandle);

  public delegate void MDataEntryActionsFreeCb(IntPtr self, FfiResult result);

  public delegate void MDataEntryActionsInsertCb(IntPtr self, FfiResult result);

  public delegate void MDataEntryActionsNewCb(IntPtr self, FfiResult result, ulong mDataEntryActionsHandle);

  public delegate void MDataGetValueCb(IntPtr self, FfiResult result, IntPtr data, IntPtr dataLen, ulong entryVersion);

  public delegate void MDataInfoDeserialiseCb(IntPtr self, FfiResult result, ulong mDataInfoHandle);

  public delegate void MDataInfoFreeCb(IntPtr self, FfiResult result);

  public delegate void MDataInfoNewPublicCb(IntPtr self, FfiResult result, ulong mDataInfoHandle);

  public delegate void MDataInfoRandomPrivateCb(IntPtr self, FfiResult result, ulong mDataInfoHandle);

  public delegate void MDataInfoRandomPublicCb(IntPtr self, FfiResult result, ulong mDataInfoHandle);

  public delegate void MDataInfoSerialiseCb(IntPtr self, FfiResult result, IntPtr data, IntPtr dataLen);

  public delegate void MDataListEntriesCb(IntPtr self, FfiResult result, ulong mDataEntriesHandle);

  public delegate void MDataMutateEntriesCb(IntPtr self, FfiResult result);

  public delegate void MDataPermissionSetNewCb(IntPtr self, FfiResult result, ulong mDataPermissionSetHandle);

  public delegate void MDataPermissionsFreeCb(IntPtr self, FfiResult result);

  public delegate void MDataPermissionsInsertCb(IntPtr self, FfiResult result);

  public delegate void MDataPermissionsNewCb(IntPtr self, FfiResult result, ulong mDataPermissionsHandle);

  public delegate void MDataPermissionSetAllowCb(IntPtr self, FfiResult result);

  public delegate void MDataPermissionSetFreeCb(IntPtr self, FfiResult result);

  public delegate void MDataPutCb(IntPtr self, FfiResult result);

  public delegate void Sha3HashCb(IntPtr self, FfiResult result, IntPtr digest, IntPtr len);

  public delegate void MDataListKeysCb(IntPtr self, FfiResult result, ulong keysHandle);

  public delegate void MDataKeysLenCb(IntPtr self, FfiResult result, IntPtr keysLen);

  public delegate void MDataKeysForEachCb(IntPtr self, IntPtr bytePtr, IntPtr byteLen);

  public delegate void MDataKeysForEachResCb(IntPtr self, FfiResult result);

  public delegate void MDataKeysFreeCb(IntPtr self, FfiResult result);

  public delegate void DecodeAuthCb(IntPtr self, uint reqId, IntPtr authGrantedFfiPtr);

  public delegate void DecodeUnregCb(IntPtr self, uint reqId, IntPtr bsConfig, IntPtr bsSize);

  public delegate void DecodeContCb(IntPtr self, uint reqId);

  public delegate void DecodeShareMDataCb(IntPtr self, uint reqId);

  public delegate void DecodeRevokedCb(IntPtr self);

  public delegate void DecodeErrorCb(IntPtr self, FfiResult result);

  public delegate void NetObsCb(IntPtr self, FfiResult result, int eventType);

  public delegate void AppRegisteredCb(IntPtr self, FfiResult result, IntPtr appPtr);

  public delegate void MDataInfoEncryptEntryKeyCb(IntPtr self, FfiResult result, IntPtr dataPtr, IntPtr dataLen);

  public delegate void MDataInfoEncryptEntryValueCb(IntPtr self, FfiResult result, IntPtr dataPtr, IntPtr dataLen);

  public delegate void SignKeyFreeCb(IntPtr self, FfiResult result);

  #endregion

  public interface INativeBindings {
    void AccessContainerGetContainerMDataInfo(IntPtr appPtr, string name, AccessContainerGetContainerMDataInfoCb callback);
    void AppExeFileStem(AppExeFileStemCb callback);
    void AppInitLogging(string fileName, InitLoggingCb callback);
    void AppOutputLogPath(string fileName, AppOutputLogPathCallback callback);
    void AppPubSignKey(IntPtr appPtr, AppPubSignKeyCb callback);

    void AppRegistered(string appId, IntPtr authGrantedFfiPtr, NetObsCb netObsCb, AppRegisteredCb appRegCb);

    void AppSetAdditionalSearchPath(string path, AppSetAdditionalSearchPathCb callback);
    void CipherOptFree(IntPtr appPtr, ulong cipherOptHandle, CipherOptFreeCb callback);
    void CipherOptNewPlaintext(IntPtr appPtr, CipherOptNewPlaintextCb callback);

    void DecodeIpcMessage(
      string encodedReq,
      DecodeAuthCb authCb,
      DecodeUnregCb unregCb,
      DecodeContCb contCb,
      DecodeShareMDataCb shareMDataCb,
      DecodeRevokedCb revokedCb,
      DecodeErrorCb errorCb);

    void DecryptSealedBox(IntPtr appPtr, IntPtr data, IntPtr len, ulong pkHandle, ulong skHandle, DecryptSealedBoxCb callback);
    void EncGenerateKeyPair(IntPtr appPtr, EncGenerateKeyPairCb callback);
    void EncodeAuthReq(IntPtr authReq, EncodeAuthReqCb callback);
    void EncPubKeyFree(IntPtr appPtr, ulong encryptPubKeyHandle, EncPubKeyFreeCb callback);
    void EncPubKeyGet(IntPtr appPtr, ulong encryptPubKeyHandle, EncPubKeyGetCb callback);
    void EncPubKeyNew(IntPtr appPtr, IntPtr asymPublicKey, EncPubKeyNewCb callback);
    void EncryptSealedBox(IntPtr appPtr, IntPtr data, IntPtr len, ulong pkHandle, EncryptSealedBoxCb callback);
    void EncSecretKeyFree(IntPtr appPtr, ulong encryptSecKeyHandle, EncSecretKeyFreeCb callback);
    void EncSecretKeyGet(IntPtr appPtr, ulong encryptSecKeyHandle, EncSecretKeyGetCb callback);
    void EncSecretKeyNew(IntPtr appPtr, IntPtr asymSecretKey, EncSecretKeyNewCb callback);
    void FreeApp(IntPtr appPtr);

    void MDataEntriesForEach(
      IntPtr appPtr,
      ulong entriesHandle,
      MDataEntriesForEachCb forEachCallback,
      MDataEntriesForEachResCb resultCallback);

    void MDataEntriesFree(IntPtr appPtr, ulong entriesHandle, MDataEntriesFreeCb callback);

    void MDataEntriesInsert(
      IntPtr appPtr,
      ulong entriesHandle,
      IntPtr keyPtr,
      IntPtr keyLen,
      IntPtr valuePtr,
      IntPtr valueLen,
      MDataEntriesInsertCb callback);

    void MDataEntriesLen(IntPtr appPtr, ulong entriesHandle, MDataEntriesLenCb callback);
    void MDataEntriesNew(IntPtr appPtr, MDataEntriesNewCb callback);
    void MDataEntryActionsFree(IntPtr appPtr, ulong actionsHandle, MDataEntryActionsFreeCb callback);

    void MDataEntryActionsInsert(
      IntPtr appPtr,
      ulong actionsHandle,
      IntPtr keyPtr,
      IntPtr keyLen,
      IntPtr valuePtr,
      IntPtr valueLen,
      MDataEntryActionsInsertCb callback);

    void MDataEntryActionsNew(IntPtr appPtr, MDataEntryActionsNewCb callback);
    void MDataGetValue(IntPtr appPtr, ulong infoHandle, IntPtr keyPtr, IntPtr keyLen, MDataGetValueCb callback);
    void MDataInfoDecrypt(IntPtr appPtr, ulong mDataInfoH, IntPtr cipherText, IntPtr cipherLen, MDataInfoDecryptCb callback);
    void MDataInfoDeserialise(IntPtr appPtr, IntPtr ptr, IntPtr len, MDataInfoDeserialiseCb callback);

    void MDataInfoEncryptEntryKey(IntPtr appPtr, ulong infoH, IntPtr inputPtr, IntPtr inputLen, MDataInfoEncryptEntryKeyCb callback);

    void MDataInfoEncryptEntryValue(IntPtr appPtr, ulong infoH, IntPtr inputPtr, IntPtr inputLen, MDataInfoEncryptEntryValueCb callback);

    void MDataInfoFree(IntPtr appPtr, ulong infoHandle, MDataInfoFreeCb callback);
    void MDataInfoNewPublic(IntPtr appPtr, IntPtr xorNameArr, ulong typeTag, MDataInfoNewPublicCb callback);
    void MDataInfoRandomPrivate(IntPtr appPtr, ulong typeTag, MDataInfoRandomPrivateCb callback);
    void MDataInfoRandomPublic(IntPtr appPtr, ulong typeTag, MDataInfoRandomPublicCb callback);
    void MDataInfoSerialise(IntPtr appPtr, ulong infoHandle, MDataInfoSerialiseCb callback);
    void MDataKeysForEach(IntPtr appPtr, ulong keysHandle, MDataKeysForEachCb forEachCb, MDataKeysForEachResCb resCb);

    void MDataKeysFree(IntPtr appPtr, ulong keysHandle, MDataKeysFreeCb callback);

    void MDataKeysLen(IntPtr appPtr, ulong keysHandle, MDataKeysLenCb callback);
    void MDataListEntries(IntPtr appPtr, ulong infoHandle, MDataListEntriesCb callback);
    void MDataListKeys(IntPtr appPtr, ulong infoHandle, MDataListKeysCb callback);
    void MDataMutateEntries(IntPtr appPtr, ulong infoHandle, ulong actionsHandle, MDataMutateEntriesCb callback);
    void MDataPermissionSetAllow(IntPtr appPtr, ulong setHandle, MDataAction action, MDataPermissionSetAllowCb callback);
    void MDataPermissionSetFree(IntPtr appPtr, ulong setHandle, MDataPermissionSetFreeCb callback);
    void MDataPermissionSetNew(IntPtr appPtr, MDataPermissionSetNewCb callback);
    void MDataPermissionsFree(IntPtr appPtr, ulong permissionsHandle, MDataPermissionsFreeCb callback);

    void MDataPermissionsInsert(
      IntPtr appPtr,
      ulong permissionsHandle,
      ulong userHandle,
      ulong permissionSetHandle,
      MDataPermissionsInsertCb callback);

    void MDataPermissionsNew(IntPtr appPtr, MDataPermissionsNewCb callback);
    void MDataPut(IntPtr appPtr, ulong infoHandle, ulong permissionsHandle, ulong entriesHandle, MDataPutCb callback);
    void Sha3Hash(IntPtr data, IntPtr len, Sha3HashCb callback);
    void SignKeyFree(IntPtr appPtr, ulong signKeyHandle, SignKeyFreeCb callback);

    // ReSharper disable InconsistentNaming
    void IDataCloseSelfEncryptor(IntPtr appPtr, ulong seH, ulong cipherOptH, IDataCloseSelfEncryptorCb callback);

    void IDataFetchSelfEncryptor(IntPtr appPtr, IntPtr xorNameArr, IDataFetchSelfEncryptorCb callback);
    void IDataNewSelfEncryptor(IntPtr appPtr, IDataNewSelfEncryptorCb callback);

    void IDataReadFromSelfEncryptor(IntPtr appPtr, ulong seHandle, ulong fromPos, ulong len, IDataReadFromSelfEncryptorCb callback);

    void IDataSelfEncryptorReaderFree(IntPtr appPtr, ulong sEReaderHandle, IDataSelfEncryptorReaderFreeCb callback);
    void IDataSelfEncryptorWriterFree(IntPtr appPtr, ulong sEWriterHandle, IDataSelfEncryptorWriterFreeCb callback);
    void IDataSize(IntPtr appPtr, ulong seHandle, IDataSizeCb callback);

    void IDataWriteToSelfEncryptor(IntPtr appPtr, ulong seHandle, IntPtr data, IntPtr size, IDataWriteToSelfEncryptorCb callback);
    // ReSharper restore InconsistentNaming
  }
}
