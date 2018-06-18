using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Fourzy
{
    public class Onboarding : MonoBehaviour
    {
        public GameObject bg_dim;
        public GameObject fullscreenButton;
        public GameObject infoTextObject;
        public GameObject hintTextObject;
        public GameObject hand;
        public GameObject gameScreenBackButton;
        public GameObject tapAreaAnim;
        public GameObject dialogBox;
        public GameObject wizard;
        private int moveCount;
        private int onboardingStep;
        private int onboardingStage;
        private int step7Attempts;
        TextMeshProUGUI infoText;
        TextMeshProUGUI hintText;
        GameObject onboarding;

        //void Awake()
        //{
        //    Debug.Log("Onboarding:Awake");
        //}

		private void OnEnable()
		{
            GamePlayManager.OnStartMove += MoveStarted;
            GamePlayManager.OnEndMove += MoveEnded;
            GamePlayManager.OnGameOver += OnGameOver;
		}

		private void OnDisable()
		{
            GamePlayManager.OnStartMove -= MoveStarted;
            GamePlayManager.OnEndMove -= MoveEnded;
		}

		public void StartOnboarding()
        {
            Debug.Log("Onboarding:StartOnboarding");
            GameManager.instance.isOnboardingActive = true;

            onboardingStep = PlayerPrefs.GetInt("onboardingStep");
            onboardingStage = PlayerPrefs.GetInt("onboardingStage");

            Debug.Log("onboardingStage: " + onboardingStage + ", onboardingStep: " + onboardingStep);

            AnalyticsManager.LogOnboardingStart(onboardingStage, onboardingStep);

            Button btn = fullscreenButton.GetComponent<Button>();
            btn.onClick.AddListener(NextStep);

            infoText = infoTextObject.GetComponent<TextMeshProUGUI>();
            hintText = hintTextObject.GetComponent<TextMeshProUGUI>();

            gameScreenBackButton.SetActive(false);
            onboarding = this.gameObject;

            //onboardingStep = 0;
            if (onboardingStep == 0) {
                onboardingStep = 1;
            }
            step7Attempts = 0;
            this.gameObject.SetActive(true);
            DoNextStep();
        }

        public void CompleteOnboarding() {
            gameScreenBackButton.SetActive(true);
        }

        void NextStep() {
            onboardingStep++;
            DoNextStep();
        }

        void DoNextStep()
        {
            Debug.Log("NextStep: step:" + onboardingStep);
            PlayerPrefs.SetInt("onboardingStep", onboardingStep);
            AnalyticsManager.LogOnboardingStart(onboardingStage, onboardingStep);

            switch (onboardingStep)
            {
                case 1:
                    wizard.transform.localPosition = new Vector3(0, 221);
                    dialogBox.transform.localPosition = new Vector3(0, -118);
                    GamePlayManager.Instance.disableInput = true;
                    bg_dim.SetActive(true);
                    fullscreenButton.SetActive(true);
                    wizard.SetActive(true);
                    dialogBox.SetActive(true);
                    infoText.SetText("Welcome to Fourzy!");
                    hintText.SetText("tap to continue...");
                    break;
                case 2:
                    infoText.SetText("First let's watch a quick game to learn how to play.");
                    hintText.SetText("tap to continue...");
                    break;
                case 3:
                    bg_dim.SetActive(false);
                    fullscreenButton.SetActive(false);
                    dialogBox.SetActive(false);
                    wizard.SetActive(false);
                    StartCoroutine(GamePlayManager.Instance.PlayInitialMoves());
                    break;
                case 4:
                    // GameManager.instance.gameType = GameType.PASSANDPLAY;
                    GameManager.instance.OpenNewGame(GameType.PASSANDPLAY, null, false, "1000");
                    moveCount = 0;
                    gameScreenBackButton.SetActive(false);
                    bg_dim.SetActive(false);
                    fullscreenButton.SetActive(false);
                    dialogBox.SetActive(true);
                    wizard.SetActive(true);
                    infoText.SetText("Now you try it. Tap this square on the perimeter of the board to make a move.");
                    hintText.SetText("");
                    TapHintPosition(-313, 136);
                    GamePlayManager.Instance.disableInput = false;
                    break;
                case 6:
                    GamePlayManager.Instance.disableInput = true;
                    hand.SetActive(false);
                    tapAreaAnim.SetActive(false);
                    bg_dim.SetActive(true);
                    fullscreenButton.SetActive(true);
                    wizard.SetActive(true);
                    dialogBox.SetActive(true);
                    infoText.SetText("To win Fourzy you must get 4 in a row, horizontally, vertically, or diagonally. Try to win vertically.");
                    hintText.SetText("tap to continue...");
                    break;
                case 7:
                    // GameManager.instance.gameType = GameType.PASSANDPLAY;
                    GameManager.instance.OpenNewGame(GameType.PASSANDPLAY, null, false, "101");
                    moveCount = 0;
                    gameScreenBackButton.SetActive(false);
                    GamePlayManager.Instance.disableInput = false;
                    bg_dim.SetActive(false);
                    fullscreenButton.SetActive(false);
                    wizard.SetActive(false);
                    dialogBox.SetActive(false);
                    if (step7Attempts > 0) {
                        TapHintPosition(45, 313);
                    }
                    break;
                case 8:
                    ShowWizardWithDialog("Congratulations!!! Now you know the basics of Fourzy.");
                    break;
                case 9:
                    GamePlayManager.Instance.BackButtonOnClick();
                    wizard.transform.localPosition = new Vector3(0, 360);
                    dialogBox.transform.localPosition = new Vector3(0, 15);
                    ShowWizardWithDialog("Try challenging other players by pressing Play or press Training for more practice.");
                    break;
                case 10:
                    GamePlayManager.Instance.BackButtonOnClick();
                    ShowWizardWithDialog("Fourzy is an early access turn-based game. We recommended starting several games as it can take extra time to find an opponent.");
                    break;
                case 11:
                    HideWizardDialog();
                    PlayerPrefs.SetInt("onboardingStage", 2);
                    GameManager.instance.isOnboardingActive = false;
                    break;
                default:
                    break;
            }
        }

        void MoveStarted() {
            
            Debug.Log("MoveStarted: onboardingStep: " + onboardingStep);
            if (onboardingStep >= 4 && onboardingStep <= 6) {
                moveCount++;
                Debug.Log("MoveStarted movecount: " + moveCount);
                switch (moveCount)
                {
                    case 1:
                        dialogBox.SetActive(false);
                        hand.SetActive(false);
                        tapAreaAnim.SetActive(false);
                        wizard.SetActive(false);
                        NextStep();
                        break;
                    case 3:
                        hand.SetActive(false);
                        tapAreaAnim.SetActive(false);
                        onboardingStep++;
                        break;
                    default:
                        break;
                }
            } else if (onboardingStep == 7) {
                hand.SetActive(false);
                tapAreaAnim.SetActive(false);
            }
        }

        void MoveEnded()
        {
            Debug.Log("MoveEnded: onboardingStep: " + onboardingStep);
            if (onboardingStep >= 4 && onboardingStep <= 6)
            {
                Debug.Log("MoveEnded movecount: " + moveCount);
                switch (moveCount)
                {
                    case 1:
                        Move move1 = new Move(4, Direction.UP, PlayerEnum.TWO);
                        GameManager.instance.CallMovePiece(move1, false, false);
                        break;
                    case 2:
                        TapHintPosition(44, -302);
                        break;
                    case 3:
                        Move move2 = new Move(2, Direction.DOWN, PlayerEnum.TWO);
                        GameManager.instance.CallMovePiece(move2, false, false);
                        break;
                    case 4:
                        hand.SetActive(true);
                        tapAreaAnim.SetActive(true);
                        DoNextStep();
                        break;
                    default:
                        break;
                }
            }
            else if (onboardingStep == 7 && GameManager.instance.activeGame.gameState.Winner != PlayerEnum.ONE)
            {
                AnalyticsManager.LogOnboardingComplete(false, onboardingStage, onboardingStep);
                onboardingStep--;
                ShowWizardWithDialog("Try Again, you must get 4 in a row to win");
                step7Attempts++;
            }
        }

        void TapHintPosition(int x, int y) {
            hand.SetActive(true);
            tapAreaAnim.SetActive(true);
            tapAreaAnim.transform.localPosition = new Vector3(x, y, -5);
            hand.transform.localPosition = new Vector3(x + 102, y - 107);
        }

        void OnGameOver() {
            Debug.Log("Onboarding:OnGameOver: " + onboardingStep);
            if (onboardingStep == 3) {
                StartCoroutine(NextStepWithWait(2));
            } else if (onboardingStep == 7) {
                if (GameManager.instance.activeGame.gameState.Winner == PlayerEnum.ONE) {
                    Debug.Log("GameManager.instance.activeGame.gameState.winner: " + GameManager.instance.activeGame.gameState.Winner);
                    AnalyticsManager.LogOnboardingComplete(true, onboardingStage, onboardingStep);
                    StartCoroutine(NextStepWithWait(2));
                }
            }
        }

        IEnumerator NextStepWithWait(int seconds) {
            yield return new WaitForSeconds(seconds);
            NextStep();
        }
        
        void ShowWizardWithDialog(string dialog) {
            GamePlayManager.Instance.disableInput = true;
            bg_dim.SetActive(true);
            fullscreenButton.SetActive(true);
            wizard.SetActive(true);
            dialogBox.SetActive(true);
            infoText.SetText(dialog);
            hintText.SetText("tap to continue...");
        }

        void HideWizardDialog() {
            GamePlayManager.Instance.disableInput = false;
            bg_dim.SetActive(false);
            fullscreenButton.SetActive(false);
            wizard.SetActive(false);
            dialogBox.SetActive(false);
        }

        void Complete()
        {
            gameScreenBackButton.SetActive(true);
        }
    }
}