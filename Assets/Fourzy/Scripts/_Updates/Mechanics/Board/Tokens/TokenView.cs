//modded @vadym udod

using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.Mechanics.Board
{
    public class TokenView : MonoBehaviour
    {
        public Token tokenType;
        public bool justDisplaying;

        public SpriteRenderer spriteRenderer { get; private set; }
        public RectTransform parentRectTransform { get; private set; }
        public RectTransform rectTransform { get; private set; }
        public GameBoardView gameboard { get; private set; }
        public Image image { get; private set; }

        private Sprite previousSprite;

        protected void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            parentRectTransform = GetComponentInParent<RectTransform>();
            gameboard = GetComponentInParent<GameBoardView>();

            if (justDisplaying)
            {
                var components = this.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour component in components)
                {
                    System.Type type = component.GetType();
                    if (type != typeof(SpriteRenderer) && type != typeof(Transform))
                    {
                        component.enabled = false;
                    }
                }
            }
        }

        protected void Start()
        {
            //check if parent have rectTransform on it
            if (parentRectTransform)
            {
                image = gameObject.AddComponent<Image>();
                rectTransform = GetComponent<RectTransform>();

                //size it
                rectTransform.sizeDelta = gameboard.step;

                if (spriteRenderer)
                    spriteRenderer.enabled = false;
            }
        }

        protected void Update()
        {
            if (parentRectTransform && spriteRenderer)
            {
                if (spriteRenderer.sprite != image.sprite)
                    image.sprite = spriteRenderer.sprite;
            }
        }
    }
}

