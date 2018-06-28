using System.Linq;
using System.Threading.Tasks;
using CommonUtils;
using SafeMessages.Helpers;
using SafeMessages.Services;
using SafeMessages.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace SafeMessages {
  public partial class App : Application {
    public const string AppName = "SAFE Messages";
    private static volatile bool _isBackgrounded;
    public AppService SafeApp => DependencyService.Get<AppService>();
    public static bool IsBackgrounded { get => _isBackgrounded; private set => _isBackgrounded = value; }

    public App() {
      InitializeComponent();

      MessagingCenter.Subscribe<AppService>(this, MessengerConstants.ResetAppViews, async _ => { await ResetViews(); });
      Current.MainPage = new NavigationPage(NewStartupPage());
    }

    public static bool IsPageValid(Page page) {
      var navPage = Current.MainPage as NavigationPage;
      if (navPage == null) {
        return false;
      }

      var validPage = navPage.Navigation.NavigationStack.FirstOrDefault();
      var checkPage = page.Navigation.NavigationStack.FirstOrDefault();
      return validPage != null && validPage == checkPage;
    }

    private static Page NewStartupPage() {
      return new AuthView();
    }

    protected override async void OnResume() {
      base.OnResume();

      IsBackgrounded = false;
      await DependencyService.Get<AppService>().CheckAndReconnect();
    }

    protected override async void OnSleep() {
      base.OnSleep();

      IsBackgrounded = true;
      await SavePropertiesAsync();
    }

    private static async Task ResetViews() {
      var navPage = Current.MainPage as NavigationPage;
      if (navPage == null) {
        return;
      }

      var navigationController = navPage.Navigation;
      foreach (var page in navigationController.NavigationStack.OfType<ICleanup>()) {
        page.MessageCenterUnsubscribe();
      }

      var rootPage = navigationController.NavigationStack.FirstOrDefault();
      if (rootPage == null) {
        return;
      }

      navigationController.InsertPageBefore(NewStartupPage(), rootPage);
      await navigationController.PopToRootAsync(true);
    }
  }
}
