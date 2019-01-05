//@vady udod

using ByteSheep.Events;
using Fourzy._Updates.Audio;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tween;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Helpers
{
    public class ButtonExtended : Button
    {
        public AdvancedEvent events;
        public AudioTypes playOnClick = AudioTypes.BUTTON_CLICK;

        public List<LabelPair> labels = new List<LabelPair>();
        public List<BadgePair> badges = new List<BadgePair>();

        public Dictionary<string, LabelPair> labelsFastAccess { get; private set; }
        public Dictionary<string, BadgePair> badgesFastAccess { get; private set; }

        public ScaleTween scaleTween { get; set; }

        private bool initialized = false;

        protected override void Awake()
        {
            base.Awake();

            Initialize();
        }

        public void SetLabel(string value, string labelName = "name")
        {
            Initialize();

            if (!labelsFastAccess.ContainsKey(labelName))
                return;

            labelsFastAccess[labelName].label.text = value;
        }

        public BadgePair GetBadge(string badgeName = "badge")
        {
            Initialize();

            if (!badgesFastAccess.ContainsKey(badgeName))
                return null;

            return badgesFastAccess[badgeName];
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        private void OnClick()
        {
            events.Invoke();

            if (playOnClick != AudioTypes.NONE)
                AudioHolder.instance.PlaySelfSfxOneShotTracked(playOnClick);

            if (scaleTween)
                scaleTween.PlayForward(true);
        }

        private void Initialize()
        {
            if (!Application.isPlaying || initialized)
                return;

            //add our events to onClick events
            onClick.AddListener(OnClick);

            //get labels/badges
            labelsFastAccess = new Dictionary<string, LabelPair>();
            foreach (LabelPair pair in labels)
                if (!labelsFastAccess.ContainsKey(pair.labelName))
                    labelsFastAccess.Add(pair.labelName, pair);

            badgesFastAccess = new Dictionary<string, BadgePair>();
            foreach (BadgePair pair in badges)
                if (!badgesFastAccess.ContainsKey(pair.badgeName))
                    badgesFastAccess.Add(pair.badgeName, pair);

            //get tweens
            scaleTween = GetComponent<ScaleTween>();

            initialized = true;
        }

        [Serializable]
        public class LabelPair
        {
            public string labelName = "name";
            public TextMeshProUGUI label;
        }

        [Serializable]
        public class BadgePair
        {
            public string badgeName = "badge";
            public Badge badge;
        }
    }
}