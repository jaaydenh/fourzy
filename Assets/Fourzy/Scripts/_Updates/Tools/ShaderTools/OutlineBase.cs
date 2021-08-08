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

        public ValueTween intensityValueTween { get; private set; }
        public ValueTween sizeValueTween { get; private set; }

        protected bool initialized = false;
        protected Material material;

        public void Animate(float from, float to, float time, float size, bool repeat)
        {
            float tan45 = Mathf.Tan(Mathf.Deg2Rad * 45);
            if (!intensityValueTween)
            {
                intensityValueTween = gameObject.AddComponent<ValueTween>();
                intensityValueTween.curve = new AnimationCurve(new Keyframe(0f, 0f, tan45, tan45), new Keyframe(1f, 1f, tan45, tan45));

                intensityValueTween._onValue += (value) =>
                {
                    intensity = value; 
                };
            }

            if (!sizeValueTween)
            {
                sizeValueTween = gameObject.AddComponent<ValueTween>();
                sizeValueTween.curve = new AnimationCurve(new Keyframe(0f, 0f, tan45, tan45), new Keyframe(1f, 1f, tan45, tan45));

                sizeValueTween._onValue += (value) =>
                {
                    this.size = value;
                };

                sizeValueTween.from = 1.15f;
            }
            else
            {
                sizeValueTween.from = sizeValueTween.to;
            }

            sizeValueTween.to = size;
            sizeValueTween.playbackTime = time;
            sizeValueTween.repeat = repeat ? RepeatType.PING_PONG : RepeatType.NONE;

            sizeValueTween.PlayForward(true);

            intensityValueTween.from = from;
            intensityValueTween.to = to;
            intensityValueTween.playbackTime = time;
            intensityValueTween.repeat = repeat ? RepeatType.PING_PONG : RepeatType.NONE;

            intensityValueTween.PlayForward(true);
        }

        public void StopAnimation()
        {
            intensityValueTween.repeat = RepeatType.NONE;
            sizeValueTween.repeat = RepeatType.NONE;
        }

        public void HideOutline()
        {
            intensity = 0f;

            if (intensityValueTween)
            {
                intensityValueTween.StopTween(false);
            }

            if (sizeValueTween)
            {
                sizeValueTween.StopTween(false);
            }
        }
    }
}
