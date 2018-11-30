//modded @vadym udod
//GameBoardView is handling players' input for game field
//it checks if current opened screen (menu) is GameplayScreen, if so, player can interract with field

using DG.Tweening;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    public class GameBoardView : MonoBehaviour
    {
        public static Action<Vector3> onMouseDown;
        public static Action<Vector3> onMouseUp;
        public static Action<Vector3> onMouseDrag;

        public static bool disableInput = false;

        [Tooltip("Need a direct reference since it look like ther will be more than 1 MenuController instance at a time")]
        public MenuController menuController;
        public SpriteRenderer backgroundImage;

        public Transform gamePiecesRootTransform;
        public Transform tokensRootTransform;

        public GamePiece[,] gamePieces;
        public GameObject[,] tokens;
        [Range(3, 8)]
        public int numRows = Constants.numRows;
        [Range(3, 8)]
        public int numColumns = Constants.numRows;

        private SpriteRenderer spriteRenderer;
        private BoxCollider2D cellsArea;

        private Vector3 topLeft;
        private Vector3 step;
        private bool isInitialized;
        /// <summary>
        /// Touch will get canceled if current screen changes from GameplayScreen to anything else during touch lifetime
        /// </summary>
        private bool touchCanceled = false;
        private bool touched = false;
        private HintBlock previousClosest;

        public int NumPiecesAnimating { get; set; }
        public GamePiece PlayerPiece { get; set; }
        public GamePiece OpponentPiece { get; set; }
        public MenuScreen assignedScreen { get; private set; }
        public List<HintBlock> hintBlocks { get; private set; }

        protected void Awake()
        {
            Input.simulateMouseWithTouches = true;
            Init();
        }

        protected void Start()
        {
            backgroundImage.sprite = GameContentManager.Instance.GetCurrentTheme().GameBackground;
        }

        protected void OnDestroy()
        {
            HintBlock.onHold -= OnHintBlockHold;
        }

        protected void Update()
        {
#if UNITY_EDITOR
            if (transform.hasChanged)
                CalculatePositions();
#endif

            if (Input.GetMouseButtonDown(0))
            {
                //only continue if current opened screen is GameplayScreen
                if (assignedScreen != menuController.currentScreen || disableInput)
                    return;

                touchCanceled = false;
                touched = true;

                if (onMouseDown != null)
                    onMouseDown.Invoke(Input.mousePosition);

                //show hint area
                ShowHintArea();

                //select
                SelectHintBlock(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                if (!touched)
                    return;

                //release controls if current screen isnt GameplayScreen
                if (assignedScreen != menuController.currentScreen || disableInput)
                {
                    touchCanceled = true;

                    OnMouseRelease(Input.mousePosition);
                    return;
                }

                if (onMouseDrag != null)
                    onMouseDrag.Invoke(Input.mousePosition);

                //select
                SelectHintBlock(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnMouseRelease(Input.mousePosition);
            }
        }

        public void OnMouseRelease(Vector3 position)
        {
            if (!touched)
                return;

            touched = false;

            if (onMouseUp != null)
                onMouseUp.Invoke(Input.mousePosition);

            //hide hint area
            HideHintArea();
        }

        public void Init()
        {
            if (isInitialized)
                return;

            disableInput = false;
            tokens = new GameObject[numRows, numColumns];
            gamePieces = new GamePiece[numRows, numColumns];
            hintBlocks = new List<HintBlock>();
            HintBlock.onHold += OnHintBlockHold;

            cellsArea = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            CalculatePositions();

            //assign control screen
            assignedScreen = menuController.GetScreen<GameplayScreen>();

            isInitialized = true;

            Move hintDirection = null;
            //init hint blocks
            for (int col = 0; col < numColumns; col++)
            {
                for (int row = 0; row < numRows; row++)
                {
                    if (row == col || (row == 0 && col == numColumns - 1) || (col == 0 && row == numRows - 1))
                        continue;

                    //top row
                    if (row == 0)
                        hintDirection = new Move(new Position(col, 0), Direction.DOWN);
                    //bottom row
                    else if (row == numRows - 1)
                        hintDirection = new Move(new Position(col, numRows - 1), Direction.UP);
                    //left side
                    else if (col == 0)
                        hintDirection = new Move(new Position(0, row), Direction.RIGHT);
                    //right side
                    else if (col == numColumns - 1)
                        hintDirection = new Move(new Position(numColumns - 1, row), Direction.LEFT);
                    else continue;

                    HintBlock hintBlock = GameContentManager.GetPrefab<HintBlock>(GameContentManager.PrefabType.BOARD_HINT_BOX, transform);
                    hintBlock.transform.position = PositionToVec3(row, col);
                    hintBlock.blockDirection = hintDirection;

                    hintBlocks.Add(hintBlock);
                }
            }
        }

        public Vector3 PositionToVec3(Position position, float z = 0.0f)
        {
            return PositionToVec3(position.row, position.column, z);
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
                        continue;

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
                        continue;

                    Token token = tokens[row, col].tokenType;

                    GameObject tokenPrefab = GameContentManager.Instance.GetTokenPrefab(token);

                    if (tokenPrefab)
                    {
                        CreateToken(row, col, tokenPrefab);
                        TokenAt(row, col).GetComponent<SpriteRenderer>().SetAlpha(initialAlpha);
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

            StartCoroutine(MovePiecesRoutine(movingPieces, activeTokens));
        }

        public GamePiece SpawnPiece(int col, int row, PlayerEnum player)
        {
            GamePiece gamePiecePrefab;

            if (player == PlayerPiece.player)
                gamePiecePrefab = PlayerPiece;
            else
                gamePiecePrefab = OpponentPiece;

            return SpawnPiece(row, col, gamePiecePrefab);
        }

        public GamePiece SpawnPiece(int row, int col, GamePiece prefab)
        {
            GamePiece gamePiece = Instantiate(prefab);

            gamePiece.gameObject.SetActive(true);
            gamePiece.gameObject.SetLayerRecursively(gameObject.layer);
            gamePiece.transform.parent = gamePiecesRootTransform;
            gamePiece.transform.position = PositionToVec3(row, col, 10.0f);
            gamePiece.transform.localScale = Vector3.one;
            gamePiece.gameBoardView = this;
            gamePiece.View.SetSortingLayer(spriteRenderer.sortingLayerID);

            return gamePiece;
        }

        public GameObject SpawnMinigameboardPiece(int row, int col, GameObject prefab)
        {
            GameObject gamePiece = Instantiate(prefab);

            gamePiece.SetLayerRecursively(gameObject.layer);
            gamePiece.transform.parent = gamePiecesRootTransform;
            gamePiece.transform.position = PositionToVec3(row, col, 10.0f);
            gamePiece.transform.localScale = Vector3.one;

            return gamePiece;
        }

        public void CreateToken(int row, int col, Token tokenType)
        {
            CreateToken(row, col, GameContentManager.Instance.GetTokenPrefab(tokenType));
        }

        public void CreateToken(int row, int col, GameObject tokenPrefab)
        {
            if (tokens[row, col] != null)
                Destroy(tokens[row, col]);

            Vector3 position = PositionToVec3(row, col);
            GameObject go = Instantiate(tokenPrefab, position, Quaternion.identity, tokensRootTransform);
            go.SetLayerRecursively(gameObject.layer);
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
                for (int j = 0; j < Constants.numRows; j++)
                    if (gamePieces[i, j] != null)
                        list.Add(gamePieces[i, j]);

            return list;
        }

        public List<GamePiece> GetWaitingGamePiecesList()
        {
            List<GamePiece> list = new List<GamePiece>();

            for (int i = 0; i < Constants.numColumns; i++)
                for (int j = 0; j < Constants.numRows; j++)
                    if (gamePieces[i, j] != null && !gamePieces[i, j].isMoving)
                        list.Add(gamePieces[i, j]);

            return list;
        }

        public void ResetGamePiecesAndTokens()
        {
            NumPiecesAnimating = 0;

            foreach (Transform piece in gamePiecesRootTransform)
                Destroy(piece.gameObject);

            foreach (Transform token in tokensRootTransform)
                Destroy(token.gameObject);

            gamePieces = new GamePiece[numRows, numColumns];
        }

        public void PrintGameBoard()
        {
            string gameboard = "GameboardView: \n";

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numColumns; col++)
                    if (gamePieces[row, col])
                        gameboard += (int)gamePieces[row, col].player;
                    else
                        gameboard += "0";

                gameboard += "\n";
            }

            Debug.Log(gameboard);
        }

        public void SetAlpha(float alpha)
        {
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;

            backgroundImage.SetAlpha(alpha);
        }

        public void Fade(float alpha, float time)
        {
            spriteRenderer.DOFade(alpha, time);
            backgroundImage.DOFade(alpha, time);
        }

        public void UpdateGameBoardSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }

        private void CalculatePositions()
        {
            BoxCollider2D boxCollider = cellsArea;

            float top = boxCollider.offset.y + (boxCollider.size.y / 2f);
            float btm = boxCollider.offset.y - (boxCollider.size.y / 2f);
            float left = boxCollider.offset.x - (boxCollider.size.x / 2f);
            float right = boxCollider.offset.x + (boxCollider.size.x / 2f);

            topLeft = transform.TransformPoint(new Vector3(left, top, 0f));
            Vector3 btmRight = transform.TransformPoint(new Vector3(right, btm, 0f));

            float stepX = (btmRight.x - topLeft.x) / Constants.numColumns;
            float stepY = (topLeft.y - btmRight.y) / Constants.numRows;

            step = new Vector3(stepX, stepY);
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
                        yield return null;
                }
            }
        }

        private void ShowHintArea()
        {
            GameState gameState = GamePlayManager.Instance.game.gameState;

            foreach (HintBlock hintBlock in hintBlocks)
                if (gameState.CanMove(hintBlock.blockDirection.GetNextPosition(), gameState.TokenBoard.tokens))
                    hintBlock.Show();

            previousClosest = null;
        }

        private void HideHintArea()
        {
            foreach (HintBlock hintBlock in hintBlocks)
                hintBlock.Hide();

            previousClosest = null;
        }

        private HintBlock GetClosestHintBlock(Vector3 position)
        {
            HintBlock closest = hintBlocks[0];

            foreach (HintBlock hintBlock in hintBlocks)
                if (Vector3.Distance(position, hintBlock.transform.position) < Vector3.Distance(position, closest.transform.position))
                    closest = hintBlock;

            return closest;
        }

        private void SelectHintBlock(Vector3 mousePosition)
        {
            HintBlock closest = GetClosestHintBlock(Camera.main.ScreenToWorldPoint(mousePosition));

            if (!closest.shown)
                return;

            if (closest != null)
            {
                if (previousClosest != null)
                {
                    if (previousClosest == closest)
                        return;
                    else
                    {
                        previousClosest.Deselect();
                        closest.Select();
                    }
                }
                else
                    closest.Select();

                previousClosest = closest;
            }
        }

        private void OnHintBlockHold(HintBlock hintBlock)
        {
            GamePlayManager.Instance.ProcessPlayerInput(hintBlock.transform.position);

            OnMouseRelease(Input.mousePosition);
        }
    }
}