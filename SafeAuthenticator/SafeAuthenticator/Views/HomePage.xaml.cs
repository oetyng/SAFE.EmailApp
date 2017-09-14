using System.Diagnostics;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views {
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class HomePage : ContentPage {
    public HomePage() {
      InitializeComponent();
      MessagingCenter.Subscribe<HomeViewModel>(
        this,
        MessengerConstants.NavLoginPage,
        async _ => {
          Debug.WriteLine("HomePage -> LoginPage");

          MessageCenterUnsubscribe();
          Navigation.InsertPageBefore(new LoginPage(), this);
          await Navigation.PopAsync();
        });
      MessagingCenter.Subscribe<HomeViewModel, RegisteredApp>(
        this,
        MessengerConstants.NavAppInfoPage,
        async (_, appInfo) => {
          await Navigation.PushAsync(new AppInfoPage(appInfo));
          AccountsView.SelectedItem = null;
        });
    }

    private void MessageCenterUnsubscribe() {
      MessagingCenter.Unsubscribe<HomeViewModel>(this, MessengerConstants.NavLoginPage);
      MessagingCenter.Unsubscribe<HomeViewModel, RegisteredApp>(this, MessengerConstants.NavAppInfoPage);
    }
  }
}
