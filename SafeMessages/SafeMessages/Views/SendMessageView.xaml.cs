using CommonUtils;
using SafeMessages.Helpers;
using SafeMessages.Models;
using SafeMessages.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMessages.Views {
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class SendMessageView : ContentPage, ICleanup {
    public SendMessageView() : this(null, string.Empty) { }

    public SendMessageView(UserId userId, string subject) {
      InitializeComponent();
      var viewModel = new SendMessageViewModel(userId, subject);
      BindingContext = viewModel;
      MessagingCenter.Subscribe<SendMessageViewModel>(
        this,
        MessengerConstants.NavPreviousPage,
        async sender => {
          MessageCenterUnsubscribe();
          if (!App.IsPageValid(this)) {
            return;
          }

          await Navigation.PopAsync();
        });
    }

    public void MessageCenterUnsubscribe() {
      MessagingCenter.Unsubscribe<SendMessageViewModel>(this, MessengerConstants.NavPreviousPage);
    }
  }
}
