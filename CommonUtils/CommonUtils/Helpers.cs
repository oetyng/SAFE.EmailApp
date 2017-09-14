using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CommonUtils {
  public static class Helpers {
    public static IntPtr StructToPtr<T>(T obj) {
      var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(obj));
      Marshal.StructureToPtr(obj, ptr, false);
      return ptr;
    }

    public static Exception ToException(this FfiResult result) {
      return new Exception($"Error Code: {result.ErrorCode}. Description: {result.Description}");
    }

    public static IntPtr ToIntPtr<T>(this List<T> list) {
      var structSize = Marshal.SizeOf(typeof(T));
      var ptr = Marshal.AllocHGlobal(structSize * list.Count);
      for (var i = 0; i < list.Count; ++i) {
        Marshal.StructureToPtr(list[i], ptr + structSize * i, false);
      }
      return ptr;
    }

    public static List<T> ToList<T>(this IntPtr ptr, IntPtr length) {
      var list = new List<T>();
      var structSize = Marshal.SizeOf(typeof(T));

      for (var i = 0; i < (int)length; ++i) {
        list.Add((T)Marshal.PtrToStructure(ptr + structSize * i, typeof(T)));
      }
      return list;
    }

    public static ObservableRangeCollection<T> ToObservableRangeCollection<T>(this IEnumerable<T> source) {
      var result = new ObservableRangeCollection<T>();
      foreach (var item in source) {
        result.Add(item);
      }
      return result;
    }

    #region Encoding Extensions

    public static string ToUtfString(this List<byte> input) {
      var ba = input.ToArray();
      return Encoding.UTF8.GetString(ba, 0, ba.Length);
    }

    public static List<byte> ToUtfBytes(this string input) {
      var byteArray = Encoding.UTF8.GetBytes(input);
      return byteArray.ToList();
    }

    public static string ToHexString(this List<byte> byteList) {
      var ba = byteList.ToArray();
      var hex = BitConverter.ToString(ba);
      return hex.Replace("-", "").ToLower();
    }

    public static List<byte> ToHexBytes(this string hex) {
      var numberChars = hex.Length;
      var bytes = new byte[numberChars / 2];
      for (var i = 0; i < numberChars; i += 2) {
        bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
      }
      return bytes.ToList();
    }

    public static string PrintByteArray(List<byte> bytes) {
      var sb = new StringBuilder("new byte[] { ");
      foreach (var b in bytes) {
        sb.Append(b + ", ");
      }
      sb.Append("}");
      return sb.ToString();
    }

    #endregion
  }
}
