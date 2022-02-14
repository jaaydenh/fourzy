//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using FourzyGameModel.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class PlayerUIWidget : WidgetBase
    {
        [SerializeField]
        private TextMeshProUGUI playerName;
        [SerializeField]
        private TextMeshProUGUI extraLabel;
        [SerializeField]
        private RectTransform pieceParent;
        [SerializeField]
        private Badge magicBadge;
        [SerializeField]
        private int playerNameMaxSize = 9;
        [SerializeField]
        private int noMagicMaxPlayerName = 20;
        [SerializeField]
        private bool updateInifinityLayout;

        public SpellsListUIWidget spellsHolder;

        private VerticalLayoutGroup verticalLayoutGroup;
        private GamePieceView current;
        private int rating;
        private int totalGames;
        private bool magicWidget = true;

        public Player AssignedPlayer { get; private set; }
        public IClientFourzy Game { get; private set; }
        public bool Shown { get; private set; }
        public bool IsMe { get; private set; }
        public int Magic { get; private set; } = 1;
        public string PlayerName { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();

            OpponentData.onRatingUdpate += OnOpponentRatingUpdate;
            OpponentData.onTotalGamesUpdate += OnOpponentTotalGamesUpdate;
            UserManager.onTotalGamesUpdate += OnTotalGamesUpdate;
            UserManager.onRatingUpdate += OnRatingUpdate;
        }

        protected void OnDestroy()
        {
            if (Game != null)
            {
                Game.onMagic -= OnMagicUpdate;
            }

            OpponentData.onRatingUdpate -= OnOpponentRatingUpdate;
            OpponentData.onTotalGamesUpdate -= OnOpponentTotalGamesUpdate;
            UserManager.onTotalGamesUpdate -= OnTotalGamesUpdate;
            UserManager.onRatingUpdate -= OnRatingUpdate;
        }

        public void SetPlayerName(string name)
        {
            int maxPName = magicWidget ? playerNameMaxSize : noMagicMaxPlayerName;

            if (string.IsNullOrEmpty(name))
            {
                PlayerName = "";
                playerName.text = "Empty Name";
            }
            else
            {
                PlayerName = name;
                if (name.Length > maxPName)
                {
                    playerName.text = name.Substring(0, maxPName - 1) + "...";
                }
                else
                {
                    playerName.text = name;
                }
            }
        }

        public void SetPlayer(Player player)
        {
            AssignedPlayer = player;
            if (Game != null)
            {
                IsMe = AssignedPlayer == Game.me;
            }

            if (current)
            {
                Destroy(current.gameObject);
            }

            current = Instantiate(player.PlayerId == 1 ? Game.playerOneGamepiece : Game.playerTwoGamepiece, pieceParent);
            current.transform.localPosition = Vector3.zero;
            current.transform.localScale = Vector3.one * 94f;

            current.gameObject.SetActive(true);

            SetMagic(player.Magic);
            SetPlayerName(player.DisplayName);
        }

        public void OnPlayerPosition(PlayerPositioning playerPosition)
        {
            switch (Game._Type)
            {
                case GameType.ONBOARDING:
                    //reset for onboarding mode
                    playerPosition = PlayerPositioning.SIDE_BY_SIDE;

                    break;
            }

            switch (playerPosition)
            {
                case PlayerPositioning.ACROSS:
                    pieceParent.anchorMin = new Vector2(0f, .5f);
                    pieceParent.anchorMax = new Vector2(0f, .5f);
                    verticalLayoutGroup.padding = new RectOffset(60, 20, 0, 0);

                    break;

                case PlayerPositioning.SIDE_BY_SIDE:
                    pieceParent.anchorMin = new Vector2(1f, .5f);
                    pieceParent.anchorMax = new Vector2(1f, .5f);
                    verticalLayoutGroup.padding = new RectOffset(20, 60, 0, 0);

                    break;
            }

            pieceParent.anchoredPosition = Vector2.zero;
        }

        public void SetGame(IClientFourzy game)
        {
            this.Game = game;

            game.onMagic += OnMagicUpdate;

            if (updateInifinityLayout)
            {
                OnPlayerPosition(PlayerPositioningPromptScreen.PlayerPositioning);
            }
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

            // update player name (label size)
            SetPlayerName(PlayerName);
        }

        public void SetRating(int value, int totalGames)
        {
            if (value < 0) return;

            if (totalGames >= InternalSettings.Current.GAMES_BEFORE_RATING_USED)
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

            Shown = true;
        }

        public override void Hide(float time)
        {
            if (alphaTween._value > 0f)
            {
                base.Hide(time);
            }

            Shown = false;
        }

        internal void SetMagic(int value)
        {
            if (!magicWidget) return;

            magicBadge.SetValue(value);
            Magic = value;
        }

        private void OnMagicUpdate(int playerId, int value)
        {
            if (AssignedPlayer.PlayerId == playerId)
            {
                SetMagic(value);
            }
        }

        private void OnOpponentRatingUpdate(int rating)
        {
            if (IsMe)
            {
                return;
            }

            SetRating(this.rating = rating, totalGames);
        }

        private void OnOpponentTotalGamesUpdate(int totalGames)
        {
            if (IsMe)
            {
                return;
            }

            SetRating(rating, this.totalGames = totalGames);
        }

        #region Local player

        private void OnRatingUpdate(int rating)
        {
            //only me
            if (!IsMe)
            {
                return;
            }

            this.rating = rating;

            SetRating(rating, totalGames);
        }

        private void OnTotalGamesUpdate(int totalGames)
        {
            //only me
            if (!IsMe)
            {
                return;
            }

            this.totalGames = totalGames;

            SetRating(rating, totalGames);
        }

        #endregion
    }
}