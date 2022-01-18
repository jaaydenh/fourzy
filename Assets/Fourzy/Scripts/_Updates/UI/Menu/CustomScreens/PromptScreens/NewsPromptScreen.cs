//@vadym udod

using System.Collections.Generic;
using System.Linq;
using TMPro;

#if !MOBILE_SKILLZ
using PlayFab.ClientModels;
#endif

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class NewsPromptScreen : PromptScreen
    {
        public TMP_Text date;

#if !MOBILE_SKILLZ
        private int currentNewsIndex = -1;
        private List<TitleNewsItem> news = new List<TitleNewsItem>();

        public void _Prompt()
        {
            //sort by unread
            news = GameManager.Instance.latestNews
                .OrderBy(item => PlayerPrefsWrapper.GetNewsOpened(item.NewsId))
                .ToList();

            if (news.Count == 0)
            {
                Prompt(LocalizationManager.Value("no_news"), "", "", "");
            }
            else
            {
                OpenNewsPage(0);
            }
        }

        public void Previous() => OpenNewsPage(currentNewsIndex - 1);

        public void Next() => OpenNewsPage(currentNewsIndex + 1);

        private void OpenNewsPage(int page)
        {
            currentNewsIndex = page;

            if (menuController.currentScreen != this)
            {
                Prompt();
            }

            promptTitle.text = news[page].Title;
            promptText.text = news[page].Body;
            date.text = news[page].Timestamp.ToString();

            UpdateAcceptButton(page > 0 ? LocalizationManager.Value("previous") : "");
            UpdateDeclineButton(page < news.Count - 1 ? LocalizationManager.Value("next") : "");

            //set news as opened
            PlayerPrefsWrapper.SetNewsOpened(news[page].NewsId, true);
        }
#endif
    }
}