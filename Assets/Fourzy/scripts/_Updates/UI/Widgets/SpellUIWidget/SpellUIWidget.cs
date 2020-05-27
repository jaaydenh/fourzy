//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
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

        public int timesUsed { get; private set; }
        public SpellState state { get; private set; }
        public SpellId spellId { get; private set; }

        public void SetData(SpellId spellId, SpellsListUIWidget spellsList)
        {
            this.spellsList = spellsList;
            this.spellId = spellId;

            data = GameContentManager.Instance.tokensDataHolder.GetTokenData(spellId);
            icon.sprite = data.GetTokenSprite();

            UpdateWidget(spellsList.game._State.ActivePlayerId);
        }

        public void OnTap()
        {
            if (state == SpellState.UNAVAILABLE) return;

            if (spellsList.activeSpell != null && spellsList.activeSpell != this) spellsList.activeSpell.CancelCast();

            switch (state)
            {
                case SpellState.NONE:
                    state = SpellState.ACTIVE;
                    spellsList.board.PrepareForSpell(spellId);

                    break;

                case SpellState.ACTIVE:
                    state = SpellState.NONE;
                    spellsList.board.CancelSpell();

                    break;
            }

            UpdateWidget(spellsList.game._State.ActivePlayerId);
            UpdateBG();
        }

        public void CancelCast()
        {
            state = SpellState.NONE;

            UpdateWidget(spellsList.game._State.ActivePlayerId);
            UpdateBG();
        }

        public void OnCast(int playerId)
        {
            timesUsed++;
            state = SpellState.NONE;

            spellsList.game.AddPlayerMagic(playerId, -data.price);

            UpdateWidget(playerId);
            UpdateBG();
        }

        public void SetButtonState(bool _state)
        {
            state = _state ? (state == SpellState.ACTIVE ? SpellState.ACTIVE : SpellState.NONE) : SpellState.UNAVAILABLE;

            button.playOnClick = _state ? AudioTypes.BUTTON_CLICK : unavailableSfx;
            button.SetState(_state);
            button.interactable = true;
        }

        public void TrySetButtonState(int playerId, bool _state)
        {
            //switch (spellsList.board.game._Type)
            //{
            //    case GameType.TURN_BASED:
            //    case GameType.PASSANDPLAY:
            //        SetButtonState(spellsList.game.magic[playerId] >= data.price && _state);

            //        break;
            //}
            SetButtonState(spellsList.game.magic[playerId] >= data.price && _state);
        }

        public void UpdateWidget(int playerId)
        {
            //update price badge
            button.GetBadge("price").badge.SetValue(data.price);

            bool? setTo = !spellsList.board.isAnimating && spellsList.game._State.ActivePlayerId == spellsList.owner.PlayerId;
            //switch (spellsList.game._Type)
            //{
            //    case GameType.PASSANDPLAY:
            //        setTo = !spellsList.board.isAnimating;

            //        break;

            //    case GameType.AI:
            //    case GameType.TURN_BASED:
            //        setTo = !spellsList.board.isAnimating && spellsList.game._State.ActivePlayerId == spellsList.owner.PlayerId;

            //        break;
            //}

            if (setTo != null)
            {
                if (setTo.Value)
                {
                    if (spellsList.owner.PlayerId != playerId)
                    {
                        if (state == SpellState.ACTIVE || state == SpellState.NONE) TrySetButtonState(playerId, false);
                        return;
                    }
                }

                TrySetButtonState(playerId, setTo.Value);
            }
        }

        private void UpdateBG()
        {
            switch (state)
            {
                case SpellState.NONE:
                case SpellState.UNAVAILABLE:
                    selectedBG.PlayBackward(true);
                    break;

                case SpellState.ACTIVE:
                    selectedBG.PlayForward(true);
                    break;
            }
        }

        public enum SpellState
        {
            NONE,
            ACTIVE,
            UNAVAILABLE,
        }
    }
}