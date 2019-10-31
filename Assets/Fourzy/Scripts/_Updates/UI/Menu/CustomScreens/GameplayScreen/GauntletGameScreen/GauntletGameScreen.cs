//vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Helpers;
using System.Collections;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GauntletGameScreen : MenuScreen
    {
        public RectTransform pieceParent;
        public Badge movesLeft;

        public IClientFourzy game { get; private set; }

        private bool isGauntlet => game.puzzleData && game.puzzleData.gauntletStatus != null;
        private GamePieceView gamepiece;
        private Animator textAnimator;

        protected override void Awake()
        {
            base.Awake();

            textAnimator = movesLeft.GetComponentInChildren<Animator>();
        }

        public void Open(IClientFourzy game)
        {
            this.game = game;

            if (game == null || !isGauntlet) return;

            movesLeft.SetValue(game.myMembers.Count);

            if (gamepiece && gamepiece.pieceData.ID != game.me.HerdId)
            {
                Destroy(gamepiece.gameObject);
                gamepiece = AddGamepiece(game.me.HerdId);
            }
            else if (!gamepiece)
                gamepiece = AddGamepiece(game.me.HerdId);

            base.Open();
        }

        public void OnMoveStarted()
        {
            if (!isOpened || !isGauntlet || game.isMyTurn) return;

            movesLeft.SetValue(game.myMembers.Count);
            textAnimator.SetTrigger("animate");
        }

        public void UpdatePlayerTurn()
        {

        }

        private GamePieceView AddGamepiece(string id)
        {
            GamePieceView _gamepiece = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(id).player1Prefab, pieceParent);
            _gamepiece.StartBlinking();

            return _gamepiece;
        }
    }
}