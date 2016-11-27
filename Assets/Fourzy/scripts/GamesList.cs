using UnityEngine;
using System.Collections;

namespace Fourzy {
    
    public class GamesList : MonoBehaviour {

    	// Use this for initialization
    	void Start () {
    	
    	}

        private void OnEnable() {
            //Debug.Log("GamesList Enabled");
            if (ChallengeManager.instance)
            {   
                ChallengeManager.instance.GetActiveChallenges();
            }
        }
    }
}
