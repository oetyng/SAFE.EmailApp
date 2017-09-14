using System.Windows.Input;
using SafeAuthenticator.Models;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels {
  internal class AppInfoViewModel : BaseViewModel {
    private RegisteredApp _appInfo;

    public RegisteredApp AppInfo { get => _appInfo; set => SetProperty(ref _appInfo, value); }

    public ICommand RevokeAppCommand { get; }

    public AppInfoViewModel(RegisteredApp appInfo) {
      AppInfo = appInfo;
      RevokeAppCommand = new Command(OnRevokeAppCommand);
    }

    private async void OnRevokeAppCommand() {
      await Application.Current.MainPage.DisplayAlert("Not Supported", "Not yet implemented.", "OK");
    }
  }
}
