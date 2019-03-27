using SafeMessages.Models.BaseModel;
using SafeMessages.Services;
using Xamarin.Forms;

namespace SafeMessages.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        bool _isUiEnabled;

        public bool IsUiEnabled
        {
            get => _isUiEnabled;
            set => SetProperty(ref _isUiEnabled, value);
        }

        public AppService AppService => DependencyService.Get<AppService>();
        public EmailIdManager EmailIdManager => DependencyService.Get<EmailIdManager>();
        public EmailInbox EmailInbox => DependencyService.Get<EmailInbox>();
    }
}