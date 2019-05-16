using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using SA.iOS;

namespace SA.iOS.Examples {
    
    public class SA_TopToggleStyle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Text m_label;
        private Image m_icon;
        private Toggle m_toggle;


        public Toggle SetToggle(bool isActive, string text, Sprite sprite) {
            m_label = transform.GetChild(1).GetComponent<Text>();
            m_label.text = text;

            m_icon = transform.GetChild(2).GetComponent<Image>();
            m_icon.sprite = sprite;

            m_toggle = GetComponent<Toggle>();
            m_toggle.isOn = isActive;

            if (m_toggle.isOn)
                SetWhite();
            else
                SetBlue();

            return m_toggle;
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!m_toggle.isOn)
                SetWhite();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!m_toggle.isOn)
                SetBlue();
        }

        public void SetWhite()
        {
            m_label.color = Color.white;
            m_icon.color = Color.white;
        }

        public void SetBlue()
        {
            m_label.color = new Color(0.18f, 0.26f, 0.33f);
            m_icon.color = new Color(0.18f, 0.26f, 0.33f);
        }


    }
}
