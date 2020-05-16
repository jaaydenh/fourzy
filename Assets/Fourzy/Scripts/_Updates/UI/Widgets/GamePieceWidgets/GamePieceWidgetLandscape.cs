//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using System;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class GamePieceWidgetLandscape : WidgetBase
    {
        public Action<GamePieceWidgetLandscape> onClick;

        [HideInInspector]
        public GamePieceData data;

        public RectTransform gamePieceParent;
        public GameObject player1Marker;
        public GameObject player2Marker;
        public GameObject p1Selection;
        public GameObject p2Selection;

        public GamePieceView gamePiece { get; private set; }

        public virtual GamePieceWidgetLandscape SetData(GamePieceData data)
        {
            if (gamePiece && gamePiece.pieceData.ID != data.ID)
            {
                Destroy(gamePiece.gameObject);
                gamePiece = AddPiece(data.ID);
            }
            else if (!gamePiece)
                gamePiece = AddPiece(data.ID);

            this.data = data;

            return this;
        }

        public GamePieceWidgetLandscape SelectAsPlayer(params int[] players)
        {
            bool p1 = players.Contains(0);
            bool p2 = players.Contains(1);

            player1Marker.SetActive(p1);
            player2Marker.SetActive(p2);
            p1Selection.SetActive(p1);
            p2Selection.SetActive(p2);

            return this;
        }

        public GamePieceWidgetLandscape SetOnClick(Action<GamePieceWidgetLandscape> action)
        {
            onClick = action;

            return this;
        }

        public void OnClick() => onClick?.Invoke(this);

        private GamePieceView AddPiece(string id)
        {
            GamePieceView _gamePiece = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(id).player1Prefab, gamePieceParent);

            _gamePiece.transform.localPosition = Vector3.zero;
            _gamePiece.StartBlinking();

            return _gamePiece;
        }
    }
}