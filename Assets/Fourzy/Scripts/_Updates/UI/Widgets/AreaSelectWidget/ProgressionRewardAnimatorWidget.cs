//@vadym udod

using Fourzy._Updates.Tween;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class ProgressionRewardAnimatorWidget : WidgetBase
    {
        [SerializeField]
        private ParticleSystem rewardParticles;
        [SerializeField]
        private TweenBase tween;

        public void Animate(float duration)
        {
            tween.repeat = RepeatType.PING_PONG;
            tween.PlayForward(true);
            rewardParticles.Play(true);

            StartRoutine("animation", duration, OnComplete);
        }

        private void OnComplete()
        {
            rewardParticles.Stop();

            tween.repeat = RepeatType.NONE;
            tween.PlayBackward(true);
        }
    }
}