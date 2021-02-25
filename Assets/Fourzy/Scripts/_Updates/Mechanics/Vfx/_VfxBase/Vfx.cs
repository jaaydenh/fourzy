//@vadym udod

using ByteSheep.Events;
using Coffee.UIExtensions;
using Fourzy._Updates.Audio;
using Fourzy._Updates.Serialized;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.Mechanics._Vfx
{
    public class Vfx : MonoBehaviour
    {
        public VfxType type;
        public float duration = 1f;
        public float uiCopyScale = 100f;
        public AdvancedEvent onStart;

        [HideInInspector]
        public VfxHolder holder;
        [HideInInspector]
        public bool isActive = false;

        public float durationLeft { get; protected set; }
        public Vfx uiLayerCopy { get; private set; }
        public Vfx copyParent { get; private set; }
        public RectTransform parentRectTransform { get; private set; }
        public RectTransform rectTransform { get; private set; }

        protected virtual void Update()
        {
            if (isActive && durationLeft != 0f)
            {
                if (durationLeft - Time.deltaTime > 0f)
                    durationLeft -= Time.deltaTime;
                else
                    Disable();
            }
        }

        public void Initialize(VfxHolder holder)
        {
            this.holder = holder;

            gameObject.SetActive(false);
            rectTransform = GetComponent<RectTransform>();
        }

        public virtual Vfx StartVfx()
        {
            if (copyParent) transform.SetAsLastSibling();

            isActive = true;
            onStart.Invoke();

            //check if this is part of ui canvas
            parentRectTransform = GetComponentInParent<RectTransform>();

            if (parentRectTransform && !copyParent && !rectTransform)
            {
                if (!uiLayerCopy)
                {
                    //get a ui layer copy of this vfx
                    uiLayerCopy = Instantiate(this);
                    uiLayerCopy.copyParent = this;

                    gameObject.SetActive(false);

                    List<ParticleSystem> particleSystems = new List<ParticleSystem>();
                    //add UIParticle
                    uiLayerCopy.GetComponentsInChildren(true, particleSystems);

                    if (particleSystems.Count > 0)
                    {
                        foreach (var p in particleSystems.Where(x => !x.GetComponent<UIParticle>()))
                            p.gameObject.AddComponent<UIParticle>();

                        //set scale to root
                        particleSystems[0].GetComponent<UIParticle>().scale = uiCopyScale;
                    }
                }

                uiLayerCopy.transform.SetParent(transform.parent);
                uiLayerCopy.transform.localScale = Vector3.one;
                uiLayerCopy.transform.localPosition = transform.localPosition;

                uiLayerCopy.StartVfx();
            }
            else
            {
                gameObject.SetActive(true);
                durationLeft = duration;
            }

            return this;
        }

        public virtual Vfx StartVfx(Transform target, Vector3 offset, float rotation)
        {
            transform.SetParent(target);

            transform.localScale = Vector3.one;

            if (target)
            {
                Canvas canvas = target.GetComponentInParent<Canvas>();

                if (canvas)
                    transform.localPosition = offset * canvas.transform.lossyScale.x;
            }
            transform.localPosition = offset;

            transform.localEulerAngles = new Vector3(0f, 0f, rotation);

            return StartVfx();
        }

        public virtual Vfx StartVfx(Transform target, Vector3 offset, float rotation, float customDuration)
        {
            duration = customDuration;

            return StartVfx(target, offset, rotation);
        }

        public virtual void AddDuration(float time)
        {
            durationLeft += time;
        }

        public virtual void RefreshDuration(float time)
        {
            durationLeft = time;
        }

        public virtual void Disable()
        {
            isActive = false;
            gameObject.SetActive(false);

            durationLeft = 0f;

            //if this vfx is controlled by its parent vfx, unactivate parent too
            if (copyParent)
                copyParent.isActive = false;
        }

        public void PlaySfx(AudioTypes audio, float volume)
        {
            AudioHolder.instance.PlaySelfSfxOneShotTracked(audio, volume);
        }
    }

    public enum VfxType
    {
        VFX_STARS_TRAIL = 0,
        VFX_TAP_ANIMATION = 1,
        VFX_BOMB_EXPLOSION = 2,
        VFX_MOVE_NEGATIVE = 3,
        VFX_BOMB_EXPLOSION_LINE = 4,

        UI_VFX_ADD_TIMER = 30,
        UI_TOUCH_VFX = 31,
        UI_REWARD_ANIMATION_PARTICLE = 32,

        LENGTH,
    }
}