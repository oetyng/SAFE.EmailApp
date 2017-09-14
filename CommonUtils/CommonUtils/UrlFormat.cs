using System;
using System.Text;

namespace CommonUtils {
  public class UrlFormat {
    public static string Convert(string inputUrl, bool toNativeLibs) {
      if (toNativeLibs) {
        inputUrl = inputUrl.Replace("://maidsafe.net/", ":");
        switch ((inputUrl.Length - inputUrl.IndexOf(":", StringComparison.Ordinal) - 1) % 4) {
          case 2:
            inputUrl += "==";
            break;
          case 3:
            inputUrl += "=";
            break;
        }
      } else {
        if (!inputUrl.StartsWith("safe-auth")) {
          var base64Pfx = inputUrl.Substring(5, inputUrl.IndexOf(':') - 5);
          var bytes = System.Convert.FromBase64String(base64Pfx);
          var normalPfx = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
          inputUrl = inputUrl.Replace($"{base64Pfx}:", $"{normalPfx}:");
        }
        inputUrl = inputUrl.Replace(":", "://maidsafe.net/").TrimEnd('=');
      }
      return inputUrl;
    }
  }
}
