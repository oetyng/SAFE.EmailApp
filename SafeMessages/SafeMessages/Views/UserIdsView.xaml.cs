using SafeMessages.Helpers;
using SafeMessages.Models;
using SafeMessages.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMessages.Views {
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class UserIdsView : ContentPage {
    public UserIdsView() {
      InitializeComponent();
      MessagingCenter.Subscribe<UserIdsViewModel>(
        this,
        MessengerConstants.NavAddIdPage,
        async _ => {
          if (Navigation.NavigationStack.Count == 0) {
            return;
          }

          await Navigation.PushAsync(new AddIdView());
        });

      MessagingCenter.Subscribe<UserIdsViewModel, UserId>(
        this,
        MessengerConstants.NavMessagesPage,
        async (_, userId) => {
          if (Navigation.NavigationStack.Count == 0) {
            return;
          }

          await Navigation.PushAsync(new MessagesView(userId));
          AccountsView.SelectedItem = null;
        });
    }
  }
}
