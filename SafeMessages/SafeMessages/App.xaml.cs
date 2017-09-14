using SafeMessages.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace SafeMessages {
  public partial class App : Application {
    public App() {
      InitializeComponent();

      SetMainPage();
    }

    public static void SetMainPage() {
      var navPage = new NavigationPage(new AuthView());
      Current.MainPage = navPage;
    }
  }
}
