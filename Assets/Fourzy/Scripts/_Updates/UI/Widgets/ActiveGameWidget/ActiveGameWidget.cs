//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using FourzyGameModel.Model;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class ActiveGameWidget : WidgetBase
    {
        public TMP_Text opponentNameLabel;
        public TMP_Text userTitle;
        public TMP_Text moveTimeAgo;
        public Transform gamepieceParent;

        public ButtonExtended rematchButton;

        public ChallengeData data { get; private set; }
        public ClientFourzyGame game { get; private set; }
        public GamesScreen gamesScreen { get; private set; }

        private Player opponent;

        public void SetData(GamesScreen _parent, ChallengeData data, ClientFourzyGame game)
        {
            gamesScreen = _parent;

            this.game = game;
            this.data = data;

            opponent = game.opponent;
            opponentNameLabel.text = (opponent.DisplayName.Length > 14) ? opponent.DisplayName.Substring(0, 15) + "..." : opponent.DisplayName;

            string opponentHerdId = "0";
            if (opponent.HerdId != null) opponentHerdId = opponent.HerdId;

            GamePieceView gamePieceView = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(opponentHerdId).player1Prefab, gamepieceParent);
            gamePieceView.transform.localPosition = Vector3.zero;
            gamePieceView.StartBlinking();

            if (game.isOver)
            {
                rematchButton.SetActive(true);
                rematchButton.SetLabel("Rematch");

                moveTimeAgo.gameObject.SetActive(false);
            }
            else
            {
                rematchButton.SetActive(false);

                PlayerTurn lastMove = data.lastTurn;

                if (lastMove != null)
                {
                    System.DateTime timestampDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).AddMilliseconds(lastMove.Timestamp).ToLocalTime();
                    moveTimeAgo.text = TimeAgo.DateTimeExtensions.TimeAgo(timestampDateTime, LocalizationManager.Instance.cultureInfo);
                }
                else
                    moveTimeAgo.text = "No moves ...";
            }

            //PlayerTurn lastMove = data.lastTurn;

            //if (lastMove != null)
            //{
            //    System.DateTime timestampDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).AddMilliseconds(lastMove.Timestamp).ToLocalTime();
            //    moveTimeAgo.text = TimeAgo.DateTimeExtensions.TimeAgo(timestampDateTime, LocalizationManager.Instance.cultureInfo);
            //}
            //else
            //    moveTimeAgo.text = "No moves were made...";
        }

        public void OpenGame()
        {
            //GameManager.Instance.StartGame(data.GetGameForPreviousMove());
        }

        public void Rematch()
        {
            if (opponent == null || string.IsNullOrEmpty(opponent.PlayerString)) return;

            Debug.Log($"Open user challenge {opponent.DisplayName}, {opponent.PlayerString}");
            menuScreen.menuController.GetOrAddScreen<MatchmakingScreen>().StartVSPlayer(opponent.PlayerString);
        }
    }
}
