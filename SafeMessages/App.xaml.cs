using System.Linq;
using System.Threading.Tasks;
using SafeMessages.Helpers;
using SafeMessages.Services;
using SafeMessages.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace SafeMessages
{
    public partial class App : Application
    {
        static volatile bool _isBackgrounded;

        public static bool IsBackgrounded
        {
            get => _isBackgrounded;
            private set => _isBackgrounded = value;
        }

        public App()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<AppService>(this, MessengerConstants.ResetAppViews, async _ => { await ResetViews(); });
            Current.MainPage = new NavigationPage(NewStartupPage());
        }

        public AppService SafeApp => DependencyService.Get<AppService>();

        public static bool IsPageValid(Page page)
        {
            if (!(Current.MainPage is NavigationPage navPage))
                return false;

            var validPage = navPage.Navigation.NavigationStack.FirstOrDefault();
            var checkPage = page.Navigation.NavigationStack.FirstOrDefault();
            return validPage != null && validPage == checkPage;
        }

        static Page NewStartupPage()
            => new AuthView();

        protected override async void OnResume()
        {
            base.OnResume();

            IsBackgrounded = false;
            await DependencyService.Get<AppService>().CheckAndReconnect();
        }

        protected override async void OnSleep()
        {
            base.OnSleep();

            IsBackgrounded = true;
            await SavePropertiesAsync();
        }

        static async Task ResetViews()
        {
            if (!(Current.MainPage is NavigationPage navPage))
                return;

            var navigationController = navPage.Navigation;
            foreach (var page in navigationController.NavigationStack.OfType<ICleanup>())
                page.MessageCenterUnsubscribe();

            var rootPage = navigationController.NavigationStack.FirstOrDefault();
            if (rootPage == null)
                return;

            navigationController.InsertPageBefore(NewStartupPage(), rootPage);
            await navigationController.PopToRootAsync(true);
        }
    }
}