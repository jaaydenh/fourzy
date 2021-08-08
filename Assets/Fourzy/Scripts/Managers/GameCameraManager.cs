//@vadym udod

using Fourzy._Updates.Tools;
using MoreMountains.Feedbacks;

namespace Fourzy._Updates.Managers
{
    public class GameCameraManager : RoutinesBase
    {
        private MMWiggle wiggle;

        protected override void Awake()
        {
            base.Awake();

            wiggle = GetComponent<MMWiggle>();
        }

        public void Wiggle()
        {
            wiggle.WigglePosition(.1f);
        }
    }
}
