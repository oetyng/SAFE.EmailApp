using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SafeMessages.Helpers;
using SafeMessages.Services;
using Xamarin.Forms;

namespace SafeMessages.ViewModels
{
    internal class AuthViewModel : BaseViewModel
    {
        private string _authProgressMessage;

        public AuthViewModel()
        {
            AppService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AppService.IsLogInitialised))
                    IsUiEnabled = AppService.IsLogInitialised;
            };

            MessagingCenter.Subscribe<AppService, string>(
                this,
                MessengerConstants.AuthRequestProgress,
                async (sender, progressText) =>
                {
                    AuthProgressMessage = progressText;
                    if (AuthProgressMessage == string.Empty)
                    {
                        MessagingCenter.Send(this, MessengerConstants.NavUserIdsPage);
                    }
                    else if (AuthProgressMessage == AppService.AuthDeniedMessage)
                    {
                        await Task.Delay(3000);
                        AuthProgressMessage = string.Empty;
                        IsUiEnabled = true;
                    }
                });
            IsUiEnabled = AppService.IsLogInitialised;

            AuthCommand = new Command(OnAuthCommand);
        }

        public ICommand AuthCommand { get; set; }

        public string AuthProgressMessage
        {
            get => _authProgressMessage;
            set => SetProperty(ref _authProgressMessage, value);
        }

        public bool AuthReconnect
        {
            get => AppService.AuthReconnect;
            set
            {
                if (AppService.AuthReconnect != value)
                    AppService.AuthReconnect = value;

                OnPropertyChanged();
            }
        }

        private async void OnAuthCommand()
        {
            try
            {
                IsUiEnabled = false;
                AuthProgressMessage = "Requesting Authentication.";
                var url = await AppService.GenerateAppRequestAsync();
                var appLaunched = await DependencyService.Get<IAppHandler>().LaunchApp(url);
                if (!appLaunched)
                {
                    IsUiEnabled = true;
                    await Application.Current.MainPage.DisplayAlert("Authorisation failed", "The SAFE Authenticator app is required to authorise this application", "OK");
                }
            }
            catch (Exception ex)
            {
                IsUiEnabled = true;
                await Application.Current.MainPage.DisplayAlert("Error", $"Generate App Request Failed: {ex.Message}", "OK");
            }
        }
    }
}
