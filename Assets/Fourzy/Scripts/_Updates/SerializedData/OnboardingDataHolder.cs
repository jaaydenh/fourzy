//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "OnboardingDataHolder", menuName = "Create Onboarding data holder")]
    public class OnboardingDataHolder : ScriptableObject
    {
        public OnboardingTask[] tasks;

        public enum OnboardingActions
        {
            SHOW_MESSAGE,
            POINT_AT,
            HIGHLIGHT,
            PLAY_INITIAL_MOVES,

        }

        [System.Serializable]
        public class OnboardingTask
        {
            public OnboardingActions action;
            public bool unfolded;

            public bool hideOther = false;
            public string message;
            public Vector2 pointAt;
            public HighlightArea[] areas;

            public OnboardingTask()
            {
                areas = new HighlightArea[] { new HighlightArea() { from = Vector2Int.zero, to = Vector2Int.one } };
            }

            [System.Serializable]
            public class HighlightArea
            {
                public Vector2Int from;
                public Vector2Int to;
            }
        }
    }
}
