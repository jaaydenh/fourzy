//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Tween;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class RealtimeScreen : MenuScreen
    {
        public TMP_Text statusLabel;

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

        public void Open(IClientFourzy game) { }

        public override void Open()
        {
            base.Open();

            statusAlphaTween.StopTween(true);
        }

        public void _Update()
        {

        }

        public void GameComplete()
        {

        }

        public void CheckWaitingForOtherPlayer()
        {
            if (PhotonNetwork.CurrentRoom == null) return;

            if (!FourzyPhotonManager.CheckPlayersReady())
            {
                menuController.OpenScreen(this);

                statusLabel.text = "Waiting for other player...";

                //animate
                statusAlphaTween.repeat = RepeatType.PING_PONG;
                statusAlphaTween.PlayForward(true);
            }
        }

        public Coroutine StartCountdown(float duration)
        {
            if (menuController.currentScreen != this) menuController.OpenScreen(this);

            return StartRoutine("countdownRoutine", CountdownRoutine(duration));
        }

        private IEnumerator CountdownRoutine(float duration)
        {
            yield return new WaitForSeconds(duration % 1f);

            statusAlphaTween.playbackTime = .5f;
            statusAlphaTween.repeat = RepeatType.PING_PONG;

            statusScaleTween.repeat = RepeatType.PING_PONG;

            statusLabel.fontSize = 182f;

            for (int timer = Mathf.FloorToInt(duration); timer > 0; timer--)
            {
                statusAlphaTween.PlayForward(true);
                statusScaleTween.PlayForward(true);
                statusRotationTween.PlayForward(true);

                if (timer > 1)
                    statusLabel.text = (timer - 1) + "";
                else
                    statusLabel.text = "GO!";

                yield return new WaitForSeconds(1f);
            }

            statusLabel.fontSize = 42f;
            statusAlphaTween.StopTween(true);
            statusScaleTween.StopTween(true);

            CloseSelf();
        }
    }
}