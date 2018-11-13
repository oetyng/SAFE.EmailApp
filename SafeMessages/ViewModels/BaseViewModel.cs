using SafeMessages.Models.BaseModel;
using SafeMessages.Services;
using Xamarin.Forms;

namespace SafeMessages.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        private bool _isUiEnabled;

        public bool IsUiEnabled
        {
            get => _isUiEnabled;
            set => SetProperty(ref _isUiEnabled, value);
        }

        public AppService SafeApp => DependencyService.Get<AppService>();
    }
}
