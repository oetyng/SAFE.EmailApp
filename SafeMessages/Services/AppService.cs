using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Newtonsoft.Json;
using SafeApp;
using SafeApp.Misc;
using SafeApp.Utilities;
using SafeMessages.Helpers;
using SafeMessages.Models;
using SafeMessages.Models.BaseModel;
using SafeMessages.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppService))]

namespace SafeMessages.Services
{
    public class AppService : ObservableObject, IDisposable
    {
        public const string AppId = "net.maidsafe.examples.mailtutorial";
        public const string AuthDeniedMessage = "Failed to receive Authentication.";
        public const string AuthInProgressMessage = "Connecting to SAFE Network...";
        private const string AuthReconnectPropKey = nameof(AuthReconnect);
        private bool _isLogInitialised;
        private Session _session;

        public AppService()
        {
            _isLogInitialised = false;
            CredentialCache = new CredentialCacheService();
            Session.Disconnected += OnSessionDisconnected;
            InitLoggingAsync();
        }

        public UserId Self { get; set; }

        private CredentialCacheService CredentialCache { get; }

        public bool IsLogInitialised
        {
            get => _isLogInitialised;
            set => SetProperty(ref _isLogInitialised, value);
        }

        public bool AuthReconnect
        {
            get
            {
                if (!Application.Current.Properties.ContainsKey(AuthReconnectPropKey))
                {
                    return false;
                }

                var val = Application.Current.Properties[AuthReconnectPropKey] as bool?;
                return val == true;
            }

            set
            {
                if (value == false)
                {
                    CredentialCache.Delete();
                }

                Application.Current.Properties[AuthReconnectPropKey] = value;
            }
        }

        public void Dispose()
        {
            FreeState();
            GC.SuppressFinalize(this);
        }

        public async Task AddIdAsync(string userId)
        {
            if (userId.Contains(".") || userId.Contains("@"))
            {
                throw new NotSupportedException("Unsupported characters '.' and '@'.");
            }

            // Check if account exits first and return error
            var dstPubIdDigest = await Crypto.Sha3HashAsync(userId.ToUtfBytes());
            var dstPubIdMDataInfoH = new MDataInfo { Name = dstPubIdDigest.ToArray(), TypeTag = 15001 };
            var accountExists = false;
            try
            {
                await _session.MData.ListKeysAsync(dstPubIdMDataInfoH);
                accountExists = true;
            }
            catch (Exception)
            {
                // ignored - acct not found
            }

            if (accountExists)
            {
                throw new Exception("Id already exists.");
            }

            // Create Self Permissions to Inbox and Archive
            using (var inboxSelfPermSetH = await _session.MDataPermissions.NewAsync())
            {
                using (var inboxPermH = await _session.MDataPermissions.NewAsync())
                {
                    using (var appSignPkH = await _session.Crypto.AppPubSignKeyAsync())
                    {
                        await _session.MDataPermissions.InsertAsync(
                            inboxSelfPermSetH,
                            appSignPkH,
                            new PermissionSet
                            { Delete = true, Insert = true, ManagePermissions = true, Read = true, Update = true });
                    }

                    // Create Archive MD
                    var archiveMDataInfoH = await _session.MDataInfoActions.RandomPrivateAsync(15001);
                    await _session.MData.PutAsync(archiveMDataInfoH, inboxPermH, NativeHandle.EmptyMDataEntries);
                    var serArchiveMdInfo = await _session.MDataInfoActions.SerialiseAsync(archiveMDataInfoH);

                    // Update Inbox permisions to allow anyone to insert
                    await _session.MDataPermissions.InsertAsync(inboxPermH, NativeHandle.EmptyMDataEntries, new PermissionSet { Insert = true });

                    // Create Inbox MD
                    var (inboxEncPk, inboxEncSk) = await _session.Crypto.EncGenerateKeyPairAsync();
                    using (inboxEncSk)
                    using (inboxEncPk)
                    {
                        List<byte> serInboxMdInfo;
                        var inboxEncPkRaw = await _session.Crypto.EncPubKeyGetAsync(inboxEncPk);
                        var inboxEncSkRaw = await _session.Crypto.EncSecretKeyGetAsync(inboxEncSk);

                        var inboxEmailPkEntryKey = "__email_enc_pk".ToUtfBytes();
                        using (var inboxEntriesH = await _session.MDataEntries.NewAsync())
                        {
                            await _session.MDataEntries.InsertAsync(
                                inboxEntriesH,
                                inboxEmailPkEntryKey,
                                inboxEncPkRaw.ToList().ToHexString().ToUtfBytes());
                            var inboxMDataInfoH = await _session.MDataInfoActions.RandomPublicAsync(15001);
                            await _session.MData.PutAsync(inboxMDataInfoH, inboxPermH, inboxEntriesH);
                            serInboxMdInfo = await _session.MDataInfoActions.SerialiseAsync(inboxMDataInfoH);
                        }

                        // Create Public Id MD
                        var idDigest = await Crypto.Sha3HashAsync(userId.ToUtfBytes());
                        var pubIdMDataInfoH = new MDataInfo { Name = idDigest.ToArray(), TypeTag = 15001 };
                        using (var pubIdEntriesH = await _session.MDataEntries.NewAsync())
                        {
                            await _session.MDataEntries.InsertAsync(pubIdEntriesH, "@email".ToUtfBytes(), serInboxMdInfo);
                            await _session.MData.PutAsync(pubIdMDataInfoH, inboxPermH, pubIdEntriesH);
                        }

                        // Update _publicNames Container
                        var publicNamesContH = await _session.AccessContainer.GetMDataInfoAsync("_publicNames");
                        var pubNamesUserIdCipherBytes =
                            await _session.MDataInfoActions.EncryptEntryKeyAsync(publicNamesContH, userId.ToUtfBytes());
                        var pubNamesMsgBoxCipherBytes =
                            await _session.MDataInfoActions.EncryptEntryValueAsync(publicNamesContH, idDigest);
                        using (var pubNamesContEntActH = await _session.MDataEntryActions.NewAsync())
                        {
                            await _session.MDataEntryActions.InsertAsync(pubNamesContEntActH, pubNamesUserIdCipherBytes, pubNamesMsgBoxCipherBytes);
                            await _session.MData.MutateEntriesAsync(publicNamesContH, pubNamesContEntActH);
                        }

                        // Finally update App Container
                        var msgBox = new MessageBox
                        {
                            EmailId = userId,
                            Inbox = new DataArray { Type = "Buffer", Data = serInboxMdInfo },
                            Archive = new DataArray { Type = "Buffer", Data = serArchiveMdInfo },
                            EmailEncPk = inboxEncPkRaw.ToList().ToHexString(),
                            EmailEncSk = inboxEncSkRaw.ToList().ToHexString()
                        };

                        var msgBoxSer = JsonConvert.SerializeObject(msgBox);
                        var appContH = await _session.AccessContainer.GetMDataInfoAsync("apps/" + AppId);
                        var userIdCipherBytes =
                            await _session.MDataInfoActions.EncryptEntryKeyAsync(appContH, userId.ToUtfBytes());
                        var msgBoxCipherBytes =
                            await _session.MDataInfoActions.EncryptEntryValueAsync(appContH, msgBoxSer.ToUtfBytes());
                        using (var appContEntActH = await _session.MDataEntryActions.NewAsync())
                        {
                            await _session.MDataEntryActions.InsertAsync(appContEntActH, userIdCipherBytes, msgBoxCipherBytes);
                            await _session.MData.MutateEntriesAsync(appContH, appContEntActH);
                        }
                    }
                }
            }
        }

        public async Task CheckAndReconnect()
        {
            try
            {
                if (_session == null)
                {
                    return;
                }

                if (_session.IsDisconnected)
                {
                    if (!AuthReconnect)
                    {
                        throw new Exception("Reconnect Disabled");
                    }

                    using (UserDialogs.Instance.Loading("Reconnecting to Network"))
                    {
                        var encodedAuthRsp = await CredentialCache.Retrieve();
                        var authGranted = JsonConvert.DeserializeObject<AuthGranted>(encodedAuthRsp);
                        _session = await Session.AppRegisteredAsync(AppId, authGranted);
                    }

                    try
                    {
                        var cts = new CancellationTokenSource(2000);
                        await UserDialogs.Instance.AlertAsync("Network connection established.", "Success", "OK", cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Unable to Reconnect: {ex.Message}", "OK");
                _session?.Dispose();
                MessagingCenter.Send(this, MessengerConstants.ResetAppViews);
            }
        }

        ~AppService()
        {
            FreeState();
        }

        public void FreeState()
        {
            // ReSharper disable once DelegateSubtraction
            Session.Disconnected -= OnSessionDisconnected;
            _session?.Dispose();
            _session = null;
        }

        public async Task<string> GenerateAppRequestAsync()
        {
            var authReq = new AuthReq
            {
                AppContainer = true,
                App = new AppExchangeInfo { Id = AppId, Scope = string.Empty, Name = "SAFE Messages", Vendor = "MaidSafe.net Ltd" },
                Containers = new List<ContainerPermissions>
                    { new ContainerPermissions { ContName = "_publicNames", Access = { Insert = true } } }
            };

            var encodedReq = await Session.EncodeAuthReqAsync(authReq);
            var formattedReq = UrlFormat.Format(AppId, encodedReq.Item2, true);
            Debug.WriteLine($"Encoded Req: {formattedReq}");
            return formattedReq;
        }

        public async Task<List<UserId>> GetIdsAsync()
        {
            var ids = new List<UserId>();
            var appContH = await _session.AccessContainer.GetMDataInfoAsync("apps/" + AppId);
            var appContEntKeys = await _session.MData.ListKeysAsync(appContH);
            foreach (var cipherTextEntKey in appContEntKeys)
            {
                try
                {
                    var plainTextEntKey = await _session.MDataInfoActions.DecryptAsync(appContH, cipherTextEntKey.Key);
                    ids.Add(new UserId(plainTextEntKey.ToUtfString()));
                }
                catch (Exception)
                {
                    // ignore incompatible entries.
                }
            }

            return ids;
        }

        public async Task<List<Message>> GetMessagesAsync(string userId)
        {
            var messages = new List<Message>();
            var appCont = await _session.AccessContainer.GetMDataInfoAsync("apps/" + AppId);
            var userIdByteList = userId.ToUtfBytes();
            var cipherBytes = await _session.MDataInfoActions.EncryptEntryKeyAsync(appCont, userIdByteList);
            var content = await _session.MData.GetValueAsync(appCont, cipherBytes);
            var plainBytes = await _session.MDataInfoActions.DecryptAsync(appCont, content.Item1);

            var msgBox = JsonConvert.DeserializeObject<MessageBox>(plainBytes.ToUtfString());
            var inboxInfo = await _session.MDataInfoActions.DeserialiseAsync(msgBox.Inbox.Data);
            var inboxEntKeys = await _session.MData.ListKeysAsync(inboxInfo);
            using (var inboxPkH = await _session.Crypto.EncPubKeyNewAsync(msgBox.EmailEncPk.ToHexBytes().ToArray()))
            {
                using (var inboxSkH =
                    await _session.Crypto.EncSecretKeyNewAsync(msgBox.EmailEncSk.ToHexBytes().ToArray()))
                {
                    foreach (var key in inboxEntKeys)
                    {
                        try
                        {
                            var entryKey = key.Key.ToUtfString();
                            if (entryKey == "__email_enc_pk")
                            {
                                continue;
                            }

                            var value = await _session.MData.GetValueAsync(inboxInfo, key.Key);
                            var iDataNameEncoded =
                                await _session.Crypto.DecryptSealedBoxAsync(value.Item1, inboxPkH, inboxSkH);
                            var iDataNameBytes = iDataNameEncoded.ToUtfString().Split(',')
                                .Select(val => Convert.ToByte(val)).ToArray();
                            using (var msgDataReaderH = await _session.IData.FetchSelfEncryptorAsync(iDataNameBytes))
                            {
                                var msgSize = await _session.IData.SizeAsync(msgDataReaderH);
                                var msgCipher =
                                    await _session.IData.ReadFromSelfEncryptorAsync(msgDataReaderH, 0, msgSize);
                                var plainTextMsg =
                                    await _session.Crypto.DecryptSealedBoxAsync(msgCipher, inboxPkH, inboxSkH);
                                var jsonMsgData = plainTextMsg.ToUtfString();
                                messages.Add(JsonConvert.DeserializeObject<Message>(jsonMsgData));
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("Exception: " + e.Message);
                        }
                    }
                }
            }

            return messages;
        }

        public async Task HandleUrlActivationAsync(string url)
        {
            try
            {
                var encodedRequest = UrlFormat.GetRequestData(url);
                var decodeResult = await Session.DecodeIpcMessageAsync(encodedRequest);
                if (decodeResult.GetType() == typeof(AuthIpcMsg))
                {
                    var ipcMsg = decodeResult as AuthIpcMsg;
                    Debug.WriteLine("Received Auth Granted from Authenticator");

                    // update auth progress message
                    MessagingCenter.Send(this, MessengerConstants.AuthRequestProgress, AuthInProgressMessage);
                    if (ipcMsg != null)
                    {
                        _session = await Session.AppRegisteredAsync(AppId, ipcMsg.AuthGranted);
                        if (AuthReconnect)
                        {
                            var encodedAuthRsp = JsonConvert.SerializeObject(ipcMsg.AuthGranted);
                            await CredentialCache.Store(encodedAuthRsp);
                        }
                    }

                    MessagingCenter.Send(this, MessengerConstants.AuthRequestProgress, string.Empty);
                }
                else
                {
                    Debug.WriteLine("Decoded Req is not Auth Granted");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Description: {ex.Message}", "OK");
                MessagingCenter.Send(this, MessengerConstants.AuthRequestProgress, AuthDeniedMessage);
            }
        }

        private async void InitLoggingAsync()
        {
            var fileList = new List<(string, string)> { ("log.toml", "log.toml") };
            var fileOps = DependencyService.Get<IFileOps>();
            await fileOps.TransferAssetsAsync(fileList);
            Debug.WriteLine("Assets Transferred");
            try
            {
                await Session.InitLoggingAsync(fileOps.ConfigFilesPath);

                Debug.WriteLine("Rust Logging Initialised.");
                IsLogInitialised = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to Initialise Logging. " + e);
            }
        }

        private void OnSessionDisconnected(object obj, EventArgs e)
        {
            if (!obj.Equals(_session))
            {
                return;
            }

            Device.BeginInvokeOnMainThread(
                async () =>
                {
                    _session?.Dispose();
                    if (App.IsBackgrounded)
                    {
                        return;
                    }

                    await CheckAndReconnect();
                });
        }

        public async Task SendMessageAsync(string to, Message msg)
        {
            var dstPubIdDigest = await Crypto.Sha3HashAsync(to.ToUtfBytes());
            var dstPubIdMDataInfoH = new MDataInfo { Name = dstPubIdDigest.ToArray(), TypeTag = 15001 };
            var serDstMsgMDataInfo = await _session.MData.GetValueAsync(dstPubIdMDataInfoH, "@email".ToUtfBytes());
            var dstInboxMDataInfoH = await _session.MDataInfoActions.DeserialiseAsync(serDstMsgMDataInfo.Item1);
            var dstInboxPkItem = await _session.MData.GetValueAsync(dstInboxMDataInfoH, "__email_enc_pk".ToUtfBytes());
            using (var dstInboxPkH =
                await _session.Crypto.EncPubKeyNewAsync(dstInboxPkItem.Item1.ToUtfString().ToHexBytes().ToArray()))
            {
                var plainTxtMsg = JsonConvert.SerializeObject(msg);
                var cipherTxt = await _session.Crypto.EncryptSealedBoxAsync(plainTxtMsg.ToUtfBytes(), dstInboxPkH);
                byte[] msgDataMapNameBytes;
                using (var msgWriterH = await _session.IData.NewSelfEncryptorAsync())
                {
                    await _session.IData.WriteToSelfEncryptorAsync(msgWriterH, cipherTxt);
                    using (var msgCipherOptH = await _session.CipherOpt.NewPlaintextAsync())
                    {
                        msgDataMapNameBytes = await _session.IData.CloseSelfEncryptorAsync(msgWriterH, msgCipherOptH);
                    }
                }

                var encodedString = string.Join(", ", msgDataMapNameBytes.Select(val => val));
                var msgDataMapNameCipher =
                    await _session.Crypto.EncryptSealedBoxAsync(encodedString.ToUtfBytes(), dstInboxPkH);
                using (var dstMsgEntActH = await _session.MDataEntryActions.NewAsync())
                {
                    await _session.MDataEntryActions.InsertAsync(dstMsgEntActH, RandomGenerator.RandomString(15).ToUtfBytes(), msgDataMapNameCipher);
                    await _session.MData.MutateEntriesAsync(dstInboxMDataInfoH, dstMsgEntActH);
                }
            }
        }
    }
}
