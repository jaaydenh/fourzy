using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will move the current "head" of an MMFeedbacks sequence back to another feedback above in the list.
    /// What feedback the head lands on depends on your settings : you can decide to have it loop at last pause, or at the last LoopStart feedback in the list (or both).
    /// Furthermore, you can decide to have it loop multiple times and cause a pause when met.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will move the current 'head' of an MMFeedbacks sequence back to another feedback above in the list. " +
        "What feedback the head lands on depends on your settings : you can decide to have it loop at last pause, " +
        "or at the last LoopStart feedback in the list (or both). Furthermore, you can decide to have it loop multiple times and cause a pause when met.")]
    [FeedbackPath("Loop/Looper")]
    public class MMFeedbackLooper : MMFeedbackPause
    {
        [Header("Loop conditions")]
        /// if this is true, this feedback, when met, will cause the MMFeedbacks to reposition its 'head' to the first pause found above it (going from this feedback to the top), or to the start if none is found
        public bool LoopAtLastPause = true;
        /// if this is true, this feedback, when met, will cause the MMFeedbacks to reposition its 'head' to the first LoopStart feedback found above it (going from this feedback to the top), or to the start if none is found
        public bool LoopAtLastLoopStart = true;

        [Header("Loop")]
        /// if this is true, the looper will loop forever
        public bool InfiniteLoop = false;
        /// how many times this loop should run
        public int NumberOfLoops = 2;
        /// the amount of loops left (updated at runtime)
        [MMFReadOnly]
        public int NumberOfLoopsLeft = 1;
        /// whether we are in an infinite loop at this time or not
        [MMFReadOnly]
        public bool InInfiniteLoop = false;


        /// sets the color of this feedback in the inspector
        #if UNITY_EDITOR
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.LooperColor; } }
        #endif
        public override bool LooperPause { get { return true; } }
        public override YieldInstruction Pause { get { return _waitForSeconds; } }

        /// the duration of this feedback is the duration of the pause
        public override float FeedbackDuration { get { return PauseDuration; } }

        /// <summary>
        /// On init we initialize our number of loops left
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
            InInfiniteLoop = InfiniteLoop;
            NumberOfLoopsLeft = NumberOfLoops;
        }

        /// <summary>
        /// On play we decrease our counter and play our pause
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                NumberOfLoopsLeft--;
                StartCoroutine(PlayPause());
            }
        }

        /// <summary>
        /// On custom stop, we exit our infinite loop
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomStopFeedback(Vector3 position, float attenuation = 1)
        {
            base.CustomStopFeedback(position, attenuation);
            InInfiniteLoop = false;
        }

        /// <summary>
        /// On reset we reset our amount of loops left
        /// </summary>
        protected override void CustomReset()
        {
            base.CustomReset();
            InInfiniteLoop = InfiniteLoop;
            NumberOfLoopsLeft = NumberOfLoops;
        }
    }
}
