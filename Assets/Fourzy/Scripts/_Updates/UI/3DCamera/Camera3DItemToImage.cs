//@vadym udod

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

        private RawImage rawImage;
        private Vector2 oldSize;
        [HideInInspector]
        public Camera3DItem item;

        private bool initialized = false;

        protected void Awake()
        {
            rawImage = GetComponent<RawImage>();

            if (!rawImage)
                rawImage = gameObject.AddComponent<RawImage>();

            rawImage.material = Camera3DManager.instance.defaultItemToIMageMaterial;

            if (initialized)
                return;

            Initialize();
        }

        protected void Start()
        {
            if (rawImage)
                rawImage.color = onStartColor;
        }

        protected void Update()
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

        public Camera3DItem Initialize()
        {
            initialized = true;

            if (prefabToDisplay)
            {
                //if object is NOT in a scene
                if (string.IsNullOrEmpty(prefabToDisplay.gameObject.scene.name))
                    item = Camera3DManager.instance.GetItem(prefabToDisplay, forceNew);
                else
                    item = prefabToDisplay;

                Camera3DManager.instance.JoinToItem(gameObject, item);

                if (rawImage)
                    rawImage.texture = item.renderTexture;

                return item;
            }

            return null;
        }
    }
}