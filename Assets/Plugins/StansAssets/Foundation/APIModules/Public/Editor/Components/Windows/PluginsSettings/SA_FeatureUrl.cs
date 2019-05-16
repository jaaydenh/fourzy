using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Editor;
using SA.Foundation.Config;


namespace SA.Foundation.Editor
{
    [Serializable]
    public class SA_FeatureUrl : SA_HyperLabel
    {

        [SerializeField] string m_url;

        public SA_FeatureUrl(string title, string url)
            :base(new GUIContent(
                title,
                SA_Skin.GetGenericIcon("list_arrow_white.png")
            ), 
                  SA_PluginSettingsWindowStyles.DescribtionLabelStyle)
        {

            m_url = url;
            SetMouseOverColor(SA_PluginSettingsWindowStyles.SelectedElementColor);

        }


        public void DrawLink(params GUILayoutOption[] options) {

            bool click = Draw(options);
            if(click) {
                Application.OpenURL(m_url);
            }

        }
    }
}