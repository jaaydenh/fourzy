//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
using StackableDecorator;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class MiniGameboardWidget : WidgetBase
    {
        private Action<MiniGameboardWidget> onClick;

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

        public GameboardView gameboardView { get; private set; }

        [HideInInspector, NonSerialized]
        public GameBoardDefinition data;

        protected override void Awake()
        {
            base.Awake();

            gameboardView = GetComponentInChildren<GameboardView>();
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

        public void Select(float time)
        {
            selectionGraphics.playbackTime = time;
            selectionGraphics.PlayForward(true);
        }

        public void Deselect(float time)
        {
            selectionGraphics.playbackTime = time;
            selectionGraphics.PlayBackward(true);
        }

        public void FinishedLoading()
        {
            boardAlphaTween.PlayForward(true);

            HideSpinner();
        }

        public GameboardView SetData(GameBoardDefinition data, bool updateArea = true)
        {
            this.data = data;

            if (data != null)
            {
                if (nameLabel) nameLabel.text = data.BoardName;

                game = new ClientFourzyGame(data, UserManager.Instance.meAsPlayer, new Player(2, "Player 2"));
                game._Type = GameType.PASSANDPLAY;

                if (updateArea) SetArea(game._Area);

                gameboardView.Initialize(game, false);
            }
            else
            {
                Clear(updateArea);
                nameLabel.text = LocalizationManager.Value("random");
            }

            questionMark.SetActive(data == null);

            return gameboardView;
        }

        public MiniGameboardWidget Clear(bool updateArea = true)
        {
            gameboardView.Clear();
            if (updateArea) SetArea(Area.NONE);

            return this;
        }

        public MiniGameboardWidget SetArea(Area area)
        {
            gameboardGraphics.sprite = sprites.GetGraphics(area);

            return this;
        }

        public MiniGameboardWidget QuickLoadBoard(GameBoardDefinition data, bool updateArea = true)
        {
            SetData(data, updateArea);

            return this;
        }

        public MiniGameboardWidget SetOnClick(Action<MiniGameboardWidget> action)
        {
            onClick = action;

            return this;
        }

        public void OnClick() => onClick?.Invoke(this);

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
