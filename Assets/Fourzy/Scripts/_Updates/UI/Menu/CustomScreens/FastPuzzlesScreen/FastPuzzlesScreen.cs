//@vadym udod

using System;
using System.Collections;
using Fourzy._Updates.Managers;
using Fourzy._Updates.UI.Helpers;
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
        public Badge noEntries;

        public void StartFastPuzzleGame() => GameManager.Instance.StartGame(GameContentManager.Instance.GetNextFastPuzzle());

        protected override void Awake()
        {
            base.Awake();

            GameManager.onNetworkAccess += NetworkAccess;
        }

        public override void Open()
        {
            base.Open();

            StartRoutine("showLB", ShowLeaderboard());

            HeaderScreen.instance.Close();
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

            ClearEntries();

            if (leaderboardRequestResult.Leaderboard.Count < 2)
            {
                noEntries.SetState(true);
                return;
            }
            else
                noEntries.SetState(false);

            foreach (PlayerLeaderboardEntry entry in leaderboardRequestResult.Leaderboard)
                widgets.Add(Instantiate(leaderboardWidgetPrefab, widgetsParent).SetData(entry));
        }

        private void NetworkAccess(bool state)
        {
            if (isOpened)
            {
                if (state)
                    StartRoutine("showLB", ShowLeaderboard());
                else
                    ClearEntries();
            }
        }

        private void ClearEntries()
        {
            foreach (WidgetBase widget in widgets) Destroy(widget.gameObject);
            widgets.Clear();
        }

        private IEnumerator ShowLeaderboard()
        {
            NetworkManager.instance.FastCheck();

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