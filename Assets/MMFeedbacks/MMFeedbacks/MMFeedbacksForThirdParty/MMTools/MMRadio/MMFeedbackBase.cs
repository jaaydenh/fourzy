using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    public class MMFeedbackBaseTarget
    {
        /// the receiver to write the level to
        public MMPropertyReceiver Target;
        /// the curve to tween the intensity on
        public MMTweenType LevelCurve = new MMTweenType(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0)));
        /// the value to remap the intensity curve's 0 to
        public float RemapLevelZero = 0f;
        /// the value to remap the intensity curve's 1 to
        public float RemapLevelOne = 1f;
        /// the value to move the intensity to in instant mode
        public float InstantLevel;
        /// the initial value for this level
        public float InitialLevel;
    }

    public abstract class MMFeedbackBase : MMFeedback
    {
        /// the possible modes for this feedback
        public enum Modes { OverTime, Instant, ShakerEvent } 
        
        [Header("Mode")]
        /// whether the feedback should affect the target property instantly or over a period of time
        public Modes Mode = Modes.OverTime;
        /// how long the target property should change over time
        [MMFEnumCondition("Mode", (int)Modes.OverTime, (int)Modes.ShakerEvent)]
        public float Duration = 0.2f;
        /// whether or not that target property should be turned off on start
        public bool StartsOff = false;
        /// whether or not the values should be relative or not
        public bool RelativeValues = true;
        
        /// the duration of this feedback is the duration of the target property, or 0 if instant
        public override float FeedbackDuration { get { return (Mode == Modes.Instant) ? 0f : Duration; } }

        protected List<MMFeedbackBaseTarget> _targets;

        /// <summary>
        /// On init we turn the target property off if needed
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);

            PrepareTargets();

            if (Active)
            {
                if (StartsOff)
                {
                    Turn(false);
                }
            }
        }

        /// <summary>
        /// Creates a new list, fills the targets, and initializes them
        /// </summary>
        protected virtual void PrepareTargets()
        {
            _targets = new List<MMFeedbackBaseTarget>();
            FillTargets();
            InitializeTargets();
        }

        /// <summary>
        /// On validate (if a value has changed in the inspector), we reinitialize what needs to be
        /// </summary>
        protected virtual void OnValidate()
        {
            PrepareTargets();
        }

        /// <summary>
        /// Fills our list of targets, meant to be extended
        /// </summary>
        protected abstract void FillTargets();

        /// <summary>
        /// Initializes each target in the list
        /// </summary>
        protected virtual void InitializeTargets()
        {
            if (_targets.Count == 0)
            {
                return;
            }

            foreach(MMFeedbackBaseTarget target in _targets)
            {
                target.Target.Initialization(this.gameObject);
                target.InitialLevel = target.Target.Level;
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
                        Instant();
                        break;
                    case Modes.OverTime:
                        StartCoroutine(UpdateValueSequence());
                        break;
                }
            }
        }

        /// <summary>
        /// Plays an instant feedback
        /// </summary>
        protected virtual void Instant()
        {
            if (_targets.Count == 0)
            {
                return;
            }

            foreach (MMFeedbackBaseTarget target in _targets)
            {
                target.Target.SetLevel(target.InstantLevel);
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
            if (_targets.Count == 0)
            {
                return;
            }
            
            foreach (MMFeedbackBaseTarget target in _targets)
            {
                float intensity = MMTween.Tween(time, 0f, 1f, target.RemapLevelZero, target.RemapLevelOne, target.LevelCurve);
                if (RelativeValues)
                {
                    intensity += target.InitialLevel;
                }
                target.Target.SetLevel(intensity);
            }
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
            if (_targets.Count == 0)
            {
                return;
            }
            foreach (MMFeedbackBaseTarget target in _targets)
            {
                if (target.Target.TargetComponent.gameObject != null)
                {
                    target.Target.TargetComponent.gameObject.SetActive(status);
                }
            }
        }
    }
}
