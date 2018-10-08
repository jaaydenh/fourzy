using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Fourzy
{
    public enum TotalView
    {
        view1 = 0,
        view2 = 1,
        view3 = 2,
        viewSocial = 3,
        viewRanking = 4,
        viewTraining,
        viewGameboardSelection,
        viewMatchMaking,
        viewSettings,
        viewAreaSelect,
        viewPuzzleSelection
    };

    public class ViewController : MonoBehaviour
    {
        //Instance
        public static ViewController instance;

        public GameObject headerUI;

        //List of Views
        public List<UIView> TabsViewList = new List<UIView>();

        //Views
        public UIView view1;
        public UIView view2;
        public UIView view3;
        public UIView viewSocial;
        public UIView viewRanking;
        public UIView viewTraining;
        public UIView viewGameboardSelection;
        public UIView viewMatchMaking;
        public UIView viewSettings;
        public UIView viewAreaSelect;
        public UIView viewPuzzleSelection;
        public UIView viewCommon;
        public UIView debugView;

        //Current View
        private UIView currentView;
        private UIView previousView;
        public TotalView currentActiveView = TotalView.view3;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            view1.Hide();
            view2.Hide();
            view3.Hide();
            viewSocial.Hide();
            viewRanking.Hide();
            viewTraining.Hide();
            viewGameboardSelection.Hide();
            viewMatchMaking.Hide();
            viewSettings.Hide();
            viewAreaSelect.Hide();
            viewPuzzleSelection.Hide();
            view3.keepHistory = true;

            if (debugView != null)
                ChangeView(debugView);
            else
                ChangeView(view3);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += SceneManager_SceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneManager_SceneLoaded;
        }

        void SceneManager_SceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if (scene.name == "gamePlay")
            {
                headerUI.SetActive(false);
            }
        }

        public void WhatIsCurrentView() {
            Debug.Log("CurrentView: " + currentView.ToString());
        }

        /// <summary>
        /// Changes the view.
        /// </summary>
        /// <param name="targetView">Target view.</param>
        public void ChangeView(UIView targetView)
        {
            //Debug.Log("targetView: " + targetView.name);

            if (currentView != null) {
                //Debug.Log("CurrentView: " + currentView.name);
                currentView.Hide();
            } else if (previousView != null) {
                //Debug.Log("PreviousView: " + previousView.name);
                previousView.Hide();
            }
            //Debug.Log("ChangeView keep history: " + targetView.keepHistory.ToString());
            if (targetView.keepHistory) {
                previousView = currentView;
                currentView = targetView;
                currentView.Show();
            } else {
                targetView.Show();
            }
        }

        public void LoadSingleView(UIView targetView) {
            if (currentView != null)
            {
                currentView.Hide();
            }
            currentView = targetView;
            currentView.Show();
        }

        public UIView GetPreviousView() {
            return previousView;
        }

        public UIView GetCurrentView()
        {
            return currentView;
        }

        public void SetActiveView(TotalView view) {
            switch (view)
            {
                case TotalView.view1:
                    currentActiveView = TotalView.view1;
                    previousView = currentView;
                    currentView = view1;
                    break;
                case TotalView.view2:
                    currentActiveView = TotalView.view2;
                    previousView = currentView;
                    currentView = view2;
                    break;
                case TotalView.view3:
                    currentActiveView = TotalView.view3;
                    previousView = currentView;
                    currentView = view3;
                    break;
                case TotalView.viewSocial:
                    currentActiveView = TotalView.viewSocial;
                    previousView = currentView;
                    currentView = viewSocial;
                    break;
                case TotalView.viewRanking:
                    currentActiveView = TotalView.viewRanking;
                    previousView = currentView;
                    currentView = viewRanking;
                    break;
                case TotalView.viewGameboardSelection:
                    currentActiveView = TotalView.viewGameboardSelection;
                    previousView = currentView;
                    currentView = viewGameboardSelection;
                    break;
                case TotalView.viewTraining:
                    currentActiveView = TotalView.viewTraining;
                    previousView = currentView;
                    currentView = viewTraining;
                    break;
                case TotalView.viewMatchMaking:
                    currentActiveView = TotalView.viewMatchMaking;
                    previousView = currentView;
                    currentView = viewMatchMaking;
                    break;
                case TotalView.viewSettings:
                    currentActiveView = TotalView.viewSettings;
                    previousView = currentView;
                    currentView = viewSettings;
                    break;
                case TotalView.viewAreaSelect:
                    currentActiveView = TotalView.viewAreaSelect;
                    previousView = currentView;
                    currentView = viewAreaSelect;
                    break;
                case TotalView.viewPuzzleSelection:
                    currentActiveView = TotalView.viewPuzzleSelection;
                    previousView = currentView;
                    currentView = viewPuzzleSelection;
                    break;
                default:
                    break;
            }
        }

        public void HideTabView() {
            viewCommon.Hide();
        }

        public void ShowTabView() {
            viewCommon.Show();
        }
    }
}
