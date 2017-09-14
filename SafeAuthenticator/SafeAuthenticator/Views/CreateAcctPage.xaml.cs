using System.Diagnostics;
using System.Linq;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views {
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class CreateAcctPage : ContentPage {
    public CreateAcctPage() {
      InitializeComponent();

      MessagingCenter.Subscribe<CreateAcctViewModel>(
        this,
        MessengerConstants.NavHomePage,
        async _ => {
          Debug.WriteLine("CreateAcctPage -> HomePage");

          var rootPage = Navigation.NavigationStack.FirstOrDefault();
          if (rootPage == null) {
            return;
          }

          MessageCenterUnsubscribe();
          Navigation.InsertPageBefore(new HomePage(), Navigation.NavigationStack.First());
          await Navigation.PopToRootAsync();
        });
    }

    private void MessageCenterUnsubscribe() {
      MessagingCenter.Unsubscribe<CreateAcctViewModel>(this, MessengerConstants.NavHomePage);
    }
  }
}
