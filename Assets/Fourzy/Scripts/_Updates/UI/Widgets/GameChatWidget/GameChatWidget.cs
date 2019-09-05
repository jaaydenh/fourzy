﻿
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class GameChatWidget : MonoBehaviour
    {
        public Button btnOpenChat;
        public Button btnSendMessage;
        public TMP_InputField chatInputField;
        public TMP_Text opponentText;

        private Queue<string> messages = new Queue<string>();
        private bool isChatOpen;

        private void Start()
        {
            if (GameManager.Instance.activeGame == null || GameManager.Instance.activeGame._Type != GameType.REALTIME)
            {
                this.gameObject.SetActive(false);
                return;
            }

            opponentText.text = string.Empty;
            chatInputField.gameObject.SetActive(isChatOpen);
            btnSendMessage.gameObject.SetActive(isChatOpen);

            this.StartCoroutine(ShowChatMessages());
        }


        //private void OnEnable()
        //{
        //    RealtimeManager.OnChatMessageReceived += RealtimeManager_OnChatMessageReceived;
        //}

        //private void OnDisable()
        //{
        //    RealtimeManager.OnChatMessageReceived -= RealtimeManager_OnChatMessageReceived;
        //}

        void RealtimeManager_OnChatMessageReceived(string chatMessage)
        {
            messages.Enqueue(chatMessage);
        }

        private IEnumerator ShowChatMessages()
        {
            const float delayForNextMessage = 0.7f;

            while (true)
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

            //opponentMessage.rectTransform.DOMoveY(opponentText.rectTransform.position.y + 0.5f, 3.0f);

            yield return new WaitForSeconds(3.0f);

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
                //Mixpanel.Track("Send Message Button Press");
                //RealtimeManager.Instance.SendChatMessage(chatInputField.text);
            }

            isChatOpen = !isChatOpen;
            chatInputField.gameObject.SetActive(isChatOpen);
            btnSendMessage.gameObject.SetActive(isChatOpen);
            chatInputField.text = string.Empty;
        }
    }
}
