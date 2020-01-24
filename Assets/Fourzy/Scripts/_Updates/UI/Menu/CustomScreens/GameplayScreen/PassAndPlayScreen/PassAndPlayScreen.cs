//@vadym udod

using Fourzy._Updates.ClientModel;

namespace Fourzy._Updates.UI.Menu
{
    public class PassAndPlayScreen : MenuScreen
    {
        public IClientFourzy game { get; private set; }

        public void Open(IClientFourzy game)
        {
            if (game == null) return;

            this.game = game;

            base.Open();
        }

        public void _Update()
        {

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