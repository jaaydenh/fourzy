using System;
using UnityEngine;

namespace SA.Foundation.Editor
{
	public class SA_GuiBeginArea : IDisposable
	{
		public SA_GuiBeginArea(Rect area) {
			GUILayout.BeginArea(area);
		}

		public SA_GuiBeginArea(Rect area, string content) {
			GUILayout.BeginArea(area, content);
		}

		public SA_GuiBeginArea(Rect area, string content, string style) {
			GUILayout.BeginArea(area, content, style);
		}

		public void Dispose() {
			GUILayout.EndArea();
		}
	}
}