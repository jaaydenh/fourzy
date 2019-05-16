//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class OnboardingScreen : MenuScreen
    {
        public static bool isActive = false;

        public OnboardingScreenBG bg;
        public OnboardingScreenDialog dialog;
        public OnboardingScreenPointer pointer;
        public OnboardingScreenHighlight highlight;
        public OnboardingScreenGraphics graphics;

        private int tutorialID;
        private int step;

        public OnboardingDataHolder current => GameContentManager.Instance.tutorials.list[tutorialID].data;

        public override void Open()
        {
            //name prompt was closed, so we do next
            if (IsCurrentBacthContains(OnboardingDataHolder.OnboardingActions.USER_CHANGE_NAME_PROMPT))
                StartRoutine("username changed", .2f, () => { Next(); });

            if (isOpened)
                return;

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
            switch (current.onFinished)
            {
                case OnboardingDataHolder.OnFinished.LOAD_MAIN_MENU:
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

        public void OpenOnboarding(int tutorialID)
        {
            if (!WillDisplayTutorial(tutorialID))
                return;

            this.tutorialID = tutorialID;
            menuController.OpenScreen(this);

            PlayerPrefsWrapper.SetTutorialOpened(current, true);
            DisplayCurrentStep();
        }

        public bool WillDisplayTutorial(int tutorialID)
        {
            if (!GameManager.Instance.displayTutorials)
                return false;

            if (tutorialID >= GameContentManager.Instance.tutorials.list.Count
                || ((PlayerPrefsWrapper.GetTutorialOpened(GameContentManager.Instance.tutorials.list[tutorialID].data) || PlayerPrefsWrapper.GetTutorialFinished(GameContentManager.Instance.tutorials.list[tutorialID].data)) && !GameManager.Instance.forceDisplayTutorials))
                return false;

            return true;
        }

        public void Next()
        {
            step++;
            DisplayCurrentStep();
        }

        public void DisplayCurrentStep()
        {
            if (step >= current.batches.Length)
            {
                //close onboarding
                menuController.CloseCurrentScreen();
                PlayerPrefsWrapper.SetTutorialState(current, true);

                return;
            }

            OnboardingDataHolder.OnboardingTasksBatch batch = current.batches[step];

            AnalyticsManager.LogOnboardingStart(tutorialID, step);
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

                    //board input
                    case OnboardingDataHolder.OnboardingActions.LIMIT_BOARD_INPUT:
                        GamePlayManager.instance.board.LimitInput(task.areas);
                        break;

                    case OnboardingDataHolder.OnboardingActions.RESET_BOARD_INPUT:
                        GamePlayManager.instance.board.ResetInputLimit(true);
                        break;

                    //bg
                    case OnboardingDataHolder.OnboardingActions.HIDE_BG:
                        bg.Hide(.2f);
                        break;

                    case OnboardingDataHolder.OnboardingActions.SHOW_BG:
                        bg.Show(.2f);
                        break;

                    case OnboardingDataHolder.OnboardingActions.PLAY_INITIAL_MOVES:
                        if (GamePlayManager.instance)
                            GamePlayManager.instance.board.TryPlayInitialMoves();
                        break;

                    case OnboardingDataHolder.OnboardingActions.OPEN_GAME:
                        ClientFourzyGame game = new ClientFourzyGame(GameContentManager.Instance.GetMiscBoard(task.intValue + ""), UserManager.Instance.meAsPlayer, new Player(2, "Player Two"));
                        game._Type = GameType.ONBOARDING;

                        GameManager.Instance.StartGame(game);
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
                }
            }
        }

        public bool IsCurrentBacthContains(OnboardingDataHolder.OnboardingActions action)
        {
            OnboardingDataHolder.OnboardingTasksBatch batch = current.batches[step];

            foreach (OnboardingDataHolder.OnboardingTask task in batch.tasks)
                if (task.action == action)
                    return true;

            return false;
        }

        private void UserManagerOnUpdateName()
        {
            OnboardingDataHolder.OnboardingTasksBatch batch = current.batches[step];

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
            OnboardingDataHolder.OnboardingTasksBatch batch = current.batches[step];

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
            OnboardingDataHolder.OnboardingTasksBatch batch = current.batches[step];

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
            OnboardingDataHolder.OnboardingTasksBatch batch = current.batches[step];

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
    }
}