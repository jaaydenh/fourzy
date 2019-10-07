//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System;
using System.Collections;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class OnboardingScreen : MenuScreen
    {
        public static OnboardingScreen instance;

        public OnboardingScreenMask masks;
        public OnboardingScreenBG bg;
        public OnboardingScreenDialog dialog;
        public OnboardingScreenPointer pointer;
        public OnboardingScreenHighlight highlight;
        public OnboardingScreenGraphics graphics;

        public bool isTutorialRunning { get; private set; }
        public ButtonExtended currentButton { get; private set; }
        public OnboardingDataHolder.OnboardingTask currentTask { get; private set; }
        public OnboardingDataHolder.OnboardingTasksBatch currentBatch { get; private set; }

        [NonSerialized]
        public GameContentManager.Tutorial tutorial;

        private int step;

        public override void Open()
        {
            //name prompt was closed, so we do next
            if (tutorial.data.batches[step].ContainsAction(OnboardingDataHolder.OnboardingActions.USER_CHANGE_NAME_PROMPT))
                StartRoutine("nameChanged", .2f, () => Next());

            if (isOpened) return;

            base.Open();

            UserManager.OnUpdateUserInfo += UserManagerOnUpdateName;

            GamePlayManager.onMoveStarted += MoveStarted;
            GamePlayManager.onMoveEnded += MoveEnded;
            GamePlayManager.onGameFinished += OnGameFinished;
        }

        public override void Close(bool animate)
        {
            base.Close(animate);

            isTutorialRunning = false;

            RemoveCurrentButton();
            StopAllCoroutines();

            UserManager.OnUpdateUserInfo -= UserManagerOnUpdateName;

            GamePlayManager.onMoveStarted -= MoveStarted;
            GamePlayManager.onMoveEnded -= MoveEnded;
            GamePlayManager.onGameFinished -= OnGameFinished;

            //load main menu if game is opened
            if (GameManager.Instance.activeGame != null) GameManager.Instance.OpenMainMenu();
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

        public void OpenTutorial(GameContentManager.Tutorial tutorial)
        {
            instance = this;

            isTutorialRunning = true;
            this.tutorial = tutorial;
            step = 0;

            menuController.OpenScreen(this);
            masks.Hide(0);

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

        private void MoveStarted(ClientPlayerTurn turn)
        {
            if (turn == null || turn.PlayerId < 1) return;

            if (tutorial.data.batches[step].ContainsAction(OnboardingDataHolder.OnboardingActions.ON_MOVE_STARTED))
                StartRoutine("moveFinishedNext", .5f, () => Next());
        }

        private void MoveEnded(ClientPlayerTurn turn)
        {
            if (turn == null || turn.PlayerId < 1) return;

            if (tutorial.data.batches[step].ContainsAction(OnboardingDataHolder.OnboardingActions.ON_MOVE_ENDED))
                StartRoutine("moveFinishedNext", .5f, () => Next());
        }

        private void OnGameFinished(IClientFourzy game)
        {
            if (tutorial.data.batches[step].ContainsAction(OnboardingDataHolder.OnboardingActions.PLAY_INITIAL_MOVES))
                StartRoutine("gameFinishedNext", 2f, () => Next());
        }

        private void OnTap()
        {
            if (currentBatch.tasks[currentBatch.tasks.Length - 1] == currentTask)
            {
                CancelRoutine("idle");
                StartRoutine("idle", .1f, () => Next());
            }

            RemoveCurrentButton();
        }

        private void UpdateCurrentButton(ButtonExtended button)
        {
            currentButton = button;
            currentButton.onTap += OnTap;
        }

        private void RemoveCurrentButton()
        {
            if (!currentButton) return;

            currentButton.onTap -= OnTap;
            currentButton = null;
        }

        public IEnumerator DisplayCurrentStep()
        {
            if (step >= tutorial.data.batches.Length)
            {
                //close onboarding
                menuController.CloseCurrentScreen();
                PlayerPrefsWrapper.SetTutorialState(tutorial.data, true);

                yield break;
            }

            currentBatch = tutorial.data.batches[step];

            IClientFourzy activeGame = GameManager.Instance.activeGame;

            foreach (OnboardingDataHolder.OnboardingTask task in currentBatch.tasks)
            {
                currentTask = task;

                switch (task.action)
                {
                    //dialog
                    case OnboardingDataHolder.OnboardingActions.SHOW_MESSAGE:
                        if (!dialog.visible) dialog.Show(.2f);

                        dialog.DisplayText(task.stringValue);

                        break;

                    case OnboardingDataHolder.OnboardingActions.HIDE_MESSAGE_BOX:
                        dialog.Hide(.2f);

                        break;

                    //pointer
                    case OnboardingDataHolder.OnboardingActions.POINT_AT:
                        if (!pointer.visible) pointer.Show(.2f);

                        pointer.PointAt(new BoardLocation((int)task.pointAt.y, (int)task.pointAt.x));

                        break;

                    case OnboardingDataHolder.OnboardingActions.HIDE_POINTER:
                        pointer.Hide(.2f);

                        break;

                    case OnboardingDataHolder.OnboardingActions.HIGHLIGHT:
                        if (!highlight.visible) highlight.Show(.2f);

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
                        masks.Hide();

                        break;

                    case OnboardingDataHolder.OnboardingActions.LIMIT_BOARD_INPUT:
                        GamePlayManager.instance.board.LimitInput(task.areas);

                        break;

                    case OnboardingDataHolder.OnboardingActions.RESET_BOARD_INPUT:
                        GamePlayManager.instance.board.SetInputMap(true);

                        break;

                    case OnboardingDataHolder.OnboardingActions.HIDE_BG:
                        bg.Hide(.2f);

                        break;

                    case OnboardingDataHolder.OnboardingActions.SHOW_BG:
                        bg.Show(.2f);

                        break;

                    case OnboardingDataHolder.OnboardingActions.PLAY_INITIAL_MOVES:
                        if (GamePlayManager.instance) GamePlayManager.instance.board.PlayInitialMoves();

                        break;

                    case OnboardingDataHolder.OnboardingActions.OPEN_GAME:
                        //wait for game to load
                        yield return GameManager.Instance.StartGame(
                            new ClientFourzyGame(
                                GameContentManager.Instance.GetMiscBoard(task.stringValue),
                                UserManager.Instance.meAsPlayer,
                                new Player(2, "Player Two"))
                            { _Type = (GameType)task.intValue });

                        break;

                    case OnboardingDataHolder.OnboardingActions.LOG_TUTORIAL:
                        AnalyticsManager.Instance.LogTutorialEvent(tutorial.data.tutorialName, task.stringValue);

                        break;

                    case OnboardingDataHolder.OnboardingActions.HIDE_WIZARD:
                        graphics.Hide(.3f);

                        break;

                    case OnboardingDataHolder.OnboardingActions.WIZARD_CENTER:
                        graphics.Show(.3f);

                        break;

                    //player1 make move
                    case OnboardingDataHolder.OnboardingActions.PLAYER_1_PLACE_GAMEPIECE:
                        Player player = activeGame.me;
                        GamePlayManager.instance.board.TakeTurn(new SimpleMove(new Piece(player.PlayerId, int.Parse(player.HerdId)), task.direction, task.intValue));

                        break;

                    case OnboardingDataHolder.OnboardingActions.PLAYER_2_PLACE_GAMEPIECE:
                        Player opponent = activeGame.opponent;
                        GamePlayManager.instance.board.TakeTurn(new SimpleMove(new Piece(opponent.PlayerId, opponent.HerdId == null ? 1 : int.Parse(opponent.HerdId)), task.direction, task.intValue));

                        break;

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

                    case OnboardingDataHolder.OnboardingActions.LOAD_MAIN_MENU:
                        GameManager.Instance.OpenMainMenu();

                        while (!FourzyMainMenuController.instance || !FourzyMainMenuController.instance.initialized) yield return null;

                        break;

                    case OnboardingDataHolder.OnboardingActions.EXEC_MENU_EVENT:
                        MenuController.AddMenuEvent(task.stringValue, task.menuEvent);
                        MenuController.GetMenu(task.stringValue).ExecuteMenuEvents();

                        yield return new WaitForSeconds(.1f);

                        break;

                        //will update currentWidget
                    case OnboardingDataHolder.OnboardingActions.HIGHLIGHT_PROGRESSION_EVENT:
                        if (!GameManager.Instance.isMainMenuLoaded) break;

                        Camera3D.Camera3dItemProgressionMap item = FourzyMainMenuController.instance.GetScreen<ProgressionMapScreen>().mapContent._item;

                        Vector2 anchors = item.GetCurrentEventCameraRelativePosition();

                        masks.ShowMasks(anchors, new Vector2(250f, 150f), true);

                        if (!pointer.visible) pointer.Show(.2f);
                        pointer.SetAnchors(anchors);

                        item.SetScrollLockedState(true);

                        UpdateCurrentButton(item.GetCurrentEvent().button);

                        break;

                    case OnboardingDataHolder.OnboardingActions.HIGHLIGHT_CURRENT_SCREEN_BUTTON:
                        if (!GameManager.Instance.isMainMenuLoaded) break;

                        //MenuScreen screen = MenuController.GetMenu(Constants.MAIN_MENU_CANVAS_NAME).currentScreen;
                        MenuScreen screen = FourzyMainMenuController.instance.currentScreen;
                        GameObject target = null;

                        //get button
                        if (screen.GetType() == typeof(MenuTabbedScreen))
                            target = (screen as MenuTabbedScreen).CurrentTab.gameObject;
                        else
                            target = screen.gameObject;

                        foreach (ButtonExtended button in target.GetComponentsInChildren<ButtonExtended>())
                        {
                            if (button.name == task.stringValue)
                            {
                                UpdateCurrentButton(button);

                                break;
                            }
                        }

                        masks.ShowMasks(task.pointAt, new Vector2(350f, 130f), true);

                        break;
                }

                while (currentButton) yield return null;
            }
        }
    }
}