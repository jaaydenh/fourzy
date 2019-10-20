﻿//@vadym udod

using System;
using System.Collections;
using Fourzy._Updates.UI.Widgets;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class FastPuzzlesScreen : MenuScreen
    {
        public LeaderboardPlayerWidget leaderboardWidgetPrefab;
        public RectTransform widgetsParent;

        public void StartFastPuzzleGame() => GameManager.Instance.StartGame(GameContentManager.Instance.GetNextFastPuzzle());

        public override void Open()
        {
            base.Open();

            StartRoutine("showLB", ShowLeaderboard());
        }

        private void OnError(PlayFabError error)
        {
            Debug.Log(error.ErrorMessage);
        }

        private void OnLeaderboardLoaded(GetLeaderboardAroundPlayerResult leaderboardRequestResult)
        {
            if (leaderboardRequestResult.Version != PlayerPrefsWrapper.GetFastPuzzlesLeaderboardVersion())
            {
                PlayerPrefsWrapper.SetFastPuzzlesLeaderboardVersion(leaderboardRequestResult.Version);

                //reset all
                GameContentManager.Instance.ResetFastPuzzles();
            }

            foreach (WidgetBase widget in widgets) Destroy(widget.gameObject);
            widgets.Clear();

            foreach (PlayerLeaderboardEntry entry in leaderboardRequestResult.Leaderboard)
                widgets.Add(Instantiate(leaderboardWidgetPrefab, widgetsParent).SetData(entry));
        }

        private IEnumerator ShowLeaderboard()
        {
            float timer = 0f;
            //wait 10 seconds
            while ((string.IsNullOrEmpty(LoginManager.playfabID) || !GameManager.NetworkAccess) && timer < 10f)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (timer >= 10f || !isOpened) yield break;

            //show leaderboard
            PlayFabClientAPI.GetLeaderboardAroundPlayer(
                new PlayFab.ClientModels.GetLeaderboardAroundPlayerRequest() { StatisticName = "PuzzlesLB", MaxResultsCount = 6 }, 
                OnLeaderboardLoaded,
                OnError);
        }
    }
}