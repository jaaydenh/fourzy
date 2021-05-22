//@vady udod

using ByteSheep.Events;
using Fourzy._Updates.Audio;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tween;
using Hellmade.Net;
using Sirenix.OdinInspector;
using StackableDecorator;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Helpers
{
    public class ButtonExtended : Button
    {
        public Action<PointerEventData> onTap;

        public static Material GREYSCALE_MATERIAL;

        [SerializeField]
        internal AdvancedEvent events;
        [SerializeField]
        internal AdvancedEvent onState;
        [SerializeField]
        internal AdvancedEvent offState;
        [SerializeField]
        public string clickSfx = "button_click";
        [SerializeField]
        private bool changeMaterialOnState = true;
        [SerializeField, HideInPlayMode]
        private bool networkAffected = false;
        [SerializeField]
        [StackableDecorator.ShowIf("$changeMaterialOnState")]
        private Material textMaterial;
        [SerializeField]
        private bool scaleOnClick = true;
        [List]
        [StackableDecorator.ShowIf("$changeMaterialOnState")]
        public ButtonGraphicsCollection buttonGraphics;

        public List<LabelPair> labels = new List<LabelPair>();
        public List<BadgePair> badges = new List<BadgePair>();

        public Dictionary<string, LabelPair> labelsFastAccess { get; private set; }
        public Dictionary<string, BadgePair> badgesFastAccess { get; private set; }

        public ScaleTween scaleTween { get; set; }
        public AlphaTween alphaTween { get; set; }
        public RectTransform rectTransform { get; private set; }
        private HashSet<AffectedGraphics> maskableGraphics { get; set; }

        private bool initialized = false;
        private CanvasGroup _canvasGroup;

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying) return;

            Initialize();
        }

        protected override void Start()
        {
            base.Start();

            if (!Application.isPlaying) return;

            if (interactable)
            {
                onState.Invoke();
            }

            if (networkAffected)
            {
                OnConnectionStatusChanged(GameManager.NetworkAccess);
                GameManager.onNetworkAccess += OnConnectionStatusChanged;
            }
        }

        protected override void OnDestroy()
        {
            if (networkAffected)
            {
                GameManager.onNetworkAccess -= OnConnectionStatusChanged;
            }
        }

        protected override void OnCanvasGroupChanged()
        {
            base.OnCanvasGroupChanged();

            _canvasGroup = GetComponentInParent<CanvasGroup>();
        }

        public LabelPair GetLabel(string labelName = "name")
        {
            Initialize();

            if (labelsFastAccess.ContainsKey(labelName))
            {
                return labelsFastAccess[labelName];
            }

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
            foreach (AffectedGraphics affected in maskableGraphics)
            {
                if (affected.isText)
                {
                    if (textMaterial)
                    {
                        (affected.graphics as TMP_Text).fontMaterial = material == null ?
                            affected.previous :
                            textMaterial;
                    }
                }
                else
                {
                    affected.graphics.material = material;
                }
            }
        }

        public void SetGreyscaleMaterial() => SetMaterial(GREYSCALE_MATERIAL);

        /// <summary>
        /// Grayed out or not
        /// </summary>
        /// <param name="state"></param>
        public void SetState(bool state)
        {
            interactable = state;

            if (changeMaterialOnState)
            {
                SetMaterial(state ? null : GREYSCALE_MATERIAL);
            }

            if (state)
            {
                onState.Invoke();
            }
            else
            {
                offState.Invoke();
            }
        }

        public void DoParse()
        {
            if (buttonGraphics == null) return;

            maskableGraphics = new HashSet<AffectedGraphics>();

            if (buttonGraphics.list.Count != 0)
            {
                foreach (ButtonGraphics obj in buttonGraphics.list)
                {
                    Parse(obj.target.transform, obj.propagate);
                }
            }
            else
            {
                Parse(transform, true);
            }
        }

        public void Parse(Transform obj, bool propagate)
        {
            MaskableGraphic maskable = obj.GetComponent<MaskableGraphic>();

            if (maskable)
            {
                AffectedGraphics _graphic = new AffectedGraphics()
                {
                    graphics = maskable,
                };
                _graphic.isText = maskable is TMP_Text; ;

                if (_graphic.isText)
                {
                    _graphic.previous = (_graphic.graphics as TMP_Text).fontMaterial;
                }
                else
                {
                    _graphic.previous = maskable.material;
                }

                maskableGraphics.Add(_graphic);
            }

            if (propagate)
            {
                for (int i = 0; i < obj.childCount; i++)
                {
                    Parse(obj.GetChild(i), propagate);
                }
            }
        }

        public virtual void Hide(float time)
        {
            if (time == 0f)
            {
                alphaTween.SetAlpha(0f);
            }
            else
            {
                alphaTween.playbackTime = time;
                alphaTween.PlayBackward(true);
            }
        }

        public virtual void Show(float time)
        {
            if (time == 0f)
            {
                alphaTween.SetAlpha(1f);
            }
            else
            {
                alphaTween.playbackTime = time;
                alphaTween.PlayForward(true);
            }
        }

        public virtual void ScaleTo(Vector3 to, float time)
        {
            ScaleTo(transform.localScale, to, time);
        }

        public virtual void ScaleTo(Vector3 from, Vector3 to, float time)
        {
            scaleTween.from = from;
            scaleTween.to = to;
            scaleTween.playbackTime = time;

            scaleTween.PlayForward(true);

        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData != null)
            {
                base.OnPointerClick(eventData);
            }

            if (!interactable || (_canvasGroup && !_canvasGroup.interactable)) return;

            events.Invoke();

            if (!string.IsNullOrEmpty(clickSfx))
            {
                AudioHolder.instance.PlaySelfSfxOneShotTracked(clickSfx);
            }

            if (scaleOnClick && scaleTween)
            {
                scaleTween.PlayForward(true);
            }

            onTap?.Invoke(eventData);
        }

        private void Initialize()
        {
            if (!Application.isPlaying || initialized) return;

            initialized = true;

            //get labels/badges
            labelsFastAccess = new Dictionary<string, LabelPair>();
            foreach (LabelPair pair in labels)
            {
                if (!labelsFastAccess.ContainsKey(pair.labelName))
                {
                    labelsFastAccess.Add(pair.labelName, pair);
                }
            }

            badgesFastAccess = new Dictionary<string, BadgePair>();
            foreach (BadgePair pair in badges)
            {
                if (!badgesFastAccess.ContainsKey(pair.badgeName))
                {
                    badgesFastAccess.Add(pair.badgeName, pair);
                }
            }

            //get tweens
            scaleTween = GetComponent<ScaleTween>();
            alphaTween = GetComponent<AlphaTween>();
            rectTransform = GetComponent<RectTransform>();

            DoParse();

            if (!GREYSCALE_MATERIAL)
            {
                GREYSCALE_MATERIAL = new Material(Shader.Find("Custom/GreyscaleUI"));
            }
        }

        private void OnConnectionStatusChanged(bool access)
        {
            GetBadge("network").badge.SetState(!access);
            SetState(access);
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

            [StackableDecorator.ShowIf("#Check")]
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

        private class AffectedGraphics
        {
            public MaskableGraphic graphics;
            public bool isText;
            public Material previous;
        }
    }
}