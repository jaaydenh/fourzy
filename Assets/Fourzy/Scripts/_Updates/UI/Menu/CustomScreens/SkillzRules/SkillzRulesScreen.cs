//@vadym udod

using Fourzy._Updates.Tween;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SkillzRulesScreen : PromptScreen
    {
        private SkillzRulesPage[] pages;
        private SkillzRulesDotButton[] dotButtons;
        private PositionTween positionTween;

        private float pageWidth;
        private int previousPage = -1;

        protected override void Awake()
        {
            base.Awake();

            pages = GetComponentsInChildren<SkillzRulesPage>();
            dotButtons = GetComponentsInChildren<SkillzRulesDotButton>();
            positionTween = GetComponentInChildren<PositionTween>();

            pageWidth = pages[0].GetComponent<RectTransform>().rect.width;
        }

        public SkillzRulesScreen _OpenScreen()
        {
            SetPage(0, false);
            Prompt();

            return this;
        }

        public void OpenNextPage()
        {
            SetPage((previousPage + 1) % pages.Length, true);
        }

        public void SetPage(int page, bool animated)
        {
            if (animated)
            {
                positionTween.from = positionTween._value;
                positionTween.to = Vector2.left * pageWidth * page;

                positionTween.PlayForward(true);
            }
            else
            {
                positionTween.SetPosition(Vector2.left * pageWidth * page);
            }

            if (previousPage > -1)
            {
                dotButtons[previousPage].SetSelected(false);
                pages[previousPage].OnPageClosed();
            }

            pages[page].OnPageOpened();

            dotButtons[page].SetSelected(true);
            previousPage = page;
        }
    }
}
