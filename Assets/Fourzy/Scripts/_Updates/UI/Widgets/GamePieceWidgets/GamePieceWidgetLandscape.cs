//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class GamePieceWidgetLandscape : WidgetBase
    {
        [HideInInspector]
        public GamePieceData data;

        public RectTransform gamePieceParent;
        public RectTransform player1Marker;
        public RectTransform player2Marker;

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

        //no need to update widget yet

        //public void UpdateData(GamePieceData _data)
        //{
        //    if (_data == null || data.ID != _data.ID) return;

        //    SetData(_data);
        //}

        //public override void _Update() => UpdateData(data);

        private GamePieceView AddPiece(string id)
        {
            GamePieceView _gamePiece = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(id).player1Prefab, gamePieceParent);

            _gamePiece.transform.localPosition = Vector3.zero;
            _gamePiece.StartBlinking();

            return _gamePiece;
        }
    }
}