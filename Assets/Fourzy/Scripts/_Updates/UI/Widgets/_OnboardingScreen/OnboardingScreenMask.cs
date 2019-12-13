﻿//@vadym udod

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenMask : MonoBehaviour
    {
        public OnboardingScreenMaskObject maskPrefab;
        public Image bg;

        private AlphaTween alphaTween;
        private List<OnboardingScreenMaskObject> masks = new List<OnboardingScreenMaskObject>();

        private CanvasGroup canvasGroup;

        protected void Awake()
        {
            alphaTween = GetComponent<AlphaTween>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void ShowMasks(OnboardingTask task, bool clear = true)
        {
            Show();

            if (clear) Clear();

            bg.color = new Color(0f, 0f, 0f, .3f);

            foreach (Rect area in task.areas)
                for (int column = (int)area.x; column < (int)(area.x + area.width); column++)
                    for (int row = (int)area.y; row < (int)(area.y + area.height); row++)
                    {
                        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(GamePlayManager.instance.board.BoardLocationToVec2(
                                    new BoardLocation(row, column)) + (Vector2)GamePlayManager.instance.board.transform.position);

                        AddMaskObject().
                            Size(task.vector2value).
                            SetStyle((OnboardingScreenMaskObject.MaskStyle)task.intValue).
                            SetAnchors(viewportPoint);
                    }
        }

        public void ShowMask(Vector2 anchors, Vector2 size, OnboardingScreenMaskObject.MaskStyle maskStyle, bool showBG, bool clear = true)
        {
            Show();

            bg.color = new Color(0f, 0f, 0f, showBG ? .3f : 0f);

            if (clear) Clear();

            AddMaskObject()
                .Size(size)
                .SetStyle(maskStyle)
                .SetAnchors(anchors);
        }

        public void ShowMask(Vector2 anchors, RectTransform toCopy, Vector3 scale, bool showBG, bool clear = true)
        {
            Show();

            bg.color = new Color(0f, 0f, 0f, showBG ? .3f : 0f);

            if (clear) Clear();

            AddMaskObject()
                .SetStyle(toCopy, scale)
                .SetAnchors(anchors);
        }

        public void Show(float time = .3f)
        {
            //if (alphaTween._value != 0f) return;

            if (time == 0f)
            {
                alphaTween.SetAlpha(1f);
            }
            else
            {
                alphaTween.playbackTime = time;
                alphaTween.PlayForward(true);
            }

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void Hide(float time = .3f)
        {
            if (time == 0f)
            {
                alphaTween.SetAlpha(0f);
            }
            else
            {
                alphaTween.playbackTime = time;
                alphaTween.PlayBackward(true);
            }

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private void Clear()
        {
            foreach (OnboardingScreenMaskObject mask in masks)
            {
                mask.ClearFilters();
                Destroy(mask.gameObject);
            }

            masks.Clear();
        }

        private OnboardingScreenMaskObject AddMaskObject()
        {
            OnboardingScreenMaskObject maskInstance = Instantiate(maskPrefab, transform);
            maskInstance.SetActive(true);
            maskInstance.rectTransform.SetAsFirstSibling();

            masks.Add(maskInstance);

            return maskInstance;
        }
    }
}