using SafeMessages.Helpers;
using SafeMessages.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMessages.Views {
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class AuthView : ContentPage {
    public AuthView() {
      InitializeComponent();
      MessagingCenter.Subscribe<AuthViewModel>(
        this,
        MessengerConstants.NavUserIdsPage,
        async sender => {
          MessageCenterUnsubscribe();
          if (Navigation.NavigationStack.Count == 0) {
            return;
          }

          Navigation.InsertPageBefore(new UserIdsView(), this);
          await Navigation.PopAsync();
        });
    }

    private void MessageCenterUnsubscribe() {
      MessagingCenter.Unsubscribe<AuthViewModel>(this, MessengerConstants.NavUserIdsPage);
    }
  }
}
