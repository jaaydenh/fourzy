//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.UI.Menu.Screens;
using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StackableDecorator;
using TMPro;
using Fourzy._Updates.Tween;

namespace Fourzy._Updates.UI.Widgets
{
    public class MiniGameboardWidget : WidgetBase
    {
        [List]
        public AreaGraphicsDataCollection sprites;
        public Image gameboardGraphics;
        public AlphaTween selectionGraphics;
        public TMP_Text nameLabel;
        public GameObject questionMark;
        public AlphaTween boardAlphaTween;
        public AlphaTween spinner;
        public RotationTween spinnerRotationTween;

        public ClientFourzyGame game;

        private GameboardSelectionScreen _screen;

        public GameboardView gameboardView { get; private set; }

        [HideInInspector, NonSerialized]
        public GameBoardDefinition data;

        protected override void Awake()
        {
            base.Awake();

            gameboardView = GetComponentInChildren<GameboardView>(); 

            _screen = menuScreen as GameboardSelectionScreen;
        }

        public void ShowSpinner()
        {
            spinner.SetAlpha(1f);
            spinnerRotationTween.PlayForward(true);
        }

        public void HideSpinner()
        {
            spinner.PlayBackward(true);
            spinnerRotationTween.StopTween(false);
        }

        public void HideBoard()
        {
            boardAlphaTween.SetAlpha(0f);
        }

        public void Select() => selectionGraphics.PlayForward(true);

        public void Deselect() => selectionGraphics.PlayBackward(true);

        public void FinishedLoading()
        {
            boardAlphaTween.PlayForward(true);

            HideSpinner();
        }

        public GameboardView SetData(GameBoardDefinition data)
        {
            this.data = data;

            nameLabel.text = data.BoardName;

            game = new ClientFourzyGame(data, UserManager.Instance.meAsPlayer, new Player(2, "Player 2"));
            game._Type = GameType.PASSANDPLAY;
            gameboardGraphics.sprite = sprites.GetGraphics(game._Area);

            gameboardView.Initialize(game, false, false);

            questionMark.SetActive(false);

            return gameboardView;
        }

        public void BoardSelect() => _screen.SetGame(this);

        [Serializable]
        public class AreaGraphicsDataCollection
        {
            public List<AreaGraphicsData> list;

            public Sprite GetGraphics(Area area) => list.Find(item => item.area == area).sprite;
        }

        [Serializable]
        public class AreaGraphicsData
        {
            [HideInInspector]
            public string _name;

            [ShowIf("#Check")]
            [StackableField]
            public Area area;
            public Sprite sprite;

            public bool Check()
            {
                _name = area.ToString();

                return true;
            }
        }
    }
}
