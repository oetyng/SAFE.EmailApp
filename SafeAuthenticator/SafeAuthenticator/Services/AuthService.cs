using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CommonUtils;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using SafeAuthenticator.Native;
using SafeAuthenticator.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthService))]

namespace SafeAuthenticator.Services {
  public class AuthService : ObservableObject, IDisposable {
    private const string AuthReconnectPropKey = nameof(AuthReconnect);
    private readonly SemaphoreSlim _reconnectSemaphore = new SemaphoreSlim(1, 1);
    private bool _isLogInitialised;

    public bool IsLogInitialised { get => _isLogInitialised; set => SetProperty(ref _isLogInitialised, value); }

    private CredentialCacheService CredentialCache { get; }

    public bool AuthReconnect {
      get {
        if (!Application.Current.Properties.ContainsKey(AuthReconnectPropKey)) {
          return false;
        }

        var val = Application.Current.Properties[AuthReconnectPropKey] as bool?;
        return val == true;
      }
      set {
        if (value == false) {
          CredentialCache.Delete();
        }

        Application.Current.Properties[AuthReconnectPropKey] = value;
      }
    }

    public AuthService() {
      _isLogInitialised = false;
      CredentialCache = new CredentialCacheService();
      InitLoggingAsync();
    }

    public void Dispose() {
      FreeState();
      GC.SuppressFinalize(this);
    }

    public async Task CheckAndReconnect() {
      await _reconnectSemaphore.WaitAsync();
      try {
        if (Session.IsDisconnected) {
          if (!AuthReconnect) {
            throw new Exception("Reconnect Disabled");
          }
          using (UserDialogs.Instance.Loading("Reconnecting to Network")) {
            var (location, password) = CredentialCache.Retrieve();
            await LoginAsync(location, password);
          }
          try {
            var cts = new CancellationTokenSource(2000);
            await UserDialogs.Instance.AlertAsync("Network connection established.", "Success", "OK", cts.Token);
          } catch (OperationCanceledException) { }
        }
      } catch (Exception ex) {
        await Application.Current.MainPage.DisplayAlert("Error", $"Unable to Reconnect: {ex.Message}", "OK");
        FreeState();
        MessagingCenter.Send(this, MessengerConstants.ResetAppViews);
      } finally {
        _reconnectSemaphore.Release(1);
      }
    }

    public async Task CreateAccountAsync(string location, string password, string invitation) {
      Debug.WriteLine($"CreateAccountAsync {location} - {password} - {invitation.Substring(0, 5)}");
      await Session.CreateAccountAsync(location, password, invitation);
      if (AuthReconnect) {
        CredentialCache.Store(location, password);
      }
    }

    ~AuthService() {
      FreeState();
    }

    public void FreeState() {
      Session.FreeAuth();
    }

    public async Task<(int, int)> GetAccountInfoAsync() {
      var acctInfo = await Session.AuthAccountInfoAsync();
      return (Convert.ToInt32(acctInfo.Used), Convert.ToInt32(acctInfo.Used + acctInfo.Available));
    }

    public Task<List<RegisteredApp>> GetRegisteredAppsAsync() {
      return Session.AuthRegisteredAppsAsync();
    }

    public async Task HandleUrlActivationAsync(string encodedUrl) {
      try {
        await CheckAndReconnect();
        var formattedUrl = UrlFormat.Convert(encodedUrl, true);
        var decodeResult = await Session.AuthDecodeIpcMsgAsync(formattedUrl);
        if (decodeResult.AuthReq.HasValue) {
          var authReq = decodeResult.AuthReq.Value;
          Debug.WriteLine($"Decoded Req From {authReq.AppExchangeInfo.Name}");
          var isGranted = await Application.Current.MainPage.DisplayAlert(
            "Auth Request",
            $"{authReq.AppExchangeInfo.Name} is requesting access",
            "Allow",
            "Deny");
          var encodedRsp = await Session.EncodeAuthRspAsync(authReq, isGranted);
          var formattedRsp = UrlFormat.Convert(encodedRsp, false);
          Debug.WriteLine($"Encoded Rsp to app: {formattedRsp}");
          Device.BeginInvokeOnMainThread(() => { Device.OpenUri(new Uri(formattedRsp)); });
        } else {
          Debug.WriteLine("Decoded Req is not Auth Req");
        }
      } catch (Exception ex) {
        var errorMsg = ex.Message;
        if (ex is ArgumentNullException) {
          errorMsg = "Ignoring Auth Request: Need to be logged in to accept app requests.";
        }
        await Application.Current.MainPage.DisplayAlert("Error", errorMsg, "OK");
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

    public async Task LoginAsync(string location, string password) {
      Debug.WriteLine($"LoginAsync {location} - {password}");
      await Session.LoginAsync(location, password);
      if (AuthReconnect) {
        CredentialCache.Store(location, password);
      }
    }

    public async Task LogoutAsync() {
      await Task.Run(() => { Session.FreeAuth(); });
    }
  }
}
