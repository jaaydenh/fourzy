//@vadym udod

using Fourzy._Updates.Serialized;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class OnboardingScreen : MenuScreen
    {
        public OnboardingDataHolder scenario;

        public override void Open()
        {
            if (isOpened)
                return;

            GamePlayManager.OnStartMove += MoveStarted;
            GamePlayManager.OnEndMove += MoveEnded;
            GamePlayManager.OnGameOver += OnGameOver;

            base.Open();

        }

        public override void Close()
        {
            base.Close();

            GamePlayManager.OnStartMove -= MoveStarted;
            GamePlayManager.OnEndMove -= MoveEnded;
        }

        public override void OnBack()
        {
            base.OnBack();
            
            menuController.GetScreen<PromptScreen>().Prompt("Quit tutorial?", "Leave tutorial level?\nYou can reset tutorial in Screen.", () =>
            {
                //close prompt
                menuController.CloseCurrentScreen();
                //close onboarding
                menuController.CloseCurrentScreen();

                GamePlayManager.Instance.BackButtonOnClick();
            });
        }

        public void Next()
        {

        }

        private void MoveStarted()
        {
            //Debug.Log("MoveStarted: onboardingStep: " + onboardingStep);
            //if (onboardingStep >= 4 && onboardingStep <= 6)
            //{
            //    moveCount++;
            //    Debug.Log("MoveStarted movecount: " + moveCount);
            //    switch (moveCount)
            //    {
            //        case 1:
            //            dialogBox.SetActive(false);
            //            hand.SetActive(false);
            //            tapAreaAnim.SetActive(false);
            //            wizard.SetActive(false);
            //            NextStep();
            //            break;
            //        case 3:
            //            hand.SetActive(false);
            //            tapAreaAnim.SetActive(false);
            //            onboardingStep++;
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //else if (onboardingStep == 7)
            //{
            //    hand.SetActive(false);
            //    tapAreaAnim.SetActive(false);
            //}
        }

        private void MoveEnded()
        {
            //Debug.Log("MoveEnded: onboardingStep: " + onboardingStep);
            //if (onboardingStep >= 4 && onboardingStep <= 6)
            //{
            //    Debug.Log("MoveEnded movecount: " + moveCount);
            //    switch (moveCount)
            //    {
            //        case 1:
            //            Move move1 = new Move(4, Direction.UP, PlayerEnum.TWO);
            //            GameManager.Instance.CallMovePiece(move1, false, false);
            //            break;
            //        case 2:
            //            TapHintPosition(44, -302);
            //            break;
            //        case 3:
            //            Move move2 = new Move(2, Direction.DOWN, PlayerEnum.TWO);
            //            GameManager.Instance.CallMovePiece(move2, false, false);
            //            break;
            //        case 4:
            //            hand.SetActive(true);
            //            tapAreaAnim.SetActive(true);
            //            DoNextStep();
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //else if (onboardingStep == 7 && GameManager.Instance.activeGame.gameState.Winner != PlayerEnum.ONE)
            //{
            //    AnalyticsManager.LogOnboardingComplete(false, onboardingStage, onboardingStep);
            //    onboardingStep--;
            //    ShowWizardWithDialog("Try Again, you must get 4 in a row to win");
            //    step7Attempts++;
            //}
        }

        private void OnGameOver()
        {
            //Debug.Log("Onboarding:OnGameOver: " + onboardingStep);
            //if (onboardingStep == 3)
            //{
            //    StartCoroutine(NextStepWithWait(2));
            //}
            //else if (onboardingStep == 7)
            //{
            //    if (GameManager.Instance.activeGame.gameState.Winner == PlayerEnum.ONE)
            //    {
            //        Debug.Log("GameManager.Instance.activeGame.gameState.winner: " + GameManager.Instance.activeGame.gameState.Winner);
            //        AnalyticsManager.LogOnboardingComplete(true, onboardingStage, onboardingStep);
            //        StartCoroutine(NextStepWithWait(2));
            //    }
            //}
        }
    }
}