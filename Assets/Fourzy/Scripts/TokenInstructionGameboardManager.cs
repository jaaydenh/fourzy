using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    public class TokenInstructionGameboardManager : MonoBehaviour
    {
        [SerializeField]
        private GameBoardView gameBoardView;

        private GameState gameState;

        private string tokenBoardID;

        public void Init(string tokenBoardID)
        {
            this.tokenBoardID = tokenBoardID;

            gameBoardView.PlayerPiece = GameContentManager.Instance.GetGamePiecePrefab(0);
            gameBoardView.OpponentPiece = GameContentManager.Instance.GetGamePiecePrefab(1);

            this.StartCoroutine(PlayInstructionMovesRoutine());
        }

        public void Close()
        {
            this.StopAllCoroutines();

            gameBoardView.ResetGamePiecesAndTokens();
        }

        private void CreateGameBoard()
        {
            TokenBoard tokenBoard = TokenBoardLoader.instance.GetTokenBoard(tokenBoardID);
            gameState = new GameState(Constants.numRows,
                                                Constants.numColumns,
                                                GameType.PASSANDPLAY,
                                                true,
                                                true,
                                                tokenBoard,
                                                tokenBoard.initialGameBoard,
                                                false,
                                                null);
        }

        private IEnumerator PlayInstructionMovesRoutine()
        {
            yield return null;

            this.CreateGameBoard();
            gameBoardView.Init();
            gameBoardView.CreateGamePieceViews(gameState.GetGameBoard());
            gameBoardView.CreateTokenViews(gameState.PreviousTokenBoard.tokens);

            float repeatTime = 3.0f;
            float t = 2.0f;

            while(true)
            {
                t += Time.deltaTime;
                if (t > repeatTime)
                {
                    this.CreateGameBoard();
                    gameBoardView.ResetGamePiecesAndTokens();
                    gameBoardView.CreateGamePieceViews(gameState.GetGameBoard());
                    gameBoardView.CreateTokenViews(gameState.PreviousTokenBoard.tokens);
                    this.StartCoroutine(PlayInitialMoves());
                    t = 0;
                }
                yield return null;
            }
        }

        private IEnumerator PlayInitialMoves()
        {
            List<MoveInfo> initialMoves = gameState.TokenBoard.initialMoves;

            for (int i = 0; i < initialMoves.Count; i++)
            {
                Move move = new Move(initialMoves[i].Location, (Direction)initialMoves[i].Direction, i % 2 == 0 ? PlayerEnum.ONE : PlayerEnum.TWO);
                yield return StartCoroutine(MovePiece(move));
            }
        }

        private IEnumerator MovePiece(Move move)
        {
            List<IToken> activeTokens;
            List<MovingGamePiece> movingPieces = gameState.MovePiece(move, false, out activeTokens);
            gameBoardView.MoveGamePieceViews(move, movingPieces, activeTokens);

            yield return new WaitWhile(() => gameBoardView.NumPiecesAnimating > 0);
        }
    }
}
