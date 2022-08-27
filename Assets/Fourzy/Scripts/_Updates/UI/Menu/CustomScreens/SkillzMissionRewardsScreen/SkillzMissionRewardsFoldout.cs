//@vadym udod

using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Widgets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SkillzMissionRewardsFoldout : WidgetBase
    {
        [SerializeField]
        private GridLayoutGroup contentParent;
        [SerializeField]
        private RectTransform contentContainer;
        [SerializeField]
        private SizeTween contentSizeTween;
        [SerializeField]
        private RotationTween arrowTween;
        [SerializeField]
        private TMP_Text label;

        private bool expanded;

        public void ToggleFoldout()
        {
            expanded = !expanded;

            if (expanded)
            {
                contentSizeTween.PlayForward(true);
                arrowTween.PlayForward(true);
            }
            else
            {
                contentSizeTween.PlayBackward(true);
                arrowTween.PlayBackward(true);
            }
        }

        public void AddObjects(RectTransform[] objects)
        {
            foreach (RectTransform rectTransform in objects)
            {
                rectTransform.SetParent(contentParent.transform);
            }
            Vector2 contentSize = contentParent.GetGridLayoutSize(objects.Length);
            if (expanded)
            {
                contentContainer.sizeDelta = new Vector2(contentContainer.sizeDelta.x, contentSize.y);
            }

            contentSizeTween.from = new Vector2(contentContainer.rect.width, 0f);
            contentSizeTween.to = new Vector2(contentContainer.rect.width, contentSize.y);

            gameObject.SetActive(objects.Length > 0);
        }

        public void SetLabelText(string text)
        {
            label.text = text;
        }
    }
}
