using UnityEngine;
using UnityEditor;

namespace SA.Foundation.Editor
{
	public interface SA_iGUIElement
	{
		void OnGui(Rect rect, SA_InputEvent e);
	}
}