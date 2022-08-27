//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.UI.Helpers;
using System;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class GamePieceWidgetSmall : WidgetBase
    {
        [SerializeField]
        protected RectTransform selectedFrame;
        [SerializeField]
        protected RectTransform gamePieceParent;
        [SerializeField]
        protected CircleProgressUI progressBar;
        [SerializeField]
        protected TextMeshProUGUI piecesCount;
        [SerializeField]
        protected Material greyscaleMaterial;
        [SerializeField]
        protected GameObject notFound;

        private Action<GamePieceWidgetSmall> onTap;

        public GamePieceView GamePiece { get; private set; }
        public GamePieceData Data { get; private set; }
        public bool UpdateLabels { get; set; } = true;

        protected override void Awake()
        {
            base.Awake();

        }

        protected void OnDestroy()
        {
        }

        public virtual WidgetBase SetData(GamePieceData data)
        {
            if (GamePiece && GamePiece.pieceData.Id != data.Id)
            {
                Destroy(GamePiece.gameObject);
                GamePiece = AddPiece(data.Id);
            }
            else if (!GamePiece)
            {
                GamePiece = AddPiece(data.Id);
            }

            Data = data;

            if (UpdateLabels)
            {
                switch (data.State)
                {
                    case GamePieceState.FoundAndLocked:
                    case GamePieceState.FoundAndUnlocked:
                        SetPiecesCount(string.Format("{0}/{1}", data.Pieces, data.PiecesToUnlock));

                        break;

                    case GamePieceState.NotFound:
                        SetPiecesCount("");

                        break;
                }
            }

            SetAsHidden(data.State == GamePieceState.NotFound);

            UpdateProgressBar();

            return this;
        }

        public void SetSelectedState(bool state) => selectedFrame.gameObject.SetActive(state);

        public void SetAsHidden(bool state)
        {
            if (!GamePiece)
            {
                return;
            }

            if (state)
            {
                GamePiece.SetMaterial(greyscaleMaterial);
                GamePiece.Sleep();
            }
            else
            {
                GamePiece.SetMaterial(null);
                GamePiece.WakeUp();
            }

            notFound.SetActive(state);
        }

        public virtual void UpdateProgressBar()
        {
            float progressValue = (float)Data.Pieces / Data.PiecesToUnlock;

            SetProgressBarValue(progressValue);

            //set progress bar
            switch (Data.State)
            {
                case GamePieceState.NotFound:
                    break;

                case GamePieceState.FoundAndLocked:
                    SetProgressbarColor_FoundAndLocked(progressValue);

                    break;

                case GamePieceState.FoundAndUnlocked:
                    SetProgressbarColor_FoundAndUnlocked(progressValue);

                    break;
            }
        }

        public virtual void SetProgressBarValue(float value)
        {
            progressBar.value = Mathf.Clamp01(value);
        }

        public virtual void SetProgressbarColor_FoundAndLocked(float value)
        {
            if (value < 1f)
            {
                progressBar.SetColor(new Color(1f, 0f, .3f, 1f));
            }
            else
            {
                progressBar.SetColor(new Color(0f, 1f, .45f, 1f));
            }
        }

        public virtual void SetProgressbarColor_FoundAndUnlocked(float value)
        {
            if (value < 1f)
            {
                progressBar.SetColor(new Color(0f, .95f, 1f, 1f));
            }
            else
            {
                progressBar.SetColor(new Color(0f, 1f, .45f, 1f));
            }
        }

        public virtual void SetPiecesCount(string text)
        {
            piecesCount.text = text;
        }

        public virtual void UpdateData(GamePieceData _data)
        {
            if (_data == null || Data.Id != _data.Id) return;

            SetData(_data);
        }

        public void SetOnTap(Action<GamePieceWidgetSmall> action)
        {
            onTap = action;
        }

        public virtual void OnTap()
        {
            onTap?.Invoke(this);
        }

        public override void _Update()
        {
            UpdateData(Data);
        }

        private GamePieceView AddPiece(string id)
        {
            GamePieceView _gamePiece = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePieceData(id).player1Prefab, gamePieceParent);

            _gamePiece.transform.localPosition = Vector3.zero;
            _gamePiece.transform.localScale = Vector3.one * 50f;
            _gamePiece.StartBlinking();

            return _gamePiece;
        }
    }
}