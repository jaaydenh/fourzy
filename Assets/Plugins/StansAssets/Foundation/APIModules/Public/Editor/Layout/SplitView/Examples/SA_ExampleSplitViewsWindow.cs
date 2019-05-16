using UnityEditor;
using UnityEngine;


namespace SA.Foundation.Editor
{
	public class SA_ExampleSplitViewsWindow : EditorWindow
	{
		[SerializeField]
		private SA_SplitViewState m_rootViewState;
		[SerializeField]
		private SA_SplitViewState m_splitView1State;
		[SerializeField]
		private SA_SplitViewState m_splitView2State;
		[SerializeField]
		private SA_SplitViewState m_splitView3State;

		private SA_SplitView m_rootView;
		private Rect m_rootViewRect = new Rect(Vector2.zero, Vector2.zero);

		[MenuItem("Window/Example Split Views")]
		private static void ShowWindow() {
			SA_ExampleSplitViewsWindow window = GetWindow<SA_ExampleSplitViewsWindow>();
			window.titleContent = new GUIContent("Example Views");
			window.minSize = new Vector2(900.0f, 700.0f);
			window.Show();
		}

		private void OnEnable() {
			if (m_rootViewState == null) {
				m_rootViewState = new SA_SplitViewState();
			}

			m_rootView = new SA_SplitView(m_rootViewState);
			m_rootView.Orientation = Orientation.Horizontal;
			m_rootView.SplitterSize = 1.0f;
			m_rootView.IsFixed = true;
			m_rootView.Panel1.MinSize = 250;
			m_rootView.Panel1.StartSize = 300;
			m_rootView.Panel1.SetView(new SA_SimpleGUIView(new GUIContent("view 1")));

			if (m_splitView1State == null) {
				m_splitView1State = new SA_SplitViewState();
			}
			SA_SplitView splitView1 = new SA_SplitView(m_splitView1State);
			splitView1.Orientation = Orientation.Vertical;
			splitView1.SplitterSize = 1.0f;
			splitView1.Panel1.MinSize = 150;
			splitView1.Panel1.StartSize = 200;
			splitView1.Panel1.SetView(new SA_SimpleGUIView(new GUIContent("view 3")));

			if (m_splitView3State == null) {
				m_splitView3State = new SA_SplitViewState();
			}
			SA_SplitView splitView3 = new SA_SplitView(m_splitView3State);
			splitView3.Orientation = Orientation.Vertical;
			splitView3.SplitterSize = 1.0f;
			splitView3.Panel1.MinSize = 150;
			splitView3.Panel1.StartSize = 250;
			splitView3.Panel1.SetView(new SA_SimpleGUIView(new GUIContent("view 7")));
			splitView3.Panel2.MinSize = 150;
			splitView3.Panel2.SetView(new SA_SimpleGUIView(new GUIContent("view 8")));

			if (m_splitView2State == null) {
				m_splitView2State = new SA_SplitViewState();
			}
			SA_SplitView splitView2 = new SA_SplitView(m_splitView2State);
			splitView2.Orientation = Orientation.Horizontal;
			splitView2.SplitterSize = 1.0f;
			splitView2.Panel1.MinSize = 150;
			splitView2.Panel1.StartSize = 300;
			splitView2.Panel1.SetView(splitView3);

			splitView2.Panel2.MinSize = 150;
			splitView2.Panel2.SetView(new SA_SimpleGUIView(new GUIContent("view 6")));

			splitView1.Panel2.MinSize = 300;
			splitView1.Panel2.SetView(splitView2);

			m_rootView.Panel2.MinSize = 300;
			m_rootView.Panel2.SetView(splitView1);

			EditorApplication.update += OnEditorUpdate;
		}

		private void OnDisable() {
			EditorApplication.update -= OnEditorUpdate;
		}

		private void OnEditorUpdate() {
			Repaint();
		}

		private void OnGUI() {
			m_rootViewRect.width = this.position.width;
			m_rootViewRect.height = this.position.height;

			BeginWindows();

			SA_InputEvent e = new SA_InputEvent(Event.current);
			m_rootView.OnGui(m_rootViewRect, e);

			EndWindows();
		}
	}
}