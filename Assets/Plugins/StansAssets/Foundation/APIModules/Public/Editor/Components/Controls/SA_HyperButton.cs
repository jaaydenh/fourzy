using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;


namespace SA.Foundation.Editor
{

    [Serializable]
	public abstract class SA_HyperButton 
    {

        private bool m_isMouseOver = false;
        private Rect m_labelRect = new Rect();


        [SerializeField] bool m_isSelected = false;


        protected abstract void OnNormal(params GUILayoutOption[] options);
        protected abstract void OnMouseOver(params GUILayoutOption[] options);




        /// <summary>
        /// True if current elements selection state is locked
        /// </summary>
        public bool IsSelectionLock {
            get {
                return m_isSelected;
            }
        }




        /// <summary>
        /// Locked button in a selected state
        /// OnMouseOver mode UI will be drawn, and button will not accept 
        /// the mouse click event.
        /// </summary>
        public void LockSelectedState(bool val) {
            m_isSelected = val;
        }


        public virtual bool Draw(params GUILayoutOption[] options) {

            if(m_isSelected) {
                OnMouseOver(options);
                return false;
            }


            if(!m_isMouseOver) {
                OnNormal(options);
            } else {
                OnMouseOver(options);
            }


            if (Event.current.type == EventType.Repaint) {
                m_labelRect = GUILayoutUtility.GetLastRect();
                m_isMouseOver = m_labelRect.Contains(Event.current.mousePosition);
            }


            if (Event.current.type == EventType.Repaint) {
                if (m_isMouseOver) {
                    EditorGUIUtility.AddCursorRect(m_labelRect, MouseCursor.Link);
                } 
            }

            var clicked = false;
            if (m_isMouseOver) {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                    clicked = true;
                    GUI.changed = true;
                    Event.current.Use();
                }
            }

            return clicked;
        }




    }
}