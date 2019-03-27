using SafeMessages.Models;
using SafeMessages.Models.BaseModel;
using Xamarin.Forms;

[assembly: Dependency(typeof(DataModel))]

namespace SafeMessages.Models
{
    public class DataModel : ObservableObject
    {
        public DataModel()
        {
            Accounts = new ObservableRangeCollection<UserId>();
            Messages = new ObservableRangeCollection<Message>();
        }

        public ObservableRangeCollection<UserId> Accounts { get; set; }
        public ObservableRangeCollection<Message> Messages { get; set; }
        public void ClearMessages() => Messages.Clear();
    }
}