using SafeMessages.Helpers;
using SafeMessages.Models;
using SafeMessages.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMessages.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayMessagePage : ContentPage, ICleanup
    {
        public DisplayMessagePage()
            : this(null)
        {
        }

        public DisplayMessagePage(Message message)
        {
            InitializeComponent();
            BindingContext = new DisplayMessageViewModel(message);
            MessagingCenter.Subscribe<DisplayMessageViewModel, string>(
                this,
                MessengerConstants.NavSendMessagePage,
                async (sender, subject) =>
                {
                    if (!App.IsPageValid(this))
                    {
                        MessageCenterUnsubscribe();
                        return;
                    }

                    await Navigation.PushAsync(new SendMessageView(new UserId(message.From), subject));
                });
        }

        public void MessageCenterUnsubscribe()
        {
            MessagingCenter.Unsubscribe<DisplayMessageViewModel, string>(this, MessengerConstants.NavSendMessagePage);
        }
    }
}
