﻿//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using Fourzy._Updates.UI.Widgets;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
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
        public List<ProgressionEvent> widgets { get; private set; } 
        public MenuScreen menuScreen { get; private set; }

        private bool initialized = false;
        private bool finished = false;

        public bool showQuantity => unlockRequirements != UnlockRequirementsEnum.NONE;

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying) return;

            Initialize();
        }

        protected void OnDestroy()
        {
            GameContentManager.Instance.existingProgressionMaps.Remove(this);
            GameManager.onGameplaySceneLoaded -= OnGameSceneLoaded;
        }

        public void CheckMapComplete()
        {
            if (finished) return;

            if (widgets.TrueForAll(widget => widget.wasRewarded))
            {
                finished = true;

                //show map finished animation
                PlayerPrefsWrapper.SetAdventureComplete(mapID, true);

                menuScreen.menuController.GetScreen<PromptScreen>().Prompt("Adventure map complete", "none", null, "OK");
            }
        }

        public void UpdateWidgets()
        {
            Initialize();

            gameObject.SetActive(true);

            widgets.ForEach(widget => widget._Update());

            CheckMapComplete();

            //set as opened
            PlayerPrefsWrapper.SetAdventureNew(mapID, false);
        }

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

            //temp
            chunk.left = (chunk.size.x * .5f) - cameraSize.x;
            chunk.right = -chunk.left;
        }

        public void Scroll(float value, bool changeCurrentScrollValue = false)
        {
            if (changeCurrentScrollValue) currentScrollValue = Mathf.Clamp(value, 0f, 1f);

            content.localPosition = new Vector3(Mathf.Lerp(chunk.left, chunk.right, Mathf.Clamp01(value)), 0f, 0f);
        }

        public float ScrollCameraRelativeValue(float value)
        {
            Scroll(currentScrollValue = Mathf.Clamp01(currentScrollValue - (value * cameraSize.x / chunk.left)));

            return currentScrollValue;
        }

        public void OnClick(Vector2 position)
        {
            //raycastin
            RaycastHit2D hit2d = Physics2D.Raycast((Vector2)transform.position + new Vector2(cameraSize.x * position.x * 2f, cameraSize.y * position.y * 2f), cam.transform.forward, cam.farClipPlane);
            if (hit2d)
            {
                ProgressionEvent progressionEvent = hit2d.transform.GetComponent<ProgressionEvent>();

                if (progressionEvent) progressionEvent.OnTap();
            }
        }

        public void ResetPlayerPrefs()
        {
            PlayerPrefsWrapper.SetAdventureNew(mapID, true);
            PlayerPrefsWrapper.SetAdventureUnlocked(mapID, false);
            PlayerPrefsWrapper.SetAdventureComplete(mapID, false);

            if (string.IsNullOrEmpty(gameObject.scene.name))
                Array.ForEach(GetComponentsInChildren<ProgressionEvent>(), widget => widget.ResetGraphics());
            else
            {
                widgets.ForEach(widget => widget.ResetGraphics());
                firstEvent.Unlock(false);
            }
        }

        private void Initialize()
        {
            if (initialized) return;

            widgets = new List<ProgressionEvent>(GetComponentsInChildren<ProgressionEvent>());
            widgets.ForEach(widget => widget.Initialize());

            initialized = true;
            firstEvent.Unlock(false);

            GameContentManager.Instance.existingProgressionMaps.Add(this);

            finished = PlayerPrefsWrapper.GetAdventureComplete(mapID);

            GameManager.onGameplaySceneLoaded += OnGameSceneLoaded;
        }

        private void OnGameSceneLoaded()
        {
            gameObject.SetActive(false);
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