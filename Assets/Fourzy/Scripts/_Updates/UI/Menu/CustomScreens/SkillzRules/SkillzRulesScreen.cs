//@vadym udod

using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using System;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SkillzRulesScreen : PromptScreen
    {
        [SerializeField]
        private SwipeHandler swipeHandler;
        [SerializeField]
        private ButtonExtended nextButton;

        private SkillzRulesPage[] pages;
        private SkillzRulesDotButton[] dotButtons;
        private PositionTween positionTween;

        private float pageWidth;
        private int currentPage = -1;

        protected override void Awake()
        {
            base.Awake();

            pages = GetComponentsInChildren<SkillzRulesPage>();
            dotButtons = GetComponentsInChildren<SkillzRulesDotButton>();
            positionTween = GetComponentInChildren<PositionTween>();
            swipeHandler.onSwipe += OnSwipe;

            pageWidth = pages[0].GetComponent<RectTransform>().rect.width;
        }

        public SkillzRulesScreen _OpenScreen()
        {
            SetPage(0, false);
            Prompt();

            return this;
        }

        public void OpenNextViaButton()
        {
            if (currentPage == pages.Length - 1)
            {
                CloseSelf();
            }
            else
            {
                OpenNextPage();
            }
        }

        public void OpenNextPage()
        {
            SetPage((currentPage + 1) % pages.Length, true);
        }

        public void OpenPreviousPage()
        {
            SetPage(currentPage == 0 ? pages.Length - 1 : currentPage - 1, true);
        }

        public void SetPage(int page, bool animated)
        {
            if (animated)
            {
                positionTween.from = currentPage > -1 ? (Vector2.left * pageWidth * currentPage) : Vector2.zero;
                positionTween.to = Vector2.left * pageWidth * page;

                positionTween.PlayForward(true);
            }
            else
            {
                positionTween.SetPosition(Vector2.left * pageWidth * page);
            }

            if (currentPage > -1)
            {
                dotButtons[currentPage].SetSelected(false);
                pages[currentPage].OnPageClosed();
            }

            pages[page].OnPageOpened();

            dotButtons[page].SetSelected(true);
            currentPage = page;

            //update button
            nextButton.GetLabel().label.text = LocalizationManager.Value(currentPage == pages.Length - 1 ? "done" : "next");
        }

        private void OnSwipe(SwipeDirection direction)
        {
            switch (direction)
            {
                case SwipeDirection.LEFT:
                    OpenNextPage();

                    break;

                case SwipeDirection.RIGHT:
                    OpenPreviousPage();

                    break;
            }
        }
    }
}
