using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SA.Foundation.Editor
{
    public class SA_Toolbar 
    {

        private int m_toolbarIndex = 0;

        private List<GUIContent> m_toolbarContent = new List<GUIContent>();
        private List<SA_iLayoutElement> m_items = new List<SA_iLayoutElement>();

  
        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="text">Item toolbar button text</param>
        /// <param name="item">Item.</param>
        public void AddItem(string text, SA_iLayoutElement item) {
            AddItem(new GUIContent(text), item);
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="icon">Item toolbar button icon</param>
        /// <param name="item">Item.</param>
        public void AddItem(Texture2D icon, SA_iLayoutElement item) {
            AddItem(new GUIContent(string.Empty, icon), item);
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="toolbarButtonContent">item toolbar button content.</param>
        /// <param name="item">Item.</param>
        public void AddItem(GUIContent toolbarButtonContent, SA_iLayoutElement item) {
            m_items.Add(item);
            m_toolbarContent.Add(toolbarButtonContent);
        }


        /// <summary>
        /// Draw the toolbar
        /// </summary>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. 
        /// Any values passed in here will override settings defined by the style.</param>
        public void Draw(params GUILayoutOption[] options) {

            if (m_items.Count == 0) { return; }
            GUI.SetNextControlName("toolbar");
            m_toolbarIndex = GUILayout.Toolbar(m_toolbarIndex, m_toolbarContent.ToArray(), options);
            m_items[m_toolbarIndex].Draw();

        }



    }
}
