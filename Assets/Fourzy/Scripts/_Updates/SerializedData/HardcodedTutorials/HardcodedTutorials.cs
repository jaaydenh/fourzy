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
                onBack = TutorialOnBack.SHOW_LEAVE_PROMPT,
                data = new OnboardingTasksBatch[]
                    {
                        //step 1: welcome message
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask_OpenGame(GameType.ONBOARDING, "200"),
                                new OnboardingTask_ShowMessage(LocalizationManager.Value("welcome_to_fourzy"), 0),
                                new OnboardingTask() { action = OnboardingActions.WIZARD_CENTER },
                                new OnboardingTask() { action = OnboardingActions.SHOW_BG },
                                new OnboardingTask_Log("1"),
                            },
                        },
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask_ShowMessage(LocalizationManager.Value("get_4_to_win"), 0),
                                new OnboardingTask_Log("2"),
                            },
                        },
                        ////step 2: load tutorial map + show rules
                        //new OnboardingTasksBatch()
                        //{
                        //    tasks = new OnboardingTask[]
                        //    {
                        //        new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                        //        new OnboardingTask() { action = OnboardingActions.HIDE_WIZARD },
                        //        new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },

                        //        new OnboardingTask_OpenGame(GameType.ONBOARDING, "201"),

                        //        new OnboardingTask() { action = OnboardingActions.SHOW_BG },
                        //        new OnboardingTask() { action = OnboardingActions.WIZARD_CENTER },
                        //        new OnboardingTask_ShowMessage(LocalizationManager.Value("tap_edge_rule"), 1),
                        //        new OnboardingTask() { action = OnboardingActions.SHOW_BOARD_HINT_AREA },
                        //        new OnboardingTask_Log("2"),
                        //    },
                        //},
                        //step 3: show player where to tap to make move
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                                //new OnboardingTask() { action = OnboardingActions.HIDE_BOARD_HINT_AREA },
                                new OnboardingTask() { action = OnboardingActions.HIDE_WIZARD },
                                new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
                                new OnboardingTask_OpenGame(GameType.ONBOARDING, "onboarding1"),
                                //new OnboardingTask_PointAt(LocalizationManager.Value("tap_to_place"), new Vector2(0f, 6f), new Vector2(2f, 6f)),
                                new OnboardingTask_PointAt("", new Vector2(4f, 0f), new Vector2(4f, 2f)),
                                new OnboardingTask_ShowMessage(LocalizationManager.Value("rule_swipe_down"), 1),
                                new OnboardingTask_LimitInput(new Rect(4f, 0f, 1f, 1f)),
                                new OnboardingTask() { action = OnboardingActions.SHOW_BOARD_HINT_AREA },
                                new OnboardingTask_ShowMaskedArea(new Rect(4f, 0f, 1f, 3f), OnboardingScreenMaskObject.MaskStyle.PX_0),
                                //this will skip to next
                                new OnboardingTask() { action = OnboardingActions.ON_MOVE_ENDED },
                                new OnboardingTask_Log("3"),
                            },
                        },
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
                                new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                                new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                                new OnboardingTask_Wait(2f),
                            }
                        },
                        ////step 4: cancel input after player move started
                        //new OnboardingTasksBatch()
                        //{
                        //    tasks = new OnboardingTask[]
                        //    {
                        //        new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
                        //        new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                        //        new OnboardingTask() { action = OnboardingActions.HIDE_BOARD_HINT_AREA },
                        //        new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                        //        //new OnboardingTask_LimitInput(new Rect[0]),
                        //        //this will skip to next
                        //        new OnboardingTask() { action = OnboardingActions.ON_MOVE_ENDED },
                        //        new OnboardingTask_OpenGame(GameType.ONBOARDING, "onboarding2"),
                        //    },
                        //},
                        //////step 5: opponent takes turn
                        ////new OnboardingTasksBatch()
                        ////{
                        ////    tasks = new OnboardingTask[]
                        ////    {
                        ////        new OnboardingTask_PlaceGamepiece(2, Direction.RIGHT, 3),
                        ////        //this will skip to next
                        ////        new OnboardingTask() { action = OnboardingActions.ON_MOVE_ENDED },
                        ////    },
                        ////},
                        //step 6: players' 2 turn + limit input
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask_OpenGame(GameType.ONBOARDING, "onboarding2"),
                                //this will skip to next
                                new OnboardingTask_PointAt("", new Vector2(0f, 3f), new Vector2(2f, 3f)),
                                new OnboardingTask_ShowMessage(LocalizationManager.Value("rule_swipe_right"), 1),
                                new OnboardingTask_LimitInput(new Rect(0f, 3f, 3f, 1f)),
                                new OnboardingTask() { action = OnboardingActions.SHOW_BOARD_HINT_AREA },
                                new OnboardingTask_ShowMaskedArea(new Rect(0f, 3f, 3f, 1f), OnboardingScreenMaskObject.MaskStyle.PX_0),
                                new OnboardingTask() { action = OnboardingActions.ON_MOVE_ENDED },
                                new OnboardingTask_Log("4"),
                            },
                        },
                        ////step 7: cancel input after player move started
                        //new OnboardingTasksBatch()
                        //{
                        //    tasks = new OnboardingTask[]
                        //    {
                        //        new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
                        //        new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                        //        new OnboardingTask() { action = OnboardingActions.HIDE_BOARD_HINT_AREA },
                        //        new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                        //        new OnboardingTask_LimitInput(new Rect[0]),
                        //        //this will skip to next
                        //        new OnboardingTask() { action = OnboardingActions.ON_MOVE_ENDED },
                        //    },
                        //},
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
                                new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                                new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                                new OnboardingTask_Wait(2f),
                            }
                        },
                        ////step 8: opponent takes turn
                        //new OnboardingTasksBatch()
                        //{
                        //    tasks = new OnboardingTask[]
                        //    {
                        //        new OnboardingTask_PlaceGamepiece(2, FourzyGameModel.Model.Direction.DOWN, 4),
                        //        //this will skip to next
                        //        new OnboardingTask() { action = OnboardingActions.ON_MOVE_ENDED },
                        //    },
                        //},
                        //step 9: show message
                        //new OnboardingTasksBatch()
                        //{
                        //    tasks = new OnboardingTask[]
                        //    {
                        //        new OnboardingTask_ShowMessage(LocalizationManager.Value("get_4_to_win"), 0),
                        //        new OnboardingTask() { action = OnboardingActions.SHOW_BG },
                        //        new OnboardingTask() { action = OnboardingActions.WIZARD_CENTER },
                        //        new OnboardingTask_Log("5"),
                        //    },
                        //},
                        //step 10: players' 3rd turn + limit input
                        //new OnboardingTasksBatch()
                        //{
                        //    tasks = new OnboardingTask[]
                        //    {
                        //        new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
                        //        new OnboardingTask() { action = OnboardingActions.HIDE_WIZARD },
                        //        new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                        //        new OnboardingTask_PointAt("", new Vector2(3f, 0f), new Vector2(3f, 2f)),
                        //        new OnboardingTask_ShowMessage(LocalizationManager.Value("rule_swipe_down"), 1),
                        //        new OnboardingTask_LimitInput(new Rect(3f, 0f, 1f, 3f)),
                        //        new OnboardingTask_ShowMaskedArea(new Rect(3f, 0f, 1f, 3f), OnboardingScreenMaskObject.MaskStyle.PX_0),
                        //        //this will skip to next
                        //        new OnboardingTask() { action = OnboardingActions.ON_MOVE_STARTED },
                        //        new OnboardingTask() { action = OnboardingActions.SHOW_BOARD_HINT_AREA },
                        //        new OnboardingTask_Log("6"),
                        //    },
                        //},
                        //step 11: cancel input after player move started
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask_OpenGame(GameType.ONBOARDING, "onboarding3"),
                                new OnboardingTask_PointAt("", new Vector2(0f, 1f), new Vector2(2f, 1f)),
                                new OnboardingTask_ShowMessage(LocalizationManager.Value("rule_swipe_right"), 1),
                                new OnboardingTask_LimitInput(new Rect(0f, 1f, 3f, 1f)),
                                new OnboardingTask() { action = OnboardingActions.SHOW_BOARD_HINT_AREA },
                                new OnboardingTask_ShowMaskedArea(new Rect(0f, 1f, 3f, 1f), OnboardingScreenMaskObject.MaskStyle.PX_0),
                                new OnboardingTask() { action = OnboardingActions.ON_MOVE_ENDED },
                                new OnboardingTask_Log("4"),
                            },
                        },
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
                                new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                                new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                                new OnboardingTask_Wait(2f),
                            }
                        },
                        //new OnboardingTasksBatch()
                        //{
                        //    tasks = new OnboardingTask[]
                        //    {
                        //        //new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
                        //        //new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                        //        //new OnboardingTask() { action = OnboardingActions.HIDE_BOARD_HINT_AREA },
                        //        //new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                        //        new OnboardingTask_LimitInput(new Rect[0]),
                        //        //this will skip to next
                        //        new OnboardingTask() { action = OnboardingActions.ON_MOVE_ENDED },
                        //        new OnboardingTask_OpenGame(GameType.ONBOARDING, "onboarding3"),
                        //    },
                        //},
                        ////step 12: show message
                        //new OnboardingTasksBatch()
                        //{
                        //    tasks = new OnboardingTask[]
                        //    {
                        //        new OnboardingTask_ShowMessage(LocalizationManager.Value("excellent"), 0),
                        //        new OnboardingTask() { action = OnboardingActions.SHOW_BG },
                        //        new OnboardingTask() { action = OnboardingActions.WIZARD_CENTER },
                        //        new OnboardingTask_Log("7"),
                        //    },
                        //},
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
                                //new OnboardingTask() { action = OnboardingActions.HIDE_WIZARD },
                                //new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                                //will continue after main menu loaded
                                new OnboardingTask() { action = OnboardingActions.LOAD_MAIN_MENU },
                                //new OnboardingDataHolder.OnboardingTask_ExecMenuEvent(Constants.MAIN_MENU_CANVAS_NAME, "openScreen", "tutorialProgressionMap"),
                                //point at adventure button button
                                new OnboardingTask_Log("8"),
                                new OnboardingTask_HighlightButton(
                                    "AdventureButton",
                                    "MainMenuCanvas",
                                    Vector2.one,
                                    Vector2.zero,
                                    messagePositionData: null,
                                    LocalizationManager.Value("tap_to_open_map")),
                                new OnboardingTask_Log("9"),
                            },
                        },
                        new OnboardingTasksBatch()
                        {
                            tasks = new OnboardingTask[]
                            {
                                //will continue after event tap
                                new OnboardingTask_HighlightProgressionEvent (
                                    0,
                                    Vector3.one * .75f,
                                    LocalizationManager.Value("tap_to_start_playing")),
                                new OnboardingTask_Log("10"),
                            },
                        },
                    },
            },

            new Tutorial()
            {
                name = "HintInstruction",
                onBack = TutorialOnBack.IGNORE,
                data = new OnboardingTasksBatch[]
                {
                    new OnboardingTasksBatch()
                    {
                        tasks = new OnboardingTask[]
                        {
                            new OnboardingTask_HighlightButton(
                                "HintButton",
                                "GameSceneCanvas",
                                Vector2.one,
                                new Vector2(.07f, 0f),
                                //new Vector2(0f, 320f)
                                new OnboardingTask_HighlightButton.MessageBoxPositionData(Vector2.zero, Vector2.up * 105f),
                                LocalizationManager.Value("suggest_hint_use"),
                                false),
                        }
                    }
                }
            }
        };

        public static void Initialize() => tutorials.ForEach(tutorial => tutorial.Initialize());
    }

    public class Tutorial
    {
        public string name;
        public bool previousState;

        public OnboardingTasksBatch[] data;
        public TutorialOnBack onBack;

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
        GAME_FINISHED,

        LIMIT_BOARD_INPUT,
        RESET_BOARD_INPUT,

        HIDE_BG,
        SHOW_BG,

        HIDE_WIZARD,
        WIZARD_CENTER,

        PLAYER_2_PLACE_GAMEPIECE,
        PLAYER_1_PLACE_GAMEPIECE,

        ON_MOVE_ENDED,
        ON_MOVE_STARTED,

        USER_CHANGE_NAME_PROMPT,

        SHOW_BOARD_HINT_AREA,
        HIDE_BOARD_HINT_AREA,

        LOG_TUTORIAL,
        WAIT,

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

    public enum TutorialOnBack
    {
        SHOW_LEAVE_PROMPT,
        IGNORE,
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
        public OnboardingTask_ShowMessage(string message, int style)
        {
            action = OnboardingActions.SHOW_MESSAGE;
            stringValue = message;
            intValue = style;
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
        public Vector2[] points;

        public OnboardingTask_PointAt(string message = "", params Vector2[] points)
        {
            action = OnboardingActions.POINT_AT;

            this.points = points;
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
        public OnboardingTask_ShowMaskedArea(Rect area, OnboardingScreenMaskObject.MaskStyle maskStyle)
        {
            action = OnboardingActions.SHOW_MASKED_AREA;

            intValue = (int)maskStyle;
            areas = new Rect[] { area };
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
        public bool showBG;

        public OnboardingTask_HighlightProgressionEvent(int index, Vector2 scale, string message = "", bool showBG = true)
        {
            action = OnboardingActions.HIGHLIGHT_PROGRESSION_EVENT;

            intValue = index;
            stringValue = message;
            vector2value = scale;

            this.showBG = showBG;
        }
    }

    public class OnboardingTask_Wait : OnboardingTask
    {
        public float time;

        public OnboardingTask_Wait(float time)
        {
            action = OnboardingActions.WAIT;
            this.time = time;
        }
    }

    public class OnboardingTask_HighlightButton : OnboardingTask
    {
        public string message;
        public string menuName;
        public bool showBG;
        public Vector2 pointerOffset;
        public MessageBoxPositionData messagePositionData;

        public OnboardingTask_HighlightButton(string buttonName, string menuName, Vector2 scale, Vector2 pointerOffset, MessageBoxPositionData messagePositionData = null, string message = "", bool showBG = true)
        {
            action = OnboardingActions.HIGHLIGHT_CURRENT_SCREEN_BUTTON;

            stringValue = buttonName;
            vector2value = scale;

            this.pointerOffset = pointerOffset;
            this.messagePositionData = messagePositionData;
            this.message = message;
            this.menuName = menuName;
            this.showBG = showBG;
        }

        public void TrySetMessagePivot(Vector2 pivot)
        {
            if (messagePositionData == null || messagePositionData.pivot != Vector2.zero) return;

            messagePositionData.pivot = pivot;
        }

        public class MessageBoxPositionData
        {
            public Vector2 pivot { get; set; }
            public Vector2 positionOffset { get; set; }

            public MessageBoxPositionData(Vector2 pivot, Vector2 positionOffset)
            {
                this.pivot = pivot;
                this.positionOffset = positionOffset;
            }
        }
    }
}
