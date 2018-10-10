using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Fourzy
{
    public class GameChatView : MonoBehaviour
    {
        [SerializeField] Button btnOpenChat;
        [SerializeField] Button btnSendMessage;
        [SerializeField] TMP_InputField chatInputField;
        [SerializeField] TMP_Text opponentText;

        private Queue<string> messages = new Queue<string>();
        private bool isChatOpen;

        private void Awake()
        {
            if (GameManager.Instance.activeGame.gameState.GameType != GameType.REALTIME)
            {
                this.gameObject.SetActive(false);
                return;
            }

            opponentText.text = string.Empty;
            chatInputField.gameObject.SetActive(isChatOpen);
            btnSendMessage.gameObject.SetActive(isChatOpen);

            this.StartCoroutine(ShowChatMessages());
        }


        private void OnEnable()
        {
            RealtimeManager.OnChatMessageReceived += RealtimeManager_OnChatMessageReceived; 
        }

        private void OnDisable()
        {
            RealtimeManager.OnChatMessageReceived -= RealtimeManager_OnChatMessageReceived;
        }

        void RealtimeManager_OnChatMessageReceived(string chatMessage)
        {
            messages.Enqueue(chatMessage);
        }

        private IEnumerator ShowChatMessages()
        {
            const float delayForNextMessage = 0.7f;

            while(true)
            {
                if (messages.Count != 0)
                {
                    string message = messages.Dequeue();
                    this.StartCoroutine(ShowMessageRoutine(message));
                    yield return new WaitForSeconds(delayForNextMessage);
                }
                yield return null;
            }
        }

        private IEnumerator ShowMessageRoutine(string chatMessage)
        {
            const float fadeTime = 0.5f;

            TMP_Text opponentMessage = Instantiate(opponentText, opponentText.rectTransform.parent, false);
            opponentMessage.alpha = 1.0f;
            opponentMessage.gameObject.SetActive(true);
            opponentMessage.text = chatMessage;

            opponentMessage.rectTransform.DOMoveY(opponentText.rectTransform.position.y + 0.5f, 2.0f);

            yield return new WaitForSeconds(1.5f);

            opponentMessage.CrossFadeAlpha(0.0f, fadeTime, false);

            yield return new WaitForSeconds(fadeTime);

            Destroy(opponentMessage.gameObject);
        }

        public void OpenChatOnClick()
        {
            isChatOpen = !isChatOpen;

            chatInputField.gameObject.SetActive(isChatOpen);
            btnSendMessage.gameObject.SetActive(isChatOpen);

            if (isChatOpen)
            {
                chatInputField.ActivateInputField();
            }
        }

        public void SendMessageOnClick()
        {
            if (chatInputField.text != string.Empty)
            {
                RealtimeManager.Instance.SendChatMessage(chatInputField.text);
            }

            isChatOpen = !isChatOpen;
            chatInputField.gameObject.SetActive(isChatOpen);
            btnSendMessage.gameObject.SetActive(isChatOpen);
            chatInputField.text = string.Empty;

        }
    }
}
