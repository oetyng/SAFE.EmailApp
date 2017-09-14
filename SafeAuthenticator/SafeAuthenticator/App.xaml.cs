using SafeAuthenticator.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace SafeAuthenticator {
  public partial class App : Application {
    public App() {
      InitializeComponent();

      Current.MainPage = new NavigationPage(new LoginPage());
    }
  }
}
