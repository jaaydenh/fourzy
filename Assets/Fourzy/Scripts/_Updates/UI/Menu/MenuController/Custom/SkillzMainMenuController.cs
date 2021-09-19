//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.Managers;
using SkillzSDK;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu
{
    public class SkillzMainMenuController : MenuController
    {
        public static SkillzMainMenuController Instance;

        protected override void Awake()
        {
            base.Awake();

            Instance = this;
        }

        protected override void Start()
        {
            base.Start();

            //play bg audio
            if (!AudioHolder.instance.IsBGAudioPlaying("bg_main_menu"))
            {
                AudioHolder.instance.PlayBGAudio("bg_main_menu", true, .75f, 1f);
            }
        }

        public void StartSkillzUI()
        {
            SkillzCrossPlatform.LaunchSkillz(SkillzGameController.Instance);
        }
    }
}
