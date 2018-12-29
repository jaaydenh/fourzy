//modded @vadym udod
//GameBoardView is handling players' input for game field
//it checks if current opened screen (menu) is GameplayScreen, if so, player can interract with field

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using Fourzy._Updates.UI.Widgets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class GameBoardView : MonoBehaviour
    {
        public Transform bitsParent;
        public bool sortByOrder = false;
        public bool spawnHintArea = false;
        public bool interactable = false;
        public MenuController menuController;
        public MenuScreen assignedScreen;

        [Range(3, 8)]
        public int numRows = Constants.numRows;
        [Range(3, 8)]
        public int numColumns = Constants.numRows;

        [HideInInspector]
        public GamePiecePrefabData playerPrefabData;
        [HideInInspector]
        public GamePiecePrefabData opponentPrefabData;

        public BoardBit[,] gamePieces;
        public BoardBit[,] tokens;

        private Vector3 topLeft;
        private bool isInitialized;
        private bool touched = false;
        private HintBlock previousClosest;

        public int piecesAnimating { get; set; }
        public List<HintBlock> hintBlocks { get; private set; }
        public BoxCollider2D boxCollider2D { get; private set; }
        public RectTransform rectTransform { get; private set; }
        public Vector3 step { get; private set; }

        private AlphaTween alphaTween;

        public GamePieceView PlayerPiece
        {
            get
            {
                return playerPrefabData.prefabs[0];
            }
        }

        public GamePieceView OpponentPiece
        {
            get
            {
                return opponentPrefabData.prefabs[(playerPrefabData.data.ID == opponentPrefabData.data.ID) ? 1 : 0];
            }
        }

        protected void Awake()
        {
            Init();
        }

        protected void OnDestroy()
        {
            if (spawnHintArea)
                HintBlock.onHold -= OnHintBlockHold;
        }

        public void OnPointerDown(Vector2 position)
        {
            if (!interactable)
                return;

            //only continue if current opened screen is GameplayScreen
            if (assignedScreen != menuController.currentScreen)
                return;

            touched = true;

            if (spawnHintArea)
            {
                //show hint area
                ShowHintArea();

                //select
                SelectHintBlock(position);
            }
        }

        public void OnPointerMove(Vector2 position)
        {
            if (!interactable)
                return;

            if (!touched)
                return;

            //release controls if current screen isnt GameplayScreen
            if (assignedScreen != menuController.currentScreen)
            {
                OnPointerRelease(position);
                return;
            }

            if (spawnHintArea)
            {
                //select
                SelectHintBlock(position);
            }
        }

        public void OnPointerRelease(Vector2 position)
        {
            if (!interactable)
                return;

            if (!touched)
                return;

            touched = false;

            if (spawnHintArea)
            {
                //hide hint area
                HideHintArea();
            }
        }

        public void Init()
        {
            if (isInitialized)
                return;

            if (!bitsParent)
                bitsParent = transform;

            tokens = new TokenView[numRows, numColumns];
            gamePieces = new GamePieceView[numRows, numColumns];
            hintBlocks = new List<HintBlock>();

            boxCollider2D = GetComponent<BoxCollider2D>();
            rectTransform = GetComponent<RectTransform>();
            alphaTween = GetComponent<AlphaTween>();

            CalculatePositions();

            //assign control screen
            if (menuController)
                assignedScreen = menuController.GetScreen<GameplayScreen>();

            isInitialized = true;

            if (!spawnHintArea)
                return;

            HintBlock.onHold += OnHintBlockHold;
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
                        hintDirection = new Move(new Position(col, -1), Direction.DOWN);
                    //bottom row
                    else if (row == numRows - 1)
                        hintDirection = new Move(new Position(col, numRows), Direction.UP);
                    //left side
                    else if (col == 0)
                        hintDirection = new Move(new Position(-1, row), Direction.RIGHT);
                    //right side
                    else if (col == numColumns - 1)
                        hintDirection = new Move(new Position(numColumns, row), Direction.LEFT);
                    else continue;

                    HintBlock hintBlock = GameContentManager.InstantiatePrefab<HintBlock>(GameContentManager.PrefabType.BOARD_HINT_BOX, transform);
                    hintBlock.transform.localPosition = PositionToVec3(row, col);
                    hintBlock.move = hintDirection;

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
            float posX = topLeft.x + step.x * .5f + step.x * column;
            float posY = topLeft.y - step.y * .5f - step.y * row;
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
            gamePieces = new GamePieceView[Constants.numRows, Constants.numColumns];

            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    int piece = board[row, col];
                    if (piece == (int)Piece.EMPTY)
                        continue;

                    PlayerEnum player = (piece == (int)Piece.BLUE) ? PlayerEnum.ONE : PlayerEnum.TWO;

                    GamePieceView pieceObject = SpawnPiece(col, row, player, false);
                    pieceObject.player = player;
                    gamePieces[row, col] = pieceObject;
                    pieceObject.SetAlpha(initialAlpha);
                }
            }

            if (sortByOrder)
                SortBits();
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

                    TokenView tokenPrefab = GameContentManager.Instance.GetTokenPrefab(token);

                    if (tokenPrefab)
                    {
                        TokenView tokenInstance = SpawnToken(row, col, tokenPrefab, false);

                        tokenInstance.SetAlpha(initialAlpha);
                    }
                }
            }

            if (sortByOrder)
                SortBits();
        }

        public void MoveGamePieceViews(Move move, List<MovingGamePiece> movingPieces, List<IToken> activeTokens)
        {
            GamePieceView gamePiece = SpawnPiece(move.position.column, move.position.row, move.player);
            gamePiece.player = move.player;

            var copyGamePieces = new BoardBit[numRows, numColumns];

            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numColumns; j++)
                    copyGamePieces[i, j] = gamePieces[i, j];

            GamePieceView movingPiece = gamePiece;
            for (int i = 0; i < movingPieces.Count; i++)
            {
                MovingGamePiece mgp = movingPieces[i];
                Position end = mgp.endPosition;
                mgp.gamePiece = movingPiece;
                movingPiece = (GamePieceView)copyGamePieces[end.row, end.column];
                copyGamePieces[end.row, end.column] = mgp.gamePiece;
            }

            StartCoroutine(MovePiecesRoutine(movingPieces, activeTokens));
        }

        public GamePieceView SpawnPiece(int col, int row, PlayerEnum player, bool sort = true)
        {
            GamePieceView gamePiecePrefab;

            if (player == PlayerPiece.player)
                gamePiecePrefab = PlayerPiece;
            else
                gamePiecePrefab = OpponentPiece;

            return SpawnPiece(row, col, gamePiecePrefab, sort);
        }

        public GamePieceView SpawnPiece(int row, int col, GamePieceView prefab, bool sort = true)
        {
            GamePieceView gamePiece = Instantiate(prefab, bitsParent);

            gamePiece.gameObject.SetActive(true);
            gamePiece.gameObject.SetLayerRecursively(gameObject.layer);
            gamePiece.transform.localPosition = PositionToVec3(row, col);

            if (sort)
                SortBits();

            return gamePiece;
        }

        public void SpawnToken(int row, int col, Token tokenType, bool sort = true)
        {
            SpawnToken(row, col, GameContentManager.Instance.GetTokenPrefab(tokenType), sort);
        }

        public TokenView SpawnToken(int row, int col, TokenView tokenPrefab, bool sort = true)
        {
            //this wont destroy gameObject, only component on it
            //if (tokens[row, col] != null)
            //    Destroy(tokens[row, col]);

            TokenView tokenInstance = Instantiate(tokenPrefab, bitsParent);
            tokenInstance.gameObject.SetLayerRecursively(gameObject.layer);
            tokenInstance.transform.localPosition = PositionToVec3(row, col);

            tokens[row, col] = tokenInstance;

            if (sort)
                SortBits();

            return tokenInstance;
        }

        public MiniGameboardPiece SpawnMinigameboardPiece(int row, int col, MiniGameboardPiece prefab)
        {
            MiniGameboardPiece gamePiece = Instantiate(prefab, bitsParent);

            gamePiece.gameObject.SetLayerRecursively(gameObject.layer);
            gamePiece.transform.localPosition = PositionToVec3(row, col);
            gamePiece.transform.localScale = Vector3.one;

            return gamePiece;
        }

        public GamePieceView GamePieceAt(Position position)
        {
            return (GamePieceView)gamePieces[position.row, position.column];
        }

        public GamePieceView GamePieceAt(int row, int col)
        {
            return (GamePieceView)gamePieces[row, col];
        }

        public TokenView TokenAt(int row, int col)
        {
            return (TokenView)tokens[row, col];
        }

        public List<GamePieceView> GetGamePiecesList()
        {
            List<GamePieceView> list = new List<GamePieceView>();

            for (int i = 0; i < Constants.numColumns; i++)
                for (int j = 0; j < Constants.numRows; j++)
                    if (gamePieces[i, j] != null)
                        list.Add((GamePieceView)gamePieces[i, j]);

            return list;
        }

        public List<GamePieceView> GetWaitingGamePiecesList()
        {
            List<GamePieceView> list = new List<GamePieceView>();

            for (int i = 0; i < Constants.numColumns; i++)
                for (int j = 0; j < Constants.numRows; j++)
                    if (gamePieces[i, j] != null && !((GamePieceView)gamePieces[i, j]).isMoving)
                        list.Add((GamePieceView)gamePieces[i, j]);

            return list;
        }

        public List<BoardBit> GetBits()
        {
            List<BoardBit> list = new List<BoardBit>();

            for (int i = 0; i < Constants.numColumns; i++)
                for (int j = 0; j < Constants.numRows; j++)
                {
                    if (gamePieces[i, j] != null)
                        list.Add(gamePieces[i, j]);

                    if (tokens[i, j] != null)
                        list.Add(tokens[i, j]);
                }

            return list;
        }

        public void ResetGamePiecesAndTokens()
        {
            piecesAnimating = 0;

            foreach (Transform piece in bitsParent)
                Destroy(piece.gameObject);

            gamePieces = new GamePieceView[numRows, numColumns];
        }

        public void PrintGameBoard()
        {
            string gameboard = "GameboardView: \n";

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numColumns; col++)
                    if (gamePieces[row, col])
                        gameboard += (int)((GamePieceView)gamePieces[row, col]).player;
                    else
                        gameboard += "0";

                gameboard += "\n";
            }

            Debug.Log(gameboard);
        }

        public void SetAlpha(float alpha)
        {
            alphaTween.SetAlpha(alpha);
        }

        public void Fade(float alpha, float time)
        {
            alphaTween.from = alphaTween._value;
            alphaTween.to = alpha;
            alphaTween.playbackTime = time;

            alphaTween.PlayForward(true);
        }

        private void CalculatePositions()
        {
            if (boxCollider2D)
            {
                topLeft = boxCollider2D.offset + new Vector2(-boxCollider2D.bounds.size.x * .5f / transform.localScale.x, boxCollider2D.bounds.size.y * .5f / transform.localScale.y);
                step = new Vector3(boxCollider2D.size.x / numColumns, boxCollider2D.size.y / numRows);
            }
            else if (rectTransform)
            {
                topLeft = new Vector3(-rectTransform.rect.size.x / 2f, rectTransform.rect.size.y / 2f);
                step = new Vector3(rectTransform.rect.width / numColumns, rectTransform.rect.height / numRows);
            }
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
                    GamePieceView nextPiece = movingPieces[i + 1].gamePiece;

                    while (!mgp.gamePiece.IsOverlapped(nextPiece))
                        yield return null;
                }
            }
        }

        private void ShowHintArea()
        {
            GameState gameState = GamePlayManager.Instance.game.gameState;

            foreach (HintBlock hintBlock in hintBlocks)
                if (gameState.CanMove(hintBlock.move.GetNextPosition(), gameState.TokenBoard.tokens))
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

        private void SortBits()
        {
            List<BoardBit> bits = new List<BoardBit>(bitsParent.GetComponentsInChildren<BoardBit>());
            bits = bits.OrderBy(bit => bit.sortingGroup.sortingOrder).ToList();

            //sort in hirerarchy
            foreach (BoardBit bit in bits)
                bit.transform.SetAsLastSibling();
        }

        private void OnHintBlockHold(HintBlock hintBlock)
        {
            GamePlayManager.Instance.ProcessPlayerInput(hintBlock.transform.position);

            OnPointerRelease(Input.mousePosition);
        }
    }
}