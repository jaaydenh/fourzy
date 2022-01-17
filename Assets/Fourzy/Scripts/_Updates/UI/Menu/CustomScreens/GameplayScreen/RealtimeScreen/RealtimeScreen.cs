//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Widgets;
using System.Collections;
using TMPro;
using UnityEngine;

#if !MOBILE_SKILLZ
using Photon.Pun;
#endif

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class RealtimeScreen : MenuScreen
    {
        public TMP_Text statusLabel;
        public RandomPickWidget randomPickWidget;

        private AlphaTween statusAlphaTween;
        private ScaleTween statusScaleTween;
        private RotationTween statusRotationTween;

        protected override void Awake()
        {
            base.Awake();

            statusAlphaTween = statusLabel.GetComponent<AlphaTween>();
            statusScaleTween = statusLabel.GetComponent<ScaleTween>();
            statusRotationTween = statusLabel.GetComponent<RotationTween>();
        }

        public void Open(IClientFourzy game)
        {
            switch (game._Type)
            {
                case GameType.REALTIME:
                    randomPickWidget.SetData(game.unactivePlayer.DisplayName, game.activePlayer.DisplayName);

                    break;
            }
        }

        public override void Open()
        {
            base.Open();

            statusAlphaTween.StopTween(true);
        }

        public void GameComplete()
        {

        }

        public void SetMessage(string text)
        {
#if !MOBILE_SKILLZ
            if (PhotonNetwork.CurrentRoom == null) return;
#endif

            if (!FourzyPhotonManager.CheckPlayersReady())
            {
                menuController.OpenScreen(this);

                statusLabel.text = text;

                //animate
                statusAlphaTween.repeat = RepeatType.PING_PONG;
                statusAlphaTween.PlayForward(true);
            }
        }

        public Coroutine StartCountdown(float duration)
        {
            if (menuController.currentScreen != this)
            {
                menuController.OpenScreen(this);
            }

            return StartRoutine("countdownRoutine", CountdownRoutine(duration));
        }

        private IEnumerator CountdownRoutine(float duration)
        {
            statusLabel.text = "";

            //1.2 seconds delay after player picked
            yield return new WaitForSeconds(randomPickWidget.StartPick(.8f) + 1.2f);
            randomPickWidget.Hide(.2f);

            yield return new WaitForSeconds(duration % 1f);

            statusAlphaTween.playbackTime = .5f;
            statusAlphaTween.repeat = RepeatType.PING_PONG;
            statusLabel.fontSize = 182f;

            for (int timer = Mathf.FloorToInt(duration); timer > 0; timer--)
            {
                statusAlphaTween.PlayForward(true);
                statusScaleTween.PlayForward(true);
                statusRotationTween.PlayForward(true);

                if (timer > 1)
                {
                    statusLabel.text = (timer - 1) + "";
                }
                else
                {
                    statusLabel.text = "GO!";
                }

                yield return new WaitForSeconds(1f);
            }

            statusLabel.fontSize = 42f;
            statusAlphaTween.StopTween(true);
            statusScaleTween.StopTween(true);

            CloseSelf();
        }
    }
}