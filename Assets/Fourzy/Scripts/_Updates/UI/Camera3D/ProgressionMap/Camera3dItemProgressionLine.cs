//@vadym udod

namespace Fourzy._Updates.UI.ProgressionMap
{
    using Fourzy._Updates.Tween;
    using UnityEngine;

    [RequireComponent(typeof(ColorTween))]
    public class Camera3dItemProgressionLine : MonoBehaviour
    {
        public Color unlockedColor;
        public Color lockedColor;

        protected ColorTween colorTween;

        protected void Awake()
        {
            colorTween = GetComponent<ColorTween>();
        }

        public Camera3dItemProgressionLine SetColor(Color color)
        {
            colorTween.SetColor(color);

            return this;
        }

        public Camera3dItemProgressionLine SetColorUnlocked() => SetColor(unlockedColor);

        public Camera3dItemProgressionLine SetColorLocked() => SetColor(lockedColor);

        public void SetActive(bool state) => gameObject.SetActive(state);
    }
}