using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SA.Foundation.Editor
{
    /// <summary>
    /// Specifies the orientation of controls or elements of controls.
    /// </summary>
    public enum Orientation
    {
        /// <summary>
        /// The control or element is oriented horizontally.
        /// </summary>
        Horizontal,

        /// <summary>
        /// The control or element is oriented vertically.
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Represents a control consisting of a movable bar (splitter) that divides a container's display area into two resizable panels.
    /// </summary>
    public class SA_SplitView : SA_iGUIElement
    {
        private Orientation m_orientation = Orientation.Horizontal;

        protected SA_SplitViewState m_state;

        protected SA_SplitViewPanel m_panel1;
        protected SA_SplitViewPanel m_panel2;

        protected Rect m_panel1Rect = new Rect();
        protected Rect m_panel2Rect = new Rect();

        protected float m_splitterSize = 2.0f;

        protected Rect m_splitterRenderRect = new Rect();
        protected Rect m_splitterActiveRect = new Rect();

        protected bool b_isFixed = false;
        protected bool b_dragStarted = false;
        protected bool b_isInitialized = false;

        protected Rect m_oldRect = new Rect();

        protected const float SPLITTER_ACTIVE_SIZE = 3.0f;

        public SA_SplitView(SA_SplitViewState state) {
            m_state = state;

            m_orientation = m_state.Orientation;
            m_splitterSize = m_state.SplitterSize;
            b_isFixed = m_state.IsFixed;

            m_panel1 = new SA_SplitViewPanel(m_state.Panel1ViewState);
            m_panel2 = new SA_SplitViewPanel(m_state.Panel2ViewState);
        }

        public void OnGui(Rect rect, SA_InputEvent e) {
            if (!b_isInitialized) {
                if (m_state.SplitterRect.Equals(new Rect())) {
                    if (m_orientation == Orientation.Horizontal) {
                        m_splitterRenderRect.xMin = ClampHorizontalSplitter(m_panel1.StartSize, rect);
                        m_splitterRenderRect.yMin = rect.yMin;
                    } else {
                        m_splitterRenderRect.xMin = rect.xMin;
                        m_splitterRenderRect.yMin = ClampVerticalSplitter(m_panel1.StartSize, rect);
                    }
                } else {
                    m_splitterRenderRect = m_state.SplitterRect;
                    if (m_orientation == Orientation.Horizontal) {
                        m_splitterRenderRect.xMin = ClampHorizontalSplitter(m_splitterRenderRect.xMin, rect);
                        m_splitterRenderRect.yMin = rect.yMin;
                    } else {
                        m_splitterRenderRect.xMin = rect.xMin;
                        m_splitterRenderRect.yMin = ClampVerticalSplitter(m_splitterRenderRect.yMin, rect);
                    }
                }

                m_oldRect = rect;
                b_isInitialized = true;
            }

            if (m_oldRect.width != rect.width || m_oldRect.height != rect.height) {
                if (m_orientation == Orientation.Horizontal) {
                    m_splitterRenderRect.xMin = ClampHorizontalSplitter(m_splitterRenderRect.xMin, rect);
                } else {
                    m_splitterRenderRect.yMin = ClampVerticalSplitter(m_splitterRenderRect.yMin, rect);
                }

                m_oldRect = rect;
            }

            if (m_orientation == Orientation.Horizontal) {
                m_splitterActiveRect.xMin = m_splitterRenderRect.xMin - SPLITTER_ACTIVE_SIZE;
                m_splitterActiveRect.xMax = m_splitterRenderRect.xMax + SPLITTER_ACTIVE_SIZE;
                m_splitterActiveRect.yMin = m_splitterRenderRect.yMin;
                m_splitterActiveRect.yMax = m_splitterRenderRect.yMax;
            } else {
                m_splitterActiveRect.xMin = m_splitterRenderRect.xMin;
                m_splitterActiveRect.xMax = m_splitterRenderRect.xMax;
                m_splitterActiveRect.yMin = m_splitterRenderRect.yMin - SPLITTER_ACTIVE_SIZE;
                m_splitterActiveRect.yMax = m_splitterRenderRect.yMax + SPLITTER_ACTIVE_SIZE;
            }

            //Set mouse cursor for splitter rect according to Split View orientation
            if (!b_isFixed && m_splitterActiveRect.Contains(e.mousePosition)) {
                EditorGUIUtility.AddCursorRect(m_splitterActiveRect,
                    m_orientation == Orientation.Horizontal ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical);
            }

            if (!b_isFixed && e.type == EventType.MouseDown && e.button == 0 && m_splitterActiveRect.Contains(e.mousePosition) && EditorGUIUtility.hotControl == 0) {
                EditorGUIUtility.hotControl = GetHashCode();
                b_dragStarted = true;
            }

            if (!b_isFixed && e.type == EventType.MouseUp && e.button == 0 && EditorGUIUtility.hotControl == GetHashCode()) {
                EditorGUIUtility.hotControl = 0;
                b_dragStarted = false;
            }

            if (m_orientation == Orientation.Horizontal) {
                if (b_dragStarted && e.type == EventType.MouseDrag && EditorGUIUtility.hotControl == GetHashCode()) {
                    m_splitterRenderRect.xMin = ClampHorizontalSplitter(e.mousePosition.x, rect);
                }

                m_splitterRenderRect.xMax = m_splitterRenderRect.xMin + m_splitterSize;
                m_splitterRenderRect.yMax = rect.yMax;

                m_panel1Rect.xMin = (int)rect.xMin;
                m_panel1Rect.xMax = (int)m_splitterRenderRect.xMin;
                m_panel1Rect.yMin = (int)rect.yMin;
                m_panel1Rect.yMax = (int)rect.yMax;

                m_panel2Rect.xMin = (int)m_splitterRenderRect.xMax;
                m_panel2Rect.xMax = (int)rect.xMax;
                m_panel2Rect.yMin = (int)rect.yMin;
                m_panel2Rect.yMax = (int)rect.yMax;
            } else {
                if (b_dragStarted && e.type == EventType.MouseDrag && EditorGUIUtility.hotControl == GetHashCode()) {
                    m_splitterRenderRect.yMin = ClampVerticalSplitter(e.mousePosition.y, rect);
                }

                m_splitterRenderRect.yMax = m_splitterRenderRect.yMin + m_splitterSize;
                m_splitterRenderRect.xMax = rect.xMax;

                m_panel1Rect.xMin = (int)rect.xMin;
                m_panel1Rect.xMax = (int)rect.xMax;
                m_panel1Rect.yMin = (int)rect.yMin;
                m_panel1Rect.yMax = (int)m_splitterRenderRect.yMin;

                m_panel2Rect.xMin = (int)rect.xMin;
                m_panel2Rect.xMax = (int)rect.xMax;
                m_panel2Rect.yMin = (int)m_splitterRenderRect.yMax;
                m_panel2Rect.yMax = (int)rect.yMax;
            }

            m_panel1.OnGui(m_panel1Rect, e);
            DrawSplitter(m_splitterRenderRect);
            m_panel2.OnGui(m_panel2Rect, e);

            m_state.SplitterRect = m_splitterRenderRect;
        }

        protected float ClampVerticalSplitter(float verticalPos, Rect viewRect) {
            float panel1VertizalMin = viewRect.yMin + m_panel1.MinSize - m_splitterSize / 2.0f;
            float panel2VerticalMin = viewRect.yMax - m_panel2.MaxSize + m_splitterSize / 2.0f;

            float panel1VertizalMax = viewRect.yMin + m_panel1.MaxSize - m_splitterSize / 2.0f;
            float panel2VerticalMax = viewRect.yMax - m_panel2.MinSize + m_splitterSize / 2.0f;

            float verticalMin = panel1VertizalMin >= panel2VerticalMin ? panel1VertizalMin : panel2VerticalMin;
            float verticalMax = panel1VertizalMax <= panel2VerticalMax ? panel1VertizalMax : panel2VerticalMax;

            if (verticalMin > verticalMax) {
                SwapFloats(ref verticalMin, ref verticalMax);
            }

            return Mathf.Clamp(verticalPos, verticalMin, verticalMax);
        }

        protected float ClampHorizontalSplitter(float horizontalPos, Rect viewRect) {
            float panel1HorizontalMin = viewRect.xMin + m_panel1.MinSize - m_splitterSize / 2.0f;
            float panel2HorizontalMin = viewRect.xMax - m_panel2.MaxSize + m_splitterSize / 2.0f;

            float panel1HorizontalMax = viewRect.xMin + m_panel1.MaxSize - m_splitterSize / 2.0f;
            float panel2HorizontalMax = viewRect.xMax - m_panel2.MinSize + m_splitterSize / 2.0f;

            float horizontalMin = panel1HorizontalMin >= panel2HorizontalMin ? panel1HorizontalMin : panel2HorizontalMin;
            float horizontalMax = panel1HorizontalMax <= panel2HorizontalMax ? panel1HorizontalMax : panel2HorizontalMax;

            if (horizontalMin > horizontalMax) {
                SwapFloats(ref horizontalMin, ref horizontalMax);
            }

            return Mathf.Clamp(horizontalPos, horizontalMin, horizontalMax);
        }

        protected void SwapFloats(ref float value1, ref float value2) {
            float temp = value1;
            value1 = value2;
            value2 = temp;
        }

        protected void DrawSplitter(Rect dragRect) {
            if (Event.current.type == EventType.Repaint) {
                Color color = GUI.color;
                Color b = (!EditorGUIUtility.isProSkin) ? new Color(0.6f, 0.6f, 0.6f, 1.333f) : new Color(0.12f, 0.12f, 0.12f, 1.333f);
                GUI.color *= b;

                //Have to cast rectangle fields to int32 to make  pixel perfect
                Rect integerRect = new Rect((int)dragRect.x,
                    (int)dragRect.y,
                    (int)dragRect.width,
                    (int)dragRect.height);

                GUI.DrawTexture(integerRect, EditorGUIUtility.whiteTexture);
                GUI.color = color;
            }
        }

        /// <summary>
        /// Gets or sets the size of a splitter in pixels. The default value is 2.0f
        /// </summary>
        public float SplitterSize {
            get {
                return m_splitterSize;
            }
            set {
                m_splitterSize = value;
                m_state.SplitterSize = m_splitterSize;
            }
        }

        /// <summary>
        /// Gets the left or top panel of the SplitView, depending on Orientation.
        /// </summary>
        public SA_SplitViewPanel Panel1 {
            get {
                return m_panel1;
            }
        }

        /// <summary>
        /// Gets the right or bottom panel of the SplitView, depending on Orientation.
        /// </summary>
        public SA_SplitViewPanel Panel2 {
            get {
                return m_panel2;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the splitter is fixed or movable.
        /// The default value is false.
        /// </summary>
        public bool IsFixed {
            get {
                return b_isFixed;
            }
            set {
                b_isFixed = value;
                m_state.IsFixed = b_isFixed;
            }
        }

        /// <summary>
        /// Gets a value indicating the horizontal or vertical orientation of the SplitView panels.
        /// </summary>
        public Orientation Orientation {
            get {
                return m_orientation;
            }
            set {
                m_orientation = value;
                m_state.Orientation = m_orientation;
            }
        }
    }
}
