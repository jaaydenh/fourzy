using UnityEngine.SceneManagement;

namespace Fourzy
{
    public class LoadingManager : Singleton<LoadingManager>
    {
        public void LoadNextScene()
        {
            int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(activeSceneIndex + 1);
        }

        public void LoadPreviousScene()
        {
            int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(activeSceneIndex - 1);
        }
    }
}
