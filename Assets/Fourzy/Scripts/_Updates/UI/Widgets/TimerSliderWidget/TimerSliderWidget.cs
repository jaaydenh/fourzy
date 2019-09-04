//@vadym udod

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
                    if (timerValue / Constants.timerSections <= stages.list[stageIndex].threshold)
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

                smallTimerGraphics.fillAmount = smallTimerValue / Constants.circularTimerValue;
            }
        }

        public bool isEmpty => timerValue <= 0f;
        public bool active { get; private set; }
        public bool isPaused { get; private set; }

        protected void Update()
        {
            if (smallTimerValueTween.isPlaying) return;

            if (!active || isEmpty || isPaused) return;

            SmallTimerValue -= Time.deltaTime;

            if (smallTimerValue <= 0f)
            {
                AddTimerValue(-1f, timerValue > 1f);

                //play sfx
                AudioHolder.instance.PlaySelfSfxOneShotTracked(Serialized.AudioTypes.TIMER_BAR_LOST);
            }
        }

        public void AssignPlayer(Player player)
        {
            this.player = player;

            SetTimerValue(Constants.initialTimerValue);
            Show(.3f);
        }

        public void AddTimerValue(float value, bool resetSmallTimer, bool vfx = false)
        {
            timerValueTween.from = timerValue / Constants.timerSections;
            TimerValue = Mathf.Clamp(TimerValue + value, 0f, Constants.timerSections);
            timerValueTween.to = timerValue / Constants.timerSections;

            if (timerValue <= 0f) onValueEmpty?.Invoke(player);

            timerValueTween.PlayForward(true);

            if (resetSmallTimer) AddSmallTimerValue(Constants.circularTimerValue);

            if (vfx) ShowAddTimerVfx($"+<color=#00FF62>{value * Constants.circularTimerValue}sec!</color>", Vector2.zero, vfxDirection);
        }

        public void SetTimerValue(float value)
        {
            timerValueTween.from = timerValue / Constants.timerSections;
            timerValueTween.to = value / Constants.timerSections;
            timerValueTween.PlayForward(true);

            TimerValue = Mathf.Clamp(value, 0f, Constants.timerSections);

            if (timerValue <= 0f) onValueEmpty?.Invoke(player);

            AddSmallTimerValue(Constants.circularTimerValue);
        }

        public void AddSmallTimerValue(float value)
        {
            smallTimerValueTween.from = SmallTimerValue;

            smallTimerValueTween.to = Mathf.Clamp(SmallTimerValue + value, 0f, Constants.circularTimerValue);
            smallTimerValueTween.PlayForward(true);
        }

        public void ShowAddTimerVfx(string value, Vector2 offset, Vector2 direction)
        {
            VfxHolder.instance.GetVfx<AddTimerVfx>(VfxType.UI_VFX_ADD_TIMER).SetValue(vfxParent, offset, direction, value);
        }

        public void Pause()
        {
            isPaused = true;
        }

        public void Unpause()
        {
            isPaused = false;
        }

        public void Activate()
        {
            if (active) return;

            active = true;
        }

        public void Deactivate()
        {
            if (!active) return;

            active = false;
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
