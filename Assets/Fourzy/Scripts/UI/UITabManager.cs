using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    public class UITabManager : MonoBehaviour
    {
        public static UITabManager instance;
        public List<UITabButton> tabButtonList = new List<UITabButton>();
        public UITabButton defaultTab;

        public void Awake()
        {
            instance = this;
        }

        public void ResetTabs(UITabButton currentTab)
        {
            //Debug.Log("ResetTabs");
            foreach (UITabButton tabButton in tabButtonList)
            {
                if (tabButton != currentTab)
                {
                    tabButton.ResetTab();
                }
            }
        }

        public void ResetAllTabs()
        {
            foreach (UITabButton tabButton in tabButtonList)
            {
                tabButton.ResetTab();
            }
        }
    }
}
