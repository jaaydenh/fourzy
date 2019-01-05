//@vadym udod

using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class ActiveGameWidget : WidgetBase
    {
        public TMP_Text opponentNameLabel;
        public TMP_Text statusLabel;
        public TMP_Text moveTimeAgo;
        public Image opponentProfilePicture;
        public Image tournamentIcon;

        [HideInInspector]
        public Game game;

        private PlayerEnum currentplayer = PlayerEnum.NONE;

        public void SetData(Game data)
        {
            tournamentIcon.enabled = game.challengeType == ChallengeType.TOURNAMENT;

            if (game.opponent == null)
            {
                Debug.Log("game.opponent is null");
                opponentNameLabel.text = LocalizationManager.Instance.GetLocalizedValue("waiting_opponent_text");
            }
            else
                opponentNameLabel.text = game.opponent.opponentName;

            if (game.opponentProfilePictureSprite != null)
                opponentProfilePicture.sprite = game.opponentProfilePictureSprite;

            if (game.isCurrentPlayer_PlayerOne)
                currentplayer = PlayerEnum.ONE;
            else
                currentplayer = PlayerEnum.TWO;

            if (game.gameState.Winner != PlayerEnum.EMPTY)
            {
                if (game.gameState.Winner == currentplayer)
                    statusLabel.text = LocalizationManager.Instance.GetLocalizedValue("you_won_text");
                else if (game.gameState.Winner == PlayerEnum.NONE || game.gameState.Winner == PlayerEnum.ALL)
                    statusLabel.text = LocalizationManager.Instance.GetLocalizedValue("draw_text");
                else
                    statusLabel.text = LocalizationManager.Instance.GetLocalizedValue("they_won_text");
            }
            else if (game.isExpired == true)
                statusLabel.text = LocalizationManager.Instance.GetLocalizedValue("expired_text");
            else
            {
                if (game.gameState.isCurrentPlayerTurn)
                    statusLabel.text = LocalizationManager.Instance.GetLocalizedValue("your_move_text");
                else
                    statusLabel.text = LocalizationManager.Instance.GetLocalizedValue("their_move_text");
            }

            if (game.gameState.MoveList.LastOrDefault() == null)
                Debug.Log("game.gameState.moveList == null");

            Move lastPlayerMove = game.gameState.MoveList.LastOrDefault();

            long timestamp = 0;
            if (lastPlayerMove != null)
                timestamp = lastPlayerMove.timeStamp;

            if (timestamp != 0)
            {
                System.DateTime timestampDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).AddMilliseconds(timestamp).ToLocalTime();
                moveTimeAgo.text = TimeAgo.DateTimeExtensions.TimeAgo(timestampDateTime, LocalizationManager.Instance.cultureInfo);
            }
        }

        public void OpenGame()
        {
            GameManager.Instance.OpenGame(game);
        }
    }
}
