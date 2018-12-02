//@vadym udod

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class GamePieceWidgetSmall : WidgetBase
    {
        [HideInInspector]
        public GamePieceData data;

        public Image gamePieceIcon;
        public CircleProgress progressBar;
        public TextMeshProUGUI piecesCount; 

        public virtual void SetData(GamePieceData data)
        {
            this.data = data;

            gamePieceIcon.sprite = GameContentManager.Instance.GetGamePieceSprite(data.ID);

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
            float collectionProgress = ((float)data.NumberOfPieces) / data.TotalNumberOfPieces;

            progressBar.SetupNewValue(collectionProgress);

            if (collectionProgress < 1.0f)
            {
                switch (data.state)
                {
                    case GamePieceState.FoundAndUnlocked:
                        progressBar.SetupNewColor(Color.green);
                        break;
                    default:
                        progressBar.SetupNewColor(Color.yellow);
                        break;
                }
            }
            else
            {
                progressBar.SetupNewColor(Color.red);
            }
        }
    }
}