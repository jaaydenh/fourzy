//@vadym udod

using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using FourzyGameModel.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Fourzy._Updates.Mechanics
{
    [RequireComponent(typeof(SortingGroup))]
    [RequireComponent(typeof(AlphaTween))]
    public abstract class BoardBit : RoutinesBase
    {
        public GameObject _body;
        public float onBoardScale = 1f;
        public float _radius = .65f;

        protected SpriteRenderer[] spriteRenderers;
        protected SpriteToImage[] imageRenderers;
        protected RectTransform parentRectTransform;
        protected OutlineBase outline;

        protected bool initialized;
        protected int originalSortingOrder;
        protected CircleCollider2D circleCollider;

        public bool active { get; set; }
        public float originalSpeed { get; private set; }
        public float speedCap { get; private set; }
        public float speedMltp { get; protected set; } = 1f;
        public float speed => Mathf.Clamp(originalSpeed * speedMltp, originalSpeed * .5f, speedCap);
        public Dictionary<BitBuffType, List<BitBuff>> buffs { get; private set; }

        public RectTransform rectTransform { get; private set; }
        public SortingGroup sortingGroup { get; private set; }
        public AlphaTween alphaTween { get; private set; }
        public PositionTween positionTween { get; private set; }
        public ScaleTween scaleTween { get; private set; }
        public ColorTween colorTween { get; private set; }
        public RotationTween rotationTween { get; private set; }
        public GameboardView gameboard { get; private set; }
        public List<TokenView> turnTokensInteractionList { get; private set; }
        public virtual Color outlineColor { get; }
        public string id { get; protected set; }

        public BoardLocation location
        {
            get
            {
                if (!gameboard)
                    return new BoardLocation(0, 0);

                return gameboard.Vec2ToBoardLocation(transform.localPosition);
            }
        }

        public float radius => _radius * transform.lossyScale.x;

        public GameObject body
        {
            get
            {
                if (!_body)
                    return gameObject;

                return _body;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            Initialize();
        }

        protected void Update()
        {
            if (sortingGroup)
                sortingGroup.sortingOrder = originalSortingOrder + location.Row;
        }

        protected void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public void SetAlpha(float value)
        {
            alphaTween.SetAlpha(value);

            if (outline) outline.outlineColor = new Color(outline.outlineColor.r, outline.outlineColor.g, outline.outlineColor.b, value);
        }

        public virtual void Hide(float time)
        {
            if (time == 0f)
                alphaTween.SetAlpha(0f);
            else
            {
                alphaTween.playbackTime = time;
                alphaTween.PlayBackward(true);
            }
        }

        public virtual void Show(float time)
        {
            if (time == 0f)
                alphaTween.SetAlpha(1f);
            else
            {
                alphaTween.playbackTime = time;
                alphaTween.PlayForward(true);
            }
        }

        public void Scale(Vector3 from, Vector3 to, float time)
        {
            scaleTween.from = from;
            scaleTween.to = to;
            scaleTween.playbackTime = time;

            scaleTween.PlayForward(true);
        }

        public virtual void ScaleToCurrent(Vector3 value, float time) => Scale(value, transform.localScale, time);

        public virtual void ScaleFromCurrent(Vector3 value, float time) => Scale(transform.localScale, value, time);

        public virtual void RotateTo(float value, RepeatType repeatType, float time, bool startTurn = false)
        {
            rotationTween.from = rotationTween._value;
            rotationTween.to = Vector3.forward * value;
            rotationTween.playbackTime = time;
            rotationTween.repeat = repeatType;

            rotationTween.PlayForward(true);
        }

        /// <summary>
        /// Time == 0 means autocalculate
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="rotation"></param>
        /// <param name="time"></param>
        public virtual float RotateTo(Direction from, Direction to, Rotation rotation, float time = 0f, bool startTurn = false)
        {
            float _from = 0f;
            float _to = 0f;
            float _time = time;

            if (rotation == Rotation.CLOCKWISE)
            {
                switch (from)
                {
                    case Direction.RIGHT:
                        _from = -90f;

                        break;

                    case Direction.DOWN:
                        _from = -180f;

                        break;

                    case Direction.LEFT:
                        _from = -270f;

                        break;
                }

                switch (to)
                {
                    case Direction.RIGHT:
                        _to = -90f;

                        break;

                    case Direction.DOWN:
                        _to = -180f;

                        break;

                    case Direction.LEFT:
                        _to = -270f;

                        break;
                }

                if (_to > _from) _to -= 360f;
            }
            else
            {
                switch (from)
                {
                    case Direction.LEFT:
                        _from = 90f;

                        break;

                    case Direction.DOWN:
                        _from = 180f;

                        break;

                    case Direction.RIGHT:
                        _from = 270f;

                        break;
                }

                switch (to)
                {
                    case Direction.LEFT:
                        _to = 90f;

                        break;

                    case Direction.DOWN:
                        _to = 180f;

                        break;

                    case Direction.RIGHT:
                        _to = 270f;

                        break;
                }

                if (_from > _to) _to += 360f;
            }

            if (_time == 0f) _time = Mathf.Abs(Mathf.Abs(_from) - Mathf.Abs(_to)) / 90f * .5f;

            rotationTween.from = Vector3.forward * _from;
            rotationTween.to = Vector3.forward * _to;
            rotationTween.playbackTime = _time;

            rotationTween.PlayForward(true);

            return _time;
        }

        public virtual void ShowOutline(bool repeat)
        {
            if (!outline)
            {
                if (parentRectTransform)
                {
                    outline = body.AddComponent<UIOutline>();
                }
                else
                {
                    outline = body.AddComponent<SpriteRendererOutline>();
                }
            }

            outline.blueSize = .0015f;
            outline.outlineColor = outlineColor;
            outline.Animate(0f, 1f, 1f, repeat);
        }

        public virtual void HideOutline(bool hide)
        {
            if (outline)
            {
                if (hide)
                {
                    outline.HideOutline();
                }
                else
                {
                    outline.Animate(outline.tween._value, 0f, .5f, false);
                }
            }
        }

        public virtual void SetMaterial(Material material)
        {
            if (parentRectTransform)
            {
                foreach (SpriteToImage imageRenderer in imageRenderers)
                {
                    imageRenderer.image.material = material;
                }
            }
            else
            {
                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.material = material;
                }
            }
        }

        public virtual void Position(BoardLocation boardLocation) => transform.localPosition = gameboard.BoardLocationToVec2(boardLocation);

        public virtual void SetPositionFromTo(Vector2 from, Vector2 to)
        {
            positionTween.from = from;
            positionTween.to = to;
        }

        public virtual float StartMoveRoutine(bool startMove, BoardLocation from, BoardLocation to)
        {
            StartCoroutine(MoveRoutine(startMove, from, to));

            return gameboard.BoardLocationsToBoardDistance(from, to) / speed;
        }

        public virtual float OnGameAction(GameAction action) { return 0f; }

        public virtual void OnBeforeTurn(bool startTurn)
        {
            turnTokensInteractionList.Clear();
            speedMltp = 1f;
        }

        public virtual void OnAfterTurn(bool startTurn) { }

        public virtual void OnBeforeMoveActions(bool startTurn, BoardLocation from, BoardLocation to) { }

        public virtual void OnAfterMoveAction(bool startTurn, BoardLocation from, BoardLocation to)
        {
            speedMltp = 1f;
        }

        public virtual void OnBitEnter(BoardBit other) { }

        public void Initialize()
        {
            if (initialized) return;

            initialized = true;
            OnInitialized();
        }

        public void ApplyMaterial(Material material)
        {
            Array.ForEach(imageRenderers, (_image) => _image.ApplyMaterial(material));
            //if (parentRectTransform)
            //    Array.ForEach(imageRenderers, (_image) => _image.ApplyMaterial(material));
            //else
            //    Array.ForEach(spriteRenderers, (_spriteRenderer) => _spriteRenderer.ApplyMaterial(material));
        }

        public virtual void _Destroy(float time = 0f)
        {
            if (time == 0f)
            {
                _Destroy();
            }
            else
            {
                StartRoutine("destroy", time, () => _Destroy());
            }
        }

        public virtual void _Destroy()
        {
            if (gameboard)
            {
                gameboard.RemoveBoardBit(this);
            }

            Destroy(gameObject);
        }

        public void AddInteraction(List<TokenView> tokens)
        {
            turnTokensInteractionList.AddRange(tokens);

            //foreach (TokenView token in tokens)
            //{
            //    switch (token.tokenType)
            //    {
            //        case TokenType.ARROW:
            //        case TokenType.ROTATING_ARROW:
            //        case TokenType.FOURWAY_ARROW:
            //            speedMltp += .1f;

            //            break;
            //    }
            //}
        }

        public virtual float _Destroy(DestroyType reason)
        {
            switch (reason)
            {
                case DestroyType.FALLING:
                    //play falling animation
                    RotateTo(360f, RepeatType.ZERO_TO_ONE, 1.2f);
                    ScaleFromCurrent(Vector3.zero, 1f);

                    _Destroy(1f);

                    return .85f;

                case DestroyType.BOMB:
                    StartRoutine("hideRoutine", .3f, () => Hide(.7f));
                    _Destroy(.7f);

                    return 0f;

                default:
                    _Destroy();

                    return 0f;
            }
        }

        public virtual float _Destroy(TransitionType transitionType)
        {
            switch (transitionType)
            {
                case TransitionType.SPELL_FADE:
                    ScaleFromCurrent(Vector3.zero, .85f);
                    Hide(.75f);
                    _Destroy(.75f);

                    return .85f;

                default:
                    Hide(.75f);
                    _Destroy(.75f);

                    return .75f;
            }
        }

        //public float WaitTimeForDistance(float distance) => (distance * gameboard.step.x) / speed;

        protected virtual void OnInitialized()
        {
            active = true;
            turnTokensInteractionList = new List<TokenView>();
            buffs = new Dictionary<BitBuffType, List<BitBuff>>();

            parentRectTransform = GetComponentInParent<RectTransform>();
            rectTransform = GetComponent<RectTransform>();
            gameboard = GetComponentInParent<GameboardView>();
            alphaTween = GetComponent<AlphaTween>();
            positionTween = GetComponent<PositionTween>();
            scaleTween = GetComponent<ScaleTween>();
            colorTween = GetComponent<ColorTween>();
            rotationTween = GetComponentInChildren<RotationTween>();
            sortingGroup = GetComponent<SortingGroup>();
            circleCollider = GetComponent<CircleCollider2D>();

            if (gameboard)
            {
                originalSpeed = gameboard.step.x * InternalSettings.Current.BASE_MOVE_SPEED;
                speedCap = gameboard.step.x * InternalSettings.Current.MOVE_SPEED_CAP;
            }

            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

            originalSortingOrder = sortingGroup.sortingOrder;

            //check if parent have rectTransform on it
            if (parentRectTransform)
            {
                imageRenderers = new SpriteToImage[spriteRenderers.Length];

                for (int rendererIndex = 0; rendererIndex < spriteRenderers.Length; rendererIndex++)
                {
                    imageRenderers[rendererIndex] = spriteRenderers[rendererIndex].gameObject.AddComponent<SpriteToImage>();
                }
            }

            //size it
            if (gameboard)
            {
                float xScale;

                if (parentRectTransform)
                {
                    RectTransform rectTransform = body.GetComponent<RectTransform>();

                    xScale = gameboard.step.x / rectTransform.rect.width * gameboard.transform.localScale.x;
                    transform.localScale = new Vector3(xScale, xScale,
                        1f)
                        * onBoardScale;
                }
                else
                {
                    SpriteRenderer spriteRenderer = body.GetComponent<SpriteRenderer>();
                    xScale = gameboard.step.x / spriteRenderer.bounds.size.x * gameboard.transform.localScale.x;

                    transform.localScale = new Vector3(xScale, xScale,
                        1f)
                        * onBoardScale;
                }
            }
            else
            {
                transform.localScale = Vector3.one;
            }

            //configure alpha tween
            alphaTween.propagate = true;
            alphaTween.DoParse();
        }

        protected virtual IEnumerator MoveRoutine(bool startTurn, BoardLocation from, BoardLocation to)
        {
            Vector2 start = gameboard.BoardLocationToVec2(from);
            Vector2 end = gameboard.BoardLocationToVec2(to);
            float distance = Vector2.Distance(start, end);

            for (float t = 0; t < 1f; t += Time.deltaTime * speed / distance)
            {
                transform.localPosition = Vector3.Lerp(start, end, t);
                yield return null;
            }

            transform.localPosition = end;
            gameboard.OnBoardLocationEnter(to, this);
        }

        public class BitBuff
        {
            public BitBuffType type;
            public float value;

            public BitBuff(BitBuffType type, float value)
            {
                this.type = type;
                this.value = value;
            }
        }

        public class SpeedBuff : BitBuff
        {
            public SpeedBuff() : base(BitBuffType.SPEED, .3f) { }
        }

        public enum BitBuffType
        {
            SPEED,
        }
    }
}
