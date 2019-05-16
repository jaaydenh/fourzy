﻿//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Helpers;
using FourzyGameModel.Model;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class PlayerUIWidget : WidgetBase
    {
        public SliderExtended bgSlider;
        public TextMeshProUGUI playerName;
        public RectTransform pieceParent;
        public Badge magicBadge;
        public int playerNameMaxSize = 9;

        public IClientFourzy game { get; private set; }

        private GamePieceView current;
        private Player assignedPlayer;

        protected void OnDestroy()
        {
            game.onMagic -= UpdateMagic;
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
                    playerName.text = name.Substring(0, playerNameMaxSize - 1) + "...";
                else
                    playerName.text = name;
            }
        }

        public void SetPlayerIcon(Player player)
        {
            assignedPlayer = player;

            if (current) Destroy(current.gameObject);

            current = Instantiate(game.me == player ? game.playerGamePiece : game.opponentGamePiece, pieceParent);
            current.transform.localPosition = Vector3.zero;
            current.transform.localScale = Vector3.one * 94f;

            current.gameObject.SetActive(true);

            UpdateMagic(player.PlayerId, player.Magic);
        }

        public void ShowPlayerTurnAnimation()
        {
            if (!current)
                return;

            current.ShowTurnAnimation();
        }

        public void StopPlayerTurnAnimation()
        {
            if (!current)
                return;

            current.StopTurnAnimation();
        }

        public void StartWinJumps()
        {
            if (!current)
                return;

            current.ShowUIWinAnimation();
        }

        public void SetGame(IClientFourzy game)
        {
            this.game = game;

            game.onMagic += UpdateMagic;
        }

        private void UpdateSlider(float value)
        {
            if (game != null && game.activePlayer.PlayerId == (int)current.player)
                bgSlider.value = value;
        }

        private void UpdateMagic(int playerId, int value)
        {
            if (assignedPlayer.PlayerId == playerId)
                magicBadge.SetValue(value);
        }
    }
}