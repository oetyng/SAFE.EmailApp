using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using SafeMessages.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace SafeMessages.Droid {
  [Activity(
     Label = "@string/app_name",
     Theme = "@style/MyTheme",
     MainLauncher = true,
     LaunchMode = LaunchMode.SingleTask,
     ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation),
   IntentFilter(
     new[] {Intent.ActionView},
     Categories = new[] {Intent.CategoryDefault, Intent.CategoryBrowsable},
     DataScheme = "safe-net.maidsafe.examples.mailtutorial")]
  public class MainActivity : FormsAppCompatActivity {
    public AppService SafeApp => DependencyService.Get<AppService>();

    private void HandleAppLaunch(string url) {
      System.Diagnostics.Debug.WriteLine($"Launched via: {url}");
      Device.BeginInvokeOnMainThread(
        async () => {
          try {
            await SafeApp.HandleUrlActivationAsync(url);
            System.Diagnostics.Debug.WriteLine("IPC Msg Handling Completed");
          } catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
          }
        });
    }

    public override void OnBackPressed() {
      if (Xamarin.Forms.Application.Current.MainPage is NavigationPage currentNav && currentNav.Navigation.NavigationStack.Count == 1) {
        SafeApp.FreeState();
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
