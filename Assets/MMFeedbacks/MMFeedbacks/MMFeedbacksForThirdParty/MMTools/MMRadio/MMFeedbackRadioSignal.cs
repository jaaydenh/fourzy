using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will let you trigger a play on a target MMRadioSignal (usually used by a MMRadioBroadcaster to emit a value that can then be listened to by MMRadioReceivers. From this feedback you can also specify a duration, timescale and multiplier.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will let you trigger a play on a target MMRadioSignal (usually used by a MMRadioBroadcaster to emit a value that can then be listened to by MMRadioReceivers. From this feedback you can also specify a duration, timescale and multiplier.")]
    [FeedbackPath("GameObject/MMRadioSignal")]
    public class MMFeedbackRadioSignal : MMFeedback
    {
        /// the duration of this feedback is the duration of the light, or 0 if instant
        public override float FeedbackDuration { get { return 0f; } }

        public MMRadioSignal TargetSignal;
        public MMRadioSignal.TimeScales TimeScale = MMRadioSignal.TimeScales.Unscaled;
        /// the duration of the shake, in seconds
        public float Duration = 1f;
        /// a global multiplier to apply to the end result of the combination
        public float GlobalMultiplier = 1f;

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.GameObjectColor; } }
#endif
        

        /// <summary>
        /// On Play we set the values on our target signal and make it start shaking its level
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                if (TargetSignal != null)
                {
                    TargetSignal.Duration = Duration;
                    TargetSignal.GlobalMultiplier = GlobalMultiplier;
                    TargetSignal.TimeScale = TimeScale;
                    TargetSignal.StartShaking();
                }
            }
        }
    }
}
