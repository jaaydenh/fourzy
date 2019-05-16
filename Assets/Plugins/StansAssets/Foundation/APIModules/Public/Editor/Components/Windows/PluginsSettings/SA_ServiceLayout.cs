using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;


namespace SA.Foundation.Editor
{
    public abstract class SA_ServiceLayout : SA_GUILayoutElement
    {

        [SerializeField] bool m_isSelected;
        [SerializeField] SA_HyperLabel m_blockTitleLabel;
        [SerializeField] SA_HyperLabel m_blockAPIStateLabel;
        [SerializeField] SA_HyperLabel m_apiEnableButton;

        [SerializeField] SA_HyperLabel m_showMoreButton;
        [SerializeField] protected List<SA_FeatureUrl> m_features;

        private AnimBool m_ShowExtraFields;
        private bool m_SearchUIActive = false;

        [SerializeField] Texture2D m_expandOpenIcon;
        [SerializeField] Texture2D m_expandClosedIcon;


        [SerializeField] Texture2D m_onToggle;
        [SerializeField] Texture2D m_offToggle;



        //--------------------------------------
        // Abstract
        //--------------------------------------



        public abstract string Title { get; }
        public abstract string Description { get; }
        protected abstract Texture2D Icon { get; }
        public abstract SA_iAPIResolver Resolver { get; }
        protected abstract void OnServiceUI();
        protected abstract void DrawServiceRequirements();


        //--------------------------------------
        // Virtual
        //--------------------------------------

        protected virtual bool CanBeDisabled {
            get {
                return true;
            }
        }


        protected virtual IEnumerable<string> SupportedPlatforms {
            get {
                return new List<string>() { "Android", "Android TV", "Android Wear" };
            }
        }


        //--------------------------------------
        // Public Methods
        //--------------------------------------

        protected void AddFeatureUrl(string title, string url) {
            var feature = new SA_FeatureUrl(title, url);
            m_features.Add(feature);
        }

        //--------------------------------------
        // Get / Set
        //--------------------------------------

        public bool IsSelected {
            get {
                return m_isSelected;
            }
        }

        //--------------------------------------
        // SA_GUILayoutElement implementation
        //--------------------------------------

        public override void OnAwake() {
            m_blockTitleLabel = new SA_HyperLabel(new GUIContent(Title), SA_PluginSettingsWindowStyles.LabelServiceBlockStyle);
            m_blockTitleLabel.SetMouseOverColor(SA_PluginSettingsWindowStyles.SelectedElementColor);

            m_blockAPIStateLabel = new SA_HyperLabel(new GUIContent("OFF"), OffStyle);
            m_blockAPIStateLabel.SetMouseOverColor(SA_PluginSettingsWindowStyles.SelectedElementColor);


            m_expandOpenIcon = SA_Skin.GetGenericIcon("expand.png");
            m_expandClosedIcon = SA_Skin.GetGenericIcon("expand_close.png");
            m_showMoreButton = new SA_HyperLabel(new GUIContent(m_expandOpenIcon));
            m_showMoreButton.SetMouseOverColor(SA_PluginSettingsWindowStyles.SelectedElementColor);



            m_onToggle = SA_Skin.GetGenericIcon("on_toggle.png");
            m_offToggle = SA_Skin.GetGenericIcon("off_toggle.png");
            m_apiEnableButton = new SA_HyperLabel(new GUIContent(m_onToggle));
            m_apiEnableButton.SetMouseOverColor(SA_PluginSettingsWindowStyles.SelectedElementColor);
            m_apiEnableButton.GuiColorOverride(true);

            m_ShowExtraFields = new AnimBool(false);

            m_features = new List<SA_FeatureUrl>();
        }


        private  Rect m_labelRect;
        const float DESCRIPTION_LABEL_ONE_LINE_HEIGHT = 16f;
        public override void OnGUI() {
            if (m_SearchUIActive)
            {
                m_SearchUIActive = false;
                m_blockTitleLabel.DisableHighLight();
                Collapse();
            }
            CheckServiceAvailability();
            DrawBlockUI();
        }

        public void OnSearchGUI(string pattern)
        {
            m_SearchUIActive = true;
            var valid = m_blockTitleLabel.Content.text.ToLower().Contains(pattern.ToLower());
            m_blockTitleLabel.HighLight(pattern);
            foreach (var feature in m_features)
            {
                if (feature.Content.text.ToLower().Contains(pattern.ToLower()))
                {
                    valid = true;
                }
            }

            if (valid)
            {
                Expand();
                DrawBlockUI(pattern);
            }
            else
            {
                Collapse();
            }
            

        }

        public void UnSelect() {
            m_isSelected = false;
        }

        private void Expand()
        {
            m_ShowExtraFields.target = true;
        }

        private void Collapse()
        {
            m_ShowExtraFields.target = false;
        }


        private void DrawBlockUI(string pattern = null) 
        {
            GUILayout.Space(5);

            bool titleClick;
            bool toggleClick;

            using (new SA_GuiBeginHorizontal()) {
                GUILayout.Space(10);
                GUILayout.Label(Icon, SA_PluginSettingsWindowStyles.LabelServiceBlockStyle, GUILayout.Width(IconSize), GUILayout.Height(IconSize));
                GUILayout.Space(5);

                using (new SA_GuiBeginVertical()) {
                    GUILayout.Space(TitleVerticalSpace);
                    titleClick = m_blockTitleLabel.Draw(GUILayout.Height(25));
                }


                GUILayout.FlexibleSpace();
                toggleClick = DrawServiceStateInfo();
                
            }

            if (titleClick || toggleClick) {
                m_isSelected = true;
            }


            GUILayout.Space(5);
            using (new SA_GuiBeginHorizontal()) {
                GUILayout.Space(15);
                EditorGUILayout.LabelField(Description, SA_PluginSettingsWindowStyles.DescribtionLabelStyle);

                if (Event.current.type == EventType.Repaint) {

                    m_labelRect = GUILayoutUtility.GetLastRect();
                }
                GUILayout.FlexibleSpace();
                using (new SA_GuiBeginVertical()) {
                    GUILayout.Space(m_labelRect.height - DESCRIPTION_LABEL_ONE_LINE_HEIGHT);
                    var click = m_showMoreButton.Draw(GUILayout.Height(22), GUILayout.Width(22));
                    if (click) {
                        if (m_ShowExtraFields.faded.Equals(0f) || m_ShowExtraFields.faded.Equals(1f)) {
                            m_ShowExtraFields.target = !m_ShowExtraFields.target;
                            if (m_ShowExtraFields.target) {
                                m_showMoreButton.SetContent(new GUIContent(m_expandClosedIcon));
                            } else {
                                m_showMoreButton.SetContent(new GUIContent(m_expandOpenIcon));
                            }
                        }
                    }
                }

                GUILayout.Space(5);
            }



            if (EditorGUILayout.BeginFadeGroup(m_ShowExtraFields.faded)) {
                GUILayout.Space(5);
                DrawFeaturesList(pattern);
                GUILayout.Space(5);
            }
            EditorGUILayout.EndFadeGroup();


            GUILayout.Space(5);
            EditorGUILayout.BeginVertical(SA_PluginSettingsWindowStyles.SeparationStyle);
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }

      

        public void DrawHeaderUI() {

            CheckServiceAvailability();

            EditorGUILayout.BeginVertical(SA_PluginSettingsWindowStyles.SeparationStyle);
            {
                GUILayout.Space(20);
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(SA_PluginSettingsWindowStyles.INDENT_PIXEL_SIZE);
                    EditorGUILayout.LabelField(Title, SA_PluginSettingsWindowStyles.LabelHeaderStyle);

                    GUILayout.FlexibleSpace();
                    DrawServiceStateInteractive();
                    GUILayout.Space(SA_PluginSettingsWindowStyles.INDENT_PIXEL_SIZE);

                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(8);


                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(SA_PluginSettingsWindowStyles.INDENT_PIXEL_SIZE);
                    EditorGUILayout.LabelField(Description, SA_PluginSettingsWindowStyles.DescribtionLabelStyle);


                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(25);
            }
            EditorGUILayout.EndVertical();
        }

        public virtual void DrawServiceUI() {


            DrawGettingStartedBlock();

            OnServiceUI();


            DrawServiceRequirements();
            DrawSupportedPlatformsBlock();


        }

        protected virtual void DrawGettingStartedBlock() {
            using (new SA_WindowBlockWithIndent(new GUIContent("Getting Started"))) {
                GettingStartedBlock();
                GUILayout.Space(-5);
                EditorGUI.indentLevel--;
                DrawFeaturesList();
                EditorGUI.indentLevel++;
            }
        }

        private void DrawSupportedPlatformsBlock() {
            using (new SA_WindowBlockWithSpace(new GUIContent("Supported Platforms"))) {
                using (new SA_GuiBeginHorizontal()) {
                    foreach (var platform in SupportedPlatforms) {
                        GUILayout.Label(platform, SA_PluginSettingsWindowStyles.AssetLabel);
                    }
                }
            }
        }


        protected virtual void GettingStartedBlock() {}

        protected virtual int IconSize {
            get {
                return 25;
            }
        }

        protected virtual int TitleVerticalSpace {
            get {
                return 4;
            }
        }

        public List<SA_FeatureUrl> Features {
            get {
                return m_features;
            }
        }


        //--------------------------------------
        // Private Methods
        //--------------------------------------


        private void DrawFeaturesList(string pattern = null) {
            EditorGUILayout.Space();

            List<SA_FeatureUrl> m_drawableFeatures;
            if (string.IsNullOrEmpty(pattern))
            {
                m_drawableFeatures = m_features;
                foreach (var feature in m_features)
                {
                    feature.DisableHighLight();
                }
            }
            else
            {
                m_drawableFeatures = new List<SA_FeatureUrl>();
                foreach (var feature in m_features)
                {
                    if (feature.Content.text.ToLower().Contains(pattern.ToLower()))
                    {
                        feature.HighLight(pattern);
                        m_drawableFeatures.Add(feature);
                    }
                }
            }

            using (new SA_GuiIndentLevel(1)) 
            {
                for (var i = 0; i < m_drawableFeatures.Count; i += 2) 
                {
                    EditorGUILayout.BeginHorizontal();

                    m_drawableFeatures[i].DrawLink(GUILayout.Width(150));
                    if (m_drawableFeatures.Count > (i + 1)) {
                        m_drawableFeatures[i + 1].DrawLink(GUILayout.Width(150));
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
            }
        }



        protected virtual bool DrawServiceStateInfo() {
            var click =  m_blockAPIStateLabel.Draw(GUILayout.Height(25), GUILayout.Width(32));
            GUILayout.Space(5);

            return click;
        }
        protected virtual void DrawServiceStateInteractive() {
            if (CanBeDisabled) {
                var click = m_apiEnableButton.Draw(GUILayout.Width(50), GUILayout.Height(25));
                if (click) {
                    GUI.changed = true;
                    Resolver.IsSettingsEnabled = !Resolver.IsSettingsEnabled;
                }
            }
        }

        protected virtual void CheckServiceAvailability() {
            if (Resolver.IsSettingsEnabled) {
                m_blockAPIStateLabel.SetStyle(OnStyle);
                m_blockAPIStateLabel.SetContent(new GUIContent("ON"));


                m_apiEnableButton.SetContent(new GUIContent(m_onToggle));
                m_apiEnableButton.SetColor(SA_PluginSettingsWindowStyles.SelectedImageColor);
                m_apiEnableButton.SetMouseOverColor(SA_PluginSettingsWindowStyles.SelectedImageColor);


            } else {

                m_blockAPIStateLabel.SetStyle(OffStyle);
                m_blockAPIStateLabel.SetContent(new GUIContent("OFF"));


                m_apiEnableButton.SetContent(new GUIContent(m_offToggle));
                m_apiEnableButton.SetColor(SA_PluginSettingsWindowStyles.DisabledImageColor);
                m_apiEnableButton.SetMouseOverColor(SA_PluginSettingsWindowStyles.SelectedImageColor);

            }
        }


        private GUIStyle m_onStyle;
        private GUIStyle OnStyle {
            get {
                if (m_onStyle == null) {
                    m_onStyle = new GUIStyle(SA_PluginSettingsWindowStyles.DescribtionLabelStyle);
                    m_onStyle.fontSize = 14;
                    m_onStyle.fontStyle = FontStyle.Bold;
                    m_onStyle.alignment = TextAnchor.MiddleRight;
                    m_onStyle.normal.textColor = SA_PluginSettingsWindowStyles.SelectedImageColor;
                }

                return m_onStyle;
            }
        }

        private GUIStyle m_offStyle;
        private GUIStyle OffStyle {
            get {
                if (m_offStyle == null) {
                    m_offStyle = new GUIStyle(OnStyle);
                    m_offStyle.normal.textColor = SA_PluginSettingsWindowStyles.DisabledImageColor;
                }


                return m_offStyle;
            }
        }

    }
}