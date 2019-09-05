//@vadym udod

using Fourzy._Updates.UI.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UnityEngine.EventSystems
{
    /// <summary>
    /// A BaseInputModule for pointer input.
    /// </summary>
    public abstract class PointerInputModuleExtended : BaseInputModule
    {
        public static Action onVirtualPointerEngage;
        public static Action onVirtualPointerRelease;
        public static Action onPointerDown;
        public static Action onPointerUp;
        public static Action<Vector2> onPointerPositionChanged;
        public static Action<KeyValuePair<string, float>> noInput;

        /// <summary>
        /// Id of the cached left mouse pointer event.
        /// </summary>
        public const int kMouseLeftId = -1;

        /// <summary>
        /// Id of the cached right mouse pointer event.
        /// </summary>
        public const int kMouseRightId = -2;

        /// <summary>
        /// Id of the cached middle mouse pointer event.
        /// </summary>
        public const int kMouseMiddleId = -3;

        /// <summary>
        /// Touch id for when simulating touches on a non touch device.
        /// </summary>
        public const int kFakeTouchesId = -4;

        public static Dictionary<string, float> noInputFilters = new Dictionary<string, float>();

        /// <summary>
        /// Is virtual pointer used?
        /// </summary>
        public bool useVirtualPointerData { get; protected set; } = false;

        /// <summary>
        /// State of virtual pointer
        /// </summary>
        public VirtualPointerState virtualPointerState { get; protected set; } = VirtualPointerState.NONE;
        public VirtualPointerState lastVirtualPointerState { get; protected set; } = VirtualPointerState.NONE;

        /// <summary>
        /// Virtual pointer position
        /// </summary>
        /// 
        [NonSerialized]
        public Vector2 virtualPoionterPosition;

        protected Vector2 previousMousePos { get; set; }

        protected SelectableUI lastGOPoinerEnter;
        protected Selectable3D last3DGOPoinerEnter;
        protected int layerMask;
        protected float noInputTimer;

        protected Dictionary<int, PointerEventData> m_PointerData = new Dictionary<int, PointerEventData>();
        protected Coroutine timerCoroutine;

        protected override void Awake()
        {
            base.Awake();

            layerMask = 1 << 8;
        }

        /// <summary>
        /// Search the cache for currently active pointers, return true if found.
        /// </summary>
        /// <param name="id">Touch ID</param>
        /// <param name="data">Found data</param>
        /// <param name="create">If not found should it be created</param>
        /// <returns>True if pointer is found.</returns>
        protected bool GetPointerData(int id, out PointerEventData data, bool create)
        {
            if (!m_PointerData.TryGetValue(id, out data) && create)
            {
                data = new PointerEventData(eventSystem)
                {
                    pointerId = id,
                };
                m_PointerData.Add(id, data);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove the PointerEventData from the cache.
        /// </summary>
        protected void RemovePointerData(PointerEventData data)
        {
            m_PointerData.Remove(data.pointerId);
        }

        /// <summary>
        /// Given a touch populate the PointerEventData and return if we are pressed or released.
        /// </summary>
        /// <param name="input">Touch being processed</param>
        /// <param name="pressed">Are we pressed this frame</param>
        /// <param name="released">Are we released this frame</param>
        /// <returns></returns>
        protected PointerEventData GetTouchPointerEventData(Touch input, out bool pressed, out bool released)
        {
            PointerEventData pointerData;
            var created = GetPointerData(input.fingerId, out pointerData, true);

            pointerData.Reset();

            pressed = created || (input.phase == TouchPhase.Began);
            released = (input.phase == TouchPhase.Canceled) || (input.phase == TouchPhase.Ended);

            if (created)
                pointerData.position = input.position;

            if (pressed)
                pointerData.delta = Vector2.zero;
            else
                pointerData.delta = input.position - pointerData.position;

            pointerData.position = input.position;

            pointerData.button = PointerEventData.InputButton.Left;

            if (input.phase == TouchPhase.Canceled)
            {
                pointerData.pointerCurrentRaycast = new RaycastResult();
            }
            else
            {
                eventSystem.RaycastAll(pointerData, m_RaycastResultCache);

                var raycast = FindFirstRaycast(m_RaycastResultCache);
                pointerData.pointerCurrentRaycast = raycast;
                m_RaycastResultCache.Clear();
            }
            return pointerData;
        }

        /// <summary>
        /// Copy one PointerEventData to another.
        /// </summary>
        protected void CopyFromTo(PointerEventData @from, PointerEventData @to)
        {
            @to.position = @from.position;
            @to.delta = @from.delta;
            @to.scrollDelta = @from.scrollDelta;
            @to.pointerCurrentRaycast = @from.pointerCurrentRaycast;
            @to.pointerEnter = @from.pointerEnter;
        }

        /// <summary>
        /// Given a mouse button return the current state for the frame.
        /// </summary>
        /// <param name="buttonId">Mouse button ID</param>
        protected PointerEventData.FramePressState StateForMouseButton(int buttonId)
        {
            bool pressed = false;
            bool released = false;

            if (useVirtualPointerData)
            {
                //only use virtual pointer for buttonId 0 (left mouse)
                if (buttonId == 0)
                {
                    switch (virtualPointerState)
                    {
                        case VirtualPointerState.PRESSED:
                            pressed = true;

                            virtualPointerState = VirtualPointerState.NONE;

                            break;

                        case VirtualPointerState.RELEASED:
                            released = true;

                            virtualPointerState = VirtualPointerState.NONE;

                            break;
                    }
                }
                else
                {
                    pressed = input.GetMouseButtonDown(buttonId);
                    released = input.GetMouseButtonUp(buttonId);
                }
            }
            else
            {
                pressed = input.GetMouseButtonDown(buttonId);
                released = input.GetMouseButtonUp(buttonId);
            }

            if (pressed && released)
                return PointerEventData.FramePressState.PressedAndReleased;
            if (pressed)
                return PointerEventData.FramePressState.Pressed;
            if (released)
                return PointerEventData.FramePressState.Released;

            return PointerEventData.FramePressState.NotChanged;
        }

        protected class ButtonState
        {
            private PointerEventData.InputButton m_Button = PointerEventData.InputButton.Left;

            public MouseButtonEventData eventData
            {
                get { return m_EventData; }
                set { m_EventData = value; }
            }

            public PointerEventData.InputButton button
            {
                get { return m_Button; }
                set { m_Button = value; }
            }

            private MouseButtonEventData m_EventData;
        }

        protected class MouseState
        {
            private List<ButtonState> m_TrackedButtons = new List<ButtonState>();

            public bool AnyPressesThisFrame()
            {
                for (int i = 0; i < m_TrackedButtons.Count; i++)
                {
                    if (m_TrackedButtons[i].eventData.PressedThisFrame())
                        return true;
                }
                return false;
            }

            public bool AnyReleasesThisFrame()
            {
                for (int i = 0; i < m_TrackedButtons.Count; i++)
                {
                    if (m_TrackedButtons[i].eventData.ReleasedThisFrame())
                        return true;
                }
                return false;
            }

            public ButtonState GetButtonState(PointerEventData.InputButton button)
            {
                ButtonState tracked = null;
                for (int i = 0; i < m_TrackedButtons.Count; i++)
                {
                    if (m_TrackedButtons[i].button == button)
                    {
                        tracked = m_TrackedButtons[i];
                        break;
                    }
                }

                if (tracked == null)
                {
                    tracked = new ButtonState { button = button, eventData = new MouseButtonEventData() };
                    m_TrackedButtons.Add(tracked);
                }
                return tracked;
            }

            public void SetButtonState(PointerEventData.InputButton button, PointerEventData.FramePressState stateForMouseButton, PointerEventData data)
            {
                var toModify = GetButtonState(button);
                toModify.eventData.buttonState = stateForMouseButton;
                toModify.eventData.buttonData = data;
            }
        }

        /// <summary>
        /// Information about a mouse button event.
        /// </summary>
        public class MouseButtonEventData
        {
            /// <summary>
            /// The state of the button this frame.
            /// </summary>
            public PointerEventData.FramePressState buttonState;

            /// <summary>
            /// Pointer data associated with the mouse event.
            /// </summary>
            public PointerEventData buttonData;

            /// <summary>
            /// Was the button pressed this frame?
            /// </summary>
            public bool PressedThisFrame()
            {
                return buttonState == PointerEventData.FramePressState.Pressed || buttonState == PointerEventData.FramePressState.PressedAndReleased;
            }

            /// <summary>
            /// Was the button released this frame?
            /// </summary>
            public bool ReleasedThisFrame()
            {
                return buttonState == PointerEventData.FramePressState.Released || buttonState == PointerEventData.FramePressState.PressedAndReleased;
            }
        }

        private readonly MouseState m_MouseState = new MouseState();

        /// <summary>
        /// Return the current MouseState. Using the default pointer.
        /// </summary>
        protected virtual MouseState GetMousePointerEventData()
        {
            return GetMousePointerEventData(0);
        }

        /// <summary>
        /// Return the current MouseState.
        /// </summary>
        protected virtual MouseState GetMousePointerEventData(int id)
        {
            // Populate the left button...
            PointerEventData leftData;
            var created = GetPointerData(kMouseLeftId, out leftData, true);

            leftData.Reset();

            if (created)
            {
                if (useVirtualPointerData)
                    leftData.position = virtualPoionterPosition;
                else
                {
                    leftData.position = input.mousePosition;
                    previousMousePos = input.mousePosition;
                }
            }

            if (useVirtualPointerData && input.mousePosition != previousMousePos)
            {
                useVirtualPointerData = false;
                onVirtualPointerRelease?.Invoke();

                virtualPointerState = VirtualPointerState.RELEASED;
            }

            previousMousePos = input.mousePosition;

            Vector2 pos;

            if (useVirtualPointerData)
                pos = virtualPoionterPosition;
            else
            {
                pos = input.mousePosition;
                virtualPoionterPosition = pos;
            }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                // We don't want to do ANY cursor-based interaction when the mouse is locked
                leftData.position = new Vector2(-1.0f, -1.0f);
                leftData.delta = Vector2.zero;
            }
            else
            {
                leftData.delta = pos - leftData.position;
                leftData.position = pos;
            }

            ///
            if (!lastGOPoinerEnter)
            {
                Camera c = Camera.main;
                Vector3 screenToWorld = c.ScreenToWorldPoint(leftData.position);
                Vector3 rayPositionAdjusted = new Vector3(screenToWorld.x, screenToWorld.y, c.transform.position.z);

                RaycastHit2D raycastHit2D = Physics2D.Raycast(c.ScreenToWorldPoint(leftData.position), c.transform.TransformDirection(Vector3.forward), c.farClipPlane, layerMask);

                if (raycastHit2D)
                {
                    Selectable3D selectable = raycastHit2D.transform.GetComponent<Selectable3D>();

                    if (selectable)
                    {
                        if (!last3DGOPoinerEnter || selectable != last3DGOPoinerEnter)
                        {
                            if (last3DGOPoinerEnter && last3DGOPoinerEnter.enabled) last3DGOPoinerEnter.OnExit();

                            last3DGOPoinerEnter = selectable;
                            if (last3DGOPoinerEnter.enabled) last3DGOPoinerEnter.OnEnter();
                        }
                    }
                }
                else if (last3DGOPoinerEnter)
                {
                    if (last3DGOPoinerEnter.enabled) last3DGOPoinerEnter.OnExit();
                    last3DGOPoinerEnter = null;
                }
            }
            ///

            leftData.scrollDelta = input.mouseScrollDelta;
            leftData.button = PointerEventData.InputButton.Left;
            eventSystem.RaycastAll(leftData, m_RaycastResultCache);
            var raycast = FindFirstRaycast(m_RaycastResultCache);
            leftData.pointerCurrentRaycast = raycast;
            m_RaycastResultCache.Clear();

            // copy the apropriate data into right and middle slots
            PointerEventData rightData;
            GetPointerData(kMouseRightId, out rightData, true);
            CopyFromTo(leftData, rightData);
            rightData.button = PointerEventData.InputButton.Right;

            PointerEventData middleData;
            GetPointerData(kMouseMiddleId, out middleData, true);
            CopyFromTo(leftData, middleData);
            middleData.button = PointerEventData.InputButton.Middle;

            m_MouseState.SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(0), leftData);
            m_MouseState.SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(1), rightData);
            m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(2), middleData);

            return m_MouseState;
        }

        /// <summary>
        /// Return the last PointerEventData for the given touch / mouse id.
        /// </summary>
        protected PointerEventData GetLastPointerEventData(int id)
        {
            PointerEventData data;
            GetPointerData(id, out data, false);
            return data;
        }

        private static bool ShouldStartDrag(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold)
        {
            if (!useDragThreshold)
                return true;

            return (pressPos - currentPos).sqrMagnitude >= threshold * threshold;
        }

        /// <summary>
        /// Process movement for the current frame with the given pointer event.
        /// </summary>
        protected virtual void ProcessMove(PointerEventData pointerEvent)
        {
            var targetGO = (Cursor.lockState == CursorLockMode.Locked ? null : pointerEvent.pointerCurrentRaycast.gameObject);
            HandlePointerExitAndEnter(pointerEvent, targetGO);
        }

        /// <summary>
        /// Process the drag for the current frame with the given pointer event.
        /// </summary>
        protected virtual void ProcessDrag(PointerEventData pointerEvent)
        {
            if (!pointerEvent.IsPointerMoving() ||
                Cursor.lockState == CursorLockMode.Locked ||
                pointerEvent.pointerDrag == null)
                return;

            if (!pointerEvent.dragging
                && ShouldStartDrag(pointerEvent.pressPosition, pointerEvent.position, eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold))
            {
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
                pointerEvent.dragging = true;
            }

            // Drag notification
            if (pointerEvent.dragging)
            {
                // Before doing drag we should cancel any pointer down state
                // And clear selection!
                if (pointerEvent.pointerPress != pointerEvent.pointerDrag)
                {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                    pointerEvent.eligibleForClick = false;
                    pointerEvent.pointerPress = null;
                    pointerEvent.rawPointerPress = null;
                }
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
            }
        }

        public override bool IsPointerOverGameObject(int pointerId)
        {
            var lastPointer = GetLastPointerEventData(pointerId);
            if (lastPointer != null)
                return lastPointer.pointerEnter != null;
            return false;
        }

        public virtual void UpdateVirtualPointerPosition(Vector2 delta)
        {
            if (!useVirtualPointerData)
            {
                useVirtualPointerData = true;
                onVirtualPointerEngage?.Invoke();
            }

            virtualPoionterPosition.Set(Mathf.Clamp(virtualPoionterPosition.x + delta.x, 0f, Screen.width), Mathf.Clamp(virtualPoionterPosition.y + delta.y, 0f, Screen.height));

            onPointerPositionChanged?.Invoke(virtualPoionterPosition);
        }

        public virtual void SetVirtualPointerState(VirtualPointerState state)
        {
            if (!useVirtualPointerData)
            {
                useVirtualPointerData = true;
                onVirtualPointerEngage?.Invoke();
            }

            virtualPointerState = state;
            lastVirtualPointerState = state;

            switch (state)
            {
                case VirtualPointerState.PRESSED:
                    onPointerDown?.Invoke();

                    break;

                case VirtualPointerState.RELEASED:
                    onPointerUp?.Invoke();

                    break;
            }
        }

        public void AddNoInputFilter(string key, float time)
        {
            if (noInputFilters.ContainsKey(key)) return;

            noInputFilters.Add(key, time);

            ResetNoInputTimer();
        }

        public void ResetNoInputTimer()
        {
            noInputTimer = 0f;
        }

        public void TriggerNoInputEvent(string key)
        {
            if (!noInputFilters.ContainsKey(key)) return;

            noInput.Invoke(new KeyValuePair<string, float>(key, noInputFilters[key]));
        }

        // walk up the tree till a common root between the last entered and the current entered is foung
        // send exit events up to (but not inluding) the common root. Then send enter events up to
        // (but not including the common root).
        protected new void HandlePointerExitAndEnter(PointerEventData currentPointerData, GameObject newEnterTarget)
        {
            //lastGOPoinerEnter = null;
            // if we have no target / pointerEnter has been deleted
            // just send exit events to anything we are tracking
            // then exit
            if (newEnterTarget == null || currentPointerData.pointerEnter == null)
            {
                for (var i = 0; i < currentPointerData.hovered.Count; ++i)
                    ExecuteEvents.Execute(currentPointerData.hovered[i], currentPointerData, ExecuteEvents.pointerExitHandler);

                currentPointerData.hovered.Clear();

                if (lastGOPoinerEnter)
                {
                    lastGOPoinerEnter.OnExit();
                    lastGOPoinerEnter = null;
                }

                if (newEnterTarget == null)
                {
                    currentPointerData.pointerEnter = null;
                    return;
                }
            }

            // if we have not changed hover target
            if (currentPointerData.pointerEnter == newEnterTarget && newEnterTarget)
                return;

            GameObject commonRoot = FindCommonRoot(currentPointerData.pointerEnter, newEnterTarget);

            // and we already an entered object from last time
            if (currentPointerData.pointerEnter != null)
            {
                // send exit handler call to all elements in the chain
                // until we reach the new target, or null!
                Transform t = currentPointerData.pointerEnter.transform;

                while (t != null)
                {
                    // if we reach the common root break out!
                    if (commonRoot != null && commonRoot.transform == t)
                        break;

                    if (lastGOPoinerEnter && lastGOPoinerEnter.transform == t)
                    {
                        lastGOPoinerEnter.OnExit();
                        lastGOPoinerEnter = null;
                    }

                    ExecuteEvents.Execute(t.gameObject, currentPointerData, ExecuteEvents.pointerExitHandler);
                    currentPointerData.hovered.Remove(t.gameObject);
                    t = t.parent;
                }
            }

            // now issue the enter call up to but not including the common root
            currentPointerData.pointerEnter = newEnterTarget;

            if (newEnterTarget != null)
            {
                Transform t = newEnterTarget.transform;

                while (t != null && t.gameObject != commonRoot)
                {
                    if (!lastGOPoinerEnter)
                    {
                        SelectableUI selectableUI = t.GetComponent<SelectableUI>();

                        if (selectableUI && selectableUI.interactable)
                        {
                            lastGOPoinerEnter = selectableUI;
                            lastGOPoinerEnter.OnEnter();

                            //cancel last3DGOPOinterEnter
                            if (last3DGOPoinerEnter)
                            {
                                if (last3DGOPoinerEnter.enabled) last3DGOPoinerEnter.OnExit();
                                last3DGOPoinerEnter = null;
                            }
                        }
                    }

                    ExecuteEvents.Execute(t.gameObject, currentPointerData, ExecuteEvents.pointerEnterHandler);
                    currentPointerData.hovered.Add(t.gameObject);
                    t = t.parent;
                }
            }
        }

        /// <summary>
        /// Clear all pointers and deselect any selected objects in the EventSystem.
        /// </summary>
        protected void ClearSelection()
        {
            var baseEventData = GetBaseEventData();

            foreach (var pointer in m_PointerData.Values)
            {
                // clear all selection
                HandlePointerExitAndEnter(pointer, null);
            }

            m_PointerData.Clear();
            eventSystem.SetSelectedGameObject(null, baseEventData);
        }

        protected IEnumerator TimerRoutine()
        {
            while (true)
            {
                foreach (var noInputFilter in noInputFilters)
                    if (noInputTimer <= noInputFilter.Value &&
                        (noInputTimer + Time.unscaledDeltaTime) > noInputFilter.Value)
                        noInput.Invoke(noInputFilter);

                noInputTimer += Time.unscaledDeltaTime;

                yield return null;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder("<b>Pointer Input Module of type: </b>" + GetType());
            sb.AppendLine();
            foreach (var pointer in m_PointerData)
            {
                if (pointer.Value == null)
                    continue;
                sb.AppendLine("<B>Pointer:</b> " + pointer.Key);
                sb.AppendLine(pointer.Value.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Deselect the current selected GameObject if the currently pointed-at GameObject is different.
        /// </summary>
        /// <param name="currentOverGo">The GameObject the pointer is currently over.</param>
        /// <param name="pointerEvent">Current event data.</param>
        protected void DeselectIfSelectionChanged(GameObject currentOverGo, BaseEventData pointerEvent)
        {
            // Selection tracking
            var selectHandlerGO = ExecuteEvents.GetEventHandler<ISelectHandler>(currentOverGo);
            // if we have clicked something new, deselect the old thing
            // leave 'selection handling' up to the press event though.
            if (selectHandlerGO != eventSystem.currentSelectedGameObject)
                eventSystem.SetSelectedGameObject(null, pointerEvent);
        }

        public enum VirtualPointerState
        {
            NONE,
            PRESSED,
            RELEASED,
        }
    }
}
