//@vadym udod

using PlayFab.ClientModels;
using System.Collections.Generic;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class NewsPromptScreen : PromptScreen
    {
        private int currentNewsIndex = -1;
        private List<TitleNewsItem> news = new List<TitleNewsItem>();

        public void _Prompt()
        {
            news = GameManager.Instance.latestNews;

            if (news.Count == 0)
                Prompt("No News", "", "", "");
            else
                OpenNewsPage(0);
        }

        public void Previous() => OpenNewsPage(currentNewsIndex - 1);

        public void Next() => OpenNewsPage(currentNewsIndex + 1);

        private void OpenNewsPage(int page)
        {
            currentNewsIndex = page;

            if (menuController.currentScreen != this) Prompt();

            promptTitle.text = news[page].Title;
            promptText.text = news[page].Body;

            UpdateAcceptButton(page > 0 ? "Previous" : "");
            UpdateDeclineButton(page < news.Count - 1 ? "Next" : "");

            //set news as opened
            PlayerPrefsWrapper.SetNewsOpened(news[page].NewsId, true);
        }
    }
}