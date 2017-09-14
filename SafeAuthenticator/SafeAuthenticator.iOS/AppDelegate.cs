using System;
using System.Diagnostics;
using Foundation;
using SafeAuthenticator.Services;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace SafeAuthenticator.iOS {
  [Register("AppDelegate")]
  public class AppDelegate : FormsApplicationDelegate {
    public AuthService Authenticator => DependencyService.Get<AuthService>();

    public override bool FinishedLaunching(UIApplication app, NSDictionary options) {
      Forms.Init();
      LoadApplication(new App());

      return base.FinishedLaunching(app, options);
    }

    public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options) {
      Device.BeginInvokeOnMainThread(
        async () => {
          try {
            await Authenticator.HandleUrlActivationAsync(url.ToString());
            Debug.WriteLine("IPC Msg Handling Completed");
          } catch (Exception ex) {
            Debug.WriteLine($"Error: {ex.Message}");
          }
        });
      return true;
    }
  }
}
