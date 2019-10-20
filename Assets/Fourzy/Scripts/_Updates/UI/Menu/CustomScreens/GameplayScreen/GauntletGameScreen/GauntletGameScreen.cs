//vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Helpers;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GauntletGameScreen : MenuScreen
    {
        public Badge movesLeft;

        public IClientFourzy game { get; private set; }

        private bool isGauntlet => game.puzzleData && game.puzzleData.gauntletStatus != null;

        public void Open(IClientFourzy game)
        {
            this.game = game;

            if (game == null || !isGauntlet) return;

            movesLeft.SetValue(game.puzzleData.gauntletStatus.FourzyCount);

            base.Open();
        }

        public void OnMoveStarted()
        {
            if (!isOpened || !isGauntlet) return;

            movesLeft.SetValue(game.puzzleData.gauntletStatus.FourzyCount);
        }

        public void UpdatePlayerTurn()
        {

        }
    }
}