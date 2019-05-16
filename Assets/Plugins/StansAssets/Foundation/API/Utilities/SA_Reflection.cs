using System;
using System.Reflection;

using UnityEngine;

namespace SA.Foundation.Utility
{

    /// <summary>
    /// Diffrent ultility methods to work with reflection
    /// </summary>
    public class SA_Reflection
    {

        /// <summary>
        /// Methods will iterate all the project Assemblies.
        /// If typeFullName will match new object instance of that type will be created 
        /// and returned as the result.
        /// </summary>
        /// <param name="typeFullName">full type name</param>
        /// <returns></returns>
        public static object CreateInstance(string typeFullName) {
            object instance = null;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                foreach (Type type in assembly.GetTypes()) {
                    if (type.FullName.Equals(typeFullName)) {
                        instance = Activator.CreateInstance(type);
                        return instance;
                    }
                }
            }

            return instance;
        }

    }
}