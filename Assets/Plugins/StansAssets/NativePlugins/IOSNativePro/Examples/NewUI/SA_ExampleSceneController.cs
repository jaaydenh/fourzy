using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SA.iOS.Examples
{
    public class SA_ExampleSceneController : MonoBehaviour
    {
        [SerializeField]
        private SA_ExampleSceneConfig m_config = null;

        [SerializeField]
        private SA_UIController m_UIController = null;

        private List<GameObject> m_links = new List<GameObject>();

        private void Start() {
            Subscribe();
            SetLogo();
            GenerateMenuUI();
        }

        private void Subscribe() {
            m_UIController.OnTopMenuClick += HandlerTopMenuClick;

            m_UIController.OnSideMenuClick += HandlerSideMenuClick;

            Application.logMessageReceived += Application_LogMessageReceived;
        }

        private void SetLogo() {
            m_UIController.SetLogo(m_config.Logo);
        }

        private void GenerateMenuUI() {
            m_links.Clear();
            bool first = true;
            for (int counter = 0; counter < m_config.Services.Count; counter++)
            {
                GameObject tmp = m_UIController.AddSideMenuElement(m_config.Services[counter].Subsections, first);
                m_links.Add(tmp);

                m_UIController.AddTopMenuElement(m_config.Services[counter].Name, m_config.Services[counter].Icon, first, counter);

                first = false;
            }
        }


        private void HandlerTopMenuClick(int index) {
            Debug.Log("2332 \n rtrh rtr \n rthtrdfbfbf \n rthsdrbgadgaree \n");
            m_UIController.ShowTopMenuItem(m_links[index]);
        }

        private void HandlerSideMenuClick(string sceneName) {
            Debug.Log("12345");
            LoadScene(sceneName);
        }

        private void LoadScene(string sceneName)
        {
            UnloadScene();
            SA.Foundation.Async.SA_SceneManager.LoadAdditively(sceneName, SceneLoaded);
        }

        private void UnloadScene()
        {
            m_UIController.UnloadContent();
            SA.Foundation.Async.SA_SceneManager.UnloadLastScene();
        }


        private void SceneLoaded(Scene scene)
        {
            GameObject canvas = scene.GetRootGameObjects()[1];

            if (canvas != null) {
                if (canvas.GetComponent<Canvas>() != null) {
                    m_UIController.LoadContent(canvas);
                    return;
                }
            }
            Debug.Log("wrong scene hierarchy");
        }

        void Application_LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            m_UIController.AddLog(condition);
        }
    }
}
