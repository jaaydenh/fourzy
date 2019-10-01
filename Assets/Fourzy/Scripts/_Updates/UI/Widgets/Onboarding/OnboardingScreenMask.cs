//@vadym udod

using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Serialized;
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

        protected void Awake()
        {
            onboardingScreen = GetComponentInParent<OnboardingScreen>();
            alphaTween = GetComponent<AlphaTween>();
        }

        public void ShowMasks(OnboardingDataHolder.OnboardingTask_ShowMaskedArea task, bool clear = true)
        {
            if (task.customMask)
                ShowMask(task.pointAt, task.size, clear);
            else
                ShowMasks(task.areas, clear);
        }

        public void ShowMasks(Rect[] areas, bool clear = true)
        {
            if (alphaTween._value == 0f) alphaTween.PlayForward(true);

            if (clear) Clear();

            foreach (Rect area in areas)
                for (int column = (int)area.x; column < (int)(area.x + area.width); column++)
                    for (int row = (int)area.y; row < (int)(area.y + area.height); row++)
                        AddMaskObject().rectTransform.anchoredPosition =
                            onboardingScreen.menuController.WorldToCanvasPoint(
                                (Vector3)GamePlayManager.instance.board.BoardLocationToVec2(
                                    new BoardLocation(row, column)) + GamePlayManager.instance.board.transform.position);
        }

        public void ShowMask(Vector2 position, Vector2 size, bool clear = true)
        {
            if (alphaTween._value == 0f) alphaTween.PlayForward(true);

            if (clear) Clear();

            AddMaskObject()
                .Size(size)
                .SetAnchors(position);
        }

        public void HideMasks()
        {
            alphaTween.PlayBackward(true);
        }

        private void Clear()
        {
            foreach (OnboardingScreenMaskObject mask in masks) Destroy(mask.gameObject);
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
