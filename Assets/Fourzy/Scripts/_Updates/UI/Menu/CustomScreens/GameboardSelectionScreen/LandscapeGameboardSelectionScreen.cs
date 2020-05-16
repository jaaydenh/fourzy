//@vadym udod

using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class LandscapeGameboardSelectionScreen : MenuScreen
    {
        public Action<GameBoardDefinition> onBoardSelected;

        public RectTransform widgetsParent;
        public GridLayoutGroup gridLayout;
        public MiniGameboardWidget miniGameboardPrefab;

        private List<GameBoardDefinition> gameboards;
        private List<MiniGameboardWidget> gameboardWidgets;

        private MiniGameboardWidget current;
        private int boardsPerPage;
        private int currentPage = -1;
        private int maxPages;

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

            gameboards = GameContentManager.Instance.passAndPlayGameboards;
            maxPages = Mathf.CeilToInt((gameboards.Count + 1) / boardsPerPage);
            gameboardWidgets = new List<MiniGameboardWidget>();

            CreateWidgets();
            SetPage(0);
        }

        public void NextPage() => SetPage((currentPage + 1) % maxPages);

        public void PrevPage() => SetPage((currentPage - 1 + maxPages) % maxPages);

        private void CreateWidgets()
        {
            //add random
            gameboardWidgets.Add(Instantiate(miniGameboardPrefab, widgetsParent).SetOnClick(OnMinigameboardClick));

            foreach (GameBoardDefinition boardDefinition in gameboards)
                gameboardWidgets.Add(
                    Instantiate(miniGameboardPrefab, widgetsParent)
                        .QuickLoadBoard(boardDefinition, false)
                        .SetOnClick(OnMinigameboardClick));

            foreach (MiniGameboardWidget widget in gameboardWidgets) widget.SetActive(false);
        }

        private void SetPage(int page)
        {
            //disable prev
            if (currentPage > -1)
            {
                int prevStartIndex = currentPage * boardsPerPage;
                for (int widgetIndex = prevStartIndex; widgetIndex < prevStartIndex + boardsPerPage; widgetIndex++) gameboardWidgets[widgetIndex].SetActive(false);
            }

            //enable next
            int startIndex = page * boardsPerPage;
            for (int widgetIndex = startIndex; widgetIndex < startIndex + boardsPerPage; widgetIndex++) gameboardWidgets[widgetIndex].SetActive(true);

            currentPage = page;
        }

        private void OnMinigameboardClick(MiniGameboardWidget miniGameboard)
        {
            if (current) current.Deselect();

            onBoardSelected?.Invoke(miniGameboard.data);
            current = miniGameboard;

            current.Select();
        }
    }
}