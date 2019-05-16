using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Config;
using SA.Foundation.Editor;

using UnityEditor;


namespace SA.iOS.XCode
{
	public class ISD_CapabilityLayout : SA_CollapsableWindowBlockLayout
	{
		
        public delegate ISD_CapabilitySettings.Capability GetCapability();

		private SA_HyperLabel m_stateLabel;
		//private ISD_CapabilitySettings.Capability m_capability;

		private GUIContent m_off;
        private GUIContent m_on;

		private Color m_normalColor;
        private GetCapability  m_getCapability;
        
        public ISD_CapabilityLayout(string name, string image, GetCapability getCapability, Action onGUI):base(new GUIContent(name, SA_EditorAssets.GetTextureAtPath(ISD_Skin.CAPABILITY_ICONS_PATH + image)), onGUI) {
            //m_capability = capability;

            m_getCapability = getCapability;

			m_on = new GUIContent("ON"); 
			m_off = new GUIContent("OFF");
			m_normalColor = EditorStyles.boldLabel.normal.textColor;
			m_stateLabel = new SA_HyperLabel(m_on, EditorStyles.boldLabel);
			m_stateLabel.SetMouseOverColor(SA_PluginSettingsWindowStyles.SelectedElementColor);
        }


		protected override void OnAfterHeaderGUI() {

            var capability = m_getCapability();

			if(capability.Enabled) {
				m_stateLabel.SetContent(m_on);
				m_stateLabel.SetColor(SA_PluginSettingsWindowStyles.SelectedElementColor);
			} else {
				m_stateLabel.SetContent(m_off);
				m_stateLabel.SetColor(m_normalColor);
			}

			GUILayout.FlexibleSpace();
			bool click = m_stateLabel.Draw(GUILayout.Width(40));
			if(click) {
                capability.Enabled = !capability.Enabled;
			}
        }

      
	}
}