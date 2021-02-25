//@vadym udod

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Camera3D
{
    public class Camera3dItemToImageProgressionMap : Camera3DItemToImage
    {
        public static float TAP_TIME = .2f;
        public static float TAP_DISTANCE = .05f;

        protected CanvasScaler canvasScaler;
        public Camera3dItemProgressionMap _item { get; private set; }

        private Vector2 originalTouchPosition;
        private Vector2 lastTouchPosition;
        private Vector2 lastTouchDelta;
        private bool pointerDown = false;
        private float touchTime;

        protected override void Awake()
        {
            canvasScaler = GetComponentInParent<CanvasScaler>();

            base.Awake();
        }

        protected override void Update()
        {
            base.Update();

            if (pointerDown)
            {
                Vector2 _newTouchPosition = new Vector2((Input.mousePosition.x / Screen.width) - .5f, (Input.mousePosition.y / Screen.height) - .5f);

                lastTouchDelta = _newTouchPosition - lastTouchPosition;
                _item.ScrollCameraRelativeValue(lastTouchDelta.x);

                lastTouchPosition = _newTouchPosition;
            }
            else
            {
                if (lastTouchDelta.magnitude > .001f)
                {
                    float currentScrollValue = _item.ScrollCameraRelativeValue(lastTouchDelta.x);

                    if (currentScrollValue != 0f && currentScrollValue != 1f)
                        lastTouchDelta *= .93f;
                    else
                        lastTouchDelta = Vector2.zero;
                }
            }
        }

        public void LoadOther(Camera3dItemProgressionMap other)
        {
            if (_item && _item.mapID == other.mapID)
            {
                if (!_item.gameObject.activeInHierarchy) _item.gameObject.SetActive(true);

                return;
            }

            Camera3DManager.instance.RemoveFromItem(gameObject);
            prefabToDisplay = other;

            Initialize();
        }

        public void UpdateWidgets() => _item.Open();

        public override Camera3DItem Initialize()
        {
            item = _item = GetItem() as Camera3dItemProgressionMap;

            GameManager.Instance.currentMap = _item;
            _item.originalTextureWidth = (int)rectTransform.rect.width;
            _item.originalTextureHeight = (int)rectTransform.rect.height;
            _item.ConfigureCamera();

            _item.Scroll(0f, true);
            _item.SetMenuScreen(menuScreen);

            base.Initialize();

            return item;
        }

        public void OnPointerDown(BaseEventData baseEventData)
        {
            pointerDown = true;
            lastTouchPosition = originalTouchPosition = new Vector2((Input.mousePosition.x / Screen.width) - .5f, (Input.mousePosition.y / Screen.height) - .5f);
            touchTime = Time.time;
        }

        public void OnPointerUp(BaseEventData baseEventData)
        {
            pointerDown = false;

            if (Time.time - touchTime < TAP_TIME && Vector2.Distance(lastTouchPosition, originalTouchPosition) < TAP_DISTANCE)
            {
                _item.OnClick(lastTouchPosition);
                lastTouchDelta = Vector2.zero;
            }
        }
    }
}
