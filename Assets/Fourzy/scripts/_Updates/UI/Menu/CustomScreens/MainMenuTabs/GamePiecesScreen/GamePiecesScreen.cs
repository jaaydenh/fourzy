//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GamePiecesScreen : MenuTab
    {
        public ScrollRect scrollRect;
        public RectTransform piecesTab;
        public RectTransform tokensTab;
        public RectTransform unlockedPiecesParent;
        public RectTransform lockedPiecesParent;
        public RectTransform unlockedCover;
        public RectTransform lockedCover;
        public SizeTween unlockedCoverTween;
        public SizeTween lockedCoverTween;
        public RotationTween arrowUnlocked;
        public RotationTween arrowLocked;
        public RectTransform tokensParent;
        public TMP_Text unlockedLabel;
        public TMP_Text lockedLabel;

        public override bool isCurrent => base.isCurrent;

        protected List<GamePieceWidgetMedium> gamePieceWidgets = new List<GamePieceWidgetMedium>();

        protected bool unlockedExpanded = true;
        protected bool lockedExpanded = true;

        protected override void Start()
        {
            base.Start();

            CreateGamePieces();
            CreateTokens();

            SetPiecesActive();
        }

        protected void Update()
        {
            //remove
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.S))
            {
                //give game pieces
                foreach (GamePieceWidgetMedium widget in gamePieceWidgets)
                {
                    widget.data.AddPieces(Random.Range(1, 5));
                    widget._Update();
                }

                PlaceGamepieceWidgets();
            }
#endif
        }

        public override void Open()
        {
            base.Open();

            PlaceGamepieceWidgets();
        }

        public void SetPiecesActive()
        {
            piecesTab.gameObject.SetActive(true);
            tokensTab.gameObject.SetActive(false);
            scrollRect.content = piecesTab;
        }

        public void SetTokensActive()
        {
            piecesTab.gameObject.SetActive(false);
            tokensTab.gameObject.SetActive(true);
            scrollRect.content = tokensTab;
        }

        public void ToggleUnlockedPiecesExpand()
        {
            unlockedExpanded = !unlockedExpanded;

            if (unlockedExpanded)
            {
                unlockedCoverTween.PlayForward(true);
                arrowUnlocked.PlayForward(true);
            }
            else
            {
                unlockedCoverTween.PlayBackward(true);
                arrowUnlocked.PlayBackward(true);
            }
        }

        public void ToggleLockedPiecesExpand()
        {
            lockedExpanded = !lockedExpanded;

            if (lockedExpanded)
            {
                lockedCoverTween.PlayForward(true);
                arrowLocked.PlayForward(true);
            }
            else
            {
                lockedCoverTween.PlayBackward(true);
                arrowLocked.PlayBackward(true);
            }
        }

        private void CreateGamePieces()
        {
            foreach (GamePiecePrefabData prefabData in GameContentManager.Instance.piecesDataHolder.gamePieces.list)
            {
                GamePieceWidgetMedium widget = GameContentManager.InstantiatePrefab<GamePieceWidgetMedium>(GameContentManager.PrefabType.GAME_PIECE_MEDIUM, transform);
                widget.SetData(prefabData.data);

                widgets.Add(widget);
                gamePieceWidgets.Add(widget);
            }
        }

        private void CreateTokens()
        {
            //load tokens
            foreach (TokensDataHolder.TokenData data in GameContentManager.Instance.enabledTokens)
                widgets.Add(GameContentManager.InstantiatePrefab<TokenWidget>(GameContentManager.PrefabType.TOKEN_SMALL, tokensParent).SetData(data));
        }

        private void PlaceGamepieceWidgets()
        {
            List<GamePieceWidgetMedium> unlocked = new List<GamePieceWidgetMedium>();
            List<GamePieceWidgetMedium> locked = new List<GamePieceWidgetMedium>();

            foreach (GamePieceWidgetMedium widget in gamePieceWidgets)
            {
                switch (widget.data.State)
                {
                    case GamePieceState.FoundAndLocked:
                    case GamePieceState.NotFound:
                        locked.Add(widget);

                        break;

                    case GamePieceState.FoundAndUnlocked:
                        unlocked.Add(widget);

                        break;
                }
            }

            //sort if needed
            //

            unlocked.ForEach(widget => widget.transform.SetParent(unlockedPiecesParent));
            locked.ForEach(widget => widget.transform.SetParent(lockedPiecesParent));

            LayoutRebuilder.ForceRebuildLayoutImmediate(unlockedPiecesParent);
            LayoutRebuilder.ForceRebuildLayoutImmediate(lockedPiecesParent);

            if (unlockedExpanded) unlockedCover.sizeDelta = new Vector2(unlockedCover.sizeDelta.x, unlockedPiecesParent.rect.height);
            if (lockedExpanded) lockedCover.sizeDelta = new Vector2(lockedCover.sizeDelta.x, lockedPiecesParent.rect.height);

            unlockedCoverTween.from = new Vector2(unlockedCover.rect.width, 0f);
            unlockedCoverTween.to = new Vector2(unlockedCover.rect.width, unlockedPiecesParent.rect.height);

            lockedCoverTween.from = new Vector2(lockedCover.rect.width, 0f);
            lockedCoverTween.to = new Vector2(lockedCover.rect.width, lockedPiecesParent.rect.height);

            unlockedLabel.text = $"Unlocked {unlocked.Count}/{GameContentManager.Instance.piecesDataHolder.gamePieces.list.Count}";
            lockedLabel.text = $"Locked {locked.Count}/{GameContentManager.Instance.piecesDataHolder.gamePieces.list.Count}";
        }
    }
}
