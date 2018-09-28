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
        private GameObject[,] tokenViews;

        private string tokenBoardID;

        public void Init(string tokenBoardID)
        {
            this.tokenBoardID = tokenBoardID;

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

        private void CreateTokenViews()
        {
            tokenViews = new GameObject[Constants.numRows, Constants.numColumns];

            TokenBoard previousTokenBoard = gameState.PreviousTokenBoard;
            IToken[,] previousTokenBoardTokens = previousTokenBoard.tokens;

            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    if (previousTokenBoardTokens[row, col] == null || previousTokenBoardTokens[row, col].tokenType == Token.EMPTY)
                    {
                        continue;
                    }

                    Token token = previousTokenBoardTokens[row, col].tokenType;

                    GameObject tokenPrefab = GameContentManager.Instance.GetTokenPrefab(token);

                    if (tokenPrefab)
                    {
                        gameBoardView.CreateToken(row, col, tokenPrefab);    
                    }
                }
            }
        }

        private IEnumerator PlayInstructionMovesRoutine()
        {
            yield return null;

            gameBoardView.Init();
            this.CreateGameBoard();
            this.CreateTokenViews();

            float repeatTime = 3.0f;
            float t = 2.0f;

            while(true)
            {
                t += Time.deltaTime;
                if (t > repeatTime)
                {
                    gameBoardView.ResetGamePiecesAndTokens();
                    this.CreateGameBoard();
                    this.CreateTokenViews();
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

            GameObject gamePiecePrefab = GameContentManager.Instance.GetGamePiecePrefab(0);

            GamePiece gamePiece = gameBoardView.SpawnPiece(move.position.row, move.position.column, gamePiecePrefab);
            gamePiece.player = move.player;
            gamePiece.column = move.position.column;
            gamePiece.row = move.position.row;

            gamePiece.Move(movingPieces, activeTokens);

            yield return new WaitWhile(() => gamePiece.isMoving);
        }
    }
}
