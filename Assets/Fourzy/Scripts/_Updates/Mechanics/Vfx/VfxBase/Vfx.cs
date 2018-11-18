//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Mechanics.Vfx
{
    public class Vfx : MonoBehaviour
    {
        public VfxType type;
        public float duration = 1f;

        [HideInInspector]
        public VfxHolder holder;
        [HideInInspector]
        public bool isActive = false;

        public float durationLeft { get; protected set; }

        protected virtual void Update()
        {
            if (isActive && durationLeft != 0f)
            {
                if (durationLeft - Time.fixedDeltaTime > 0f)
                    durationLeft -= Time.fixedDeltaTime;
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

            durationLeft = duration;
        }

        public virtual void StartVfx(Transform target, Vector3 offset, float rotation)
        {
            transform.SetParent(target);

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
        VFX_CAR_SPARK = 0,
        VFX_WHEEL_SKID = 1,
        VFX_OIL_SKID = 2,

        VFX_COIN_EXPLOSION = 10,

        LENGTH,
    }
}