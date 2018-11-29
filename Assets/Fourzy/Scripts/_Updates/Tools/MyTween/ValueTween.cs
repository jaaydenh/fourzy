//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Tween
{
    public class ValueTween : TweenBase
    {
        public float from;
        public float to;

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            if (onProgress != null)
                onProgress.Invoke(Mathf.Lerp(from, to, value));
        }
    }
}
