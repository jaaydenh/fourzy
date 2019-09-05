//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.UI.Helpers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class TurnBaseScreen : MenuScreen
    {
        public ButtonExtended nextButton;
        public TMP_Text challengeIDLabel;

        private IClientFourzy game;
        [HideInInspector]
        public ChallengeData nextChallenge;

        protected override void Awake()
        {
            base.Awake();

            ChallengeManager.OnChallengesUpdate += OnUpdateChallenges;
            ChallengeManager.OnChallengeUpdate += OnChallengeUpdate;
        }

        protected void OnDestroy()
        {
            if (game != null)
                switch (game._Type)
                {
                    case GameType.TURN_BASED:
                        ChallengeManager.OnChallengesUpdate -= OnUpdateChallenges;
                        ChallengeManager.OnChallengeUpdate -= OnChallengeUpdate;
                        break;
                }
        }

        public void Open(IClientFourzy game)
        {
            this.game = game;

            switch (game._Type)
            {
                case GameType.TURN_BASED:
                    challengeIDLabel.text = $"ChallengeID: {game.GameID}";

                    OnUpdateChallenges(ChallengeManager.Instance.Challenges);
                    Open();
                    break;
            }
        }

        public void GameComplete()
        {
            HideButton();
        }

        public void OpenNext()
        {
            if (nextChallenge != null) GamePlayManager.instance.LoadGame(nextChallenge.GetGameForPreviousMove());
        }

        public void HideButton()
        {
            nextButton.SetActive(false);
        }

        private void OnUpdateChallenges(List<ChallengeData> challenges)
        {
            if (game._Type != GameType.TURN_BASED) return;

            nextChallenge = null;
            int currentChallengeIndex = challenges.FindIndex(challenge => challenge.challengeInstanceId == game.GameID);
            for (int challengeIndex = 1; challengeIndex < challenges.Count; challengeIndex++)
            {
                ChallengeData _challenge;

                if (currentChallengeIndex + challengeIndex < challenges.Count)
                    _challenge = challenges[currentChallengeIndex + challengeIndex];
                else
                    _challenge = challenges[challengeIndex - (challenges.Count - currentChallengeIndex)];

                if (_challenge.canBeNext)
                {
                    nextChallenge = _challenge;
                    break;
                }
            }

            nextButton.SetActive(nextChallenge != null);
        }

        private void OnChallengeUpdate(ChallengeData challengeData)
        {
            switch (game._Type)
            {
                case GameType.TURN_BASED:
                    //if there is no next challenge, check if challengeData can be one
                    if (nextChallenge == null && game.GameID != challengeData.challengeInstanceId)
                    {
                        if (challengeData.canBeNext)
                        {
                            nextChallenge = challengeData;
                            nextButton.SetActive(true);
                        }
                    }
                    break;
            }
        }
    }
}
