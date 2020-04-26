//@vadym udod

using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class RankingScreen : MenuTab
    {
        public TMP_Text noPlayerText;
        public LeaderboardPlayerWidget playerLeaderboardWidgetPrefab;
        public RectTransform widgetsParent;
        public ToggleExtended winsButton;

        private bool loadingLeaderboard = false;

        public override void Open()
        {
            base.Open();

            GameManager.onNetworkAccess += OnNetwowkAccess;

            InvokeCurrent();
        }

        public override void Close(bool animate)
        {
            base.Close(animate);

            GameManager.onNetworkAccess -= OnNetwowkAccess;
        }

        public void GetWinsLeaderboard()
        {
            if (loadingLeaderboard || !isCurrent || !GameManager.NetworkAccess)
                return;

            loadingLeaderboard = true;

            // LoginManager.Instance.GetWinsLeaderboard(GetLeaderboardCallback);
        }

        public void GetTrophiesLeaderboard()
        {
            if (loadingLeaderboard || !isCurrent || !GameManager.NetworkAccess)
                return;

            loadingLeaderboard = true;

            // LoginManager.Instance.GetCoinsEarnedLeaderboard(GetLeaderboardCallback);
        }

        public void InvokeCurrent()
        {
            //remove old ones
            foreach (Transform playerWidget in widgetsParent)
                Destroy(playerWidget.gameObject);

            if (winsButton.isOn)
                GetWinsLeaderboard();
            else
                GetTrophiesLeaderboard();
        }

        private void GetLeaderboardCallback(List<LeaderboardEntry> leaderboards, string errorMessage)
        {
            loadingLeaderboard = false;

            //remove old ones
            foreach (Transform playerWidget in widgetsParent)
                Destroy(playerWidget.gameObject);

            if (leaderboards == null)
            {
                noPlayerText.text = errorMessage;
                noPlayerText.gameObject.SetActive(true);
                return;
            }

            noPlayerText.gameObject.SetActive(leaderboards.Count == 0);

            foreach (LeaderboardEntry leaderboard in leaderboards)
            {
                LeaderboardPlayerWidget leaderboardPlayerWidget = Instantiate(playerLeaderboardWidgetPrefab, widgetsParent);

                leaderboardPlayerWidget.SetData(leaderboard);
            }
        }

        private void OnNetwowkAccess(bool state)
        {
            if (state)
                InvokeCurrent();
        }

        public class LeaderboardEntry
        {
            public string userId;
            public string userName;
            public string facebookId;
            public long rank;
            public bool isWinsLeaderboard;

            public string value;
        }
    }
}