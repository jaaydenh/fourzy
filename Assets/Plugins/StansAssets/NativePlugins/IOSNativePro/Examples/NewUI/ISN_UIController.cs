using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA.iOS.Examples
{
    public class ISN_UIController : SA_UIController
    {
        public ScrollRect scroll;

        public override GameObject AddSideMenuElement(List<SA_ExampleSubsectionConfig> subsections, bool isActive)
        {
            GameObject scroll = Instantiate(m_sideScroll, m_sideScroll.transform.parent);

            for (int counter = 0; counter < subsections.Count; counter++)
            {
                GameObject tmp = Instantiate(m_sideMenuItem, scroll.transform);

                Toggle toggle = tmp.GetComponent<Toggle>();

                if (counter == 0) {
                    toggle.isOn = true;
                    if (OnSideMenuClick != null && isActive)
                        OnSideMenuClick(subsections[counter].Scene.SceneName);
                }

                SetToggle(subsections[counter].Scene.SceneName, subsections[counter].Name, tmp.GetComponent<SA_SideToggleStyle>(), toggle);

                tmp.SetActive(true);
            }

            if(isActive) {
                scroll.SetActive(isActive);
                m_prevSideItem = scroll;
            }

            return scroll;
        }

        private void SetToggle(string sceneName, string text, SA_SideToggleStyle style, Toggle toggle)
        {
            toggle.onValueChanged.AddListener((value) => {
                if (value)
                {
                    if (OnSideMenuClick != null)
                        OnSideMenuClick(sceneName);
                    style.SetBlue();
                }
                else
                {
                    style.SetWhite();
                }
            });

            style.SetParam(text);
        }


        public override Toggle AddTopMenuElement(string name, Sprite icon, bool isActive, int index)
        {
            GameObject tmp = Instantiate(m_topMenuItem, m_topMenuItem.transform.parent);

            SA_TopToggleStyle style = tmp.GetComponent<SA_TopToggleStyle>();

            Toggle toggle = style.SetToggle(isActive, name, icon);

            toggle.onValueChanged.AddListener((value) => {
                if (value) {
                    if (OnTopMenuClick != null)
                        OnTopMenuClick(index);
                } else {
                    style.SetBlue();
                }
            });

            tmp.SetActive(true);

            return toggle;
        }

        public override void SetLogo(Sprite sprite)
        {
            m_logo.sprite = sprite;
        }

        public override void UnloadContent()
        {
            for (int counter = m_mainContent.transform.childCount - 1; counter >= 0; counter--)
            {
                Destroy(m_mainContent.transform.GetChild(counter).gameObject);
            }
        }

        public override void LoadContent(GameObject canvas)
        {
            for (int counter = canvas.transform.childCount - 1; counter >= 0; counter--)
            {
                Transform tmp = canvas.transform.GetChild(counter);
                tmp.SetParent(m_mainContent.transform, false);
                tmp.SetAsFirstSibling();
            }
        }

        public override void ShowTopMenuItem(GameObject menuItem)
        {
            if (m_prevSideItem != null)
                m_prevSideItem.SetActive(false);
            m_prevSideItem = menuItem;
            m_prevSideItem.SetActive(true);
        }

        public override void AddLog(string log)
        {
            m_logger.text +=  log + "\n";

            Canvas.ForceUpdateCanvases();
            scroll.verticalNormalizedPosition = 0f;
        }
    }
}
