using SafeMessages.Helpers;
using SafeMessages.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMessages.Views {
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class AddIdView : ContentPage {
    public AddIdView() {
      InitializeComponent();
      MessagingCenter.Subscribe<AddIdViewModel>(
        this,
        MessengerConstants.NavUserIdsPage,
        async sender => {
          MessageCenterUnsubscribe();
          if (Navigation.NavigationStack.Count == 0) {
            return;
          }

          await Navigation.PopAsync();
        });
    }

    private void MessageCenterUnsubscribe() {
      MessagingCenter.Unsubscribe<AddIdViewModel>(this, MessengerConstants.NavUserIdsPage);
    }
  }
}
