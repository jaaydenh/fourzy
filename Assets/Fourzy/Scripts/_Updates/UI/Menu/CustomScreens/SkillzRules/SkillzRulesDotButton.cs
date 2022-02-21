//@vadym udod

using Fourzy._Updates.UI.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SkillzRulesDotButton : MonoBehaviour
    {
        [SerializeField]
        private Color selectedColor;

        private SkillzRulesScreen screen;
        private ButtonExtended button;
        private Image image;

        private Color originalColor;
        private int pageIndex;

        private void Awake()
        {
            button = GetComponent<ButtonExtended>();
            screen = GetComponentInParent<SkillzRulesScreen>();
            image = GetComponent<Image>();

            originalColor = image.color;
            pageIndex = transform.GetSiblingIndex();

            button.events.AddListener(Click);
        }

        public void SetSelected(bool state)
        {
            image.color = state ? selectedColor : originalColor;
        }

        private void Click()
        {
            screen.SetPage(pageIndex, true);
        }
    }
}
