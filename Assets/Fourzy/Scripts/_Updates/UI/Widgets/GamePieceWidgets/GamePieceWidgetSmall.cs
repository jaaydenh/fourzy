//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Helpers;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class GamePieceWidgetSmall : WidgetBase
    {
        [HideInInspector]
        public GamePieceData data;

        public RectTransform gamePieceParent;
        public CircleProgressUI progressBar;
        public TextMeshProUGUI piecesCount;

        protected GamePieceView gamePiece;

        public virtual void SetData(GamePieceData data)
        {
            this.data = data;

            gamePiece = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(data.ID).prefabs[0], gamePieceParent.transform);
            gamePiece.transform.localPosition = Vector3.zero;
            gamePiece.transform.localScale = Vector3.one * 100f;
            gamePiece.gameObject.SetLayerRecursively(gamePieceParent.gameObject.layer);
            gamePiece.StartBlinking();

            switch (data.state)
            {
                case GamePieceState.FoundAndLocked:
                case GamePieceState.FoundAndUnlocked:
                    UpdateProgressBar();
                    piecesCount.text = string.Format("{0}/{1}", data.NumberOfPieces, data.TotalNumberOfPieces);
                    break;

                case GamePieceState.NotFound:
                    break;
            }
        }

        public virtual void UpdateProgressBar()
        {
            //float collectionProgress = ((float)data.NumberOfPieces) / data.TotalNumberOfPieces;

            progressBar.progress = Random.value;

            if (progressBar.progress < 1.0f)
            {
                switch (data.state)
                {
                    case GamePieceState.FoundAndUnlocked:
                        progressBar.SetColor(Color.green);
                        break;
                    default:
                        progressBar.SetColor(Color.yellow);
                        break;
                }
            }
            else
            {
                progressBar.SetColor(Color.red);
            }
        }
    }
}