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
using Fourzy._Updates.UI.Helpers;
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

        public static float EXPECTED_SWIPE_SPEED;
        public static float SCREEN_10_PERCENT;
        public static float SCREEN_5_PERCENT;

        public static float MAX_SWIPE_SPEED_MLT = 5f;

        /// <summary>
        /// How far outside the board touch can be picked up for Swipe2 intup method (1 == 1 cell), currently broken and will be fixed when is needed (workds only with value 1)
        /// </summary>
        public static int SOOTB = 1;
        /// <summary>
        /// If piece is to be spawned outside the board, make it spawn a bit closer
        /// </summary>
        public static float OUTSIDE_BOARD_OFFSET = .5f;
        public static bool CAN_CANCEL_TAP = false;

        private static float SCREEN_PERCENT = (Screen.width + Screen.height) * .01f;

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
        public Action<GamePieceView> onPieceSpawned;
        public Action<GamePieceView> onGamepieceSmashed;

        public ClientPlayerTurn turn = null;

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
        private float swipeSpeedScale;
        private bool twoStepSwipePhaseTwo;
        private bool acrossLayout;
        private bool upsideDown;

        private GamePieceView spawnedGamepiece;
        private AlphaTween alphaTween;
        private Vfx negativeVfx;
        private Transform vfxsParent;
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
        public bool Paused { get; private set; }

        public bool FlipCheck
        {
            get
            {
                switch (game._Type)
                {
                    case GameType.ONBOARDING:
                        return false;

                    default:
                        return gameplayManager && acrossLayout;
                }
            }
        }

        public List<BoardBit> tokens => boardBits.Where(bit => (bit.GetType() == typeof(TokenView) || bit.GetType().IsSubclassOf(typeof(TokenView)))).ToList();

        public List<BoardBit> gamePieces => boardBits.Where(bit => (bit.GetType() == typeof(GamePieceView) || bit.GetType().IsSubclassOf(typeof(GamePieceView)))).ToList();

        public List<InputMapValue> inputMapActiveOnly => inputMap.Where(_inputMapValue => _inputMapValue.value).ToList();

        protected override void Awake()
        {
            base.Awake();

            if (!bitsParent)
            {
                bitsParent = transform;
            }

            boxCollider2D = GetComponent<BoxCollider2D>();
            rectTransform = GetComponent<RectTransform>();
            alphaTween = GetComponent<AlphaTween>();

            GameObject _vfxsParent = new GameObject("vfxsParent");
            _vfxsParent.transform.SetParent(transform);
            if (rectTransform)
            {
                RectTransform _rectVfxsParent = _vfxsParent.AddComponent<RectTransform>();
                _rectVfxsParent.sizeDelta = rectTransform.sizeDelta;
                vfxsParent = _rectVfxsParent;
            }
            else
            {
                vfxsParent = _vfxsParent.transform;
            }
            vfxsParent.localPosition = Vector3.zero;
            vfxsParent.localScale = Vector3.one;
            if (bitsParent != transform)
            {
                vfxsParent.SetAsLastSibling();
            }

            moveArrow = GetComponentInChildren<MoveArrow>();
            arrowsController = GetComponentInChildren<MoveArrowsController>();

            EXPECTED_SWIPE_SPEED = SCREEN_PERCENT * 1f;
            SCREEN_10_PERCENT = SCREEN_PERCENT * 10f;
        }

        protected void Start()
        {
            negativeVfx = VfxHolder.instance.GetVfx<Vfx>("VFX_MOVE_NEGATIVE", -1);
        }

        protected void Update()
        {
            if (isAnimating) return;

            switch (GameManager.Instance.placementStyle)
            {
                case GameManager.PlacementStyle.EDGE_TAP:
                    if (selectedBoardLocation != null && (holdTimer -= Time.deltaTime) <= 0f)
                    {
                        OnMove();
                    }

                    break;

                case GameManager.PlacementStyle.TWO_STEP_SWIPE:
                case GameManager.PlacementStyle.TWO_STEP_TAP:
                    if (!spawnedGamepiece && selectedBoardLocation != null && (holdTimer -= Time.deltaTime) <= 0f)
                    {
                        Direction _direction = selectedBoardLocation.Value.GetDirection(game);
                        BoardLocation _outsideBoardMove = selectedBoardLocation.Value.Neighbor(BoardLocation.Reverse(_direction));

                        spawnedGamepiece = SpawnPiece(_outsideBoardMove.Row, _outsideBoardMove.Column, (PlayerEnum)game._State.ActivePlayerId, true, false);
                        spawnedGamepiece.Show(.25f);
                        spawnedGamepiece.ScaleToCurrent(Vector3.zero, .25f);

                        Vector3 offset = new Vector3(0f, -.2f);
                        switch (_direction)
                        {
                            case Direction.UP:
                                offset = new Vector3(0f, .2f);

                                break;

                            case Direction.LEFT:
                                offset = new Vector3(-.2f, 0f);

                                break;

                            case Direction.RIGHT:
                                offset = new Vector3(.2f, 0f);

                                break;
                        }
                        spawnedGamepiece.PlaySubtleMove(offset, 1f);

                        //add spawn vfx
                        Vfx poofVfx = VfxHolder.instance
                            .GetVfx<Vfx>("VFX_GAMEPIECE_SPAWN")
                            .StartVfx(vfxsParent, BoardLocationToVec2(_outsideBoardMove.Row, _outsideBoardMove.Column), 0f);
                        poofVfx.SetSaLastSibling();
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

        public void ShowHelpForTokenAtPosition(Vector2 position)
        {
            if (gameplayManager && gameplayManager.GameState == GameplayScene.GameState.HELP_STATE)
            {
                BoardLocation _location = Vec2ToBoardLocation(Camera.main.ScreenToWorldPoint(position) - transform.localPosition);
                IEnumerable<TokenView> _tokens = BoardBitsAt<TokenView>(_location);
                if (_tokens.Count() == 0)
                {
                    if (gameplayManager.GameState == GameplayScene.GameState.HELP_STATE)
                    {
                        gameplayManager.ToggleHelpState();
                    }

                    return;
                }

                PersistantMenuController.Instance.GetOrAddScreen<TokenPrompt>().Prompt(_tokens.First());

                if (gameplayManager.GameState == GameplayScene.GameState.HELP_STATE)
                {
                    gameplayManager.ToggleHelpState();
                }

                return;
            }
        }

        public void OnPointerDown(Vector2 position)
        {
            if (isAnimating || game.IsOver) return;

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
                    switch (GameManager.Instance.placementStyle)
                    {
                        case GameManager.PlacementStyle.TWO_STEP_SWIPE:
                        case GameManager.PlacementStyle.TWO_STEP_TAP:
                            break;

                        default:
                            if (selectedBoardLocation != null)
                            {
                                if (!CAN_CANCEL_TAP) return;

                                selectedBoardLocation = null;
                                moveArrow._Reset();
                                return;
                            }

                            break;
                    }

                    switch (GameManager.Instance.placementStyle)
                    {
                        case GameManager.PlacementStyle.EDGE_TAP:
                        case GameManager.PlacementStyle.SWIPE:
                        case GameManager.PlacementStyle.TAP_AND_DRAG:
                        case GameManager.PlacementStyle.TWO_STEP_SWIPE:
                        case GameManager.PlacementStyle.TWO_STEP_TAP:
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

                        case GameManager.PlacementStyle.TWO_STEP_SWIPE:
                            if (spawnedGamepiece)
                            {
                                BoardLocation _next = Vec2ToBoardLocation(Camera.main.ScreenToWorldPoint(position) - transform.localPosition);

                                if (Mathf.Abs(_next.Row - selectedBoardLocation.Value.Row) <= 1 && Mathf.Abs(_next.Column - selectedBoardLocation.Value.Column) <= 1)
                                {
                                    touchOriginalLocation = position;
                                    touchPreviousLocation = position;
                                }
                            }

                            break;

                        case GameManager.PlacementStyle.TWO_STEP_TAP:
                            if (spawnedGamepiece)
                            {
                                if (selectedBoardLocation != null)
                                {
                                    Direction _direction = selectedBoardLocation.Value.GetDirection(game);
                                    List<BoardLocation> toCheck = new List<BoardLocation>() {
                                        selectedBoardLocation.Value,
                                        selectedBoardLocation.Value.Neighbor(BoardLocation.Reverse(_direction)),
                                        selectedBoardLocation.Value.Neighbor(BoardLocation.Reverse(_direction), 2),
                                        selectedBoardLocation.Value.Neighbor(BoardLocation.Reverse(_direction), 3)
                                    };

                                    BoardLocation mouseBoardLocation = Vec2ToBoardLocation(Camera.main.ScreenToWorldPoint(position) - transform.localPosition);

                                    //check locations
                                    if (toCheck.Any(_loc => mouseBoardLocation.Equals(_loc)))
                                    {
                                        OnMove();
                                    }
                                    else
                                    {
                                        CheckTwoStepTapMove(Camera.main.ScreenToWorldPoint(position) - transform.localPosition);
                                    }
                                }
                                else
                                {
                                    CheckTwoStepTapMove(Camera.main.ScreenToWorldPoint(position) - transform.localPosition);
                                }
                            }
                            else
                            {
                                CheckTwoStepTapMove(Camera.main.ScreenToWorldPoint(position) - transform.localPosition);
                            }

                            break;

                        case GameManager.PlacementStyle.SWIPE:
                            if (touchOriginalLocation != Vector2.zero) return;

                            touchOriginalLocation = position;
                            touchPreviousLocation = position;

                            break;

                        case GameManager.PlacementStyle.TAP_AND_DRAG:
                            if (touchOriginalLocation != Vector2.zero) return;

                            BoardLocation _temp = Vec2ToBoardLocation(Camera.main.ScreenToWorldPoint(position) - transform.localPosition);

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
                    List<BoardLocation> locationsList = SpellEvaluator.GetValidSpellLocations(game._State.Board, activeSpell);

                    //modify by current spells
                    foreach (TokenSpell _spell in createdSpellTokens)
                    {
                        locationsList.RemoveAll(_location => _location.Equals(_spell.location));
                    }

                    BoardLocation touchLocation = Vec2ToBoardLocation(Camera.main.ScreenToWorldPoint(position) - transform.localPosition);

                    if (!locationsList.Contains(touchLocation))
                    {
                        onCastCanceled?.Invoke();
                        Debug.Log("location not available");
                    }
                    else
                    {
                        CastSpell(touchLocation, activeSpellID);

                        SendMoveEvent(activeSpellID.ToString(), touchLocation.GetDirection(), touchLocation.GetLocation(game), MoveType.SPELL);
                    }

                    actionState = BoardActionState.MOVE;
                    HideHintArea(HintAreaAnimationPattern.DIAGONAL);

                    break;
            }
        }

        public void OnPointerMove(Vector2 position)
        {
            if (!interactable || !touched) return;
            if (gameplayManager && gameplayManager.GameState == GameplayScene.GameState.HELP_STATE) return;

            Vector2 positionToWorldPosition = Camera.main.ScreenToWorldPoint(position) - transform.localPosition;

            switch (GameManager.Instance.placementStyle)
            {
                case GameManager.PlacementStyle.EDGE_TAP:
                    CheckEdgeTapMove(positionToWorldPosition);

                    break;

                case GameManager.PlacementStyle.TWO_STEP_SWIPE:
                    if (spawnedGamepiece)
                    {
                        CalculateTouchOffset(position);

                        if (touchOffset.magnitude > SCREEN_PERCENT * 2f)
                        {
                            twoStepSwipePhaseTwo = true;
                        }

                        if (twoStepSwipePhaseTwo)
                        {
                            swipeDirection = DirectionFromVec2(touchOffset);
                            moveArrow.SetProgress(touchOffset.magnitude / (SCREEN_PERCENT * 7f));

                            if (swipeDirection != selectedBoardLocation.Value.GetDirection(game))
                            {
                                CancelSwipe2Move();
                            }
                            else if (touchOffset.magnitude >= SCREEN_PERCENT * 7f)
                            {
                                //finish swipe
                                OnPointerRelease(position);
                            }
                        }
                    }

                    break;

                case GameManager.PlacementStyle.SWIPE:
                    CalculateTouchOffset(position);

                    if (touchOffset.magnitude >= SCREEN_PERCENT * 3f)
                    {
                        if (selectedBoardLocation == null)
                        {
                            BoardLocation _temp = Vec2ToBoardLocation(positionToWorldPosition);

                            swipeDirection = DirectionFromVec2(touchOffset);

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
                            if (relativeSwipeDistace < .9f)
                            {
                                int offsetIndex = Mathf.RoundToInt(offsetOnOppositeAxis / gameplayManager.menuController.WorldToCanvasSize(step).x);
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
                                moveArrow.SetProgress(relativeSwipeDistace / .9f);
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

                case GameManager.PlacementStyle.TAP_AND_DRAG:
                    CalculateTouchOffset(position);

                    if (touchOffset.magnitude < SCREEN_PERCENT * 3f)
                    {
                        arrowsController.SetInitialProgress(touchOffset / (SCREEN_PERCENT * 3f));
                    }
                    else
                    {
                        if (arrowsController.pickedDirection == Direction.NONE)
                        {
                            arrowsController.SetInitialProgress(touchOffset / SCREEN_10_PERCENT);
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

                        if (offsetOnDirectionAxis < SCREEN_10_PERCENT)
                        {
                            arrowsController.ContinueProgress(offsetOnDirectionAxis / SCREEN_10_PERCENT);
                        }
                        else
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
                        if (!isAnimating)
                        {
                            moveArrow._Reset();
                        }

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

                    if (selectedBoardLocation != null && relativeSwipeDistace > .9f)
                    {
                        OnMove();
                        moveArrow.ParticleExplode();
                    }

                    moveArrow.Hide();
                    touchOriginalLocation = Vector2.zero;
                    selectedBoardLocation = null;
                    HideHintArea(HintAreaAnimationPattern.NONE);

                    break;

                case GameManager.PlacementStyle.TAP_AND_DRAG:
                    if (selectedBoardLocation != null)
                    {
                        CalculateTouchOffset(position);

                        if (touchOffset.magnitude >= SCREEN_10_PERCENT)
                        {
                            OnMove();
                            arrowsController.ExplodeCurrent();
                        }
                    }

                    arrowsController.Clear();
                    touchOriginalLocation = Vector2.zero;
                    selectedBoardLocation = null;

                    break;

                case GameManager.PlacementStyle.TWO_STEP_SWIPE:
                    if (twoStepSwipePhaseTwo)
                    {
                        CalculateTouchOffset(position);

                        if (touchOffset.magnitude < SCREEN_PERCENT * 7f)
                        {
                            CancelSwipe2Move();
                        }

                        if (selectedBoardLocation != null)
                        {
                            OnMove();

                            touchOriginalLocation = Vector2.zero;
                            selectedBoardLocation = null;
                            twoStepSwipePhaseTwo = false;

                            moveArrow.ParticleExplode();
                            moveArrow.Hide();
                        }
                    }
                    else
                    {
                        if (!spawnedGamepiece)
                        {
                            TwoStepCellSelected(position);
                        }
                        else
                        {
                            if (!Vec2ToBoardLocation(Camera.main.ScreenToWorldPoint(position) - transform.localPosition).Equals(selectedBoardLocation.Value))
                            {
                                CancelSwipe2Move();
                                TwoStepCellSelected(position);
                            }
                        }
                    }

                    HideHintArea(HintAreaAnimationPattern.NONE);

                    void TwoStepCellSelected(Vector3 position)
                    {
                        if (CheckEdgeTapMove(Camera.main.ScreenToWorldPoint(position) - transform.localPosition, true, false))
                        {
                            holdTimer = .05f;
                        }

                        twoStepSwipePhaseTwo = false;
                    }

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

        public float BoardLocationsToBoardDistance(params BoardLocation[] locations)
        {
            float distance = 0f;

            if (locations.Length > 1)
            {
                for (int index = 1; index < locations.Length; index++)
                {
                    distance += Vector2.Distance(BoardLocationToVec2(locations[index - 1]), BoardLocationToVec2(locations[index]));
                }
            }
            else
            {
                return step.x;
            }

            return distance;
        }

        public IEnumerable<T> BoardBitsAt<T>(BoardLocation at) where T : BoardBit =>
            boardBits.
            Where(bit => bit.active && bit.location.Equals(at) && (bit.GetType() == typeof(T) || bit.GetType().IsSubclassOf(typeof(T)))).
            Cast<T>();

        public T BoardBitsAt<T>(string id) where T : BoardBit =>
            boardBits.Find(bit => bit.id == id) as T;

        public IEnumerable<T> BoardTokenAt<T>(BoardLocation at, TokenType tokenType) where T : TokenView =>
            boardBits.
            Where(bit => bit.active && bit.location.Equals(at) && (bit.GetType() == typeof(T) || bit.GetType().IsSubclassOf(typeof(T))) && (bit as TokenView).Token.Type == tokenType).
            Cast<T>();

        public IEnumerable<TokenView> BoardTokensAt(BoardLocation at) =>
            boardBits.Where(bit => bit.active && bit.location.Equals(at) && (bit.GetType() == typeof(TokenView) || bit.GetType().IsSubclassOf(typeof(TokenView)))).
            Cast<TokenView>().
            ToList();

        public T BoardSpellAt<T>(BoardLocation at, SpellId spellId) where T : TokenSpell =>
            boardBits.Find(bit => bit.active && bit.location.Equals(at) && (bit.GetType() == typeof(T) || bit.GetType().IsSubclassOf(typeof(T))) && (bit as TokenSpell).spellId == spellId) as T;

        public GamePieceView SpawnPiece(float row, float col, PlayerEnum player, bool addToBits = true, bool sort = true)
        {
            GamePieceView gamePiece = Instantiate(player == PlayerEnum.ONE ? game.playerOneGamepiece : game.playerTwoGamepiece, bitsParent);

            if (gameplayManager)
            {
                OnRatio.DisplayRatioOption ratio = OnRatio.GetRatio(DeviceOrientation.Portrait);

                if (ratio == OnRatio.DisplayRatioOption.IPHONEX || ratio == OnRatio.DisplayRatioOption.IPHONE)
                {
                    if (col < 0f)
                    {
                        col = -OUTSIDE_BOARD_OFFSET;
                    }
                    else if (col >= game.Columns)
                    {
                        col = game.Columns - OUTSIDE_BOARD_OFFSET;
                    }
                }
            }

            Vector2 pieceLocalPosition = BoardLocationToVec2(row, col);

            gamePiece.gameObject.SetActive(true);
            gamePiece.gameObject.SetLayerRecursively(gameObject.layer);
            gamePiece.transform.localPosition = pieceLocalPosition;
            gamePiece.StartBlinking();

            //flip if needed
            if (FlipCheck)
            {
                gamePiece.RotateTo(upsideDown ? 180f : 0f, RepeatType.NONE, 0f);
            }

            if (addToBits)
            {
                boardBits.Add(gamePiece);

                if (sort)
                {
                    SortBits();
                }
            }

            onPieceSpawned?.Invoke(gamePiece);

            return gamePiece;
        }

        public TokenView SpawnToken(IToken token, bool sort = true)
        {
            switch (token.Type)
            {
                case TokenType.NONE:
                    return null;

                default:
                    return SpawnToken<TokenView>(token.Space.Location.Row, token.Space.Location.Column, token.Type, sort)
                        .SetData(token);
            }
        }

        public TokenView SpawnToken(BoardLocation location, IToken token, bool sort = true)
        {
            switch (token.Type)
            {
                case TokenType.NONE:
                    return null;

                default:
                    return SpawnToken<TokenView>(
                            location.Row,
                            location.Column,
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
                if (gameplayManager.GameState == GameplayScene.GameState.HELP_STATE)
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
                            gameplayManager.BoardView != _board)
                        {
                            return;
                        }

                        turn = new ClientPlayerTurn(turnResults.Turn.Moves);
                        turn.PlayerId = turnResults.Turn.PlayerId;
                        turn.PlayerString = turnResults.Turn.PlayerString;
                        turn.Timestamp = turnResults.Turn.Timestamp;

                        turn.AITurn = true;

                        StartRoutine("boardUpdateRoutine", BoardUpdateRoutine(turnResults, false));
                        SetPausedState("boardUpdateRoutine", Paused);

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

            return TakeTurn(turn);
        }

        public Coroutine TakeTurn(ClientPlayerTurn turn)
        {
            this.turn = turn;

            //change help state
            if (gameplayManager)
            {
                if (gameplayManager.GameState == GameplayScene.GameState.HELP_STATE)
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
            //record new state for realtime games
            if (turn.createdOnThisDevice)
            {
                switch (game._Type)
                {
                    case GameType.REALTIME:
                        switch (GameManager.Instance.ExpectedGameType)
                        {
                            case GameTypeLocal.REALTIME_LOBBY_GAME:
                            case GameTypeLocal.REALTIME_QUICKMATCH:
                                gameplayManager.RecordLastBoardStateAsRoomProperty(false);

                                break;
                        }

                        break;
                }
            }

            while (turnResults.Activity.Count > 0 && turnResults.Activity[0].Timing != GameActionTiming.MOVE)
            {
                turnResults.Activity.RemoveAt(0);
            }

            Coroutine boardUpdateRoutine = StartRoutine("boardUpdateRoutine", BoardUpdateRoutine(turnResults, false));
            SetPausedState("boardUpdateRoutine", Paused);

            return boardUpdateRoutine;
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
            CancelSwipe2Move();

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

        public void Fade(float alpha, float time)
        {
            if (alphaTween)
            {
                alphaTween.from = alphaTween._value;
                alphaTween.to = alpha;
                alphaTween.playbackTime = time;

                alphaTween.PlayForward(true);
            }
            else
            {
                Debug.LogError("Alpha tween not set!");
            }
        }

        public void Pause()
        {
            Paused = true;

            SetPausedState(true);
            gamePieces.ForEach(_gp => _gp.SetPausedState(true));
            tokens.ForEach(_token => _token.SetPausedState(true));
        }

        public void Resume()
        {
            Paused = false;

            SetPausedState(false);
            gamePieces.ForEach(_gp => _gp.SetPausedState(false));
            tokens.ForEach(_token => _token.SetPausedState(false));
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

            //sort bits
            SortBits();
        }

        public void OnBoardLocationExit(BoardLocation location, BoardBit bit)
        {
            foreach (TokenView token in BoardTokensAt(location))
            {
                token.OnBitExit(bit);
            }
        }

        public void FadeTokens(float to, float time)
        {
            tokens.ForEach(token =>
            {
                if (!token.active) return;

                token.alphaTween.playbackTime = time;
                token.alphaTween.from = token.alphaTween._value;
                token.alphaTween.to = to * token.originalColor.a;

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
            acrossLayout = GameManager.Instance.buildIntent == BuildIntent.MOBILE_INFINITY && PlayerPositioningPromptScreen.PlayerPositioning == PlayerPositioning.ACROSS;
            upsideDown = game.activePlayer == game.player2;
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

            StopAllRoutines(false);
        }

        public void SortBits()
        {
            boardBits = boardBits.OrderBy(bit => bit.sortingGroup.sortingOrder).ToList();

            //sort in hirerarchy
            foreach (BoardBit bit in boardBits)
            {
                bit.transform.SetAsLastSibling();
            }

            if (bitsParent == transform)
            {
                vfxsParent.SetAsLastSibling();
            }
        }

        public void OnMove()
        {
            GameManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.HeavyImpact);

            Piece piece = game.activePlayerPiece;
            Direction direction = GetDirection(selectedBoardLocation.Value);
            int location = GetLocationFromBoardLocation(selectedBoardLocation.Value);

            SimpleMove _move = new SimpleMove(piece, direction, location);

            //analytics event
            SendMoveEvent(piece.HerdId, direction, location, MoveType.SIMPLE);

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
                            turnToSend.playerTimerLeft = gameplayManager.GameplayScreen.MyTimerLeft;
                            turnToSend.magicLeft = gameplayManager.GameplayScreen.MyMagicLeft;
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
                coroutine = StartRoutine("boardUpdateRoutine", BoardUpdateRoutine(game.StartTurn(), true));
                SetPausedState("boardUpdateRoutine", Paused);
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

        public List<BoardLocation> GetPossibleMoves()
        {
            List<BoardLocation> result = new List<BoardLocation>();
            TurnEvaluator turnEvaluator = game.turnEvaluator;

            //only active inputMap values
            foreach (InputMapValue inputMapValue in inputMapActiveOnly)
            {
                if (turnEvaluator.CanIMakeMove(inputMapValue.Move))
                {
                    result.Add(inputMapValue.location);
                }
            }

            return result;
        }

        public void ShowHintArea(HintAreaStyle style, HintAreaAnimationPattern pattern)
        {
            Dictionary<BoardLocation, HintBlock> affected = new Dictionary<BoardLocation, HintBlock>();

            TurnEvaluator turnEvaluator = game.turnEvaluator;
            Piece piece = game.activePlayerPiece;

            CancelRoutine("showHintBlocks");
            CancelRoutine("hideHintBlocks");
            foreach (HintBlock hintBlock in hintBlocks.Values)
            {
                hintBlock.CancelAnimation();
            }

            switch (actionState)
            {
                case BoardActionState.MOVE:
                    switch (GameManager.Instance.placementStyle)
                    {
                        case GameManager.PlacementStyle.EDGE_TAP:
                            //only active inputMap values
                            foreach (InputMapValue inputMapValue in inputMapActiveOnly)
                            {
                                if (hintBlocks.ContainsKey(inputMapValue.location) && turnEvaluator.CanIMakeMove(inputMapValue.Move))
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
                    List<BoardLocation> locationsList = SpellEvaluator.GetValidSpellLocations(game._State.Board, activeSpell);

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

        private void CancelSwipe2Move()
        {
            if (spawnedGamepiece != null)
            {
                boardBits.Remove(spawnedGamepiece);

                spawnedGamepiece._Destroy(.2f);
                spawnedGamepiece.Hide(.2f);

                spawnedGamepiece = null;
            }

            selectedBoardLocation = null;
            touchOriginalLocation = Vector2.zero;
            twoStepSwipePhaseTwo = false;
            moveArrow._Reset();
        }

        private Direction DirectionFromVec2(Vector2 vec2)
        {
            if (Mathf.Abs(vec2.x) > Mathf.Abs(vec2.y))
            {
                if (vec2.x >= 0f)
                {
                    return Direction.RIGHT;
                }
                else
                {
                    return Direction.LEFT;
                }
            }
            else
            {
                if (vec2.y >= 0f)
                {
                    return Direction.UP;
                }
                else
                {
                    return Direction.DOWN;
                }
            }
        }

        private void SendMoveEvent(string herdId, Direction direction, int location, MoveType moveType)
        {
            bool sendEvent = false;
            switch (game._Type)
            {
                case GameType.REALTIME:
                case GameType.TRY_TOKEN:
                case GameType.ONBOARDING:
                    sendEvent = true;

                    break;
            }

            if (sendEvent)
            {
                AnalyticsManager.Instance.LogEvent(
                    "movePiece",
                    AnalyticsManager.AnalyticsProvider.ALL,
                    new KeyValuePair<string, object>("piece", herdId),
                    new KeyValuePair<string, object>("direction", direction.ToString()),
                    new KeyValuePair<string, object>("location", location),
                    new KeyValuePair<string, object>("moveType", MoveType.SIMPLE),
                    new KeyValuePair<string, object>("notation", moveType));
            }
        }

        internal void OnGamepieceSmashed(GamePieceView gamepieceView)
        {
            onGamepieceSmashed?.Invoke(gamepieceView);
        }

        private void CalculatePositions()
        {
            if (boxCollider2D)
            {
                topLeft = boxCollider2D.offset +
                    new Vector2(
                        -boxCollider2D.bounds.size.x * .5f / transform.localScale.x,
                        boxCollider2D.bounds.size.y * .5f / transform.localScale.y);
                //topLeft = boxCollider2D.bounds.min;
                step = new Vector3(boxCollider2D.size.x / game.Columns, boxCollider2D.size.y / game.Rows);
            }
            else if (rectTransform)
            {
                topLeft = new Vector3(-rectTransform.rect.size.x / 2f, rectTransform.rect.size.y / 2f);
                step = new Vector3(rectTransform.rect.width / game.Columns, rectTransform.rect.height / game.Rows);
            }
        }

        private float GetRelativeSwipeDistace(float currentOffset)
        {
            switch (swipeDirection)
            {
                case Direction.UP:
                case Direction.RIGHT:
                    return Mathf.Clamp(currentOffset, 0f, SCREEN_10_PERCENT) / SCREEN_10_PERCENT;

                case Direction.DOWN:
                case Direction.LEFT:
                    return Mathf.Clamp(currentOffset, -SCREEN_10_PERCENT, 0f) / -SCREEN_10_PERCENT;
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

        private bool CheckEdgeTapMove(Vector2 position, bool tap = false, bool animateArrow = true)
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
                BoardLocation location = inputMapValue.location;

                previousLocation = location;

                if (InputMap(location) &&
                    turnEvaluator.CanIMakeMove(new SimpleMove(
                        piece,
                        GetDirection(location),
                        GetLocationFromBoardLocation(location))))
                {
                    if (!location.Equals(selectedBoardLocation))
                    {
                        holdTimer = HOLD_TIME;
                        selectedBoardLocation = location;

                        //position arrow
                        moveArrow.Position(inputMapValue.location);
                        moveArrow.Rotate(GetDirection(inputMapValue.location));

                        moveArrow._Reset();
                        if (animateArrow)
                        {
                            moveArrow.Animate();
                        }
                    }

                    return true;
                }
                else
                {
                    if (!previousLocation.Equals(location) || tap)
                    {
                        negativeVfx.StartVfx(
                            null,
                            (Vector2)transform.position + BoardLocationToVec2(inputMapValue.location),
                            0f);
                    }

                    moveArrow._Reset();
                    selectedBoardLocation = null;

                    return false;
                }
            }
        }

        private void CheckTwoStepTapMove(Vector2 position)
        {
            TurnEvaluator turnEvaluator = game.turnEvaluator;
            Piece piece = game.activePlayerPiece;

            InputMapValue inputMapValue = GetClosestInputMapValue(inputMapActiveOnly, position, 1.5f);

            if (inputMapValue == null)
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
            }
            else
            {
                BoardLocation location = inputMapValue.location;

                if (InputMap(location) &&
                    turnEvaluator.CanIMakeMove(new SimpleMove(
                        piece,
                        GetDirection(location),
                        GetLocationFromBoardLocation(location))))
                {
                    if (!location.Equals(selectedBoardLocation))
                    {
                        if (spawnedGamepiece)
                        {
                            CancelSwipe2Move();
                        }

                        holdTimer = .05f;
                        selectedBoardLocation = location;
                    }
                }
                else
                {
                    negativeVfx.StartVfx(
                        null,
                        (Vector2)transform.position + BoardLocationToVec2(inputMapValue.location),
                        0f);
                }
            }
        }

        private InputMapValue GetClosestInputMapValue(List<InputMapValue> _inputMap, Vector3 position, float maxDistance = 1.5f)
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
                    HintBlock hintBlock = GameContentManager.InstantiatePrefab<HintBlock>(
                        "BOARD_HINT_BOX",
                        bitsParent);
                    hintBlock.transform.localPosition = BoardLocationToVec2(row, col);

                    hintBlocks.Add(new BoardLocation(row, col), hintBlock);
                }
            }
        }

        private void UpdateHintArea()
        {
            if (hintBlocks == null || game.IsOver) return;

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
            if (touchDelta.magnitude >= EXPECTED_SWIPE_SPEED)
            {
                swipeSpeedScale = Mathf.Clamp(touchDelta.magnitude / EXPECTED_SWIPE_SPEED, 0f, MAX_SWIPE_SPEED_MLT);
            }
            else
            {
                swipeSpeedScale = 0f;
            }

            touchOffset = (position - touchOriginalLocation) + (swipeSpeedScale * touchDelta);
        }

        private bool CheckIfWillMoveFurther(List<GameAction> actions, int index)
        {
            do
            {
                if (index >= actions.Count)
                {
                    return false;
                }
                else
                {
                    switch (actions[index].Type)
                    {
                        case GameActionType.TRANSITION:
                            index++;
                            continue;

                        case GameActionType.MOVE_PIECE:
                            return true;

                        default:
                            return false;
                    }
                }
            } while (index < actions.Count);

            return false;
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

            SetHintAreaColliderState(false);
            isAnimating = true;
            boardBits.ForEach(bit =>
            {
                if (bit.active)
                {
                    bit.OnBeforeTurn(startTurn);
                }
            });

            //invoke onMoveStart
            onMoveStarted?.Invoke(turn, startTurn);

            int actionIndex = 0;
            bool firstGameActionMoveFound = false;
            float customDelay = 0f;
            float delay = 0f;
            string lastPieceId = "";
            bool gameOver = false;

            GameActionType delayedActionType = GameActionType.INVALID;
            //spawn gamepiece using first action
            GameActionMove moveAction = null;
            TokenView token;

            while (actionIndex < turnResults.Activity.Count)
            {
                switch (turnResults.Activity[actionIndex].Type)
                {
                    case GameActionType.MOVE_PIECE:
                        bool preSpawned = false;
                        moveAction = turnResults.Activity[actionIndex].AsMoveAction();

                        if (!firstGameActionMoveFound)
                        {
                            moveAction = moveAction.InDirection(BoardLocation.Reverse(moveAction.Piece.Direction), 1);

                            if (!spawnedGamepiece)
                            {
                                //spawn gamepiece using first action
                                spawnedGamepiece = SpawnPiece(moveAction.Start.Row, moveAction.Start.Column, (PlayerEnum)moveAction.Piece.PlayerId);

                                spawnedGamepiece.Show(.25f);
                                spawnedGamepiece.ScaleToCurrent(Vector3.zero, .25f);

                                //add spawn vfx
                                Vfx poofVfx = VfxHolder.instance
                                    .GetVfx<Vfx>("VFX_GAMEPIECE_SPAWN")
                                    .StartVfx(vfxsParent, BoardLocationToVec2(moveAction.Start.Row, moveAction.Start.Column), 0f);
                                poofVfx.SetSaLastSibling();
                            }
                            else
                            {
                                spawnedGamepiece.StopSubtleMove();

                                preSpawned = true;
                            }

                            spawnedGamepiece.SetPiece(moveAction.Piece.Piece);

                            firstGameActionMoveFound = true;
                        }

                        int prevActionIndex = actionIndex;

                        GamePieceView bit = spawnedGamepiece ?? BoardBitsAt<GamePieceView>(moveAction.Piece.UniqueId);

                        BoardLocation from = moveAction.Start;
                        BoardLocation to = moveAction.End;

                        if (string.IsNullOrEmpty(lastPieceId) || lastPieceId != moveAction.Piece.UniqueId)
                        {
                            bit.OnBeforeMoveActions(startTurn, from, to);
                            lastPieceId = moveAction.Piece.UniqueId;
                        }

                        if (spawnedGamepiece != null && !preSpawned)
                        {
                            yield return new WaitForSeconds(Constants.GAMEPIECE_AFTER_SPAWN_DELAY);
                        }

                        //move gamepiece
                        float waitTime = bit.StartMoveRoutine(startTurn, from, to);

                        actionIndex++;

                        //check next action
                        if (actionIndex < turnResults.Activity.Count)
                        {
                            switch (turnResults.Activity[actionIndex].Type)
                            {
                                case GameActionType.PUSH:
                                    waitTime = Mathf.Max(0f, waitTime - (step.x / bit.speed * .9f));

                                    break;
                            }
                        }

                        spawnedGamepiece = null;
                        yield return new WaitForSeconds(waitTime);

                        if (!CheckIfWillMoveFurther(turnResults.Activity, actionIndex))
                        {
                            bit.OnAfterMoveAction(startTurn, from, to);
                        }

                        break;

                    case GameActionType.BOSS_POWER:
                        GameActionBossPower bossPowerAction = turnResults.Activity[actionIndex] as GameActionBossPower;

                        gameplayManager.GameplayScreen.gameInfoWidget.DisplayPower(bossPowerAction.Power.PowerType);
                        game.BossMoves++;

                        actionIndex++;

                        break;

                    case GameActionType.ADD_TOKEN:
                        GameActionTokenDrop tokenDrop = turnResults.Activity[actionIndex] as GameActionTokenDrop;

                        Debug.Log($"Spawned: {tokenDrop.Token.Type}, Reason: {tokenDrop.Reason}");

                        //add new token
                        token = SpawnToken(tokenDrop.Token);
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

                        if (tokens.Count() > 0)
                        {
                            yield return new WaitForSeconds(tokens.First()._Destroy(tokenRemove.Reason));
                        }

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
                                    .GetVfx<Vfx>("VFX_BOMB_EXPLOSION_LINE")
                                    .StartVfx(vfxsParent, BoardLocationToVec2(location), 0f);
                            }
                        }
                        else
                        {
                            bombVfx = VfxHolder.instance
                                .GetVfx<Vfx>("VFX_BOMB_EXPLOSION")
                                .StartVfx(vfxsParent, BoardLocationToVec2(effect.Center), 0f);
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

                            yield return new WaitForSeconds(BoardTokenAt<TokenView>(_tokenTransition.Location, _tokenTransition.Before.Type).First().OnGameAction(_tokenTransition));
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

                                    delay = token.StartMoveRoutine(startTurn, _tokenMovement.Start, _tokenMovement.End);

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

                            GameActionTokenRotation _tokenRotation = turnResults.Activity[actionIndex] as GameActionTokenRotation;
                            token = BoardTokenAt<TokenView>(_tokenRotation.Token.Space.Location, _tokenRotation.Token.Type).First();

                            switch (_tokenRotation.Reason)
                            {
                                default:
                                    delay = token.RotateTo(_tokenRotation.StartOrientation, _tokenRotation.EndOrientation, _tokenRotation.RotationDirection, 0f, startTurn);
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
                        gameOver = true;
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

                        gameplayManager.GameplayScreen.gameInfoWidget.PassTurn(turnPassDuration);

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

            while (Paused) yield return null;

            game.CheckLost();

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
                            _token.spell.Cast(game._State, out List<IToken> tokens);
                            _token.SetData(tokens.FirstOrDefault());

                            _token.SetAlpha(1f);
                        }
                        else
                        {
                            _token._Destroy();
                        }
                    });

                    createdSpellTokens.Clear();
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
            boardBits.ForEach(bit =>
            {
                if (bit.active)
                {
                    bit.OnAfterTurn(startTurn);
                }
            });

            //flip pieces if applies
            if (!gameOver && !game.IsOver)
            {
                if (FlipCheck)
                {
                    upsideDown = game.activePlayer == game.player2;
                    foreach (BoardBit _bit in gamePieces)
                    {
                        _bit.RotateTo(upsideDown ? 180f : 0f, RepeatType.NONE, .3f);
                    }

                    yield return new WaitForSeconds(.35f);
                }
            }

            onMoveEnded?.Invoke(turn, turnResults, startTurn);

            //update hint blocks
            UpdateHintArea();

            SetHintAreaSelectableState(true);

            turn = null;
            

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

                    foreach (IToken token in boardSpace.Tokens.Values)
                    {
                        SpawnToken(token);
                    }

                    foreach (Piece piece in boardSpace.Pieces)
                    {
                        SpawnPiece(row, col, (PlayerEnum)piece.PlayerId).SetPiece(piece);
                    }

                    if (delay && (boardSpace.Tokens.Values.Count + boardSpace.Pieces.Count) > 0)
                        yield return new WaitForEndOfFrame();
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
