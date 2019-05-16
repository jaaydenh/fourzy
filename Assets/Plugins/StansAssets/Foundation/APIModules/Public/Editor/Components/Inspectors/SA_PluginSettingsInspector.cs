using UnityEditor;
using UnityEngine;

using SA.Foundation.Patterns;

namespace SA.Foundation.Editor
{

    [CustomEditor(typeof(SA_ScriptableSettings), true)]
    public class SA_PluginSettingsInspector : UnityEditor.Editor
    {

        private const string DESCRIBTION_TEXT = "This ScriptableObject hold's plugin setting. " +
                "You may use it to backup the settings or to transfer it into the new project. " +
                "It’s not recommended to modify the settings via Default settings Inspector menu. " +
                "Use plugin editor window instead. ";


        private SA_PluginActiveTextLink m_aboutScriptableObjects;
        private SA_PluginActiveTextLink m_pluginSettings;
        private SA_PluginActiveTextLink m_documentation;


      
        protected virtual void OnEnable() {
            m_aboutScriptableObjects = new SA_PluginActiveTextLink("About ScriptableObject");
            m_pluginSettings = new SA_PluginActiveTextLink("Plugin Settings");
            m_documentation = new SA_PluginActiveTextLink("Documentation");
        }

        public override void OnInspectorGUI() {
            Repaint();
            HeaderBlock();
            InfoBlock();
        }


    
        private void InfoBlock() {
            using (new SA_WindowBlockWithSpace(new GUIContent("Where to go from here?"))) {
                using (new SA_GuiBeginHorizontal()) {
                    GUILayout.Space(5);
                    bool click;
                    click = m_pluginSettings.DrawWithCalcSize();
                    if (click) {
                        EditorApplication.ExecuteMenuItem(TargetSettings.SettingsUIMenuItem);
                    }

                    click = m_aboutScriptableObjects.DrawWithCalcSize();
                    if (click) {
                        Application.OpenURL("https://docs.unity3d.com/ScriptReference/ScriptableObject.html");
                    }

                }

                using (new SA_GuiBeginHorizontal()) {
                    GUILayout.Space(5);
                    bool click = m_documentation.DrawWithCalcSize();
                    if (click) {
                        Application.OpenURL(TargetSettings.DocumentationURL);
                    }
                }
            }

            using (new SA_WindowBlockWithSpace(new GUIContent("Default Settings Inspector"))) {
                DrawDefaultInspector();
            }
        }


        private void HeaderBlock() {

            EditorGUILayout.BeginVertical(SA_PluginSettingsWindowStyles.SeparationStyle);
            {
                GUILayout.Space(20);
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(SA_PluginSettingsWindowStyles.INDENT_PIXEL_SIZE);
                    EditorGUILayout.LabelField(TargetSettings.PluginName + " Settings", SA_PluginSettingsWindowStyles.LabelHeaderStyle);
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(8);


                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(SA_PluginSettingsWindowStyles.INDENT_PIXEL_SIZE);
                    EditorGUILayout.LabelField(DESCRIBTION_TEXT, SA_PluginSettingsWindowStyles.DescribtionLabelStyle);
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(2);
                using (new SA_GuiBeginHorizontal()) {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.SelectableLabel("v: " + TargetSettings.GetFormattedVersion(), SA_PluginSettingsWindowStyles.VersionLabelStyle, GUILayout.Width(120));
                    GUILayout.Space(10);
                }

                GUILayout.Space(5);
            }
            EditorGUILayout.EndVertical();

        }



        public SA_ScriptableSettings TargetSettings {
            get {
                return (SA_ScriptableSettings)target;
            }
        }

    }
}