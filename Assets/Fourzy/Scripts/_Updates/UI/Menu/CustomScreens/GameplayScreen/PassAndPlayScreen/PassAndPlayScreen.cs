//@vadym udod

using Fourzy._Updates.ClientModel;

namespace Fourzy._Updates.UI.Menu
{
    public class PassAndPlayScreen : MenuScreen
    {
        private ClientFourzyGame game;

        public void Open(IClientFourzy game)
        {
            this.game = game.asFourzyGame;

            base.Open();
        }

        public void GameComplete()
        {

        }

        public void UpdatePlayerTurn()
        {
            if (game == null || game._Type != GameType.PASSANDPLAY) return;
        }
    }
}