//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class LeaderboardPlayerWidget : WidgetBase
    {
        public TMP_Text playerNameLabel;
        public TMP_Text rankLabel;
        public TMP_Text typeLabel;
        public RectTransform iconParent;

        private RankingScreen.LeaderboardEntry leaderboard;

        public void SetData(RankingScreen.LeaderboardEntry leaderboard)
        {
            this.leaderboard = leaderboard;

            playerNameLabel.text = leaderboard.userName;
            rankLabel.text = leaderboard.rank.ToString();
            typeLabel.text = (leaderboard.isWinsLeaderboard ? "<color=#808080ff>Wins :</color> " : "<color=#808080ff>Puzzles :</color> ") + leaderboard.value;

            GamePieceView gamePieceView = Instantiate(GameContentManager.Instance.piecesDataHolder.gamePieces.list.Random().player1Prefab, iconParent);
            gamePieceView.transform.localPosition = Vector3.zero;
            gamePieceView.StartBlinking();
        }

        public void OpenNewLeaderboardChallengeGame()
        {
            Debug.Log($"Open user challenge {leaderboard.userName}, {leaderboard.userId}");

            menuScreen.menuController.GetScreen<MatchmakingScreen>().StartVSPlayer(leaderboard.userId);
        }
    }
}