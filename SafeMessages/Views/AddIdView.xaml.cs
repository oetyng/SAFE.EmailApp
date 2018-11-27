using SafeMessages.Helpers;
using SafeMessages.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMessages.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddIdView : ContentPage, ICleanup
    {
        public AddIdView()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<AddIdViewModel>(
                this,
                MessengerConstants.NavUserIdsPage,
                async sender =>
                {
                    MessageCenterUnsubscribe();
                    if (!App.IsPageValid(this))
                        return;

                    await Navigation.PopAsync();
                });
        }

        public void MessageCenterUnsubscribe()
        {
            MessagingCenter.Unsubscribe<AddIdViewModel>(this, MessengerConstants.NavUserIdsPage);
        }
    }
}
