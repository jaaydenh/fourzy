using System;
using Unity.Cloud.BugReporting;
using Unity.Cloud.BugReporting.Plugin;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.BugReporting
{
    public class BugReportingWindow : EditorWindow
    {
        #region Static Methods

        [MenuItem("In-Game Bug Reporting/Bug Report Form")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(BugReportingWindow));
        }

        #endregion

        #region Constructors

        public BugReportingWindow()
        {
            this.titleContent = new GUIContent("Bug Report");
            this.summary = string.Empty;
            this.description = string.Empty;
        }

        #endregion

        #region Fields

        private BugReport bugReport;

        private bool creating;

        private string description;

        private bool saving;

        private bool submitting;

        private string summary;

        private Texture thumbnailTexture;

        #endregion

        #region Methods

        private void Create()
        {
            this.creating = false;
            if (Application.isPlaying)
            {
                if (UnityBugReporting.CurrentClient != null)
                {
                    UnityBugReporting.CurrentClient.TakeScreenshot(2048, 2048, s => { });
                    UnityBugReporting.CurrentClient.TakeScreenshot(512, 512, s => { });
                    UnityBugReporting.CurrentClient.CreateBugReport((br) =>
                    {
                        this.SetThumbnail(br);
                        this.summary = string.Empty;
                        this.description = string.Empty;
                        this.bugReport = br;
                    });
                }
                else
                {
                    EditorUtility.DisplayDialog("In-Game Bug Reporting", "In-Game Bug Reporting is not configured. Call UnityBugReporting.Configure() to configure In-Game Bug Reporting or add the BugReportingPrefab to your scene.", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("In-Game Bug Reporting", "In-game bug reports can only be sent in play mode.", "OK");
            }
        }

        private void CreatePropertyField(string propertyName)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty stringsProperty = serializedObject.FindProperty(propertyName);
            EditorGUILayout.PropertyField(stringsProperty, true);
            serializedObject.ApplyModifiedProperties();
        }

        private void OnGUI()
        {
            float spacing = 16;
            if (Application.isPlaying)
            {
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.margin = new RectOffset(8, 8, 4, 4);
                buttonStyle.padding = new RectOffset(8, 8, 4, 4);
                GUIStyle textAreaStyle = new GUIStyle(GUI.skin.textArea);
                textAreaStyle.wordWrap = true;
                GUILayout.Space(spacing);
                this.creating = GUILayout.Button("Create Bug Report", buttonStyle, GUILayout.ExpandWidth(false));
                if (this.bugReport != null)
                {
                    GUILayout.Space(spacing);
                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    float imageHeight = Mathf.Min(this.position.width, 256);
                    EditorGUI.DrawPreviewTexture(new Rect(0, lastRect.yMax, this.position.width, imageHeight), this.thumbnailTexture, null, ScaleMode.ScaleToFit);
                    GUILayout.Space(imageHeight);
                    GUILayout.Space(spacing);
                    GUILayout.Label("Summary");
                    this.summary = EditorGUILayout.TextField(this.summary, textAreaStyle, GUILayout.MinHeight(32));
                    GUILayout.Label("Description");
                    this.description = EditorGUILayout.TextArea(this.description, textAreaStyle, GUILayout.MinHeight(128));
                    GUILayout.Space(spacing);
                    EditorGUILayout.BeginHorizontal();
                    this.submitting = GUILayout.Button("Submit Bug Report", buttonStyle, GUILayout.ExpandWidth(false));
                    this.saving = GUILayout.Button("Save to Disk", buttonStyle, GUILayout.ExpandWidth(false));
                    GUILayout.Space(spacing);
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.alignment = TextAnchor.UpperCenter;
                labelStyle.margin = new RectOffset(8, 8, 8, 8);
                labelStyle.wordWrap = true;
                GUILayout.Space(spacing);
                EditorGUILayout.HelpBox("In-game bug reports can only be sent in play mode.", MessageType.Info);
                GUILayout.Space(spacing);
            }
        }

        public void OnInspectorUpdate()
        {
            this.Repaint();
        }

        private void Save()
        {
            this.saving = false;
            if (Application.isPlaying)
            {
                if (UnityBugReporting.CurrentClient != null)
                {
                    this.bugReport.Summary = this.summary;
                    this.bugReport.Fields.Add(new BugReportNamedValue("Description", this.description));

                    // Save
                    UnityBugReporting.CurrentClient.SaveBugReportToDisk(this.bugReport);
                    EditorUtility.DisplayDialog("In-Game Bug Reporting", "In-game bug report saved to disk as BugReport.json.", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("In-Game Bug Reporting", "In-Game Bug Reporting is not configured. Call UnityBugReporting.Configure() to configure In-Game Bug Reporting or add the BugReportingPrefab to your scene.", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("In-Game Bug Reporting", "In-game bug reports can only be sent in play mode.", "OK");
            }
        }

        private void SetThumbnail(BugReport bugReport)
        {
            if (bugReport != null)
            {
                byte[] data = Convert.FromBase64String(bugReport.Thumbnail.DataBase64);
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(data);
                this.thumbnailTexture = texture;
            }
        }

        private void Submit()
        {
            this.submitting = false;
            if (Application.isPlaying)
            {
                if (UnityBugReporting.CurrentClient != null)
                {
                    this.bugReport.Summary = this.summary;
                    this.bugReport.Fields.Add(new BugReportNamedValue("Description", this.description));
                    this.bugReport.Dimensions.Add(new BugReportNamedValue("FromEditor", "True"));

                    // Send
                    UnityBugReporting.CurrentClient.SendBugReport(this.bugReport, (success, br2) =>
                    {
                        this.bugReport = null;
                        if (EditorUtility.DisplayDialog("In-Game Bug Reporting", "In-game bug report submitted. Would you like to view it?", "View", "Don't View"))
                        {
                            string url = string.Format("https://andy-developer.cloud.unity3d.com/bugreporting/direct/projects/{0}/reports/{1}", br2.ProjectIdentifier, br2.Identifier);
                            Application.OpenURL(url);
                        }
                    });
                }
                else
                {
                    EditorUtility.DisplayDialog("In-Game Bug Reporting", "In-Game Bug Reporting is not configured. Call UnityBugReporting.Configure() to configure In-Game Bug Reporting or add the BugReportingPrefab to your scene.", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("In-Game Bug Reporting", "In-game bug reports can only be sent in play mode.", "OK");
            }
        }

        private void Update()
        {
            if (this.creating)
            {
                this.Create();
            }
            if (this.submitting)
            {
                this.Submit();
            }
            if (this.saving)
            {
                this.Save();
            }
        }

        #endregion
    }
}