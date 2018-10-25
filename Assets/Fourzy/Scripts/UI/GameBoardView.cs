using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

namespace Fourzy
{
    public class GameBoardView : MonoBehaviour 
    {
        [SerializeField]
        private Transform gamePiecesRootTransform;

        [SerializeField]
        private Transform tokensRootTransform;

        public int NumPiecesAnimating { get; set; }
        public GamePiece PlayerPiece { get; set; }
        public GamePiece OpponentPiece { get; set; }

        public GamePiece[,] gamePieces; 
        public GameObject[,] tokens; 
        [Range(3, 8)]
        public int numRows = Constants.numRows;
        [Range(3, 8)]
        public int numColumns = Constants.numRows;

        private SpriteRenderer spriteRenderer;
        private BoxCollider2D cellsArea;
        private Transform cachedTransform;

        private Vector3 topLeft;
        private Vector3 step;

        private bool isInitialized;

        void Awake () 
        {
            this.Init();
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
        public void Init()
        {
            if (isInitialized)
            {
                return;
            }

            tokens = new GameObject[numRows, numColumns];
            gamePieces = new GamePiece[numRows, numColumns];

            cachedTransform = this.transform;
            cellsArea = this.GetComponent<BoxCollider2D>();
            spriteRenderer = this.GetComponent<SpriteRenderer>();

            this.CalculatePositions();

            isInitialized = true;
        }

        private void CalculatePositions()
        {
            BoxCollider2D boxCollider = cellsArea;

            float top = boxCollider.offset.y + (boxCollider.size.y / 2f);
            float btm = boxCollider.offset.y - (boxCollider.size.y / 2f);
            float left = boxCollider.offset.x - (boxCollider.size.x / 2f);
            float right = boxCollider.offset.x + (boxCollider.size.x / 2f);

            topLeft = cachedTransform.TransformPoint(new Vector3(left, top, 0f));
            Vector3 btmRight = cachedTransform.TransformPoint(new Vector3(right, btm, 0f));

            float stepX = (btmRight.x - topLeft.x) / Constants.numColumns;
            float stepY = (topLeft.y - btmRight.y) / Constants.numRows;

            step = new Vector3(stepX, stepY);
        }

        public Vector3 PositionToVec3(Position position, float z = 0.0f)
        {
            return this.PositionToVec3(position.row, position.column, z);
        }

        public Vector3 PositionToVec3(int row, int column, float z = 0.0f)
        {
            float posX = topLeft.x + step.x * 0.5f + step.x * column;
            float posY = topLeft.y - step.y * 0.5f - step.y * row;
            return new Vector3(posX, posY, transform.position.z + z);
        }

        public Position Vec3ToPosition(Vector3 vec3)
        {
            int x = Mathf.FloorToInt((vec3.x - topLeft.x) / step.x);
            int y = Mathf.FloorToInt(-(vec3.y - topLeft.y) / step.y);

            return new Position(x, y);
        }

        public void CreateGamePieceViews(int[,] board, float initialAlpha = 1.0f)
        {
            gamePieces = new GamePiece[Constants.numRows, Constants.numColumns];

            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    int piece = board[row, col];
                    if (piece == (int)Piece.EMPTY)
                    {
                        continue;
                    }

                    PlayerEnum player = (piece == (int)Piece.BLUE) ? PlayerEnum.ONE : PlayerEnum.TWO;

                    GamePiece pieceObject = SpawnPiece(col, row, player);
                    pieceObject.player = player;
                    gamePieces[row, col] = pieceObject;
                    pieceObject.View.SetAlpha(initialAlpha);
                }
            }
        }

        public void CreateTokenViews(IToken[,] tokens, float initialAlpha = 1.0f)
        {
            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    if (tokens[row, col] == null || tokens[row, col].tokenType == Token.EMPTY)
                    {
                        continue;
                    }

                    Token token = tokens[row, col].tokenType;

                    GameObject tokenPrefab = GameContentManager.Instance.GetTokenPrefab(token);

                    if (tokenPrefab)
                    {
                        this.CreateToken(row, col, tokenPrefab);
                        this.TokenAt(row, col).GetComponent<SpriteRenderer>().SetAlpha(initialAlpha);
                    }
                }
            }
        }

        public void MoveGamePieceViews(Move move, List<MovingGamePiece> movingPieces, List<IToken> activeTokens)
        {
            GamePiece gamePiece = this.SpawnPiece(move.position.column, move.position.row, move.player);
            gamePiece.player = move.player;
            gamePiece.column = move.position.column;
            gamePiece.row = move.position.row;

            var copyGamePieces = new GamePiece[numRows, numColumns];
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    copyGamePieces[i, j] = gamePieces[i, j];
                }
            }

            GamePiece movingPiece = gamePiece;
            for (int i = 0; i < movingPieces.Count; i++)
            {
                MovingGamePiece mgp = movingPieces[i];
                Position end = mgp.endPosition;
                mgp.gamePiece = movingPiece;
                movingPiece = copyGamePieces[end.row, end.column];
                copyGamePieces[end.row, end.column] = mgp.gamePiece;
            }

            this.StartCoroutine(MovePiecesRoutine(movingPieces, activeTokens));
        }

        private IEnumerator MovePiecesRoutine(List<MovingGamePiece> movingPieces, List<IToken> activeTokens)
        {
            for (int i = 0; i < movingPieces.Count; i++)
            {
                MovingGamePiece mgp = movingPieces[i];
                bool firstPiece = (i == 0);
                bool nextPieceExists = (i + 1 < movingPieces.Count);
                mgp.playHitAnimation = ((nextPieceExists && mgp.positions.Count > 2) || firstPiece);

                mgp.gamePiece.Move(mgp, activeTokens);

                if (nextPieceExists)
                {
                    GamePiece nextPiece = movingPieces[i + 1].gamePiece;
                    while (!mgp.gamePiece.IsOverlapped(nextPiece))
                    {
                        yield return null;
                    }
                }
            }
        }

        public GamePiece SpawnPiece(int col, int row, PlayerEnum player)
        {
            GamePiece gamePiecePrefab;
            if (player == PlayerPiece.player)
            {
                gamePiecePrefab = PlayerPiece;
            }
            else
            {
                gamePiecePrefab = OpponentPiece;
            }

            return this.SpawnPiece(row, col, gamePiecePrefab);
        }

        public GamePiece SpawnPiece(int row, int col, GamePiece prefab)
        {
            GamePiece gamePiece = Instantiate(prefab);
            gamePiece.gameObject.SetActive(true);
            gamePiece.CachedGO.SetLayerRecursively(this.gameObject.layer);
            gamePiece.CachedTransform.parent = this.gamePiecesRootTransform;
            gamePiece.CachedTransform.position = this.PositionToVec3(row, col, 10.0f);
            gamePiece.CachedTransform.localScale = Vector3.one;
            gamePiece.gameBoardView = this;
            gamePiece.View.SetSortingLayer(spriteRenderer.sortingLayerID);
            return gamePiece;
        }

        public GameObject SpawnMinigameboardPiece(int row, int col, GameObject prefab)
        {
            GameObject gamePiece = Instantiate(prefab);
            gamePiece.SetLayerRecursively(this.gameObject.layer);
            gamePiece.transform.parent = this.gamePiecesRootTransform;
            gamePiece.transform.position = this.PositionToVec3(row, col, 10.0f);
            gamePiece.transform.localScale = Vector3.one;
            return gamePiece;
        }

        public void CreateToken(int row, int col, Token tokenType)
        {
            GameObject tokenPrefab = GameContentManager.Instance.GetTokenPrefab(tokenType);
            this.CreateToken(row, col, tokenPrefab);
        }

        public void CreateToken(int row, int col, GameObject tokenPrefab)
        {
            if (tokens[row, col] != null) 
            {
                Destroy(tokens[row, col]);
            }

            Vector3 position = this.PositionToVec3(row, col);
            GameObject go = Instantiate(tokenPrefab, position, Quaternion.identity, this.tokensRootTransform);
            go.SetLayerRecursively(this.gameObject.layer);
            go.transform.rotation = tokenPrefab.transform.rotation;
            go.GetComponent<SpriteRenderer>().sortingLayerID = spriteRenderer.sortingLayerID;
            tokens[row, col] = go;
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

        public GamePiece GamePieceAt(int row, int col)
        {
            return gamePieces[row, col];
        }

        public GameObject TokenAt(int row, int col)
        {
            return tokens[row, col];
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
            NumPiecesAnimating = 0;

            foreach (Transform piece in gamePiecesRootTransform)
            {
                Destroy(piece.gameObject);
            }

            foreach (Transform token in tokensRootTransform)
            {
                Destroy(token.gameObject);
            }

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

        public void UpdateGameBoardSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }
    }
}