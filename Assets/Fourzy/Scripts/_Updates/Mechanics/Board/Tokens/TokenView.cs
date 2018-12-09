//modded @vadym udod

using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using UnityEngine;
using UnityEngine.Rendering;

namespace Fourzy._Updates.Mechanics.Board
{
    [RequireComponent(typeof(SortingGroup))]
    [RequireComponent(typeof(AlphaTween))]
    public class TokenView : MonoBehaviour
    {
        public Token tokenType;
        public bool justDisplaying;

        public RectTransform parentRectTransform { get; private set; }
        public GameBoardView gameboard { get; private set; }
        public AlphaTween alphaTween { get; private set; }
        public SortingGroup sortingGroup { get; private set; }

        private SpriteRenderer[] spriteRenderers;
        private SrpiteToImage[] spriteToImage;

        protected void Awake()
        {
            parentRectTransform = GetComponentInParent<RectTransform>();
            gameboard = GetComponentInParent<GameBoardView>();
            alphaTween = GetComponent<AlphaTween>();
            sortingGroup = GetComponent<SortingGroup>();

            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            if (justDisplaying)
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
            else
            {
                //check if parent have rectTransform on it
                if (parentRectTransform)
                {
                    spriteToImage = new SrpiteToImage[spriteRenderers.Length];

                    for (int rendererIndex = 0; rendererIndex < spriteRenderers.Length; rendererIndex++)
                    {
                        spriteToImage[rendererIndex] = spriteRenderers[rendererIndex].gameObject.AddComponent<SrpiteToImage>();

                        //size it
                        transform.localScale = gameboard.step * .9f;
                    }
                }
                else
                {
                    transform.localScale = Vector3.one;
                }

                //configure alpha tween
                alphaTween.DoParse();
            }
        }

        public void SetAlpha(float value)
        {
            alphaTween.SetAlpha(value);
        }

        public void TrySetsortingLayerID(int layerID)
        {
            if (!parentRectTransform)
                sortingGroup.sortingLayerID = layerID;
        }
    }
}

