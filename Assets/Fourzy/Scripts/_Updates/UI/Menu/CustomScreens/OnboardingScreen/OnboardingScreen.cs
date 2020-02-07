//@vadym udod

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System;
using System.Collections;
using System.Linq;
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
        public OnboardingScreenInstruction instructions;

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
            if (tutorial.data[step].ContainsAction(OnboardingActions.USER_CHANGE_NAME_PROMPT)) StartRoutine("nameChanged", .2f, () => Next());

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

            pointer.HidePointer();
            RemoveCurrentButton();
            StopAllCoroutines();

            UserManager.OnUpdateUserInfo -= UserManagerOnUpdateName;

            GamePlayManager.onMoveStarted -= MoveStarted;
            GamePlayManager.onMoveEnded -= MoveEnded;
            GamePlayManager.onGameFinished -= OnGameFinished;
        }

        public override void OnBack()
        {
            switch (tutorial.onBack)
            {
                case TutorialOnBack.IGNORE:

                    break;

                case TutorialOnBack.SHOW_LEAVE_PROMPT:
                    base.OnBack();

                    menuController.GetOrAddScreen<PromptScreen>().Prompt("Quit tutorial?", "Leave tutorial level?\nYou can reset tutorial in Options Screen.", () =>
                    {
                        //close prompt
                        menuController.CloseCurrentScreen();
                        //close onboarding
                        menuController.CloseCurrentScreen();

                        GameManager.Instance.OpenMainMenu();
                    });

                    break;
            }
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
            dialog.Hide(0f);
            instructions.Hide(0f);

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
            if (IsRoutineActive("moveNext"))
            {
                CancelRoutine("moveNext");
                return;
            }
            print("next");

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

            if (tutorial.data[step].ContainsAction(OnboardingActions.ON_MOVE_STARTED)) Next();
        }

        private void MoveEnded(ClientPlayerTurn turn)
        {
            if (turn == null || turn.PlayerId < 1) return;

            if (tutorial.data[step].ContainsAction(OnboardingActions.ON_MOVE_ENDED))
                StartRoutine("moveNext", .5f, () => Next());
        }

        private void OnGameFinished(IClientFourzy game)
        {
            if (tutorial.data[step].ContainsAction(OnboardingActions.PLAY_INITIAL_MOVES))
                StartRoutine("gameFinishedNext", 2f, () => Next());
            else if (tutorial.data[step].ContainsAction(OnboardingActions.GAME_FINISHED))
                Next();
        }

        private void OnTap()
        {
            RemoveCurrentButton();

            CancelRoutine("idle");
            StartRoutine("idle", .1f, () => Next());
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

            for (int taskIndex = 0; taskIndex < currentBatch.tasks.Length; taskIndex++)
            {
                currentTask = currentBatch.tasks[taskIndex];

                switch (currentTask.action)
                {
                    //dialog
                    case OnboardingActions.SHOW_MESSAGE:
                        OnboardingTask_ShowMessage instructionTask = currentTask as OnboardingTask_ShowMessage;

                        switch (instructionTask.intValue)
                        {
                            case 0:
                                dialog.DisplayText(instructionTask.yAnchor, currentTask.stringValue);

                                break;

                            case 1:
                                instructions.DisplayText(instructionTask.yAnchor, currentTask.stringValue);

                                break;
                        }

                        break;

                    case OnboardingActions.HIDE_MESSAGE_BOX:
                        dialog._Hide();
                        instructions._Hide();

                        break;

                    //pointer
                    case OnboardingActions.POINT_AT:
                        GameboardView board = GamePlayManager.instance.board;
                        if (board == null) break;

                        OnboardingTask_PointAt pointAtTask = currentTask as OnboardingTask_PointAt;

                        pointer.AnimatePointer(pointAtTask.points);
                        pointer.SetMessage(currentTask.stringValue);

                        break;

                    case OnboardingActions.HIDE_POINTER:
                        pointer.HidePointer();

                        break;

                    case OnboardingActions.HIGHLIGHT:
                        if (!highlight.visible) highlight.Show(.2f);

                        highlight.ShowHighlight(currentTask.areas);

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
                        masks.ShowMasks(currentTask);

                        break;

                    case OnboardingActions.HIDE_MAKSED_AREA:
                        masks.Hide();
                        instructions._Hide();

                        break;

                    case OnboardingActions.LIMIT_BOARD_INPUT:
                        GamePlayManager.instance.board.LimitInput(currentTask.areas);

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
                                GameContentManager.Instance.GetMiscBoard(currentTask.stringValue),
                                UserManager.Instance.meAsPlayer,
                                new Player(2, "Player Two"))
                            { _Type = (GameType)currentTask.intValue });

                        break;

                    case OnboardingActions.WAIT:
                        OnboardingTask_Wait waitTask = currentTask as OnboardingTask_Wait;

                        if (taskIndex == currentBatch.tasks.Length - 1)
                            yield return StartRoutine("moveNext", waitTask.time, () => Next());
                        else
                            yield return new WaitForSeconds(waitTask.time);

                        break;

                    case OnboardingActions.LOG_TUTORIAL:
                        AnalyticsManager.Instance.LogTutorialEvent(tutorial.name, currentTask.stringValue);

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
                        GamePlayManager.instance.board.TakeTurn(
                            new SimpleMove(
                                new Piece(player.PlayerId, int.Parse(player.HerdId)), currentTask.direction, currentTask.intValue));

                        break;

                    case OnboardingActions.PLAYER_2_PLACE_GAMEPIECE:
                        Player opponent = activeGame.opponent;
                        GamePlayManager.instance.board.TakeTurn(
                            new SimpleMove(
                                new Piece(opponent.PlayerId, opponent.HerdId == null ? 1 : int.Parse(opponent.HerdId)), currentTask.direction, currentTask.intValue));

                        break;

                    case OnboardingActions.USER_CHANGE_NAME_PROMPT:
                        menuController.GetOrAddScreen<ChangeNamePromptScreen>()._Prompt();

                        break;

                    case OnboardingActions.SKIP_TO_NEXT_IF_DEMO_MODE:
                        if (SettingsManager.Get(SettingsManager.KEY_DEMO_MODE))
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
                        MenuController.AddMenuEvent(currentTask.stringValue, currentTask.menuEvent);
                        MenuController.GetMenu(currentTask.stringValue).ExecuteMenuEvents();

                        yield return new WaitForSeconds(.1f);

                        break;

                    //will update currentWidget
                    case OnboardingActions.HIGHLIGHT_PROGRESSION_EVENT:
                        if (!GameManager.Instance.isMainMenuLoaded) break;

                        OnboardingTask_HighlightProgressionEvent _eventTask = currentTask as OnboardingTask_HighlightProgressionEvent;

                        ProgressionEvent progressionEvent = null;

                        if (_eventTask.intValue == -1)
                            progressionEvent = GameManager.Instance.currentMap.GetCurrentEvent();
                        else
                            progressionEvent = GameManager.Instance.currentMap.widgets[_eventTask.intValue];

                        GameManager.Instance.currentMap.FocusOn(progressionEvent);
                        anchors = GameManager.Instance.currentMap.GetEventCameraRelativePosition(progressionEvent);

                        masks.ShowMask(anchors, progressionEvent.rectTransform, _eventTask.vector2value, Vector2.zero, _eventTask.showBG);

                        if (!pointer.visible) pointer.Show(.2f);
                        pointer.SetAnchors(anchors);
                        pointer.SetMessage(_eventTask.stringValue);

                        GameManager.Instance.currentMap.SetScrollLockedState(true);

                        UpdateCurrentButton(progressionEvent.button);

                        break;

                    case OnboardingActions.HIGHLIGHT_CURRENT_SCREEN_BUTTON:
                        OnboardingTask_HighlightButton _buttonTask = currentTask as OnboardingTask_HighlightButton;

                        MenuController _menuController = MenuController.GetMenu(_buttonTask.menuName);
                        MenuScreen screen = _menuController.currentScreen;

                        if (screen.GetType() == typeof(MenuTabbedScreen)) screen = (screen as MenuTabbedScreen).CurrentTab;

                        //get button
                        foreach (ButtonExtended button in screen.GetComponentsInChildren<ButtonExtended>())
                        {
                            if (button.name == currentTask.stringValue)
                            {
                                UpdateCurrentButton(button);

                                break;
                            }
                        }

                        Vector2 viewportPosition = currentButton.rectTransform.GetViewportPosition();
                        _buttonTask.TrySetMessagePivot(viewportPosition);

                        masks.ShowMask(viewportPosition, currentButton.rectTransform, _buttonTask.vector2value, _buttonTask.maskOffset, _buttonTask.showBG);

                        //pointer
                        if (!pointer.visible) pointer.Show(.2f);
                        pointer.SetAnchors(viewportPosition);

                        if (_buttonTask.messageData != null)
                        {
                            instructions.DisplayText(viewportPosition.y, _buttonTask.message);
                            instructions.SetLocalPosition(_buttonTask.messageData.positionOffset);
                        }
                        else
                            instructions.DisplayText(viewportPosition.y, _buttonTask.message);

                        pointer.SetLocalPosition(_buttonTask.pointerOffset);

                        break;
                }

                while (currentButton) yield return null;
            }
        }
    }
}