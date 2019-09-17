//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics;
using FourzyGameModel.Model;
using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class ChallengeManager : UnitySingleton<ChallengeManager>
    {
        public static Action<List<ChallengeData>> OnChallengesUpdate;

        //when challenge update received from a server
        public static Action<ChallengeData> OnChallengeUpdate;
        //when challenge was modified localy
        public static Action<ChallengeData> OnChallengeUpdateLocal;

        public static Action<string, string> OnReceivedOpponentGamepiece;
        public static Action<bool> OnFindChallengeResult;
        public static Action<int> OnReceivedRatingDelta;

        public static Action<ChallengeTurnTakenMessage> OnChallengeTakeTurn;
        public static Action<ChallengeJoinedMessage> OnChallengeJoined;
        public static Action<ChallengeWonMessage> OnChallengeWon;
        public static Action<ChallengeLostMessage> OnChallengeLost;
        public static Action<ChallengeIssuedMessage> OnChallengeIssued;
        public static Action<ChallengeDrawnMessage> OnChallengeDrawn;
        public static Action<ChallengeStartedMessage> OnChallengeStarted;

        public double daysUntilChallengeExpires = 60;

        private List<ChallengeData> challenges = new List<ChallengeData>();
        private bool gettingChallenges = false;

        public List<ChallengeData> Challenges
        {
            get
            {
                return challenges;
            }
            set
            {
                challenges = value;

                OnChallengesUpdate?.Invoke(challenges);
            }
        }

        public List<ChallengeData> NextChallenges => challenges.Where(challenge => challenge.canBeNext).ToList();

        protected void OnEnable()
        {
            ChallengeTurnTakenMessage.Listener += _OnChallengeTurnTaken;
            ChallengeJoinedMessage.Listener += _OnChallengeJoined;
            ChallengeWonMessage.Listener += _OnChallengeWon;
            ChallengeLostMessage.Listener += _OnChallengeLost;
            ChallengeIssuedMessage.Listener += _OnChallengeIssued;
            ChallengeDrawnMessage.Listener += _OnChallengeDrawn;
            ChallengeStartedMessage.Listener += _OnChallengeStarted;
        }

        protected void OnDisable()
        {
            ChallengeTurnTakenMessage.Listener -= _OnChallengeTurnTaken;
            ChallengeJoinedMessage.Listener -= _OnChallengeJoined;
            ChallengeWonMessage.Listener -= _OnChallengeWon;
            ChallengeLostMessage.Listener -= _OnChallengeLost;
            ChallengeDrawnMessage.Listener -= _OnChallengeDrawn;
            ChallengeIssuedMessage.Listener -= _OnChallengeIssued;
            ChallengeStartedMessage.Listener -= _OnChallengeStarted;
        }

        public void CreateTurnBasedGame(string oppponentID, Area selectedArea, Action<LogEventResponse> successCallback, Action<LogEventResponse> errorCallback)
        {
            AnalyticsManager.Instance.LogCreateGame(GameType.TURN_BASED, selectedArea, UserManager.Instance.userId, oppponentID);

            new LogEventRequest().SetEventKey("createTurnBased")
                // .SetDurable(true)
                .SetMaxResponseTimeInMillis(15000)
                .SetEventAttribute("opponentId", oppponentID)
                .SetEventAttribute("area", (long)selectedArea)
                .Send(successCallback, errorCallback);
        }

        public void GetChallengeRequest(string challengeInstanceId)
        {
            new GetChallengeRequest()
                .SetChallengeInstanceId(challengeInstanceId)
                .SetMaxResponseTimeInMillis(17000)
                .Send((response) =>
                {
                    CheckChallenge(response.Challenge.BaseData);
                }, 
                (_response) =>
                {
                    Debug.Log("***** Error Challenge Request: " + _response.Errors.JSON);
                });
        }

        public void GetChallengesRequest()
        {
            if (gettingChallenges || !GameManager.NetworkAccess)
                return;

            gettingChallenges = true;

            List<string> challengeStates = new List<string> { "RUNNING" };
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

            //We send a ListChallenge Request with the shortcode of our challenge, we set this in our GameSparks Portal
            new ListChallengeRequest()
                .SetShortCode("chalRanked")
                .SetMaxResponseTimeInMillis(20000)
                .SetStates(challengeStates)
                .SetEntryCount(Constants.running_challenges_count)
                .Send(((response) =>
                {
                    if (response.HasErrors)
                    {
                        gettingChallenges = false;

                        Debug.Log("***** Error Listing Challenges Request: " + response.Errors.JSON);
                        AnalyticsManager.Instance.LogError(response.Errors.JSON, AnalyticsManager.AnalyticsErrorType.challenge_manager);

                        GetChallengesRequest();
                        return;
                    }

                    List<ChallengeData> _challenges = new List<ChallengeData>();

                    Debug.Log($"Challenges found: {response.ChallengeInstances.Count()}");

                    foreach (var gsChallenge in response.ChallengeInstances)
                    {
                        if (!ChallengeData.ValidateGSData(gsChallenge.BaseData)) continue;

                        ChallengeData newChallenge = new ChallengeData(gsChallenge.BaseData);

                        if (newChallenge.Validate()) _challenges.Add(newChallenge);
                    }

                    gettingChallenges = false;

                    stopwatch.Stop();
                    //Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                    //customAttributes.Add("endtime", stopwatch.Elapsed);
                    //AnalyticsManager.LogEvent("ListChallengeRequest_response_endtime", customAttributes);

                    stopwatch.Reset();

                    AddCompleteChallenges(_challenges);
                }));
        }

        private void AddCompleteChallenges(List<ChallengeData> runningChallenges)
        {
            if (gettingChallenges || !GameManager.NetworkAccess) return;

            gettingChallenges = true;

            List<string> challengeStates = new List<string> { "COMPLETE" };
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

            //We send a ListChallenge Request with the shortcode of our challenge, we set this in our GameSparks Portal
            new ListChallengeRequest()
                .SetShortCode("chalRanked")
                .SetMaxResponseTimeInMillis(20000)
                .SetStates(challengeStates)
                .SetEntryCount(Constants.complete_challenges_count)
                .Send(((response) =>
                {
                    if (response.HasErrors)
                    {
                        gettingChallenges = false;

                        Debug.Log("***** Error Listing Complete Challenges Request: " + response.Errors.JSON);
                        AnalyticsManager.Instance.LogError(response.Errors.JSON, AnalyticsManager.AnalyticsErrorType.challenge_manager);

                        //start over again
                        GetChallengesRequest();
                        return;
                    }

                    Debug.Log($"Complete challenges found: {response.ChallengeInstances.Count()}");

                    foreach (var gsChallenge in response.ChallengeInstances)
                    {
                        if (!ChallengeData.ValidateGSData(gsChallenge.BaseData)) continue;

                        ChallengeData completeChallenge = new ChallengeData(gsChallenge.BaseData);

                        if (completeChallenge.Validate()) runningChallenges.Add(completeChallenge);
                    }

                    Challenges = runningChallenges;

                    gettingChallenges = false;

                    stopwatch.Stop();
                    //Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                    //customAttributes.Add("endtime", stopwatch.Elapsed);
                    //AnalyticsManager.LogEvent("ListChallengeRequest_response_endtime", customAttributes);

                    stopwatch.Reset();
                }));
        }

        private void _OnChallengeTurnTaken(ChallengeTurnTakenMessage message)
        {
            CheckChallenge(message.Challenge.BaseData);

            OnChallengeTakeTurn?.Invoke(message);
        }

        private void _OnChallengeJoined(ChallengeJoinedMessage message)
        {
            CheckChallenge(message.Challenge.BaseData);

            OnChallengeJoined?.Invoke(message);
        }

        private void _OnChallengeWon(ChallengeWonMessage message)
        {
            CheckChallenge(message.Challenge.BaseData);

            OnChallengeWon?.Invoke(message);
        }

        private void _OnChallengeLost(ChallengeLostMessage message)
        {
            CheckChallenge(message.Challenge.BaseData);

            OnChallengeLost?.Invoke(message);
        }

        private void _OnChallengeDrawn(ChallengeDrawnMessage message)
        {
            CheckChallenge(message.Challenge.BaseData);

            OnChallengeDrawn?.Invoke(message);
        }

        private void _OnChallengeStarted(ChallengeStartedMessage message)
        {
            CheckChallenge(message.Challenge.BaseData);

            OnChallengeStarted?.Invoke(message);
        }

        private void _OnChallengeIssued(ChallengeIssuedMessage message)
        {
            OnChallengeIssued?.Invoke(message);
        }

        private void CheckChallenge(GSData baseData)
        {
            //update game
            ChallengeData _challenge = new ChallengeData(baseData);
            ChallengeData toModify = challenges.Find(__challenge => __challenge.challengeInstanceId == _challenge.challengeInstanceId);

            //if for some reason this challenge is not on a local challenges list
            if (toModify == null)
            {
                challenges.Add(_challenge);

                OnChallengesUpdate?.Invoke(challenges);
            }
            else
            {
                challenges[challenges.IndexOf(toModify)] = _challenge;

                //check if updated game finished
                //if so, check if active game is the one that got update, so it can be set as viewed
                if (GameManager.Instance.activeGame != null && GameManager.Instance.activeGame._Type == GameType.TURN_BASED)
                {
                    if (GameManager.Instance.activeGame.GameID == _challenge.challengeInstanceId && _challenge.lastTurnGame.isOver)
                        PlayerPrefsWrapper.SetGameViewed(_challenge.challengeInstanceId);
                }

                OnChallengeUpdate?.Invoke(_challenge);
            }
        }
    }
}