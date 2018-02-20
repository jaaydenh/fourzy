//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Fourzy
//{
//    public class Player : MonoBehaviour
//    {
//        public static Player instance = null;
//        public int gamePieceId;
//        private bool didLoadGamePieces;

//        void Awake()
//        {
//            if (instance == null)
//            {
//                instance = this;
//            }
//            else if (instance != this)
//            {
//                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a Player.
//                Destroy(gameObject);
//            }

//            //Sets this to not be destroyed when reloading scene
//            DontDestroyOnLoad(gameObject);
//        }

//        private void OnEnable()
//        {
//            ChallengeManager.OnReceivedPlayerGamePiece += SetPlayerGamePiece;
//            ChallengeManager.OnSetGamePieceSuccess += SetPlayerGamePiece;
//        }

//        private void OnDisable()
//        {
//            ChallengeManager.OnReceivedPlayerGamePiece -= SetPlayerGamePiece;
//            ChallengeManager.OnSetGamePieceSuccess -= SetPlayerGamePiece;
//        }

//        void Start()
//        {
//            ChallengeManager.instance.GetPlayerGamePiece();
//        }

//        private void SetPlayerGamePiece(string id)
//        {
//            Debug.Log("SetPlayerGamePiece: gamepieceid: " + id);
//            gamePieceId = int.Parse(id);
//            if (!didLoadGamePieces) {
//                GamePieceSelectionManager.instance.LoadGamePieces(id);
//                didLoadGamePieces = true;
//            }
//        }
//    }
//}