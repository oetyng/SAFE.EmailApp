using SafeAuthenticator.Models;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views {
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class AppInfoPage : ContentPage {
    public AppInfoPage() : this(null) { }

    public AppInfoPage(RegisteredApp appInfo) {
      InitializeComponent();
      BindingContext = new AppInfoViewModel(appInfo);
    }
  }
}
