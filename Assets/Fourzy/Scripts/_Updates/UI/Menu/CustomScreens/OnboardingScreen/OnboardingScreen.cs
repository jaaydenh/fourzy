//@vadym udod

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.ClientModel;
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
using UnityEngine.EventSystems;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class OnboardingScreen : MenuScreen
    {
        public static OnboardingScreen Instance;

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

        [NonSerialized]
        public Tutorial tutorial;

        private bool _yield;

        public override void Open()
        {
            //name prompt was closed, so we do next
            //if (tutorial.tasks.Any(task => OnboardingActions.USER_CHANGE_NAME_PROMPT.)) StartRoutine("nameChanged", .2f, () => Next());

            if (isOpened) return;

            base.Open();

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
            StopAllCoroutines();
            Instance = this;

            RemoveCurrentButton();
            _yield = false;
            isTutorialRunning = true;
            this.tutorial = tutorial;

            if (menuController.currentScreen != this) menuController.OpenScreen(this);

            masks.Hide(0f);
            pointer.Hide(0f);
            dialog.Hide(0f);
            instructions.Hide(0f);
            bg.Hide(0f);
            graphics.Hide(0f);

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

        public void SkipToNext()
        {
            _yield = false;
            CancelRoutine("messageYield");
        }

        public static void CloseTutorial()
        {
            if (!Instance) return;

            if (Instance.isOpened) Instance.CloseSelf();
        }

        private void MoveStarted(ClientPlayerTurn turn)
        {
            if (turn == null || turn.PlayerId < 1) return;

            if (currentTask.action == OnboardingActions.ON_MOVE_STARTED) SkipToNext();
        }

        private void MoveEnded(ClientPlayerTurn turn)
        {
            if (turn == null || turn.PlayerId < 1) return;

            if (currentTask.action == OnboardingActions.ON_MOVE_ENDED) SkipToNext();
        }

        private void OnGameFinished(IClientFourzy game)
        {
            if (currentTask.action == OnboardingActions.GAME_FINISHED) SkipToNext();
        }

        private void OnButtonTap(PointerEventData data)
        {
            RemoveCurrentButton();
            SkipToNext();
        }

        private void UpdateCurrentButton(ButtonExtended button)
        {
            currentButton = button;
            currentButton.onTap += OnButtonTap;
        }

        private void RemoveCurrentButton()
        {
            if (!currentButton) return;

            currentButton.onTap -= OnButtonTap;
            currentButton = null;
        }

        public IEnumerator DisplayCurrentStep()
        {
            IClientFourzy activeGame = GameManager.Instance.activeGame;
            Vector2 anchors;

            for (int taskIndex = 0; taskIndex < tutorial.tasks.Length; taskIndex++)
            {
                currentTask = tutorial.tasks[taskIndex];

                switch (currentTask.action)
                {
                    //dialog
                    case OnboardingActions.SHOW_MESSAGE:
                        OnboardingTask_ShowMessage messageTask = currentTask as OnboardingTask_ShowMessage;

                        switch (messageTask.intValue)
                        {
                            case 0:
                                dialog.DisplayText(messageTask.yAnchor, currentTask.stringValue);

                                break;

                            case 1:
                                instructions.DisplayText(messageTask.yAnchor, currentTask.stringValue);

                                break;
                        }

                        if (messageTask.skipAfter != -1f)
                        {
                            _yield = true;
                            StartRoutine("messageYield", messageTask.skipAfter, () => SkipToNext(), null);
                        }
                        else if (bg.shown) _yield = true;

                        break;

                    case OnboardingActions.HIDE_MESSAGE_BOX:
                        dialog._Hide();
                        instructions._Hide();

                        break;

                    //pointer
                    case OnboardingActions.POINT_AT:
                        GameboardView board = GamePlayManager.Instance.board;
                        if (board == null) break;

                        OnboardingTask_PointAt pointAtTask = currentTask as OnboardingTask_PointAt;

                        pointer.AnimatePointer(
                            pointAtTask.points.ContainsKey(GameManager.Instance.placementStyle) ?
                            pointAtTask.points[GameManager.Instance.placementStyle] :
                            pointAtTask.points.Values.First());
                        pointer.SetMessage(currentTask.stringValue);

                        break;

                    case OnboardingActions.HIDE_POINTER:
                        pointer.HidePointer();

                        break;

                    case OnboardingActions.ON_MOVE_STARTED:
                    case OnboardingActions.ON_MOVE_ENDED:
                    case OnboardingActions.GAME_FINISHED:
                        _yield = true;

                        break;

                    case OnboardingActions.HIGHLIGHT:
                        if (!highlight.visible) highlight.Show(.2f);

                        highlight.ShowHighlight(currentTask.areas);

                        break;

                    case OnboardingActions.HIDE_HIGHLIGHT:
                        highlight.Hide(.2f);

                        break;

                    case OnboardingActions.SHOW_BOARD_HINT_AREA:
                        GamePlayManager.Instance.board.SetHintAreaSelectableState(false);
                        GamePlayManager.Instance.board.ShowHintArea(Mechanics.Board.GameboardView.HintAreaStyle.ANIMATION_LOOP, Mechanics.Board.GameboardView.HintAreaAnimationPattern.DIAGONAL);

                        yield return null;

                        break;

                    case OnboardingActions.HIDE_BOARD_HINT_AREA:
                        GamePlayManager.Instance.board.SetHintAreaSelectableState(true);
                        GamePlayManager.Instance.board.HideHintArea(Mechanics.Board.GameboardView.HintAreaAnimationPattern.DIAGONAL);

                        yield return null;

                        break;

                    case OnboardingActions.SHOW_MASKED_AREA:
                        masks.ShowMasks(currentTask as OnboardingTask_ShowMaskedArea);

                        break;

                    case OnboardingActions.HIDE_MAKSED_AREA:
                        masks.Hide();
                        instructions._Hide();

                        break;

                    case OnboardingActions.LIMIT_BOARD_INPUT:
                        GamePlayManager.Instance.board.LimitInput(currentTask.areas);

                        break;

                    case OnboardingActions.RESET_BOARD_INPUT:
                        GamePlayManager.Instance.board.SetInputMap(true);

                        break;

                    case OnboardingActions.HIDE_BG:
                        bg._Hide();

                        break;

                    case OnboardingActions.SHOW_BG:
                        bg._Show(currentTask.intValue == 0);

                        break;

                    case OnboardingActions.PLAY_INITIAL_MOVES:
                        if (GamePlayManager.Instance) GamePlayManager.Instance.board.PlayInitialMoves();

                        break;

                    case OnboardingActions.OPEN_GAME:
                        //wait for game to load
                        yield return GameManager.Instance.StartGame(
                            new ClientFourzyGame(
                                GameContentManager.Instance.GetMiscBoard(currentTask.stringValue),
                                UserManager.Instance.meAsPlayer,
                                new Player(2, "Player Two"))
                            { _Type = (GameType)currentTask.intValue }, GameTypeLocal.LOCAL_GAME);

                        break;

                    case OnboardingActions.WAIT:
                        OnboardingTask_Wait waitTask = currentTask as OnboardingTask_Wait;

                        //if (taskIndex == currentBatch.tasks.Length - 1)
                        //    yield return StartRoutine("moveNext", waitTask.time, () => Next());
                        //else
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
                        GamePlayManager.Instance.board.TakeTurn(
                            new SimpleMove(
                                new Piece(player.PlayerId, int.Parse(player.HerdId)), currentTask.direction, currentTask.intValue));

                        _yield = true;

                        break;

                    case OnboardingActions.PLAYER_2_PLACE_GAMEPIECE:
                        Player opponent = activeGame.opponent;
                        GamePlayManager.Instance.board.TakeTurn(
                            new SimpleMove(
                                new Piece(opponent.PlayerId, opponent.HerdId == null ? 1 : int.Parse(opponent.HerdId)), currentTask.direction, currentTask.intValue));

                        _yield = true;

                        break;

                    case OnboardingActions.USER_CHANGE_NAME_PROMPT:
                        menuController.GetOrAddScreen<ChangeNamePromptScreen>()._Prompt();

                        break;

                    //case OnboardingActions.SKIP_TO_NEXT_IF_DEMO_MODE:
                    //    if (SettingsManager.Get(SettingsManager.KEY_DEMO_MODE))
                    //    {
                    //        Next();
                    //        yield break;
                    //    }

                    //    break;

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

                        ProgressionEvent progressionEvent;

                        if (_eventTask.intValue == -1)
                            progressionEvent = GameManager.Instance.currentMap.GetCurrentEvent();
                        else
                            progressionEvent = GameManager.Instance.currentMap.widgets[_eventTask.intValue];

                        GameManager.Instance.currentMap.FocusOn(progressionEvent);
                        anchors = GameManager.Instance.currentMap.GetEventCameraRelativePosition(progressionEvent);

                        masks.ShowMask(anchors, progressionEvent.rectTransform, _eventTask.vector2value, Vector2.zero, _eventTask.showBG);

                        if (!pointer.visible) pointer.Show(.2f);
                        pointer.SetAnchors(anchors);

                        if (_eventTask.messageData != null)
                        {
                            instructions.DisplayText(anchors.y, _eventTask.messageData.message);
                            instructions.SetLocalPosition(_eventTask.messageData.positionOffset);
                        }

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
                            instructions.DisplayText(viewportPosition.y, _buttonTask.messageData.message);
                            instructions.SetLocalPosition(_buttonTask.messageData.positionOffset);
                        }

                        pointer.SetLocalPosition(_buttonTask.pointerOffset);

                        break;
                }

                while (currentButton || _yield) yield return null;
            }

            //close onboarding
            menuController.CloseCurrentScreen();
            PlayerPrefsWrapper.SetTutorialState(tutorial.name, true);
        }
    }
}