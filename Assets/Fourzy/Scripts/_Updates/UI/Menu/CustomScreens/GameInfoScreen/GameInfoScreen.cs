//@vadym udod

using Fourzy._Updates.Tools.Timing;
using TMPro;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GameInfoScreen : MenuScreen
    {
        public TextMeshProUGUI title;
        public TextMeshProUGUI subTitle;

        private AdvancedTimingEventsSet events;

        protected override void Awake()
        {
            base.Awake();

            events = GetComponent<AdvancedTimingEventsSet>();
        }

        public void Open(string titleText, string subtitleText)
        {
            menuController.OpenScreen(this);

            title.text = titleText;
            subTitle.text = subtitleText;
        }

        public override void Open()
        {
            base.Open();

            if (tween)
                tween.PlayForward(true);

            events.StartTimer();
        }

        public void SetData(Game game)
        {
            switch (game.gameState.GameType)
            {
                case GameType.PUZZLE:
                    if (GamePlayManager.Instance.IsPlayerWinner())
                        Open(game.puzzleChallengeInfo.Name + " Complete!", "continue to next level...");
                    else
                        Open("Level Failed.", "try again...");
                    break;

                case GameType.PASSANDPLAY:
                    if (game.gameState.Winner == PlayerEnum.ONE)
                        Open("Player 1 won!", "");
                    else
                        Open("Player 2 won!", "");
                    break;

                default:
                    Open("You won!", "");
                    break;
            }
        }
    }
}