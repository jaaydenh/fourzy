//@vadym udod

using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class TopRowWidget : MonoBehaviour
    {
        public TMP_Text textField;
        public int maxValue = 99999;

        public void SetValue(string value)
        {
            int intValue = int.Parse(value);

            if (intValue > maxValue)
                textField.text = $"{maxValue / 1000},{maxValue % 1000}+";
            else
            {
                if (intValue > 1000)
                    textField.text = $"{intValue / 1000},{intValue % 1000}";
                else
                    textField.text = value;
            }
        }
    }
}
