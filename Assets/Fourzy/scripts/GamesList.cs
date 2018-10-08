using UnityEngine;

namespace Fourzy {
    public class GamesList : MonoBehaviour {

        private void OnEnable() {
            if (ChallengeManager.Instance)
            {   
                //ChallengeManager.Instance.GetChallenges();

            }
        }
    }
}
