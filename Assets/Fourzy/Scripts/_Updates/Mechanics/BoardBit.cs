//@vadym udod

using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using FourzyGameModel.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Fourzy._Updates.Mechanics
{
    [RequireComponent(typeof(SortingGroup))]
    [RequireComponent(typeof(AlphaTween))]
    [RequireComponent(typeof(ScaleTween))]
    [RequireComponent(typeof(ColorTween))]
    [RequireComponent(typeof(RotationTween))]
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

        public SortingGroup sortingGroup { get; private set; }
        public AlphaTween alphaTween { get; private set; }
        public ScaleTween scaleTween { get; private set; }
        public ColorTween colorTween { get; private set; }
        public RotationTween rotationTween { get; private set; }
        public GameboardView gameboard { get; private set; }
        public List<TokenView> turnTokensInteractionList { get; private set; }
        public virtual Color outlineColor { get; }

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

        public virtual void Hide(float fadeTime)
        {
            alphaTween.playbackTime = fadeTime;

            alphaTween.PlayBackward(true);
        }

        public virtual void Show(float fadeTime)
        {
            alphaTween.playbackTime = fadeTime;

            alphaTween.PlayForward(true);
        }

        public virtual void ScaleToCurrent(Vector3 value, float time)
        {
            scaleTween.from = value;
            scaleTween.to = transform.localScale;
            scaleTween.playbackTime = time;

            scaleTween.PlayForward(true);
        }

        public virtual void ScaleFromCurrent(Vector3 value, float time)
        {
            scaleTween.from = transform.localScale;
            scaleTween.to = value;
            scaleTween.playbackTime = time;

            scaleTween.PlayForward(true);
        }

        public virtual void RotateTo(float value, RepeatType repeatType, float time)
        {
            rotationTween.from = rotationTween._value;
            rotationTween.to = Vector3.forward * value;
            rotationTween.playbackTime = time;
            rotationTween.repeat = repeatType;

            rotationTween.PlayForward(true);
        }

        public virtual void ShowOutline(bool repeat)
        {
            if (!outline)
            {
                if (parentRectTransform)
                    outline = body.AddComponent<UIOutline>();
                else
                    outline = body.AddComponent<SpriteRendererOutline>();
            }

            outline.outlineColor = outlineColor;
            outline.Animate(0f, 1f, 1f, repeat);
        }

        public virtual void HideOutline(bool hide)
        {
            if (outline)
            {
                if (hide)
                    outline.HideOutline();
                else
                    outline.Animate(outline.tween._value, 0f, .5f, false);
            }
        }

        public virtual void SetMaterial(Material material)
        {
            if (parentRectTransform)
                foreach (SpriteToImage imageRenderer in imageRenderers)
                    imageRenderer.image.material = material;
            else
                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                    spriteRenderer.material = material;
        }

        public virtual float ExecuteGameAction(params GameAction[] actions)
        {
            if (actions == null || actions.Length == 0)
                return 0f;

            switch (actions[0].Type)
            {
                case GameActionType.MOVE_PIECE:
                    float distance = 0f;
                    List<GameActionMove> actionsMove = new List<GameActionMove>();

                    foreach (GameAction action in actions)
                    {
                        GameActionMove _action = action as GameActionMove;

                        Vector2 start = gameboard.BoardLocationToVec2(_action.Start);
                        Vector2 end = gameboard.BoardLocationToVec2(_action.End);

                        distance += Vector2.Distance(start, end);
                        actionsMove.Add(_action);
                    }

                    StartCoroutine(MoveRoutine(actionsMove.ToArray()));

                    return distance / gameboard.bitSpeed;
            }

            return 0f;
        }

        public virtual void OnBeforeTurn()
        {
            turnTokensInteractionList.Clear();
        }

        public virtual void OnAfterTurn() { }

        public virtual void OnBeforeMoveAction(params GameActionMove[] actionsMoves) { }

        public virtual void OnAfterMove(params GameActionMove[] actionsMoves) { }

        public virtual void OnBitEnter(BoardBit other) { }

        public void Initialize()
        {
            if (initialized)
                return;

            initialized = true;
            OnInitialized();
        }

        protected virtual void OnInitialized()
        {
            active = true;
            turnTokensInteractionList = new List<TokenView>();

            parentRectTransform = GetComponentInParent<RectTransform>();
            gameboard = GetComponentInParent<GameboardView>();
            alphaTween = GetComponent<AlphaTween>();
            scaleTween = GetComponent<ScaleTween>();
            colorTween = GetComponent<ColorTween>();
            rotationTween = GetComponent<RotationTween>();
            sortingGroup = GetComponent<SortingGroup>();
            circleCollider = GetComponent<CircleCollider2D>();

            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

            originalSortingOrder = sortingGroup.sortingOrder;

            //check if parent have rectTransform on it
            if (parentRectTransform)
            {
                imageRenderers = new SpriteToImage[spriteRenderers.Length];

                for (int rendererIndex = 0; rendererIndex < spriteRenderers.Length; rendererIndex++)
                    imageRenderers[rendererIndex] = spriteRenderers[rendererIndex].gameObject.AddComponent<SpriteToImage>();
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
                transform.localScale = Vector3.one;

            //configure alpha tween
            alphaTween.propagate = true;
            alphaTween.DoParse();
        }

        public virtual void _Destroy()
        {
            if (gameboard)
                gameboard.RemoveBoardBit(this);

            Destroy(gameObject);
        }

        public virtual float _Destroy(DestroyType reason)
        {
            switch (reason)
            {
                case DestroyType.FALLING:
                    //play falling animation
                    RotateTo(360f, RepeatType.ZERO_TO_ONE, 1.2f);
                    ScaleFromCurrent(Vector3.zero, 1f);

                    StartRoutine("destroy", 1f, () => _Destroy());

                    return .85f;

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

                    StartRoutine("destroy", 1f, () => _Destroy());

                    return .85f;

                default:
                    _Destroy();
                    return 0f;
            }
        }

        protected virtual IEnumerator MoveRoutine(params GameActionMove[] actionsMoves)
        {
            if (actionsMoves == null || actionsMoves.Length == 0)
                yield break;

            OnBeforeMoveAction(actionsMoves);

            GameActionMove startMoveAction = actionsMoves[0] as GameActionMove;
            GameActionMove endMoveAction = actionsMoves[actionsMoves.Length - 1] as GameActionMove;

            Vector2 start = gameboard.BoardLocationToVec2(startMoveAction.Start);
            Vector2 end = gameboard.BoardLocationToVec2(endMoveAction.End);
            float distance = Vector2.Distance(start, end);
            int actionIndex = 0;
            GameActionMove closest = null;

            for (float t = 0; t < 1; t += Time.deltaTime * gameboard.bitSpeed / distance)
            {
                transform.localPosition = Vector3.Lerp(start, end, t);

                if (actionIndex < actionsMoves.Length &&
                    Vector2.Distance(gameboard.BoardLocationToVec2(actionsMoves[actionIndex].End), transform.localPosition) <= radius &&
                    (closest == null || closest != actionsMoves[actionIndex]))
                {
                    closest = actionsMoves[actionIndex];
                    actionIndex++;

                    gameboard.OnBoardLocationEnter(closest.End, this);
                }

                yield return null;
            }
            transform.localPosition = end;

            OnAfterMove(actionsMoves);

            yield return true;
        }
    }
}
