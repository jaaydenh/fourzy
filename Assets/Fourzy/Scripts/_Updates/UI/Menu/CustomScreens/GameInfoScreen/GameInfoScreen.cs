//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Tween;
using TMPro;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GameInfoScreen : MenuScreen
    {
        public TextMeshProUGUI title;
        public TextMeshProUGUI subTitle;
        public ScaleTween scaleTween;

        public void Open(string titleText, string subtitleText)
        {
            title.text = titleText;
            subTitle.text = subtitleText;

            Open();
        }

        public override void Open()
        {
            if (isOpened)
                return;

            base.Open();
            scaleTween.PlayForward(true);
        }

        public override void Close(bool animate = true)
        {
            base.Close(animate);
            scaleTween.AtProgress(0f, PlaybackDirection.FORWARD);
        }

        public void SetData(IClientFourzy game)
        {
            switch (game._Type)
            {
                case GameType.PUZZLE:
                    if (game.IsWinner())
                        Open(/*game.puzzleChallengeInfo.Name + */LocalizationManager.Instance.GetLocalizedValue("challenge_completed_title"), 
                            LocalizationManager.Instance.GetLocalizedValue("challenge_completed_subtitle"));
                    else
                        Open(LocalizationManager.Instance.GetLocalizedValue("challenge_failed_title"), LocalizationManager.Instance.GetLocalizedValue("challenge_failed_subtitle"));
                    break;

                case GameType.PASSANDPLAY:
                    if (game.IsWinner())
                        Open("Player 1 won!", "");
                    else
                        Open("Player 2 won!", "");
                    break;

                case GameType.ONBOARDING:
                    break;

                case GameType.AI:
                case GameType.TURN_BASED:
                    if (game.IsWinner())
                        Open(game.me.DisplayName + LocalizationManager.Instance.GetLocalizedValue("won_suffix"), "");
                    else
                        Open(game.opponent.DisplayName + LocalizationManager.Instance.GetLocalizedValue("won_suffix"), "");
                    break;

                default:
                    Open(LocalizationManager.Instance.GetLocalizedValue("you_won_text"), "");
                    break;
            }
        }
    }
}