//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Board;
using FourzyGameModel.Model;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class SpellsListUIWidget : WidgetBase
    {
        public SpellUIWidget spellWidgetPrefab;
        public RectTransform spellsContainer;

        public IClientFourzy game { get; private set; }
        public GameboardView board { get; private set; }
        public List<SpellUIWidget> spellWidgets { get; private set; }
        public ToggleGroup toggleGroup { get; private set; }

        public SpellUIWidget activeSpell => spellWidgets.Find(spell => spell.state == SpellUIWidget.SpellState.ACTIVE);

        public void Open(IClientFourzy game, GameboardView board)
        {
            //clear spells widgets
            foreach (SpellUIWidget widget in spellWidgets) Destroy(widget.gameObject);
            spellWidgets.Clear();

            this.game = game;
            this.board = board;

            switch (game._Type)
            {
                case GameType.PASSANDPLAY:
                    //hardoceded for now
                    AddSpell(SpellId.HEX);
                    break;

                case GameType.TURN_BASED:
                    //hardoceded for now
                    AddSpell(SpellId.HEX);
                    break;
            }

            board.onCast += OnCast;
            board.onCastCanceled += CancelCurrentSpell;
            
            board.onMoveStarted += UpdateSpells;
        }

        public void OnCast(SpellId spellId, int playerId) => activeSpell?.OnCast(playerId);

        public void CancelCurrentSpell() => activeSpell?.CancelCast();

        public void AddSpell(SpellId spellID)
        {
            SpellUIWidget spellWidget = Instantiate(spellWidgetPrefab, spellsContainer);
            spellWidget.transform.localScale = Vector3.one;

            spellWidget.SetData(spellID, this);

            spellWidgets.Add(spellWidget);
        }
        
        public void UpdateSpells(int value) => spellWidgets.ForEach(widget => widget.UpdateWidget(value));

        protected override void OnInitialized()
        {
            base.OnInitialized();

            spellWidgets = new List<SpellUIWidget>();
            toggleGroup = GetComponent<ToggleGroup>();
        }
    }
}