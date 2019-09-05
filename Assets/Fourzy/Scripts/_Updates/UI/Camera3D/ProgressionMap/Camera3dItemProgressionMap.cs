﻿//@vadym udod

using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Camera3D
{
    public class Camera3dItemProgressionMap : Camera3DItem
    {
        public ProgressionEvent firstEvent;
        
        public Transform content;
        public Camera3dItemProgressionMapChunk chunk;
        public SpriteRenderer bg;

        public int currentMapChunkIndex { get; private set; }
        [HideInInspector]
        public float currentScrollValue;

        protected Vector2 cameraSize;
        protected List<ProgressionEvent> widgets;

        protected void Start()
        {
            if (!Application.isPlaying) return;

            firstEvent.Unlock(false);
        }

        public void UpdateWidgets() => SetWidgetsIfNull().ForEach(widget => widget._Update());

        public void SetMenuScreen(MenuScreen menuScreen) => SetWidgetsIfNull().ForEach(widget => widget.menuScreen = menuScreen);

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
            SetWidgetsIfNull().ForEach(widget => widget.ResetGraphics());
            firstEvent.Unlock(false);
        }

        private List<ProgressionEvent> SetWidgetsIfNull()
        {
            if (widgets == null) widgets = new List<ProgressionEvent>(GetComponentsInChildren<ProgressionEvent>());

            return widgets;
        }
    }
}