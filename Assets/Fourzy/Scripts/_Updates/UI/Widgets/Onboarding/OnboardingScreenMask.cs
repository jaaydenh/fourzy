//@vadym udod

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
        public GameObject maskPrefab;

        private AlphaTween alphaTween;
        private List<GameObject> masks = new List<GameObject>();
        private OnboardingScreen onboardingScreen;

        protected void Awake()
        {
            onboardingScreen = GetComponentInParent<OnboardingScreen>();
            alphaTween = GetComponent<AlphaTween>();
        }

        public void ShowMasks(Rect[] rects)
        {
            alphaTween.PlayForward(true);

            foreach (GameObject mask in masks) Destroy(mask);
            masks.Clear();

            Camera c = Camera.main;

            foreach (Rect area in rects)
            {
                for (int column = (int)area.x; column < (int)(area.x + area.width); column++)
                {
                    for (int row = (int)area.y; row < (int)(area.y + area.height); row++)
                    {
                        GameObject maskInstance = Instantiate(maskPrefab, transform);
                        maskInstance.SetActive(true);

                        RectTransform rectTransform = maskInstance.GetComponent<RectTransform>();
                        rectTransform.SetAsFirstSibling();
                        rectTransform.anchoredPosition = 
                            onboardingScreen.menuController.WorldToCanvasPoint((Vector3)GamePlayManager.instance.board.BoardLocationToVec2(new BoardLocation(row, column)) + 
                            GamePlayManager.instance.board.transform.position);

                        masks.Add(maskInstance);
                    }
                }
            }
        }

        public void HideMasks()
        {
            alphaTween.PlayBackward(true);
        }
    }
}
