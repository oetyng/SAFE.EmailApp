using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CommonUtils;
using SafeAuthenticator.Models;
using SafeAuthenticator.Native;
using SafeAuthenticator.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthService))]

namespace SafeAuthenticator.Services {
  public class AuthService : ObservableObject, IDisposable {
    private bool _isLogInitialised;

    public bool IsLogInitialised { get => _isLogInitialised; set => SetProperty(ref _isLogInitialised, value); }

    public AuthService() {
      _isLogInitialised = false;
      InitLoggingAsync();
    }

    public void Dispose() {
      FreeState();
      GC.SuppressFinalize(this);
    }

    public Task CreateAccountAsync(string location, string password, string invitation) {
      Debug.WriteLine($"CreateAccountAsync {location} - {password} - {invitation.Substring(0, 5)}");
      return Session.CreateAccountAsync(location, password, invitation);
    }

    ~AuthService() {
      FreeState();
    }

    public void FreeState() {
      Session.FreeAuth();
    }

    public async Task<Tuple<int, int>> GetAccountInfoAsync() {
      var acctInfo = await Session.AuthAccountInfoAsync();
      return Tuple.Create(Convert.ToInt32(acctInfo.Used), Convert.ToInt32(acctInfo.Used + acctInfo.Available));
    }

    public Task<List<RegisteredApp>> GetRegisteredAppsAsync() {
      return Session.AuthRegisteredAppsAsync();
    }

    public async Task HandleUrlActivationAsync(string encodedUrl) {
      try {
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

    public Task LoginAsync(string location, string password) {
      Debug.WriteLine($"LoginAsync {location} - {password}");
      return Session.LoginAsync(location, password);
    }

    public async Task LogoutAsync() {
      await Task.Run(() => { Session.FreeAuth(); });
    }
  }
}
