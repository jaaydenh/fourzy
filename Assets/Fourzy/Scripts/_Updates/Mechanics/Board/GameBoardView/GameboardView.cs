//@vadym udod

using ExitGames.Client.Photon;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics._Vfx;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Threading;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Fourzy._Updates.Mechanics.Board
{
    public class GameboardView : RoutinesBase
    {
        public static float HOLD_TIME = .35f;
        public static float QUICK_TAP_TIME = .34f;
        public static float DISTANCE_SELECT_DIRECTION_SWIPE_1 = 40f;
        public static float DISTANCE_SELECT_DIRECTION_SWIPE_2 = 80f;
        public static float DISTANCE_TO_FINISH_SWIPE = 250f;
        public static float EXPECTED_SWIPE_SPEED = 13f;
        public static float MAX_SWIPE_SPEED_MLT = 5f;
        public static float DISTANCE_TO_FINISH_SWIPE_ANIMATION = .9f;
        /// <summary>
        /// How far outside the board touch can be picked up for Swipe2 intup method (1 == 1 cell), currently broken and will be fixed when is needed (workds only with value 1)
        /// </summary>
        public static int SOOTB = 1;
        public static bool CAN_CANCEL_TAP = false;

        public Transform bitsParent;
        public bool debugBoard = false;
        public bool sortByOrder = false;
        public bool interactable = false;
        public IClientFourzy game;

        public Action<IClientFourzy> onInitialized;
        public Action<IClientFourzy> onGameFinished;
        public Action<IClientFourzy> onNoPossibleMove;
        public Action<IClientFourzy> onDraw;
        public Action<ClientPlayerTurn, bool> onMoveStarted;
        public Action<ClientPlayerTurn, PlayerTurnResult, bool> onMoveEnded;
        public Action onCastCanceled;
        public Action<SpellId, int> onCast;
        public Action onWrongTurn;

        private Vector3 topLeft;
        private Vector2 touchOriginalLocation;
        private Vector2 touchPreviousLocation;
        private Vector2 touchDelta;
        private Vector2 touchOffset;
        private Direction swipeDirection;
        private int originalSwipeLocationIndex;
        private int currentSwipeLocationIndex;
        private List<BoardLocation> possibleSwipeLocations;
        private List<Direction> possibleSwipeDirections;
        private List<BoardLocation> blockedSwipeLocations;
        private List<TokenView> lastHighlighted = new List<TokenView>();
        private bool touched = false;
        private float holdTimer;
        private float tapStartTime;
        private float distanceToFinishSwipeAnimation;
        private float swipeSpeedScale;

        private AlphaTween alphaTween;
        public ClientPlayerTurn turn = null;
        private Dictionary<string, Coroutine> boardUpdateRoutines;
        private Vfx negativeVfx;
        private Thread aiTurnThread;

        private int lastRow = 0;
        private int lastCol = 0;

        public Dictionary<BoardLocation, HintBlock> hintBlocks { get; private set; }
        public List<InputMapValue> inputMap { get; private set; }
        public BoxCollider2D boxCollider2D { get; private set; }
        public RectTransform rectTransform { get; private set; }
        public Vector3 step { get; private set; }
        public List<BoardBit> boardBits { get; private set; }
        public bool isAnimating { get; private set; }
        public BoardActionState actionState { get; private set; }
        public SpellId activeSpellID { get; private set; }
        public ISpell activeSpell { get; private set; }
        public List<TokenSpell> createdSpellTokens { get; private set; }
        public MoveArrow moveArrow { get; private set; }
        public MoveArrowsController arrowsController { get; private set; }
        public GamePlayManager gameplayManager { get; set; }
        public BoardLocation? selectedBoardLocation { get; private set; }
        public BoardLocation previousLocation { get; private set; }
        public int direction { get; private set; }

        public List<BoardBit> tokens => boardBits.Where(bit => (bit.GetType() == typeof(TokenView) || bit.GetType().IsSubclassOf(typeof(TokenView)))).ToList();

        public List<BoardBit> gamePieces => boardBits.Where(bit => (bit.GetType() == typeof(GamePieceView) || bit.GetType().IsSubclassOf(typeof(GamePieceView)))).ToList();

        public List<InputMapValue> inputMapActiveOnly => inputMap.Where(_inputMapValue => _inputMapValue.value).ToList();

        protected override void Awake()
        {
            base.Awake();

            if (!bitsParent) bitsParent = transform;

            boxCollider2D = GetComponent<BoxCollider2D>();
            rectTransform = GetComponent<RectTransform>();
            alphaTween = GetComponent<AlphaTween>();

            moveArrow = GetComponentInChildren<MoveArrow>();
            arrowsController = GetComponentInChildren<MoveArrowsController>();
            distanceToFinishSwipeAnimation = DISTANCE_TO_FINISH_SWIPE_ANIMATION * DISTANCE_TO_FINISH_SWIPE;
        }

        protected void Start()
        {
            negativeVfx = VfxHolder.instance.GetVfx<Vfx>(VfxType.VFX_MOVE_NEGATIVE, -1);
        }

        protected void Update()
        {
            switch (GameManager.Instance.placementStyle)
            {
                case GameManager.PlacementStyle.EDGE_TAP:
                    if (selectedBoardLocation != null && (holdTimer -= Time.deltaTime) <= 0f)
                    {
                        OnMove();
                    }

                    break;
            }

            //cheats
            if (Application.isEditor || Debug.isDebugBuild)
            {
                if (interactable)
                {
                    if (Input.GetKeyDown(KeyCode.P))
                    {
                        DropPiece(Camera.main.ScreenToWorldPoint(Input.mousePosition), true);
                    }
                    else if (Input.GetKeyDown(KeyCode.O))
                    {
                        DropPiece(Camera.main.ScreenToWorldPoint(Input.mousePosition), false);
                    }
                }
            }
        }

        protected void OnDestroy()
        {
            StopAIThread();
        }

        public void StopAIThread()
        {
            if (aiTurnThread != null && aiTurnThread.IsAlive)
            {
                aiTurnThread.Abort();
            }
        }

        public void OnPointerDown(Vector2 position)
        {
            if (isAnimating || game.isOver) return;

            if (gameplayManager && gameplayManager.gameState == GameplayScene.GameState.HELP_STATE)
            {
                BoardLocation _location =
                    Vec2ToBoardLocation(Camera.main.ScreenToWorldPoint(position) - transform.localPosition);
                IEnumerable<TokenView> _tokens = BoardBitsAt<TokenView>(_location);
                if (_tokens.Count() == 0)
                {
                    if (gameplayManager.gameState == GameplayScene.GameState.HELP_STATE)
                    {
                        gameplayManager.ToggleHelpState();
                    }

                    return;
                }

                PersistantMenuController.Instance.GetOrAddScreen<TokenPrompt>().Prompt(_tokens.First());

                if (gameplayManager.gameState == GameplayScene.GameState.HELP_STATE)
                {
                    gameplayManager.ToggleHelpState();
                }

                return;
            }

            if (game._Type != GameType.PASSANDPLAY)
            {
                if (!game.isMyTurn)
                {
                    onWrongTurn?.Invoke();
                    return;
                }
            }

            if (!interactable) return;

            switch (actionState)
            {
                case BoardActionState.MOVE:
                    //cancel move animation
                    if (selectedBoardLocation != null)
                    {
                        if (!CAN_CANCEL_TAP) return;

                        selectedBoardLocation = null;
                        moveArrow._Reset();
                        return;
                    }

                    switch (GameManager.Instance.placementStyle)
                    {
                        case GameManager.PlacementStyle.EDGE_TAP:
                        case GameManager.PlacementStyle.SWIPE:
                        case GameManager.PlacementStyle.SWIPE_STYLE_2:
                            touched = true;
                            tapStartTime = Time.time;
                            swipeSpeedScale = 1f;

                            break;
                    }

                    switch (GameManager.Instance.placementStyle)
                    {
                        case GameManager.PlacementStyle.DEMO_STYLE:
                            DropPiece(Camera.main.ScreenToWorldPoint(position) - transform.localPosition, true);

                            break;

                        case GameManager.PlacementStyle.EDGE_TAP:
                            CheckEdgeTapMove(Camera.main.ScreenToWorldPoint(position) - transform.localPosition, true);

                            break;

                        case GameManager.PlacementStyle.SWIPE:
                            if (touchOriginalLocation != Vector2.zero) return;

                            touchOriginalLocation = position;
                            touchPreviousLocation = position;

                            break;

                        case GameManager.PlacementStyle.SWIPE_STYLE_2:
                            if (touchOriginalLocation != Vector2.zero) return;

                            BoardLocation _temp = Vec2ToBoardLocation(Camera.main.ScreenToWorldPoint(position) - 
                                transform.localPosition);

                            //check if outside the board
                            if (_temp.OnBoard(game._State.Board) ||
                                _temp.Row == -SOOTB ||
                                _temp.Column == -SOOTB ||
                                _temp.Row == (game.Rows - 1) + SOOTB ||
                                _temp.Column == (game.Columns - 1) + SOOTB)
                            {
                                List<BoardLocation> locationsToSample = new List<BoardLocation>();

                                if (_temp.OnBoard(game._State.Board))
                                {
                                    locationsToSample.Add(new BoardLocation(_temp.Row, 0));
                                    locationsToSample.Add(new BoardLocation(_temp.Row, game.Columns - 1));
                                    locationsToSample.Add(new BoardLocation(0, _temp.Column));
                                    locationsToSample.Add(new BoardLocation(game.Rows - 1, _temp.Column));
                                }
                                else
                                {
                                    if (_temp.Row == -SOOTB)
                                        locationsToSample.Add(new BoardLocation(_temp.Row + SOOTB, _temp.Column));
                                    else if (_temp.Row == (game.Rows - 1) + SOOTB)
                                        locationsToSample.Add(new BoardLocation(game.Rows - SOOTB, _temp.Column));
                                    else if (_temp.Column == -SOOTB)
                                        locationsToSample.Add(new BoardLocation(_temp.Row, _temp.Column + SOOTB));
                                    else
                                        locationsToSample.Add(new BoardLocation(_temp.Row, game.Columns - SOOTB));
                                }

                                touchOriginalLocation = position;
                                touchPreviousLocation = position;

                                //get possible locations
                                possibleSwipeLocations = new List<BoardLocation>();
                                possibleSwipeDirections = new List<Direction>();
                                blockedSwipeLocations = new List<BoardLocation>();

                                TurnEvaluator _turnEvaluator = game.turnEvaluator;
                                foreach (InputMapValue inputMapValue in inputMapActiveOnly)
                                {
                                    if (locationsToSample.Contains(inputMapValue.location))
                                    {
                                        if (_turnEvaluator.CanIMakeMove(inputMapValue.Move))
                                        {
                                            possibleSwipeLocations.Add(inputMapValue.location);
                                            possibleSwipeDirections.Add(inputMapValue.Move.Direction);
                                        }
                                        else
                                        {
                                            blockedSwipeLocations.Add(inputMapValue.location);
                                        }
                                    }
                                }

                                if (possibleSwipeLocations.Count == 0)
                                {
                                    negativeVfx.StartVfx(
                                        null, 
                                        (Vector2)transform.position + BoardLocationToVec2(_temp),
                                        0f);

                                    OnPointerRelease(position);

                                    break;
                                }

                                arrowsController.GetObjects(possibleSwipeLocations, blockedSwipeLocations);
                            }
                            else
                            {
                                negativeVfx.StartVfx(null, Camera.main.ScreenToWorldPoint(position), 0f);
                                OnPointerRelease(position);
                            }

                            break;
                    }

                    break;

                case BoardActionState.CAST_SPELL:
                    List<BoardLocation> locationsList =
                        SpellEvaluator.GetValidSpellLocations(game._State.Board, activeSpell);

                    //modify by current spells
                    foreach (TokenSpell _spell in createdSpellTokens)
                    {
                        locationsList.RemoveAll(_location => _location.Equals(_spell.location));
                    }

                    BoardLocation touchLocation =
                        Vec2ToBoardLocation(Camera.main.ScreenToWorldPoint(position) - transform.localPosition);

                    if (!locationsList.Contains(touchLocation))
                    {
                        onCastCanceled?.Invoke();
                        Debug.Log("location not available");
                    }
                    else
                    {
                        CastSpell(touchLocation, activeSpellID);
                    }

                    actionState = BoardActionState.MOVE;
                    HideHintArea(HintAreaAnimationPattern.DIAGONAL);

                    break;
            }
        }

        public void OnPointerMove(Vector2 position)
        {
            if (!interactable || !touched) return;
            if (gameplayManager && gameplayManager.gameState == GameplayScene.GameState.HELP_STATE) return;

            Vector2 positionToWorldPosition = Camera.main.ScreenToWorldPoint(position) - transform.localPosition;

            switch (GameManager.Instance.placementStyle)
            {
                case GameManager.PlacementStyle.EDGE_TAP:
                    CheckEdgeTapMove(positionToWorldPosition);

                    break;

                case GameManager.PlacementStyle.SWIPE:
                    CalculateTouchOffset(position);

                    if (touchOffset.magnitude >= DISTANCE_SELECT_DIRECTION_SWIPE_1)
                    {
                        if (selectedBoardLocation == null)
                        {
                            BoardLocation _temp = Vec2ToBoardLocation(positionToWorldPosition);

                            if (Mathf.Abs(touchOffset.x) > Mathf.Abs(touchOffset.y))
                            {
                                if (touchOffset.x >= 0f) swipeDirection = Direction.RIGHT;
                                else swipeDirection = Direction.LEFT;
                            }
                            else
                            {
                                if (touchOffset.y >= 0f) swipeDirection = Direction.UP;
                                else swipeDirection = Direction.DOWN;
                            }

                            possibleSwipeLocations = new List<BoardLocation>();

                            TurnEvaluator _turnEvaluator = game.turnEvaluator;
                            foreach (InputMapValue inputMapValue in inputMapActiveOnly)
                            {
                                if (_turnEvaluator.CanIMakeMove(inputMapValue.Move) && 
                                    inputMapValue.Move.Direction == swipeDirection)
                                {
                                    possibleSwipeLocations.Add(inputMapValue.location);
                                }
                            }

                            if (_temp.OnBoard(game._State.Board))
                            {
                                originalSwipeLocationIndex = 
                                    GetPossibleLocationIndex(_temp.GetLocation(swipeDirection));
                            }
                            else
                            {
                                originalSwipeLocationIndex = possibleSwipeLocations.Count / 2;
                            }

                            if (originalSwipeLocationIndex == -1)
                            {
                                //no location available
                                OnPointerRelease(position);
                                return;
                            }

                            selectedBoardLocation = new BoardLocation(
                                swipeDirection,
                                possibleSwipeLocations[originalSwipeLocationIndex].GetLocation(swipeDirection),
                                game.Rows,
                                game.Columns);

                            moveArrow.Rotate(swipeDirection);
                            moveArrow._Reset();
                            moveArrow.Position(selectedBoardLocation.Value);

                            ShowHintArea(HintAreaStyle.NONE, HintAreaAnimationPattern.NONE);
                        }
                        else
                        {
                            float offsetOnDirectionAxis = 0f;
                            float offsetOnOppositeAxis = 0f;

                            switch (swipeDirection)
                            {
                                case Direction.LEFT:
                                case Direction.RIGHT:
                                    offsetOnDirectionAxis = touchOffset.x;
                                    offsetOnOppositeAxis = touchOffset.y;

                                    break;

                                case Direction.UP:
                                case Direction.DOWN:
                                    offsetOnDirectionAxis = touchOffset.y;
                                    offsetOnOppositeAxis = -touchOffset.x;

                                    break;
                            }

                            float relativeSwipeDistace = GetRelativeSwipeDistace(offsetOnDirectionAxis);

                            //still can change location
                            if (relativeSwipeDistace < DISTANCE_TO_FINISH_SWIPE_ANIMATION)
                            {
                                int offsetIndex = Mathf.RoundToInt(offsetOnOppositeAxis / 100f);
                                int newLocationIndex = originalSwipeLocationIndex - offsetIndex;

                                if (newLocationIndex > -1 && 
                                    newLocationIndex < possibleSwipeLocations.Count && 
                                    newLocationIndex != currentSwipeLocationIndex)
                                {
                                    currentSwipeLocationIndex = newLocationIndex;
                                    selectedBoardLocation = new BoardLocation(
                                        swipeDirection,
                                        possibleSwipeLocations[currentSwipeLocationIndex].GetLocation(swipeDirection),
                                        game.Rows,
                                        game.Columns);
                                    moveArrow.Position(selectedBoardLocation.Value);
                                }

                                //update swipe animation
                                moveArrow.SetProgress(relativeSwipeDistace / DISTANCE_TO_FINISH_SWIPE_ANIMATION);
                            }
                            //force move
                            else if (relativeSwipeDistace == 1f)
                            {
                                OnPointerRelease(position);
                                return;
                            }
                        }
                    }

                    break;

                case GameManager.PlacementStyle.SWIPE_STYLE_2:
                    CalculateTouchOffset(position);

                    if (touchOffset.magnitude < DISTANCE_SELECT_DIRECTION_SWIPE_2)
                    {
                        arrowsController.SetInitialProgress(touchOffset / DISTANCE_SELECT_DIRECTION_SWIPE_2);
                    }
                    else
                    {
                        if (arrowsController.pickedDirection == Direction.NONE)
                        {
                            arrowsController.SetInitialProgress(touchOffset / DISTANCE_SELECT_DIRECTION_SWIPE_2);
                        }

                        if (possibleSwipeDirections.Contains(arrowsController.pickedDirection) &&
                            (selectedBoardLocation == null || swipeDirection != arrowsController.pickedDirection))
                        {
                            swipeDirection = arrowsController.pickedDirection;

                            if (swipeDirection != Direction.NONE)
                            {
                                selectedBoardLocation = possibleSwipeLocations.Find(
                                    location => location.GetDirection(game) == swipeDirection);
                            }
                        }

                        if (selectedBoardLocation == null) return;

                        float offsetOnDirectionAxis = 0f;

                        switch (swipeDirection)
                        {
                            case Direction.RIGHT:
                                offsetOnDirectionAxis = touchOffset.x;

                                break;

                            case Direction.LEFT:
                                offsetOnDirectionAxis = -touchOffset.x;

                                break;

                            case Direction.UP:
                                offsetOnDirectionAxis = touchOffset.y;

                                break;

                            case Direction.DOWN:
                                offsetOnDirectionAxis = -touchOffset.y;

                                break;
                        }

                        if (offsetOnDirectionAxis < distanceToFinishSwipeAnimation)
                        {
                            float value = offsetOnDirectionAxis / distanceToFinishSwipeAnimation;
                            arrowsController.ContinueProgress((value - (DISTANCE_SELECT_DIRECTION_SWIPE_2 / distanceToFinishSwipeAnimation)) / (1f - value));
                        }
                        else if (offsetOnDirectionAxis >= DISTANCE_TO_FINISH_SWIPE)
                        {
                            //only force move for quick swipes
                            if (swipeSpeedScale >= MAX_SWIPE_SPEED_MLT * .5f)
                            {
                                OnPointerRelease(position);
                                return;
                            }
                        }
                    }

                    break;
            }

            touchPreviousLocation = position;
        }

        public void OnPointerRelease(Vector2 position)
        {
            if (!interactable || !touched) return;

            touched = false;

            switch (GameManager.Instance.placementStyle)
            {
                case GameManager.PlacementStyle.EDGE_TAP:
                    float tapTime = Time.time - tapStartTime;

                    if (tapTime > QUICK_TAP_TIME)
                    {
                        if (!isAnimating) moveArrow._Reset();
                        selectedBoardLocation = null;
                    }

                    break;

                case GameManager.PlacementStyle.SWIPE:
                    CalculateTouchOffset(position);
                    float offsetOnDirectionAxis = 0f;

                    switch (swipeDirection)
                    {
                        case Direction.LEFT:
                        case Direction.RIGHT:
                            offsetOnDirectionAxis = touchOffset.x;

                            break;

                        case Direction.UP:
                        case Direction.DOWN:
                            offsetOnDirectionAxis = touchOffset.y;

                            break;
                    }

                    float relativeSwipeDistace = GetRelativeSwipeDistace(offsetOnDirectionAxis);

                    if (selectedBoardLocation != null && relativeSwipeDistace > DISTANCE_TO_FINISH_SWIPE_ANIMATION)
                    {
                        OnMove();
                        moveArrow.ParticleExplode();
                    }

                    moveArrow.Hide();
                    touchOriginalLocation = Vector2.zero;
                    selectedBoardLocation = null;
                    HideHintArea(HintAreaAnimationPattern.NONE);

                    break;

                case GameManager.PlacementStyle.SWIPE_STYLE_2:
                    if (selectedBoardLocation != null)
                    {
                        CalculateTouchOffset(position);

                        if (touchOffset.magnitude >= distanceToFinishSwipeAnimation)
                        {
                            OnMove();
                            arrowsController.ExplodeCurrent();
                        }
                    }

                    arrowsController.Clear();
                    touchOriginalLocation = Vector2.zero;
                    selectedBoardLocation = null;

                    break;
            }
        }

        public Vector2 BoardLocationToVec2(BoardLocation location) => BoardLocationToVec2(location.Row, location.Column);

        public Vector2 BoardLocationToVec2(float row, float column)
        {
            float posX = topLeft.x + step.x * .5f + step.x * column;
            float posY = topLeft.y - step.y * .5f - step.y * row;

            return new Vector3(posX, posY, transform.position.z);
        }

        public BoardLocation Vec2ToBoardLocation(Vector3 vec3) => new BoardLocation(
            Mathf.FloorToInt(-(vec3.y - topLeft.y) / step.y), 
            Mathf.FloorToInt((vec3.x - topLeft.x) / step.x));

        public IEnumerable<T> BoardBitsAt<T>(BoardLocation at) where T : BoardBit =>
            boardBits.
            Where(bit => bit.active && bit.location.Equals(at) && (bit.GetType() == typeof(T) || bit.GetType().IsSubclassOf(typeof(T)))).
            Cast<T>();

        public T BoardBitsAt<T>(BoardLocation at, string id) where T : BoardBit => 
            boardBits.Find(bit => bit.id == id) as T;

        public IEnumerable<T> BoardTokenAt<T>(BoardLocation at, TokenType tokenType) where T : TokenView =>
            boardBits.
            Where(bit => bit.active && bit.location.Equals(at) && (bit.GetType() == typeof(T) || bit.GetType().IsSubclassOf(typeof(T))) && (bit as TokenView).tokenType == tokenType).
            Cast<T>();

        public IEnumerable<TokenView> BoardTokensAt(BoardLocation at) =>
            boardBits.Where(bit => bit.active && bit.location.Equals(at) && (bit.GetType() == typeof(TokenView) || bit.GetType().IsSubclassOf(typeof(TokenView)))).
            Cast<TokenView>().
            ToList();

        public T BoardSpellAt<T>(BoardLocation at, SpellId spellId) where T : TokenSpell =>
            boardBits.Find(bit => bit.active && bit.location.Equals(at) && (bit.GetType() == typeof(T) || bit.GetType().IsSubclassOf(typeof(T))) && (bit as TokenSpell).spellId == spellId) as T;

        public GamePieceView SpawnPiece(int row, int col, PlayerEnum player, bool sort = true)
        {
            GamePieceView gamePiece = Instantiate(
                player == PlayerEnum.ONE ? game.playerOneGamepiece : game.playerTwoGamepiece, 
                bitsParent);

            gamePiece.gameObject.SetActive(true);
            gamePiece.gameObject.SetLayerRecursively(gameObject.layer);
            gamePiece.transform.localPosition = BoardLocationToVec2(row, col);
            gamePiece.StartBlinking();

            boardBits.Add(gamePiece);

            if (sort)
            {
                SortBits();
            }

            return gamePiece;
        }

        public TokenView SpawnToken(IToken token, bool sort = true)
        {
            switch (token.Type)
            {
                case TokenType.NONE:
                    return null;

                default:
                    return SpawnToken<TokenView>(
                            token.Space.Location.Row, 
                            token.Space.Location.Column, 
                            token.Type, sort)
                        .SetData(token);
            }
        }

        public T SpawnToken<T>(int row, int col, TokenType tokenType, bool sort = true) where T : TokenView
            => SpawnToken<T>(row, col, GameContentManager.Instance.GetTokenPrefab(tokenType, game._Area), sort);

        public T SpawnToken<T>(int row, int col, TokenView tokenPrefab, bool sort = true) where T : TokenView
        {
            TokenView tokenInstance = Instantiate(tokenPrefab, bitsParent);
            tokenInstance.gameObject.SetLayerRecursively(gameObject.layer);
            tokenInstance.transform.localPosition = BoardLocationToVec2(row, col);

            boardBits.Add(tokenInstance);

            if (sort)
            {
                SortBits();
            }

            return tokenInstance as T;
        }

        public void RemoveBoardBit(BoardBit bit)
        {
            if (boardBits.Contains(bit))
            {
                boardBits.Remove(bit);
            }
        }

        public List<GamePieceView> GetWinningPieces()
        {
            List<GamePieceView> result = new List<GamePieceView>();

            if (game._State.WinningLocations != null)
            {
                for (int locationIndex = 0; locationIndex < game._State.WinningLocations.Count; locationIndex++)
                {
                    result.AddRange(BoardBitsAt<GamePieceView>(game._State.WinningLocations[locationIndex]));
                }
            }

            return result;
        }

        public void TakeAITurn(float delay)
        {
            StartRoutine("takeAITurnDelay", delay, TakeAITurn, null);
        }

        public void TakeAITurn()
        {
            //change help state
            if (gameplayManager)
            {
                if (gameplayManager.gameState == GameplayScene.GameState.HELP_STATE)
                {
                    gameplayManager.ToggleHelpState();
                }
            }

            if (actionState == BoardActionState.CAST_SPELL)
            {
                onCastCanceled?.Invoke();
            }

            selectedBoardLocation = null;

            if (touched)
            {
                OnPointerRelease(Vector2.zero);
            }

            aiTurnThread = ThreadsQueuer.Instance.StartThreadForFunc(() =>
            {
                try
                {
                    string gameId = game.BoardID;

                    PlayerTurnResult turnResults = null;
                    GameboardView _board = this;

                    turnResults = game.TakeAITurn(true);

                    //clear first before move actions
                    while (
                        turnResults.Activity.Count > 0 && 
                        turnResults.Activity[0].Timing != GameActionTiming.MOVE)
                    {
                        turnResults.Activity.RemoveAt(0);
                    }

                    ThreadsQueuer.Instance.QueueFuncToExecuteFromMainThread(() =>
                    {
                        if (GameManager.Instance.activeGame == null ||
                            GameManager.Instance.activeGame.BoardID != gameId ||
                            gameplayManager.board != _board)
                        {
                            return;
                        }

                        turn = new ClientPlayerTurn(turnResults.Turn.Moves);
                        turn.PlayerId = turnResults.Turn.PlayerId;
                        turn.PlayerString = turnResults.Turn.PlayerString;
                        turn.Timestamp = turnResults.Turn.Timestamp;

                        turn.AITurn = true;

                        boardUpdateRoutines.Add(
                            turnResults.GameState.UniqueId, 
                            StartCoroutine(BoardUpdateRoutine(turnResults, false)));

                        //manually reset noInput timer
                        StandaloneInputModuleExtended.instance.ResetNoInputTimer();
                    });
                }
                catch (ThreadAbortException) { print("Thread cancelled"); }
            });
        }

        public Coroutine TakeTurn(SimpleMove move)
        {
            if (!game.turnEvaluator.CanIMakeMove(move))
            {
                Debug.Log("Cannot Make Move");
                return null;
            }

            if (turn == null)
            {
                turn = new ClientPlayerTurn(new List<IMove>() { move });
            }
            else
            {
                turn.Moves.Add(move);
            }

            if (turn.PlayerId == 0)
            {
                turn.PlayerId = move.Piece.PlayerId;
            }

            if (selectedBoardLocation != null)
            {
                turn.createdOnThisDevice = true;
            }

            Coroutine _coroutine = TakeTurn(turn);

            return _coroutine;
        }

        public Coroutine TakeTurn(ClientPlayerTurn turn)
        {
            this.turn = turn;

            //change help state
            if (gameplayManager)
            {
                if (gameplayManager.gameState == GameplayScene.GameState.HELP_STATE)
                {
                    gameplayManager.ToggleHelpState();
                }
            }

            if (actionState == BoardActionState.CAST_SPELL)
            {
                onCastCanceled?.Invoke();
            }

            if (!game.turnEvaluator.CanIMakeMove(turn.GetMove()))
            {
                Debug.Log("Cannot Make Move");
                return null;
            }
            PlayerTurnResult turnResults = game.TakeTurn(turn, true);

            while (turnResults.Activity.Count > 0 && turnResults.Activity[0].Timing != GameActionTiming.MOVE)
            {
                turnResults.Activity.RemoveAt(0);
            }

            Coroutine coroutine = StartCoroutine(BoardUpdateRoutine(turnResults, false));

            boardUpdateRoutines.Add(turnResults.GameState.UniqueId, coroutine);

            return coroutine;
        }

        /// <summary>
        /// When local player casts a spell
        /// </summary>
        /// <param name="location"></param>
        public void CastSpell(BoardLocation location, SpellId spellID)
        {
            UpdateHintArea();
            SetHintAreaSelectableState(true);

            IMove move = null;

            TokenSpell token = SpawnToken<TokenSpell>(location.Row, location.Column, spellID.ToTokenType());

            switch (spellID)
            {
                case SpellId.HEX:
                    HexSpell hex = new HexSpell(game._State.ActivePlayerId, location);

                    move = hex;
                    activeSpell = hex;

                    break;

                case SpellId.HOLD_FOURZY:
                    HoldFourzySpell hold = new HoldFourzySpell(game._State.ActivePlayerId, location);

                    move = hold;
                    activeSpell = hold;

                    break;

                case SpellId.PRISON:
                    PrisonSpell prison = new PrisonSpell(game._State.ActivePlayerId, location);

                    move = prison;
                    activeSpell = prison;

                    break;

                case SpellId.DARKNESS:
                    DarknessSpell darkness = new DarknessSpell(game._State.ActivePlayerId, location);

                    move = darkness;
                    activeSpell = darkness;

                    break;

                case SpellId.FRUIT:
                    FruitSpell fruit = new FruitSpell(game._State.ActivePlayerId, location);

                    move = fruit;
                    activeSpell = fruit;

                    break;

                case SpellId.SLURP:
                    SlurpSpell slurp = new SlurpSpell(game._State.ActivePlayerId, location);

                    move = slurp;
                    activeSpell = slurp;

                    break;

                case SpellId.SQUIRT_WATER:
                    SquirtWaterSpell squirtWater = new SquirtWaterSpell(game._State.ActivePlayerId, location);

                    move = squirtWater;
                    activeSpell = squirtWater;

                    break;

                case SpellId.THROW_BOMB:
                    BombSpell bomb = new BombSpell(game._State.ActivePlayerId, location);

                    move = bomb;
                    activeSpell = bomb;

                    break;

                case SpellId.PLACE_LURE:
                    LureSpell lure = new LureSpell(game._State.ActivePlayerId, location);

                    move = lure;
                    activeSpell = lure;

                    break;

                case SpellId.ICE_WALL:
                    IceWallSpell iceWall = new IceWallSpell(location);

                    move = iceWall;
                    activeSpell = iceWall;

                    break;

                case SpellId.FIRE_WALL:
                    FireWallSpell fireWall = new FireWallSpell(game._State.ActivePlayerId, location);

                    move = fireWall;
                    activeSpell = fireWall;

                    break;

                case SpellId.DIG:
                    DigSpell dig = new DigSpell(game._State.ActivePlayerId, location);

                    move = dig;
                    activeSpell = dig;

                    break;

                case SpellId.GROWL:
                    GrowlSpell growl = new GrowlSpell(game._State.ActivePlayerId, location);

                    move = growl;
                    activeSpell = growl;

                    break;

                case SpellId.SUMMON_SPECTER:
                    SpecterSpell specter = new SpecterSpell(game._State.ActivePlayerId, location);

                    move = specter;
                    activeSpell = specter;

                    break;

                case SpellId.RAINBOW:
                    RainbowSpell rainbow = new RainbowSpell(game._State.ActivePlayerId, location);

                    move = rainbow;
                    activeSpell = rainbow;

                    break;

                case SpellId.PUNCH:
                    PunchSpell punch = new PunchSpell(game._State.ActivePlayerId, location);

                    move = punch;
                    activeSpell = punch;

                    break;

                case SpellId.FREEZE:
                    FreezeSpell freeze = new FreezeSpell(game._State.ActivePlayerId, location);

                    move = freeze;
                    activeSpell = freeze;

                    break;

                case SpellId.MELT:
                    MeltSpell melt = new MeltSpell(game._State.ActivePlayerId, location);

                    move = melt;
                    activeSpell = melt;

                    break;

                case SpellId.LIFE:
                    LifeSpell life = new LifeSpell(game._State.ActivePlayerId, location);

                    move = life;
                    activeSpell = life;

                    break;

                case SpellId.DEATH:
                    DeathSpell death = new DeathSpell(game._State.ActivePlayerId, location);

                    move = death;
                    activeSpell = death;

                    break;
            }

            token.SetData(activeSpell, move);
            createdSpellTokens.Add(token);

            //make semi-transparent
            token.SetAlpha(.3f);

            game.AddPlayerMagic(
                game._State.ActivePlayerId, 
                -GameContentManager.Instance.tokensDataHolder.GetTokenData(activeSpell.SpellId).price);

            //invoke cast spell
            onCast?.Invoke(activeSpellID, game._State.ActivePlayerId);
        }

        public void PrepareForSpell(SpellId spellId)
        {
            actionState = BoardActionState.CAST_SPELL;
            activeSpellID = spellId;
            activeSpell = spellId.AsSpell(new BoardLocation());

            CancelRoutine("wrongMove");
            //show hint area depending on spell
            ShowHintArea(HintAreaStyle.NONE, HintAreaAnimationPattern.CIRCLE);
        }

        public void CancelSpell()
        {
            actionState = BoardActionState.MOVE;
            HideHintArea(HintAreaAnimationPattern.CIRCLE);

            UpdateHintArea();
            SetHintAreaSelectableState(true);
        }

        /// <summary>
        /// Get chain of move actions that follow same direction
        /// </summary>
        /// <returns></returns>
        public GameAction[] GetMoveActions(List<GameAction> activity, int startIndex)
        {
            List<GameAction> actionsMove = new List<GameAction>();
            Direction lastDirection = Direction.NONE;

            for (int actionIndex = startIndex; actionIndex < activity.Count; actionIndex++)
            {
                if (activity[actionIndex].Type == GameActionType.MOVE_PIECE)
                {
                    GameActionMove moveAction = activity[actionIndex] as GameActionMove;

                    if (lastDirection == Direction.NONE)
                    {
                        actionsMove.Add(activity[actionIndex]);
                    }
                    else
                    {
                        if (lastDirection == moveAction.Direction)
                        {
                            actionsMove.Add(activity[actionIndex]);
                        }
                        else break;
                    }

                    lastDirection = moveAction.Direction;
                }
                else break;
            }

            return actionsMove.ToArray();
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

        public void LimitInput(Rect[] areas)
        {
            SetInputMap(false);

            foreach (Rect area in areas)
            {
                for (int column = (int)area.x; column < (int)(area.x + area.width); column++)
                {
                    for (int row = (int)area.y; row < (int)(area.y + area.height); row++)
                    {
                        InputMapValue inputMapValue = inputMap.Find(
                            _inputMapValue => _inputMapValue.location.Equals(new BoardLocation(row, column)));

                        if (inputMapValue != null)
                        {
                            inputMapValue.value = true;
                        }
                    }
                }
            }
        }

        public void SetInputMap(bool state) => inputMap.ForEach(inputMapValue => inputMapValue.value = state);

        public void OnBoardLocationEnter(BoardLocation location, BoardBit bit)
        {
            List<TokenView> other = BoardTokensAt(location).ToList();

            other.ForEach(_bit => _bit.OnBitEnter(bit));

            bit.AddInteraction(other);

            //sort bits
            SortBits();
        }

        public void FadeTokens(float to, float time)
        {
            tokens.ForEach(token =>
            {
                if (!token.active) return;

                token.alphaTween.playbackTime = time;
                token.alphaTween.from = token.alphaTween._value;
                token.alphaTween.to = to;

                token.alphaTween.PlayForward(true);
            });
        }

        public void FadeGamepieces(float to, float time)
        {
            gamePieces.ForEach(piece =>
            {
                piece.alphaTween.playbackTime = time;
                piece.alphaTween.from = piece.alphaTween._value;
                piece.alphaTween.to = to;

                piece.alphaTween.PlayForward(true);
            });
        }

        public Coroutine PlayMoves(List<PlayerTurn> moves)
        {
            if (moves == null || moves.Count == 0) return null;

            //only one set can play at a time
            CancelRoutine("playMoves");

            return StartRoutine("playMoves", PlayTurnsRoutine(moves), null);
        }

        /// <summary>
        /// Will play initial moves
        /// </summary>
        public Coroutine PlayInitialMoves() => PlayMoves(game.InitialTurns);

        public void Initialize(IClientFourzy game, bool hintAreas = true, bool createBits = true)
        {
            this.game = game;
            turn = null;
            lastCol = 0;
            lastRow = 0;

            actionState = BoardActionState.MOVE;
            createdSpellTokens = new List<TokenSpell>();

            CancelRoutine("lock_board");
            CancelRoutine("takeAITurnDelay");

            CreateInputMap();
            StopBoardUpdates();
            CalculatePositions();

            if (hintAreas)
            {
                CreateHintArea();
            }

            if (createBits)
            {
                CancelRoutine("createBitsRoutine");
                StartRoutine("createBitsRoutine", CreateBitsRoutine());
            }

            UpdateHintArea();

            onInitialized?.Invoke(game);
        }

        public void Clear()
        {
            game = null;

            CancelRoutine("createBitsRoutine");
            StartRoutine("createBitsRoutine", CreateBitsRoutine());
        }

        public void StopBoardUpdates()
        {
            CancelRoutine("playMoves");
            CancelRoutine("createBitsRoutine");

            if (boardUpdateRoutines != null)
            {
                foreach (Coroutine boardUpdateRoutine in boardUpdateRoutines.Values)
                {
                    if (boardUpdateRoutine != null)
                    {
                        StopCoroutine(boardUpdateRoutine);
                    }
                }
            }

            boardUpdateRoutines = new Dictionary<string, Coroutine>();

            StopAllCoroutines();
        }

        public void SortBits()
        {
            boardBits = boardBits.OrderBy(bit => bit.sortingGroup.sortingOrder).ToList();

            //sort in hirerarchy
            foreach (BoardBit bit in boardBits)
            {
                bit.transform.SetAsLastSibling();
            }
        }

        public void OnMove()
        {
            GameManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.HeavyImpact);
            SimpleMove _move = new SimpleMove(
                game.activePlayerPiece,
                GetDirection(selectedBoardLocation.Value),
                GetLocationFromBoardLocation(selectedBoardLocation.Value));

            switch (game._Type)
            {
                case GameType.REALTIME:
                    switch (GameManager.Instance.ExpectedGameType)
                    {
                        case GameTypeLocal.REALTIME_LOBBY_GAME:
                        case GameTypeLocal.REALTIME_QUICKMATCH:
                            //cant send same turn, as it should contain spells
                            List<IMove> _movesToSend = new List<IMove>();
                            _movesToSend.Add(_move);
                            foreach (TokenSpell _spell in createdSpellTokens)
                            {
                                _movesToSend.Add(_spell.spellMove);
                            }

                            ClientPlayerTurn turnToSend = new ClientPlayerTurn(_movesToSend);
                            //add local timer data
                            turnToSend.playerTimerLeft = gameplayManager.gameplayScreen.myTimerLeft;
                            turnToSend.magicLeft = gameplayManager.gameplayScreen.myMagicLeft;
                            turnToSend.PlayerId = _move.Piece.PlayerId;

                            bool result = PhotonNetwork.RaiseEvent(
                                Constants.TAKE_TURN,
                                JsonConvert.SerializeObject(turnToSend),
                                new Photon.Realtime.RaiseEventOptions()
                                {
                                    Flags = new Photon.Realtime.WebFlags(Photon.Realtime.WebFlags.HttpForwardConst)
                                    {
                                        HttpForward = true
                                    }
                                },
                                SendOptions.SendReliable);

                            Debug.Log("Turn sent to other client: " + result);

                            break;
                    }

                    break;
            }

            TakeTurn(_move);

            if (touched)
            {
                OnPointerRelease(Input.mousePosition);
            }

            selectedBoardLocation = null;
        }

        public Coroutine OnPlayManagerReady()
        {
            Coroutine coroutine = null;

            if (game._allTurnRecord.Count == 0)
            {
                coroutine = StartCoroutine(BoardUpdateRoutine(game.StartTurn(), true));

                //play first turns' before actions
                boardUpdateRoutines.Add(game._State.UniqueId, coroutine);
            }

            return coroutine;
        }

        public int GetLocationFromBoardLocation(BoardLocation _boardLocation) => _boardLocation.GetLocation(game);

        public Direction GetDirection(BoardLocation _boardLocation) => _boardLocation.GetDirection(game);

        public bool InputMap(BoardLocation location)
        {
            InputMapValue inputMapValue = inputMap.Find(_value => _value.location.Equals(location));

            return inputMapValue == null ? false : inputMapValue.value;
        }

        public void ShowHintArea(HintAreaStyle style, HintAreaAnimationPattern pattern)
        {
            Dictionary<BoardLocation, HintBlock> affected = new Dictionary<BoardLocation, HintBlock>();

            TurnEvaluator turnEvaluator = game.turnEvaluator;
            Piece piece = game.activePlayerPiece;

            CancelRoutine("showHintBlocks");
            CancelRoutine("hideHintBlocks");
            foreach (HintBlock hintBlock in hintBlocks.Values) hintBlock.CancelAnimation();

            switch (actionState)
            {
                case BoardActionState.MOVE:
                    switch (GameManager.Instance.placementStyle)
                    {
                        case GameManager.PlacementStyle.EDGE_TAP:
                            //only active inputMap values
                            foreach (InputMapValue inputMapValue in inputMapActiveOnly)
                            {
                                if (hintBlocks.ContainsKey(inputMapValue.location) &&
                                    turnEvaluator.CanIMakeMove(inputMapValue.Move))
                                {
                                    affected.Add(inputMapValue.location, hintBlocks[inputMapValue.location]);
                                }
                            }

                            break;

                        case GameManager.PlacementStyle.SWIPE:
                            if (selectedBoardLocation == null) return;
                            Direction targetDir = selectedBoardLocation.Value.GetDirection(game);

                            //only hightight current edge
                            foreach (InputMapValue inputMapValue in inputMapActiveOnly)
                            {
                                if (hintBlocks.ContainsKey(inputMapValue.location) &&
                                    turnEvaluator.CanIMakeMove(inputMapValue.Move) &&
                                    inputMapValue.Move.Direction == targetDir)
                                {
                                    affected.Add(inputMapValue.location, hintBlocks[inputMapValue.location]);
                                }
                            }

                            break;
                    }

                    break;

                case BoardActionState.CAST_SPELL:
                    List<BoardLocation> locationsList =
                        SpellEvaluator.GetValidSpellLocations(game._State.Board, activeSpell);

                    //modify by current spells
                    foreach (TokenSpell _spell in createdSpellTokens)
                    {
                        locationsList.RemoveAll(_location => _location.Equals(_spell.location));
                    }

                    foreach (KeyValuePair<BoardLocation, HintBlock> hintBlock in hintBlocks)
                    {
                        if (locationsList.Contains(hintBlock.Key))
                        {
                            affected.Add(hintBlock.Key, hintBlock.Value);
                        }
                    }

                    SetHintAreaColliderState(true);
                    SetHintAreaSelectableState(false);

                    break;

            }

            StartRoutine("showHintBlocks", AnimateHintBlocks(affected, style, pattern));
        }

        public void HideHintArea(HintAreaAnimationPattern pattern)
        {
            CancelRoutine("showHintBlocks");

            StartRoutine("hideHintBlocks", HideHintBlocksAnimation(pattern));
        }

        public void SetHintAreaColliderState(bool state)
        {
            if (hintBlocks != null && hintBlocks.Count > 0)
            {
                foreach (KeyValuePair<BoardLocation, HintBlock> hintBlock in hintBlocks)
                {
                    hintBlock.Value.SetColliderState(state);

                    if (!state)
                    {
                        hintBlock.Value.Hide();
                    }
                }
            }
        }

        public void SetHintAreaSelectableState(bool state)
        {
            if (hintBlocks != null && hintBlocks.Count > 0)
            {
                foreach (KeyValuePair<BoardLocation, HintBlock> hintBlock in hintBlocks)
                {
                    hintBlock.Value.SetSelectableState(state);
                }
            }
        }

        public void SetHelpState(bool state)
        {
            Dictionary<BoardLocation, HintBlock> affected = new Dictionary<BoardLocation, HintBlock>();

            if (state)
            {
                List<TokenView> tokensToHighlight = new List<TokenView>();

                foreach (TokenView token in tokens)
                {
                    bool contains = false;
                    foreach (TokenView other in tokensToHighlight)
                    {
                        if (other.location.Equals(token.location))
                        {
                            contains = true;
                        }
                    }

                    if (!contains)
                    {
                        tokensToHighlight.Add(token);
                        affected.Add(token.location, hintBlocks[token.location]);
                    }
                }

                lastHighlighted = tokensToHighlight;

                StartRoutine(
                    "showHintBlocks",
                    AnimateHintBlocks(affected, HintAreaStyle.NONE, HintAreaAnimationPattern.CIRCLE));
            }
            else
            {
                foreach (TokenView token in lastHighlighted)
                {
                    affected.Add(token.location, hintBlocks[token.location]);
                }
                lastHighlighted = new List<TokenView>();

                HideHintArea(HintAreaAnimationPattern.NONE);
            }
        }

        public void LockBoard(float time)
        {
            CancelRoutine("lock_board");
            interactable = false;
            StartRoutine("lock_board", time, () => interactable = true, null);
        }

        private void CalculatePositions()
        {
            if (boxCollider2D)
            {
                topLeft = boxCollider2D.offset +
                    new Vector2(
                        -boxCollider2D.bounds.size.x * .5f / transform.localScale.x,
                        boxCollider2D.bounds.size.y * .5f / transform.localScale.y);
                step = new Vector3(boxCollider2D.size.x / game.Columns, boxCollider2D.size.y / game.Rows);
            }
            else if (rectTransform)
            {
                topLeft = new Vector3(-rectTransform.rect.size.x / 2f, rectTransform.rect.size.y / 2f);
                step = new Vector3(rectTransform.rect.width / game.Columns, rectTransform.rect.height / game.Rows);
            }
        }

        private void SetNewSwipeLocation()
        {
            moveArrow.Position(selectedBoardLocation.Value);
            moveArrow.Animate();
        }

        private float GetRelativeSwipeDistace(float currentOffset)
        {
            switch (swipeDirection)
            {
                case Direction.UP:
                case Direction.RIGHT:
                    return Mathf.Clamp(currentOffset, 0f, DISTANCE_TO_FINISH_SWIPE) / DISTANCE_TO_FINISH_SWIPE;

                case Direction.DOWN:
                case Direction.LEFT:
                    return Mathf.Clamp(currentOffset, -DISTANCE_TO_FINISH_SWIPE, 0f) / -DISTANCE_TO_FINISH_SWIPE;
            }

            return 0f;
        }

        private int GetPossibleLocationIndex(int startingLocation)
        {
            int location;
            int currentOffset = 0;

            for (int tries = 0; tries < game.Rows; tries++)
            {
                location = startingLocation + currentOffset;

                foreach (BoardLocation boardLocation in possibleSwipeLocations)
                {
                    if (boardLocation.GetLocation(swipeDirection) == location)
                    {
                        return possibleSwipeLocations.IndexOf(boardLocation);
                    }
                }

                currentOffset = -currentOffset + Mathf.RoundToInt(Mathf.Sign(-currentOffset));

                if (location <= 0 || location >= game.Rows) continue;
            }

            return -1;
        }

        private bool CheckEdgeTapMove(Vector2 position, bool tap = false)
        {
            TurnEvaluator turnEvaluator = game.turnEvaluator;
            Piece piece = game.activePlayerPiece;

            InputMapValue inputMapValue = GetClosestInputMapValue(inputMapActiveOnly, position);

            if (inputMapValue == null)
            {
                if (tap)
                {
                    if (Vec2ToBoardLocation(position).OnBoard(game._State.Board))
                    {
                        negativeVfx.StartVfx(
                            null,
                            (Vector2)transform.position + BoardLocationToVec2(Vec2ToBoardLocation(position)), 0f);
                    }
                    else
                    {
                        negativeVfx.StartVfx(null, position, 0f);
                    }

                    //dont show hint area on onboarding game mode
                    if (game._Type != GameType.ONBOARDING)
                    {
                        SetHintAreaColliderState(false);

                        CancelRoutine("wrongMove");
                        //start wrong move routine
                        StartRoutine("wrongMove", 1.3f, () => UpdateHintArea(), null);

                        //only show in demo mode
                        if (SettingsManager.Get(SettingsManager.KEY_DEMO_MODE))
                        {
                            //light up hint area
                            ShowHintArea(HintAreaStyle.ANIMATION, HintAreaAnimationPattern.NONE);
                        }
                    }
                }

                return false;
            }
            else
            {
                //BoardLocation mirrored = SettingsManager.Instance.Get(SettingsManager.KEY_MOVE_ORIGIN) ? inputMapValue.location.Mirrored(model) : inputMapValue.location;
                BoardLocation mirrored = inputMapValue.location/*.Mirrored(model)*/;

                if (InputMap(mirrored) &&
                    turnEvaluator.CanIMakeMove(new SimpleMove(
                        piece,
                        GetDirection(mirrored),
                        GetLocationFromBoardLocation(mirrored))))
                {
                    if (!mirrored.Equals(selectedBoardLocation))
                    {
                        holdTimer = HOLD_TIME;
                        selectedBoardLocation = mirrored;

                        //position arrow
                        moveArrow.Position(inputMapValue.location);
                        moveArrow.Rotate(GetDirection(inputMapValue.location));

                        moveArrow._Reset();
                        moveArrow.Animate();
                    }

                    previousLocation = mirrored;

                    return true;
                }
                else
                {
                    if (!previousLocation.Equals(mirrored) || tap)
                    {
                        negativeVfx.StartVfx(
                            null,
                            (Vector2)transform.position + BoardLocationToVec2(inputMapValue.location),
                            0f);
                    }

                    moveArrow._Reset();
                    selectedBoardLocation = null;

                    previousLocation = mirrored;

                    return false;
                }
            }
        }

        private InputMapValue GetClosestInputMapValue(
            List<InputMapValue> _inputMap,
            Vector3 position,
            float maxDistance = 1.5f)
        {
            float _maxDistance = maxDistance * step.x;

            InputMapValue closest = null;

            foreach (InputMapValue inputMapValue in _inputMap)
            {
                float distanceToCurrent = Vector2.Distance(position, BoardLocationToVec2(inputMapValue.location));

                if (distanceToCurrent <= _maxDistance)
                {
                    if (closest == null)
                    {
                        closest = inputMapValue;
                    }
                    else
                    {
                        if (distanceToCurrent < Vector2.Distance(position, BoardLocationToVec2(closest.location)))
                            closest = inputMapValue;
                    }
                }
            }

            return closest;
        }

        /// <summary>
        /// First pass for users' input
        /// </summary>
        private void CreateInputMap()
        {
            inputMap = new List<InputMapValue>();

            for (int col = 0; col < game.Columns; col++)
            {
                for (int row = 0; row < game.Rows; row++)
                {
                    //skip diagonals 
                    if (row == col || (game.Columns - 1) - col == row) continue;

                    //skip inside cells
                    if (row > 0 && col > 0 && row < game.Rows - 1 && col < game.Columns - 1) continue;

                    inputMap.Add(new InputMapValue(game, new BoardLocation(row, col), true));
                }
            }
        }

        private void CreateHintArea()
        {
            if (hintBlocks != null)
            {
                foreach (HintBlock _hintBlock in hintBlocks.Values)
                {
                    Destroy(_hintBlock.gameObject);
                }
            }

            hintBlocks = new Dictionary<BoardLocation, HintBlock>();

            for (int col = 0; col < game.Columns; col++)
            {
                for (int row = 0; row < game.Rows; row++)
                {
                    HintBlock hintBlock = GameContentManager.InstantiatePrefab<HintBlock>(GameContentManager.PrefabType.BOARD_HINT_BOX, bitsParent);
                    hintBlock.transform.localPosition = BoardLocationToVec2(row, col);

                    hintBlocks.Add(new BoardLocation(row, col), hintBlock);
                }
            }
        }

        private void UpdateHintArea()
        {
            if (hintBlocks == null || game.isOver) return;

            List<BoardLocation> affected = new List<BoardLocation>();

            TurnEvaluator turnEvaluator = game.turnEvaluator;

            //only active inputMap values
            foreach (InputMapValue inputMapValue in inputMapActiveOnly)
            {
                if (turnEvaluator.CanIMakeMove(inputMapValue.Move) && hintBlocks.ContainsKey(inputMapValue.location))
                {
                    affected.Add(inputMapValue.location);
                }
            }

            foreach (BoardLocation key in hintBlocks.Keys)
            {
                hintBlocks[key].SetColliderState(affected.Contains(key));
            }
        }

        private void DropPiece(Vector2 worldPoint, bool player)
        {
            BoardLocation location = Vec2ToBoardLocation(worldPoint - (Vector2)transform.localPosition);

            if (!location.OnBoard(game._State.Board) || BoardBitsAt<GamePieceView>(location).Count() != 0) return;

            Piece _piece = player ? game.playerPiece : game.opponentPiece;
            game._State.Board.AddPiece(_piece, location);

            SpawnPiece(location.Row, location.Column, (PlayerEnum)_piece.PlayerId).SetPiece(_piece);
        }

        private void CalculateTouchOffset(Vector2 position)
        {
            touchDelta = position - touchPreviousLocation;
            swipeSpeedScale = Mathf.Clamp(
                touchDelta.magnitude / EXPECTED_SWIPE_SPEED, 
                swipeSpeedScale,
                MAX_SWIPE_SPEED_MLT);

            touchOffset = (position - touchOriginalLocation) * swipeSpeedScale;
        }

        private IEnumerator BoardUpdateRoutine(PlayerTurnResult turnResults, bool startTurn)
        {
            HideHintArea(HintAreaAnimationPattern.NONE);

            //reset ActivePlayerID
            if (game.puzzleData && !game.puzzleData.hasAIOpponent)
            {
                game._State.ActivePlayerId = game.me.PlayerId;
            }

            bool localyCreatedTurn = turn != null ? turn.createdOnThisDevice : false;

            //SimpleMove move = turn != null ? turn.GetMove() : null;

            SetHintAreaColliderState(false);
            isAnimating = true;
            boardBits.ForEach(bit => { if (bit.active) bit.OnBeforeTurn(startTurn); });

            //invoke onMoveStart
            onMoveStarted?.Invoke(turn, startTurn);

            int actionIndex = 0;
            bool firstGameActionMoveFound = false;
            float customDelay = 0f;
            float delay = 0f;
            GameActionType delayedActionType = GameActionType.INVALID;

            //spawn gamepiece using first action
            GameActionMove moveAction = null;
            GamePieceView newGamePiece = null;

            TokenView token;

            while (actionIndex < turnResults.Activity.Count)
            {
                switch (turnResults.Activity[actionIndex].Type)
                {
                    case GameActionType.MOVE_PIECE:
                        if (!firstGameActionMoveFound)
                        {
                            //spawn gamepiece using first action
                            moveAction = turnResults.Activity[actionIndex].AsMoveAction();
                            newGamePiece = SpawnPiece(moveAction.Start.Row, moveAction.Start.Column, (PlayerEnum)moveAction.Piece.PlayerId, false);
                            newGamePiece.SetPiece(moveAction.Piece.Piece);
                            newGamePiece.Show(0f);

                            if (!localyCreatedTurn)
                            {
                                newGamePiece.Show(.25f);
                                newGamePiece.ScaleToCurrent(Vector3.zero, .25f);
                            }

                            firstGameActionMoveFound = true;
                        }

                        int prevActionIndex = actionIndex;

                        GameAction[] moveActions = GetMoveActions(turnResults.Activity, actionIndex);
                        actionIndex += moveActions.Length;

                        GamePieceView targetPiece = BoardBitsAt<GamePieceView>(moveActions[0].AsMoveAction().Start, moveActions[0].AsMoveAction().Piece.UniqueId);

                        GamePieceView bit = (newGamePiece != null) ? newGamePiece : targetPiece;

                        float waitTime = !localyCreatedTurn && (newGamePiece != null)
                            ? bit.ExecuteGameAction(startTurn,
                                moveActions.AddElementToStart(moveAction.InDirection(BoardLocation.Reverse(moveAction.Piece.Direction), 2)))
                            : bit.ExecuteGameAction(startTurn, moveActions);

                        //check next action
                        if (actionIndex < turnResults.Activity.Count)
                        {
                            switch (turnResults.Activity[actionIndex].Type)
                            {
                                case GameActionType.PUSH:
                                    waitTime = Mathf.Clamp01(waitTime - bit.WaitTimeForDistance(.9f));

                                    break;
                            }
                        }

                        newGamePiece = null;
                        yield return new WaitForSeconds(waitTime);

                        break;

                    case GameActionType.BOSS_POWER:
                        GameActionBossPower bossPowerAction = turnResults.Activity[actionIndex] as GameActionBossPower;

                        gameplayManager.gameplayScreen.gameInfoWidget.DisplayPower(bossPowerAction.Power.PowerType);
                        game.BossMoves++;

                        actionIndex++;

                        break;

                    case GameActionType.ADD_TOKEN:
                        GameActionTokenDrop tokenDrop = turnResults.Activity[actionIndex] as GameActionTokenDrop;

                        Debug.Log($"Spawned: {tokenDrop.Token.Type}, Reason: {tokenDrop.Reason}");

                        //add new token
                        token = SpawnToken<TokenView>(tokenDrop.Destination.Row, tokenDrop.Destination.Column, tokenDrop.Token.Type);
                        token.SetData(tokenDrop.Token);
                        token.Show(.5f);

                        switch (tokenDrop.Token.Type)
                        {
                            case TokenType.FRUIT:
                                //play tree sound
                                foreach (TokenView _token in
                                    BoardTokenAt<TokenView>(tokenDrop.Source, TokenType.FRUIT_TREE))
                                {
                                    _token.OnActivate();
                                }

                                break;
                        }

                        yield return new WaitForSeconds(.5f);

                        actionIndex++;

                        break;

                    case GameActionType.REMOVE_TOKEN:
                        GameActionTokenRemove tokenRemove = turnResults.Activity[actionIndex] as GameActionTokenRemove;
                        IEnumerable<TokenView> tokens = BoardTokenAt<TokenView>(tokenRemove.Location, tokenRemove.Before.Type);

                        if (tokens.Count() > 0) yield return new WaitForSeconds(tokens.First()._Destroy(tokenRemove.Reason));

                        actionIndex++;

                        break;

                    case GameActionType.DESTROY:
                        //destroy gamepiece
                        GameActionDestroyed _destroy = turnResults.Activity[actionIndex] as GameActionDestroyed;

                        token = BoardBitsAt<TokenView>(_destroy.End).FirstOrDefault();

                        switch (_destroy.Reason)
                        {
                            case DestroyType.FALLING:
                                token.OnActivate();

                                break;

                            case DestroyType.BOMB:

                                break;
                        }

                        yield return new WaitForSeconds(BoardBitsAt<GamePieceView>(_destroy.End).First().
                            _Destroy(_destroy.Reason));

                        //and in cases like its a pit, destroy pit too
                        switch (_destroy.Reason)
                        {
                            case DestroyType.FALLING:
                                token.Hide(.45f);

                                yield return new WaitForSeconds(.45f);

                                token._Destroy();

                                break;
                        }

                        actionIndex++;

                        break;

                    case GameActionType.EFFECT:
                        GameActionExplosion effect = turnResults.Activity[actionIndex] as GameActionExplosion;
                        Vfx bombVfx = null;

                        if (effect.Explosion.Count > 9)
                        {
                            foreach (BoardLocation location in effect.Explosion)
                            {
                                VfxHolder.instance
                                    .GetVfx<Vfx>(VfxType.VFX_BOMB_EXPLOSION_LINE)
                                    .StartVfx(transform, (Vector3)BoardLocationToVec2(location) + Vector3.back, 0f);
                            }
                        }
                        else
                        {
                            bombVfx = VfxHolder.instance
                                .GetVfx<Vfx>(VfxType.VFX_BOMB_EXPLOSION)
                                .StartVfx(transform, (Vector3)BoardLocationToVec2(effect.Center) + Vector3.back, 0f);
                        }

                        //destroy bomb
                        TokenView bomb =
                            BoardTokenAt<TokenView>(effect.Center, TokenType.CIRCLE_BOMB).FirstOrDefault() ??
                            BoardTokenAt<TokenView>(effect.Center, TokenType.CROSS_BOMB).FirstOrDefault();

                        bomb?._Destroy();

                        yield return new WaitForSeconds(bombVfx ? bombVfx.duration : 2f);

                        actionIndex++;

                        break;

                    case GameActionType.TRANSITION:
                        if (turnResults.Activity[actionIndex].GetType() == typeof(GameActionTokenTransition))
                        {
                            GameActionTokenTransition _tokenTransition = turnResults.Activity[actionIndex] as GameActionTokenTransition;

                            yield return new WaitForSeconds(BoardTokenAt<TokenView>(_tokenTransition.Location, _tokenTransition.Before.Type).First().ExecuteGameAction(startTurn, _tokenTransition));
                        }
                        else if (turnResults.Activity[actionIndex].GetType() == typeof(GameActionTokenMovement))
                        {
                            //reset delay value
                            if (actionIndex > 0 && 
                                turnResults.Activity[actionIndex - 1].Type != GameActionType.TRANSITION)
                            {
                                delay = 0f;
                            }

                            GameActionTokenMovement _tokenMovement = turnResults.Activity[actionIndex] as GameActionTokenMovement;

                            token = BoardTokenAt<TokenView>(_tokenMovement.Start, _tokenMovement.Token.Type).First();

                            switch (_tokenMovement.Reason)
                            {
                                //case TransitionType.GHOST_MOVE:
                                default:

                                    delay = token.StartMoveRoutine(startTurn, _tokenMovement);
                                    if (delay > customDelay)
                                    {
                                        customDelay = delay;
                                    }

                                    break;
                            }
                        }
                        else if (turnResults.Activity[actionIndex].GetType() == typeof(GameActionTokenRotation))
                        {
                            //reset delay value
                            if (actionIndex > 0 && 
                                turnResults.Activity[actionIndex - 1].Type != GameActionType.TRANSITION)
                            {
                                delay = 0f;
                            }

                            GameActionTokenRotation _tokenRotation = 
                                turnResults.Activity[actionIndex] as GameActionTokenRotation;

                            token = BoardTokenAt<TokenView>(
                                _tokenRotation.Token.Space.Location,
                                _tokenRotation.Token.Type)
                                .First();

                            switch (_tokenRotation.Reason)
                            {
                                default:
                                    delay = token.RotateTo(
                                        _tokenRotation.StartOrientation, 
                                        _tokenRotation.EndOrientation, 
                                        _tokenRotation.RotationDirection,
                                        0f,
                                        startTurn);

                                    if (delay > customDelay)
                                    {
                                        customDelay = delay;
                                    }

                                    break;
                            }
                        }

                        delayedActionType = turnResults.Activity[actionIndex].Type;
                        actionIndex++;

                        break;

                    case GameActionType.GAME_END:
                        SetHintAreaColliderState(false);

                        if (turnResults.Activity[actionIndex].GetType() == typeof(GameActionGameEnd))
                        {
                            GameActionGameEnd _gameEndAction = turnResults.Activity[actionIndex] as GameActionGameEnd;

                            switch (_gameEndAction.GameEndType)
                            {
                                case GameEndType.WIN:
                                    game.OnVictory();
                                    onGameFinished?.Invoke(game);

                                    break;

                                case GameEndType.DRAW:
                                    game.OnDraw();
                                    onDraw?.Invoke(game);

                                    break;

                                case GameEndType.NOPIECES:
                                    SetHintAreaColliderState(false);
                                    onGameFinished?.Invoke(game);

                                    break;
                            }
                        }
                        else if (turnResults.Activity[actionIndex].GetType() == typeof(GameActionPuzzleStatus))
                        {
                            GameActionPuzzleStatus puzzleStatusAction = turnResults.Activity[actionIndex] as GameActionPuzzleStatus;

                            switch (puzzleStatusAction.Status)
                            {
                                case PuzzleStatus.FAILED:
                                    SetHintAreaColliderState(false);
                                    onGameFinished?.Invoke(game);

                                    break;
                            }
                        }

                        List<GamePieceView> winningGamepieces = GetWinningPieces();
                        for (int index = 0; index < winningGamepieces.Count; index++)
                        {
                            winningGamepieces[index].PlayWinAnimation(index * .15f + .15f);
                        }

                        actionIndex++;

                        break;

                    case GameActionType.PASS:
                        float turnPassDuration = 1.8f;

                        gameplayManager.gameplayScreen.gameInfoWidget.PassTurn(turnPassDuration);

                        //use some delay
                        yield return new WaitForSeconds(turnPassDuration);

                        actionIndex++;

                        break;

                    default:
                        actionIndex++;

                        break;
                }

                if (customDelay > 0f && (actionIndex >= turnResults.Activity.Count || delayedActionType != turnResults.Activity[actionIndex].Type))
                {
                    yield return new WaitForSeconds(customDelay + .3f);

                    //consume delay
                    customDelay = 0f;
                }
            }

            game.CheckLost();

            //check if gauntlet game finished
            if (game._Mode == GameMode.GAUNTLET && turn != null && turn.PlayerId == game.me.PlayerId)
            {
                if (game._State.Herds[turn.PlayerId].Members.Count == 0 && game._State.WinningLocations == null)
                {
                    SetHintAreaColliderState(false);
                    onGameFinished?.Invoke(game);
                }
            }

            //check if new spells are covered by gamepieces
            if (!startTurn)
            {
                if (createdSpellTokens.Count > 0)
                {
                    gamePieces.ForEach(gamePiece =>
                    {
                        TokenSpell spell =
                            createdSpellTokens.Find(spellToken => spellToken.location.Equals(gamePiece.location));

                        //remove it
                        if (spell)
                        {
                            spell._Destroy();
                            createdSpellTokens.Remove(spell);
                        }
                    });

                    //add spells to model
                    createdSpellTokens.ForEach(_token =>
                    {
                        if (_token.spell != null)
                        {
                            _token.spell.Cast(game._State);
                            _token.SetAlpha(1f);
                        }
                        else
                        {
                            _token._Destroy();
                        }
                    });
                }
            }

            //add delay if needed
            if (turn != null)
            {
                switch (game._Type)
                {
                    case GameType.PASSANDPLAY:
                        yield return new WaitForSeconds(InternalSettings.Current.EXTRA_DELAY_BETWEEN_TURNS);

                        break;
                }
            }

            isAnimating = false;
            boardBits.ForEach(bit => { if (bit.active) bit.OnAfterTurn(startTurn); });

            onMoveEnded?.Invoke(turn, turnResults, startTurn);

            //update hint blocks
            UpdateHintArea();

            SetHintAreaSelectableState(true);

            turn = null;
            createdSpellTokens.Clear();

            boardUpdateRoutines.Remove(turnResults.GameState.UniqueId);

            if (!game.turnEvaluator.IsAvailableSimpleMove())
            {
                onNoPossibleMove?.Invoke(game);
            }
        }

        private IEnumerator AnimateHintBlocks(Dictionary<BoardLocation, HintBlock> affected, HintAreaStyle style, HintAreaAnimationPattern pattern)
        {
            switch (pattern)
            {
                case HintAreaAnimationPattern.NONE:
                    switch (style)
                    {
                        case HintAreaStyle.NONE:
                            foreach (HintBlock hintBlock in affected.Values) hintBlock.Show();

                            break;

                        case HintAreaStyle.ANIMATION:
                            foreach (HintBlock hintBlock in affected.Values) hintBlock.Animate();


                            break;

                        case HintAreaStyle.ANIMATION_LOOP:
                            foreach (HintBlock hintBlock in affected.Values) hintBlock.Animate(loop: true);

                            break;
                    }

                    break;

                case HintAreaAnimationPattern.DIAGONAL:
                    for (int diagonalIndex = 0; diagonalIndex < game.Rows * 2 - 1; diagonalIndex++)
                    {
                        for (int col = 0; col <= diagonalIndex; col++)
                        {
                            BoardLocation location = new BoardLocation(diagonalIndex - col, col);

                            if (affected.ContainsKey(location))
                            {
                                switch (style)
                                {
                                    case HintAreaStyle.NONE:
                                        affected[location].Show();

                                        break;

                                    case HintAreaStyle.ANIMATION:
                                        affected[location].Animate();

                                        break;

                                    case HintAreaStyle.ANIMATION_LOOP:
                                        affected[location].Animate(loop: true);

                                        break;
                                }
                            }
                        }

                        yield return new WaitForSeconds(.04f);
                    }

                    break;

                case HintAreaAnimationPattern.CIRCLE:
                    List<BoardLocation> animated = new List<BoardLocation>();

                    for (int distance = 0; distance < game.Rows; distance++)
                    {
                        foreach (KeyValuePair<BoardLocation, HintBlock> hintBlock in affected)
                            if (!animated.Contains(hintBlock.Key) && 
                                BoardLocationToVec2(hintBlock.Key).magnitude < distance * step.x)
                            {
                                switch (style)
                                {
                                    case HintAreaStyle.NONE:
                                        hintBlock.Value.Show();

                                        break;

                                    case HintAreaStyle.ANIMATION:
                                        hintBlock.Value.Animate();

                                        break;

                                    case HintAreaStyle.ANIMATION_LOOP:
                                        hintBlock.Value.Animate(loop: true);

                                        break;
                                }

                                animated.Add(hintBlock.Key);
                            }

                        yield return new WaitForSeconds(.07f);
                    }

                    break;
            }
        }

        private IEnumerator HideHintBlocksAnimation(HintAreaAnimationPattern pattern)
        {
            if (hintBlocks != null && hintBlocks.Count > 0)
            {
                switch (pattern)
                {
                    case HintAreaAnimationPattern.NONE:
                        foreach (HintBlock hintBlock in hintBlocks.Values)
                        {
                            hintBlock.CancelAnimation();
                        }

                        break;

                    case HintAreaAnimationPattern.DIAGONAL:
                        for (int diagonalIndex = 0; diagonalIndex < game.Rows * 2 - 1; diagonalIndex++)
                        {
                            for (int col = 0; col <= diagonalIndex; col++)
                            {
                                BoardLocation location = new BoardLocation(diagonalIndex - col, col);

                                if (hintBlocks.ContainsKey(location))
                                {
                                    hintBlocks[location].CancelAnimation();
                                }
                            }

                            yield return new WaitForSeconds(.04f);
                        }

                        break;

                    case HintAreaAnimationPattern.CIRCLE:
                        List<BoardLocation> animated = new List<BoardLocation>();

                        for (int distance = 0; distance < game.Rows; distance++)
                        {
                            foreach (KeyValuePair<BoardLocation, HintBlock> hintBlock in hintBlocks)
                            {
                                if (!animated.Contains(hintBlock.Key) &&
                                    BoardLocationToVec2(hintBlock.Key).magnitude < distance * step.x)
                                {
                                    hintBlock.Value.CancelAnimation();

                                    animated.Add(hintBlock.Key);
                                }
                            }

                            yield return new WaitForSeconds(.07f);
                        }

                        break;
                }
            }
        }

        public IEnumerator CreateBitsRoutine(bool clear = true, bool delay = false)
        {
            if (clear)
            {
                if (boardBits != null)
                {
                    foreach (BoardBit bit in boardBits)
                    {
                        Destroy(bit.gameObject);
                    }
                }

                boardBits = new List<BoardBit>();
            }

            if (game == null) yield break;

            int _lastRow = lastRow;
            int _lastCol = lastCol;

            for (int row = _lastRow; row < game.Rows; row++)
            {
                lastRow = row;

                for (int col = _lastCol; col < game.Columns; col++)
                {
                    lastCol = col;

                    BoardSpace boardSpace = game._State.Board.ContentsAt(row, col);

                    foreach (IToken token in boardSpace.Tokens.Values) SpawnToken(token);

                    foreach (Piece piece in boardSpace.Pieces) SpawnPiece(row, col, (PlayerEnum)piece.PlayerId).SetPiece(piece);

                    if (delay && (boardSpace.Tokens.Values.Count + boardSpace.Pieces.Count) > 0) yield return new WaitForEndOfFrame();
                }

                _lastCol = 0;
            }
        }

        private IEnumerator PlayTurnsRoutine(List<PlayerTurn> turns)
        {
            foreach (PlayerTurn turn in turns)
            {
                yield return TakeTurn(new ClientPlayerTurn(turn.Moves) { PlayerId = turn.PlayerId });

                yield return new WaitForSeconds(.5f);
            }
        }

        public class InputMapValue
        {
            public BoardLocation location;
            public bool value;

            public SimpleMove Move
            {
                get
                {
                    move.Piece = model.activePlayerPiece;

                    return move;
                }
            }

            //
            private SimpleMove move;
            private IClientFourzy model;

            public InputMapValue(IClientFourzy model, BoardLocation location, bool value)
            {
                this.model = model;
                this.location = location;
                this.value = value;

                move = new SimpleMove(
                    model.activePlayerPiece,
                    location.GetDirection(model),
                    location.GetLocation(model));
            }
        }

        public enum BoardActionState
        {
            MOVE,
            CAST_SPELL,
        }

        public enum HintAreaStyle
        {
            NONE,
            ANIMATION,
            ANIMATION_LOOP,
        }

        public enum HintAreaAnimationPattern
        {
            NONE,
            DIAGONAL,
            CIRCLE,
        }
    }
}
