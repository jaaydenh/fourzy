//@vadym udod

using TMPro;

namespace Fourzy._Updates.UI.Menu.Widgets
{
    public class CoinsWidgetSmall : WidgetBase
    {
        public TextMeshProUGUI label;
        public string format = "{0}";

        public void SetValue(int value)
        {
            label.text = string.Format(format, value);
        }
    }
}