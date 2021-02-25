//@vadym udod

using ExitGames.Client.Photon;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Helpers;
using FourzyGameModel.Model;
using Photon.Pun;
using System;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class PlayerUIWidget : WidgetBase
    {
        public TextMeshProUGUI playerName;
        public TextMeshProUGUI extraLabel;
        public RectTransform pieceParent;
        public Badge magicBadge;
        public int playerNameMaxSize = 9;
        public SpellsListUIWidget spellsHolder;

        private GamePieceView current;
        private int rating;
        private int totalGames;
        private bool magicWidget = true;

        public Player assignedPlayer { get; private set; }
        public IClientFourzy game { get; private set; }

        public bool shown { get; private set; }
        public bool isMe { get; private set; }
        public int magic { get; private set; } = 1;

        protected override void Awake()
        {
            base.Awake();

            OpponentData.onRatingUdpate += OnOpponentRatingUpdate;
            OpponentData.onTotalGamesUpdate += OnOpponentTotalGamesUpdate;
            UserManager.onTotalGamesUpdate += OnTotalGamesUpdate;
            UserManager.onRatingUpdate += OnRatingUpdate;
        }

        protected void OnDestroy()
        {
            if (game != null)
            {
                game.onMagic -= OnMagicUpdate;
            }

            OpponentData.onRatingUdpate -= OnOpponentRatingUpdate;
            OpponentData.onTotalGamesUpdate -= OnOpponentTotalGamesUpdate;
            UserManager.onTotalGamesUpdate -= OnTotalGamesUpdate;
            UserManager.onRatingUpdate -= OnRatingUpdate;
        }

        public void SetPlayerName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                playerName.text = "Empty Name";
            }
            else
            {
                if (name.Length > playerNameMaxSize)
                {
                    playerName.text = name.Substring(0, playerNameMaxSize - 1) + "...";
                }
                else
                {
                    playerName.text = name;
                }
            }
        }

        public void SetPlayer(Player player)
        {
            assignedPlayer = player;
            if (game != null)
            {
                isMe = assignedPlayer == game.me;
            }

            if (current)
            {
                Destroy(current.gameObject);
            }

            current = Instantiate(player.PlayerId == 1 ?
                game.playerOneGamepiece :
                game.playerTwoGamepiece, pieceParent);
            current.transform.localPosition = Vector3.zero;
            current.transform.localScale = Vector3.one * 94f;

            current.gameObject.SetActive(true);

            SetMagic(player.Magic);

            SetPlayerName(player.DisplayName);
        }

        public void SetGame(IClientFourzy game)
        {
            this.game = game;

            game.onMagic += OnMagicUpdate;
        }

        public void ShowPlayerTurnAnimation()
        {
            if (!current) return;

            current.ShowTurnAnimation();
        }

        public void StopPlayerTurnAnimation()
        {
            if (!current) return;

            current.StopTurnAnimation();
        }

        public void StartWinJumps()
        {
            if (!current) return;

            current.ShowUIWinAnimation();
        }

        public void SetExtraData(string text)
        {
            extraLabel.text = text;
        }

        public void SetMagicWidget(bool value)
        {
            magicWidget = value;
            magicBadge.SetState(value);
        }

        public void SetRating(int value, int totalGames)
        {
            if (value < 0) return;

            if (totalGames >= Constants.GAMES_BEFORE_RATING_DISPLAYED)
            {
                SetExtraData(value.ToString());
            }
            else
            {
                SetExtraData("Apprentice");
            }
        }

        public override void Show(float time)
        {
            if (alphaTween._value < 1f)
            {
                base.Show(time);
            }

            shown = true;
        }

        public override void Hide(float time)
        {
            if (alphaTween._value > 0f)
            {
                base.Hide(time);
            }

            shown = false;
        }

        internal void SetMagic(int value)
        {
            if (!magicWidget) return;

            magicBadge.SetValue(value);
            magic = value;
        }

        private void OnMagicUpdate(int playerId, int value)
        {
            if (assignedPlayer.PlayerId == playerId)
            {
                SetMagic(value);
            }
        }

        private void OnOpponentRatingUpdate(int rating)
        {
            if (isMe)
            {
                return;
            }

            SetRating(this.rating = rating, totalGames);
        }

        private void OnOpponentTotalGamesUpdate(int totalGames)
        {
            if (isMe)
            {
                return;
            }

            SetRating(rating, this.totalGames = totalGames);
        }

        #region Local player

        private void OnRatingUpdate(int rating)
        {
            //only me
            if (!isMe)
            {
                return;
            }

            this.rating = rating;

            SetRating(rating, totalGames);
        }

        private void OnTotalGamesUpdate(int totalGames)
        {
            //only me
            if (!isMe)
            {
                return;
            }

            this.totalGames = totalGames;

            SetRating(rating, totalGames);
        }

        #endregion
    }
}