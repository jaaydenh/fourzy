//@vadym udod

using Fourzy._Updates.Tween;
using UnityEngine;

namespace Fourzy._Updates.Tools
{
    [ExecuteInEditMode]
    public abstract class OutlineBase : RoutinesBase
    {
        [Range(0f, 3f)]
        public float intensity = 1f;
        [Range(0f, 3f)]
        public float size = 1.15f;
        [Range(0f, .2f)]
        public float blueSize = .03f;
        public Color outlineColor = Color.black;

        public ValueTween tween { get; private set; }

        protected bool initialized = false;
        protected Material material;

        public void Animate(float from, float to, float time, bool repeat)
        {
            if (!tween)
            {
                float tan45 = Mathf.Tan(Mathf.Deg2Rad * 45);

                tween = gameObject.AddComponent<ValueTween>();
                tween.curve = new AnimationCurve(new Keyframe(0f, 0f, tan45, tan45), new Keyframe(1f, 1f, tan45, tan45));

                tween._onValue += (value) => { intensity = value; };
            }

            tween.from = from;
            tween.to = to;
            tween.playbackTime = time;
            tween.repeat = repeat ? RepeatType.PING_PONG : RepeatType.NONE;

            tween.PlayForward(true);
        }

        public void StopAnimation()
        {
            tween.repeat = RepeatType.NONE;
        }

        public void HideOutline()
        {
            intensity = 0f;

            if (tween)
                tween.StopTween(false);
        }
    }
}
