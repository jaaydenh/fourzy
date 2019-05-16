using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;
using SA.Foundation.Config;


namespace SA.Foundation.Editor
{
    [Serializable]
    public class SA_PluginActiveTextLink : SA_HyperLabel
    {

        public SA_PluginActiveTextLink(string title)
            : base(new GUIContent(title), SA_PluginSettingsWindowStyles.DescribtionLabelStyle) {
            SetColor(SA_PluginSettingsWindowStyles.ActiveLinkColor);
            SetMouseOverColor(SA_PluginSettingsWindowStyles.SelectedElementColor);
        }
    }
}