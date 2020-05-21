//@vadym udod

using Fourzy._Updates.UI.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Fourzy._Updates.Managers
{
    public class CustomInputManager : MonoBehaviour
    {
        public static int MAX_GAMEPAD_COUNT = 2;

        public static CustomInputManager Instance;

        public VirtualPointer pointerPrefab;
        public float gamepadMoveSpeed = 2f;

        private bool initialized = false;
        private Vector2 defaultPointerPosition;

        private List<VirtualPointer> pointers = new List<VirtualPointer>();
        private Dictionary<VirtualPointer, SelectableUI> hoverObjects = new Dictionary<VirtualPointer, SelectableUI>();

        public static int GamepadCount { get; private set; }

        private int _gamepadCount => InputSystem.devices.Where(_device => _device is Gamepad).Count();

        protected void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            StandaloneInputModuleExtended.GamepadMode = StandaloneInputModuleExtended.GamepadModeFilter.MULTIPLE;

            Initialize();

            InputSystem.onDeviceChange +=
            (device, change) =>
            {
                if (device is Gamepad)
                {
                    if (change == InputDeviceChange.Added) GamepadCount++;
                    else if (change == InputDeviceChange.Removed) GamepadCount--;

                    for (int index = 0; index < pointers.Count; index++)
                        pointers[index].SetState(GamepadCount > index);
                }
            };
        }

        protected void Update()
        {
            if (GamepadCount == 0) return;

            //update gampads inputs
            if (GamepadCount == 1)
            {
                Vector2 value = new Vector2(Input.GetAxis("JoystickAnyX"), Input.GetAxis("JoystickAnyY"));

                for (int gpIndex = 0; gpIndex < MAX_GAMEPAD_COUNT; gpIndex++)
                    if (pointers[gpIndex].Active && value.magnitude > 0f) pointers[gpIndex].Move(value * gamepadMoveSpeed * Time.deltaTime);
            }
            else if (GamepadCount > 1)
            {
                for (int gpIndex = 0; gpIndex < MAX_GAMEPAD_COUNT; gpIndex++)
                {
                    Vector2 value = new Vector2(Input.GetAxis("Joystick" + (gpIndex + 1) + "XAxis"), Input.GetAxis("Joystick" + (gpIndex + 1) + "YAxis"));
                    if (value.magnitude > 0f) pointers[gpIndex].Move(value * gamepadMoveSpeed * Time.deltaTime);
                }
            }

            foreach (VirtualPointer pointer in pointers)
            {
                if (!pointer.Active) continue;
                pointer.Process();

                if (pointer.HoverObject)
                {
                    if (hoverObjects.ContainsValue(pointer.HoverObject)) continue;

                    if (!hoverObjects[pointer])
                    {
                        hoverObjects[pointer] = pointer.HoverObject;
                        hoverObjects[pointer].OnEnter();
                    }
                    else if (hoverObjects[pointer] != pointer.HoverObject)
                    {
                        hoverObjects[pointer].OnExit();
                        hoverObjects[pointer] = pointer.HoverObject;
                        hoverObjects[pointer].OnEnter();
                    }
                }
                else if (hoverObjects[pointer])
                {
                    hoverObjects[pointer].OnExit();
                    hoverObjects[pointer] = null;
                }
            }
        }

        protected void Initialize()
        {
            if (initialized) return;

            defaultPointerPosition = new Vector2(Screen.width * .5f, Screen.height * .5f);

            //get prefabs
            for (int pointerIndex = 0; pointerIndex < MAX_GAMEPAD_COUNT; pointerIndex++)
            {
                VirtualPointer _pointer =
                    Instantiate(pointerPrefab, transform)
                    .Initialize(defaultPointerPosition, pointerIndex);

                hoverObjects.Add(_pointer, null);
                pointers.Add(_pointer);
            }

            GamepadCount = _gamepadCount;

            initialized = true;
        }
    }
}