using System;
using System.Linq;
using System.Windows.Input;
using SafeMessages.Helpers;
using SafeMessages.Models;
using Xamarin.Forms;

namespace SafeMessages.ViewModels
{
    internal class UserIdsViewModel : BaseViewModel
    {
        bool _isRefreshing;

        public UserIdsViewModel()
        {
            IsRefreshing = false;
            RefreshAccountsCommand = new Command(OnRefreshAccountsCommand);
            AddAccountCommand = new Command(OnAddAccountCommand);
            UserIdSelectedCommand = new Command<UserId>(OnUserIdSelectedCommand);
            Device.BeginInvokeOnMainThread(OnRefreshAccountsCommand);
        }

        public DataModel AppData => DependencyService.Get<DataModel>();

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand RefreshAccountsCommand { get; }

        public ICommand AddAccountCommand { get; }

        public ICommand UserIdSelectedCommand { get; }

        void OnAddAccountCommand()
            => MessagingCenter.Send(this, MessengerConstants.NavAddIdPage);

        async void OnRefreshAccountsCommand()
        {
            IsRefreshing = true;
            try
            {
                var accounts = await EmailIdManager.GetIdsAsync();
                AppData.Accounts.AddRange(accounts.Except(AppData.Accounts));
                AppData.Accounts.Sort();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Fetch Ids Failed: {ex.Message}", "OK");
            }

            IsRefreshing = false;
        }

        void OnUserIdSelectedCommand(UserId userId)
        {
            AppService.Self = userId;
            MessagingCenter.Send(this, MessengerConstants.NavMessagesPage, userId);
        }
    }
}