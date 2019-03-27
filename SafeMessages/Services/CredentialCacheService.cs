using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SafeMessages.Services
{
    internal class CredentialCacheService
    {
        const string AuthRspKey = "AuthRsp";

        public void Delete()
        {
            try
            {
                SecureStorage.RemoveAll();
            }
            catch (Exception)
            {
                // ignore acct not existing
            }
        }

        public async Task<string> Retrieve()
        {
            var authResponse = await SecureStorage.GetAsync(AuthRspKey);
            if (authResponse == null)
                throw new NullReferenceException("authResponse");

            return authResponse;
        }

        public Task Store(string authRsp)
            => SecureStorage.SetAsync(AuthRspKey, authRsp);
    }
}