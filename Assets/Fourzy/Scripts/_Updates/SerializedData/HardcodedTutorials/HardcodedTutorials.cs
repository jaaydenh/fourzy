//@vadym udod

using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    public class HardcodedTutorials
    {
        public static List<GameContentManager.Tutorial> tutorials = new List<GameContentManager.Tutorial>()
        {
            new GameContentManager.Tutorial()
            {
                data = new OnboardingDataHolder()
                {
                    tutorialName = "Onboarding",
                    batches = new OnboardingDataHolder.OnboardingTasksBatch[]
                    {
                        //step 1: welcome message
                        new OnboardingDataHolder.OnboardingTasksBatch()
                        {
                            tasks = new OnboardingDataHolder.OnboardingTask[]
                            {
                                new OnboardingDataHolder.OnboardingTask_OpenGame(GameType.ONBOARDING, "200"),
                                new OnboardingDataHolder.OnboardingTask_ShowMessage("Welcome to Fourzy!"),
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.WIZARD_CENTER },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.SHOW_BG },
                                new OnboardingDataHolder.OnboardingTask_Log("1"),
                            },
                        },
                        //step 2: load tutorial map + show rules
                        new OnboardingDataHolder.OnboardingTasksBatch()
                        {
                            tasks = new OnboardingDataHolder.OnboardingTask[]
                            {
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_BG },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_WIZARD },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_MESSAGE_BOX },

                                new OnboardingDataHolder.OnboardingTask_OpenGame(GameType.ONBOARDING, "201"),

                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.SHOW_BG },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.WIZARD_CENTER },
                                new OnboardingDataHolder.OnboardingTask_ShowMessage("Tap on the perimeter squares to make a move"),
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.SHOW_BOARD_HINT_AREA },
                                new OnboardingDataHolder.OnboardingTask_Log("2"),
                            },
                        },
                        //step 3: show player where to tap to make move
                        new OnboardingDataHolder.OnboardingTasksBatch()
                        {
                            tasks = new OnboardingDataHolder.OnboardingTask[]
                            {
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_BG },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_BOARD_HINT_AREA },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_WIZARD },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_MESSAGE_BOX },
                                new OnboardingDataHolder.OnboardingTask_PointAt(new Vector2(0, 6)),
                                new OnboardingDataHolder.OnboardingTask_LimitInput(new Rect(0f, 6f, 1f, 1f)),
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.SHOW_BOARD_HINT_AREA },
                                new OnboardingDataHolder.OnboardingTask_ShowMaskedArea(new Rect(0f, 6f, 1f, 1f)),
                                //this will skip to next
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.ON_MOVE_STARTED },
                                new OnboardingDataHolder.OnboardingTask_Log("3"),
                            },
                        },
                        //step 4: cancel input after player move started
                        new OnboardingDataHolder.OnboardingTasksBatch()
                        {
                            tasks = new OnboardingDataHolder.OnboardingTask[]
                            {
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_POINTER },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_BOARD_HINT_AREA },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_MAKSED_AREA },
                                new OnboardingDataHolder.OnboardingTask_LimitInput(new Rect[0]),
                                //this will skip to next
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.ON_MOVE_ENDED },
                            },
                        },
                        //step 5: opponent takes turn
                        new OnboardingDataHolder.OnboardingTasksBatch()
                        {
                            tasks = new OnboardingDataHolder.OnboardingTask[]
                            {
                                new OnboardingDataHolder.OnboardingTask_PlaceGamepiece(2, FourzyGameModel.Model.Direction.RIGHT, 3),
                                //this will skip to next
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.ON_MOVE_ENDED },
                            },
                        },
                        //step 6: players' 2d turn + limit input
                        new OnboardingDataHolder.OnboardingTasksBatch()
                        {
                            tasks = new OnboardingDataHolder.OnboardingTask[]
                            {
                                //this will skip to next
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.ON_MOVE_STARTED },
                                new OnboardingDataHolder.OnboardingTask_PointAt(new Vector2(7, 5)),
                                new OnboardingDataHolder.OnboardingTask_LimitInput(new Rect(7f, 5f, 1f, 1f)),
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.SHOW_BOARD_HINT_AREA },
                                new OnboardingDataHolder.OnboardingTask_ShowMaskedArea(new Rect(7f, 5f, 1f, 1f)),
                                new OnboardingDataHolder.OnboardingTask_Log("4"),
                            },
                        },
                        //step 7: cancel input after player move started
                        new OnboardingDataHolder.OnboardingTasksBatch()
                        {
                            tasks = new OnboardingDataHolder.OnboardingTask[]
                            {
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_POINTER },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_BOARD_HINT_AREA },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_MAKSED_AREA },
                                new OnboardingDataHolder.OnboardingTask_LimitInput(new Rect[0]),
                                //this will skip to next
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.ON_MOVE_ENDED },
                            },
                        },
                        //step 8: opponent takes turn
                        new OnboardingDataHolder.OnboardingTasksBatch()
                        {
                            tasks = new OnboardingDataHolder.OnboardingTask[]
                            {
                                new OnboardingDataHolder.OnboardingTask_PlaceGamepiece(2, FourzyGameModel.Model.Direction.DOWN, 4),
                                //this will skip to next
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.ON_MOVE_ENDED },
                            },
                        },
                        //step 9: show message
                        new OnboardingDataHolder.OnboardingTasksBatch()
                        {
                            tasks = new OnboardingDataHolder.OnboardingTask[]
                            {
                                new OnboardingDataHolder.OnboardingTask_ShowMessage("Get 4 in a row to Win!"),
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.SHOW_BG },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.WIZARD_CENTER },
                                new OnboardingDataHolder.OnboardingTask_Log("5"),
                            },
                        },
                        //step 10: players' 3rd turn + limit input
                        new OnboardingDataHolder.OnboardingTasksBatch()
                        {
                            tasks = new OnboardingDataHolder.OnboardingTask[]
                            {
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_MESSAGE_BOX },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_WIZARD },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_BG },
                                new OnboardingDataHolder.OnboardingTask_PointAt(new Vector2(3, 0)),
                                new OnboardingDataHolder.OnboardingTask_LimitInput(new Rect(3f, 0f, 1f, 1f)),
                                new OnboardingDataHolder.OnboardingTask_ShowMaskedArea(new Rect(3f, 0f, 1f, 1f)),
                                //this will skip to next
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.ON_MOVE_STARTED },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.SHOW_BOARD_HINT_AREA },
                                new OnboardingDataHolder.OnboardingTask_Log("6"),
                            },
                        },
                        //step 11: cancel input after player move started
                        new OnboardingDataHolder.OnboardingTasksBatch()
                        {
                            tasks = new OnboardingDataHolder.OnboardingTask[]
                            {
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_POINTER },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_BOARD_HINT_AREA },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_MAKSED_AREA },
                                new OnboardingDataHolder.OnboardingTask_LimitInput(new Rect[0]),
                                //this will skip to next
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.ON_MOVE_ENDED },
                            },
                        },
                        //step 12: show message
                        new OnboardingDataHolder.OnboardingTasksBatch()
                        {
                            tasks = new OnboardingDataHolder.OnboardingTask[]
                            {
                                new OnboardingDataHolder.OnboardingTask_ShowMessage("Good job!"),
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.SHOW_BG },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.WIZARD_CENTER },
                                new OnboardingDataHolder.OnboardingTask_Log("7"),
                            },
                        },
                        // //step 13: change name message
                        // new OnboardingDataHolder.OnboardingTasksBatch()
                        // {
                        //     tasks = new OnboardingDataHolder.OnboardingTask[]
                        //     {
                        //         new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.SKIP_TO_NEXT_IF_DEMO_MODE },
                        //         new OnboardingDataHolder.OnboardingTask_ShowMessage("Now lets change your name!"),
                        //         new OnboardingDataHolder.OnboardingTask_Log("8"),
                        //     },
                        // },
                        // //step 14: open changename prompt
                        // new OnboardingDataHolder.OnboardingTasksBatch()
                        // {
                        //     tasks = new OnboardingDataHolder.OnboardingTask[]
                        //     {
                        //         new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.SKIP_TO_NEXT_IF_DEMO_MODE },
                        //         new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.USER_CHANGE_NAME_PROMPT },
                        //         new OnboardingDataHolder.OnboardingTask_Log("9"),
                        //     },
                        // },
                        new OnboardingDataHolder.OnboardingTasksBatch()
                        {
                            tasks = new OnboardingDataHolder.OnboardingTask[]
                            {
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_MESSAGE_BOX },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_WIZARD },
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.HIDE_BG },
                                //will continue after main menu loaded
                                new OnboardingDataHolder.OnboardingTask() { action = OnboardingDataHolder.OnboardingActions.LOAD_MAIN_MENU },
                                //new OnboardingDataHolder.OnboardingTask_ExecMenuEvent(Constants.MAIN_MENU_CANVAS_NAME, "openScreen", "tutorialProgressionMap"),
                                //point at adventure button button
                                new OnboardingDataHolder.OnboardingTask_HighlightButton("AdventureButton"),
                                //will continue after event tap
                                new OnboardingDataHolder.OnboardingTask_HighlightProgressionEvent (-1),
                            },
                        },
                    },
                },
            },

        };

        public static void Initialize() => tutorials.ForEach(tutorial => tutorial.Initialize());
    }
}
