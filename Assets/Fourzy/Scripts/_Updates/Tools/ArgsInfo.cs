using UnityEngine;
using System;

public class ArgsInfo : MonoBehaviour
{
    static string cmdInfo = "";

    void Start()
    {
        Debug.Log(SystemInfo.deviceUniqueIdentifier);
        string[] arguments = Environment.GetCommandLineArgs();
        foreach (string arg in arguments)
        {
            cmdInfo += arg.ToString() + "\n ";
        }
    }

    void OnGUI()
    {
        Rect r = new Rect(5, 5, 800, 500);
        GUI.Label(r, cmdInfo);
    }
}