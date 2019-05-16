using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


using System.Runtime.InteropServices;



namespace SA.iOS.Utilities
{
    public static class ISN_MonoPCallback
    {
        #if UNITY_IPHONE || UNITY_TVOS
        [DllImport("__Internal")] static extern void RegisterCallbackDelegate(MonoPCallbackDelegate callbackDelegate);
        #endif


        private delegate void MonoPCallbackDelegate(IntPtr actionPtr, string data);

#if UNITY_IPHONE || UNITY_TVOS
        [RuntimeInitializeOnLoadMethod]
         private static void Initialize() {
            if(!Application.isEditor) {
                RegisterCallbackDelegate(MonoPCallbackInvoke);
            }
         }
#endif


        [AOT.MonoPInvokeCallback(typeof(MonoPCallbackDelegate))]
        private static void MonoPCallbackInvoke(IntPtr actionPtr, string data) {
            if (IntPtr.Zero.Equals(actionPtr)) {
                return;
            }

            // Возвращаем по указателю хранящийся там Action
            var action = IntPtrToObject(actionPtr, false);
            if (action == null) {
                Debug.LogError("Callaback not found");
                return;
            }

            try {

               
                var paramTypes = action.GetType().GetGenericArguments();
                var arg = paramTypes.Length == 0 ? null : ConvertObject(data, paramTypes[0]);

                // Вызываем Action с передачей ему данных колбека,
                // приведенных к нужному типу
                var invokeMethod = action.GetType().GetMethod("Invoke", paramTypes.Length == 0 ? new Type[0] : new[] { paramTypes[0] });
                if (invokeMethod != null) {
                    invokeMethod.Invoke(action, paramTypes.Length == 0 ? new object[] { } : new[] { arg });
                } else {
                    Debug.LogError("Failed to invoke callback " + action + " with arg " + data + ": invoke method not found");
                }
            } catch (Exception e) {
                Debug.LogError("Failed to invoke callback " + action + " with arg " + data + ": " + e.Message);
            }
        }


        private static object ConvertObject(string value, Type objectType) {
            if (value == null || objectType == typeof(string)) {
                return value;
            }

            if(objectType == typeof(bool)) {
                return Convert.ToBoolean(value);
            }

            if (objectType == typeof(int)) {
                return Convert.ToInt32(value);
            }

            if (objectType == typeof(long)) {
                return Convert.ToInt64(value);
            }

            if (objectType == typeof(float)) {
                return Convert.ToSingle(value);
            }

            return JsonUtility.FromJson(value, objectType);
        }




        // Функция получения указателя для переданного объекта
        public static IntPtr ObjectToIntPtr(object obj) {
            if (obj == null) {
                return IntPtr.Zero;
            }

            var handle = GCHandle.Alloc(obj);
            return GCHandle.ToIntPtr(handle);
        }

        // Вспомогательная функция, потребуется в дальнейшем
        public static IntPtr ActionToIntPtr<T>(Action<T> action) {
            return ObjectToIntPtr(action);
        }

        // Функция получения объекта по его указателю
        public static object IntPtrToObject(IntPtr handle, bool unpinHandle) {
            if (IntPtr.Zero.Equals(handle)) {
                return null;
            }

            var gcHandle = GCHandle.FromIntPtr(handle);
            var result = gcHandle.Target;
            if (unpinHandle) {
                gcHandle.Free();
            }
            return result;
        }
    }
}