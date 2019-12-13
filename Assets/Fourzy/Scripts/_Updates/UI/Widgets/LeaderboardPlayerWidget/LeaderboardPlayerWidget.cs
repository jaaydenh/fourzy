//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using PlayFab;
using PlayFab.ClientModels;
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

        public LeaderboardPlayerWidget SetData(RankingScreen.LeaderboardEntry leaderboard)
        {
            this.leaderboard = leaderboard;

            playerNameLabel.text = leaderboard.userName;
            rankLabel.text = leaderboard.rank.ToString();
            typeLabel.text = (leaderboard.isWinsLeaderboard ? "<color=#808080ff>Wins :</color> " : "<color=#808080ff>Puzzles :</color> ") + leaderboard.value;

            AddGamepieceView();

            return this;
        }

        public LeaderboardPlayerWidget SetData(PlayerLeaderboardEntry entry)
        {
            playerNameLabel.text = entry.DisplayName;
            rankLabel.text = (entry.Position + 1) + "";
            typeLabel.text = entry.StatValue + "";

            if (entry.PlayFabId == LoginManager.playerMasterAccountID)
            {
                rankLabel.color = Color.green;
                AddGamepieceView(UserManager.Instance.gamePieceID);
            }
            else
                AddGamepieceView();

            return this;
        }

        public void OpenNewLeaderboardChallengeGame()
        {
            Debug.Log($"Open user challenge {leaderboard.userName}, {leaderboard.userId}");

            menuScreen.menuController.GetOrAddScreen<MatchmakingScreen>().StartVSPlayer(leaderboard.userId);
        }

        private GamePieceView AddGamepieceView(string pieceID = "")
        {
            GamePieceView gamePieceView = null;
            if (string.IsNullOrEmpty(pieceID))
                gamePieceView = Instantiate(GameContentManager.Instance.piecesDataHolder.gamePieces.list.Random().player1Prefab, iconParent);
            else
                gamePieceView = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(pieceID).player1Prefab, iconParent);
            gamePieceView.transform.localPosition = Vector3.zero;
            gamePieceView.StartBlinking();

            return gamePieceView;
        }
    }
}