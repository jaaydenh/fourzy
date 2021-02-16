//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Mechanics.GameplayScene;
using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [NonSerialized]
        public Player owner;

        public SpellUIWidget activeSpell =>
            spellWidgets.Find(spell => spell.state == SpellState.ACTIVE);

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


        public void SetData(IClientFourzy game, GameboardView board, Player owner)
        {
            this.game = game;
            this.board = board;
            this.owner = owner;

            ClearSpells();

            if (game.puzzleData != null)
            {
                switch (game._Mode)
                {
                    case GameMode.GAUNTLET:

                        break;

                    default:
                        Array.ForEach(game.puzzleData.availableSpells, (spell) => AddSpell(spell));

                        break;
                }
            }
            else
            {
                switch (game._Type)
                {
                    case GameType.REALTIME:

                    //break;

                    default:
                        foreach (SpellId spell in GameContentManager.Instance.piecesDataHolder
                            .GetGamePieceData(owner.HerdId).Spells)
                        {
                            AddSpell(spell);
                        }

                        break;
                }
            }

            board.onCast += OnCast;
            board.onCastCanceled += CancelCurrentSpell;

            board.onMoveStarted += OnMoveStarted;
            board.onMoveEnded += OnMoveEnded;
            board.onGameFinished += OnGameFinished;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            spellWidgets = new List<SpellUIWidget>();
            toggleGroup = GetComponent<ToggleGroup>();

            GamePlayManager.onGameStarted += OnGameStarted;
        }

        protected void OnDestroy()
        {
            GamePlayManager.onGameStarted -= OnGameStarted;
        }

        private void AddSpell(SpellId spellId)
        {
            spellWidgets.Add(
                Instantiate(spellWidgetPrefab, spellsContainer)
                .SetData(spellId, this, owner));
        }

        private void ClearSpells()
        {
            foreach (SpellUIWidget widget in spellWidgets)
            {
                Destroy(widget.gameObject);
            }
            spellWidgets.Clear();
        }

        private void OnCast(SpellId spellId, int playerId)
        {
            activeSpell?.OnCast(playerId);

            spellWidgets.ForEach(widget => widget.TryActivate());
        }

        private void CancelCurrentSpell()
        {
            activeSpell?.CancelCast();
        }

        private void OnMoveStarted(ClientPlayerTurn turn, bool startTurn)
        {
            if (startTurn || turn == null || turn.PlayerId < 1)
            {
                return;
            }

            spellWidgets.ForEach(widget => widget.SetState(SpellState.UNAVAILABLE));
        }

        private void OnMoveEnded(ClientPlayerTurn turn, PlayerTurnResult turnResult, bool startTurn)
        {
            if (startTurn || turn == null || turn.PlayerId < 1)
            {
                return;
            }

            spellWidgets.ForEach(widget => widget.TryActivate());
        }

        private void OnGameStarted(IClientFourzy game)
        {
            spellWidgets.ForEach(widget => widget.TryActivate());
        }

        private void OnGameFinished(IClientFourzy game)
        {
            spellWidgets.ForEach(widget => widget.SetState(SpellState.UNAVAILABLE));
        }
    }
}