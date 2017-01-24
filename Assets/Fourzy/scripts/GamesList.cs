using UnityEngine;
using System.Collections;

namespace Fourzy {
    
    public class GamesList : MonoBehaviour {

        private void OnEnable() {
            if (ChallengeManager.instance)
            {   
                ChallengeManager.instance.GetChallenges();
            }
        }
    }
}
