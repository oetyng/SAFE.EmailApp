using System.Threading.Tasks;
using Android.Content;
using SafeMessages.Droid.Helpers;
using SafeMessages.Services;

[assembly: Xamarin.Forms.Dependency(typeof(AppHandler))]
namespace SafeMessages.Droid.Helpers
{
    class AppHandler : IAppHandler
    {
        public Task<bool> LaunchApp(string uri)
        {
            bool result = false;

            try
            {
                var aUri = Android.Net.Uri.Parse(uri.ToString());
                var intent = new Intent(Intent.ActionView, aUri);
#pragma warning disable CS0618 // Type or member is obsolete
                Xamarin.Forms.Forms.Context.StartActivity(intent);
#pragma warning restore CS0618 // Type or member is obsolete
                result = true;
            }
            catch (ActivityNotFoundException)
            {
                result = false;
            }

            return Task.FromResult(result);
        }
    }
}
