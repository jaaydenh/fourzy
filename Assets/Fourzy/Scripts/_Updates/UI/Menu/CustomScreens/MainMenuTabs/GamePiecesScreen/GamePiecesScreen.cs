//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System.Collections.Generic;
using System.Linq;
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
        public ButtonExtended unlockedButton;
        public ButtonExtended foundButton;
        public ButtonExtended lockedButton;
        public GridLayoutGroup unlockedPiecesParent;
        public GridLayoutGroup foundPiecesParent;
        public GridLayoutGroup lockedPiecesParent;
        public RectTransform unlockedCover;
        public RectTransform foundCover;
        public RectTransform lockedCover;
        public SizeTween unlockedCoverTween;
        public SizeTween foundCoverTween;
        public SizeTween lockedCoverTween;
        public RotationTween arrowUnlocked;
        public RotationTween arrowFound;
        public RotationTween arrowLocked;
        public TMP_Text unlockedLabel;
        public TMP_Text foundLabel;
        public TMP_Text lockedLabel;

        public RectTransform tokensParent;

        public override bool isCurrent => base.isCurrent;

        protected List<GamePieceWidgetMedium> gamePieces = new List<GamePieceWidgetMedium>();
        protected List<TokenWidget> tokens = new List<TokenWidget>();

        protected bool unlockedExpanded = true;
        protected bool foundExpanded = true;
        protected bool lockedExpanded = true;

        private bool catalogLoaded = false;

        protected override void Awake()
        {
            base.Awake();

            InternalSettings.onLoaded += OnInternalSettingsLoaded;
            UserManager.onTokenUnlocked += OnTokenUnlocked;
            UserManager.onPlayfabValuesLoaded += OnPlayfabValueLoaded;
        }

        protected void OnDestroy()
        {
            InternalSettings.onLoaded -= OnInternalSettingsLoaded;
            UserManager.onTokenUnlocked -= OnTokenUnlocked;
            UserManager.onPlayfabValuesLoaded -= OnPlayfabValueLoaded;
        }

        public override void Open()
        {
            base.Open();

            foreach (GamePieceWidgetMedium widget in gamePieces)
            {
                //highlight selected one
                widget.SetSelectedState(widget.data.Id == UserManager.Instance.gamePieceId);
            }

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

        public void ToggleFoundPiecesExpand()
        {
            foundExpanded = !foundExpanded;

            if (foundExpanded)
            {
                foundCoverTween.PlayForward(true);
                arrowFound.PlayForward(true);
            }
            else
            {
                foundCoverTween.PlayBackward(true);
                arrowFound.PlayBackward(true);
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
            foreach (GamePieceData prefabData in GameContentManager.Instance.piecesDataHolder.gamePieces)
            {
                GamePieceWidgetMedium widget = GameContentManager.InstantiatePrefab<GamePieceWidgetMedium>(
                    "GAME_PIECE_MEDIUM",
                    transform);
                widget.SetData(prefabData);

                widgets.Add(widget);
                gamePieces.Add(widget);
            }
        }

        private void CreateTokens()
        {
            while (tokens.Count > 0)
            {
                Destroy(tokens[0].gameObject);
                widgets.Remove(tokens[0]);
                tokens.RemoveAt(0);
            }

            //load tokens
            foreach (TokensDataHolder.TokenData data in GameContentManager.Instance.tokens)
            {
                TokenWidget _tokenWidget = GameContentManager.InstantiatePrefab<TokenWidget>("TOKEN_SMALL", tokensParent);
                _tokenWidget.SetData(data);

                tokens.Add(_tokenWidget);
                widgets.Add(_tokenWidget);
            }
        }

        private void UpdateTokens()
        {
            var unlockedTokens = PlayerPrefsWrapper.GetUnlockedTokens().Select(_data => _data.tokenType);

            foreach (TokenWidget tokenWidget in tokens)
            {
                bool state = unlockedTokens.Contains(tokenWidget.tokenData.tokenType);

                tokenWidget.SetState(state);

                //sort
                if (state)
                {
                    tokenWidget.transform.SetAsFirstSibling();
                }
            }
        }

        private void PlaceGamepieceWidgets()
        {
            List<GamePieceWidgetMedium> unlocked = new List<GamePieceWidgetMedium>();
            List<GamePieceWidgetMedium> found = new List<GamePieceWidgetMedium>();
            List<GamePieceWidgetMedium> locked = new List<GamePieceWidgetMedium>();

            foreach (GamePieceWidgetMedium widget in gamePieces)
            {
                switch (widget.data.State)
                {
                    case GamePieceState.FoundAndUnlocked:
                        unlocked.Add(widget);

                        break;

                    case GamePieceState.FoundAndLocked:
                        found.Add(widget);

                        break;

                    case GamePieceState.NotFound:
                        locked.Add(widget);

                        break;
                }
            }

            //sort if needed
            //

            unlocked.ForEach(widget => widget.transform.SetParent(unlockedPiecesParent.transform));
            found.ForEach(widget => widget.transform.SetParent(foundPiecesParent.transform));
            locked.ForEach(widget => widget.transform.SetParent(lockedPiecesParent.transform));

            Vector2 unlcokedSize = unlockedPiecesParent.GetGridLayoutSize(unlocked.Count);
            Vector2 foundSize = foundPiecesParent.GetGridLayoutSize(found.Count);
            Vector2 lockedSize = lockedPiecesParent.GetGridLayoutSize(locked.Count);

            if (unlockedExpanded) unlockedCover.sizeDelta = new Vector2(unlockedCover.sizeDelta.x, unlcokedSize.y);
            if (foundExpanded) foundCover.sizeDelta = new Vector2(foundCover.sizeDelta.x, foundSize.y);
            if (lockedExpanded) lockedCover.sizeDelta = new Vector2(lockedCover.sizeDelta.x, lockedSize.y);

            unlockedCoverTween.from = new Vector2(unlockedCover.rect.width, 0f);
            unlockedCoverTween.to = new Vector2(unlockedCover.rect.width, unlcokedSize.y);

            foundCoverTween.from = new Vector2(foundCover.rect.width, 0f);
            foundCoverTween.to = new Vector2(foundCover.rect.width, foundSize.y);

            lockedCoverTween.from = new Vector2(lockedCover.rect.width, 0f);
            lockedCoverTween.to = new Vector2(lockedCover.rect.width, lockedSize.y);

            //hide/show buttons
            unlockedButton.SetActive(unlocked.Count != 0);
            unlockedCover.gameObject.SetActive(unlocked.Count != 0);
            foundButton.SetActive(found.Count != 0);
            foundCover.gameObject.SetActive(found.Count != 0);
            lockedButton.SetActive(locked.Count != 0);
            lockedCover.gameObject.SetActive(locked.Count != 0);

            unlockedLabel.text = $"{LocalizationManager.Value("unlocked")} {unlocked.Count}/" +
                $"{GameContentManager.Instance.piecesDataHolder.gamePieces.Count}";
            foundLabel.text = $"{LocalizationManager.Value("found")} {found.Count}/" +
                $"{GameContentManager.Instance.piecesDataHolder.gamePieces.Count}";
            lockedLabel.text = $"{LocalizationManager.Value("not_found")} {locked.Count}/" +
                $"{GameContentManager.Instance.piecesDataHolder.gamePieces.Count}";
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            CreateGamePieces();
            CreateTokens();
            UpdateTokens();

            SetPiecesActive();
        }

        private void OnInternalSettingsLoaded()
        {
            UpdateTokens();
        }

        private void OnTokenUnlocked(IEnumerable<TokenType> newTokens, TokenUnlockType unlockType)
        {
            UpdateTokens();
        }

        private void OnPlayfabValueLoaded(PlayfabValuesLoaded value)
        {
            if (value == PlayfabValuesLoaded.USER_INVENTORY_RECEIVED)
            {
                UpdateTokens();
            }

            if (UserManager.Instance.IsPlayfabValueLoaded(PlayfabValuesLoaded.CATALOG_INFO_RECEIVED, PlayfabValuesLoaded.USER_INVENTORY_RECEIVED) && !catalogLoaded)
            {
                catalogLoaded = true;

                if (isOpened)
                {
                    foreach (GamePieceWidgetMedium widget in gamePieces)
                    {
                        widget._Update();
                    }

                    PlaceGamepieceWidgets();
                }
            }
        }
    }
}
