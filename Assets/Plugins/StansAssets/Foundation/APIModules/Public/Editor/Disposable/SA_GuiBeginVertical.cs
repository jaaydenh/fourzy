using UnityEngine;
using UnityEditor;
using System;

namespace SA.Foundation.Editor
{
	public class SA_GuiBeginVertical : IDisposable
	{
		public SA_GuiBeginVertical(params GUILayoutOption[] layoutOptions) {
			EditorGUILayout.BeginVertical(layoutOptions);
		}

        public SA_GuiBeginVertical(GUIStyle style,  params GUILayoutOption[] layoutOptions) {
            EditorGUILayout.BeginVertical(style, layoutOptions);
        }

        public void Dispose() {
			EditorGUILayout.EndVertical();
		}
	}
}