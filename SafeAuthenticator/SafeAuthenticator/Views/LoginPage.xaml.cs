using System.Diagnostics;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views {
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class LoginPage : ContentPage {
    public LoginPage() {
      InitializeComponent();

      MessagingCenter.Subscribe<LoginViewModel>(
        this,
        MessengerConstants.NavHomePage,
        async sender => {
          Debug.WriteLine("LoginPage -> HomePage");

          MessageCenterUnsubscribe();
          Navigation.InsertPageBefore(new HomePage(), this);
          await Navigation.PopAsync();
        });

      MessagingCenter.Subscribe<LoginViewModel>(
        this,
        MessengerConstants.NavCreateAcctPage,
        async _ => {
          Debug.WriteLine("LoginPage -> CreateAcctPage");
          await Navigation.PushAsync(new CreateAcctPage());
        });
    }

    private void MessageCenterUnsubscribe() {
      MessagingCenter.Unsubscribe<LoginViewModel>(this, MessengerConstants.NavHomePage);
      MessagingCenter.Unsubscribe<LoginViewModel>(this, MessengerConstants.NavCreateAcctPage);
    }
  }
}
