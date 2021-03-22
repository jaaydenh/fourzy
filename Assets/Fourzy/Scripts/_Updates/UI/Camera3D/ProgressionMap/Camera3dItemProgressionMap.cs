//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using Fourzy._Updates.UI.Widgets;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.UI.Camera3D
{
    public class Camera3dItemProgressionMap : Camera3DItem
    {
        [InlineButton("ResetRewardID", "Reset ID")]
        public string mapID = Guid.NewGuid().ToString();
        public ProgressionEvent firstEvent;

        public Transform content;
        public Camera3dItemProgressionMapChunk chunk;
        public SpriteRenderer bg;
        public Transform linesParent;

        public UnlockRequirementsEnum unlockRequirements;
        [ShowIf("$showQuantity")]
        public int quantity = 0;

        public int currentMapChunkIndex { get; private set; }
        [HideInInspector]
        public float currentScrollValue;

        protected Vector2 cameraSize;

        private bool initialized = false;
        private bool finished = false;
        private float minScrollValue;
        private float maxScrollValue;
        private float chunkLeft;
        private float chunkRight;

        public List<ProgressionEvent> widgets { get; private set; }
        public MenuScreen menuScreen { get; private set; }

        public int totalGames
        {
            get
            {
                int result = 0;

                foreach (ProgressionEvent widget in widgets)
                {
                    if (widget.EventType == ProgressionEvent.ProgressionEventType.GAME)
                    {
                        result += widget.PuzzlePack.enabledPuzzlesData.Count;
                    }
                }

                return result;
            }
        }

        public int totalGamesComplete
        {
            get
            {
                int result = 0;

                foreach (ProgressionEvent widget in widgets)
                {
                    if (widget.EventType == ProgressionEvent.ProgressionEventType.GAME)
                    {
                        result += widget.PuzzlePack.puzzlesComplete.Count;
                    }
                }

                return result;
            }
        }

        public bool showQuantity => unlockRequirements != UnlockRequirementsEnum.NONE;
        public float contentPosition => -content.localPosition.x + chunkLeft;
        public bool scrollLocked { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying) return;

            Initialize();
        }

        protected void OnDestroy()
        {
            GameContentManager.Instance.existingProgressionMaps.Remove(this);
            GameManager.onSceneChanged -= OnSceneChanged;
        }

        public void CheckMapComplete()
        {
            if (finished) return;

            if (widgets.TrueForAll(widget => widget.wasRewarded))
            {
                finished = true;
                PlayerPrefsWrapper.SetAdventureComplete(mapID, true);

                MenuController
                    .GetMenu(Constants.MAIN_MENU_CANVAS_NAME)
                    .GetOrAddScreen<PromptScreen>()
                    .Prompt("Adventure map complete", null, null, "OK");
            }
        }

        public void Open()
        {
            Initialize();

            gameObject.SetActive(true);

            UpdateWidgets();

            CheckMapComplete();

            //set as opened
            PlayerPrefsWrapper.SetAdventureNew(mapID, false);

            //unlock scroll
            SetScrollLockedState(false);

            //focus on a current one
            ProgressionEvent current = GetCurrentEvent();
            switch (current.EventType)
            {
                case ProgressionEvent.ProgressionEventType.GAME:
                    if (!current.PuzzlePack.complete)
                    {
                        FocusOn(current);
                    }

                    break;
            }
        }

        public void UpdateWidgets() => widgets.ForEach(widget => widget._Update());

        public void SetMenuScreen(MenuScreen menuScreen)
        {
            this.menuScreen = menuScreen;

            widgets.ForEach(widget => widget.menuScreen = menuScreen);
        }

        public override void ConfigureCamera()
        {
            if (bg && bg.sprite != null)
            {
                cam.orthographic = true;
                cam.orthographicSize = bg.bounds.extents.y;
            }

            base.ConfigureCamera();

            cameraSize = new Vector2(cam.aspect * cam.orthographicSize, cam.orthographicSize);

            minScrollValue = cameraSize.x / chunk.size.x;
            maxScrollValue = 1f - minScrollValue;
            chunkLeft = chunk.size.x * .5f;
            chunkRight = -chunkLeft;
        }

        public void Scroll(float value, bool changeCurrentScrollValue = false)
        {
            if (scrollLocked) return;

            float clampedValue = Mathf.Clamp(value, minScrollValue, maxScrollValue);

            if (changeCurrentScrollValue)
            {
                currentScrollValue = clampedValue;
            }

            content.localPosition = new Vector3(Mathf.Lerp(chunkLeft, chunkRight, clampedValue), 0f, 0f);
        }

        public float ScrollCameraRelativeValue(float value)
        {
            Scroll(Mathf.Clamp01(currentScrollValue - (value * cameraSize.x / chunkLeft)), true);

            return currentScrollValue;
        }

        public void FocusOn(ProgressionEvent @event) => 
            Scroll(widgets[widgets.IndexOf(@event)].rectTransform.anchoredPosition.x / chunk.size.x, true);

        public void OnClick(Vector2 position)
        {
            //raycastin
            RaycastHit2D hit2d = Physics2D.Raycast((Vector2)transform.position + new Vector2(cameraSize.x * position.x * 2f, cameraSize.y * position.y * 2f), cam.transform.forward, cam.farClipPlane);
            if (hit2d)
            {
                ProgressionEvent progressionEvent = hit2d.transform.GetComponent<ProgressionEvent>();

                if (progressionEvent)
                {
                    progressionEvent.button.OnPointerClick(null);
                }
            }
        }

        public List<ProgressionEvent> Next(string currentID)
        {
            ProgressionEvent current = null;

            foreach (ProgressionEvent @event in widgets)
            {
                bool found = false;
                switch (@event.EventType)
                {
                    case ProgressionEvent.ProgressionEventType.GAME:
                        if (currentID == @event.PuzzlePack.packId)
                        {
                            current = @event;
                            found = true;
                        }

                        break;

                    case ProgressionEvent.ProgressionEventType.REWARD:
                        if (currentID == @event.otherRewardID)
                        {
                            current = @event;
                            found = true;
                        }

                        break;
                }

                if (found) break;
            }

            if (current)
            {
                return current.unlockWhenComplete;
            }
            else
            {
                return new List<ProgressionEvent>();
            }
        }

        public BasicPuzzlePack GetNextPack(string currentID)
        {
            List<ProgressionEvent> nextEvents = Next(currentID);

            foreach (ProgressionEvent @event in nextEvents)
            {
                if (@event.EventType == ProgressionEvent.ProgressionEventType.GAME)
                {
                    return @event.PuzzlePack;
                }
            }

            return null;
        }

        public void SetScrollLockedState(bool state) => scrollLocked = state;

        public void ResetPlayerPrefs()
        {
            finished = false;

            PlayerPrefsWrapper.SetAdventureNew(mapID, true);
            PlayerPrefsWrapper.SetAdventureUnlocked(mapID, false);
            PlayerPrefsWrapper.SetAdventureComplete(mapID, false);

            if (string.IsNullOrEmpty(gameObject.scene.name))
            {
                Array.ForEach(GetComponentsInChildren<ProgressionEvent>(), widget => widget.ResetEvent());
            }
            else
            {
                widgets.ForEach(widget => widget.ResetEvent());
                firstEvent.Unlock(false);
            }
        }

        public void CompleteAll(int amountToExclude = 0)
        {
            if (!SettingsManager.Get(SettingsManager.KEY_DEMO_MODE)) return;

            ProgressionEvent[] events = GetComponentsInChildren<ProgressionEvent>();
            for (int eventIndex = 0; eventIndex < events.Length - amountToExclude; eventIndex++)
            {
                switch (events[eventIndex].EventType)
                {
                    case ProgressionEvent.ProgressionEventType.GAME:

                        BasicPuzzlePack pack = 
                            GameContentManager.Instance.GetExternalPuzzlePack(events[eventIndex].packName);

                        PlayerPrefsWrapper.SetPuzzlePackUnlocked(pack.packId, true);
                        PlayerPrefsWrapper.SetPuzzlePackOpened(pack.packId, true);

                        pack.puzzlesData.ForEach(_data =>
                        {
                            PlayerPrefsWrapper.SetPuzzleChallengeComplete(_data.ID, true);
                            PlayerPrefsWrapper.SetGameRewarded(_data.ID, true);

                            foreach (RewardsManager.Reward reward in _data.rewards)
                            {
                                PlayerPrefsWrapper.SetRewardRewarded(_data.GetRewardID(reward), true);
                            }
                        });

                        break;
                }
            }

            if (string.IsNullOrEmpty(gameObject.scene.name) && gameObject.activeInHierarchy)
            {
                Open();
            }
        }

        public ProgressionEvent GetCurrentEvent()
        {
            List<ProgressionEvent> options = widgets
                .Where(@event => @event._unlocked && !@event._rewarded)
                .ToList();

            if (options.Count == 0)
            {
                return widgets[0];
            }

            return options.Find(@event => @event.EventType == ProgressionEvent.ProgressionEventType.GAME) ??
                options.Find(@event => @event.EventType == ProgressionEvent.ProgressionEventType.CURRENCY);
        }

        public Vector2 GetEventCameraRelativePosition(ProgressionEvent @event) => 
            new Vector2(
                (@event.rectTransform.anchoredPosition.x - (contentPosition - cameraSize.x)) / (cameraSize.x * 2f),
                @event.rectTransform.anchoredPosition.y / (cameraSize.y * 2f));

        public Vector2 GetCurrentEventSize() => GetCurrentEvent().size;

        private void Initialize()
        {
            if (initialized) return;

            widgets = new List<ProgressionEvent>(GetComponentsInChildren<ProgressionEvent>());
            widgets.ForEach(widget => widget.Initialize());

            initialized = true;
            firstEvent.Unlock(false);

            GameContentManager.Instance.existingProgressionMaps.Add(this);

            finished = PlayerPrefsWrapper.GetAdventureComplete(mapID);

            GameManager.onSceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged(string sceneName)
        {
            if (sceneName == Constants.GAMEPLAY_SCENE_NAME)
            {
                gameObject.SetActive(false);
            }
        }

        private void ResetRewardID()
        {
            mapID = Guid.NewGuid().ToString();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}