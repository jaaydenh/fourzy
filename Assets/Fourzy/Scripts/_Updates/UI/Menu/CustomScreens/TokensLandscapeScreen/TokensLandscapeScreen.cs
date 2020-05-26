//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class TokensLandscapeScreen : MenuScreen
    {
        public GameObject nextPageButton;
        public GameObject prevPageButton;

        public GridLayoutGroup tokensParent;

        private RectTransform gridRectTransform;
        private List<WidgetBase> tokenWidgets;

        private int columns = 1;
        private int rows = 1;
        private int currentPage = -1;
        private int pages = 0;
        private int elementsPerPage;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            gridRectTransform = tokensParent.GetComponent<RectTransform>();

            Vector2 spacing = tokensParent.spacing;
            List<TokensDataHolder.TokenData> allTokens = GameContentManager.Instance.enabledTokens;
            Debug.Log("Tokens count: " + allTokens.Count);
            TokenWidget prefab = GameContentManager.GetPrefab<TokenWidget>(GameContentManager.PrefabType.TOKEN_SMALL);

            Vector2 size = tokensParent.cellSize;
            Vector2 prefabSize = size;

            while (size.x + prefabSize.x + spacing.x < gridRectTransform.rect.width)
            {
                columns++;
                size.x += prefabSize.x + spacing.x;
            }

            while (size.y + prefabSize.y + spacing.y < gridRectTransform.rect.height)
            {
                rows++;
                size.y += prefabSize.y + spacing.y;
            }

            elementsPerPage = columns * rows;
            pages = Mathf.CeilToInt((float)allTokens.Count / elementsPerPage);

            tokenWidgets = new List<WidgetBase>();
            //load tokens
            foreach (TokensDataHolder.TokenData data in allTokens)
            {
                WidgetBase widget = Instantiate(prefab, gridRectTransform).SetData(data);
                widget.SetActive(false);

                tokenWidgets.Add(widget);
            }

            nextPageButton.SetActive(pages > 1);
            prevPageButton.SetActive(pages > 1);

            SetPage(0);
        }

        public override void OnBack()
        {
            base.OnBack();

            CloseSelf();
        }

        public void NextPage() => SetPage((currentPage + 1) % pages);

        public void PrevPage() => SetPage((currentPage - 1 + pages) % pages);

        public void SetPage(int page)
        {
            int start;
            if (currentPage > -1)
            {
                start = currentPage * elementsPerPage;
                for (int index = start; index < start + elementsPerPage; index++) tokenWidgets[index].SetActive(false);
            }

            currentPage = page;

            start = currentPage * elementsPerPage;
            for (int index = start; index < start + elementsPerPage; index++) tokenWidgets[index].SetActive(true);
        }
    }
}