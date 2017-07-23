#if NETFX_CORE // Workaround to support changes in the .Net reflection API for Windows Store Apps

using System;
using System.Reflection;

namespace UnityEngine.Analytics.Experimental.Tracker
{
    public static class TypeExtensions
    {
        public static PropertyInfo GetProperty(this Type type, string propertyName)
        {
            return type.GetTypeInfo().GetDeclaredProperty(propertyName);
        }

        public static FieldInfo GetField(this Type type, string fieldName)
        {
            return type.GetTypeInfo().GetDeclaredField(fieldName);
        }
    }
}
#endif
