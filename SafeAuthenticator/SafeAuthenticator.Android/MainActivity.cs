using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using SafeAuthenticator.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace SafeAuthenticator.Droid {
  [Activity(
     Label = "@string/app_name",
     Theme = "@style/MyTheme",
     MainLauncher = true,
     LaunchMode = LaunchMode.SingleTask,
     ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation),
   IntentFilter(new[] {Intent.ActionView}, Categories = new[] {Intent.CategoryDefault, Intent.CategoryBrowsable}, DataScheme = "safe-auth")]
  public class MainActivity : FormsAppCompatActivity {
    public AuthService Authenticator => DependencyService.Get<AuthService>();

    private void HandleAppLaunch(string url) {
      System.Diagnostics.Debug.WriteLine($"Launched via: {url}");
      Device.BeginInvokeOnMainThread(
        async () => {
          try {
            await Authenticator.HandleUrlActivationAsync(url);
            System.Diagnostics.Debug.WriteLine("IPC Msg Handling Completed");
          } catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
          }
        });
    }

    public override void OnBackPressed() {
      if (Xamarin.Forms.Application.Current.MainPage is NavigationPage currentNav && currentNav.Navigation.NavigationStack.Count == 1) {
        Authenticator.FreeState();
      }

      base.OnBackPressed();
    }

    protected override void OnCreate(Bundle bundle) {
      TabLayoutResource = Resource.Layout.Tabbar;
      ToolbarResource = Resource.Layout.Toolbar;

      base.OnCreate(bundle);
      Forms.Init(this, bundle);
      LoadApplication(new App());

      if (Intent?.Data != null) {
        HandleAppLaunch(Intent.Data.ToString());
      }
    }

    protected override void OnNewIntent(Intent intent) {
      base.OnNewIntent(intent);
      if (intent?.Data != null) {
        HandleAppLaunch(intent.Data.ToString());
      }
    }
  }
}
