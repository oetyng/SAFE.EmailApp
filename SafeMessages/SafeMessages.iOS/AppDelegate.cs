using System;
using System.Diagnostics;
using Foundation;
using SafeMessages.Services;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace SafeMessages.iOS {
  [Register("AppDelegate")]
  public class AppDelegate : FormsApplicationDelegate {
    public AppService SafeApp => DependencyService.Get<AppService>();

    public override bool FinishedLaunching(UIApplication app, NSDictionary options) {
      Forms.Init();
      LoadApplication(new App());

      return base.FinishedLaunching(app, options);
    }

    public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options) {
      Device.BeginInvokeOnMainThread(
        async () => {
          try {
            await SafeApp.HandleUrlActivationAsync(url.ToString());
            Debug.WriteLine("IPC Msg Handling Completed");
          } catch (Exception ex) {
            Debug.WriteLine($"Error: {ex.Message}");
          }
        });
      return true;
    }
  }
}
