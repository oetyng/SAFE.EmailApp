using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtils;
using Newtonsoft.Json;
using SafeMessages.Helpers;
using SafeMessages.Models;
using SafeMessages.Native.App;
using SafeMessages.Native.IData;
using SafeMessages.Native.MData;
using SafeMessages.Native.Misc;
using SafeMessages.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppService))]

namespace SafeMessages.Services {
  public class AppService : ObservableObject, IDisposable {
    public const string AuthDeniedMessage = "Failed to receive Authentication.";
    public const string AuthInProgressMessage = "Connecting to SAFE Network...";
    private string _appId;

    private bool _isLogInitialised;

    private string AppId {
      get {
        if (_appId == string.Empty) {
          throw new ArgumentNullException();
        }
        return _appId;
      }
      set => _appId = value;
    }

    public UserId Self { get; set; }

    public bool IsLogInitialised { get => _isLogInitialised; set => SetProperty(ref _isLogInitialised, value); }

    public AppService() {
      _isLogInitialised = true;
      InitLoggingAsync();
    }

    public void Dispose() {
      FreeState();
      GC.SuppressFinalize(this);
    }

    public async Task AddIdAsync(string userId) {
      if (userId.Contains(".") || userId.Contains("@")) {
        throw new NotSupportedException("Unsupported characters '.' and '@'.");
      }
      
      // Check if account exits first and return error
      var dstPubIdDigest = await NativeUtils.Sha3HashAsync(userId.ToUtfBytes());
      var dstPubIdMDataInfoH = await MDataInfo.MDataInfoNewPublicAsync(dstPubIdDigest, 15001);
      var accountExists = false;
      try {
        await MData.MDataListKeysAsync(dstPubIdMDataInfoH);
        accountExists = true;
      } catch (Exception) {
        // ignored
      }
      if (accountExists) {
        throw new Exception("Id already exists.");
      }

      // Create Self Permissions to Inbox and Archive
      var inboxSelfPermSetH = await MDataPermissions.MDataPermissionSetNewAsync();
      await MDataPermissions.MDataPermissionsSetAllowAsync(inboxSelfPermSetH, MDataAction.kInsert);
      await MDataPermissions.MDataPermissionsSetAllowAsync(inboxSelfPermSetH, MDataAction.kUpdate);
      await MDataPermissions.MDataPermissionsSetAllowAsync(inboxSelfPermSetH, MDataAction.kDelete);
      await MDataPermissions.MDataPermissionsSetAllowAsync(inboxSelfPermSetH, MDataAction.kManagePermissions);

      var appSignPk = await Crypto.AppPubSignKeyAsync();
      var inboxPermH = await MDataPermissions.MDataPermissionsNewAsync();
      await MDataPermissions.MDataPermissionsInsertAsync(inboxPermH, appSignPk, inboxSelfPermSetH);

      // Create Archive MD
      var archiveMDataInfoH = await MDataInfo.MDataInfoRandomPrivateAsync(15001);
      await MData.MDataPutAsync(archiveMDataInfoH, inboxPermH, 0);

      // Update Inbox permisions to allow anyone to insert
      var inboxAnyonePermSetH = await MDataPermissions.MDataPermissionSetNewAsync();
      await MDataPermissions.MDataPermissionsSetAllowAsync(inboxAnyonePermSetH, MDataAction.kInsert);
      await MDataPermissions.MDataPermissionsInsertAsync(inboxPermH, Session.UserAnyoneSignPk, inboxAnyonePermSetH);

      // Create Inbox MD
      var inboxEncKeysH = await Crypto.EncGenerateKeyPairAsync();
      var inboxEncPk = await Crypto.EncPubKeyGetAsync(inboxEncKeysH.Item1);
      var inboxEmailPkEntryKey = "__email_enc_pk".ToUtfBytes();
      var inboxEntriesH = await MDataEntries.MDataEntriesNewAsync();
      await MDataEntries.MDataEntriesInsertAsync(inboxEntriesH, inboxEmailPkEntryKey, inboxEncPk.ToHexString().ToUtfBytes());
      var inboxMDataInfoH = await MDataInfo.MDataInfoRandomPublicAsync(15001);
      await MData.MDataPutAsync(inboxMDataInfoH, inboxPermH, inboxEntriesH);
      
      var serInboxMdInfo = await MDataInfo.MDataInfoSerialiseAsync(inboxMDataInfoH);
      var serArchiveMdInfo = await MDataInfo.MDataInfoSerialiseAsync(archiveMDataInfoH);

      // Create Public Id MD
      var idDigest = await NativeUtils.Sha3HashAsync(userId.ToUtfBytes());
      var pubIdMDataInfoH = await MDataInfo.MDataInfoNewPublicAsync(idDigest, 15001);
      var pubIdEntriesH = await MDataEntries.MDataEntriesNewAsync();
      await MDataEntries.MDataEntriesInsertAsync(pubIdEntriesH, "@email".ToUtfBytes(), serInboxMdInfo);
      await MData.MDataPutAsync(pubIdMDataInfoH, inboxPermH, pubIdEntriesH);

      // Update _publicNames Container
      var publicNamesContH = await AccessContainer.GetContainerMDataInfoAsync("_publicNames");
      var pubNamesUserIdCipherBytes = await MDataInfo.MDataInfoEncryptEntryKeyAsync(publicNamesContH, userId.ToUtfBytes());
      var pubNamesMsgBoxCipherBytes = await MDataInfo.MDataInfoEncryptEntryValueAsync(publicNamesContH, idDigest);
      var pubNamesContEntActH = await MDataEntries.MDataEntryActionsNewAsync();
      await MDataEntries.MDataEntryActionsInsertAsync(pubNamesContEntActH, pubNamesUserIdCipherBytes, pubNamesMsgBoxCipherBytes);
      await MData.MDataMutateEntriesAsync(publicNamesContH, pubNamesContEntActH);

      // Finally update App Container
      var inboxEncSk = await Crypto.EncSecretKeyGetAsync(inboxEncKeysH.Item2);
      var msgBox = new MessageBox
      {
        EmailId = userId,
        Inbox = new DataArray { Type = "Buffer", Data = serInboxMdInfo },
        Archive = new DataArray { Type = "Buffer", Data = serArchiveMdInfo },
        EmailEncPk = inboxEncPk.ToHexString(),
        EmailEncSk = inboxEncSk.ToHexString()
      };

      var msgBoxSer = JsonConvert.SerializeObject(msgBox);
      var appContH = await AccessContainer.GetContainerMDataInfoAsync("apps/" + AppId);
      var userIdCipherBytes = await MDataInfo.MDataInfoEncryptEntryKeyAsync(appContH, userId.ToUtfBytes());
      var msgBoxCipherBytes = await MDataInfo.MDataInfoEncryptEntryValueAsync(appContH, msgBoxSer.ToUtfBytes());
      var appContEntActH = await MDataEntries.MDataEntryActionsNewAsync();
      await MDataEntries.MDataEntryActionsInsertAsync(appContEntActH, userIdCipherBytes, msgBoxCipherBytes);
      await MData.MDataMutateEntriesAsync(appContH, appContEntActH);

      // Free Native Handles
      var taskHandles = new List<Task> {
        MDataPermissions.MDataPermissionsSetFreeAsync(inboxSelfPermSetH),
        MDataPermissions.MDataPermissionsSetFreeAsync(inboxAnyonePermSetH),
        MDataEntries.MDataEntriesFreeAsync(inboxEntriesH),
        MDataEntries.MDataEntriesFreeAsync(pubIdEntriesH),
        Crypto.EncPubKeyFreeAsync(inboxEncKeysH.Item1),
        Crypto.EncSecretKeyFreeAsync(inboxEncKeysH.Item2),
        MDataEntries.MDataEntryActionsFreeAsync(appContEntActH),
        MDataEntries.MDataEntryActionsFreeAsync(pubNamesContEntActH),
        MDataInfo.MDataInfoFreeAsync(archiveMDataInfoH),
        MDataInfo.MDataInfoFreeAsync(inboxMDataInfoH),
        MDataInfo.MDataInfoFreeAsync(appContH),
        MDataInfo.MDataInfoFreeAsync(pubIdMDataInfoH),
        MDataInfo.MDataInfoFreeAsync(publicNamesContH)
      };

      await Task.WhenAll(taskHandles);
    }

    ~AppService() {
      FreeState();
    }

    public void FreeState() {
      Session.FreeApp();
    }

    public async Task<string> GenerateAppRequestAsync() {
      AppId = "net.maidsafe.examples.mailtutorial";
      var authReq = new AuthReq {
        AppContainer = true,
        AppExchangeInfo = new AppExchangeInfo {Id = AppId, Scope = "", Name = "SAFE Messages", Vendor = "MaidSafe.net"},
        Containers = new List<ContainerPermissions> {new ContainerPermissions {ContainerName = "_publicNames", Access = {Insert = true}}}
      };

      var encodedReq = await Session.EncodeAuthReqAsync(authReq);
      var formattedReq = UrlFormat.Convert(encodedReq, false);
      Debug.WriteLine($"Encoded Req: {formattedReq}");
      return formattedReq;
    }

    public async Task<List<UserId>> GetIdsAsync() {
      var appContH = await AccessContainer.GetContainerMDataInfoAsync("apps/" + AppId);
      var appContEntKeysH = await MData.MDataListKeysAsync(appContH);
      var cipherTextEntKeys = await MDataEntries.MDataKeysForEachAsync(appContEntKeysH);

      var ids = new List<UserId>();
      foreach (var cipherTextEntKey in cipherTextEntKeys) {
        try {
          var plainTextEntKey = await MDataInfo.MDataInfoDecryptAsync(appContH, cipherTextEntKey);
          ids.Add(new UserId(plainTextEntKey.ToUtfString()));
        } catch (Exception) {
          // We're ignoring any entries we cannot parse just so we can work with the valid entries.
          // ignored
        }
      }

      // Free the handles
      await Task.WhenAll(MDataEntries.MDataKeysFreeAsync(appContEntKeysH), MDataInfo.MDataInfoFreeAsync(appContH));

      return ids;
    }

    public async Task<List<Message>> GetMessagesAsync(string userId) {
      var appContH = await AccessContainer.GetContainerMDataInfoAsync("apps/" + AppId);
      var userIdByteList = userId.ToUtfBytes();
      var cipherBytes = await MDataInfo.MDataInfoEncryptEntryKeyAsync(appContH, userIdByteList);
      var content = await MData.MDataGetValueAsync(appContH, cipherBytes);
      var plainBytes = await MDataInfo.MDataInfoDecryptAsync(appContH, content.Item1);
      var msgBox = JsonConvert.DeserializeObject<MessageBox>(plainBytes.ToUtfString());
      var inboxPkH = await Crypto.EncPubKeyNewAsync(msgBox.EmailEncPk.ToHexBytes());
      var inboxSkH = await Crypto.EncSecretKeyNewAsync(msgBox.EmailEncSk.ToHexBytes());
      var inboxInfoH = await MDataInfo.MDataInfoDeserialiseAsync(msgBox.Inbox.Data);
      var inboxEntH = await MData.MDataListEntriesAsync(inboxInfoH);
      var cipherTextEntries = await MDataEntries.MDataEntriesForEachAsync(inboxEntH);

      var messages = new List<Message>();
      var taskHandles = new List<Task>();
      foreach (var entry in cipherTextEntries) {
        try {
          var entryKey = entry.Item1.ToUtfString();
          if (entryKey == "__email_enc_pk") {
            continue;
          }

          var iDataNameEncoded = await Crypto.DecryptSealedBoxAsync(entry.Item2, inboxPkH, inboxSkH);
          var iDataNameBytes = iDataNameEncoded.ToUtfString().Split(',').Select(val => Convert.ToByte(val)).ToList();
          var msgDataReaderH = await IData.IDataFetchSelfEncryptorAsync(iDataNameBytes);
          var msgSize = await IData.IDataSizeAsync(msgDataReaderH);
          var msgCipher = await IData.IDataReadFromSelfEncryptorAsync(msgDataReaderH, 0, msgSize);
          var plainTextMsg = await Crypto.DecryptSealedBoxAsync(msgCipher, inboxPkH, inboxSkH);
          var jsonMsgData = plainTextMsg.ToUtfString();
          messages.Add(JsonConvert.DeserializeObject<Message>(jsonMsgData));
          taskHandles.Add(IData.IDataSelfEncryptorReaderFreeAsync(msgDataReaderH));
        } catch (Exception e) {
          Debug.WriteLine("Exception: " + e.Message);
        }
      }

      taskHandles.Add(MDataInfo.MDataInfoFreeAsync(appContH));
      taskHandles.Add(MDataInfo.MDataInfoFreeAsync(inboxInfoH));
      taskHandles.Add(Crypto.EncPubKeyFreeAsync(inboxPkH));
      taskHandles.Add(Crypto.EncSecretKeyFreeAsync(inboxSkH));
      taskHandles.Add(MDataEntries.MDataEntriesFreeAsync(inboxEntH));
      await Task.WhenAll(taskHandles);

      return messages;
    }

    public async Task HandleUrlActivationAsync(string encodedUrl) {
      try {
        var formattedUrl = UrlFormat.Convert(encodedUrl, true);
        var decodeResult = await Session.DecodeIpcMessageAsync(formattedUrl);
        if (decodeResult.AuthGranted.HasValue) {
          var authGranted = decodeResult.AuthGranted.Value;
          Debug.WriteLine("Received Auth Granted from Authenticator");
          // update auth progress message
          MessagingCenter.Send(this, MessengerConstants.AuthRequestProgress, AuthInProgressMessage);
          await Session.AppRegisteredAsync(AppId, authGranted);
          MessagingCenter.Send(this, MessengerConstants.AuthRequestProgress, string.Empty);
        } else {
          Debug.WriteLine("Decoded Req is not Auth Granted");
        }
      } catch (Exception ex) {
        await Application.Current.MainPage.DisplayAlert("Error", $"Description: {ex.Message}", "OK");
        MessagingCenter.Send(this, MessengerConstants.AuthRequestProgress, AuthDeniedMessage);
      }
    }

    private async void InitLoggingAsync() {
      var started = await Session.InitLoggingAsync();
      if (!started) {
        Debug.WriteLine("Unable to Initialise Logging.");
        return;
      }

      Debug.WriteLine("Rust Logging Initialised.");
      IsLogInitialised = true;
    }

    public async Task SendMessageAsync(string to, Message msg) {
      var dstPubIdDigest = await NativeUtils.Sha3HashAsync(to.ToUtfBytes());
      var dstPubIdMDataInfoH = await MDataInfo.MDataInfoNewPublicAsync(dstPubIdDigest, 15001);
      var serDstMsgMDataInfo = await MData.MDataGetValueAsync(dstPubIdMDataInfoH, "@email".ToUtfBytes());
      var dstInboxMDataInfoH = await MDataInfo.MDataInfoDeserialiseAsync(serDstMsgMDataInfo.Item1);
      var dstInboxPkItem = await MData.MDataGetValueAsync(dstInboxMDataInfoH, "__email_enc_pk".ToUtfBytes());
      var dstInboxPkH = await Crypto.EncPubKeyNewAsync(dstInboxPkItem.Item1.ToUtfString().ToHexBytes());
      var plainTxtMsg = JsonConvert.SerializeObject(msg);
      var cipherTxt = await Crypto.EncryptSealedBoxAsync(plainTxtMsg.ToUtfBytes(), dstInboxPkH);
      var msgWriterH = await IData.IDataNewSelfEncryptorAsync();
      await IData.IDataWriteToSelfEncryptorAsync(msgWriterH, cipherTxt);
      var msgCipherOptH = await CipherOpt.CipherOptNewPlaintextAsync();
      var msgDataMapNameBytes = await IData.IDataCloseSelfEncryptorAsync(msgWriterH, msgCipherOptH);
      var sb = new StringBuilder("");
      for (var i = 0; i < msgDataMapNameBytes.Count - 1; ++i) {
        sb.Append(msgDataMapNameBytes[i] + ", ");
      }
      sb.Append(msgDataMapNameBytes[msgDataMapNameBytes.Count - 1]);
      Debug.WriteLine(sb.ToString());
      var msgDataMapNameCipher = await Crypto.EncryptSealedBoxAsync(sb.ToString().ToUtfBytes(), dstInboxPkH);
      var dstMsgEntActH = await MDataEntries.MDataEntryActionsNewAsync();
      await MDataEntries.MDataEntryActionsInsertAsync(dstMsgEntActH, Mock.RandomString(15).ToUtfBytes(), msgDataMapNameCipher);
      await MData.MDataMutateEntriesAsync(dstInboxMDataInfoH, dstMsgEntActH);

      await MDataEntries.MDataEntryActionsFreeAsync(dstMsgEntActH);
      await MDataInfo.MDataInfoFreeAsync(dstPubIdMDataInfoH);
      await MDataInfo.MDataInfoFreeAsync(dstInboxMDataInfoH);
      await Crypto.EncPubKeyFreeAsync(dstInboxPkH);
      await CipherOpt.CipherOptFreeAsync(msgCipherOptH);
    }
  }
}
