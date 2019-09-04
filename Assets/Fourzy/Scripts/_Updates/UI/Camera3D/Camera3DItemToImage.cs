//@vadym udod

using Fourzy._Updates.UI.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Camera3D
{
    /// <summary>
    /// Uses RawImage component to display texture rendered by specified Camera3DItem
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class Camera3DItemToImage : MonoBehaviour
    {
        [Tooltip("Prefab of Camera3DItem to display, will try to use an existing one unless <b>forceNew</b> is set")]
        public Camera3DItem prefabToDisplay;
        public bool autoSizeRenderTexture = false;
        [Tooltip("Will create a new instance of this prefab even if one already exists")]
        public bool forceNew = false;
        public Color onStartColor = Color.white;

        protected RawImage rawImage;
        protected Vector2 oldSize;
        protected RectTransform rectTransform;
        protected MenuScreen menuScreen;

        [HideInInspector]
        public Camera3DItem item;

        private bool initialized = false;

        protected virtual void Awake()
        {
            rawImage = GetComponent<RawImage>();
            rectTransform = GetComponent<RectTransform>();
            menuScreen = GetComponentInParent<MenuScreen>();

            if (!rawImage) rawImage = gameObject.AddComponent<RawImage>();

            rawImage.material = Camera3DManager.instance.defaultItemToIMageMaterial;

            if (initialized)  return;

            Initialize();
        }

        protected virtual void Start()
        {
            if (rawImage) rawImage.color = onStartColor;
        }

        protected virtual void Update()
        {
            if (rawImage && autoSizeRenderTexture)
            {
                if (oldSize != rawImage.rectTransform.rect.size)
                    item.SizeTexture(rawImage.rectTransform.rect.size);

                oldSize = rawImage.rectTransform.rect.size;
            }
        }

        protected void OnDestroy()
        {
            Camera3DManager.instance.RemoveFromItem(gameObject);
        }

        public virtual Camera3DItem Initialize()
        {
            initialized = true;

            if (prefabToDisplay)
            {
                if (!item) item = GetItem();

                Camera3DManager.instance.JoinToItem(gameObject, item);

                if (rawImage) rawImage.texture = item.renderTexture;

                return item;
            }

            return null;
        }

        public virtual Camera3DItem GetItem()
        {
            //if object is NOT in a scene
            if (string.IsNullOrEmpty(prefabToDisplay.gameObject.scene.name))
                return Camera3DManager.instance.GetItem(prefabToDisplay, forceNew);
            else
                return prefabToDisplay;
        }
    }
}