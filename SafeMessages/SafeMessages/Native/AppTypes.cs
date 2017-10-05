using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SafeMessages.Native {
  public struct AppKeys {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] OwnerKeys;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] EncKey;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] SignPk;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public byte[] SignSk;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] EncPk;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] EncSk;
  }

  public struct AccessContInfo {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] Id;
    public ulong Tag;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)] public byte[] SymNonce;
  }

  public struct AuthGrantedFfi {
    public AppKeys AppKeys;
    public AccessContInfo AccessContainer;
    public IntPtr BootStrapConfigPtr;
    public IntPtr BootStrapConfigLen;
    public IntPtr BootStrapConfigCap;
  }

  public struct AuthGranted {
    public AppKeys AppKeys;
    public AccessContInfo AccessContainer;
    public List<byte> BootStrapConfig;
  }

  public struct DecodeIpcResult {
    public AuthGranted? AuthGranted;
    public (IntPtr, IntPtr) UnRegAppInfo;
    public uint? ContReqId;
    public uint? ShareMData;
    public bool? Revoked;
  }
}
