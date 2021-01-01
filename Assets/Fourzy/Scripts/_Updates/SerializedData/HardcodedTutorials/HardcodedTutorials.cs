//@vadym udod

using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Fourzy.GameManager;

namespace Fourzy._Updates._Tutorial
{
    public class HardcodedTutorials
    {
        public static Tutorial GetByName(string name) => tutorials.Find(_tutorial => _tutorial.name == name);

        public static List<Tutorial> tutorials = new List<Tutorial>()
        {
            new Tutorial()
            {
                name = "Onboarding",
                onBack = TutorialOnBack.SHOW_LEAVE_PROMPT,
                tasks = new OnboardingTask[]
                {
                    new OnboardingTask_OpenGame(GameType.ONBOARDING, "200"),
                    new OnboardingTask() { action = OnboardingActions.WIZARD_CENTER },
                    new OnboardingTask() { action = OnboardingActions.SHOW_BG },
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("welcome_to_fourzy"), 0, .75f),
                    new OnboardingTask_Log("1"),

                    new OnboardingTask_ShowMessage(LocalizationManager.Value("get_4_to_win"), 0, .75f),

                    new OnboardingTask() { action = OnboardingActions.HIDE_WIZARD },
                    new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },

                    new OnboardingTask_OpenGame(GameType.ONBOARDING, "onboarding1"),

                    new OnboardingTask_PointAt(new Dictionary<PlacementStyle, Vector2[]>() {
                        [PlacementStyle.SWIPE_STYLE_2] = new Vector2[] { new Vector2(4f, 0f), new Vector2(4f, 2f) },
                        [PlacementStyle.EDGE_TAP] = new Vector2[] { new Vector2(4f, 0f) },
                    }),
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("tap_to_place"), 1, .15f),
                    new OnboardingTask_LimitInput(new Rect(4f, 0f, 1f, 1f)),
                    //new OnboardingTask() { action = OnboardingActions.SHOW_BOARD_HINT_AREA },
                    new OnboardingTask_ShowMaskedArea(new Dictionary<PlacementStyle, Rect>() {
                        [PlacementStyle.SWIPE_STYLE_2] = new Rect(4f, 0f, 1f, 3f),
                        [PlacementStyle.EDGE_TAP] = new Rect(4f, 0f, 1f, 1f),
                    }, OnboardingScreenMaskObject.MaskStyle.PX_0),

                    new OnboardingTask() { action = OnboardingActions.ON_MOVE_STARTED },
                    new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                    new OnboardingTask_Log("2"),

                    new OnboardingTask() { action = OnboardingActions.GAME_FINISHED },
                    new OnboardingTask() { action = OnboardingActions.SHOW_BG, intValue = -1},
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("win_vertical_rule"), 1, .15f, 5f),

                    new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
                    new OnboardingTask_Wait(1f),

                    new OnboardingTask_OpenGame(GameType.ONBOARDING, "onboarding2"),

                    new OnboardingTask_PointAt(new Dictionary<PlacementStyle, Vector2[]>() {
                        [PlacementStyle.SWIPE_STYLE_2] = new Vector2[] { new Vector2(0f, 3f), new Vector2(2f, 3f) },
                        [PlacementStyle.EDGE_TAP] = new Vector2[] { new Vector2(0f, 3f) },
                    }),
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("tap_to_place"), 1, .15f),
                    new OnboardingTask_LimitInput(new Rect(0f, 3f, 3f, 1f)),
                    //new OnboardingTask() { action = OnboardingActions.SHOW_BOARD_HINT_AREA },
                    new OnboardingTask_ShowMaskedArea(new Dictionary<PlacementStyle, Rect>() {
                        [PlacementStyle.SWIPE_STYLE_2] = new Rect(0f, 3f, 3f, 1f),
                        [PlacementStyle.EDGE_TAP] = new Rect(0f, 3f, 1f, 1f),
                    }, OnboardingScreenMaskObject.MaskStyle.PX_0),

                    new OnboardingTask() { action = OnboardingActions.ON_MOVE_STARTED },
                    new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                    new OnboardingTask_Log("3"),

                    new OnboardingTask() { action = OnboardingActions.GAME_FINISHED },
                    new OnboardingTask() { action = OnboardingActions.SHOW_BG, intValue = -1},
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("win_horizontal_rule"), 1, .15f, 5),

                    new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
                    new OnboardingTask_Wait(1f),

                    new OnboardingTask_OpenGame(GameType.ONBOARDING, "onboarding3"),

                    new OnboardingTask_PointAt(new Dictionary<PlacementStyle, Vector2[]>(){
                        [PlacementStyle.SWIPE_STYLE_2] = new Vector2[] { new Vector2(0f, 1f), new Vector2(2f, 1f) },
                        [PlacementStyle.EDGE_TAP] = new Vector2[] { new Vector2(0f, 1f) },
                    }),
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("tap_to_place"), 1, .15f),
                    new OnboardingTask_LimitInput(new Rect(0f, 1f, 3f, 1f)),
                    //new OnboardingTask() { action = OnboardingActions.SHOW_BOARD_HINT_AREA },
                    new OnboardingTask_ShowMaskedArea(new Dictionary<PlacementStyle, Rect>() {
                        [PlacementStyle.SWIPE_STYLE_2] = new Rect(0f, 1f, 3f, 1f),
                        [PlacementStyle.EDGE_TAP] = new Rect(0f, 1f, 1f, 1f),
                    }, OnboardingScreenMaskObject.MaskStyle.PX_0),

                    new OnboardingTask() { action = OnboardingActions.ON_MOVE_STARTED },
                    new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                    new OnboardingTask_Log("4"),

                    new OnboardingTask() { action = OnboardingActions.GAME_FINISHED },
                    new OnboardingTask() { action = OnboardingActions.SHOW_BG, intValue = -1},
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("win_diagonal_rule"), 1, .15f, 5f),

                    new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                    new OnboardingTask_Wait(1f),
                    new OnboardingTask() { action = OnboardingActions.LOAD_MAIN_MENU },

                    //point at adventure button button
                    new OnboardingTask_Log("5"),
                    new OnboardingTask_HighlightButton(
                        "AdventureButton",
                        "MainMenuCanvas",
                        Vector2.one,
                        Vector2.zero,
                        Vector2.zero,
                        new MessageData(LocalizationManager.Value("tap_to_open_map"), Vector2.zero, new Vector2(0f, -165f))),
                    new OnboardingTask_Log("6"),

                    new OnboardingTask_HighlightProgressionEvent (
                        0,
                        Vector3.one * .75f,
                        new MessageData(LocalizationManager.Value("tap_to_start_playing"), Vector2.zero, new Vector2(0f, -170f))),
                    new OnboardingTask_Log("7"),
                },
            },

            new Tutorial()
            {
                name = "OnboardingLandscape",
                onBack = TutorialOnBack.SHOW_LEAVE_PROMPT,
                tasks = new OnboardingTask[]
                {
                    new OnboardingTask_OpenGame(GameType.ONBOARDING, "200"),
                    new OnboardingTask() { action = OnboardingActions.WIZARD_CENTER },
                    new OnboardingTask() { action = OnboardingActions.SHOW_BG },
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("welcome_to_fourzy"), 0, .75f),
                    new OnboardingTask_Log("1"),

                    new OnboardingTask_ShowMessage(LocalizationManager.Value("get_4_to_win"), 0, .75f),

                    new OnboardingTask() { action = OnboardingActions.HIDE_WIZARD },
                    new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },

                    new OnboardingTask_OpenGame(GameType.ONBOARDING, "onboarding1"),

                    new OnboardingTask_PointAt(new Dictionary<PlacementStyle, Vector2[]>() {
                        [PlacementStyle.SWIPE_STYLE_2] = new Vector2[] { new Vector2(4f, 0f), new Vector2(4f, 2f) },
                        [PlacementStyle.EDGE_TAP] = new Vector2[] { new Vector2(4f, 0f) },
                    }),
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("rule_tap_2"), 1, .15f),
                    new OnboardingTask_LimitInput(new Rect(4f, 0f, 1f, 1f)),
                    //new OnboardingTask() { action = OnboardingActions.SHOW_BOARD_HINT_AREA },
                    new OnboardingTask_ShowMaskedArea(new Dictionary<PlacementStyle, Rect>() {
                        [PlacementStyle.SWIPE_STYLE_2] = new Rect(4f, 0f, 1f, 3f),
                        [PlacementStyle.EDGE_TAP] = new Rect(4f, 0f, 1f, 1f),
                    }, OnboardingScreenMaskObject.MaskStyle.PX_0),

                    new OnboardingTask() { action = OnboardingActions.ON_MOVE_STARTED },
                    new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                    new OnboardingTask_Log("2"),

                    new OnboardingTask() { action = OnboardingActions.GAME_FINISHED },
                    new OnboardingTask() { action = OnboardingActions.SHOW_BG, intValue = -1},
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("win_vertical_rule"), 1, .15f, 5f),

                    new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
                    new OnboardingTask_Wait(1f),

                    new OnboardingTask_OpenGame(GameType.ONBOARDING, "onboarding2"),

                    new OnboardingTask_PointAt(new Dictionary<PlacementStyle, Vector2[]>() {
                        [PlacementStyle.SWIPE_STYLE_2] = new Vector2[] { new Vector2(0f, 3f), new Vector2(2f, 3f) },
                        [PlacementStyle.EDGE_TAP] = new Vector2[] { new Vector2(0f, 3f) },
                    }),
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("rule_tap_2"), 1, .15f),
                    new OnboardingTask_LimitInput(new Rect(0f, 3f, 3f, 1f)),
                    //new OnboardingTask() { action = OnboardingActions.SHOW_BOARD_HINT_AREA },
                    new OnboardingTask_ShowMaskedArea(new Dictionary<PlacementStyle, Rect>() {
                        [PlacementStyle.SWIPE_STYLE_2] = new Rect(0f, 3f, 3f, 1f),
                        [PlacementStyle.EDGE_TAP] = new Rect(0f, 3f, 1f, 1f),
                    }, OnboardingScreenMaskObject.MaskStyle.PX_0),

                    new OnboardingTask() { action = OnboardingActions.ON_MOVE_STARTED },
                    new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                    new OnboardingTask_Log("3"),

                    new OnboardingTask() { action = OnboardingActions.GAME_FINISHED },
                    new OnboardingTask() { action = OnboardingActions.SHOW_BG, intValue = -1},
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("win_horizontal_rule"), 1, .15f, 5),

                    new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
                    new OnboardingTask_Wait(1f),

                    new OnboardingTask_OpenGame(GameType.ONBOARDING, "onboarding3"),

                    new OnboardingTask_PointAt(new Dictionary<PlacementStyle, Vector2[]>(){
                        [PlacementStyle.SWIPE_STYLE_2] = new Vector2[] { new Vector2(0f, 1f), new Vector2(2f, 1f) },
                        [PlacementStyle.EDGE_TAP] = new Vector2[] { new Vector2(0f, 1f) },
                    }),
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("rule_tap_3"), 1, .15f),
                    new OnboardingTask_LimitInput(new Rect(0f, 1f, 3f, 1f)),
                    //new OnboardingTask() { action = OnboardingActions.SHOW_BOARD_HINT_AREA },
                    new OnboardingTask_ShowMaskedArea(new Dictionary<PlacementStyle, Rect>() {
                        [PlacementStyle.SWIPE_STYLE_2] = new Rect(0f, 1f, 3f, 1f),
                        [PlacementStyle.EDGE_TAP] = new Rect(0f, 1f, 1f, 1f),
                    }, OnboardingScreenMaskObject.MaskStyle.PX_0),

                    new OnboardingTask() { action = OnboardingActions.ON_MOVE_STARTED },
                    new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                    new OnboardingTask_Log("4"),

                    new OnboardingTask() { action = OnboardingActions.GAME_FINISHED },
                    new OnboardingTask() { action = OnboardingActions.SHOW_BG, intValue = -1},
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("win_diagonal_rule"), 1, .15f, 5f),

                    new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                    new OnboardingTask_Wait(1f),
                    new OnboardingTask() { action = OnboardingActions.LOAD_MAIN_MENU },
                },
            },

            new Tutorial()
            {
                name = "HintInstruction",
                onBack = TutorialOnBack.IGNORE,
                tasks = new OnboardingTask[]
                {
                    new OnboardingTask_HighlightButton(
                        "HintButton",
                        "GameSceneCanvas",
                        Vector2.one,
                        Vector2.zero,
                        new Vector2(40f, -40f),
                        new MessageData(LocalizationManager.Value("suggest_hint_use"), Vector2.zero, Vector2.up * 120f),
                        true),
                }
            }
        };

        public static void Initialize() => tutorials.ForEach(tutorial => tutorial.Initialize());
    }

    public class Tutorial
    {
        public string name;
        public bool previousState;

        public OnboardingTask[] tasks;
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

    //public class OnboardingTasksBatch
    //{
    //    public OnboardingTask[] tasks;

    //    public OnboardingTasksBatch()
    //    {
    //        tasks = new OnboardingTask[0];
    //    }

    //    public bool ContainsAction(OnboardingActions action)
    //    {
    //        foreach (OnboardingTask task in tasks)
    //            if (task.action == action)
    //                return true;

    //        return false;
    //    }
    //}

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
        public float yAnchor;
        public float skipAfter;

        public OnboardingTask_ShowMessage(string message, int style, float yAnchor, float skipAfter = -1f)
        {
            action = OnboardingActions.SHOW_MESSAGE;
            stringValue = message;
            intValue = style;

            this.yAnchor = yAnchor;
            this.skipAfter = skipAfter;
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
        public Dictionary<PlacementStyle, Vector2[]> points;

        public OnboardingTask_PointAt(Dictionary<PlacementStyle, Vector2[]> points, string message = "")
        {
            action = OnboardingActions.POINT_AT;

            this.points = points;
            if (points.ContainsKey(PlacementStyle.EDGE_TAP)) vector2value = points[PlacementStyle.EDGE_TAP][0];
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
        public Dictionary<PlacementStyle, Rect> areasByPlacement;

        public OnboardingTask_ShowMaskedArea(Dictionary<PlacementStyle, Rect> areas, OnboardingScreenMaskObject.MaskStyle maskStyle)
        {
            action = OnboardingActions.SHOW_MASKED_AREA;

            intValue = (int)maskStyle;
            this.areas = new Rect[] { areas.Values.First() };
            areasByPlacement = areas;
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
        public MessageData messageData;

        public OnboardingTask_HighlightProgressionEvent(int index, Vector2 scale, MessageData messageData, bool showBG = true)
        {
            action = OnboardingActions.HIGHLIGHT_PROGRESSION_EVENT;

            intValue = index;
            vector2value = scale;

            this.messageData = messageData;
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
        public string menuName;
        public bool showBG;
        public Vector2 pointerOffset;
        public Vector2 maskOffset;
        public MessageData messageData;

        public OnboardingTask_HighlightButton(
            string buttonName,
            string menuName,
            Vector2 scale,
            Vector2 maskOffset,
            Vector2 pointerOffset,
            MessageData messagePositionData = null,
            bool showBG = true)
        {
            action = OnboardingActions.HIGHLIGHT_CURRENT_SCREEN_BUTTON;

            stringValue = buttonName;
            vector2value = scale;

            this.maskOffset = maskOffset;
            this.pointerOffset = pointerOffset;
            this.messageData = messagePositionData;
            this.menuName = menuName;
            this.showBG = showBG;
        }

        public void TrySetMessagePivot(Vector2 pivot)
        {
            if (messageData == null || messageData.pivot != Vector2.zero) return;

            messageData.pivot = pivot;
        }
    }

    public class MessageData
    {
        public string message;
        public Vector2 pivot;
        public Vector2 positionOffset;

        public MessageData(string message, Vector2 pivot, Vector2 positionOffset)
        {
            this.message = message;
            this.pivot = pivot;
            this.positionOffset = positionOffset;
        }
    }
}
