using System;
using System.Linq;
using Xamarin.Auth;

namespace SafeMessages.Services {
  internal class CredentialCacheService {
    private const string AuthRspKey = "AuthRsp";

    public void Delete() {
      try {
        var acctInfo = GetAccountInfo();
        AccountStore.Create().Delete(acctInfo, App.AppName);
      } catch (NullReferenceException) {
        // ignore acct not existing
      }
    }

    private static Account GetAccountInfo() {
      var acctInfo = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
      if (acctInfo == null) {
        throw new NullReferenceException("acctInfo");
      }
      return acctInfo;
    }

    public string Retrieve() {
      var acctInfo = GetAccountInfo();
      return acctInfo.Properties[AuthRspKey];
    }

    public void Store(string authRsp) {
      var acctInfo = new Account {Username = "CachedAcct"};
      acctInfo.Properties.Add(AuthRspKey, authRsp);
      AccountStore.Create().Save(acctInfo, App.AppName);
    }
  }
}
