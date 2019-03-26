using System;
using System.IO;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using SafeMessages.Helpers;
using SafeMessages.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Debug = System.Diagnostics.Debug;

namespace SafeMessages.Droid
{
    [Activity(
        Label = "@string/app_name",
        Theme = "@style/MyTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTask,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = AppConstants.AppId)]
    public class MainActivity : FormsAppCompatActivity
    {
        static string LogFolderPath => DependencyService.Get<IFileOps>().ConfigFilesPath;

        AppService AppService => DependencyService.Get<AppService>();

        void HandleAppLaunch(string url)
        {
            Debug.WriteLine($"Launched via: {url}");
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await AppService.HandleUrlActivationAsync(url);
                    Debug.WriteLine("IPC Msg Handling Completed");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            });
        }

        public override void OnBackPressed()
        {
            if (Xamarin.Forms.Application.Current.MainPage is NavigationPage currentNav &&
                currentNav.Navigation.NavigationStack.Count == 1)
                AppService.FreeState();

            base.OnBackPressed();
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvOnUnhandledExceptionRaiser;

            base.OnCreate(bundle);
            Forms.Init(this, bundle);

            DisplayCrashReport();

            UserDialogs.Init(this);
            LoadApplication(new App());

            if (Intent?.Data != null)
                HandleAppLaunch(Intent.Data.ToString());
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            if (intent?.Data != null)
                HandleAppLaunch(intent.Data.ToString());
        }

        #region Error Handling

        static void AndroidEnvOnUnhandledExceptionRaiser(object o, RaiseThrowableEventArgs exEventArgs)
        {
            var newExc = new Exception("AndroidEnvironmentOnUnhandledExceptionRaiser", exEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs exEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", exEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs exEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", exEventArgs.ExceptionObject as Exception);
            LogUnhandledException(newExc);
        }

        internal static void LogUnhandledException(Exception exception)
        {
            try
            {
                const string errorFileName = "Fatal.log";
                var errorFilePath = Path.Combine(LogFolderPath, errorFileName);
                var errorMessage = $"Time: {DateTime.Now}\nError: Unhandled Exception\n{exception}\n\n";
                File.AppendAllText(errorFilePath, errorMessage);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }

        void DisplayCrashReport()
        {
            const string errorFilename = "Fatal.log";
            var errorFilePath = Path.Combine(LogFolderPath, errorFilename);

            if (!File.Exists(errorFilePath))
                return;

            var errorText = File.ReadAllText(errorFilePath);
            new AlertDialog.Builder(this).SetPositiveButton("Clear", (sender, args) => { File.Delete(errorFilePath); })
                .SetNegativeButton("Close", (sender, args) => { }).SetMessage(errorText).SetTitle("Crash Report")
                .Show();
        }

        #endregion
    }
}