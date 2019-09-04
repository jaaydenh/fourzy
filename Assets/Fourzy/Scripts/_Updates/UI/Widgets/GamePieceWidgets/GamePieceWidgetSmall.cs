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

        public GamePieceView gamePiece { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            GamePieceData.onUpgrade += UpdateData;
        }

        protected void OnDestroy()
        {
            GamePieceData.onUpgrade -= UpdateData;
        }

        public virtual void SetData(GamePieceData data)
        {
            this.data = data;

            if (gamePiece) Destroy(gamePiece.gameObject);

            gamePiece = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(data.ID).player1Prefab, gamePieceParent);

            gamePiece.transform.localPosition = Vector3.zero;
            gamePiece.transform.localScale = Vector3.one * 90f;
            gamePiece.StartBlinking();

            switch (data.State)
            {
                case GamePieceState.FoundAndLocked:
                case GamePieceState.FoundAndUnlocked:
                    piecesCount.text = string.Format("{0}/{1}", data.pieces, data.GetCurrentTierProgression);
                    break;

                case GamePieceState.NotFound:
                    piecesCount.text = "";
                    break;
            }

            UpdateProgressBar();
        }

        public virtual void UpdateProgressBar()
        {
            float progressValue = (float)data.pieces / data.GetCurrentTierProgression;

            progressBar.value = Mathf.Clamp01(progressValue);

            //set progress bar
            switch (data.State)
            {
                case GamePieceState.NotFound:
                    break;

                case GamePieceState.FoundAndLocked:
                    if (progressValue < 1f)
                        progressBar.SetColor(new Color(1f, 0f, .3f, 1f));
                    else
                        progressBar.SetColor(new Color(0f, 1f, .45f, 1f));
                    break;

                case GamePieceState.FoundAndUnlocked:
                    if (progressValue < 1f)
                        progressBar.SetColor(new Color(0f, .95f, 1f, 1f));
                    else
                        progressBar.SetColor(new Color(0f, 1f, .45f, 1f));
                    break;
            }
        }

        public void UpdateData(GamePieceData _data)
        {
            if (_data == null)
                return;

            if (data != null && data != _data)
                return;

            SetData(_data);
        }

        public override void _Update()
        {
            UpdateData(data);
        }
    }
}