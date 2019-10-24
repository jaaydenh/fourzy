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

        public virtual WidgetBase SetData(GamePieceData data)
        {
            if (gamePiece && gamePiece.pieceData.ID != data.ID)
            {
                Destroy(gamePiece.gameObject);
                gamePiece = AddPiece(data.ID);
            }
            else if (!gamePiece)
                gamePiece = AddPiece(data.ID);

            this.data = data;

            switch (data.State)
            {
                case GamePieceState.FoundAndLocked:
                case GamePieceState.FoundAndUnlocked:
                    piecesCount.text = string.Format("{0}/{1}", data.Pieces, data.GetCurrentTierProgression);
                    break;

                case GamePieceState.NotFound:
                    piecesCount.text = "";
                    break;
            }

            UpdateProgressBar();

            return this;
        }

        public virtual void UpdateProgressBar()
        {
            float progressValue = (float)data.Pieces / data.GetCurrentTierProgression;

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
            if (_data == null || data.ID != _data.ID) return;

            SetData(_data);
        }

        public override void _Update() => UpdateData(data);

        private GamePieceView AddPiece(string id)
        {
            GamePieceView _gamePiece = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(id).player1Prefab, gamePieceParent);

            _gamePiece.transform.localPosition = Vector3.zero;
            _gamePiece.transform.localScale = Vector3.one * 90f;
            _gamePiece.StartBlinking();

            return _gamePiece;
        }
    }
}