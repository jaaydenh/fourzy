//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Tools;
using Fourzy._Updates._Tutorial;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System;
using System.Collections;
using UnityEngine;
using Fourzy._Updates.Mechanics.Board;

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
        public OnboardingTask currentTask { get; private set; }
        public OnboardingTasksBatch currentBatch { get; private set; }

        [NonSerialized]
        public Tutorial tutorial;

        private int step;

        public override void Open()
        {
            //name prompt was closed, so we do next
            if (tutorial.data[step].ContainsAction(OnboardingActions.USER_CHANGE_NAME_PROMPT))
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

        public void OpenTutorial(Tutorial tutorial)
        {
            instance = this;

            isTutorialRunning = true;
            this.tutorial = tutorial;
            step = 0;

            menuController.OpenScreen(this);
            masks.Hide(0f);
            pointer.Hide(0f);

            PlayerPrefsWrapper.SetTutorialOpened(tutorial.name, true);
            StartCoroutine(DisplayCurrentStep());
        }

        public bool WillDisplayTutorial(Tutorial tutorial)
        {
            if (!GameManager.Instance.displayTutorials) return false;

            if (tutorial == null || ((PlayerPrefsWrapper.GetTutorialOpened(tutorial.name) || PlayerPrefsWrapper.GetTutorialFinished(tutorial.name)) && !GameManager.Instance.forceDisplayTutorials))
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
            OnboardingTasksBatch batch = tutorial.data[step];

            foreach (OnboardingTask task in batch.tasks)
            {
                switch (task.action)
                {
                    case OnboardingActions.USER_CHANGE_NAME_PROMPT:

                        break;
                }
            }
        }

        private void MoveStarted(ClientPlayerTurn turn)
        {
            if (turn == null || turn.PlayerId < 1) return;

            if (tutorial.data[step].ContainsAction(OnboardingActions.ON_MOVE_STARTED))
                StartRoutine("moveFinishedNext", .5f, () => Next());
        }

        private void MoveEnded(ClientPlayerTurn turn)
        {
            if (turn == null || turn.PlayerId < 1) return;

            if (tutorial.data[step].ContainsAction(OnboardingActions.ON_MOVE_ENDED))
                StartRoutine("moveFinishedNext", .5f, () => Next());
        }

        private void OnGameFinished(IClientFourzy game)
        {
            if (tutorial.data[step].ContainsAction(OnboardingActions.PLAY_INITIAL_MOVES))
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
            if (step >= tutorial.data.Length)
            {
                //close onboarding
                menuController.CloseCurrentScreen();
                PlayerPrefsWrapper.SetTutorialState(tutorial.name, true);

                yield break;
            }

            currentBatch = tutorial.data[step];

            IClientFourzy activeGame = GameManager.Instance.activeGame;
            Vector2 anchors;

            foreach (OnboardingTask task in currentBatch.tasks)
            {
                currentTask = task;

                switch (task.action)
                {
                    //dialog
                    case OnboardingActions.SHOW_MESSAGE:
                        if (!dialog.visible) dialog.Show(.2f);

                        dialog.DisplayText(task.stringValue);

                        break;

                    case OnboardingActions.HIDE_MESSAGE_BOX:
                        dialog.Hide(.2f);

                        break;

                    //pointer
                    case OnboardingActions.POINT_AT:
                        GameboardView board = GamePlayManager.instance.board;

                        if (board == null) break;

                        if (!pointer.visible) pointer.Show(.2f);

                        anchors = Camera.main.WorldToViewportPoint(
                            (Vector2)board.transform.position +
                            board.BoardLocationToVec2(new BoardLocation((int)task.vector2value.y, (int)task.vector2value.x)));

                        pointer.SetAnchors(anchors);
                        pointer.SetMessage(task.stringValue);

                        break;

                    case OnboardingActions.HIDE_POINTER:
                        pointer.Hide(.2f);

                        break;

                    case OnboardingActions.HIGHLIGHT:
                        if (!highlight.visible) highlight.Show(.2f);

                        highlight.ShowHighlight(task.areas);

                        break;

                    case OnboardingActions.HIDE_HIGHLIGHT:
                        highlight.Hide(.2f);

                        break;

                    case OnboardingActions.SHOW_BOARD_HINT_AREA:
                        GamePlayManager.instance.board.SetHintAreaSelectableState(false);
                        GamePlayManager.instance.board.ShowHintArea(Mechanics.Board.GameboardView.HintAreaStyle.ANIMATION_LOOP, Mechanics.Board.GameboardView.HintAreaAnimationPattern.DIAGONAL);

                        yield return null;

                        break;

                    case OnboardingActions.HIDE_BOARD_HINT_AREA:
                        GamePlayManager.instance.board.SetHintAreaSelectableState(true);
                        GamePlayManager.instance.board.HideHintArea(Mechanics.Board.GameboardView.HintAreaAnimationPattern.DIAGONAL);

                        yield return null;

                        break;

                    case OnboardingActions.SHOW_MASKED_AREA:
                        masks.ShowMasks(task, FourzyMainMenuController.instance);

                        break;

                    case OnboardingActions.HIDE_MAKSED_AREA:
                        masks.Hide();

                        break;

                    case OnboardingActions.LIMIT_BOARD_INPUT:
                        GamePlayManager.instance.board.LimitInput(task.areas);

                        break;

                    case OnboardingActions.RESET_BOARD_INPUT:
                        GamePlayManager.instance.board.SetInputMap(true);

                        break;

                    case OnboardingActions.HIDE_BG:
                        bg.Hide(.2f);

                        break;

                    case OnboardingActions.SHOW_BG:
                        bg.Show(.2f);

                        break;

                    case OnboardingActions.PLAY_INITIAL_MOVES:
                        if (GamePlayManager.instance) GamePlayManager.instance.board.PlayInitialMoves();

                        break;

                    case OnboardingActions.OPEN_GAME:
                        //wait for game to load
                        yield return GameManager.Instance.StartGame(
                            new ClientFourzyGame(
                                GameContentManager.Instance.GetMiscBoard(task.stringValue),
                                UserManager.Instance.meAsPlayer,
                                new Player(2, "Player Two"))
                            { _Type = (GameType)task.intValue });

                        break;

                    case OnboardingActions.LOG_TUTORIAL:
                        AnalyticsManager.Instance.LogTutorialEvent(tutorial.name, task.stringValue);

                        break;

                    case OnboardingActions.HIDE_WIZARD:
                        graphics.Hide(.3f);

                        break;

                    case OnboardingActions.WIZARD_CENTER:
                        graphics.Show(.3f);

                        break;

                    //player1 make move
                    case OnboardingActions.PLAYER_1_PLACE_GAMEPIECE:
                        Player player = activeGame.me;
                        GamePlayManager.instance.board.TakeTurn(new SimpleMove(new Piece(player.PlayerId, int.Parse(player.HerdId)), task.direction, task.intValue));

                        break;

                    case OnboardingActions.PLAYER_2_PLACE_GAMEPIECE:
                        Player opponent = activeGame.opponent;
                        GamePlayManager.instance.board.TakeTurn(new SimpleMove(new Piece(opponent.PlayerId, opponent.HerdId == null ? 1 : int.Parse(opponent.HerdId)), task.direction, task.intValue));

                        break;

                    case OnboardingActions.USER_CHANGE_NAME_PROMPT:
                        menuController.GetScreen<ChangeNamePromptScreen>()._Prompt();

                        break;

                    case OnboardingActions.SKIP_TO_NEXT_IF_DEMO_MODE:
                        if (SettingsManager.Instance.Get(SettingsManager.KEY_DEMO_MODE))
                        {
                            Next();
                            yield break;
                        }

                        break;

                    case OnboardingActions.LOAD_MAIN_MENU:
                        GameManager.Instance.OpenMainMenu();

                        while (!FourzyMainMenuController.instance || !FourzyMainMenuController.instance.initialized) yield return null;

                        break;

                    case OnboardingActions.EXEC_MENU_EVENT:
                        MenuController.AddMenuEvent(task.stringValue, task.menuEvent);
                        MenuController.GetMenu(task.stringValue).ExecuteMenuEvents();

                        yield return new WaitForSeconds(.1f);

                        break;

                        //will update currentWidget
                    case OnboardingActions.HIGHLIGHT_PROGRESSION_EVENT:
                        if (!GameManager.Instance.isMainMenuLoaded) break;

                        Camera3D.Camera3dItemProgressionMap item = FourzyMainMenuController.instance.GetScreen<ProgressionMapScreen>().mapContent._item;

                        anchors = item.GetCurrentEventCameraRelativePosition();

                        masks.ShowMask(anchors, new Vector2(250f, 150f), OnboardingScreenMaskObject.MaskStyle.PX_16, true);

                        if (!pointer.visible) pointer.Show(.2f);
                        pointer.SetAnchors(anchors);
                        pointer.SetMessage(task.stringValue);

                        item.SetScrollLockedState(true);

                        UpdateCurrentButton(item.GetCurrentEvent().button);

                        break;

                    case OnboardingActions.HIGHLIGHT_CURRENT_SCREEN_BUTTON:
                        if (!GameManager.Instance.isMainMenuLoaded) break;

                        MenuController _menuController = FourzyMainMenuController.instance;
                        MenuScreen screen = _menuController.currentScreen;
                        OnboardingTask_HighlightButton _buttonTask = task as OnboardingTask_HighlightButton;

                        if (screen.GetType() == typeof(MenuTabbedScreen)) screen = (screen as MenuTabbedScreen).CurrentTab;

                        //get button
                        foreach (ButtonExtended button in screen.GetComponentsInChildren<ButtonExtended>())
                        {
                            if (button.name == task.stringValue)
                            {
                                UpdateCurrentButton(button);

                                break;
                            }
                        }

                        Vector3 position = currentButton.transform.position - screen.transform.position
                            + (Vector3)(currentButton.rectTransform.GetCenterOffset() * _menuController.transform.localScale.x);

                        Vector2 anchor = Vector2.one * .5f + new Vector2(position.x / _menuController.size.x, position.y / _menuController.size.y);

                        masks.ShowMask(anchor, new Vector2(330f, 122f), OnboardingScreenMaskObject.MaskStyle.PX_16, true);

                        //pointer
                        if (!pointer.visible) pointer.Show(.2f);
                        pointer.SetAnchors(anchor);
                        pointer.SetMessage(_buttonTask.message);

                        break;
                }

                while (currentButton) yield return null;
            }
        }
    }
}