//@vadym udod

#if UNITY_EDITOR

using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class FourzyPreBuld : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        ResourceDB _instance = ResourceDB.Instance;

        Debug.Log($"---- Before update: files, {_instance.FileCount} folders {_instance.FolderCount}");
        ResourceDB.Instance.UpdateDB();
        Debug.Log($"---- Resource DB updated: files, {_instance.FileCount} folders {_instance.FolderCount}");
    }
}

#endif