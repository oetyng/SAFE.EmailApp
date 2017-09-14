using System.Collections.Generic;
using System.Threading;

namespace CommonUtils {
  public static class CallbackManager {
    private static readonly List<object> Callbacks = new List<object>();
    public static readonly Mutex Mutex = new Mutex();

    public static void Register(object callback) {
      Mutex.WaitOne();
      Callbacks.Add(callback);
      Mutex.ReleaseMutex();
    }

    public static void Register(List<object> callbacks) {
      Mutex.WaitOne();
      Callbacks.Add(callbacks);
      Mutex.ReleaseMutex();
    }

    public static void Unregister(object callback) {
      Mutex.WaitOne();
      Callbacks.Remove(callback);
      Mutex.ReleaseMutex();
    }

    public static void Unregister(List<object> callbacks) {
      Mutex.WaitOne();
      Callbacks.Remove(callbacks);
      Mutex.ReleaseMutex();
    }
  }
}
