//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.GameplayScene;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SkillzGameScreen : MenuScreen
    {
        [SerializeField]
        private TMP_Text timerLabel;
        [SerializeField]
        private TMP_Text scoreLabel;

        private float gameStartTime;
        private float inPauseTime;
        private float gameTimer;

        public IClientFourzy game { get; private set; }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                inPauseTime = Time.time;
            }
            else
            {
                inPauseTime = Time.time - inPauseTime;
            }
        }

        public void Open(IClientFourzy game)
        {
            if (game == null)
            {
                if (isOpened)
                {
                    Close();
                }

                return;
            }

            base.Open();

            this.game = game;

            gameTimer = SkillzGameController.Instance.GetGameTimerValue;
        }

        private IEnumerator GameTimerRoutine()
        {
            while (gameTimer > 0f)
            {
                if (inPauseTime > 0f)
                {
                    gameTimer -= inPauseTime;
                    inPauseTime = 0f;
                }

                gameTimer -= Time.deltaTime;
                yield return null;
            }

            GamePlayManager.Instance.OnGameFinished(game);
        }
    }
}
