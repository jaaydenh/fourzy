//@vadym udod

using Fourzy;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevScene1Tester : MonoBehaviour
{
    protected void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene2()
    {
        SceneManager.LoadScene(Constants.MAIN_MENU_P_SCENE_NAME);
    }

    private void OnSceneUnloaded(Scene scene)
    {
        Debug.Log($"scene unloaded {scene.name}");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        Debug.Log($"scene loaded {scene.name} {loadMode}");
    }
}
