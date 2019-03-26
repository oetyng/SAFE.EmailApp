using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Newtonsoft.Json;
using SafeApp;
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
        public const string AuthDeniedMessage = "Failed to receive Authentication.";
        public const string AuthInProgressMessage = "Connecting to SAFE Network...";
        const string AuthReconnectPropKey = nameof(AuthReconnect);
        bool _isLogInitialised;
        Session _session;

        public AppService()
        {
            _isLogInitialised = false;
            CredentialCache = new CredentialCacheService();
            Session.Disconnected += OnSessionDisconnected;
            InitLoggingAsync();
        }

        CredentialCacheService CredentialCache { get; }

        public UserId Self { get; set; }

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
                    return false;

                var val = Application.Current.Properties[AuthReconnectPropKey] as bool?;
                return val == true;
            }

            set
            {
                if (value == false)
                    CredentialCache.Delete();

                Application.Current.Properties[AuthReconnectPropKey] = value;
            }
        }

        public void Dispose()
        {
            FreeState();
            GC.SuppressFinalize(this);
        }

        public async Task CheckAndReconnect()
        {
            try
            {
                if (_session == null)
                    return;

                if (_session.IsDisconnected)
                {
                    if (!AuthReconnect)
                        throw new Exception("Reconnect Disabled");

                    using (UserDialogs.Instance.Loading("Reconnecting to Network"))
                    {
                        var encodedAuthRsp = await CredentialCache.Retrieve();
                        var authGranted = JsonConvert.DeserializeObject<AuthGranted>(encodedAuthRsp);
                        _session = await Session.AppRegisteredAsync(AppConstants.AppId, authGranted);
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

        ~AppService() => FreeState();

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
                App = new AppExchangeInfo { Id = AppConstants.AppId, Scope = string.Empty, Name = AppConstants.AppName, Vendor = "oetyng" },
                Containers = new List<ContainerPermissions>
                { new ContainerPermissions { ContName = "_publicNames", Access = { Insert = true } } }
            };

            var encodedReq = await Session.EncodeAuthReqAsync(authReq);
            var formattedReq = UrlFormat.Format(AppConstants.AppId, encodedReq.Item2, true);
            Debug.WriteLine($"Encoded Req: {formattedReq}");
            return formattedReq;
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
                        _session = await Session.AppRegisteredAsync(AppConstants.AppId, ipcMsg.AuthGranted);
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

        async void InitLoggingAsync()
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

        void OnSessionDisconnected(object obj, EventArgs e)
        {
            if (!obj.Equals(_session))
                return;

            Device.BeginInvokeOnMainThread(
                async () =>
                {
                    _session?.Dispose();
                    if (App.IsBackgrounded)
                        return;

                    await CheckAndReconnect();
                });
        }
    }
}