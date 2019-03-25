using System.Threading.Tasks;

namespace SafeMessages.Services
{
    public interface IAppHandler
    {
        Task<bool> LaunchApp(string uri);
    }
}
