//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.UI.Widgets;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class RandomPlayerPickScreen : MenuScreen
    {
        private RandomPickWidget randomPickWidget;

        protected override void Awake()
        {
            base.Awake();

            randomPickWidget = GetComponentInChildren<RandomPickWidget>();
        }

        public void SetData(IClientFourzy game)
        {
            CancelRoutine("playerPicked");

            switch (game._Type)
            {
                case GameType.TURN_BASED:
                    if (game.asFourzyGame.challengeData.haveMoves)
                    {
                        if (isOpened) CloseSelf();
                    }
                    else
                        randomPickWidget.SetData(game.activePlayer.DisplayName, game.unactivePlayer.DisplayName);

                    break;

                case GameType.REALTIME:
                case GameType.PASSANDPLAY:
                    randomPickWidget.SetData(game.activePlayer.DisplayName, game.unactivePlayer.DisplayName);

                    break;
            }
        }

        public void _Open()
        {
            if (menuController.currentScreen != this) menuController.OpenScreen(this);

            randomPickWidget.StartPick(1f, true);

            StartRoutine("playerPicked", RandomPickWidget.ANIMATION_TIME + 1.1f, DisplayWinner, randomPickWidget.Cancel);
        }

        public void DisplayWinner()
        {
            //assign new active player
            GamePlayManager.instance.UpdatePlayerTurn();

            StartRoutine("fadeRoutine", .7f, () => CloseSelf());
        }
    }
}