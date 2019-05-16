using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SA.iOS.Examples {
    
    public class SA_SideToggleStyle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [HideInInspector]
        public Text Label;

        private GameObject m_mainContent;

        private string m_sceneName; 
        private Toggle m_toggle;

        public void SetParam(string name) {


            Label = transform.GetChild(1).GetComponent<Text>();
            Label.text = name;

            m_toggle = GetComponent<Toggle>();

            if (m_toggle.isOn) {
                SetBlue();
            } else
                SetWhite();
            
        }


        public void OnPointerEnter(PointerEventData eventData) {
            if (!m_toggle.isOn)
                SetBlue();
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (!m_toggle.isOn)
                SetWhite();
        }

        public void SetWhite() {
            Label.color = Color.white;
        }

        public void SetBlue() {
            Label.color = new Color(0.18f, 0.26f, 0.33f);
        }


    }
}