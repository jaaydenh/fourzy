//@vadym udod

namespace Fourzy._Updates.UI.Toasts
{
    public class TopGameToast : GameToast
    {
        public override void Hide(float time)
        {
            if (IsRoutineActive("removeFromQueue"))
                return;

            base.Hide(time);

            CancelRoutine("displayCoroutine");
            StartRoutine("removeFromQueue", time, () =>
            {
                if (owner)
                {
                    if (owner.topActiveToasts.Contains(this))
                        owner.topActiveToasts.Dequeue();

                    owner.topMovableToast.Dequeue();
                }

                available = true;
            }, null);
        }
    }
}