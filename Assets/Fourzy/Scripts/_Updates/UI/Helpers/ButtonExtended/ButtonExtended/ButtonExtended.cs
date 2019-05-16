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
using StackableDecorator;

namespace Fourzy._Updates.UI.Helpers
{
    public class ButtonExtended : Button
    {
        public static Material GREYSCALE_MATERIAL;

        public AdvancedEvent events;
        public AdvancedEvent onState;
        public AdvancedEvent offState;
        public AudioTypes playOnClick = AudioTypes.BUTTON_CLICK;
        public bool changeMaterialOnState = true;
        [List]
        [ShowIf("$changeMaterialOnState")]
        public ButtonGraphicsCollection buttonGraphics;

        public List<LabelPair> labels = new List<LabelPair>();
        public List<BadgePair> badges = new List<BadgePair>();

        public Dictionary<string, LabelPair> labelsFastAccess { get; private set; }
        public Dictionary<string, BadgePair> badgesFastAccess { get; private set; }

        public ScaleTween scaleTween { get; set; }
        public List<MaskableGraphic> maskableGraphics { get; private set; }

        private bool initialized = false;

        protected override void Awake()
        {
            base.Awake();

            Initialize();
        }

        protected override void Start()
        {
            base.Start();

            if (interactable) onState.Invoke();
        }

        public LabelPair GetLabel(string labelName = "name")
        {
            Initialize();

            if (labelsFastAccess.ContainsKey(labelName))
                return labelsFastAccess[labelName];

            return null;
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

        public void HideAllBadges() => badges.ForEach(_badge => _badge.badge.Hide());

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        public void SetMaterial(Material material)
        {
            foreach (MaskableGraphic maskable in maskableGraphics)
                maskable.material = material;
        }

        public void SetState(bool state)
        {
            interactable = state;

            if (changeMaterialOnState) SetMaterial(state ? null : GREYSCALE_MATERIAL);

            if (state)
                onState.Invoke();
            else
                offState.Invoke();
        }

        public void DoParse()
        {
            if (buttonGraphics == null) return;

            maskableGraphics = new List<MaskableGraphic>();

            if (buttonGraphics.list.Count != 0)
                foreach (ButtonGraphics obj in buttonGraphics.list)
                    Parse(obj.target.transform, obj.propagate);
            else
                Parse(transform, true);
        }

        public void Parse(Transform obj, bool propagate)
        {
            MaskableGraphic maskable = obj.GetComponent<MaskableGraphic>();

            if (!maskable) return;

            maskableGraphics.Add(maskable);

            if (propagate)
                for (int i = 0; i < obj.childCount; i++)
                    Parse(obj.GetChild(i), propagate);
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

            initialized = true;

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

            DoParse();

            if (!GREYSCALE_MATERIAL) GREYSCALE_MATERIAL = new Material(Shader.Find("Custom/GreyscaleUI"));
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

        [Serializable]
        public class ButtonGraphicsCollection
        {
            public List<ButtonGraphics> list;
        }

        [Serializable]
        public class ButtonGraphics
        {
            [HideInInspector]
            public string _name;

            [ShowIf("#Check")]
            [StackableField]
            public GameObject target;
            public bool propagate;

            public bool Check()
            {
                if (!target) return true;

                _name = target.name;

                return true;
            }
        }
    }
}