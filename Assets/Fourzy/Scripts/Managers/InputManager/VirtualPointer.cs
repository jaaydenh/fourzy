//@vadym udod

using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

namespace Fourzy._Updates.Managers
{
    public class VirtualPointer : RoutinesBase
    {
        public Sprite[] sprites;
        public Image image;

        private AlphaTween alphaTween;
        private CanvasGroup canvasGroup;
        private float timeoutDelay = 2f;

        private int layerMask;
        private GameObject lastEnter;
        private GameObject lastClickTarget;

        private Camera _camera => Camera.main;

        public bool Active { get; private set; } = false;
        public PointerEventData PointerData { get; private set; }
        public Vector3 Position => PointerData.position;
        public int PointerID => PointerData.pointerId;
        public PointerEventData.FramePressState state { get; private set; }
        public SelectableUI HoverObject { get; private set; }

        protected override void Awake()
        {
            alphaTween = GetComponent<AlphaTween>();
            canvasGroup = GetComponent<CanvasGroup>();

            PointerData = new PointerEventData(EventSystem.current);

            layerMask = LayerMask.GetMask("UI");
        }

        public VirtualPointer Initialize(Vector2 defaultPos)
        {
            PointerData.position = defaultPos;

            return this;
        }

        public VirtualPointer SetState(bool state)
        {
            Active = state;

            if (Active) Show();
            else Hide();

            return this;
        }

        public VirtualPointer SetIndex(int index)
        {
            PointerData.pointerId = index;
            image.sprite = sprites[index];

            return this;
        }

        public VirtualPointer Show(float time = -1f)
        {
            if (alphaTween)
            {
                if (time > -1f) alphaTween.playbackTime = time;
                alphaTween.PlayForward(true);
            }
            else if (canvasGroup) canvasGroup.alpha = 1f;

            return this;
        }

        public VirtualPointer Hide(float time = -1f)
        {
            if (alphaTween)
            {
                if (time > -1f) alphaTween.playbackTime = time;
                alphaTween.PlayBackward(true);
            }
            else if (canvasGroup) canvasGroup.alpha = 0f;

            return this;
        }

        public VirtualPointer Move(Vector2 offset)
        {
            SetPosition(PointerData.position + offset);

            if (!Active) SetState(true);

            return this;
        }

        public VirtualPointer SetPosition(Vector3 position)
        {
            PointerData.position = new Vector3(Mathf.Clamp(position.x, 0f, Screen.width), Mathf.Clamp(position.y, 0f, Screen.height), position.z);
            transform.position = PointerData.position;

            return this;
        }

        public VirtualPointer Process()
        {
            List<RaycastResult> raycasts = new List<RaycastResult>();
            EventSystem.current.RaycastAll(PointerData, raycasts);
            PointerData.pointerCurrentRaycast = FindFirstRaycast(raycasts);
            raycasts.Clear();

            GameObject raycastTarget = PointerData.pointerCurrentRaycast.gameObject;

            if (raycastTarget)
            {
                Transform step = raycastTarget.transform;
                GameObject clickTarget = null;

                if (lastEnter != raycastTarget)
                    while (step.parent)
                    {
                        if (step.TryGetComponent(out IPointerClickHandler button))
                        {
                            clickTarget = step.gameObject;
                            break;
                        }
                        else
                            step = step.parent;
                    }
                else
                    clickTarget = lastClickTarget;

                lastEnter = raycastTarget;

                if (clickTarget)
                {
                    if (clickTarget != lastClickTarget)
                    {
                        //get UI selectable
                        SelectableUI _temp = clickTarget.GetComponent<SelectableUI>();
                        if (_temp) HoverObject = _temp;
                        //get 3D selectable
                        else
                        {
                            RaycastHit2D raycastHit2D = Physics2D.Raycast(PointerData.position, _camera.transform.TransformDirection(Vector3.forward), _camera.farClipPlane, layerMask);

                            if (raycastHit2D)
                            {
                                SelectableUI selectable = raycastHit2D.transform.GetComponent<SelectableUI>();

                                if (selectable) HoverObject = selectable;
                                else HoverObject = null;
                            }
                            else
                                HoverObject = null;
                        }

                        lastClickTarget = clickTarget;
                    }

                    if (((ButtonControl)Gamepad.all[PointerData.pointerId]["buttonSouth"]).wasPressedThisFrame)
                    {
                        state = PointerEventData.FramePressState.Pressed;

                        PointerData.eligibleForClick = true;
                        PointerData.dragging = false;
                        PointerData.pressPosition = PointerData.position;
                        PointerData.pointerPressRaycast = PointerData.pointerCurrentRaycast;
                        PointerData.clickCount = 1;
                        PointerData.clickTime = Time.time;

                        if (PointerData.pointerEnter != raycastTarget) PointerData.pointerEnter = raycastTarget;

                        PointerData.pointerPress = clickTarget;
                        PointerData.rawPointerPress = raycastTarget;

                        ExecuteEvents.ExecuteHierarchy(clickTarget, PointerData, ExecuteEvents.pointerDownHandler);
                        if (PointerData.eligibleForClick)
                            ExecuteEvents.Execute(PointerData.pointerPress, PointerData, ExecuteEvents.pointerClickHandler);

                        PointerData.eligibleForClick = false;
                        PointerData.clickCount = 0;
                        PointerData.pointerPress = null;
                        PointerData.rawPointerPress = null;
                    }
                    else if (((ButtonControl)Gamepad.all[PointerData.pointerId]["buttonSouth"]).wasReleasedThisFrame)
                    {
                        state = PointerEventData.FramePressState.Released;

                        if (lastClickTarget)
                        {
                            ExecuteEvents.ExecuteHierarchy(lastClickTarget, PointerData, ExecuteEvents.pointerUpHandler);
                            lastClickTarget = null;
                            lastEnter = null;
                        }
                    }
                }
                else
                {
                    lastClickTarget = null;
                    HoverObject = null;
                }
            }
            else
                HoverObject = null;

            return this;
        }

        private static RaycastResult FindFirstRaycast(List<RaycastResult> candidates)
        {
            for (int index = 0; index < candidates.Count; ++index)
            {
                if (!((UnityEngine.Object)candidates[index].gameObject == (UnityEngine.Object)null))
                    return candidates[index];
            }
            return new RaycastResult();
        }

        private IEnumerator SleepRoutine()
        {
            yield return new WaitForSeconds(timeoutDelay);

            Hide();
        }
    }
}
