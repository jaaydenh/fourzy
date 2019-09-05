//@vadym udod

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Helpers
{
    public class SpriteToImage : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer { get; private set; }
        public SortingGroup sortingGroup { get; private set; }
        public Image image { get; private set; }
        public RectTransform rectTransform { get; private set; }

        protected void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            sortingGroup = GetComponent<SortingGroup>();

            if (spriteRenderer)
            {
                spriteRenderer.color = Color.clear;

                image = gameObject.AddComponent<Image>();
                rectTransform = GetComponent<RectTransform>();
            }

            if (sortingGroup)
                sortingGroup.enabled = false;

            UpdateImage();
        }

        protected void Update()
        {
            UpdateImage();
        }

        public void UpdateImage()
        {
            if (!image || !spriteRenderer) return;

            if(spriteRenderer.sprite == null/* && image.color != Color.clear*/)
            {
                image.enabled = false;
            }
            else if (spriteRenderer.sprite != null && !image.enabled/* && image.color == Color.clear*/)
            {
                image.enabled = true;
                //image.color = Color.white;
            }

            if (image.sprite != spriteRenderer.sprite)
            {
                image.sprite = spriteRenderer.sprite;
                rectTransform.sizeDelta = new Vector2(image.sprite.rect.width / image.sprite.pixelsPerUnit, image.sprite.rect.height / image.sprite.pixelsPerUnit);
            }
        }

        public void ApplyMaterial(Material material)
        {
            if (image)
                image.material = material;
            else if (spriteRenderer)
                spriteRenderer.sharedMaterial = material;
        }
    }
}