//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class GamePieceWidgetLandscape : WidgetBase
    {
        public Action<PointerEventData, GamePieceWidgetLandscape> onClick;

        [HideInInspector]
        public GamePieceData data;

        public RectTransform gamePieceParent;
        public GameObject player1Marker;
        public Image player2Marker;
        public Image p1Selection;
        public Image p2Selection;
        public GameObject random;
        public Sprite p2Sprite;
        public Sprite cpuSprite;
        public Sprite p2SelectionSprite;
        public Sprite cpuSelectionSprite;

        private Image currentHighlight;
        private VSLandscapeScreen _menuScreen;
        private SelectableUI selectableUI;
        private int[] players;

        public GamePieceView gamePiece { get; private set; }

        public virtual GamePieceWidgetLandscape SetData(GamePieceData data)
        {
            if (data != null)
            {
                if (gamePiece && gamePiece.pieceData.Id != data.Id)
                {
                    Destroy(gamePiece.gameObject);
                    gamePiece = AddPiece(data.Id);
                }
                else if (!gamePiece)
                    gamePiece = AddPiece(data.Id);

                random.SetActive(false);
            }
            else
            {
                random.SetActive(true);
            }

            this.data = data;

            return this;
        }

        public GamePieceWidgetLandscape SelectAsPlayer(params int[] players)
        {
            bool p1 = Array.IndexOf(players, 0) != -1;// .Contains(0);
            bool p2 = Array.IndexOf(players, 1) != -1;
            // bool p2 = players.Contains(1);

            this.players = Array.IndexOf(players, -1) != -1 ? null : players;
            // this.players = players.Contains(-1) ? null : players;

            player1Marker.SetActive(p1);
            player2Marker.gameObject.SetActive(p2);

            p1Selection.gameObject.SetActive(p1);
            p2Selection.gameObject.SetActive(p2);

            p1Selection.fillAmount = p1 && p2 ? .5f : 1f;
            p2Selection.fillAmount = p1 && p2 ? .5f : 1f;

            return this;
        }

        public GamePieceWidgetLandscape SetP2AsCPU(bool state)
        {
            player2Marker.sprite = state ? cpuSprite : p2Sprite;
            player2Marker.SetNativeSize();

            p2Selection.sprite = state ? cpuSelectionSprite : p2SelectionSprite;

            return this;
        }

        public GamePieceWidgetLandscape SetOnClick(Action<PointerEventData, GamePieceWidgetLandscape> action)
        {
            onClick = action;

            return this;
        }

        public void OnPieceSelected(GamePieceWidgetLandscape piece)
        {

        }

        private void OnPointerEnter(PointerEventData data)
        {
            if (players != null || (_menuScreen.selectedPlayers[0] && _menuScreen.selectedPlayers[1])) return;

            if (CustomInputManager.GamepadCount > 1 && data.pointerId > -1)
            {
                switch (data.pointerId)
                {
                    case 1:
                        currentHighlight = p2Selection;

                        p2Selection.sprite = _menuScreen.p2DifficultyLevel > -1 ? cpuSelectionSprite : p2SelectionSprite;
                        p2Selection.gameObject.SetActive(true);
                        p2Selection.fillAmount = 1f;

                        break;

                    default:
                        currentHighlight = p1Selection;

                        p1Selection.gameObject.SetActive(true);
                        p1Selection.fillAmount = 1f;

                        break;
                }
            }
            else
            {
                if (_menuScreen.selectedPlayers[0])
                {
                    currentHighlight = p2Selection;

                    p2Selection.sprite = _menuScreen.p2DifficultyLevel > -1 ? cpuSelectionSprite : p2SelectionSprite;
                    p2Selection.gameObject.SetActive(true);
                    p2Selection.fillAmount = 1f;
                }
                else
                {
                    currentHighlight = p1Selection;

                    p1Selection.gameObject.SetActive(true);
                    p1Selection.fillAmount = 1f;
                }
            }
        }

        private void OnPointerExit(PointerEventData data)
        {
            if (players != null || !currentHighlight) return;

            if (currentHighlight)
            {
                currentHighlight.gameObject.SetActive(false);
                currentHighlight = null;
            }
        }

        private void OnClick(PointerEventData pointerEventData) => onClick?.Invoke(pointerEventData, this);

        private GamePieceView AddPiece(string id)
        {
            GamePieceView _gamePiece = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(id).player1Prefab, gamePieceParent);

            _gamePiece.transform.localPosition = Vector3.zero;
            _gamePiece.StartBlinking();

            return _gamePiece;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            selectableUI = GetComponent<SelectableUI>();
            selectableUI.onEnter += OnPointerEnter;
            selectableUI.onLeave += OnPointerExit;

            button.onTap += OnClick;

            _menuScreen = menuScreen as VSLandscapeScreen;
        }
    }
}