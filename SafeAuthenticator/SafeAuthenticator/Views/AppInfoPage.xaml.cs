using CommonUtils;
using SafeAuthenticator.Models;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views {
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class AppInfoPage : ContentPage, ICleanup {
    public AppInfoPage() : this(null) { }

    public AppInfoPage(RegisteredAppModel appModelInfo) {
      InitializeComponent();
      BindingContext = new AppInfoViewModel(appModelInfo);
    }

    public void MessageCenterUnsubscribe() { }
  }
}
