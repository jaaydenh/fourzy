using UnityEngine;
using UnityEditor;

/// <summary>
/// Internal class for working with Stan's Assets plugin
/// </summary>
public static class SA_PluginTools 
{
    /// <summary>
    /// Used to determin if we currently working inside Stan's Assets development environment
    /// Or this is live user project
    /// </summary>
    public static bool IsDevelopmentMode {
        get {
#if SA_DEVELOPMENT_PROJECT
            return true;
#else
            return false;
#endif
        }
    }
}