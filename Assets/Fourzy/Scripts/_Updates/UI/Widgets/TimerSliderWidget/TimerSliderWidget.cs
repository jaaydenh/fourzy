﻿//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.Audio;
using Fourzy._Updates.Mechanics._Vfx;
using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
using StackableDecorator;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    /// <summary>
    /// Have nothing to do with sliders anymore, but just can't change it's name due to collab issues
    /// </summary>
    public class TimerSliderWidget : WidgetBase
    {
        public Action<Player> onValueEmpty;

        public Vector2 vfxDirection = Vector2.up;
        public Image timerGraphics;
        public Image smallTimerGraphics;
        public Transform vfxParent;

        [List]
        public TimerStagesCollection stages;

        private ValueTween timerValueTween;
        private ValueTween smallTimerValueTween;

        private float timerValue;
        private float smallTimerValue;

        public Player player { get; private set; }

        public float TimerValue
        {
            get => timerValue;

            set
            {
                timerValue = value;

                //check stage
                for (int stageIndex = 0; stageIndex < stages.list.Count; stageIndex++)
                {
                    if (timerValue / Constants.TIMER_SECTIONS <= stages.list[stageIndex].threshold)
                    {
                        stages.list[stageIndex].@event.Invoke();

                        break;
                    }
                }
            }
        }

        public float SmallTimerValue
        {
            get => smallTimerValue;

            set
            {
                smallTimerValue = value;

                smallTimerGraphics.fillAmount = smallTimerValue / InternalSettings.Current.CIRCULAR_TIMER_SECONDS;
            }
        }

        public float TotalTimeLeft
        {
            get
            {
                return Mathf.Clamp(
                    (timerValue - 1) * InternalSettings.Current.CIRCULAR_TIMER_SECONDS + smallTimerValue, 
                    0f, 
                    float.MaxValue);
            }

            set
            {
                float realSmallTimerValue = value % InternalSettings.Current.CIRCULAR_TIMER_SECONDS;

                if (value > 0f)
                {
                    if (realSmallTimerValue == 0f)
                    {
                        realSmallTimerValue = InternalSettings.Current.CIRCULAR_TIMER_SECONDS;
                    }
                }

                SetTimerValue(Mathf.Ceil(value / InternalSettings.Current.CIRCULAR_TIMER_SECONDS), realSmallTimerValue);
            }
        }

        public bool IsEmpty => timerValue <= 0f;
        public bool Active { get; private set; }
        public bool IsPaused { get; private set; }

        protected void Update()
        {
            if (smallTimerValueTween.isPlaying) return;

            if (!Active || IsEmpty || IsPaused) return;

            SmallTimerValue -= Time.deltaTime;

            if (smallTimerValue <= 0f)
            {
                AddTimerValue(-1f, timerValue > 1f);

                //play sfx
                AudioHolder.instance.PlaySelfSfxOneShotTracked("timer_bar_lost");
            }
        }

        public void AssignPlayer(Player player)
        {
            this.player = player;

            SetTimerValue(
                InternalSettings.Current.INITIAL_TIMER_SECTIONS, 
                InternalSettings.Current.CIRCULAR_TIMER_SECONDS);

            Show(.3f);
        }

        public void AddTimerValue(float value, bool resetSmallTimer, bool vfx = false)
        {
            timerValueTween.from = timerValue / Constants.TIMER_SECTIONS;
            TimerValue = Mathf.Clamp(TimerValue + value, 0f, Constants.TIMER_SECTIONS);
            timerValueTween.to = timerValue / Constants.TIMER_SECTIONS;

            if (timerValue <= 0f)
            {
                onValueEmpty?.Invoke(player);
            }

            timerValueTween.PlayForward(true);

            if (resetSmallTimer)
            {
                AddSmallTimerValue(InternalSettings.Current.CIRCULAR_TIMER_SECONDS);
            }

            if (vfx)
            {
                ShowAddTimerVfx(
                    $"+<color=#00FF62>{value * InternalSettings.Current.CIRCULAR_TIMER_SECONDS}sec!</color>", 
                    Vector2.zero, 
                    vfxDirection);
            }
        }

        public void SetTimerValue(float value, float smallTimerValue)
        {
            timerValueTween.from = timerValue / Constants.TIMER_SECTIONS;
            timerValueTween.to = value / Constants.TIMER_SECTIONS;
            timerValueTween.PlayForward(true);

            TimerValue = Mathf.Clamp(value, 0f, Constants.TIMER_SECTIONS);

            if (timerValue <= 0f)
            {
                onValueEmpty?.Invoke(player);
            }

            AddSmallTimerValue(smallTimerValue);
        }

        public void AddSmallTimerValue(float value)
        {
            smallTimerValueTween.from = SmallTimerValue;

            smallTimerValueTween.to = Mathf.Clamp(
                SmallTimerValue + value,
                0f, 
                InternalSettings.Current.CIRCULAR_TIMER_SECONDS);
            smallTimerValueTween.PlayForward(true);
        }

        public void ShowAddTimerVfx(string value, Vector2 offset, Vector2 direction)
        {
            VfxHolder.instance
                .GetVfx<AddTimerVfx>("UI_VFX_ADD_TIMER")
                .SetValue(vfxParent, offset, direction, value);
        }

        public void Pause(float time = -1f)
        {
            IsPaused = true;

            CancelRoutine("pause");
            if (time != -1f)
            {
                StartRoutine("pause", time, Unpause, null);
            }
        }

        public void Unpause()
        {
            CancelRoutine("pause");
            IsPaused = false;
        }

        public void Activate()
        {
            if (Active) return;

            Active = true;
        }

        public void Deactivate()
        {
            if (!Active) return;

            Active = false;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            timerValueTween = timerGraphics.GetComponent<ValueTween>();
            smallTimerValueTween = smallTimerGraphics.GetComponent<ValueTween>();
        }

        [Serializable]
        public class TimerStage
        {
            public float threshold;
            public AdvancedEvent @event;
        }

        [Serializable]
        public class TimerStagesCollection
        {
            public List<TimerStage> list;
        }
    }
}
