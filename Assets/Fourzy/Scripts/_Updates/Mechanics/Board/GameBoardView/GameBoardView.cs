//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics._Vfx;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Threading;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
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
        public static bool CAN_CANCEL_TAP = false;

        public Transform bitsParent;
        public bool debugBoard = false;
        public bool sortByOrder = false;
        public bool interactable = false;
        public IClientFourzy model;

        public Action<IClientFourzy> onGameFinished;
        public Action<IClientFourzy> onDraw;
        public Action<ClientPlayerTurn> onMoveStarted;
        public Action<ClientPlayerTurn, PlayerTurnResult> onMoveEnded;
        public Action onCastCanceled;
        public Action<SpellId, int> onCast;
        public Action onWrongTurn;

        private Vector3 topLeft;
        private bool touched = false;
        private float holdTimer;
        private float tapStartTime;

        private AlphaTween alphaTween;
        public ClientPlayerTurn turn = null;
        private Dictionary<string, Coroutine> boardUpdateRoutines;
        private Vfx negativeVfx;
        private Thread aiTurnThread;

        //for bits creation
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
        public SpellId activeSpell { get; private set; }
        public List<TokenSpell> createdSpellTokens { get; private set; }
        public MoveArrow moveArrow { get; private set; }
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
        }

        protected void Start()
        {
            negativeVfx = VfxHolder.instance.GetVfx<Vfx>(VfxType.VFX_MOVE_NEGATIVE, -1);
        }

        protected void Update()
        {
            if (selectedBoardLocation != null && (holdTimer -= Time.deltaTime) <= 0f) OnMove();

#if UNITY_EDITOR
            //cheats
            if (interactable) if (Input.GetKeyDown(KeyCode.P)) DropPiece(Camera.main.ScreenToWorldPoint(Input.mousePosition));
#endif
        }

        protected void OnDestroy()
        {
            if (aiTurnThread != null && aiTurnThread.IsAlive) aiTurnThread.Abort();
        }

        public void OnPointerDown(Vector2 position)
        {
            if (isAnimating) return;

            if (model.isOver) return;

            if (model._Type != GameType.PASSANDPLAY)
            {
                if (!model.isMyTurn)
                {
                    onWrongTurn?.Invoke();
                    return;
                }
            }

            if (!interactable) return;

            switch (GameManager.Instance.placementStyle)
            {
                case GameManager.PlacementStyle.DEFAULT:
                    if (selectedBoardLocation != null)
                    {
                        if (!CAN_CANCEL_TAP) return;

                        selectedBoardLocation = null;
                        moveArrow._Reset();
                        return;
                    }

                    touched = true;
                    tapStartTime = Time.time;

                    switch (actionState)
                    {
                        case BoardActionState.SIMPLE_MOVE:
                            CheckMove(Camera.main.ScreenToWorldPoint(position) - transform.localPosition, true);

                            break;

                        case BoardActionState.CAST_SPELL:
                            List<BoardLocation> locationsList = SpellEvaluator.GetValidSpellLocations(model._State.Board, new HexSpell(0, new BoardLocation()));

                            BoardLocation touchLocation = Vec2ToBoardLocation(Camera.main.ScreenToWorldPoint(position) - transform.localPosition);

                            if (!locationsList.Contains(touchLocation))
                                onCastCanceled?.Invoke();
                            else
                                CastSpell(touchLocation, activeSpell);

                            actionState = BoardActionState.SIMPLE_MOVE;
                            HideHintArea(HintAreaAnimationPattern.DIAGONAL);
                            break;
                    }

                    break;

                case GameManager.PlacementStyle.DEMO_STYLE:
                    DropPiece(Camera.main.ScreenToWorldPoint(position));

                    break;
            }
        }

        public void OnPointerMove(Vector2 position)
        {
            if (!interactable || !touched) return;

            CheckMove(Camera.main.ScreenToWorldPoint(position) - transform.localPosition);
        }

        public void OnPointerRelease(Vector2 position)
        {
            if (!interactable || !touched) return;

            touched = false;

            switch (actionState)
            {
                case BoardActionState.SIMPLE_MOVE:
                    float tapTime = Time.time - tapStartTime;

                    if (tapTime > QUICK_TAP_TIME)
                    {
                        if (!isAnimating) moveArrow._Reset();
                        selectedBoardLocation = null;
                    }
                    break;
            }
        }

        public Vector2 BoardLocationToVec2(BoardLocation location) => BoardLocationToVec2(location.Row, location.Column);

        public Vector2 BoardLocationToVec2(int row, int column)
        {
            float posX = topLeft.x + step.x * .5f + step.x * column;
            float posY = topLeft.y - step.y * .5f - step.y * row;

            return new Vector3(posX, posY, transform.position.z);
        }

        public BoardLocation Vec2ToBoardLocation(Vector3 vec3)
        {
            int x = Mathf.FloorToInt((vec3.x - topLeft.x) / step.x);
            int y = Mathf.FloorToInt(-(vec3.y - topLeft.y) / step.y);

            return new BoardLocation(y, x);
        }

        public T BoardBitAt<T>(BoardLocation at) where T : BoardBit =>
            boardBits.Find(bit => bit.active && bit.location.Equals(at) && (bit.GetType() == typeof(T) || bit.GetType().IsSubclassOf(typeof(T)))) as T;

        public List<BoardBit> BoardBitsAt(BoardLocation at) => boardBits.Where(bit => bit.active && bit.location.Equals(at)).ToList();

        public T BoardTokenAt<T>(BoardLocation at, TokenType tokenType) where T : TokenView =>
            boardBits.Find(bit => bit.active && bit.location.Equals(at) && (bit.GetType() == typeof(T) || bit.GetType().IsSubclassOf(typeof(T))) && (bit as TokenView).tokenType == tokenType) as T;

        public List<TokenView> BoardTokensAt(BoardLocation at) =>
            boardBits.Where(bit => bit.active && bit.location.Equals(at) && (bit.GetType() == typeof(TokenView) || bit.GetType().IsSubclassOf(typeof(TokenView)))).Cast<TokenView>().ToList();

        public T BoardSpellAt<T>(BoardLocation at, SpellId spellId) where T : TokenSpell =>
            boardBits.Find(bit => bit.active && bit.location.Equals(at) && (bit.GetType() == typeof(T) || bit.GetType().IsSubclassOf(typeof(T))) && (bit as TokenSpell).spellId == spellId) as T;

        public GamePieceView SpawnPiece(int row, int col, PlayerEnum player, bool sort = true)
        {
            GamePieceView gamePiece = Instantiate(player == model.playerOneGamepiece.player ? model.playerOneGamepiece : model.playerTwoGamepiece, bitsParent);

            gamePiece.gameObject.SetActive(true);
            gamePiece.gameObject.SetLayerRecursively(gameObject.layer);
            gamePiece.transform.localPosition = BoardLocationToVec2(row, col);
            gamePiece.StartBlinking();

            boardBits.Add(gamePiece);

            if (sort)
                SortBits();

            return gamePiece;
        }

        public TokenView SpawnToken(IToken token, bool sort = true)
        {
            switch (token.Type)
            {
                case TokenType.NONE:
                    return null;

                default:
                    return SpawnToken<TokenView>(token.Space.Location.Row, token.Space.Location.Column, token.Type, sort).SetData(token);
            }
        }

        public T SpawnToken<T>(int row, int col, TokenType tokenType, bool sort = true) where T : TokenView
            => SpawnToken<T>(row, col, GameContentManager.Instance.GetTokenPrefab(tokenType, model._Area), sort);

        public T SpawnToken<T>(int row, int col, TokenView tokenPrefab, bool sort = true) where T : TokenView
        {
            TokenView tokenInstance = Instantiate(tokenPrefab, bitsParent);
            tokenInstance.gameObject.SetLayerRecursively(gameObject.layer);
            tokenInstance.transform.localPosition = BoardLocationToVec2(row, col);

            boardBits.Add(tokenInstance);

            if (sort) SortBits();

            return tokenInstance as T;
        }

        public void RemoveBoardBit(BoardBit bit)
        {
            if (boardBits.Contains(bit))
                boardBits.Remove(bit);
        }

        public List<GamePieceView> GetWinningPieces()
        {
            List<GamePieceView> result = new List<GamePieceView>();

            if (model._State.WinningLocations != null)
                for (int locationIndex = 0; locationIndex < model._State.WinningLocations.Count; locationIndex++)
                    result.Add(BoardBitAt<GamePieceView>(model._State.WinningLocations[locationIndex]));

            return result;
        }

        public void TakeAITurn()
        {
            if (actionState == BoardActionState.CAST_SPELL) CancelSpell();

            if (touched)
            {
                touched = false;
                moveArrow._Reset();
            }
            selectedBoardLocation = null;

            aiTurnThread = ThreadsQueuer.Instance.StartThreadForFunc(() =>
            {
                string gameId = model.GameID;
                
                PlayerTurnResult turnResults = null;

                //if (model._allTurnRecord.Count > 0)
                //{
                    turnResults = model.TakeAITurn(true);

                    //clear first before move actions
                    while (turnResults.Activity.Count > 0 && turnResults.Activity[0].Timing != GameActionTiming.MOVE) turnResults.Activity.RemoveAt(0);
                //}
                //else
                //{
                //    turnResults = model.TakeAITurn(true);
                //}

                ThreadsQueuer.Instance.QueueFuncToExecuteFromMainThread(() =>
                {
                    if (GameManager.Instance.activeGame == null || GameManager.Instance.activeGame.GameID != gameId) return;

                    turn = new ClientPlayerTurn(turnResults.Turn.Moves);
                    turn.PlayerId = turnResults.Turn.PlayerId;
                    turn.PlayerString = turnResults.Turn.PlayerString;
                    turn.Timestamp = turnResults.Turn.Timestamp;

                    turn.AITurn = true;

                    boardUpdateRoutines.Add(turnResults.GameState.UniqueId, StartCoroutine(BoardUpdateRoutine(turnResults)));

                    //manually reset noInput timer
                    StandaloneInputModuleExtended.instance.ResetNoInputTimer();
                });
            });
        }

        //when playerturn feed externaly (like when playing previous turn)
        //or realtime turn received from another client
        //local only
        public Coroutine TakeTurn(PlayerTurn playerTurn)
        {
            turn = null;

            //since spell are not created as a result of TakeTurn, need to create them manually
            playerTurn.Moves.ForEach(imove =>
            {
                if (imove.MoveType == MoveType.SPELL)
                {
                    ISpell spell = imove as ISpell;

                    switch (spell.SpellId)
                    {
                        //HEX spell
                        case SpellId.HEX:
                            HexSpell hexSpell = spell as HexSpell;

                            CastSpell(hexSpell.Location, hexSpell.SpellId);
                            break;
                    }
                }
            });

            return TakeTurn(playerTurn.GetMove(), true);
        }
        
        public Coroutine TakeTurn(Direction direction, int location, bool local = false) => TakeTurn(new SimpleMove(model.activePlayerPiece, direction, location), local);

        public Coroutine TakeTurn(SimpleMove move, bool local = false)
        {
            if (actionState == BoardActionState.CAST_SPELL) CancelSpell();

            if (!model.turnEvaluator.CanIMakeMove(move))
            {
                Debug.Log("Cannot Make Move");
                return null;
            }

            AddIMoveToTurn(move);

            PlayerTurnResult turnResults = null;

            //if (model._allTurnRecord.Count > 0)
            //{
                turnResults = model.TakeTurn(turn, local, true);

                //clear first before move actions
                while (turnResults.Activity.Count > 0 && turnResults.Activity[0].Timing != GameActionTiming.MOVE) turnResults.Activity.RemoveAt(0);
            //}
            //else
            //{
            //    turnResults = model.TakeTurn(turn, local, true);
            //}

            Coroutine coroutine = StartCoroutine(BoardUpdateRoutine(turnResults));

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

            IMove spell = null;
            TokenSpell token = InstantiateSpellToken(location, spellID);

            switch (spellID)
            {
                case SpellId.HEX:
                    spell = new HexSpell(model._State.ActivePlayerId, location);

                    break;

                    //case SpellId.FIRE_WALL:
                    //    spell = new FireWallSpell(game.State.ActivePlayerId, location);
                    //    token = SpawnToken(new FireToken()) as TokenSpell;
                    //    break;

                    //case SpellId.ICE_WALL:
                    //    spell = new IceWallSpell(location);
                    //    token = SpawnToken(new IceToken()) as TokenSpell;
                    //    break;
            }

            createdSpellTokens.Add(token);

            //make semi-transparent
            token.SetAlpha(.3f);

            AddIMoveToTurn(spell);

            //cast spell
            onCast?.Invoke(activeSpell, model._State.ActivePlayerId);
        }

        public TokenSpell InstantiateSpellToken(BoardLocation location, SpellId spellID)
        {
            TokenSpell token = null;

            switch (spellID)
            {
                case SpellId.HEX:
                    token = SpawnToken<TokenSpell>(location.Row, location.Column, TokenType.HEX);
                    break;

                    //case SpellId.FIRE_WALL:
                    //    spell = new FireWallSpell(game.State.ActivePlayerId, location);
                    //    token = SpawnToken(new FireToken()) as TokenSpell;
                    //    break;

                    //case SpellId.ICE_WALL:
                    //    spell = new IceWallSpell(location);
                    //    token = SpawnToken(new IceToken()) as TokenSpell;
                    //    break;
            }

            return token;
        }

        public void PrepareForSpell(SpellId spellId)
        {
            actionState = BoardActionState.CAST_SPELL;
            activeSpell = spellId;

            CancelRoutine("wrongMove");
            //show hint area depending on spell
            ShowHintArea(HintAreaStyle.NONE, HintAreaAnimationPattern.CIRCLE);
        }

        public void CancelSpell()
        {
            actionState = BoardActionState.SIMPLE_MOVE;
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
                        actionsMove.Add(activity[actionIndex]);
                    else
                    {
                        if (lastDirection == moveAction.Direction) actionsMove.Add(activity[actionIndex]);
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
                        InputMapValue inputMapValue = inputMap.Find(_inputMapValue => _inputMapValue.location.Equals(new BoardLocation(row, column)));

                        if (inputMapValue != null) inputMapValue.value = true;
                    }
                }
            }
        }

        public void SetInputMap(bool state) => inputMap.ForEach(inputMapValue => inputMapValue.value = state);

        public void OnBoardLocationEnter(BoardLocation location, BoardBit bit)
        {
            List<TokenView> other = BoardTokensAt(location);

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
        public Coroutine PlayInitialMoves() => PlayMoves(model.InitialTurns);

        public void Initialize(IClientFourzy model, bool hintAreas = true, bool createBits = true)
        {
            this.model = model;
            turn = null;
            lastCol = 0;
            lastRow = 0;

            actionState = BoardActionState.SIMPLE_MOVE;
            createdSpellTokens = new List<TokenSpell>();

            if (moveArrow) moveArrow.SetData(this, HOLD_TIME);

            CreateInputMap();

            StopBoardUpdates();

            CalculatePositions();

            if (hintAreas) CreateHintArea();

            if (createBits)
            {
                CancelRoutine("createBitsRoutine");
                StartRoutine("createBitsRoutine", CreateBitsRoutine());
            }

            UpdateHintArea();
        }

        public void StopBoardUpdates()
        {
            CancelRoutine("playMoves");
            CancelRoutine("createBitsRoutine");

            if (boardUpdateRoutines != null)
                foreach (Coroutine boardUpdateRoutine in boardUpdateRoutines.Values)
                   if(boardUpdateRoutine != null) StopCoroutine(boardUpdateRoutine);

            boardUpdateRoutines = new Dictionary<string, Coroutine>();

            StopAllCoroutines();
        }

        public void SortBits()
        {
            boardBits = boardBits.OrderBy(bit => bit.sortingGroup.sortingOrder).ToList();

            //sort in hirerarchy
            foreach (BoardBit bit in boardBits) bit.transform.SetAsLastSibling();
        }

        public void OnMove()
        {
            GameManager.Vibrate(MoreMountains.NiceVibrations.HapticTypes.HeavyImpact);

            TakeTurn(new SimpleMove(model.activePlayerPiece, GetDirection(selectedBoardLocation.Value), GetLocationFromBoardLocation(selectedBoardLocation.Value)));

            if (touched) OnPointerRelease(Input.mousePosition);

            selectedBoardLocation = null;
        }

        public Coroutine OnPlayManagerReady()
        {
            Coroutine coroutine = null;

            if (model._allTurnRecord.Count == 0)
            {
                coroutine = StartCoroutine(BoardUpdateRoutine(model.StartTurn()));

                //play first turns' before actions
                boardUpdateRoutines.Add(model._State.UniqueId, coroutine);
            }

            return coroutine;
        }

        public int GetLocationFromBoardLocation(BoardLocation _boardLocation) => _boardLocation.GetLocation(model);

        public Direction GetDirection(BoardLocation _boardLocation) => _boardLocation.GetTurnDirection(model);

        public bool InputMap(BoardLocation location)
        {
            InputMapValue inputMapValue = inputMap.Find(_value => _value.location.Equals(location));

            return inputMapValue == null ? false : inputMapValue.value;
        }

        public void ShowHintArea(HintAreaStyle style, HintAreaAnimationPattern pattern)
        {
            Dictionary<BoardLocation, HintBlock> affected = new Dictionary<BoardLocation, HintBlock>();

            TurnEvaluator turnEvaluator = model.turnEvaluator;
            Piece piece = model.activePlayerPiece;

            CancelRoutine("showHintBlocks");
            CancelRoutine("hideHintBlocks");
            foreach (HintBlock hintBlock in hintBlocks.Values) hintBlock.CancelAnimation();

            switch (actionState)
            {
                case BoardActionState.SIMPLE_MOVE:
                    //only active inputMap values
                    foreach (InputMapValue inputMapValue in inputMapActiveOnly)
                        if (turnEvaluator.CanIMakeMove(inputMapValue.Move))
                            if (hintBlocks.ContainsKey(inputMapValue.location))
                                affected.Add(inputMapValue.location, hintBlocks[inputMapValue.location]);

                    break;

                case BoardActionState.CAST_SPELL:
                    List<BoardLocation> locationsList = null;

                    switch (activeSpell)
                    {
                        case SpellId.HEX:
                            locationsList = SpellEvaluator.GetValidSpellLocations(model._State.Board, new HexSpell(0, new BoardLocation()));

                            foreach (KeyValuePair<BoardLocation, HintBlock> hintBlock in hintBlocks)
                                if (locationsList.Contains(hintBlock.Key))
                                    affected.Add(hintBlock.Key, hintBlock.Value);

                            SetHintAreaColliderState(true);
                            SetHintAreaSelectableState(false);
                            break;
                    }
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
            foreach (KeyValuePair<BoardLocation, HintBlock> hintBlock in hintBlocks)
            {
                hintBlock.Value.SetColliderState(state);

                if (!state)
                    hintBlock.Value.Hide();
            }
        }

        public void SetHintAreaSelectableState(bool state)
        {
            foreach (KeyValuePair<BoardLocation, HintBlock> hintBlock in hintBlocks) hintBlock.Value.SetSelectableState(state);
        }

        private void CalculatePositions()
        {
            if (boxCollider2D)
            {
                topLeft = boxCollider2D.offset + new Vector2(-boxCollider2D.bounds.size.x * .5f / transform.localScale.x, boxCollider2D.bounds.size.y * .5f / transform.localScale.y);
                step = new Vector3(boxCollider2D.size.x / model.Columns, boxCollider2D.size.y / model.Rows);
            }
            else if (rectTransform)
            {
                topLeft = new Vector3(-rectTransform.rect.size.x / 2f, rectTransform.rect.size.y / 2f);
                step = new Vector3(rectTransform.rect.width / model.Columns, rectTransform.rect.height / model.Rows);
            }
        }

        private bool CheckMove(Vector2 position, bool tap = false)
        {
            TurnEvaluator turnEvaluator = model.turnEvaluator;
            Piece piece = model.activePlayerPiece;

            InputMapValue inputMapValue = GetClosestInputMapValue(inputMapActiveOnly, position);

            if (inputMapValue == null)
            {
                if (tap)
                {
                    if (Vec2ToBoardLocation(position).OnBoard(model._State.Board))
                        negativeVfx.StartVfx(null, (Vector2)transform.position + BoardLocationToVec2(Vec2ToBoardLocation(position)), 0f);
                    else
                        negativeVfx.StartVfx(null, position, 0f);

                    //dont show hint area on onboarding game mode
                    if (model._Type != GameType.ONBOARDING)
                    {
                        SetHintAreaColliderState(false);

                        CancelRoutine("wrongMove");
                        //start wrong move routine
                        StartRoutine("wrongMove", 1.3f, () => UpdateHintArea(), null);

                        //only show in demo mode
                        if (SettingsManager.Instance.Get(SettingsManager.KEY_DEMO_MODE))
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

                if (InputMap(mirrored) && turnEvaluator.CanIMakeMove(new SimpleMove(piece, GetDirection(mirrored), GetLocationFromBoardLocation(mirrored))))
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
                    if (!previousLocation.Equals(mirrored) || tap) negativeVfx.StartVfx(null, (Vector2)transform.position + BoardLocationToVec2(inputMapValue.location), 0f);

                    moveArrow._Reset();
                    selectedBoardLocation = null;

                    previousLocation = mirrored;

                    return false;
                }
            }
        }

        private InputMapValue GetClosestInputMapValue(List<InputMapValue> _inputMap, Vector3 position, float maxDistance = 1.5f)
        {
            float _maxDistance = 0f;

            _maxDistance = maxDistance * step.x;

            InputMapValue closest = null;

            foreach (InputMapValue inputMapValue in _inputMap)
            {
                float distanceToCurrent = Vector2.Distance(position, BoardLocationToVec2(inputMapValue.location));

                if (distanceToCurrent <= _maxDistance)
                {
                    if (closest == null)
                        closest = inputMapValue;
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

            for (int col = 0; col < model.Columns; col++)
            {
                for (int row = 0; row < model.Rows; row++)
                {
                    //skip diagonals 
                    if (row == col || (model.Columns - 1) - col == row) continue;

                    //skip inside cells
                    if (row > 0 && col > 0 && row < model.Rows - 1 && col < model.Columns - 1) continue;

                    inputMap.Add(new InputMapValue(model, new BoardLocation(row, col), true));
                }
            }
        }

        private void CreateHintArea()
        {
            if (hintBlocks != null)
                foreach (HintBlock _hintBlock in hintBlocks.Values)
                    Destroy(_hintBlock.gameObject);

            hintBlocks = new Dictionary<BoardLocation, HintBlock>();

            for (int col = 0; col < model.Columns; col++)
            {
                for (int row = 0; row < model.Rows; row++)
                {
                    HintBlock hintBlock = GameContentManager.InstantiatePrefab<HintBlock>(GameContentManager.PrefabType.BOARD_HINT_BOX, bitsParent);
                    hintBlock.transform.localPosition = BoardLocationToVec2(row, col);

                    hintBlocks.Add(new BoardLocation(row, col), hintBlock);
                }
            }
        }

        private void UpdateHintArea()
        {
            if (hintBlocks == null || model.isOver) return;

            List<BoardLocation> affected = new List<BoardLocation>();

            TurnEvaluator turnEvaluator = model.turnEvaluator;
            Piece piece = model.activePlayerPiece;

            //only active inputMap values
            foreach (InputMapValue inputMapValue in inputMapActiveOnly)
                if (turnEvaluator.CanIMakeMove(inputMapValue.Move) && hintBlocks.ContainsKey(inputMapValue.location))
                    affected.Add(inputMapValue.location);

            foreach (BoardLocation key in hintBlocks.Keys) hintBlocks[key].SetColliderState(affected.Contains(key));
        }

        private void DropPiece(Vector2 worldPoint)
        {
            BoardLocation location = Vec2ToBoardLocation(worldPoint - (Vector2)transform.localPosition);

            if (BoardBitAt<GamePieceView>(location)) return;

            //drop gamepiece at location
            model._State.Board.AddPiece(model.activePlayerPiece, location);
            SpawnPiece(location.Row, location.Column, (PlayerEnum)model._State.ActivePlayerId);
        }

        private void AddIMoveToTurn(IMove move)
        {
            if (turn == null)
                turn = new ClientPlayerTurn(new List<IMove>() { move });
            else
                turn.Moves.Add(move);

            if (move.MoveType == MoveType.SIMPLE && turn.PlayerId == 0)
                turn.PlayerId = (move as SimpleMove).Piece.PlayerId;
        }

        private IEnumerator BoardUpdateRoutine(PlayerTurnResult turnResults)
        {
            HideHintArea(HintAreaAnimationPattern.NONE);

            if (turnResults.Activity.Count == 0) yield break;

            bool isGauntlet = model.puzzleData && model.puzzleData.gauntletStatus != null;
            SimpleMove move = turn != null ? turn.GetMove() : null;

            SetHintAreaColliderState(false);
            isAnimating = true;
            boardBits.ForEach(bit => { if (bit.active) bit.OnBeforeTurn(); });

            //if gauntlet game, remove member
            if (isGauntlet && move != null && move.Piece.PlayerId == model.me.PlayerId)
            {
                List<Creature> members = model._State.Herds[move.Piece.PlayerId].Members;
                if (members.Count > 0)
                {
                    members.RemoveAt(members.Count - 1);
                    model.puzzleData.pack.RemoveHerdMember();
                }
            }

            //invoke onMoveStart
            onMoveStarted?.Invoke(turn);

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
                            newGamePiece.Show(.25f);
                            newGamePiece.ScaleToCurrent(Vector3.zero, .25f);

                            firstGameActionMoveFound = true;
                        }

                        int prevActionIndex = actionIndex;

                        GameAction[] moveActions = GetMoveActions(turnResults.Activity, actionIndex);
                        actionIndex += moveActions.Length;

                        GamePieceView bit = (newGamePiece != null) ? newGamePiece : BoardBitAt<GamePieceView>((moveActions[0] as GameActionMove).Start);

                        float waitTime = (newGamePiece != null) ?
                            bit.ExecuteGameAction(moveActions.AddElementToStart(moveAction.InDirection(BoardLocation.Reverse(moveAction.Piece.Direction), 2))) : bit.ExecuteGameAction(moveActions);

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

                        GamePlayManager.instance.gameplayScreen.gameInfoWidget.DisplayPower(bossPowerAction.Power.PowerType);
                        model.BossMoves++;

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
                                BoardTokenAt<TokenView>(tokenDrop.Source, TokenType.FRUIT_TREE).OnActivate();
                                break;
                        }

                        //switch (tokenDrop.Reason)
                        //{
                        //    case TransitionType.BOSS_POWER:

                        //        break;
                        //}

                        yield return new WaitForSeconds(.5f);

                        actionIndex++;

                        break;

                    case GameActionType.REMOVE_TOKEN:
                        GameActionTokenRemove tokenRemove = turnResults.Activity[actionIndex] as GameActionTokenRemove;
                        yield return new WaitForSeconds(BoardTokenAt<TokenView>(tokenRemove.Location, tokenRemove.Before.Type)._Destroy(tokenRemove.Reason));
                        actionIndex++;

                        break;

                    case GameActionType.DESTROY:
                        //destroy gamepiece
                        GameActionDestroyed _destroy = turnResults.Activity[actionIndex] as GameActionDestroyed;

                        token = BoardBitAt<TokenView>(_destroy.End);

                        switch (_destroy.Reason)
                        {
                            case DestroyType.FALLING:
                                token.OnActivate();

                                break;

                            case DestroyType.BOMB:

                                break;
                        }

                        yield return new WaitForSeconds(BoardBitAt<GamePieceView>(_destroy.End)._Destroy(_destroy.Reason));

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
                        GameActionExplosion _explosion = turnResults.Activity[actionIndex] as GameActionExplosion;

                        Vfx bombVfx = VfxHolder.instance.GetVfx<Vfx>(VfxType.VFX_BOMB_EXPLOSION).StartVfx(transform, (Vector3)BoardLocationToVec2(_explosion.Center) + Vector3.back, 0f);

                        //destroy bomb
                        TokenView bomb = BoardTokenAt<TokenView>(_explosion.Center, TokenType.CIRCLE_BOMB) ?? BoardTokenAt<TokenView>(_explosion.Center, TokenType.CROSS_BOMB);
                        bomb?._Destroy();

                        yield return new WaitForSeconds(bombVfx.duration);

                        actionIndex++;

                        break;

                    case GameActionType.TRANSITION:
                        if (turnResults.Activity[actionIndex].GetType() == typeof(GameActionTokenTransition))
                        {
                            GameActionTokenTransition _tokenTransition = turnResults.Activity[actionIndex] as GameActionTokenTransition;

                            yield return new WaitForSeconds(BoardTokenAt<TokenView>(_tokenTransition.Location, _tokenTransition.Before.Type).ExecuteGameAction(_tokenTransition));
                        }
                        else if (turnResults.Activity[actionIndex].GetType() == typeof(GameActionTokenMovement))
                        {
                            //reset delay value
                            if (actionIndex > 0 && turnResults.Activity[actionIndex - 1].Type != GameActionType.TRANSITION) delay = 0f;

                            GameActionTokenMovement _tokenMovement = turnResults.Activity[actionIndex] as GameActionTokenMovement;

                            token = BoardTokenAt<TokenView>(_tokenMovement.Start, _tokenMovement.Token.Type);

                            switch (_tokenMovement.Reason)
                            {
                                //case TransitionType.GHOST_MOVE:
                                default:

                                    delay = token.StartMoveRoutine(_tokenMovement);
                                    if (delay > customDelay) customDelay = delay;

                                    break;
                            }
                        }
                        else if (turnResults.Activity[actionIndex].GetType() == typeof(GameActionTokenRotation))
                        {
                            //reset delay value
                            if (actionIndex > 0 && turnResults.Activity[actionIndex - 1].Type != GameActionType.TRANSITION) delay = 0f;

                            GameActionTokenRotation _tokenRotation = turnResults.Activity[actionIndex] as GameActionTokenRotation;

                            token = BoardTokenAt<TokenView>(_tokenRotation.Token.Space.Location, _tokenRotation.Token.Type);

                            switch (_tokenRotation.Reason)
                            {
                                default:
                                    //dont wait
                                    //yield return new WaitForSeconds(token.RotateTo(_tokenRotation.StartOrientation, _tokenRotation.EndOrientation, _tokenRotation.RotationDirection));
                                    delay = token.RotateTo(_tokenRotation.StartOrientation, _tokenRotation.EndOrientation, _tokenRotation.RotationDirection);
                                    if (delay > customDelay) customDelay = delay;

                                    break;
                            }
                        }

                        delayedActionType = turnResults.Activity[actionIndex].Type;
                        actionIndex++;

                        break;

                    case GameActionType.GAME_END:
                        SetHintAreaColliderState(false);

                        GameActionGameEnd _gameEndAction = turnResults.Activity[actionIndex] as GameActionGameEnd;

                        switch (_gameEndAction.GameEndType)
                        {
                            case GameEndType.WIN:
                                model.OnVictory();
                                onGameFinished?.Invoke(model);

                                break;

                            case GameEndType.DRAW:
                                model.OnDraw();
                                onDraw?.Invoke(model);

                                break;
                        }

                        List<GamePieceView> winningGamepieces = GetWinningPieces();
                        for (int index = 0; index < winningGamepieces.Count; index++)
                            winningGamepieces[index].PlayWinAnimation(index * .15f + .15f);

                        actionIndex++;

                        break;

                    case GameActionType.PASS:
                        float turnPassDuration = 1.8f;

                        ////show message
                        //GamePlayManager.instance.gameplayScreen.ShowOpponentMessage("Turn pass..", turnPassDuration - .22f);
                        GamePlayManager.instance.gameplayScreen.gameInfoWidget.PassTurn(turnPassDuration);

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

            //check if gauntlet game finished
            if (isGauntlet && move != null && move.Piece.PlayerId == model.me.PlayerId)
            {
                if (model._State.Herds[move.Piece.PlayerId].Members.Count == 0 && model._State.WinningLocations == null)
                {
                    SetHintAreaColliderState(false);
                    onGameFinished?.Invoke(model);
                }
            }

            //since puzzle games dont send "lose" event, need to check manually
            switch (model._Type)
            {
                case GameType.PUZZLE:
                    if (model.isOver)
                    {
                        //player lost
                        if (model._State.WinningLocations == null)
                        {
                            SetHintAreaColliderState(false);
                            onGameFinished?.Invoke(model);
                        }
                    }

                    break;
            }

            //check if any of created space been covered by gamepieces
            if (createdSpellTokens.Count > 0)
                gamePieces.ForEach(gamePiece =>
                {
                    TokenSpell spell = createdSpellTokens.Find(spellToken => spellToken.location.Equals(gamePiece.location));

                    //remove it
                    if (spell)
                    {
                        spell._Destroy();
                        createdSpellTokens.Remove(spell);
                    }
                });

            if (turn != null)
            {
                //add spells to model
                createdSpellTokens.ForEach(spell =>
                {
                    switch (spell.spellId)
                    {
                        case SpellId.HEX:
                            (new HexSpell(turn.PlayerId, spell.location)).Cast(model._State);
                            spell.SetAlpha(1f);

                            break;
                    }
                });
            }

            isAnimating = false;
            boardBits.ForEach(bit => { if (bit.active) bit.OnAfterTurn(); });

            onMoveEnded?.Invoke(turn, turnResults);

            //update hint blocks
            UpdateHintArea();

            SetHintAreaSelectableState(true);

            turn = null;
            createdSpellTokens.Clear();

            boardUpdateRoutines.Remove(turnResults.GameState.UniqueId);
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
                    for (int diagonalIndex = 0; diagonalIndex < model.Rows * 2 - 1; diagonalIndex++)
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

                    for (int distance = 0; distance < model.Rows; distance++)
                    {
                        foreach (KeyValuePair<BoardLocation, HintBlock> hintBlock in affected)
                            if (!animated.Contains(hintBlock.Key) && BoardLocationToVec2(hintBlock.Key).magnitude < distance * step.x)
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
            switch (pattern)
            {
                case HintAreaAnimationPattern.NONE:
                    foreach (HintBlock hintBlock in hintBlocks.Values) hintBlock.CancelAnimation();

                    break;

                case HintAreaAnimationPattern.DIAGONAL:
                    for (int diagonalIndex = 0; diagonalIndex < model.Rows * 2 - 1; diagonalIndex++)
                    {
                        for (int col = 0; col <= diagonalIndex; col++)
                        {
                            BoardLocation location = new BoardLocation(diagonalIndex - col, col);

                            if (hintBlocks.ContainsKey(location))
                                hintBlocks[location].CancelAnimation();
                        }

                        yield return new WaitForSeconds(.04f);
                    }

                    break;

                case HintAreaAnimationPattern.CIRCLE:
                    List<BoardLocation> animated = new List<BoardLocation>();

                    for (int distance = 0; distance < model.Rows; distance++)
                    {
                        foreach (KeyValuePair<BoardLocation, HintBlock> hintBlock in hintBlocks)
                            if (!animated.Contains(hintBlock.Key) && BoardLocationToVec2(hintBlock.Key).magnitude < distance * step.x)
                            {
                                hintBlock.Value.CancelAnimation();

                                animated.Add(hintBlock.Key);
                            }

                        yield return new WaitForSeconds(.07f);
                    }

                    break;
            }
        }

        public IEnumerator CreateBitsRoutine(bool clear = true, bool delay = false)
        {
            if (clear)
            {
                if (boardBits != null)
                    foreach (BoardBit bit in boardBits)
                        Destroy(bit.gameObject);

                boardBits = new List<BoardBit>();
            }

            if (model == null) yield break;

            int _lastRow = lastRow;
            int _lastCol = lastCol;

            for (int row = _lastRow; row < model.Rows; row++)
            {
                lastRow = row;

                for (int col = _lastCol; col < model.Columns; col++)
                {
                    lastCol = col;

                    BoardSpace boardSpace = model._State.Board.ContentsAt(row, col);

                    foreach (IToken token in boardSpace.Tokens.Values)
                        SpawnToken(token);

                    foreach (Piece piece in boardSpace.Pieces)
                        SpawnPiece(row, col, (PlayerEnum)piece.PlayerId);

                    if (delay && (boardSpace.Tokens.Values.Count + boardSpace.Pieces.Count) > 0) yield return new WaitForEndOfFrame();
                }

                _lastCol = 0;
            }
        }

        private IEnumerator PlayTurnsRoutine(List<PlayerTurn> turns)
        {
            foreach (PlayerTurn turn in turns)
            {
                yield return TakeTurn(turn.GetMove().Direction, turn.GetMove().Location, true);
                
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

                move = new SimpleMove(model.activePlayerPiece, location.GetTurnDirection(model), location.GetLocation(model));
            }
        }

        public enum BoardActionState
        {
            SIMPLE_MOVE,
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