using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;


using SA.Foundation.Config;
using SA.Foundation.Utility;

namespace SA.Foundation.Editor
{
    [Serializable]
	public class SA_CollapsableWindowBlockLayout 
    {

		private Action m_onGUI;      
		private SA_HyperLabel m_header;
		private SA_HyperLabel m_arrrow;

		private AnimBool m_showExtraFields = new AnimBool(false);

		private GUIContent m_collapsedContent;
		private GUIContent m_expandedContent;

	
              

		public SA_CollapsableWindowBlockLayout(GUIContent content, Action onGUI) {
            if(content.image != null) {
                content.text = " " + content.text;
            }

			m_onGUI = onGUI;
			m_header = new SA_HyperLabel(content, SA_PluginSettingsWindowStyles.ServiceBlockHeader);
			m_header.SetMouseOverColor(SA_PluginSettingsWindowStyles.SelectedElementColor);

			var rightArrow = SA_Skin.GetGenericIcon("arrow_right.png"); 
			var arrow_down = SA_Skin.GetGenericIcon("arrow_down.png");  
			m_collapsedContent = new GUIContent(rightArrow);
			m_expandedContent = new GUIContent(arrow_down); 


			m_arrrow = new SA_HyperLabel(m_collapsedContent, SA_PluginSettingsWindowStyles.ServiceBlockHeader);
                   
        }

		protected virtual void OnAfterHeaderGUI() {
			
		}

		public void OnGUI() {
			GUILayout.Space(5);
			using (new SA_GuiBeginHorizontal()) {
				GUILayout.Space(10);

				var content = m_collapsedContent;
				if(m_showExtraFields.target) {
					content = m_expandedContent;
				}

				m_arrrow.SetContent(content);
				bool arClick = m_arrrow.Draw(GUILayout.Width(20));         
				GUILayout.Space(-5);

				float headerWidth = m_header.CalcSize().x;
				bool click = m_header.Draw(GUILayout.Width(headerWidth));
				if (click || arClick) {
                    m_showExtraFields.target = !m_showExtraFields.target;
                }

				OnAfterHeaderGUI();
            }                  
			using(new SA_GuiHorizontalSpace(10)) {
				if (EditorGUILayout.BeginFadeGroup(m_showExtraFields.faded)) {
                    GUILayout.Space(5);
					m_onGUI.Invoke();
                    GUILayout.Space(5);
                }
                EditorGUILayout.EndFadeGroup();
			}


			GUILayout.Space(5);
            EditorGUILayout.BeginVertical(SA_PluginSettingsWindowStyles.SeparationStyle);
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();

		}
        
    }
}