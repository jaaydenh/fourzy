using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class DeviceIDManager
{
    public static Action<string> onDeviceID;


#if UNITY_IPHONE && !UNITY_EDITOR
    [DllImport("__Internal")]
    static extern string _Get_Device_id();
#endif

    public static void GetDeviceID()
    {
        string password = string.Empty;

#if UNITY_IPHONE && !UNITY_EDITOR
		password = _Get_Device_id();
        onDeviceID?.Invoke(password);
#else
        password = SystemInfo.deviceUniqueIdentifier;
        onDeviceID?.Invoke(password);
#endif
    }
}
