using UnityEngine;
using UnityEditor;
using System;

namespace SA.Foundation.Editor
{
	public class SA_GuiBeginScrollView : IDisposable
	{
        public Vector2 Scroll { get; set; }

        public SA_GuiBeginScrollView(ref Vector2 scrollPosition) {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        }


        public SA_GuiBeginScrollView(ref Vector2 scrollPosition, params GUILayoutOption[] options) {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, options);
        }

        public SA_GuiBeginScrollView(Vector2 scrollPosition, GUIStyle style) {
            Scroll = GUILayout.BeginScrollView(scrollPosition, style);
        }

        public SA_GuiBeginScrollView(Vector2 scrollPosition, GUIStyle style, params GUILayoutOption[] options) {
            Scroll = GUILayout.BeginScrollView(scrollPosition, style, options);
        }

        public SA_GuiBeginScrollView(Vector2 scrollPosition, GUIStyle horizontalScrollBar, GUIStyle verticalScrollBar) {
            Scroll = GUILayout.BeginScrollView(scrollPosition, horizontalScrollBar, verticalScrollBar);
        }

        public SA_GuiBeginScrollView(Vector2 scrollPosition, bool alwaysShowHorizontalScrollBar,
            bool alwaysShowVerticalScrollBar, GUIStyle horizontalScrollBar, GUIStyle verticalScrollBar) {
            Scroll = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontalScrollBar, alwaysShowVerticalScrollBar,
                horizontalScrollBar, verticalScrollBar);
        }

        public SA_GuiBeginScrollView(Vector2 scrollPosition, bool alwaysShowHorizontalScrollBar,
            bool alwaysShowVerticalScrollBar) {
            Scroll = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontalScrollBar, alwaysShowVerticalScrollBar);
        }

        public SA_GuiBeginScrollView(Vector2 scrollPosition, GUIStyle horizontalScrollBar, GUIStyle verticalScrollBar,
            params GUILayoutOption[] options) {
            Scroll = GUILayout.BeginScrollView(scrollPosition, horizontalScrollBar, verticalScrollBar, options);
        }

        public void Dispose() {
            GUILayout.EndScrollView();
        }
    }
}