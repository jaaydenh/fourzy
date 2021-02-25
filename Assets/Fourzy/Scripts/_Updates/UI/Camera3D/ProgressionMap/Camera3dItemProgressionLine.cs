//@vadym udod

namespace Fourzy._Updates.UI.ProgressionMap
{
    using BansheeGz.BGSpline.Curve;
    using Fourzy._Updates.Tween;
    using Fourzy._Updates.UI.Widgets;
    using UnityEngine;

    [RequireComponent(typeof(ColorTween)), ExecuteInEditMode]
    public class Camera3dItemProgressionLine : MonoBehaviour
    {
        public Color unlockedColor;
        public Color lockedColor;

        protected ColorTween colorTween;

        protected void Awake()
        {
            if (!Application.isPlaying) return;

            colorTween = GetComponent<ColorTween>();
        }

        protected void OnDestroy()
        {
            if (Application.isPlaying) return;

            ProgressionEvent @event = GetComponentInParent<ProgressionEvent>();

            if (!@event) return;

            @event.lines.Remove(this);
        }

        public Camera3dItemProgressionLine SetColor(Color color)
        {
            if (!colorTween)
            {
                colorTween = GetComponent<ColorTween>();
                colorTween.Initialize();
            }

            colorTween.SetColor(color);

            return this;
        }

        public Camera3dItemProgressionLine SetColorUnlocked() => SetColor(unlockedColor);

        public Camera3dItemProgressionLine SetColorLocked() => SetColor(lockedColor);

        public Camera3dItemProgressionLine Initialize(Transform target)
        {
            transform.localPosition = Vector3.zero;

            BGCurve curve = GetComponent<BGCurve>();
            curve.Clear();
            curve.AddPoint(new BGCurvePoint(curve, target.localPosition, BGCurvePoint.ControlTypeEnum.BezierSymmetrical, false));
            curve.AddPoint(new BGCurvePoint(curve, target.localPosition + Vector3.right * 2f, BGCurvePoint.ControlTypeEnum.BezierSymmetrical, false));

            return this;
        }
    }
}