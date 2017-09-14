using CommonUtils;
using SafeAuthenticator.Services;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels {
  public class BaseViewModel : ObservableObject {
    public AuthService Authenticator => DependencyService.Get<AuthService>();
  }
}
