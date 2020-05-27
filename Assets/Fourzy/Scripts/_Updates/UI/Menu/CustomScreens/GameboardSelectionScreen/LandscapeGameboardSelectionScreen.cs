//@vadym udod

using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class LandscapeGameboardSelectionScreen : MenuScreen
    {
        public Action<GameBoardDefinition> onBoardSelected;

        public TMP_Text areaNameField;
        public RectTransform widgetsParent;
        public GridLayoutGroup gridLayout;
        public MiniGameboardWidget miniGameboardPrefab;
        public GameObject nextButton;
        public GameObject prevButton;
        public Area filterByArea = Area.NONE;

        private List<GameBoardDefinition> allBoards;
        private List<MiniGameboardWidget> gameboardWidgets;
        private List<MiniGameboardWidget> areaBoards;

        private MiniGameboardWidget current;
        private int boardsPerPage;
        private int currentPage = -1;
        private int maxPages;

        public List<MiniGameboardWidget> Boards => filterByArea != Area.NONE ? areaBoards : gameboardWidgets;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            gridLayout = widgetsParent.GetComponent<GridLayoutGroup>();
            //get page size
            RectTransform _widgetRect = miniGameboardPrefab.GetComponent<RectTransform>();
            int cols = 0;
            float _width = _widgetRect.rect.width;
            while (widgetsParent.rect.width > _width)
            {
                cols++;
                _width += _widgetRect.rect.width + gridLayout.spacing.x;
            }

            int rows = 0;
            float _height = _widgetRect.rect.height;
            while (widgetsParent.rect.height > _height)
            {
                rows++;
                _height += _widgetRect.rect.height + gridLayout.spacing.y;
            }

            boardsPerPage = cols * rows;

            areaBoards = new List<MiniGameboardWidget>();
            allBoards = GameContentManager.Instance.passAndPlayGameboards;
            gameboardWidgets = new List<MiniGameboardWidget>();

            CreateWidgets();
        }

        public override void Open()
        {
            base.Open();

            areaNameField.text = LocalizationManager.Value(filterByArea != Area.NONE ? GameContentManager.Instance.themesDataHolder.GetThemeName(filterByArea) : "random");
            areaBoards = gameboardWidgets.Where(widget => widget.data == null || (widget.data != null && widget.data.Area == filterByArea)).ToList();
            maxPages = Mathf.CeilToInt((float)Boards.Count / boardsPerPage);

            if (current)
            {
                current.Deselect(0f);
                current = null;
            }

            bool buttonsActive = Boards.Count > boardsPerPage + 1;
            nextButton.SetActive(buttonsActive);
            prevButton.SetActive(buttonsActive);

            HideBoards();

            SetPage(0);
        }

        public void Open(Area area)
        {
            filterByArea = area;

            menuController.OpenScreen(this);
        }

        public void NextPage() => SetPage((currentPage + 1) % maxPages);

        public void PrevPage() => SetPage((currentPage - 1 + maxPages) % maxPages);

        private void CreateWidgets()
        {
            //clear old
            foreach (MiniGameboardWidget widget in gameboardWidgets) Destroy(widget.gameObject);
            gameboardWidgets.Clear();
            areaBoards.Clear();

            //add random
            gameboardWidgets.Add(Instantiate(miniGameboardPrefab, widgetsParent).SetOnClick(OnMinigameboardClick));

            foreach (GameBoardDefinition boardDefinition in allBoards)
                gameboardWidgets.Add(
                    Instantiate(miniGameboardPrefab, widgetsParent)
                        .QuickLoadBoard(boardDefinition, false)
                        .SetOnClick(OnMinigameboardClick));

            foreach (MiniGameboardWidget widget in gameboardWidgets) widget.SetActive(false);
        }

        private void SetPage(int page)
        {
            //disable prev
            if (currentPage > -1) HideBoards();

            //enable next
            int startIndex = page * boardsPerPage;
            int to = Mathf.Min(startIndex + boardsPerPage, Boards.Count);
            for (int widgetIndex = startIndex; widgetIndex < to; widgetIndex++) Boards[widgetIndex].SetActive(true);

            currentPage = page;
        }

        private void OnMinigameboardClick(MiniGameboardWidget miniGameboard)
        {
            if (current) current.Deselect(.25f);

            onBoardSelected?.Invoke(miniGameboard.data);
            current = miniGameboard;

            current.Select(.25f);

            OnBack();
        }

        private void HideBoards()
        {
            foreach (MiniGameboardWidget widget in gameboardWidgets) widget.SetActive(false);
        }
    }
}