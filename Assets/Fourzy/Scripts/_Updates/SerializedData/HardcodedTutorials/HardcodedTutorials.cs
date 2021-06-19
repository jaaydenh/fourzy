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
                    new OnboardingTask_Log("Tutorial Step 1"),

                    new OnboardingTask() { action = OnboardingActions.SET_BACK_BUTTON_STATE, boolValue = true },
                    new OnboardingTask_OpenGame(GameType.ONBOARDING, GameMode.NONE, "OnboardingBoard0"),
                    new OnboardingTask() { action = OnboardingActions.SHOW_GRAPHICS, vector2value = new Vector2(.5f, .75f) },
                    new OnboardingTask_ShowBG(),
                    new OnboardingTask_ShowMiniboard(new Vector2(.5f, .35f), "TutorialBoard_01"),
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("instruction_01"), new Vector2(.5f, .57f), 32f),

                    new OnboardingTask() { action = OnboardingActions.HIDE_GRAPHICS },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MINIBOARD },
                    new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },

                    new OnboardingTask_PointAt(new Dictionary<PlacementStyle, Vector2[]>() {
                        [PlacementStyle.SWIPE_STYLE_2] = new Vector2[] { new Vector2(4f, 0f), new Vector2(4f, 2f) },
                        [PlacementStyle.EDGE_TAP] = new Vector2[] { new Vector2(4f, 0f) },
                    }),
                    new OnboardingTask_LimitInput(new Rect(4f, 0f, 1f, 1f)),
                    new OnboardingTask_ShowMaskedBoardCells(new Dictionary<PlacementStyle, Rect>() {
                        [PlacementStyle.SWIPE_STYLE_2] = new Rect(4f, 0f, 1f, 3f),
                        [PlacementStyle.EDGE_TAP] = new Rect(4f, 0f, 1f, 1f),
                    }, OnboardingScreenMaskObject.MaskStyle.PX_0),
                    new OnboardingTask_ShowBubbleMessage(LocalizationManager.Value("instruction_02"), new Vector2(.5f, .15f)),
                    new OnboardingTask() { action = OnboardingActions.ON_MOVE_STARTED },

                    new OnboardingTask_Log("Tutorial Step 2 part a"),

                    new OnboardingTask() { action = OnboardingActions.PAUSE_BOARD },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                    new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                    new OnboardingTask_Wait (.5f),
                    new OnboardingTask_LimitInput(new Rect(0f, 0f, 1f, 1f)),
                    new OnboardingTask_ShowBubbleMessage(LocalizationManager.Value("instruction_03"), new Vector2(.5f, .15f)),
                    new OnboardingTask_ShowBG(),
                    new OnboardingTask() { action = OnboardingActions.HIGHLIGHT_GAMPIECES },
                    new OnboardingTask_Wait (-1f),

                    new OnboardingTask_Log("Tutorial Step 2 part b"),

                    new OnboardingTask() { action = OnboardingActions.HIDE_BUBBLE_MESSAGE },
                    new OnboardingTask() { action = OnboardingActions.RESUME_BOARD },
                    new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                    new OnboardingTask() { action = OnboardingActions.HIDE_GAMEPIECES },
                    new OnboardingTask_Wait (.5f),

                    new OnboardingTask_Log("Tutorial Step 3"),

                    new OnboardingTask_ShowBubbleMessage(LocalizationManager.Value("instruction_04"), new Vector2(.5f, .15f)),
                    new OnboardingTask_ShowMaskedArea(
                        Constants.GAMEPLAY_MENU_CANVAS_NAME, 
                        "Player2UIWidget", 
                        new Vector2(315f, 130f),
                        new Vector2(65f, 20f),
                        OnboardingScreenMaskObject.MaskStyle.PX_16, 
                        true),
                    new OnboardingTask_Wait (-1f),

                    new OnboardingTask_Log("Tutorial Step 4"),
                    new OnboardingTask_ShowBubbleMessage(LocalizationManager.Value("instruction_05"), new Vector2(.5f, .15f)),

                    new OnboardingTask_Wait (-1f),
                    new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                    new OnboardingTask_Wait (.4f),
                    new OnboardingTask_PlaceGamepiece(2, Direction.RIGHT, 3),
                    new OnboardingTask() { action = OnboardingActions.ON_MOVE_ENDED },
                    new OnboardingTask() { action = OnboardingActions.HIDE_BUBBLE_MESSAGE },
                    new OnboardingTask_Wait (1.5f),

                    new OnboardingTask_Log("Tutorial Step 5"),

                    new OnboardingTask_OpenGame(GameType.ONBOARDING, GameMode.NONE, "OnboardingBoard1"),
                    new OnboardingTask_ShowBubbleMessage(LocalizationManager.Value("instruction_06"), new Vector2(.5f, .15f)),
                    new OnboardingTask_PointAt(new Dictionary<PlacementStyle, Vector2[]>() {
                        [PlacementStyle.SWIPE_STYLE_2] = new Vector2[] { new Vector2(7f, 4f), new Vector2(5f, 4f) },
                        [PlacementStyle.EDGE_TAP] = new Vector2[] { new Vector2(7f, 4f) },
                    }),
                    new OnboardingTask_LimitInput(new Rect(7f, 4f, 1f, 1f)),
                    new OnboardingTask_ShowMaskedBoardCells(new Dictionary<PlacementStyle, Rect>() {
                        [PlacementStyle.SWIPE_STYLE_2] = new Rect(0f, 4f, 8f, 1f),
                        [PlacementStyle.EDGE_TAP] = new Rect(0f, 4f, 8f, 1f),
                    }, OnboardingScreenMaskObject.MaskStyle.PX_0),
                    new OnboardingTask() { action = OnboardingActions.ON_MOVE_ENDED },
                    new OnboardingTask_Wait (2f),
                    new OnboardingTask() { action = OnboardingActions.HIDE_BUBBLE_MESSAGE },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
                    new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
                    new OnboardingTask_Wait (.5f),
                    new OnboardingTask() { action = OnboardingActions.SHOW_GRAPHICS, vector2value = new Vector2(.5f, .75f) },
                    new OnboardingTask_ShowBG(),

                    new OnboardingTask_Log("Tutorial Step 6"),

                    new OnboardingTask_ShowMiniboard(new Vector2(.5f, .35f), "TutorialBoard_02", "TutorialBoard_03", "TutorialBoard_04"),
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("instruction_07"), new Vector2(.5f, .57f), 32f),
                    new OnboardingTask() { action = OnboardingActions.HIDE_MINIBOARD },
                    new OnboardingTask_ShowMessage(LocalizationManager.Value("instruction_08"), new Vector2(.5f, .57f), 32f),

                    new OnboardingTask_Log("Tutorial Step 7"),

                    new OnboardingTask() { action = OnboardingActions.HIDE_BG },
                    new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
                    new OnboardingTask() { action = OnboardingActions.HIDE_GRAPHICS },
                    new OnboardingTask() { action = OnboardingActions.SET_BACK_BUTTON_STATE, boolValue = false },
                    new OnboardingTask_OpenGame(GameType.ONBOARDING, GameMode.VERSUS, "OnboardingBoard2", new Player(2, "Triangry", AIProfile.EasyAI) { HerdId = "triangry" }),

                    new OnboardingTask() { action = OnboardingActions.GAME_FINISHED },
                    new OnboardingTask_Log("Tutorial Step 8"),
                },
            },

            //new Tutorial()
            //{
            //    name = "OnboardingLandscape",
            //    onBack = TutorialOnBack.SHOW_LEAVE_PROMPT,
            //    tasks = new OnboardingTask[]
            //    {
            //        new OnboardingTask_OpenGame(GameType.ONBOARDING, "200"),
            //        new OnboardingTask() { action = OnboardingActions.SHOW_GRAPHICS },
            //        new OnboardingTask() { action = OnboardingActions.SHOW_BG },
            //        new OnboardingTask_ShowMessage(LocalizationManager.Value("welcome_to_fourzy"), new Vector2(.5f, .75f)),
            //        new OnboardingTask_Log("1"),

            //        new OnboardingTask_ShowMessage(LocalizationManager.Value("get_4_to_win"), new Vector2(.5f, .75f)),

            //        new OnboardingTask() { action = OnboardingActions.HIDE_GRAPHICS },
            //        new OnboardingTask() { action = OnboardingActions.HIDE_BG },
            //        new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },

            //        new OnboardingTask_OpenGame(GameType.ONBOARDING, "onboarding1"),

            //        new OnboardingTask_PointAt(new Dictionary<PlacementStyle, Vector2[]>() {
            //            [PlacementStyle.SWIPE_STYLE_2] = new Vector2[] { new Vector2(4f, 0f), new Vector2(4f, 2f) },
            //            [PlacementStyle.EDGE_TAP] = new Vector2[] { new Vector2(4f, 0f) },
            //        }),
            //        new OnboardingTask_ShowMessage(LocalizationManager.Value("rule_tap_2"), new Vector2(.5f, .15f)),
            //        new OnboardingTask_LimitInput(new Rect(4f, 0f, 1f, 1f)),
            //        new OnboardingTask_ShowMaskedBoardCells(new Dictionary<PlacementStyle, Rect>() {
            //            [PlacementStyle.SWIPE_STYLE_2] = new Rect(4f, 0f, 1f, 3f),
            //            [PlacementStyle.EDGE_TAP] = new Rect(4f, 0f, 1f, 1f),
            //        }, OnboardingScreenMaskObject.MaskStyle.PX_0),

            //        new OnboardingTask() { action = OnboardingActions.ON_MOVE_STARTED },
            //        new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
            //        new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
            //        new OnboardingTask_Log("2"),

            //        new OnboardingTask() { action = OnboardingActions.GAME_FINISHED },
            //        new OnboardingTask() { action = OnboardingActions.SHOW_BG, intValue = -1},
            //        new OnboardingTask_ShowBubbleMessage(LocalizationManager.Value("win_vertical_rule"), new Vector2(.5f, .15f), 3f),

            //        new OnboardingTask() { action = OnboardingActions.HIDE_BG },
            //        new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
            //        new OnboardingTask_Wait(1f),

            //        new OnboardingTask_OpenGame(GameType.ONBOARDING, "onboarding2"),

            //        new OnboardingTask_PointAt(new Dictionary<PlacementStyle, Vector2[]>() {
            //            [PlacementStyle.SWIPE_STYLE_2] = new Vector2[] { new Vector2(0f, 3f), new Vector2(2f, 3f) },
            //            [PlacementStyle.EDGE_TAP] = new Vector2[] { new Vector2(0f, 3f) },
            //        }),
            //        new OnboardingTask_ShowBubbleMessage(LocalizationManager.Value("rule_tap_2"), new Vector2(.5f, .15f)),
            //        new OnboardingTask_LimitInput(new Rect(0f, 3f, 3f, 1f)),
            //        new OnboardingTask_ShowMaskedBoardCells(new Dictionary<PlacementStyle, Rect>() {
            //            [PlacementStyle.SWIPE_STYLE_2] = new Rect(0f, 3f, 3f, 1f),
            //            [PlacementStyle.EDGE_TAP] = new Rect(0f, 3f, 1f, 1f),
            //        }, OnboardingScreenMaskObject.MaskStyle.PX_0),

            //        new OnboardingTask() { action = OnboardingActions.ON_MOVE_STARTED },
            //        new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
            //        new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
            //        new OnboardingTask_Log("3"),

            //        new OnboardingTask() { action = OnboardingActions.GAME_FINISHED },
            //        new OnboardingTask() { action = OnboardingActions.SHOW_BG, intValue = -1},
            //        new OnboardingTask_ShowBubbleMessage(LocalizationManager.Value("win_horizontal_rule"), new Vector2(.5f, .15f), 3f),

            //        new OnboardingTask() { action = OnboardingActions.HIDE_BG },
            //        new OnboardingTask() { action = OnboardingActions.HIDE_MESSAGE_BOX },
            //        new OnboardingTask_Wait(1f),

            //        new OnboardingTask_OpenGame(GameType.ONBOARDING, "onboarding3"),

            //        new OnboardingTask_PointAt(new Dictionary<PlacementStyle, Vector2[]>(){
            //            [PlacementStyle.SWIPE_STYLE_2] = new Vector2[] { new Vector2(0f, 1f), new Vector2(2f, 1f) },
            //            [PlacementStyle.EDGE_TAP] = new Vector2[] { new Vector2(0f, 1f) },
            //        }),
            //        new OnboardingTask_ShowBubbleMessage(LocalizationManager.Value("rule_tap_3"), new Vector2(.5f, .15f)),
            //        new OnboardingTask_LimitInput(new Rect(0f, 1f, 3f, 1f)),
            //        new OnboardingTask_ShowMaskedBoardCells(new Dictionary<PlacementStyle, Rect>() {
            //            [PlacementStyle.SWIPE_STYLE_2] = new Rect(0f, 1f, 3f, 1f),
            //            [PlacementStyle.EDGE_TAP] = new Rect(0f, 1f, 1f, 1f),
            //        }, OnboardingScreenMaskObject.MaskStyle.PX_0),

            //        new OnboardingTask() { action = OnboardingActions.ON_MOVE_STARTED },
            //        new OnboardingTask() { action = OnboardingActions.HIDE_POINTER },
            //        new OnboardingTask() { action = OnboardingActions.HIDE_MAKSED_AREA },
            //        new OnboardingTask_Log("4"),

            //        new OnboardingTask() { action = OnboardingActions.GAME_FINISHED },
            //        new OnboardingTask() { action = OnboardingActions.SHOW_BG, intValue = -1},
            //        new OnboardingTask_ShowBubbleMessage(LocalizationManager.Value("win_diagonal_rule"), new Vector2(.5f, .15f), 3f),

            //        new OnboardingTask() { action = OnboardingActions.HIDE_BG },
            //        new OnboardingTask_Wait(1f),
            //        new OnboardingTask() { action = OnboardingActions.LOAD_MAIN_MENU },
            //    },
            //},

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

        SHOW_BUBBLE_MESSAGE,
        HIDE_BUBBLE_MESSAGE,

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

        HIDE_GRAPHICS,
        SHOW_GRAPHICS,

        PLAYER_2_PLACE_GAMEPIECE,
        PLAYER_1_PLACE_GAMEPIECE,

        ON_MOVE_ENDED,
        ON_MOVE_STARTED,

        USER_CHANGE_NAME_PROMPT,

        SHOW_BOARD_HINT_AREA,
        HIDE_BOARD_HINT_AREA,

        LOG_TUTORIAL,
        WAIT,

        SHOW_MASKED_BOARD_CELL,
        SHOW_MASKED_AREA,
        HIDE_MAKSED_AREA,

        SKIP_TO_NEXT_IF_DEMO_MODE,

        SHOW_OPPONENT,
        HIDE_OPPONENT,

        LOAD_MAIN_MENU,
        EXEC_MENU_EVENT,

        HIGHLIGHT_PROGRESSION_EVENT,
        HIGHLIGHT_CURRENT_SCREEN_BUTTON,

        SHOW_MINIBOARD,
        HIDE_MINIBOARD,

        PAUSE_BOARD,
        RESUME_BOARD,

        HIGHLIGHT_GAMPIECES,
        HIDE_GAMEPIECES,

        SET_BACK_BUTTON_STATE,
    }

    public enum TutorialOnBack
    {
        SHOW_LEAVE_PROMPT,
        IGNORE,
    }

    public class OnboardingTask
    {
        public OnboardingActions action;
        public bool boolValue;

        public string stringValue;
        public int intValue;
        public Vector2 vector2value;
        public float floatValue;
        public Rect[] areas;
        public Direction direction;
        public KeyValuePair<string, object> menuEvent;

        public OnboardingTask()
        {
            areas = new Rect[] { new Rect(0, 0, 1, 1), };
        }
    }

    public class OnboardingTask_ShowBG : OnboardingTask
    {
        public bool Interactable => boolValue;

        public OnboardingTask_ShowBG(bool interactable = true)
        {
            action = OnboardingActions.SHOW_BG;

            boolValue = interactable;
        }
    }
    
    public class OnboardingTask_ShowMiniboard : OnboardingTask
    {
        public List<string> Boards = new List<string>();
        public Vector2 Anchor => vector2value;

        public OnboardingTask_ShowMiniboard(Vector2 anchor, params string[] boards)
        {
            action = OnboardingActions.SHOW_MINIBOARD;
            vector2value = anchor;
            Boards.AddRange(boards);
        }
    }

    //shortcuts
    public class OnboardingTask_OpenGame : OnboardingTask
    {
        /// <summary>
        /// Same as intValue
        /// </summary>
        public GameType Type => (GameType)intValue;
        /// <summary>
        /// Same as stringValue
        /// </summary>
        public string GameId => stringValue;
        public GameMode Mode { get; private set; }
        public Player Player;

        public OnboardingTask_OpenGame(GameType type, GameMode mode, string gameId, Player player = null)
        {
            action = OnboardingActions.OPEN_GAME;
            Mode = mode;

            intValue = (int)type;
            stringValue = gameId;
            Player = player;
        }
    }

    public class OnboardingTask_ShowMessage : OnboardingTask
    {
        /// <summary>
        /// Same as stringValue
        /// </summary>
        public string Message => stringValue;
        /// <summary>
        /// Same as floatValue
        /// </summary>
        public float SkipAfter => floatValue;

        public float FontSize;

        public OnboardingTask_ShowMessage(string message, Vector2 anchor, float fontSize = 64f, float skipAfter = -1f)
        {
            action = OnboardingActions.SHOW_MESSAGE;
            stringValue = message;
            vector2value = anchor;
            floatValue = skipAfter;
            FontSize = fontSize;
        }
    }

    public class OnboardingTask_ShowBubbleMessage : OnboardingTask_ShowMessage
    {
        public OnboardingTask_ShowBubbleMessage(string message, Vector2 anchor, float fontSize = 64f, float skipAfter = -1f) : base(message, anchor, fontSize, skipAfter)
        {
            action = OnboardingActions.SHOW_BUBBLE_MESSAGE;
        }
    }

    public class OnboardingTask_Log : OnboardingTask
    {
        /// <summary>
        /// Same as stringValue
        /// </summary>
        public string ID => stringValue;

        public OnboardingTask_Log(string id)
        {
            action = OnboardingActions.LOG_TUTORIAL;
            stringValue = id;
        }
    }

    public class OnboardingTask_PointAt : OnboardingTask
    {
        public Dictionary<PlacementStyle, Vector2[]> points;

        /// <summary>
        /// String value
        /// </summary>
        public string Message => stringValue;

        public OnboardingTask_PointAt(Dictionary<PlacementStyle, Vector2[]> points, string message = "")
        {
            action = OnboardingActions.POINT_AT;

            this.points = points;
            if (points.ContainsKey(PlacementStyle.EDGE_TAP))
            {
                vector2value = points[PlacementStyle.EDGE_TAP][0];
            }
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

    public class OnboardingTask_ShowMaskedBoardCells : OnboardingTask
    {
        public Dictionary<PlacementStyle, Rect> areasByPlacement;

        public OnboardingTask_ShowMaskedBoardCells(Dictionary<PlacementStyle, Rect> areas, OnboardingScreenMaskObject.MaskStyle maskStyle)
        {
            action = OnboardingActions.SHOW_MASKED_BOARD_CELL;

            intValue = (int)maskStyle;
            this.areas = new Rect[] { areas.Values.First() };
            areasByPlacement = areas;
        }
    }

    public class OnboardingTask_ShowMaskedArea : OnboardingTask
    {
        public Vector2 Anchor { get; private set; }
        public Vector2 Pivot { get; private set; }
        public Vector2 Offset { get; private set; }
        /// <summary>
        /// Same as vector2value
        /// </summary>
        public Vector2 Size => vector2value;
        /// <summary>
        /// Same as intValue
        /// </summary>
        public OnboardingScreenMaskObject.MaskStyle MaskStyle => (OnboardingScreenMaskObject.MaskStyle)intValue;
        /// <summary>
        /// Same as boolValue
        /// </summary>
        public bool Interactable => boolValue;
        public string Target { get; private set; }

        public OnboardingTask_ShowMaskedArea(
            Vector2 size, 
            Vector2 anchor, 
            Vector2 pivot, 
            Vector2 offset, 
            OnboardingScreenMaskObject.MaskStyle maskStyle, 
            bool interactable = false)
        {
            action = OnboardingActions.SHOW_MASKED_AREA;

            intValue = (int)maskStyle;
            vector2value = size;
            boolValue = interactable;

            Anchor = anchor;
            Pivot = pivot;
            Offset = offset;
        }

        public OnboardingTask_ShowMaskedArea(
            string menu, 
            string target, 
            Vector2 size, 
            Vector2 offset, 
            OnboardingScreenMaskObject.MaskStyle maskStyle, 
            bool interactable = false)
        {
            action = OnboardingActions.SHOW_MASKED_AREA;

            intValue = (int)maskStyle;
            vector2value = size;
            boolValue = interactable;

            stringValue = menu;
            Target = target;
            Offset = offset;
        }
    }

    public class OnboardingTask_PlaceGamepiece : OnboardingTask
    {
        /// <summary>
        /// Same as intValue
        /// </summary>
        public int Location => intValue;

        public OnboardingTask_PlaceGamepiece(int playerId, Direction direction, int location)
        {
            action = playerId == 1 ? OnboardingActions.PLAYER_1_PLACE_GAMEPIECE : OnboardingActions.PLAYER_2_PLACE_GAMEPIECE;
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
        /// <summary>
        /// Same as floatValue
        /// </summary>
        public float WaitTime => floatValue;

        public OnboardingTask_Wait(float time)
        {
            action = OnboardingActions.WAIT;
            floatValue = time;
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
