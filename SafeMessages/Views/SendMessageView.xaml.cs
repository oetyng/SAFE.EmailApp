﻿using SafeMessages.Helpers;
using SafeMessages.Models;
using SafeMessages.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMessages.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendMessageView : ContentPage, ICleanup
    {
        public SendMessageView()
            : this(null, string.Empty, string.Empty)
        {
        }

        public SendMessageView(UserId userId, string subject, string inReplyTo)
        {
            InitializeComponent();
            var viewModel = new SendMessageViewModel(userId, subject, inReplyTo);
            BindingContext = viewModel;
            MessagingCenter.Subscribe<SendMessageViewModel>(
                this,
                MessengerConstants.NavPreviousPage,
                async sender =>
                {
                    MessageCenterUnsubscribe();
                    if (!App.IsPageValid(this))
                        return;

                    await Navigation.PopAsync();
                });
        }

        public void MessageCenterUnsubscribe()
            => MessagingCenter.Unsubscribe<SendMessageViewModel>(this, MessengerConstants.NavPreviousPage);
    }
}