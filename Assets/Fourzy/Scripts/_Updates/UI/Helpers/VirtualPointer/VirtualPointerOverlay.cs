//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.UI.Menu;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Fourzy._Updates.UI.Helpers
{
    public class VirtualPointerOverlay : MonoBehaviour
    {
        public FakeVirtualPointer pointer;

        public AdvancedEvent onJoystick1;
        public AdvancedEvent onJoystick2;
        public AdvancedEvent onAny;
        public AdvancedEvent onSpecific;

        private StandaloneInputModuleExtended.GamepadControlFilter currentFilterMode;

        protected void Start()
        {
            PointerInputModuleExtended.onVirtualPointerRelease += ReleasePointer;
            PointerInputModuleExtended.onPointerPositionChanged += OnPointerPositionChanged;

            PointerInputModuleExtended.onPointerDown += OnPointerPressed;
            PointerInputModuleExtended.onPointerUp += OnPointerReleased;

            StandaloneInputModuleExtended.onFilterChanged += OnGamepadFilter;
            StandaloneInputModuleExtended.onGamepadIDChanged += OnGamepadID;
        }

        private void OnPointerPressed()
        {
            pointer.Press();
        }

        private void OnPointerReleased()
        {
            pointer.Relase();
        }

        private void OnPointerPositionChanged(Vector2 position)
        {
            //move pointer
            pointer.SetPosition(StandaloneInputModuleExtended.instance.virtualPoionterPosition);
        }

        private void ReleasePointer()
        {
            pointer.Hide();
        }

        private void OnGamepadID(int gamepadID)
        {
            if (currentFilterMode == StandaloneInputModuleExtended.GamepadControlFilter.SPECIFIC_GAMEPAD)
            {
                switch (gamepadID)
                {
                    case 0:
                        onJoystick1.Invoke();

                        break;

                    case 1:
                        onJoystick2.Invoke();

                        break;
                }
            }
        }

        private void OnGamepadFilter(StandaloneInputModuleExtended.GamepadControlFilter filter)
        {
            currentFilterMode = filter;

            switch (filter)
            {
                case StandaloneInputModuleExtended.GamepadControlFilter.ANY_GAMEPAD:
                    onAny.Invoke();

                    break;

                case StandaloneInputModuleExtended.GamepadControlFilter.SPECIFIC_GAMEPAD:
                    onSpecific.Invoke();

                    break;
            }
        }
    }
}
