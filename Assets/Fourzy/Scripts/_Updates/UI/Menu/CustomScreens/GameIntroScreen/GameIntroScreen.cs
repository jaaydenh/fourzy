//@vadym udod

using TMPro;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GameIntroScreen : MenuScreen
    {
        public TextMeshProUGUI titleLabel;
        public TextMeshProUGUI subtitleLabel;

        public void Open(string titleText, string subtitleText)
        {
            titleLabel.text = titleText;
            subtitleLabel.text = subtitleText;

            menuController.OpenScreen(this);
        }

        public void Init(Game game)
        {
            if (!game.displayIntroUI)
                return;

            string title = game.title;
            string subtitle = game.subtitle;
            if (subtitle == "")
            {
                switch (game.gameState.GameType)
                {
                    case GameType.PASSANDPLAY:
                        subtitle = LocalizationManager.Instance.GetLocalizedValue("pnp_button");
                        break;
                    case GameType.FRIEND:
                        subtitle = LocalizationManager.Instance.GetLocalizedValue("friend_challenge_text");
                        break;
                    case GameType.LEADERBOARD:
                        subtitle = LocalizationManager.Instance.GetLocalizedValue("leaderboard_challenge_text");
                        break;
                    case GameType.RANDOM:
                        subtitle = LocalizationManager.Instance.GetLocalizedValue("random_opponent_button");
                        break;
                    case GameType.AI:
                        subtitle = LocalizationManager.Instance.GetLocalizedValue("ai_challenge_text");
                        break;
                    default:
                        break;
                }
            }

            if (title == "")
                title = game.gameState.TokenBoard.name;

            Open(title, subtitle);
        }
    }
}
