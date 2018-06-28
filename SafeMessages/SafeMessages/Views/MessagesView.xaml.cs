using CommonUtils;
using SafeMessages.Helpers;
using SafeMessages.Models;
using SafeMessages.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMessages.Views {
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class MessagesView : ContentPage, ICleanup {
    public DataModel AppData => DependencyService.Get<DataModel>();

    public MessagesView() : this(null) { }

    public MessagesView(UserId userId) {
      InitializeComponent();

      var viewModel = new MessagesViewModel(userId);
      BindingContext = viewModel;
      MessagingCenter.Subscribe<MessagesViewModel>(
        this,
        MessengerConstants.NavSendMessagePage,
        async _ => {
          if (!App.IsPageValid(this)) {
            MessageCenterUnsubscribe();
            return;
          }

          await Navigation.PushAsync(new SendMessageView());
        });

      MessagingCenter.Subscribe<MessagesViewModel, Message>(
        this,
        MessengerConstants.NavDisplayMessageView,
        async (_, message) => {
          if (!App.IsPageValid(this)) {
            MessageCenterUnsubscribe();
            return;
          }

          await Navigation.PushAsync(new DisplayMessagePage(message));
          MessagesListView.SelectedItem = null;
        });

      if (!viewModel.RefreshCommand.CanExecute(null)) {
        return;
      }

      AppData.ClearMessages();
      viewModel.RefreshCommand.Execute(null);
    }

    public void MessageCenterUnsubscribe() {
      MessagingCenter.Unsubscribe<MessagesViewModel>(this, MessengerConstants.NavSendMessagePage);
      MessagingCenter.Unsubscribe<MessagesViewModel, Message>(this, MessengerConstants.NavDisplayMessageView);
    }
  }
}
