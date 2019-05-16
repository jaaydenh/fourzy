using System;
using UnityEditor;
using UnityEngine.Networking;


namespace SA.Foundation.Editor
{

    public class SA_EditorWebRequest
    {

        private Action<UnityWebRequest> m_onComplete = null;
        private readonly UnityWebRequest m_request = null;

        private bool m_showProgress = false;
        private string m_progressDialogTitle = string.Empty;


        public SA_EditorWebRequest(string url) {
            m_request = UnityWebRequest.Get(url);
        }

        public SA_EditorWebRequest(UnityWebRequest request) {
            m_request = request;
        }

        public void SetProgressDialog(string title) {
            m_showProgress = true;
            m_progressDialogTitle = title;
        } 


        public void Send(Action<UnityWebRequest> callback) {
            m_onComplete = callback;
#if UNITY_EDITOR
            EditorApplication.update += OnEditorUpdate;
#endif


#if UNITY_2017_2_OR_NEWER
            m_request.SendWebRequest();
#else
            m_request.Send();
#endif

        }

        public UnityWebRequest UnityRequest {
            get {
                return m_request;
            }
        }

        public string DataAsText {
            get {
                return m_request.downloadHandler.text;
            }
        }


        private void OnEditorUpdate() {

            if (m_showProgress) {
                string progress = String.Format("Download Progress: {0}%", Convert.ToInt32(m_request.downloadProgress * 100f));
                EditorUtility.DisplayProgressBar(m_progressDialogTitle, progress, m_request.downloadProgress);
            }


            if (m_request.isDone) {
                EditorUtility.ClearProgressBar();
                EditorApplication.update -= OnEditorUpdate;
                m_onComplete.Invoke(m_request);
            }
        }
    }
}