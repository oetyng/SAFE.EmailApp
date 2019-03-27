using System.Windows.Input;
using SafeMessages.Helpers;
using SafeMessages.Models;
using Xamarin.Forms;

namespace SafeMessages.ViewModels
{
    internal class DisplayMessageViewModel : BaseViewModel
    {
        Message _message;

        public DisplayMessageViewModel(Message message)
        {
            IsUiEnabled = true;
            Message = message;
            ReplyCommand = new Command(OnReplyCommand);
        }

        public ICommand ReplyCommand { get; }

        public Message Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        void OnReplyCommand()
        {
            var subject = Message.Subject.StartsWith("Re: ") ? Message.Subject : $"Re: {Message.Subject}";
            MessagingCenter.Send(this, MessengerConstants.NavSendMessagePage, subject);
        }
    }
}