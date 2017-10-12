using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SafeAuthenticator.Native {
  public enum MDataAction {
    kInsert,
    kUpdate,
    kDelete,
    kManagePermissions
  }

  public struct FfiResult {
    public int ErrorCode;
    public string Description;
  }

  public struct AppExchangeInfo {
    public string Id;
    public string Scope;
    public string Name;
    public string Vendor;
  }

  public struct PermissionSet {
    [MarshalAs(UnmanagedType.U1)] public bool Read;
    [MarshalAs(UnmanagedType.U1)] public bool Insert;
    [MarshalAs(UnmanagedType.U1)] public bool Update;
    [MarshalAs(UnmanagedType.U1)] public bool Delete;
    [MarshalAs(UnmanagedType.U1)] public bool ManagePermissions;
  }

  public struct ContainerPermissions {
    public string ContainerName;
    public PermissionSet Access;
  }

  public struct AuthReq {
    public AppExchangeInfo AppExchangeInfo;
    public bool AppContainer;
    public List<ContainerPermissions> Containers;
  }

  public struct AuthReqFfi {
    public AppExchangeInfo AppExchangeInfo;
    [MarshalAs(UnmanagedType.U1)] public bool AppContainer;
    public IntPtr ContainersArrayPtr;
    public IntPtr ContainersLen;
    public IntPtr ContainersCap;
  }

  public struct AccountInfo {
    public ulong Used;
    public ulong Available;
  }

  public struct RegisteredAppFfi {
    public AppExchangeInfo AppExchangeInfo;
    public IntPtr ContainersArrayPtr;
    public IntPtr ContainersLen;
    public IntPtr ContainersCap;
  }

  public struct DecodeIpcResult {
    public AuthReq? AuthReq;
    public uint? UnRegAppReq;
    public IntPtr? ContReq;
    public (IntPtr, IntPtr) ShareMDataReq;
  }
}
