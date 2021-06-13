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
        public OnboardingScreenMiniboard miniboard;
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
                        AnalyticsManager.Instance.LogTutorialEvent(tutorial.name, "tutorialSkipped");

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

            if (menuController.currentScreen != this)
            {
                menuController.OpenScreen(this);
            }

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

            if (tutorial == null ||
                ((PlayerPrefsWrapper.GetTutorialOpened(tutorial.name) ||
                  PlayerPrefsWrapper.GetTutorialFinished(tutorial.name)) &&
                  !GameManager.Instance.forceDisplayTutorials))
            {
                return false;
            }

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

            if (Instance.isOpened)
            {
                Instance.CloseSelf();
            }
        }

        private void MoveStarted(ClientPlayerTurn turn, bool startTurn)
        {
            if (turn == null || turn.PlayerId < 1) return;

            if (currentTask.action == OnboardingActions.ON_MOVE_STARTED)
            {
                SkipToNext();
            }
        }

        private void MoveEnded(ClientPlayerTurn turn, PlayerTurnResult turnResult, bool startTurn)
        {
            if (turn == null || turn.PlayerId < 1) return;

            if (currentTask.action == OnboardingActions.ON_MOVE_ENDED)
            {
                SkipToNext();
            }
        }

        private void OnGameFinished(IClientFourzy game)
        {
            if (currentTask.action == OnboardingActions.GAME_FINISHED)
            {
                SkipToNext();
            }
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
            GameboardView board;
            Vector2 anchors;

            for (int taskIndex = 0; taskIndex < tutorial.tasks.Length; taskIndex++)
            {
                currentTask = tutorial.tasks[taskIndex];

                switch (currentTask.action)
                {
                    case OnboardingActions.SHOW_MINIBOARD:
                        OnboardingTask_ShowMiniboard miniboardTask = currentTask as OnboardingTask_ShowMiniboard;

                        miniboard.SetBoards(miniboardTask.Boards);
                        miniboard.Show(.2f);
                        miniboard.SetAnchors(miniboardTask.Anchor);

                        break;

                    case OnboardingActions.HIDE_MINIBOARD:
                        miniboard.Hide(.2f);

                        break;

                    //dialog
                    case OnboardingActions.SHOW_MESSAGE:
                        OnboardingTask_ShowMessage messageTask = currentTask as OnboardingTask_ShowMessage;

                        dialog
                            .SetText(messageTask.Message)
                            .SetFontSize(messageTask.FontSize)
                            .SetAnchors(currentTask.vector2value)
                            .Show(.2f);

                        if (messageTask.SkipAfter != -1f)
                        {
                            _yield = true;
                            StartRoutine("messageYield", messageTask.SkipAfter, SkipToNext, null);
                        }
                        else if (bg.shown)
                        {
                            _yield = true;
                        }

                        break;

                    case OnboardingActions.HIDE_MESSAGE_BOX:
                        dialog.Hide(.2f);

                        break;

                    case OnboardingActions.SHOW_BUBBLE_MESSAGE:
                        OnboardingTask_ShowBubbleMessage bubbleMessageTask = currentTask as OnboardingTask_ShowBubbleMessage;

                        instructions
                            .SetText(bubbleMessageTask.Message)
                            .SetFontSize(bubbleMessageTask.FontSize)
                            .SetAnchors(currentTask.vector2value)
                            .Show(.2f);

                        if (bubbleMessageTask.SkipAfter != -1f)
                        {
                            _yield = true;
                            StartRoutine("messageYield", bubbleMessageTask.SkipAfter, SkipToNext, null);
                        }
                        else if (bg.shown)
                        {
                            _yield = true;
                        }

                        break;

                    case OnboardingActions.HIDE_BUBBLE_MESSAGE:
                        instructions.Hide(.2f);

                        break;

                    case OnboardingActions.PAUSE_BOARD:
                        board = GamePlayManager.Instance.board;
                        if (board == null) break;

                        board.Pause();

                        break;

                    case OnboardingActions.RESUME_BOARD:
                        board = GamePlayManager.Instance.board;
                        if (board == null) break;

                        board.Resume();

                        break;

                    case OnboardingActions.HIGHLIGHT_GAMPIECES:
                        bg.ShowGamepieces();

                        break;

                    case OnboardingActions.HIDE_GAMEPIECES:
                        bg.HideGamepieces();

                        break;

                    //pointer
                    case OnboardingActions.POINT_AT:
                        board = GamePlayManager.Instance.board;
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
                        if (!highlight.visible)
                        {
                            highlight.Show(.2f);
                        }

                        highlight.ShowHighlight(currentTask.areas);

                        break;

                    case OnboardingActions.HIDE_HIGHLIGHT:
                        highlight.Hide(.2f);

                        break;

                    case OnboardingActions.SHOW_BOARD_HINT_AREA:
                        board = GamePlayManager.Instance.board;
                        board.SetHintAreaSelectableState(false);
                        board.ShowHintArea(GameboardView.HintAreaStyle.ANIMATION_LOOP, GameboardView.HintAreaAnimationPattern.DIAGONAL);

                        yield return null;

                        break;

                    case OnboardingActions.HIDE_BOARD_HINT_AREA:
                        board = GamePlayManager.Instance.board;
                        board.SetHintAreaSelectableState(true);
                        board.HideHintArea(GameboardView.HintAreaAnimationPattern.DIAGONAL);

                        yield return null;

                        break;

                    case OnboardingActions.SHOW_MASKED_BOARD_CELL:
                        masks.ShowMasks(currentTask as OnboardingTask_ShowMaskedBoardCells);

                        break;

                    case OnboardingActions.HIDE_MAKSED_AREA:
                        masks.Hide();

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
                        OnboardingTask_ShowBG showBGTask = currentTask as OnboardingTask_ShowBG;
                        bg._Show(true);
                        bg.SetInteractable(showBGTask.Interactable);

                        break;

                    case OnboardingActions.PLAY_INITIAL_MOVES:
                        if (GamePlayManager.Instance)
                        {
                            GamePlayManager.Instance.board.PlayInitialMoves();
                        }

                        break;

                    case OnboardingActions.SET_BACK_BUTTON_STATE:
                        if (GamePlayManager.Instance)
                        {
                            GamePlayManager.Instance.backButton.SetActive(currentTask.boolValue);
                        }

                        break;

                    case OnboardingActions.OPEN_GAME:
                        OnboardingTask_OpenGame _openGameTask = currentTask as OnboardingTask_OpenGame;

                        ClientFourzyGame _game = new ClientFourzyGame(
                                GameContentManager.Instance.GetMiscBoard(_openGameTask.GameId),
                                UserManager.Instance.meAsPlayer,
                                _openGameTask.Player == null ? new Player(2, "Player Two") : _openGameTask.Player)
                        {
                            _Type = _openGameTask.Type,
                            _Mode = _openGameTask.Mode
                        };
                        _game.UpdateFirstState();

                        //wait for game to load
                        yield return GameManager.Instance.StartGame(_game, GameTypeLocal.LOCAL_GAME);

                        break;

                    case OnboardingActions.WAIT:
                        OnboardingTask_Wait waitTask = currentTask as OnboardingTask_Wait;

                        if (waitTask.WaitTime > 0f)
                        {
                            yield return new WaitForSeconds(waitTask.WaitTime);
                        }
                        else
                        {
                            _yield = true;
                        }

                        break;

                    case OnboardingActions.LOG_TUTORIAL:
                        AnalyticsManager.Instance.LogTutorialEvent(tutorial.name, currentTask.stringValue);

                        break;

                    case OnboardingActions.HIDE_GRAPHICS:
                        graphics.Hide(.3f);

                        break;

                    case OnboardingActions.SHOW_GRAPHICS:
                        graphics.Show(.3f);
                        graphics.SetAnchors(currentTask.vector2value);

                        break;

                    //player1 make move
                    case OnboardingActions.PLAYER_1_PLACE_GAMEPIECE:
                        GamePlayManager.Instance.board.TakeTurn(
                            new SimpleMove(GameManager.Instance.activeGame.playerPiece, currentTask.direction, currentTask.intValue));

                        break;

                    case OnboardingActions.PLAYER_2_PLACE_GAMEPIECE:
                        GamePlayManager.Instance.board.TakeTurn(
                            new SimpleMove(GameManager.Instance.activeGame.opponentPiece, currentTask.direction, currentTask.intValue));

                        break;

                    case OnboardingActions.USER_CHANGE_NAME_PROMPT:
                        menuController.GetOrAddScreen<ChangeNamePromptScreen>()._Prompt();

                        break;

                    case OnboardingActions.LOAD_MAIN_MENU:
                        GameManager.Instance.OpenMainMenu();

                        while (!FourzyMainMenuController.instance ||
                            !FourzyMainMenuController.instance.initialized)
                        {
                            yield return null;
                        }

                        break;

                    case OnboardingActions.EXEC_MENU_EVENT:
                        MenuController.AddMenuEvent(currentTask.stringValue, currentTask.menuEvent);
                        MenuController.GetMenu(currentTask.stringValue).ExecuteMenuEvents();

                        yield return new WaitForSeconds(.1f);

                        break;

                    case OnboardingActions.HIGHLIGHT_PROGRESSION_EVENT:
                        if (!GameManager.Instance.isMainMenuLoaded) break;

                        OnboardingTask_HighlightProgressionEvent _eventTask =
                            currentTask as OnboardingTask_HighlightProgressionEvent;

                        ProgressionEvent progressionEvent;

                        if (_eventTask.intValue == -1)
                        {
                            progressionEvent = GameManager.Instance.currentMap.GetCurrentEvent();
                        }
                        else
                        {
                            progressionEvent = GameManager.Instance.currentMap.widgets[_eventTask.intValue];
                        }

                        GameManager.Instance.currentMap.FocusOn(progressionEvent);
                        anchors = GameManager.Instance.currentMap.GetEventCameraRelativePosition(progressionEvent);

                        masks.ShowMask(
                            anchors,
                            progressionEvent.rectTransform,
                            _eventTask.vector2value,
                            Vector2.zero,
                            _eventTask.showBG);

                        if (!pointer.visible)
                        {
                            pointer.Show(.2f);
                        }
                        pointer.SetAnchors(anchors);

                        if (_eventTask.messageData != null)
                        {
                            instructions.SetText(_eventTask.messageData.message);
                            instructions.SetAnchors(anchors);
                            instructions.SetLocalPosition(_eventTask.messageData.positionOffset);
                        }

                        GameManager.Instance.currentMap.SetScrollLockedState(true);

                        UpdateCurrentButton(progressionEvent.button);

                        break;

                    case OnboardingActions.HIGHLIGHT_CURRENT_SCREEN_BUTTON:
                        OnboardingTask_HighlightButton _buttonTask = currentTask as OnboardingTask_HighlightButton;

                        MenuController _menuController = MenuController.GetMenu(_buttonTask.menuName);
                        MenuScreen screen = _menuController.currentScreen;

                        if (screen.GetType() == typeof(MenuTabbedScreen))
                        {
                            screen = (screen as MenuTabbedScreen).CurrentTab;
                        }

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

                        masks.ShowMask(
                            viewportPosition,
                            currentButton.rectTransform,
                            _buttonTask.vector2value,
                            _buttonTask.maskOffset,
                            _buttonTask.showBG);

                        //pointer
                        if (!pointer.visible)
                        {
                            pointer.Show(.2f);
                        }
                        pointer.SetAnchors(viewportPosition);

                        if (_buttonTask.messageData != null)
                        {
                            instructions.SetText(_buttonTask.messageData.message);
                            instructions.SetAnchors(viewportPosition);
                            instructions.SetLocalPosition(_buttonTask.messageData.positionOffset);
                        }

                        pointer.SetLocalPosition(_buttonTask.pointerOffset);

                        break;

                    case OnboardingActions.SHOW_MASKED_AREA:
                        OnboardingTask_ShowMaskedArea areaMaskTask = currentTask as OnboardingTask_ShowMaskedArea;

                        if (string.IsNullOrEmpty(areaMaskTask.Target))
                        {
                            masks.ShowMask(
                                areaMaskTask.Anchor,
                                areaMaskTask.Pivot,
                                areaMaskTask.Size,
                                areaMaskTask.Offset,
                                areaMaskTask.MaskStyle,
                                true);
                        }
                        else
                        {
                            _menuController = MenuController.GetMenu(areaMaskTask.stringValue);
                            RectTransform _target = _menuController.transform.FindRecursive(areaMaskTask.Target) as RectTransform;

                            if (_target)
                            {
                                if (_menuController.canvas)
                                {
                                    if (_menuController.canvas.renderMode == RenderMode.ScreenSpaceCamera)
                                    {
                                        masks.ShowMask(
                                            Camera.main.WorldToViewportPoint(_target.transform.position),
                                            _target.pivot,
                                            areaMaskTask.Size,
                                            areaMaskTask.Offset,
                                            areaMaskTask.MaskStyle,
                                            true);
                                    }
                                    else
                                    {
                                        //todo
                                    }
                                }
                            }
                        }

                        masks.Interactable(areaMaskTask.Interactable);

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