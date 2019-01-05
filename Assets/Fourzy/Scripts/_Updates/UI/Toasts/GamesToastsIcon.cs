//@vadym udod

using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Toasts
{
    public class GamesToastsIcon : MonoBehaviour
    {
        public Image icon;
        public Sprite successIcon;
        public Sprite failIcon;

        public void SetIcon(ToastIconStyle style)
        {
            switch (style)
            {
                case ToastIconStyle.NONE:
                    gameObject.SetActive(false);
                    break;
                case ToastIconStyle.SUCCESS:
                    gameObject.SetActive(true);

                    icon.sprite = successIcon;
                    break;
                case ToastIconStyle.FAIL:
                    gameObject.SetActive(true);

                    icon.sprite = failIcon;
                    break;
            }
        }

        public void SetIcon(Sprite sprite)
        {
            gameObject.SetActive(true);
            icon.sprite = sprite;
        }
    }
}