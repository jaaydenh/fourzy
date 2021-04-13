using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will let you target (almost) any property, on any object in your scene. 
    /// It also works on scriptable objects. Drag an object, select a property, and setup your feedback " +
    /// to update that property over time
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will let you target (almost) any property, on any object in your scene. " +
        "It also works on scriptable objects. Drag an object, select a property, and setup your feedback " +
        "to update that property over time.")]
    [FeedbackPath("GameObject/Property")]
    public class MMFeedbackProperty : MMFeedback
    {
        /// the duration of this feedback is the duration of the target property, or 0 if instant
        public override float FeedbackDuration { get { return (Mode == Modes.Instant) ? 0f : Duration; } }
        /// sets the inspector color for this feedback
        #if UNITY_EDITOR
            public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.GameObjectColor; } }
        #endif
        
        /// the possible modes for this feedback
        public enum Modes { OverTime, Instant } 
        
        [Header("Target Property")]
        /// the receiver to write the level to
        public MMPropertyReceiver Target;

        [Header("Mode")]
        /// whether the feedback should affect the target property instantly or over a period of time
        public Modes Mode = Modes.OverTime;
        /// how long the target property should change over time
        [MMFEnumCondition("Mode", (int)Modes.OverTime)]
        public float Duration = 0.2f;
        /// whether or not that target property should be turned off on start
        public bool StartsOff = false;
        /// whether or not the values should be relative or not
        public bool RelativeValues = true;

        [Header("Level")]
        /// the curve to tween the intensity on
        [MMFEnumCondition("Mode", (int)Modes.OverTime)]
        public MMTweenType LevelCurve = new MMTweenType(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0)));
        /// the value to remap the intensity curve's 0 to
        [MMFEnumCondition("Mode", (int)Modes.OverTime)]
        public float RemapLevelZero = 0f;
        /// the value to remap the intensity curve's 1 to
        [MMFEnumCondition("Mode", (int)Modes.OverTime)]
        public float RemapLevelOne = 1f;
        /// the value to move the intensity to in instant mode
        [MMFEnumCondition("Mode", (int)Modes.Instant)]
        public float InstantLevel;

        protected float _initialIntensity;

        /// <summary>
        /// On init we turn the target property off if needed
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);

            Target.Initialization(this.gameObject);
            _initialIntensity = Target.Level; 
            
            if (Active)
            {
                if (StartsOff)
                {
                    Turn(false);
                }
            }

        }

        /// <summary>
        /// On Play we turn our target property on and start an over time coroutine if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                Turn(true);
                switch (Mode)
                {
                    case Modes.Instant:
                        Target.SetLevel(InstantLevel);
                        break;
                    case Modes.OverTime:
                        StartCoroutine(UpdateValueSequence());
                        break;
                }
            }
        }

        /// <summary>
        /// This coroutine will modify the values on the target property
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator UpdateValueSequence()
        {
            float journey = 0f;

            while (journey < Duration)
            {
                float remappedTime = MMFeedbacksHelpers.Remap(journey, 0f, Duration, 0f, 1f);

                SetValues(remappedTime);

                journey += FeedbackDeltaTime;
                yield return null;
            }
            SetValues(1f);
            if (StartsOff)
            {
                Turn(false);
            }
            yield return null;
        }

        /// <summary>
        /// Sets the various values on the target property on a specified time (between 0 and 1)
        /// </summary>
        /// <param name="time"></param>
        protected virtual void SetValues(float time)
        {
            float intensity = MMTween.Tween(time, 0f, 1f, RemapLevelZero, RemapLevelOne, LevelCurve);

            if (RelativeValues)
            {
                intensity += _initialIntensity;
            }

            Target.SetLevel(intensity);
        }

        /// <summary>
        /// Turns the target property object off on stop if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomStopFeedback(Vector3 position, float attenuation = 1)
        {
            base.CustomStopFeedback(position, attenuation);
            if (Active)
            {
                Turn(false);
            }
        }

        /// <summary>
        /// Turns the target object on or off
        /// </summary>
        /// <param name="status"></param>
        protected virtual void Turn(bool status)
        {
            if (Target.TargetComponent.gameObject != null)
            {
                Target.TargetComponent.gameObject.SetActive(status);
            }
        }
    }
}
