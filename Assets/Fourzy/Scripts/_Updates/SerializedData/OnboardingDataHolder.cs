//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "OnboardingDataHolder", menuName = "Create Onboarding data holder")]
    public class OnboardingDataHolder : ScriptableObject
    {

        public enum OnboardingActions
        {
            SHOW_MESSAGE,

        }

        public interface OnboardingTask
        {

        }
    }
}
