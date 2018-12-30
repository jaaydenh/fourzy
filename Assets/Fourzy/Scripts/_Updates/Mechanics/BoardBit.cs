//@vadym udod

using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using UnityEngine;
using UnityEngine.Rendering;

namespace Fourzy._Updates.Mechanics
{
    [RequireComponent(typeof(SortingGroup))]
    [RequireComponent(typeof(AlphaTween))]
    [RequireComponent(typeof(ScaleTween))]
    public abstract class BoardBit : RoutinesBase
    {
        public GameObject _body;
        public float onBoardScale = 1f;

        protected SpriteRenderer[] spriteRenderers;
        protected SpriteToImage[] spriteToImage;
        protected RectTransform parentRectTransform;
        protected AlphaTween alphaTween;
        protected ScaleTween scaleTween;
        protected OutlineBase outline;

        public GameBoardView gameboard { get; private set; }
        public SortingGroup sortingGroup { get; private set; }
        
        public Position position
        {
            get
            {
                if (!gameboard)
                    return Position.zero;

                return gameboard.Vec3ToPosition(transform.localPosition);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            parentRectTransform = GetComponentInParent<RectTransform>();
            gameboard = GetComponentInParent<GameBoardView>();
            alphaTween = GetComponent<AlphaTween>();
            scaleTween = GetComponent<ScaleTween>();
            sortingGroup = GetComponent<SortingGroup>();

            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

            if (!_body)
                _body = gameObject;

            //check if parent have rectTransform on it
            if (parentRectTransform)
            {
                spriteToImage = new SpriteToImage[spriteRenderers.Length];

                for (int rendererIndex = 0; rendererIndex < spriteRenderers.Length; rendererIndex++)
                    spriteToImage[rendererIndex] = spriteRenderers[rendererIndex].gameObject.AddComponent<SpriteToImage>();
            }

            //size it
            if (gameboard)
            {
                if (parentRectTransform)
                {
                    RectTransform rectTransform = _body.GetComponent<RectTransform>();

                    transform.localScale = new Vector3(gameboard.step.x / rectTransform.rect.width, gameboard.step.y / rectTransform.rect.height, 1f);
                }
                else
                {
                    SpriteRenderer spriteRenderer = _body.GetComponent<SpriteRenderer>();

                    transform.localScale = new Vector3(gameboard.step.x / spriteRenderer.bounds.size.x, gameboard.step.y / spriteRenderer.bounds.size.y, 1f) * onBoardScale;
                }
            }
            else
                transform.localScale = Vector3.one;

            //configure alpha tween
            alphaTween.propagate = true;
            alphaTween.DoParse();
        }
        
        public void DisabelComponents()
        {
            var components = GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour component in components)
            {
                System.Type type = component.GetType();
                if (type != typeof(SpriteRenderer) && type != typeof(Transform))
                {
                    component.enabled = false;
                }
            }
        }

        public void SetAlpha(float value)
        {
            alphaTween.SetAlpha(value);
        }

        public void Fade(float alpha, float fadeTime)
        {
            alphaTween.from = alphaTween._value;
            alphaTween.to = alpha;
            alphaTween.playbackTime = fadeTime;

            alphaTween.PlayForward(true);
        }

        public void Scale(Vector3 value, float time)
        {
            scaleTween.from = scaleTween._value;
            scaleTween.to = value;
            scaleTween.playbackTime = time;

            scaleTween.PlayForward(true);
        }

        public void SetSortingLayer(int layer)
        {
            sortingGroup.sortingOrder = layer;
        }

        public void ShowOutline(bool repeat)
        {
            if (!outline)
            {
                if (parentRectTransform)
                    outline = _body.AddComponent<UIOutline>();
                else
                    outline = _body.AddComponent<SpriteRendererOutline>();
            }

            outline.outlineColor = Color.green;
            outline.Animate(0f, 1f, 1f, repeat);
        }

        public void HideOutline(bool hide)
        {
            if (outline)
            {
                if (hide)
                    outline.HideOutline();
                else
                    outline.Animate(outline.tween._value, 0f, .5f, false);
            }
        }
    }
}
