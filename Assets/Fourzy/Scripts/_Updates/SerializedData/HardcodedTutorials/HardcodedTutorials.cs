//@vadym udod

using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates._Tutorial
{
    public class HardcodedTutorials
    {
        public static List<Tutorial> tutorials = new List<Tutorial>()
        {
            new Tutorial()
            {
                name = "Onboarding",
                data = new OnboardingTasksBatch[]
                    {
                        //step 1: welcome message
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask_OpenGame(GameType.ONBOARDING, "200"),
                                new OnboardingTask_ShowMessage(LocalizationManager.Value("welcome_to_fourzy")),
                                new OnboardingTask() { action = OnboardingActions.WIZARD_CENTER },
                                new OnboardingTask() { action = OnboardingActions.SHOW_BG },
                                new OnboardingTask_Log("1"),
                            },
                        },
                        //step 2: load tutorial map + show rules
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                                new OnboardingTask() { action = OnboardingActions.HIDE_WIZARD },
                                new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },

                                new OnboardingTask_OpenGame(GameType.ONBOARDING, "201"),

                                new OnboardingTask() { action = OnboardingActions.SHOW_BG },
                                new OnboardingTask() { action = OnboardingActions.WIZARD_CENTER },
                                new OnboardingTask_ShowMessage(LocalizationManager.Value("tap_edge_rule")),
                                new OnboardingTask() { action = OnboardingActions.SHOW_BOARD_HINT_AREA },
                                new OnboardingTask_Log("2"),
                            },
                        },
                        //step 3: show player where to tap to make move
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                                new OnboardingTask() { action = OnboardingActions.HIDE_BOARD_HINT_AREA },
                                new OnboardingTask() { action = OnboardingActions.HIDE_WIZARD },
                                new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
                                new OnboardingTask_PointAt(new Vector2(0, 6), LocalizationManager.Value("tap_to_place")),
                                new OnboardingTask_LimitInput(new Rect(0f, 6f, 1f, 1f)),
                                new OnboardingTask() { action = OnboardingActions.SHOW_BOARD_HINT_AREA },
                                new OnboardingTask_ShowMaskedArea(new Rect(0f, 6f, 1f, 1f), OnboardingScreenMaskObject.MaskStyle.PX_0, new Vector2(72f, 72f)),
                                //this will skip to next
                                new OnboardingTask() { action = OnboardingActions.ON_MOVE_STARTED },
                                new OnboardingTask_Log("3"),
                            },
                        },
                        //step 4: cancel input after player move started
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                                new OnboardingTask() { action = OnboardingActions.HIDE_BOARD_HINT_AREA },
                                new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                                new OnboardingTask_LimitInput(new Rect[0]),
                                //this will skip to next
                                new OnboardingTask() { action = OnboardingActions.ON_MOVE_ENDED },
                            },
                        },
                        //step 5: opponent takes turn
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask_PlaceGamepiece(2, FourzyGameModel.Model.Direction.RIGHT, 3),
                                //this will skip to next
                                new OnboardingTask() { action = OnboardingActions.ON_MOVE_ENDED },
                            },
                        },
                        //step 6: players' 2d turn + limit input
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                //this will skip to next
                                new OnboardingTask() { action = OnboardingActions.ON_MOVE_STARTED },
                                new OnboardingTask_PointAt(new Vector2(7, 5), LocalizationManager.Value("tap_to_place")),
                                new OnboardingTask_LimitInput(new Rect(7f, 5f, 1f, 1f)),
                                new OnboardingTask() { action = OnboardingActions.SHOW_BOARD_HINT_AREA },
                                new OnboardingTask_ShowMaskedArea(new Rect(7f, 5f, 1f, 1f), OnboardingScreenMaskObject.MaskStyle.PX_0, new Vector2(72f, 72f)),
                                new OnboardingTask_Log("4"),
                            },
                        },
                        //step 7: cancel input after player move started
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                                new OnboardingTask() { action = OnboardingActions.HIDE_BOARD_HINT_AREA },
                                new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                                new OnboardingTask_LimitInput(new Rect[0]),
                                //this will skip to next
                                new OnboardingTask() { action = OnboardingActions.ON_MOVE_ENDED },
                            },
                        },
                        //step 8: opponent takes turn
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask_PlaceGamepiece(2, FourzyGameModel.Model.Direction.DOWN, 4),
                                //this will skip to next
                                new OnboardingTask() { action = OnboardingActions.ON_MOVE_ENDED },
                            },
                        },
                        //step 9: show message
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask_ShowMessage(LocalizationManager.Value("get_4_to_win")),
                                new OnboardingTask() { action = OnboardingActions.SHOW_BG },
                                new OnboardingTask() { action = OnboardingActions.WIZARD_CENTER },
                                new OnboardingTask_Log("5"),
                            },
                        },
                        //step 10: players' 3rd turn + limit input
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
                                new OnboardingTask() { action = OnboardingActions.HIDE_WIZARD },
                                new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                                new OnboardingTask_PointAt(new Vector2(3, 0), LocalizationManager.Value("tap_to_place")),
                                new OnboardingTask_LimitInput(new Rect(3f, 0f, 1f, 1f)),
                                new OnboardingTask_ShowMaskedArea(new Rect(3f, 0f, 1f, 1f), OnboardingScreenMaskObject.MaskStyle.PX_0, new Vector2(72f, 72f)),
                                //this will skip to next
                                new OnboardingTask() { action = OnboardingActions.ON_MOVE_STARTED },
                                new OnboardingTask() { action = OnboardingActions.SHOW_BOARD_HINT_AREA },
                                new OnboardingTask_Log("6"),
                            },
                        },
                        //step 11: cancel input after player move started
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                                new OnboardingTask() { action = OnboardingActions.HIDE_BOARD_HINT_AREA },
                                new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                                new OnboardingTask_LimitInput(new Rect[0]),
                                //this will skip to next
                                new OnboardingTask() { action = OnboardingActions.ON_MOVE_ENDED },
                            },
                        },
                        //step 12: show message
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask_ShowMessage(LocalizationManager.Value("excellent")),
                                new OnboardingTask() { action = OnboardingActions.SHOW_BG },
                                new OnboardingTask() { action = OnboardingActions.WIZARD_CENTER },
                                new OnboardingTask_Log("7"),
                            },
                        },
                        // //step 13: change name message
                        // new OnboardingTasksBatch()
                        // {
                        //     tasks = new OnboardingTask[]
                        //     {
                        //         new OnboardingTask() { action = OnboardingActions.SKIP_TO_NEXT_IF_DEMO_MODE },
                        //         new OnboardingTask_ShowMessage("Now lets change your name!"),
                        //         new OnboardingTask_Log("8"),
                        //     },
                        // },
                        // //step 14: open changename prompt
                        // new OnboardingTasksBatch()
                        // {
                        //     tasks = new OnboardingTask[]
                        //     {
                        //         new OnboardingTask() { action = OnboardingActions.SKIP_TO_NEXT_IF_DEMO_MODE },
                        //         new OnboardingTask() { action = OnboardingActions.USER_CHANGE_NAME_PROMPT },
                        //         new OnboardingTask_Log("9"),
                        //     },
                        // },
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
                                new OnboardingTask() { action = OnboardingActions.HIDE_WIZARD },
                                new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                                //will continue after main menu loaded
                                new OnboardingTask() { action = OnboardingActions.LOAD_MAIN_MENU },
                                //new OnboardingDataHolder.OnboardingTask_ExecMenuEvent(Constants.MAIN_MENU_CANVAS_NAME, "openScreen", "tutorialProgressionMap"),
                                //point at adventure button button
                                new OnboardingTask_HighlightButton("AdventureButton", LocalizationManager.Value("tap_to_open_map")),
                                //will continue after event tap
                                new OnboardingTask_HighlightProgressionEvent (-1, LocalizationManager.Value("tap_to_start_playing")),
                            },
                        },
                    },
            },

        };

        public static void Initialize() => tutorials.ForEach(tutorial => tutorial.Initialize());
    }

    public class Tutorial
    {
        public string name;
        public bool previousState;

        public OnboardingTasksBatch[] data;

        public bool wasFinishedThisSession => !previousState && PlayerPrefsWrapper.GetTutorialFinished(name);

        public void Initialize()
        {
            previousState = PlayerPrefsWrapper.GetTutorialFinished(name);
        }
    }

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

    public class OnboardingTask
    {
        public OnboardingActions action;
        public bool unfolded;

        public string stringValue;
        public int intValue;
        public Vector2 vector2value;
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
        public OnboardingTask_PointAt(Vector2 boardLocation, string message = "")
        {
            action = OnboardingActions.POINT_AT;

            vector2value = boardLocation;
            stringValue = message;
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
        public OnboardingTask_ShowMaskedArea(Rect area, OnboardingScreenMaskObject.MaskStyle maskStyle, Vector2 size)
        {
            action = OnboardingActions.SHOW_MASKED_AREA;

            intValue = (int)maskStyle;
            areas = new Rect[] { area };
            vector2value = size;
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
        public OnboardingTask_HighlightProgressionEvent(int index, string message = "")
        {
            action = OnboardingActions.HIGHLIGHT_PROGRESSION_EVENT;
            intValue = index;
            stringValue = message;
        }
    }

    public class OnboardingTask_HighlightButton : OnboardingTask
    {
        public string message;

        public OnboardingTask_HighlightButton(string buttonName, string message = "")
        {
            action = OnboardingActions.HIGHLIGHT_CURRENT_SCREEN_BUTTON;

            stringValue = buttonName;
            this.message = message;
        }
    }
}
