//@vadym udod

using FourzyGameModel.Model;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "OnboardingDataHolder", menuName = "Create Onboarding Data Holder")]
    public class OnboardingDataHolder : ScriptableObject
    {
        public string tutorialName = "tutorial";
        public bool showPlayer2;
        public OnFinished onFinished;
        public OnboardingTasksBatch[] batches;

        public enum OnboardingActions
        {
            SHOW_MESSAGE,
            HIDE_MESSAGE_BOX,

            POINT_AT,
            HIDE_POINTER,

            HIGHLIGHT,
            HIDE_HIGHLIGHT,

            PLAY_INITIAL_MOVES,
            OPEN_GAME,

            LIMIT_BOARD_INPUT,
            RESET_BOARD_INPUT,

            HIDE_BG,
            SHOW_BG,

            HIDE_WIZARD,
            WIZARD_CENTER,

            PLAYER_2_PLACE_GAMEPIECE,
            PLAYER_1_PLACE_GAMEPIECE,

            ON_PLAYER1_MOVE_ENDED,
            ON_PLAYER2_MOVE_ENDED,
            ON_PLAYER1_MOVE_STARTED,
            ON_PLAYER2_MOVE_STARTED,

            USER_CHANGE_NAME_PROMPT,
        }

        public enum OnFinished
        {
            LOAD_MAIN_MENU,
        }

        public enum OnGameFinished
        {
            CONTINUE,
        }

        public enum NextAction
        {
            NEXT,
        }

        [System.Serializable]
        public class OnboardingTasksBatch
        {
            public OnboardingTask[] tasks;

            public OnboardingTasksBatch()
            {
                tasks = new OnboardingTask[0];
            }
        }

        [System.Serializable]
        public class OnboardingTask
        {
            public OnboardingActions action;
            public bool unfolded;

            public string message;
            public int intValue;
            public Vector2 pointAt;
            public Rect[] areas;
            public OnGameFinished onGameFinished;
            public NextAction nextAction;
            public Direction direction;

            public OnboardingTask()
            {
                areas = new Rect[] { new Rect(0, 0, 1, 1), };
            }
        }
    }
}
