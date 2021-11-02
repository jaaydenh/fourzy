//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class InfinityTableMainScreen : MenuScreen
    {
        public override void OnBack()
        {
            base.OnBack();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
            using (var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                var activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call("finish");
            }
#endif
            Application.Quit();
        }

        public void PlayLocal()
        {
            menuController.GetScreen<PlayerPositioningPromptScreen>()._Prompt(OnPositioningSelected);
        }

        private void OnPositioningSelected()
        {
            Close();

            menuController.OpenScreen<AreaSelectLandscapeScreen>();
        }
    }
}