using UnityEngine;

namespace SA.Foundation.Editor
{
	/// <summary>
	/// Creates a panel that is associated with a SplitView.
	/// </summary>
	public sealed class SA_SplitViewPanel : SA_iGUIElement
	{
		private int m_size = 0;

		private int m_minSize = 0;
		private int m_maxSize = int.MaxValue;
		private int m_startSize = 100;

		private SA_ViewState m_state;

		public SA_SplitViewPanel(SA_ViewState state) {
			m_state = state;

			m_minSize = m_state.MinSize;
			m_maxSize = m_state.MaxSize;
			m_startSize = m_state.StartSize;
		}

		public SA_SplitViewPanel() {
		}

		public void OnGui(Rect rect, SA_InputEvent e) {
			m_size = (int)rect.width;

			if (View == null) {
				//Nothing to render, just return
				return;
			}

			GUI.BeginGroup(rect);
			Rect rc = new Rect(0.0f, 0.0f, rect.width, rect.height);
			View.OnGui(rc, e);
			GUI.EndGroup();
		}

		/// <summary>
		/// Sets the view contained within the SplitViewPanel
		/// </summary>
		/// <param name="view">The view contained within the SplitViewPanel</param>
		public void SetView(SA_iGUIElement view) {
			View = view;
		}

		/// <summary>
		/// Gets or sets the minimum width or height of the panel in pixels depending on the SplitView Orientation.
		/// The default value is 0 pixels, regardless of SplitView Orientation.
		/// </summary>
		public int MinSize {
			get {
				return m_minSize;
			}
			set {
				m_minSize = value;
				m_state.MinSize = m_minSize;
			}
		}

		/// <summary>
		/// Gets or sets the maximum width or height of the panel in pixels depending on the SplitView Orientation.
		/// The default value is int.MaxValue pixels, regardless of SplitView Orientation.
		/// </summary>
		public int MaxSize {
			get {
				return m_maxSize;
			}
			set {
				m_maxSize = value;
				m_state.MaxSize = m_maxSize;
			}
		}

		/// <summary>
		/// Gets the view contained within the SplitViewPanel
		/// </summary>
		public SA_iGUIElement View { get; private set; }

		/// <summary>
		/// Gets current width or height of a panel depending on the SplitView Orientation.
		/// </summary>
		public int Size {
			get {
				return m_size;
			}
		}

		/// <summary>
		/// Gets or sets the start width or height of the panel depending on the SplitView Orientation.
		/// <para>Important: Make sure that this value is between MinSize and MaxSize of the panel.</para>
		/// </summary>
		public int StartSize {
			get {
				return m_startSize;
			}
			set {
				m_startSize = value;
				m_state.StartSize = m_startSize;
			}
		}
	}
}
