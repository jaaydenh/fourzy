//@vadym udod

using Fourzy._Updates.UI.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        public static int GamepadsCount { get; private set; }

        private int _gamepadsCount => Input.GetJoystickNames().Where(_name => !string.IsNullOrEmpty(_name)).Count();

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
        }

        protected void Update()
        {
            //check gamepads connected
            int count = _gamepadsCount;
            if (count != GamepadsCount)
            {
                GamepadsCount = count;

                for (int index = 0; index < pointers.Count; index++)
                    pointers[index].SetState(GamepadsCount > index);
            }

            if (GamepadsCount == 0) return;

            //update gampads inputs
            if (GamepadsCount == 1)
            {
                Vector2 value = new Vector2(Input.GetAxis("JoystickAnyX"), Input.GetAxis("JoystickAnyY"));
                print(value);
                for (int gpIndex = 0; gpIndex < MAX_GAMEPAD_COUNT; gpIndex++)
                    if (pointers[gpIndex].Active && value.magnitude > 0f) pointers[gpIndex].Move(value * gamepadMoveSpeed * Time.deltaTime);
            }
            else if (GamepadsCount > 1)
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

            GamepadsCount = _gamepadsCount;

            initialized = true;
        }
    }
}