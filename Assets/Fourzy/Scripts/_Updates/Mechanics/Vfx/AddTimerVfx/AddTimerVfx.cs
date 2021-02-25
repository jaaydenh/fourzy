//@vadym udod

using Fourzy._Updates.Tween;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.Mechanics._Vfx
{
    public class AddTimerVfx : Vfx
    {
        public float speed = 2f;

        public TMP_Text label;

        public Vector3 direction { get; set; }

        private AddTimerVfx _copyParent;

        protected override void Update()
        {
            base.Update();

            if (isActive && durationLeft != 0f)
                transform.localPosition += (_copyParent ? _copyParent.direction : direction) * Time.deltaTime * speed;
        }

        public Vfx SetValue(Transform target, Vector3 offset, Vector3 direction, string value)
        {
            this.direction = direction;

            label.text = value;

            return StartVfx(target, offset, 0f);
        }

        public override Vfx StartVfx()
        {
            if (copyParent) _copyParent = copyParent as AddTimerVfx;

            return base.StartVfx();
        }
    }
}
