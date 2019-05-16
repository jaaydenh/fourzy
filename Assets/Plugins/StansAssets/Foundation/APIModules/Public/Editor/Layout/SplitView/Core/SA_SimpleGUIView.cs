using UnityEditor;
using UnityEngine;

namespace SA.Foundation.Editor
{
	public sealed class SA_SimpleGUIView : SA_iGUIElement
	{
		private GUIContent m_content;

		public SA_SimpleGUIView(GUIContent content) {
			m_content = content;
		}

		public void OnGui(Rect rect, SA_InputEvent e) {
			GUI.BeginGroup(rect);

			Rect rc = new Rect(0.0f, 0.0f, rect.width, rect.height);
			GUI.Box(rc, m_content);
			GUI.EndGroup();
		}
	}
}
