using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Core;
using GameSparks.Api.Responses;
using System.Linq;
using TMPro;

namespace Fourzy
{
    public class ViewMatchMaking : UIView
    {
        public static ViewMatchMaking instance;
        public TextMeshProUGUI userFacingMessage;
        public TextMeshProUGUI timerText;
        public GameObject backButton;
        private int findChallengerErrorCount = 0;
        private int joinChallengeErrorCount = 0;
        private int getChallengeError = 0;
        private string challengeIdToJoin;
        private float startTime;
        private float elapsedTime;
        private float minimumTimerTime = 3f;

        // Use this for initialization
        void Start()
        {
            instance = this;
            keepHistory = false;
        }

        public override void Show()
        {
            base.Show();
            startTime = Time.time;
            userFacingMessage.text = "Finding Match...";
            backButton.SetActive(false);
            ChallengeManager.instance.FindRandomChallenge(FindChallengeSuccess, FindChallengeError);
        }

        public override void Hide()
        {
            base.Hide();
            timerText.text = "0";
        }

		private void Update()
		{
            if (isVisible) {
                elapsedTime = Time.time - startTime;
                string seconds = (elapsedTime % 60).ToString("f0");
                timerText.text = seconds;                
            }
		}

		public void BackButton()
        {
            ViewController.instance.ChangeView(ViewController.instance.view3);
            ViewController.instance.ShowTabView();
        }

        private void FindChallengeSuccess(FindChallengeResponse response)
        {
            //Debug.Log("ViewMatchMaking : FindChallengeSuccess");
            challengeIdToJoin = "";

            GSEnumerable<FindChallengeResponse._Challenge> challengeInstances = response.ChallengeInstances;
            //GSData scriptData = response.ScriptData; 

            if (challengeInstances.Count() > 0)
            {
                List<string> challengeInstanceIds = new List<string>();

                //for every object in the challenges array, get the challengeId field and push to challengeInstanceId[]
                foreach (var chalInstance in challengeInstances)
                {
                    challengeInstanceIds.Add(chalInstance.ChallengeId);
                }

                int randNum = UnityEngine.Random.Range(0, challengeInstanceIds.Count - 1);

                //reference the id at that random numbers location
                challengeIdToJoin = challengeInstanceIds[randNum];
                //each time you run this code, a different id is set in the scriptdata
                //Spark.setScriptData("challenge to join", randomChallengeId);

                // For now players are joined to a random challenge
                ChallengeManager.instance.JoinChallenge(challengeIdToJoin, JoinChallengeSuccess, JoinChallengeError);
            }
            else
            {
                if (elapsedTime < minimumTimerTime) {
                    Invoke("OpenNewMultiplayerGame", minimumTimerTime - elapsedTime);
                    Invoke("Hide", minimumTimerTime - elapsedTime);
                } else {
                    //Send player to Game Screen to make the first move
                    ChallengeManager.instance.OpenNewMultiplayerGame();
                    Hide();
                }
            }
        }

        private void OpenNewMultiplayerGame() {
            ChallengeManager.instance.OpenNewMultiplayerGame();
        }

        private void FindChallengeError(FindChallengeResponse response)
        {
            findChallengerErrorCount++;
            Debug.Log("***** Error Finding Random Challenge: " + response.Errors.JSON);
            AnalyticsManager.LogError("find_challenge_request_error", response.Errors.JSON);

            if (findChallengerErrorCount < 2) {
                ChallengeManager.instance.FindRandomChallenge(FindChallengeSuccess, FindChallengeError);
            } else {
                userFacingMessage.text = response.Errors.JSON;
                isVisible = false;
                timerText.text = "";
                backButton.SetActive(true);
            }
        }

        private void JoinChallengeSuccess(JoinChallengeResponse response) {
            //Debug.Log("ViewMatchMaking : JoinChallengeSuccess");
            // GameManager.instance.challengeInstanceId = challengeIdToJoin;
            //Send Player to Game Screen to make a move
            ChallengeManager.instance.GetChallenge(challengeIdToJoin, GetChallengeSuccess, GetChallengeError);
        }

        private void JoinChallengeError(JoinChallengeResponse response)
        {
            joinChallengeErrorCount++;
            Debug.Log("***** Error Joining Challenge: " + response.Errors.JSON);
            AnalyticsManager.LogError("join_challenge_request_error", response.Errors.JSON);

            if (joinChallengeErrorCount < 2) {
                ChallengeManager.instance.JoinChallenge(challengeIdToJoin, JoinChallengeSuccess, JoinChallengeError);
            } else {
                userFacingMessage.text = response.Errors.JSON;
                isVisible = false;
                timerText.text = "";
                backButton.SetActive(true);
            }
        }

        private void GetChallengeSuccess(GetChallengeResponse response) {
            //Debug.Log("ViewMatchMaking : GetChallengeSuccess");
            var challenge = response.Challenge;
            GSData scriptData = response.ScriptData;

            ChallengeManager.instance.challenge = challenge;

            if (elapsedTime < minimumTimerTime)
            {
                Invoke("OpenJoinedMultiplayerGame", minimumTimerTime - elapsedTime);
                Invoke("Hide", minimumTimerTime - elapsedTime);
            } else {
                ChallengeManager.instance.OpenJoinedMultiplayerGame();
                Hide();
            }
        }

        private void OpenJoinedMultiplayerGame() {
            ChallengeManager.instance.OpenJoinedMultiplayerGame();
        }

        private void GetChallengeError(GetChallengeResponse response) {
            getChallengeError++;
            Debug.Log("***** Error Getting Challenge: " + response.Errors);
            AnalyticsManager.LogError("get_challenge_request_error", response.Errors.JSON);

            if (getChallengeError < 2) {
                ChallengeManager.instance.GetChallenge(challengeIdToJoin, GetChallengeSuccess, GetChallengeError);
            } else {
                userFacingMessage.text = response.Errors.JSON;
                isVisible = false;
                timerText.text = "";
                backButton.SetActive(true);
            }
        }
    }
}
