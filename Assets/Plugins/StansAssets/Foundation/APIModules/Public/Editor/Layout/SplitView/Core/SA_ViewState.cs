using System;
using UnityEngine;

namespace SA.Foundation.Editor
{
	[Serializable]
	public class SA_ViewState
	{
		public int MinSize = 0;
		public int MaxSize = int.MaxValue;
		public int StartSize = 100;
	}

	[Serializable]
	public class SA_SplitViewState
	{
		public Orientation Orientation;

		public bool IsFixed = false;
		public float SplitterSize = 2.0f;

		public Rect SplitterRect;

		public SA_ViewState Panel1ViewState;
		public SA_ViewState Panel2ViewState;

		public SA_SplitViewState() {
			if (Panel1ViewState == null) { Panel1ViewState = new SA_ViewState(); }
			if (Panel2ViewState == null) { Panel2ViewState = new SA_ViewState(); }
		}
	}
}
