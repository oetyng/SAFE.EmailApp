using SafeMessages.Helpers;
using SafeMessages.Models;
using SafeMessages.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMessages.Views {
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class UserIdsView : ContentPage, ICleanup {
    public UserIdsView() {
      InitializeComponent();
      MessagingCenter.Subscribe<UserIdsViewModel>(
        this,
        MessengerConstants.NavAddIdPage,
        async _ => {
          if (!App.IsPageValid(this)) {
            MessageCenterUnsubscribe();
            return;
          }

          await Navigation.PushAsync(new AddIdView());
        });

      MessagingCenter.Subscribe<UserIdsViewModel, UserId>(
        this,
        MessengerConstants.NavMessagesPage,
        async (_, userId) => {
          if (!App.IsPageValid(this)) {
            MessageCenterUnsubscribe();
            return;
          }

          await Navigation.PushAsync(new MessagesView(userId));
          AccountsView.SelectedItem = null;
        });
    }

    public void MessageCenterUnsubscribe() {
      MessagingCenter.Unsubscribe<UserIdsViewModel>(this, MessengerConstants.NavAddIdPage);
      MessagingCenter.Unsubscribe<UserIdsViewModel, UserId>(this, MessengerConstants.NavMessagesPage);
    }
  }
}
