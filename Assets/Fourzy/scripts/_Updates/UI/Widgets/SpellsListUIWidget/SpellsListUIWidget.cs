//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.Board;
using FourzyGameModel.Model;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
        public Player owner;

        public SpellUIWidget activeSpell => spellWidgets.Find(spell => spell.state == SpellUIWidget.SpellState.ACTIVE);

        //protected void Update()
        //{
        //    if (game == null) return;

        //    switch (game._Type)
        //    {
        //        case GameType.PASSANDPLAY:
        //            switch (StandaloneInputModuleExtended.GamepadFilter)
        //            {
        //                case StandaloneInputModuleExtended.GamepadControlFilter.ANY_GAMEPAD:

        //                    if (Input.GetKeyDown(StandaloneInputModuleExtended.GetKeyCode(3, 0)) || Input.GetKeyDown(StandaloneInputModuleExtended.GetKeyCode(3, 1)))
        //                    {
        //                        spellWidgets[0].OnTap();
        //                    }

        //                    break;

        //                case StandaloneInputModuleExtended.GamepadControlFilter.SPECIFIC_GAMEPAD:
        //                    switch (StandaloneInputModuleExtended.GamepadID)
        //                    {
        //                        case 0:
        //                            if (Input.GetKeyDown(StandaloneInputModuleExtended.GetKeyCode(3, 0)))
        //                            {
        //                                spellWidgets[0].OnTap();
        //                            }

        //                            break;

        //                        case 1:
        //                            if (Input.GetKeyDown(StandaloneInputModuleExtended.GetKeyCode(3, 1)))
        //                            {
        //                                spellWidgets[0].OnTap();
        //                            }

        //                            break;
        //                    }

        //                    break;
        //            }

        //            break;
        //    }
        //}

        public void Open(IClientFourzy game, GameboardView board, Player owner)
        {
            //ignore if spells disabled
            if (!SettingsManager.Get(SettingsManager.KEY_MAGIC)) return;

            //clear spells widgets
            foreach (SpellUIWidget widget in spellWidgets) Destroy(widget.gameObject);
            spellWidgets.Clear();

            this.game = game;
            this.board = board;
            this.owner = owner;

            if (game.puzzleData != null)
            {
                System.Array.ForEach(game.puzzleData.availableSpells, (spell) => AddSpell(spell));
            }
            else
            {
                switch (game._Type)
                {
                    case GameType.PUZZLE:
                    case GameType.ONBOARDING:
                    case GameType.PRESENTATION:
                    case GameType.FRIEND:
                    case GameType.LEADERBOARD:
                    case GameType.REALTIME:

                        break;

                    case GameType.AI:
                    case GameType.PASSANDPLAY:
                        //List<SpellId> spells = GameContentManager.Instance.piecesDataHolder.GetGamePieceData(owner.HerdId).spells;

                        //if (spells.Count > 0)
                        //    foreach (SpellId spell in spells)
                        //        AddSpell(spell);
                        //else
                            AddSpell(SpellId.HEX);
                            AddSpell(SpellId.PLACE_LURE);
                            //AddSpell(SpellId.DARKNESS);

                        break;

                    case GameType.TURN_BASED:
                        //hardoceded for now
                        AddSpell(SpellId.HEX);

                        break;
                }
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

        public void UpdateSpells(ClientPlayerTurn turn, bool startTurn)
        {
            if (startTurn || turn == null || turn.PlayerId < 1) return;

            spellWidgets.ForEach(widget => widget.UpdateWidget(turn.PlayerId));
        }

        public void UpdateSpells(int playerID) => spellWidgets.ForEach(widget => widget.UpdateWidget(playerID));

        protected override void OnInitialized()
        {
            base.OnInitialized();

            spellWidgets = new List<SpellUIWidget>();
            toggleGroup = GetComponent<ToggleGroup>();
        }
    }
}