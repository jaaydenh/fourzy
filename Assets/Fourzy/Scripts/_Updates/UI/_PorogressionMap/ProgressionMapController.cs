//@vadym udod

using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.ProgressionMap
{
    [ExecuteInEditMode]
    public class ProgressionMapController : RoutinesBase
    {
        public ProgressionEvent firstEvent;
        public Image bgImage;

        public RectTransform rectTransform { get; private set; }

        protected override void Awake()
        {
            if (!Application.isPlaying) return;

            base.Awake();

            rectTransform = GetComponent<RectTransform>();
        }

        protected void Start()
        {
            if (!Application.isPlaying) return;

            firstEvent.Unlock(false);

            AdjustBGImage();
        }

        protected void Update()
        {
            if (Application.isPlaying) return;

            AdjustBGImage();
        }

        public void AdjustBGImage()
        {
            if (!rectTransform) rectTransform = GetComponent<RectTransform>();
            if (!bgImage) return;

            //adjust bg image
            rectTransform.sizeDelta =
                new Vector2((float)bgImage.sprite.texture.width / bgImage.sprite.texture.height * bgImage.rectTransform.rect.height, 0f);
        }
    }
}