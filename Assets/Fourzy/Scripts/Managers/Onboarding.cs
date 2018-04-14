using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Fourzy
{
    public class Onboarding : MonoBehaviour
    {
        public GameObject bg_dim;
        public GameObject infoTextObject;
        public GameObject hintTextObject;
        public GameObject hand;
        public GameObject gameScreenBackButton;
        public GameObject tabAreaAnim;
        public GameObject dialogBox;
        public GameObject wizard;
        private int moveCount;
        TextMeshProUGUI infoText;
        TextMeshProUGUI hintText;
        GameObject onboarding;

        void Start()
        {
            Button btn = bg_dim.GetComponent<Button>();
            btn.onClick.AddListener(ContinueFlow);

            infoText = infoTextObject.GetComponent<TextMeshProUGUI>();
            hintText = hintTextObject.GetComponent<TextMeshProUGUI>();

            gameScreenBackButton.SetActive(false);
            onboarding = this.gameObject;
        }

		private void OnEnable()
		{
            GameManager.OnStartMove += MoveStarted;
            GameManager.OnEndMove += MoveEnded;
		}

		private void OnDisable()
		{
            GameManager.OnStartMove -= MoveStarted;
            GameManager.OnEndMove -= MoveEnded;
		}

		public void StartOnboarding()
        {
            GameManager.instance.disableInput = true;
            GameManager.instance.isOnboardingActive = true;
            Debug.Log("GameManager.instance.isLoadingUI: " + GameManager.instance.isLoadingUI);
            this.gameObject.SetActive(true);
        }

        public void CompleteOnboarding() {
            gameScreenBackButton.SetActive(true);
        }

        void ContinueFlow()
        {
            bg_dim.SetActive(false);
            infoText.SetText("Tap any square on the perimeter of the board to make a move.");
            hintText.SetText("");
            hand.SetActive(true);
            tabAreaAnim.SetActive(true);
            GameManager.instance.disableInput = false;
        }

        void MoveStarted() {
            moveCount++;
            Debug.Log("MoveStarted movecount: " + moveCount);
            switch (moveCount)
            {
                case 1:
                    dialogBox.SetActive(false);
                    hand.SetActive(false);
                    tabAreaAnim.SetActive(false);
                    wizard.SetActive(false);
                    break;
                case 3:
                    hand.SetActive(false);
                    tabAreaAnim.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        void MoveEnded() {
            Debug.Log("MoveEnded movecount: " + moveCount);
            switch (moveCount)
            {
                case 2:
                    hand.SetActive(true);
                    tabAreaAnim.SetActive(true);
                    break;
                case 4:
                    hand.SetActive(true);
                    tabAreaAnim.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        void Complete()
        {
            gameScreenBackButton.SetActive(true);
        }
    }
}