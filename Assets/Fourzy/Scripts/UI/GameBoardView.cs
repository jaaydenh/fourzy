using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace Fourzy
{
    public class GameBoardView : MonoBehaviour 
    {
        public Transform gamePiecesRootTransform;
        public Transform tokensRootTransform;

        public GamePiece[,] gamePieces; //Collection of GamePiece Views
        public GameObject[,] tokens; //Collection of Token Views
        [Range(3, 8)]
        public int numRows = Constants.numRows;
        [Range(3, 8)]
        public int numColumns = Constants.numRows;

        private SpriteRenderer spriteRenderer;
        private BoxCollider2D cellsArea;
        private Transform cachedTransform;

        void Awake () 
        {
            tokens = new GameObject[numRows, numColumns];
            gamePieces = new GamePiece[numRows, numColumns];

            cachedTransform = this.transform;
            cellsArea = this.GetComponent<BoxCollider2D>();
            spriteRenderer = this.GetComponent<SpriteRenderer>();

            this.CalculatePositions();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (cachedTransform.hasChanged)
            {
                CalculatePositions();
            }
        }
#endif

        public void CalculatePositions()
        {
            BoxCollider2D boxCollider = cellsArea;

            float top = boxCollider.offset.y + (boxCollider.size.y / 2f);
            float btm = boxCollider.offset.y - (boxCollider.size.y / 2f);
            float left = boxCollider.offset.x - (boxCollider.size.x / 2f);
            float right = boxCollider.offset.x + (boxCollider.size.x / 2f);

            Vector3 topLeft = cachedTransform.TransformPoint(new Vector3(left, top, 0f));
            Vector3 btmRight = cachedTransform.TransformPoint(new Vector3(right, btm, 0f));

            float stepX = (btmRight.x - topLeft.x) / Constants.numColumns;
            float stepY = (topLeft.y - btmRight.y) / Constants.numRows;

            Position.topLeft = topLeft;
            Position.step = new Vector3(stepX, stepY);
        }

        public void SwapPiecePosition(Position oldPos, Position newPos) 
        {
            GamePiece gamePiece = gamePieces[oldPos.row, oldPos.column];
            
            gamePiece.column = newPos.column;
            gamePiece.row = newPos.row;
            gamePieces[oldPos.row, oldPos.column] = null;
            gamePieces[newPos.row, newPos.column] = gamePiece;
        }

        public GamePiece GamePieceAt(Position position)
        {            
            return gamePieces[position.row, position.column];
        }

        public List<GamePiece> GetGamePiecesList()
        {
            List<GamePiece> list = new List<GamePiece>();

            for (int i = 0; i < Constants.numColumns; i++)
            {
                for (int j = 0; j < Constants.numRows; j++)
                {
                    if (gamePieces[i, j] != null)
                    {
                        list.Add(gamePieces[i, j]);
                    }
                }
            }

            return list;
        }

        public List<GamePiece> GetWaitingGamePiecesList()
        {
            List<GamePiece> list = new List<GamePiece>();

            for (int i = 0; i < Constants.numColumns; i++)
            {
                for (int j = 0; j < Constants.numRows; j++)
                {
                    if (gamePieces[i, j] != null && !gamePieces[i, j].isMoving)
                    {
                        list.Add(gamePieces[i, j]);
                    }
                }
            }

            return list;
        }

        public void ResetGamePiecesAndTokens()
        {
            for (int i = gamePiecesRootTransform.childCount - 1; i >= 0; i--)
            {
                Transform piece = gamePiecesRootTransform.GetChild(i);
                piece.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                Destroy(piece.gameObject);
                //LeanPool.Despawn(piece.gameObject);
            }

            for (int i = tokensRootTransform.childCount - 1; i >= 0; i--)
            {
                Transform token = tokensRootTransform.GetChild(i);
                DestroyImmediate(token.gameObject);
            }

            this.Clear();
        }

        public void Clear() 
        {
            gamePieces = new GamePiece[numRows, numColumns];
        }

        public void PrintGameBoard() 
        {
            string gameboard = "GameboardView: \n";

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numColumns; col++)
                {
                    if (gamePieces[row, col]) {
                        gameboard += (int)gamePieces[row, col].player;
                    } else {
                        gameboard += "0";
                    }
                }
                gameboard += "\n";
            }
            Debug.Log(gameboard);
        }

        public void SetAlpha(float alpha)
        {
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
        }

        public void Fade(float alpha, float time)
        {
            spriteRenderer.DOFade(alpha, time);
        }
    }
}