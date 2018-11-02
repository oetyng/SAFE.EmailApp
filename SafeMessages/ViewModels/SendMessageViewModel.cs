using System;
using System.Windows.Input;
using SafeMessages.Helpers;
using SafeMessages.Models;
using Xamarin.Forms;

namespace SafeMessages.ViewModels
{
    internal class SendMessageViewModel : BaseViewModel
    {
        // FIXME Prop Names
        private string _body;

        private string _subject;

        private string _to;

        public SendMessageViewModel(UserId userId, string subject)
        {
            IsUiEnabled = true;
            Body = string.Empty;
            Subject = subject;
            To = userId == null ? string.Empty : userId.Name;
            SendCommand = new Command(OnSendCommand);
        }

        public ICommand SendCommand { get; }

        public string Body
        {
            get => _body;
            set => SetProperty(ref _body, value);
        }

        public string To
        {
            get => _to;
            set => SetProperty(ref _to, value);
        }

        public string Subject
        {
            get => _subject;
            set => SetProperty(ref _subject, value);
        }

        private async void OnSendCommand()
        {
            IsUiEnabled = false;
            try
            {
                if (Subject.Length > 150) throw new Exception("Max subject length is 150 characters.");

                if (Body.Length > 150) throw new Exception("Max body length is 150 characters.");

                await SafeApp.SendMessageAsync(To,
                    new Message(SafeApp.Self.Name, Subject, DateTime.UtcNow.ToString("r"), Body));
                MessagingCenter.Send(this, MessengerConstants.NavPreviousPage);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Send Message Failed: {ex.Message}", "OK");
                IsUiEnabled = true;
            }
        }
    }
}
