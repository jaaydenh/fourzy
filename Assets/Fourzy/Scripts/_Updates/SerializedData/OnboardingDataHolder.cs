//@vadym udod

using FourzyGameModel.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    public class OnboardingDataHolder
    {
        public string tutorialName = "tutorial";
        //public OnFinished onFinished;
        //public OpenScreen openScreen;
        //public GameType gameType;
        //public string stringValue;
        public OnboardingTasksBatch[] batches;

        public static implicit operator bool (OnboardingDataHolder value) => value != null;

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

            ON_MOVE_ENDED,
            ON_MOVE_STARTED = 18,

            USER_CHANGE_NAME_PROMPT = 20,

            SHOW_BOARD_HINT_AREA,
            HIDE_BOARD_HINT_AREA,

            LOG_TUTORIAL,

            SHOW_MASKED_AREA,
            HIDE_MAKSED_AREA,

            SKIP_TO_NEXT_IF_DEMO_MODE,

            SHOW_OPPONENT,
            HIDE_OPPONENT,

            LOAD_MAIN_MENU,
            EXEC_MENU_EVENT,

            HIGHLIGHT_PROGRESSION_EVENT,
            HIGHLIGHT_CURRENT_SCREEN_BUTTON,
        }

        public enum OnFinished
        {
            LOAD_MAIN_MENU,
            LOAD_GAME_SCENE,
        }

        public enum OpenScreen
        {
            MAIN,
            PUZZLES_SCREEN,
        }

        public enum OnGameFinished
        {
            CONTINUE,
        }

        public enum NextAction
        {
            NEXT,
        }

        public class OnboardingTasksBatch
        {
            public OnboardingTask[] tasks;

            public OnboardingTasksBatch()
            {
                tasks = new OnboardingTask[0];
            }

            public bool ContainsAction(OnboardingActions action)
            {
                foreach (OnboardingTask task in tasks)
                    if (task.action == action)
                        return true;

                return false;
            }
        }

        [System.Serializable]
        public class OnboardingTask
        {
            public OnboardingActions action;
            public bool unfolded;

            public string stringValue;
            public int intValue;
            public Vector2 pointAt;
            public Rect[] areas;
            public Direction direction;
            public KeyValuePair<string, object> menuEvent;

            public OnboardingTask()
            {
                areas = new Rect[] { new Rect(0, 0, 1, 1), };
            }
        }

        //shortcuts
        public class OnboardingTask_OpenGame : OnboardingTask
        {
            public OnboardingTask_OpenGame(GameType type, string gameID)
            {
                action = OnboardingActions.OPEN_GAME;

                intValue = (int)type;
                stringValue = gameID;
            }
        }

        public class OnboardingTask_ShowMessage : OnboardingTask
        {
            public OnboardingTask_ShowMessage(string message)
            {
                action = OnboardingActions.SHOW_MESSAGE;
                stringValue = message;
            }
        }

        public class OnboardingTask_Log : OnboardingTask
        {
            public OnboardingTask_Log(string stage)
            {
                action = OnboardingActions.LOG_TUTORIAL;
                stringValue = stage;
            }
        }

        public class OnboardingTask_PointAt : OnboardingTask
        {
            public OnboardingTask_PointAt(Vector2 at)
            {
                action = OnboardingActions.POINT_AT;
                pointAt = at;
            }
        }

        public class OnboardingTask_LimitInput : OnboardingTask
        {
            public OnboardingTask_LimitInput(Rect area)
            {
                action = OnboardingActions.LIMIT_BOARD_INPUT;
                areas = new Rect[] { area };
            }

            public OnboardingTask_LimitInput(Rect[] areas)
            {
                action = OnboardingActions.LIMIT_BOARD_INPUT;
                this.areas = areas;
            }
        }

        public class OnboardingTask_ShowMaskedArea : OnboardingTask
        {
            public bool customMask => size != Vector2.zero;

            public Vector2 size;

            public OnboardingTask_ShowMaskedArea(Rect area)
            {
                action = OnboardingActions.SHOW_MASKED_AREA;
                areas = new Rect[] { area };
            }

            public OnboardingTask_ShowMaskedArea(Rect[] areas)
            {
                action = OnboardingActions.SHOW_MASKED_AREA;
                this.areas = areas;
            }

            public OnboardingTask_ShowMaskedArea(Vector2 position, Vector2 size)
            {
                action = OnboardingActions.SHOW_MASKED_AREA;
                pointAt = position;
                this.size = size;
            }
        }

        public class OnboardingTask_PlaceGamepiece : OnboardingTask
        {
            public OnboardingTask_PlaceGamepiece(int playerID, Direction direction, int location)
            {
                if (playerID == 1) action = OnboardingActions.PLAYER_1_PLACE_GAMEPIECE;
                else action = OnboardingActions.PLAYER_2_PLACE_GAMEPIECE;
                this.direction = direction;
                intValue = location;
            }
        }

        public class OnboardingTask_ExecMenuEvent : OnboardingTask
        {
            public OnboardingTask_ExecMenuEvent(string menuName, string command, object value)
            {
                action = OnboardingActions.EXEC_MENU_EVENT;
                stringValue = menuName;
                menuEvent = new KeyValuePair<string, object>(command, value);
            }
        }

        public class OnboardingTask_HighlightProgressionEvent : OnboardingTask
        {
            public OnboardingTask_HighlightProgressionEvent(int index)
            {
                action = OnboardingActions.HIGHLIGHT_PROGRESSION_EVENT;
                intValue = index;
            }
        }

        public class OnboardingTask_HighlightButton : OnboardingTask
        {
            public OnboardingTask_HighlightButton(string buttonName)
            {
                action = OnboardingActions.HIGHLIGHT_CURRENT_SCREEN_BUTTON;
                stringValue = buttonName;
            }
        }
    }
}
