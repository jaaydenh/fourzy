//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class OnboardingScreen : MenuScreen
    {
        public static bool isActive = false;

        public OnboardingScreenMask masks;
        public OnboardingScreenBG bg;
        public OnboardingScreenDialog dialog;
        public OnboardingScreenPointer pointer;
        public OnboardingScreenHighlight highlight;
        public OnboardingScreenGraphics graphics;

        [NonSerialized]
        public GameContentManager.Tutorial tutorial;
        private int step;

        public override void Open()
        {
            //name prompt was closed, so we do next
            if (IsCurrentBatchContains(OnboardingDataHolder.OnboardingActions.USER_CHANGE_NAME_PROMPT)) StartRoutine("username changed", .2f, () => { Next(); });

            if (isOpened) return;

            base.Open();

            isActive = true;

            UserManager.OnUpdateUserInfo += UserManagerOnUpdateName;

            GamePlayManager.onMoveStarted += MoveStarted;
            GamePlayManager.onMoveEnded += MoveEnded;
            GamePlayManager.onGameFinished += OnGameFinished;
        }

        public override void Close(bool animate)
        {
            base.Close(animate);

            isActive = false;

            UserManager.OnUpdateUserInfo -= UserManagerOnUpdateName;

            GamePlayManager.onMoveStarted -= MoveStarted;
            GamePlayManager.onMoveEnded -= MoveEnded;
            GamePlayManager.onGameFinished -= OnGameFinished;

            //scenario finished
            switch (tutorial.data.onFinished)
            {
                case OnboardingDataHolder.OnFinished.LOAD_MAIN_MENU:
                    switch (tutorial.data.openScreen)
                    {
                        case OnboardingDataHolder.OpenScreen.PUZZLES_SCREEN:
                            //add menu event
                            MenuController.AddMenuEvent(Constants.MAIN_MENU_CANVAS_NAME, new KeyValuePair<string, object>("openScreen", "puzzlesScreen"));

                            break;
                    }

                    if (GameManager.Instance.activeGame != null)
                        GamePlayManager.instance.BackButtonOnClick();
                    else
                        GameManager.Instance.OpenMainMenu();

                    return;
            }
        }

        public override void OnBack()
        {
            base.OnBack();

            menuController.GetScreen<PromptScreen>().Prompt("Quit tutorial?", "Leave tutorial level?\nYou can reset tutorial in Options Screen.", () =>
            {
                //close prompt
                menuController.CloseCurrentScreen();
                //close onboarding
                menuController.CloseCurrentScreen();
            });
        }

        public void OpenOnboarding(GameContentManager.Tutorial tutorial)
        {
            if (!WillDisplayTutorial(tutorial))
                return;

            this.tutorial = tutorial;
            menuController.OpenScreen(this);

            PlayerPrefsWrapper.SetTutorialOpened(tutorial.data, true);

            StartCoroutine(DisplayCurrentStep());
        }

        public bool WillDisplayTutorial(GameContentManager.Tutorial tutorial)
        {
            if (!GameManager.Instance.displayTutorials) return false;

            if (tutorial == null || ((PlayerPrefsWrapper.GetTutorialOpened(tutorial.data) || PlayerPrefsWrapper.GetTutorialFinished(tutorial.data)) && !GameManager.Instance.forceDisplayTutorials))
                return false;

            return true;
        }

        public void Next()
        {
            step++;

            StartCoroutine(DisplayCurrentStep());
        }

        public bool IsCurrentBatchContains(OnboardingDataHolder.OnboardingActions action)
        {
            OnboardingDataHolder.OnboardingTasksBatch batch = tutorial.data.batches[step];

            foreach (OnboardingDataHolder.OnboardingTask task in batch.tasks)
                if (task.action == action)
                    return true;

            return false;
        }

        private void UserManagerOnUpdateName()
        {
            OnboardingDataHolder.OnboardingTasksBatch batch = tutorial.data.batches[step];

            foreach (OnboardingDataHolder.OnboardingTask task in batch.tasks)
            {
                switch (task.action)
                {
                    case OnboardingDataHolder.OnboardingActions.USER_CHANGE_NAME_PROMPT:

                        break;
                }
            }
        }

        private void MoveEnded(int playerID)
        {
            OnboardingDataHolder.OnboardingTasksBatch batch = tutorial.data.batches[step];

            foreach (OnboardingDataHolder.OnboardingTask task in batch.tasks)
            {
                switch (task.action)
                {
                    case OnboardingDataHolder.OnboardingActions.ON_PLAYER1_MOVE_ENDED:
                        if (playerID != (int)PlayerEnum.ONE)
                            return;

                        switch (task.nextAction)
                        {
                            case OnboardingDataHolder.NextAction.NEXT:
                                StartRoutine("moveFinishedNext", .5f, () => { Next(); });
                                break;
                        }
                        break;

                    case OnboardingDataHolder.OnboardingActions.ON_PLAYER2_MOVE_ENDED:
                        if (playerID != (int)PlayerEnum.TWO)
                            return;

                        switch (task.nextAction)
                        {
                            case OnboardingDataHolder.NextAction.NEXT:
                                StartRoutine("moveFinishedNext", .5f, () => { Next(); });
                                break;
                        }
                        break;
                }
            }
        }

        private void MoveStarted(int playerID)
        {
            OnboardingDataHolder.OnboardingTasksBatch batch = tutorial.data.batches[step];

            foreach (OnboardingDataHolder.OnboardingTask task in batch.tasks)
            {
                switch (task.action)
                {
                    case OnboardingDataHolder.OnboardingActions.ON_PLAYER1_MOVE_STARTED:
                        if (playerID != (int)PlayerEnum.ONE)
                            return;

                        switch (task.nextAction)
                        {
                            case OnboardingDataHolder.NextAction.NEXT:
                                StartRoutine("moveFinishedNext", .5f, () => { Next(); });
                                break;
                        }
                        break;

                    case OnboardingDataHolder.OnboardingActions.ON_PLAYER2_MOVE_STARTED:
                        if (playerID != (int)PlayerEnum.TWO)
                            return;

                        switch (task.nextAction)
                        {
                            case OnboardingDataHolder.NextAction.NEXT:
                                StartRoutine("moveFinishedNext", .5f, () => { Next(); });
                                break;
                        }
                        break;
                }
            }
        }

        private void OnGameFinished(IClientFourzy game)
        {
            OnboardingDataHolder.OnboardingTasksBatch batch = tutorial.data.batches[step];

            foreach (OnboardingDataHolder.OnboardingTask task in batch.tasks)
                switch (task.action)
                {
                    case OnboardingDataHolder.OnboardingActions.PLAY_INITIAL_MOVES:
                        switch (task.onGameFinished)
                        {
                            case OnboardingDataHolder.OnGameFinished.CONTINUE:
                                StartRoutine("gameFinishedNext", 2f, () => { Next(); });
                                break;
                        }
                        break;
                }
        }

        public IEnumerator DisplayCurrentStep()
        {
            if (step >= tutorial.data.batches.Length)
            {
                //close onboarding
                menuController.CloseCurrentScreen();
                PlayerPrefsWrapper.SetTutorialState(tutorial.data, true);

                yield return null;
                yield break;
            }

            OnboardingDataHolder.OnboardingTasksBatch batch = tutorial.data.batches[step];

            IClientFourzy activeGame = GameManager.Instance.activeGame;

            foreach (OnboardingDataHolder.OnboardingTask task in batch.tasks)
            {
                switch (task.action)
                {
                    //dialog
                    case OnboardingDataHolder.OnboardingActions.SHOW_MESSAGE:
                        if (!dialog.visible)
                            dialog.Show(.2f);

                        dialog.DisplayText(task.message);
                        break;

                    case OnboardingDataHolder.OnboardingActions.HIDE_MESSAGE_BOX:
                        dialog.Hide(.2f);
                        break;

                    //pointer
                    case OnboardingDataHolder.OnboardingActions.POINT_AT:
                        if (!pointer.visible)
                            pointer.Show(.2f);

                        BoardLocation pointLocation = new BoardLocation((int)task.pointAt.y, (int)task.pointAt.x);

                        pointer.PointAt(pointLocation);
                        break;

                    case OnboardingDataHolder.OnboardingActions.HIDE_POINTER:
                        pointer.Hide(.2f);
                        break;

                    //highlight
                    case OnboardingDataHolder.OnboardingActions.HIGHLIGHT:
                        if (!highlight.visible)
                            highlight.Show(.2f);

                        highlight.ShowHighlight(task.areas);
                        break;

                    case OnboardingDataHolder.OnboardingActions.HIDE_HIGHLIGHT:
                        highlight.Hide(.2f);
                        break;

                    case OnboardingDataHolder.OnboardingActions.SHOW_BOARD_HINT_AREA:
                        GamePlayManager.instance.board.SetHintAreaSelectableState(false);
                        GamePlayManager.instance.board.ShowHintArea(Mechanics.Board.GameboardView.HintAreaStyle.ANIMATION_LOOP, Mechanics.Board.GameboardView.HintAreaAnimationPattern.DIAGONAL);
                        yield return null;

                        break;

                    case OnboardingDataHolder.OnboardingActions.HIDE_BOARD_HINT_AREA:
                        GamePlayManager.instance.board.SetHintAreaSelectableState(true);
                        GamePlayManager.instance.board.HideHintArea(Mechanics.Board.GameboardView.HintAreaAnimationPattern.DIAGONAL);
                        yield return null;

                        break;

                    case OnboardingDataHolder.OnboardingActions.SHOW_MASKED_AREA:
                        masks.ShowMasks(task.areas);

                        break;

                    case OnboardingDataHolder.OnboardingActions.HIDE_MAKSED_AREA:
                        masks.HideMasks();

                        break;

                    //board input
                    case OnboardingDataHolder.OnboardingActions.LIMIT_BOARD_INPUT:
                        GamePlayManager.instance.board.LimitInput(task.areas);

                        break;

                    case OnboardingDataHolder.OnboardingActions.RESET_BOARD_INPUT:
                        GamePlayManager.instance.board.SetInputMap(true);

                        break;

                    //bg
                    case OnboardingDataHolder.OnboardingActions.HIDE_BG:
                        bg.Hide(.2f);
                        break;

                    case OnboardingDataHolder.OnboardingActions.SHOW_BG:
                        bg.Show(.2f);
                        break;

                    case OnboardingDataHolder.OnboardingActions.PLAY_INITIAL_MOVES:
                        if (GamePlayManager.instance) GamePlayManager.instance.board.TryPlayInitialMoves();

                        break;

                    case OnboardingDataHolder.OnboardingActions.OPEN_GAME:
                        GameManager.Instance.StartGame(
                            new ClientFourzyGame(
                                GameContentManager.Instance.GetMiscBoard(task.intValue + ""), 
                                UserManager.Instance.meAsPlayer, 
                                new Player(2, "Player Two"))
                            { _Type = GameType.ONBOARDING, hideOpponent = !tutorial.data.showPlayer2 });

                        break;

                    case OnboardingDataHolder.OnboardingActions.LOG_TUTORIAL:
                        AnalyticsManager.Instance.LogTutorialEvent(tutorial.data.tutorialName, task.message);

                        break;

                    //wizard
                    case OnboardingDataHolder.OnboardingActions.HIDE_WIZARD:
                        graphics.SetWizardState(false);

                        break;

                    case OnboardingDataHolder.OnboardingActions.WIZARD_CENTER:
                        graphics.SetWizardState(true);

                        break;

                    //make player1 move
                    case OnboardingDataHolder.OnboardingActions.PLAYER_1_PLACE_GAMEPIECE:
                        GamePlayManager.instance.board.TakeTurn(task.direction, task.intValue, true);

                        break;

                    //make player2 move
                    case OnboardingDataHolder.OnboardingActions.PLAYER_2_PLACE_GAMEPIECE:
                        GamePlayManager.instance.board.TakeTurn(task.direction, task.intValue, true);

                        break;

                    //user change name
                    case OnboardingDataHolder.OnboardingActions.USER_CHANGE_NAME_PROMPT:
                        menuController.GetScreen<ChangeNamePromptScreen>().Prompt("Change Name", "Current name: " + UserManager.Instance.userName, () => { menuController.CloseCurrentScreen(); });

                        break;

                    case OnboardingDataHolder.OnboardingActions.SKIP_TO_NEXT_IF_DEMO_MODE:
                        if (SettingsManager.Instance.Get(SettingsManager.KEY_DEMO_MODE))
                        {
                            Next();
                            yield break;
                        }

                        break;
                }
            }

            yield return null;
        }
    }
}