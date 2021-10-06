//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.GameplayScene;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SkillzPauseMenuScreen : MenuScreen
    {
        [SerializeField]
        private TMP_Text scoreLabel;

        private SkillzGameScreen skillzScreen;
        private PromptScreen forfeitPrompt;

        public override void Open()
        {
            base.Open();

            scoreLabel.text = SkillzGameController.Instance.Points + "";
        }

        public override void Close(bool animate = true)
        {
            base.Close(animate);
        }

        public void _Open()
        {
            menuController.OpenScreen(this);

            if (SkillzGameController.Instance.MatchPausesLeft > 0)
            {
                GamePlayManager.Instance.PauseGame();
                SkillzGameController.Instance.UsePause();
            }
        }

        public void _Close(bool animate = false)
        {
            CloseForfeitPrompt(animate);

            CloseSelf();
        }

        public void ShowRules()
        {

        }

        public void ToggleMusic()
        {
            SettingsManager.Toggle(SettingsManager.KEY_AUDIO);
        }

        public void ToggleSfx()
        {
            SettingsManager.Toggle(SettingsManager.KEY_SFX);
        }

        public void Continue()
        {
            if (GamePlayManager.Instance.gameState == GameState.PAUSED)
            {
                GamePlayManager.Instance.UnpauseGame();
            }

            _Close();
        }

        public void Forfeit()
        {
            forfeitPrompt = menuController.GetOrAddScreen<PromptScreen>()
                .Prompt(
                    LocalizationManager.Value("are_you_sure"),
                    LocalizationManager.Value("leave_skillz_game_message"),
                    LocalizationManager.Value("yes"),
                    LocalizationManager.Value("no"),
                    () =>
                    {
                        SkillzGameController.Instance.ForfeitMatch();
                        CloseSelf();
                    });
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            skillzScreen = menuController.GetScreen<SkillzGameScreen>();
        }

        private void CloseForfeitPrompt(bool animate = false)
        {
            if (forfeitPrompt && forfeitPrompt.isCurrent)
            {
                forfeitPrompt.CloseSelf(animate);
                forfeitPrompt = null;
            }
        }
    }
}
