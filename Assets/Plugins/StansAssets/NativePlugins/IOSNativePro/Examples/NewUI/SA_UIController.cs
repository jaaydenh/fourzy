using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA.iOS.Examples
{
    public abstract class SA_UIController : MonoBehaviour
    {
        public Action<int> OnTopMenuClick;

        public Action<String> OnSideMenuClick;

        [SerializeField]
        protected Image m_logo;

        [SerializeField]
        protected GameObject m_topMenuItem;

        [SerializeField]
        protected GameObject m_sideMenuItem;

        [SerializeField]
        protected GameObject m_sideScroll;

        [SerializeField]
        protected GameObject m_mainContent;

        [SerializeField]
        protected Text m_logger;

        protected GameObject m_prevSideItem;

        abstract public void SetLogo(Sprite sprite);

        abstract public Toggle AddTopMenuElement(string name, Sprite icon, bool isActive, int index);

        abstract public void ShowTopMenuItem(GameObject menuItem);

        abstract public GameObject AddSideMenuElement(List<SA_ExampleSubsectionConfig> subsections, bool isActive);

        abstract public void UnloadContent();

        abstract public void LoadContent(GameObject canvas);

        abstract public void AddLog(string log);
    }
}