//@vadym udod

using Coffee.UIExtensions;
using Fourzy._Updates._Tutorial;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Menu.Screens;
using FourzyGameModel.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenMask : MonoBehaviour
    {
        public OnboardingScreenMaskObject maskPrefab;

        private AlphaTween alphaTween;
        private List<OnboardingScreenMaskObject> masks = new List<OnboardingScreenMaskObject>();
        private OnboardingScreen onboardingScreen;

        private CanvasGroup canvasGroup;

        protected void Awake()
        {
            onboardingScreen = GetComponentInParent<OnboardingScreen>();
            alphaTween = GetComponent<AlphaTween>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void ShowMasks(OnboardingTask task, bool clear = true)
        {
            Show();

            if (clear) Clear();

            foreach (Rect area in task.areas)
                for (int column = (int)area.x; column < (int)(area.x + area.width); column++)
                    for (int row = (int)area.y; row < (int)(area.y + area.height); row++)
                        AddMaskObject().rectTransform.anchoredPosition =
                            onboardingScreen.menuController.WorldToCanvasPoint(
                                (Vector3)GamePlayManager.instance.board.BoardLocationToVec2(
                                    new BoardLocation(row, column)) + GamePlayManager.instance.board.transform.position);
        }

        public void ShowMasks(Vector2 anchors, Vector2 size, bool clear = true)
        {
            Show();

            if (clear) Clear();

            AddMaskObject()
                .Size(size)
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
                Destroy(mask.unmaskRaycast);
                Destroy(mask.gameObject);
            }

            masks.Clear();
        }

        private OnboardingScreenMaskObject AddMaskObject()
        {
            OnboardingScreenMaskObject maskInstance = Instantiate(maskPrefab, transform);
            maskInstance.SetActive(true);
            maskInstance.rectTransform.SetAsFirstSibling();

            UnmaskRaycastFilter filter = maskInstance.unmaskRaycast = gameObject.AddComponent<UnmaskRaycastFilter>();
            filter.targetUnmask = maskInstance.unmask;

            masks.Add(maskInstance);

            return maskInstance;
        }
    }
}
