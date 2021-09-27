//@vadym udod

using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class SkillzProgressionScreenPointsEntry : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text nameLabel;
        [SerializeField]
        private TMP_Text valueLabel;

        public void SetValues(string name, int value)
        {
            nameLabel.text = name;
            valueLabel.text = value + "";
        }

        public SkillzProgressionScreenPointsEntry SetSize(float size)
        {
            nameLabel.fontSize = size;
            valueLabel.fontSize = size;

            return this;
        }

        public SkillzProgressionScreenPointsEntry SetColor(Color color)
        {
            nameLabel.color = color;
            valueLabel.color = color;

            return this;
        }
    }
}
