//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics._Vfx;
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

namespace Fourzy._Updates.Mechanics.Board
{
    public class GameboardView : RoutinesBase
    {
        public static float HOLD_TIME = .5f;
        public static float QUICK_TAP_TIME = .15f;

        public Transform bitsParent;
        public bool debugBoard = false;
        public bool sortByOrder = false;
        public bool interactable = false;
        public IClientFourzy model;
        
        public Action<IClientFourzy> onGameFinished;
        public Action<IClientFourzy> onDraw;
        public Action<int> onMoveStarted;
        public Action<int> onMoveEnded;
        public Action onCastCanceled;
        public Action<SpellId, int> onCast;
        public Action onWrongTurn;

        private Vector3 topLeft;
        private bool touched = false;
        private float holdTimer;
        private float tapStartTime;
        private HintBlock previousClosest;

        private AlphaTween alphaTween;
        private PlayerTurn turn = null;
        private Dictionary<string, Coroutine> boardUpdateRoutines;
        private Vfx negativeVfx;
        private Thread aiTurnThread;

        public Dictionary<BoardLocation, HintBlock> hintBlocks { get; private set; }
        public Dictionary<BoardLocation, CellWrapper> allowedInputMap { get; private set; }
        public BoxCollider2D boxCollider2D { get; private set; }
        public RectTransform rectTransform { get; private set; }
        public Vector3 step { get; private set; }
        public List<BoardBit> boardBits { get; private set; }
        public bool isAnimating { get; private set; }
        public BoardActionState actionState { get; private set; }
        public BoardMode boardMode { get; private set; }
        public SpellId activeSpell { get; private set; }
        public List<TokenSpell> createdSpellTokens { get; private set; }
        public MoveArrow moveArrow { get; private set; }
        public BoardLocation? selectedBoardLocation { get; private set; }
        public BoardLocation previousLocation { get; private set; }
        public int direction { get; private set; }

        public float bitSpeed => Constants.moveSpeed * step.x;

        public List<BoardBit> tokens => boardBits.Where(bit => (bit.GetType() == typeof(TokenView) || bit.GetType().IsSubclassOf(typeof(TokenView)))).ToList();

        public List<BoardBit> gamePieces => boardBits.Where(bit => (bit.GetType() == typeof(GamePieceView) || bit.GetType().IsSubclassOf(typeof(GamePieceView)))).ToList();

        protected override void Awake()
        {
            base.Awake();

            if (!bitsParent)
                bitsParent = transform;

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
            if (selectedBoardLocation != null && (holdTimer -= Time.deltaTime) <= 0f)
                OnMove();
        }

        protected void OnDestroy()
        {
            if (aiTurnThread != null && aiTurnThread.IsAlive) aiTurnThread.Abort();
        }

        public void OnPointerDown(Vector2 position)
        {
            if (isAnimating) return;

            BoardLocation _location = Vec2ToBoardLocation(Camera.main.ScreenToWorldPoint(position) - transform.localPosition);

            if (model.isOver) return;

            if (_location.OnBoard(model._State.Board))
            {
                if (model._Type != GameType.PASSANDPLAY)
                {
                    if (!model.isMyTurn)
                    {
                        onWrongTurn?.Invoke();
                        return;
                    }
                }
            }

            if (!interactable) return;

            if (selectedBoardLocation != null)
            {
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
                    HintBlock hintBlock = GetClosestHintBlock(Camera.main.ScreenToWorldPoint(position), false);

                    if (!hintBlock)
                        onCastCanceled?.Invoke();
                    else
                        CastSpell(hintBlock.boardLocation, activeSpell);

                    actionState = BoardActionState.SIMPLE_MOVE;
                    HideHintArea();
                    break;
            }
        }

        public void OnPointerMove(Vector2 position)
        {
            if (!interactable || !touched)
                return;

            CheckMove(Camera.main.ScreenToWorldPoint(position) - transform.localPosition);
        }

        public void OnPointerRelease(Vector2 position)
        {
            if (!interactable || !touched)
                return;

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

            if (sort)
                SortBits();

            return tokenInstance as T;
        }

        public void RemoveBoardBit(BoardBit bit)
        {
            if (boardBits.Contains(bit))
                boardBits.Remove(bit);
        }

        public void TakeAITurn()
        {
            aiTurnThread = ThreadsQueuer.Instance.StartThreadForFunc(() =>
            {
                string gameId = model.GameID;

                PlayerTurnResult turnResults = model.TakeAITurn(true);

                //clear first before move actions
                while (turnResults.Activity.Count > 0 && turnResults.Activity[0].Timing != GameActionTiming.MOVE) turnResults.Activity.RemoveAt(0);

                ThreadsQueuer.Instance.QueueFuncToExecuteFromMainThread(() =>
                {
                    if (GameManager.Instance.activeGame == null || GameManager.Instance.activeGame.GameID != gameId) return;

                    turn = turnResults.Turn;
                    boardUpdateRoutines.Add(turnResults.GameState.UniqueId, StartCoroutine(BoardUpdateRoutine(turnResults)));
                });
            });
        }

        //when playerturn feed externaly (like when playing previous turn)
        //local only
        public void TakeTurn(PlayerTurn playerTurn)
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

            TakeTurn(playerTurn.GetMove(), true);
        }

        //public void TakeTurn(Direction direction, int location, bool local = false) => TakeTurn(new SimpleMove(game.activePlayerPiece, direction, location), local);
        public void TakeTurn(Direction direction, int location, bool local = false) => TakeTurn(new SimpleMove(model.activePlayerPiece, direction, location), local);

        public void TakeTurn(SimpleMove move, bool local = false)
        {
            if (!model.turnEvaluator.CanIMakeMove(move))
            {
                Debug.Log("Cannot Make Move");
                return;
            }

            AddIMoveToTurn(move);

            PlayerTurnResult turnResults = model.TakeTurn(turn, local);

            //clear first before move actions
            while (turnResults.Activity.Count > 0 && turnResults.Activity[0].Timing != GameActionTiming.MOVE) turnResults.Activity.RemoveAt(0);

            boardUpdateRoutines.Add(turnResults.GameState.UniqueId, StartCoroutine(BoardUpdateRoutine(turnResults)));
        }

        /// <summary>
        /// When local player casts a spell
        /// </summary>
        /// <param name="location"></param>
        public void CastSpell(BoardLocation location, SpellId spellID)
        {
            IMove spell = null;
            TokenSpell token = InstantiateSpellToken(location, spellID);

            switch (spellID)
            {
                case SpellId.HEX:
                    //spell = new HexSpell(game.State.ActivePlayerId, location);
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

        public float WaitTimeForDistance(float distance) => (distance * step.x) / bitSpeed;

        public void PrepareForSpell(SpellId spellId)
        {
            actionState = BoardActionState.CAST_SPELL;
            activeSpell = spellId;

            //show hint area depending on spell
            ShowHintArea();
        }

        public void CancelSpell()
        {
            actionState = BoardActionState.SIMPLE_MOVE;
            HideHintArea();
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
                        if (lastDirection == moveAction.Direction)
                            actionsMove.Add(activity[actionIndex]);
                        else
                            break;
                    }

                    lastDirection = moveAction.Direction;
                }
                else
                    break;
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
            ResetInputLimit(false);

            foreach (Rect area in areas)
            {
                for (int column = (int)area.x; column < (int)(area.x + area.width); column++)
                {
                    for (int row = (int)area.y; row < (int)(area.y + area.height); row++)
                    {
                        BoardLocation location = new BoardLocation(row, column);
                        
                        if (SettingsManager.Instance.Get(SettingsManager.KEY_MOVE_ORIGIN)) location = location.Mirrored(model);

                        if (allowedInputMap.ContainsKey(location)) allowedInputMap[location].state = true;
                    }
                }
            }
        }

        public void ResetInputLimit(bool state)
        {
            foreach (CellWrapper cell in allowedInputMap.Values)
                cell.state = state;
        }

        public void OnBoardLocationEnter(BoardLocation location, BoardBit bit)
        {
            List<TokenView> other = BoardTokensAt(location);

            other.ForEach(_bit => _bit.OnBitEnter(bit));

            bit.turnTokensInteractionList.AddRange(other);

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

        public void PlayMoves(List<PlayerTurn> moves)
        {
            if (moves == null)
                return;

            //only one set can play at a time
            CancelRoutine("playMoves");

            StartRoutine("playMoves", PlayTurnsRoutine(moves), null);
        }

        /// <summary>
        /// Will play initial moves if game was created with gameboardDefinition constructor
        /// </summary>
        public void TryPlayInitialMoves()
        {
            if (model.InitialTurns != null && model.InitialTurns.Count > 0) PlayMoves(model.InitialTurns);
        }

        public void Initialize(IClientFourzy model)
        {
            this.model = model;
            turn = null;

            boardMode = model.GetType() == typeof(ClientFourzyGame) ? BoardMode.FOURZY_GAME : BoardMode.FOURZY_PUZZLE;
            actionState = BoardActionState.SIMPLE_MOVE;
            createdSpellTokens = new List<TokenSpell>();

            if (moveArrow) moveArrow.SetData(this, HOLD_TIME);

            StopBoardUpdates();

            CalculatePositions();

            CreateHintArea();

            GenerateBoard();
        }

        public void StopBoardUpdates()
        {
            CancelRoutine("playMoves");

            if (boardUpdateRoutines != null)
                foreach (Coroutine boardUpdateRoutine in boardUpdateRoutines.Values)
                    StopCoroutine(boardUpdateRoutine);

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

            if (touched)
                OnPointerRelease(Input.mousePosition);
            else
                selectedBoardLocation = null;
        }
        
        public int GetLocationFromBoardLocation(BoardLocation _boardLocation) => _boardLocation.GetLocation(model);
        
        public Direction GetDirection(BoardLocation _boardLocation) => _boardLocation.GetDirection(model);

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

            BoardLocation _newLocation = Vec2ToBoardLocation(position);
            BoardLocation mirrored = SettingsManager.Instance.Get(SettingsManager.KEY_MOVE_ORIGIN) ? _newLocation.Mirrored(model) : _newLocation;

            //if (mirrored.OnBoard(game.State.Board))
            if (mirrored.OnBoard(model._State.Board))
                {
                if (allowedInputMap.ContainsKey(mirrored) && allowedInputMap[mirrored].state &&
                    turnEvaluator.CanIMakeMove(new SimpleMove(piece, GetDirection(mirrored), GetLocationFromBoardLocation(mirrored))))
                {
                    if (!mirrored.Equals(selectedBoardLocation))
                    {
                        holdTimer = HOLD_TIME;
                        selectedBoardLocation = mirrored;

                        //position arrow
                        moveArrow.Position(_newLocation);
                        moveArrow.Rotate(GetDirection(_newLocation));

                        moveArrow._Reset();
                        moveArrow.Animate();
                    }

                    previousLocation = mirrored;

                    return true;
                }
                else
                {
                    if (!previousLocation.Equals(mirrored) || tap) negativeVfx.StartVfx(null, (Vector2)transform.position + BoardLocationToVec2(_newLocation), 0f);

                    moveArrow._Reset();
                    selectedBoardLocation = null;

                    previousLocation = mirrored;

                    return false;
                }
            }
            else
                return false;
        }

        private void ShowHintArea()
        {
            TurnEvaluator turnEvaluator = model.turnEvaluator;
            Piece piece = model.activePlayerPiece;

            switch (actionState)
            {
                case BoardActionState.SIMPLE_MOVE:
                    foreach (HintBlock hintBlock in hintBlocks.Values)
                        if (hintBlock.active && turnEvaluator.CanIMakeMove(hintBlock.GetMove(piece)))
                            hintBlock.Show();
                    break;

                case BoardActionState.CAST_SPELL:
                    List<BoardLocation> locationsList = null;

                    switch (activeSpell)
                    {
                        case SpellId.HEX:
                            locationsList = SpellEvaluator.GetValidSpellLocations(model._State.Board, new HexSpell(0, new BoardLocation()));

                            foreach (HintBlock hintBlock in hintBlocks.Values)
                                if (hintBlock.active && locationsList.Contains(hintBlock.boardLocation))
                                    hintBlock.Show();
                            break;
                    }
                    break;
            }

            previousClosest = null;
        }

        private void HideHintArea()
        {
            foreach (HintBlock hintBlock in hintBlocks.Values)
                hintBlock.Hide();

            previousClosest = null;
        }

        private HintBlock GetClosestHintBlock(Vector3 position, bool snapToClosest = true)
        {
            HintBlock closest = null;

            //get first unblocked one
            if (snapToClosest)
                closest = hintBlocks.Values.ToList().Find(hintBlock => hintBlock.active && hintBlock.shown);
            else
                return hintBlocks.Values.ToList().Where(hintBlock => hintBlock.active && hintBlock.shown).ToList().Find(hintBlock => Vector2.Distance(position, hintBlock.transform.position) < hintBlock.radius);

            if (closest != null)
                foreach (HintBlock hintBlock in hintBlocks.Values)
                    if (hintBlock.active && hintBlock.shown && Vector2.Distance(position, hintBlock.transform.position) < Vector2.Distance(position, closest.transform.position))
                        closest = hintBlock;

            return closest;
        }

        private void SelectHintBlock(Vector3 mousePosition)
        {
            HintBlock closest = GetClosestHintBlock(Camera.main.ScreenToWorldPoint(mousePosition), /*!hintBlocks.Any(hintBlock => !hintBlock.Value.active)*/false);

            if (closest == null || !closest.shown)
                return;

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

        private void GenerateBoard()
        {
            if (boardBits != null)
                foreach (BoardBit bit in boardBits)
                    Destroy(bit.gameObject);

            boardBits = new List<BoardBit>();
            
            if (model == null) return;

            for (int row = 0; row < model.Rows; row++)
            {
                for (int col = 0; col < model.Columns; col++)
                {
                    BoardSpace boardSpace = model._State.Board.ContentsAt(row, col);

                    foreach (IToken token in boardSpace.Tokens.Values)
                        SpawnToken(token);

                    foreach (Piece piece in boardSpace.Pieces)
                        SpawnPiece(row, col, (PlayerEnum)piece.PlayerId);
                }
            }
        }

        private void CreateHintArea()
        {
            if (hintBlocks != null)
                foreach (HintBlock _hintBlock in hintBlocks.Values)
                    Destroy(_hintBlock.gameObject);

            hintBlocks = new Dictionary<BoardLocation, HintBlock>();

            //create input map
            allowedInputMap = new Dictionary<BoardLocation, CellWrapper>();

            for (int col = 0; col < model.Columns; col++)
            {
                for (int row = 0; row < model.Rows; row++)
                {
                    HintBlock hintBlock = GameContentManager.InstantiatePrefab<HintBlock>(GameContentManager.PrefabType.BOARD_HINT_BOX, bitsParent);
                    hintBlock.transform.localPosition = BoardLocationToVec2(row, col);

                    BoardLocation location = new BoardLocation(row, col);
                    hintBlock.SetData(location, this);

                    hintBlocks.Add(location, hintBlock);

                    //input map
                    if (!location.IsCorner(model._State.Board))
                        if (col == 0 || col == model.Columns - 1 || row == 0 || row == model.Rows - 1) allowedInputMap.Add(location, new CellWrapper() { state = true, });
                }
            }
        }

        private void AddIMoveToTurn(IMove move)
        {
            if (turn == null)
                turn = new PlayerTurn(new List<IMove>() { move });
            else
                turn.Moves.Add(move);

            if (move.MoveType == MoveType.SIMPLE && turn.PlayerId == 0)
                turn.PlayerId = (move as SimpleMove).Piece.PlayerId;
        }

        private IEnumerator BoardUpdateRoutine(PlayerTurnResult turnResults)
        {
            isAnimating = true;
            boardBits.ForEach(bit => { if (bit.active) bit.OnBeforeTurn(); });

            onMoveStarted?.Invoke(turn.PlayerId);

            int actionIndex = 0;
            bool firstGameActionMoveFound = false;

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
                                    waitTime = Mathf.Clamp01(waitTime - WaitTimeForDistance(.9f));
                                    break;
                            }
                        }

                        newGamePiece = null;
                        yield return new WaitForSeconds(waitTime);
                        break;

                    case GameActionType.ADD_TOKEN:
                        GameActionTokenDrop tokenDrop = turnResults.Activity[actionIndex] as GameActionTokenDrop;

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

                        VfxHolder.instance.ShowVfx(VfxType.VFX_BOMB_EXPLOSION, transform, BoardLocationToVec2(_explosion.Center));

                        //destroy bomb
                        TokenView bomb = BoardTokenAt<TokenView>(_explosion.Center, TokenType.CIRCLE_BOMB) ?? BoardTokenAt<TokenView>(_explosion.Center, TokenType.CROSS_BOMB);
                        bomb?._Destroy();

                        actionIndex++;
                        break;

                    case GameActionType.TRANSITION:
                        GameActionTokenTransition _tokenTransition = turnResults.Activity[actionIndex] as GameActionTokenTransition;

                        if (_tokenTransition.After != null)
                            StartCoroutine(TransitionAction(_tokenTransition));

                        actionIndex++;
                        break;

                    case GameActionType.GAME_END:
                        GameActionGameEnd _gameEndAction = turnResults.Activity[actionIndex] as GameActionGameEnd;

                        switch (_gameEndAction.GameEndType)
                        {
                            case GameEndType.WIN:
                                onGameFinished?.Invoke(model);
                                break;

                            case GameEndType.DRAW:
                                onDraw?.Invoke(model);
                                break;
                        }

                        if (model._State.WinningLocations != null)
                            for (int locationIndex = 0; locationIndex < model._State.WinningLocations.Count; locationIndex++)
                                BoardBitAt<GamePieceView>(model._State.WinningLocations[locationIndex]).PlayWinAnimation(locationIndex * .15f);

                        actionIndex++;
                        break;

                    default:
                        actionIndex++;
                        break;
                }
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

            isAnimating = false;
            boardBits.ForEach(bit => { if (bit.active) bit.OnAfterTurn(); });

            onMoveEnded?.Invoke(turn.PlayerId);

            turn = null;
            createdSpellTokens.Clear();

            boardUpdateRoutines.Remove(turnResults.GameState.UniqueId);
        }

        private IEnumerator TransitionAction(GameActionTokenTransition _transition)
        {
            TokenView activated = BoardTokenAt<TokenView>(_transition.Location, _transition.Before.Type);

            TokenView newToken = SpawnToken<TokenView>(_transition.Location.Row, _transition.Location.Column, _transition.After.Type, true);
            newToken.SetAlpha(0f);

            yield return StartCoroutine(activated.OnActivated());

            newToken.Show(.3f);
        }

        private IEnumerator PlayTurnsRoutine(List<PlayerTurn> turns)
        {
            foreach (PlayerTurn turn in turns)
            {
                TakeTurn(turn.GetMove().Direction, turn.GetMove().Location, true);

                yield return new WaitWhile(() => isAnimating);
                yield return new WaitForSeconds(.5f);
            }
        }

        public class CellWrapper
        {
            public bool state;
        }

        public enum BoardActionState
        {
            SIMPLE_MOVE,
            CAST_SPELL,
        }

        public enum BoardMode
        {
            NONE,
            FOURZY_GAME,
            FOURZY_PUZZLE,
        }
    }
}