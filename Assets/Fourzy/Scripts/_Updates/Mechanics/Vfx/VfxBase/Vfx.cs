//@vadym udod

using ByteSheep.Events;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Vfx
{
    public class Vfx : MonoBehaviour
    {
        public VfxType type;
        public float duration = 1f;
        public AdvancedEvent onStart;

        [HideInInspector]
        public VfxHolder holder;
        [HideInInspector]
        public bool isActive = false;

        public float durationLeft { get; protected set; }

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
        }

        public virtual void StartVfx()
        {
            isActive = true;
            gameObject.SetActive(true);
            onStart.Invoke();

            durationLeft = duration;
        }

        public virtual void StartVfx(Transform target, Vector3 offset, float rotation)
        {
            transform.SetParent(target);

            transform.localScale = Vector3.one;
            transform.position = target.position + offset;
            transform.localEulerAngles = new Vector3(0f, 0f, rotation);

            StartVfx();
        }

        public virtual void StartVfx(Transform target, Vector3 offset, float rotation, float customDuration)
        {
            duration = customDuration;

            StartVfx(target, offset, rotation);
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
        }
    }

    public enum VfxType
    {
        VFX_STARS_TRAIL = 0,
        VFX_TAP_ANIMATION = 1,

        LENGTH,
    }
}