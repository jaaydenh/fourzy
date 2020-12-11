//vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Helpers;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GauntletGameScreen : MenuScreen
    {
        public RectTransform pieceParent;
        public Badge movesLeft;

        public IClientFourzy game { get; private set; }

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

            if (game == null) return;

            switch (game._Mode)
            {
                case GameMode.GAUNTLET:
                    if (gamepiece && gamepiece.pieceData.ID != game.me.HerdId)
                    {
                        Destroy(gamepiece.gameObject);
                        gamepiece = AddGamepiece(game.me.HerdId);
                    }
                    else if (!gamepiece) gamepiece = AddGamepiece(game.me.HerdId);

                    Open();

                    break;

                default:
                    if (isOpened) Close();

                    break;
            }
        }

        public override void Open()
        {
            UpdateCounter();

            base.Open();
        }

        public void _Update() { }

        public void OnMoveStarted()
        {
            if (!isOpened || game._Mode != GameMode.GAUNTLET || game.isMyTurn) return;

            UpdateCounter();
        }

        public void UpdatePlayerTurn() { }

        public void UpdateCounter()
        {
            movesLeft.SetValue(game.myMembers.Count);
            textAnimator.SetTrigger("animate");
        }

        private GamePieceView AddGamepiece(string id)
        {
            GamePieceView _gamepiece = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(id).player1Prefab, pieceParent);
            _gamepiece.StartBlinking();

            return _gamepiece;
        }
    }
}