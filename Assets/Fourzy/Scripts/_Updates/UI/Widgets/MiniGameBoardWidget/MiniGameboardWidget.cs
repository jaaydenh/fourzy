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

namespace Fourzy._Updates.UI.Widgets
{
    public class MiniGameboardWidget : WidgetBase
    {
        [List]
        public AreaGraphicsDataCollection sprites;
        public Image gameboardGraphics;
        public TMP_Text nameLabel;
        public GameObject questionMark;

        public ClientFourzyGame game;

        private GameboardSelectionScreen _screen;

        public GameboardView gameboardView { get; private set; }
        public Toggle toggle { get; private set; }

        private GameBoardDefinition data;

        protected override void Awake()
        {
            base.Awake();

            gameboardView = GetComponentInChildren<GameboardView>();
            toggle = GetComponent<Toggle>();

            _screen = menuScreen as GameboardSelectionScreen;
        }

        public void SetData(GameBoardDefinition data)
        {
            this.data = data;

            nameLabel.text = data.BoardName;

            game = new ClientFourzyGame(data, UserManager.Instance.meAsPlayer, new Player(2, "Player 2"));
            game._Type = GameType.PASSANDPLAY;
            gameboardGraphics.sprite = sprites.GetGraphics(game._Area);

            gameboardView.Initialize(game);

            questionMark.SetActive(false);
        }

        public void BoardSelect()
        {
            _screen.SetGame(data);
        }

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
