using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class GamesListManager : MonoBehaviour {

        public static GamesListManager instance;

        public delegate void ShowDeleteGame(bool show);
        public static event ShowDeleteGame OnShowDeleteGame;

        private bool deleteButtonsVisible = false;

        public GameObject editButton;
        Text editButtonText;

    	void Start () {
            instance = this;
            editButtonText = editButton.GetComponentInChildren<Text>();
    	}
            
        public void ShowDeleteButtons() {
            if (OnShowDeleteGame != null)
            {
                if (deleteButtonsVisible)
                {
                    OnShowDeleteGame(false);
                    deleteButtonsVisible = false;
                    editButtonText.text = "Edit";
                }
                else
                {
                    OnShowDeleteGame(true);
                    deleteButtonsVisible = true;
                    editButtonText.text = "Done";
                }
            }
        }

        public void ResetEditButton() {
            editButtonText.text = "Edit";
            deleteButtonsVisible = false;
        }
    }
}