using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    
    public class NextGameButton : MonoBehaviour {

        void OnEnable() {
            Debug.Log("Next Game Button - OnEnable");
            Debug.Log("Next Game Button - Challengeinstanceid" + GameManager.instance.challengeInstanceId);

            //if (ChallengeManager.instance.activeGameIds.Count == 0) {
                //this.gameObject.SetActive(false);
            //}
        }

        public void CheckForActiveGames() {
            
        }
    }
}
