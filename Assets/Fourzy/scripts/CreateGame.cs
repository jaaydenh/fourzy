using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class CreateGame : MonoBehaviour {

        Image createButtonImage;
        Text buttonText;
        Color gameFound = new Color32(33, 206, 153, 255);
        Color gameCreate = new Color32(0,176,255,255);
        Color gameSearching = new Color32(255,155,0,255);
        private bool isRunning = false;

        void Start () {
            createButtonImage = this.GetComponent<Image>();
            buttonText = this.GetComponentInChildren<Text>();
        }

        public IEnumerator SetButtonState (bool isSearching) {
            isRunning = true;
            if (isSearching)
            {
                createButtonImage.color = gameSearching;
                buttonText.text = "Searching";
            }
            else
            {
                createButtonImage.color = gameFound;
                buttonText.text = "Game Found";

                yield return new WaitForSeconds(2f);

                createButtonImage.color = gameCreate;
                buttonText.text = "Find Quick Match";
            }

            yield return null;
            isRunning = false;
    }

//        public void ResetFindMatchButton() {
//            createButtonImage.color = gameCreate;
//            buttonText.text = "Find Quick Match";
//        }

        public void SetButtonStateWrapper(bool isSearching){
            if(isRunning == false){
                StartCoroutine(SetButtonState(isSearching));
            }
        }
    }
}
