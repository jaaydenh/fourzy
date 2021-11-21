//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.UI.Helpers;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PauseMenuScreen : MenuScreen
    {
        [SerializeField]
        private TMP_Text scoreLabel;
        [SerializeField]
        private GameObject scorePanel;
        [SerializeField]
        private GameObject toDashboardButton;
        [SerializeField]
        private GameObject rulesButton;
        [SerializeField]
        private ButtonExtended exitButton;

        /// <summary>
        /// To be closed when this prompt is closed
        /// </summary>
        private PromptScreen secondaryPopup;

        public override void Open()
        {
            base.Open();

            switch (GamePlayManager.Instance.Game._Type)
            {
                case GameType.SKILLZ_ASYNC:
                    scorePanel.SetActive(true);
                    scoreLabel.text = SkillzGameController.Instance.Points + "";

                    break;

                default:
                    scorePanel.SetActive(false);

                    break;
            }

            //update rules button state
            switch (GamePlayManager.Instance.Game._Type)
            {
                case GameType.SKILLZ_ASYNC:
                    rulesButton.SetActive(true);

                    break;

                default:
                    rulesButton.SetActive(false);

                    break;
            }

            //update exit button
            switch (GamePlayManager.Instance.Game._Type)
            {
                case GameType.REALTIME:
                    exitButton.SetLabel(LocalizationManager.Value("forfeit"));

                    break;

                default:
                    exitButton.SetLabel(LocalizationManager.Value("exit"));

                    break;
            }
        }

        public void _Open()
        {
            menuController.OpenScreen(this);

            switch (GamePlayManager.Instance.Game._Type)
            {
                case GameType.SKILLZ_ASYNC:
                    if (SkillzGameController.Instance.MatchPausesLeft > 0)
                    {
                        GamePlayManager.Instance.PauseGame();
                        SkillzGameController.Instance.UsePause();
                    }

                    break;

                case GameType.PASSANDPLAY:
                    GamePlayManager.Instance.PauseGame();

                    break;
            }

            switch (GameManager.Instance.buildIntent)
            {
                case BuildIntent.MOBILE_INFINITY:
                    toDashboardButton.SetActive(true);

                    break;

                default:
                    toDashboardButton.SetActive(false);

                    break;
            }
        }

        public void _Close(bool animate = false)
        {
            CloseSecondaryPopup(animate);

            CloseSelf();
        }

        public void ExitToDashboard()
        {
            GameManager.Instance.CloseApp();
        }

        public void ShowRules()
        {
            switch (GamePlayManager.Instance.Game._Type)
            {
                case GameType.SKILLZ_ASYNC:
                    secondaryPopup = menuController.GetOrAddScreen<PromptScreen>().Prompt(LocalizationManager.Value("rules"), LocalizationManager.Value("rules_skillz_game"), "", LocalizationManager.Value("close"));

                    break;

                case GameType.REALTIME:
                    secondaryPopup = menuController.GetOrAddScreen<PromptScreen>().Prompt(LocalizationManager.Value("rules"), LocalizationManager.Value("rules_realtime_game"), "", LocalizationManager.Value("close"));

                    break;

                case GameType.PASSANDPLAY:
                    secondaryPopup = menuController.GetOrAddScreen<PromptScreen>().Prompt(LocalizationManager.Value("rules"), LocalizationManager.Value("rules_passandplay_game"), "", LocalizationManager.Value("close"));

                    break;
            }

            switch (GamePlayManager.Instance.Game._Mode)
            {
                case GameMode.GAUNTLET:
                    secondaryPopup = menuController.GetOrAddScreen<PromptScreen>().Prompt(LocalizationManager.Value("rules"), LocalizationManager.Value("rules_gauntlet_game"), "", LocalizationManager.Value("close"));

                    break;

                case GameMode.PUZZLE_PACK:
                case GameMode.PUZZLE_FAST:
                    secondaryPopup = menuController.GetOrAddScreen<PromptScreen>().Prompt(LocalizationManager.Value("rules"), LocalizationManager.Value("rules_puzzlepack_game"), "", LocalizationManager.Value("close"));

                    break;
            }
        }

        public void ToggleMusic()
        {
            SettingsManager.Toggle(SettingsManager.KEY_AUDIO);
        }

        public void ToggleSfx()
        {
            SettingsManager.Toggle(SettingsManager.KEY_SFX);
        }

        /// <summary>
        /// Invoked by contunie button
        /// </summary>
        public void Continue()
        {
            switch (GamePlayManager.Instance.Game._Type)
            {
                case GameType.SKILLZ_ASYNC:
                case GameType.PASSANDPLAY:
                    if (GamePlayManager.Instance.GameState == GameState.PAUSED)
                    {
                        GamePlayManager.Instance.UnpauseGame();
                    }

                    break;
            }

            _Close();
        }

        /// <summary>
        /// Invoked by forfeit button
        /// </summary>
        public void Forfeit()
        {
            switch (GamePlayManager.Instance.Game._Type)
            {
                case GameType.PUZZLE:
                case GameType.TURN_BASED:
                case GameType.PRESENTATION:
                    GamePlayManager.Instance.BackButtonOnClick();

                    break;

                case GameType.SKILLZ_ASYNC:
                    secondaryPopup = menuController.GetOrAddScreen<PromptScreen>()
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

                    break;

                case GameType.REALTIME:
                    if (GamePlayManager.Instance.Game.IsOver)
                    {
                        GamePlayManager.Instance.BackButtonOnClick();
                    }
                    else
                    {
                        secondaryPopup = menuController.GetOrAddScreen<PromptScreen>()
                            .Prompt(
                                LocalizationManager.Value("are_you_sure"),
                                LocalizationManager.Value("leave_realtime_game_message"),
                                LocalizationManager.Value("yes"),
                                LocalizationManager.Value("no"),
                                () =>
                                {
                                    GamePlayManager.Instance.BackButtonOnClick();
                                });
                    }

                    break;

                default:
                    if (GamePlayManager.Instance.Game.IsOver)
                    {
                        GamePlayManager.Instance.BackButtonOnClick();
                    }
                    else
                    {
                        menuController.GetOrAddScreen<PromptScreen>()
                            .Prompt(
                                LocalizationManager.Value("leave_game"),
                                "",
                                LocalizationManager.Value("yes"),
                                LocalizationManager.Value("no"),
                                () => GamePlayManager.Instance.BackButtonOnClick());
                    }

                    break;
            }
        }

        private void CloseSecondaryPopup(bool animate = false)
        {
            if (secondaryPopup && secondaryPopup.isCurrent)
            {
                secondaryPopup.CloseSelf(animate);
                secondaryPopup = null;
            }
        }
    }
}
