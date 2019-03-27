﻿using System;
using System.Linq;
using System.Windows.Input;
using SafeMessages.Helpers;
using SafeMessages.Models;
using Xamarin.Forms;

namespace SafeMessages.ViewModels
{
    internal class MessagesViewModel : BaseViewModel
    {
        bool _isRefreshing;
        UserId _userId;

        public MessagesViewModel(UserId userId)
        {
            UserId = userId ?? new UserId("Unknown");
            IsRefreshing = false;
            RefreshCommand = new Command(OnRefreshCommand);
            SendCommand = new Command(OnSendCommand);
            MessageSelectedCommand = new Command<Message>(OnMessageSelectedCommand);
        }

        public DataModel AppData => DependencyService.Get<DataModel>();

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public UserId UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand SendCommand { get; }
        public ICommand MessageSelectedCommand { get; }

        void OnMessageSelectedCommand(Message message)
            => MessagingCenter.Send(this, MessengerConstants.NavDisplayMessageView, message);

        async void OnRefreshCommand()
        {
            IsRefreshing = true;
            try
            {
                var messages = await EmailInbox.GetMessagesAsync(UserId.Name);
                AppData.Messages.RemoveRange(AppData.Messages.Except(messages));
                AppData.Messages.AddRange(messages.Except(AppData.Messages));
                AppData.Messages.Sort(true);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Fetch Messages Failed: {ex.Message}", "OK");
            }

            IsRefreshing = false;
        }

        void OnSendCommand()
            => MessagingCenter.Send(this, MessengerConstants.NavSendMessagePage);
    }
}