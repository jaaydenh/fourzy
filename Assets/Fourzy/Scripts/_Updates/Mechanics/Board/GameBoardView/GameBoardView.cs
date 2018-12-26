//modded @vadym udod
//GameBoardView is handling players' input for game field
//it checks if current opened screen (menu) is GameplayScreen, if so, player can interract with field

using DG.Tweening;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using Fourzy._Updates.UI.Widgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class GameBoardView : MonoBehaviour
    {
        public SpriteRenderer backgroundImage;

        public Transform gamePiecesRootTransform;
        public Transform tokensRootTransform;

        public GamePiece[,] gamePieces;
        public TokenView[,] tokens;
        [Range(3, 8)]
        public int numRows = Constants.numRows;
        [Range(3, 8)]
        public int numColumns = Constants.numRows;

        [HideInInspector]
        public bool interactable = false;
        [HideInInspector]
        public bool spawnHintArea = false;
        [HideInInspector]
        public MenuController menuController;
        [HideInInspector]
        public GamePiecePrefabData playerPrefabData;
        [HideInInspector]
        public GamePiecePrefabData opponentPrefabData;

        private Vector3 topLeft;
        private bool isInitialized;
        private bool touchCanceled = false;
        private bool touched = false;
        private HintBlock previousClosest;

        public int NumPiecesAnimating { get; set; }
        public MenuScreen assignedScreen { get; private set; }
        public List<HintBlock> hintBlocks { get; private set; }
        public SpriteRenderer spriteRenderer { get; private set; }
        public BoxCollider2D boxCollider2D { get; private set; }
        public RectTransform rectTransform { get; private set; }
        public Vector3 step { get; private set; }

        public GamePiece PlayerPiece
        {
            get
            {
                return playerPrefabData.prefabs[0];
            }
        }

        public GamePiece OpponentPiece
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

        protected void Start()
        {
            //check aspect ratio
            //if aspect is more than 9/16 fit width, else fit height
            Camera _camera = Camera.main;
            if (backgroundImage)
            {
                if (_camera.aspect > .57f)
                {
                    backgroundImage.sprite = GameContentManager.Instance.GetCurrentTheme().GameBackgroundWide;
                    backgroundImage.size = backgroundImage.sprite.rect.size / backgroundImage.sprite.pixelsPerUnit;

                    _camera.orthographicSize = backgroundImage.size.y * backgroundImage.transform.localScale.y / 2f;
                }
                else
                {
                    backgroundImage.sprite = GameContentManager.Instance.GetCurrentTheme().GameBackground;
                    backgroundImage.size = backgroundImage.sprite.rect.size / backgroundImage.sprite.pixelsPerUnit;

                    _camera.orthographicSize = backgroundImage.size.x * backgroundImage.transform.localScale.x / 2f / _camera.aspect;
                }
            }
        }

        protected void OnDestroy()
        {
            if (spawnHintArea)
                HintBlock.onHold -= OnHintBlockHold;
        }

        public void OnPointerDown(Vector2 position)
        {
            if (!interactable || !GamePlayManager.AcceptMoveInput)
                return;

            //only continue if current opened screen is GameplayScreen
            if (assignedScreen != menuController.currentScreen)
                return;

            touchCanceled = false;
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
                touchCanceled = true;

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

            tokens = new TokenView[numRows, numColumns];
            gamePieces = new GamePiece[numRows, numColumns];
            hintBlocks = new List<HintBlock>();

            boxCollider2D = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            rectTransform = GetComponent<RectTransform>();

            CalculatePositions();

            //assign control screen
            if (interactable)
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
                    pieceObject.gamePieceView.SetAlpha(initialAlpha);
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

                    TokenView tokenPrefab = GameContentManager.Instance.GetTokenPrefab(token);

                    if (tokenPrefab)
                    {
                        TokenView tokenInstance = CreateToken(row, col, tokenPrefab);

                        tokenInstance.SetAlpha(initialAlpha);
                    }
                }
            }
        }

        public void MoveGamePieceViews(Move move, List<MovingGamePiece> movingPieces, List<IToken> activeTokens)
        {
            GamePiece gamePiece = SpawnPiece(move.position.column, move.position.row, move.player);
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
            GamePiece gamePiece = Instantiate(prefab, gamePiecesRootTransform);

            gamePiece.gameObject.SetActive(true);
            gamePiece.gameObject.SetLayerRecursively(gameObject.layer);
            gamePiece.transform.localPosition = PositionToVec3(row, col);

            return gamePiece;
        }

        public MiniGameboardPiece SpawnMinigameboardPiece(int row, int col, MiniGameboardPiece prefab)
        {
            MiniGameboardPiece gamePiece = Instantiate(prefab, gamePiecesRootTransform);

            gamePiece.gameObject.SetLayerRecursively(gameObject.layer);
            gamePiece.transform.localPosition = PositionToVec3(row, col);
            gamePiece.transform.localScale = Vector3.one;

            return gamePiece;
        }

        public void CreateToken(int row, int col, Token tokenType)
        {
            CreateToken(row, col, GameContentManager.Instance.GetTokenPrefab(tokenType));
        }

        public TokenView CreateToken(int row, int col, TokenView tokenPrefab)
        {
            if (tokens[row, col] != null)
                Destroy(tokens[row, col]);

            TokenView tokenInstance = Instantiate(tokenPrefab, tokensRootTransform);
            tokenInstance.gameObject.SetLayerRecursively(gameObject.layer);
            tokenInstance.transform.localPosition = PositionToVec3(row, col);

            //if (spriteRenderer)
            //    tokenInstance.TrySetsortingLayerID(spriteRenderer.sortingLayerID);

            tokens[row, col] = tokenInstance;

            return tokenInstance;
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

        public TokenView TokenAt(int row, int col)
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
            if (spriteRenderer)
            {
                Color c = spriteRenderer.color;
                c.a = alpha;
                spriteRenderer.color = c;
            }

            if (backgroundImage)
                backgroundImage.SetAlpha(alpha);
        }

        public void Fade(float alpha, float time)
        {
            if (spriteRenderer)
                spriteRenderer.DOFade(alpha, time);

            if (backgroundImage)
                backgroundImage.DOFade(alpha, time);
        }

        public void UpdateGameBoardSprite(Sprite sprite)
        {
            if (spriteRenderer)
                spriteRenderer.sprite = sprite;
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

        private void OnHintBlockHold(HintBlock hintBlock)
        {
            GamePlayManager.Instance.ProcessPlayerInput(hintBlock.transform.position);

            OnPointerRelease(Input.mousePosition);
        }
    }
}