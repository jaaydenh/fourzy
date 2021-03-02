using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class DeviceIDManager
{
    public static Action<string> onDeviceID;

    public static void GetDeviceID()
    {
        string password = string.Empty;
        password = SystemInfo.deviceUniqueIdentifier;
        onDeviceID?.Invoke(password);
    }
}
