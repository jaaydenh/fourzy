using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace SA.Foundation.Editor
{

    [Serializable]
    public class SA_HyperToolbar 
    {

        [SerializeField] int m_buttonsWidth = 0;
        [SerializeField] int m_buttonsHeight = 0;
        [SerializeField] float m_itemsSapce = 5f;
        [SerializeField] List<SA_HyperLabel> m_buttons = new List<SA_HyperLabel>();



        public void AddButtons(params SA_HyperLabel[] buttons) {

            if(m_buttons == null) {
                m_buttons = new List<SA_HyperLabel>();
            }

            foreach(var newBtn in buttons) {
                m_buttons.Add(newBtn);
            }

            ValidateButtons();
        }


        private void ValidateButtons() {

            if(m_buttons.Count == 0) {
                return;
            }


            bool hasActive = false;
            foreach (var button in m_buttons) {
              if(button.IsSelectionLock) {
                    hasActive = true;
                    break;
                }
            }

            if(!hasActive) {
                m_buttons[0].LockSelectedState(true);
            }
        }


        public void SetSelectedIndex(int index) {
           
            foreach(var button in m_buttons) {
                button.LockSelectedState(false);
            }

            var selectedButton = m_buttons[index];
            selectedButton.LockSelectedState(true);
        }


        /// <summary>
        /// Draw toolbar with buttons.
        /// Returns selected button index
        /// </summary>
        public int Draw() {

            if(m_buttons == null) {
                return 0;
            }
           

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space();

                for(int i = 0; i < m_buttons.Count; i++) {

                    float width;
                    var button = m_buttons[i];

                    if (m_buttonsWidth == 0) {
                        width = button.CalcSize().x + m_itemsSapce;
                    } else {
                        width = m_buttonsWidth;
                    }

                    bool click;
                    if (m_buttonsHeight !=0) {
                        click = button.Draw(GUILayout.Width(width), GUILayout.Height(m_buttonsHeight));
                    } else {
                        click = button.Draw(GUILayout.Width(width));
                    }

                    if (click) {
                        SetSelectedIndex(m_buttons.IndexOf(button));
                    }
                }
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndHorizontal();

            return SelectionIndex;
        }


        /// <summary>
        /// Set's custom width for all toolbar buttons.
        /// The default value is 0. When value is 0 button width is calculated
        /// based on button GUI rect.
        /// </summary>
        /// <param name="width">width value</param>
        public void SetButtonsWidth(int width) {
            m_buttonsWidth = width;
        }

        /// <summary>
        /// Set's custom height for all toolbar buttons.
        /// The default value is 0. When value is 0 button height is calculated
        /// based on button GUI rect.
        /// </summary>
        /// <param name="height">height value</param>
        public void SetButtonsHeight(int height) {
            m_buttonsHeight = height;
        }

        /// <summary>
        /// Set's space betwen items.
        /// Default space it 5
        /// </summary>
        /// <param name="space">items space value</param>
        public void SetItemsSapce(float space) {
            m_itemsSapce = space;
        }



        /// <summary>
        /// Toolbar buttons
        /// </summary>
        public List<SA_HyperLabel> Buttons {
            get {
                return m_buttons;
            }
        }

        public int SelectionIndex {
            get {
                foreach (var button in m_buttons) {
                    if (button.IsSelectionLock) {
                        return m_buttons.IndexOf(button);
                    }
                }

                return 0;
            }
        }
    }
	
}
