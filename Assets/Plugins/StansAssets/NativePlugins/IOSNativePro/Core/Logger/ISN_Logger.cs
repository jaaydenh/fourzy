using UnityEngine;

namespace SA.iOS.Utilities
{

    public static class ISN_Logger
    {

        [RuntimeInitializeOnLoadMethod]
        static void Init() {
            if (Application.isEditor) { return; }
            var logLevel = ISN_Settings.Instance.LogLevel;
            ISN_LoggerNativeAPI.SetLogLevel(logLevel.Info, logLevel.Warning, logLevel.Error);
        }


        public static void Log(object message) {
            if (!ISN_Settings.Instance.LogLevel.Info) { return; }

            if(Application.isEditor) {
                Debug.Log(message);
            } else {
                ISN_LoggerNativeAPI.NativeLog("Info: " + message.ToString());
            }
        }


        public static void LogCommunication(string methodName, params string[] methodParams) {
            string message = methodName;

            for (int i = 0; i < methodParams.Length; i++) {
                if(i == 0) {
                    message += ":: ";
                } else {
                    message += "| ";
                }

                message += methodParams[i];
            }

            Log(message);
        }

        public static void LogWarning(object message) {
            if (!ISN_Settings.Instance.LogLevel.Warning) { return; }

            if (Application.isEditor) {
                Debug.LogWarning(message);
            } else {
                ISN_LoggerNativeAPI.NativeLog("Warning: " + message.ToString());
            }
        }


        public static void LogError(object message) {
            if (!ISN_Settings.Instance.LogLevel.Error) { return; }

            if (Application.isEditor) {
                Debug.LogError(message);
            } else {
                ISN_LoggerNativeAPI.NativeLog("Error: " + message.ToString());
            }
        }

    }
}