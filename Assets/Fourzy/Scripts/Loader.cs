using UnityEngine;

namespace Fourzy
{
    public class Loader : MonoBehaviour 
    {
        public GameObject gameManager;  //GameManager prefab to instantiate.
        public GameObject soundManager; //SoundManager prefab to instantiate.

        void Awake ()
        {
            //Check if a GameManager has already been assigned to static variable GameManager.Instance or if it's still null
            if (GameManager.Instance == null) {
                //Instantiate gameManager prefab
                Instantiate(gameManager);
            }

            //Check if a SoundManager has already been assigned to static variable SoundManager.Instance or if it's still null
             if (SoundManager.Instance == null) {
                 //Instantiate SoundManager prefab
                 Instantiate(soundManager);
             }
        }
    }
}