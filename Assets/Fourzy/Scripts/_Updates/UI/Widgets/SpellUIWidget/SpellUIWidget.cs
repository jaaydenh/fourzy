//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class SpellUIWidget : WidgetBase
    {
        public Image icon;
        public AlphaTween selectedBG;
        public AudioTypes unavailableSfx = AudioTypes.NEGATIVE;

        private TokensDataHolder.TokenData data;
        private SpellsListUIWidget spellsList;
        private IClientFourzy game;
        private Player owner;
        public SpellState prevState;

        public int timesUsed { get; private set; }
        public SpellState state { get; private set; }
        public SpellId spellId { get; private set; }

        public SpellUIWidget SetData(SpellId spellId, SpellsListUIWidget spellsList, Player owner)
        {
            this.spellsList = spellsList;
            this.spellId = spellId;
            this.owner = owner;

            game = spellsList.game;
            data = GameContentManager.Instance.tokensDataHolder.GetTokenData(spellId);
            icon.sprite = data.GetTokenSprite();

            button.GetBadge("price").badge.SetValue(data.price);

            SetState(SpellState.UNAVAILABLE);

            return this;
        }

        /// <summary>
        /// Called via button
        /// </summary>
        public void OnTap()
        {
            if (state == SpellState.UNAVAILABLE) return;
            if (owner.PlayerId != game._State.ActivePlayerId) return;

            //cancel prev spell is theres one
            if (spellsList.activeSpell != null && spellsList.activeSpell != this)
            {
                spellsList.activeSpell.CancelCast();
            }

            switch (state)
            {
                case SpellState.NONE:
                    SetState(SpellState.ACTIVE);
                    spellsList.board.PrepareForSpell(spellId);

                    break;

                case SpellState.ACTIVE:
                    SetState(SpellState.NONE);
                    spellsList.board.CancelSpell();

                    break;
            }
        }

        internal void CancelCast()
        {
            SetState(SpellState.NONE);
        }

        internal void OnCast(int playerId)
        {
            timesUsed++;
        }

        internal void SetState(SpellState toSet)
        {
            switch (game._Type)
            {
                case GameType.REALTIME:
                    if (game.opponent.PlayerId == owner.PlayerId)
                    {
                        toSet = SpellState.NONE;
                    }

                    break;
            }

            state = toSet;

            bool buttonState = state == SpellState.NONE || state == SpellState.ACTIVE;
            button.playOnClick = buttonState ? AudioTypes.BUTTON_CLICK : unavailableSfx;
            button.SetState(buttonState);
            button.interactable = buttonState;

            switch (state)
            {
                case SpellState.NONE:
                case SpellState.UNAVAILABLE:
                    if (prevState == SpellState.ACTIVE)
                    {
                        selectedBG.PlayBackward(true);
                    }

                    break;

                case SpellState.ACTIVE:
                    if (prevState == SpellState.NONE || prevState == SpellState.UNAVAILABLE)
                    {
                        selectedBG.PlayForward(true);
                    }

                    break;
            }

            prevState = state;
        }

        internal void TryActivate()
        {
            SpellState toSet = SpellState.UNAVAILABLE;

            //not enough magic
            if (game.magic[owner.PlayerId] >= data.price && game._State.ActivePlayerId == owner.PlayerId)
            {
                toSet = SpellState.NONE;
            }

            SetState(toSet);
        }

        internal bool MagicCheck()
        {
            return game.magic[owner.PlayerId] >= data.price;
        }
    }

    public enum SpellState
    {
        NONE,
        ACTIVE,
        UNAVAILABLE,
    }
}