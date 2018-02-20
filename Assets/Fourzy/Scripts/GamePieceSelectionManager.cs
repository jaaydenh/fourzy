using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class GamePieceSelectionManager : MonoBehaviour
    {
        public GameObject gamePieceGrid;
        public List<Sprite> gamePieces = new List<Sprite>();
        public GameObject gamePiecePrefab;
        GamePieceData[] gamePieceData;

        private static GamePieceSelectionManager _instance;
        public static GamePieceSelectionManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GamePieceSelectionManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        private void Start()
        {
            //ChallengeManager.instance.GetGamePiece();
        }

        public string GetGamePieceName(int gamePieceId) {
            for (int i = 0; i < gamePieceData.Length; i++)
            {
                if (gamePieceData[i].ID == gamePieceId.ToString()) {
                    return gamePieceData[i].Name;
                }
            }
            return "Error";
        }

        public void LoadGamePieces(string gamePieceId)
        {
            gamePieceData = TokenBoardLoader.instance.GetAllGamePieces();

            Debug.Log("LoadGamePieces gamePieceId: " + gamePieceId);
            foreach (var piece in gamePieceData)
            {
                GameObject go = Instantiate(gamePiecePrefab) as GameObject;
                go.transform.localScale = new Vector3(1, 1, 1);
                GamePieceUI gamePieceUI = go.GetComponent<GamePieceUI>();
                gamePieceUI.id = piece.ID;
                gamePieceUI.name = piece.Name;
                go.GetComponentInChildren<Image>().sprite = gamePieces[int.Parse(piece.ID)];

                //gamePieceUI.GetComponent<Image>().sprite = gamePieces[int.Parse(piece.ID)];
                //gamePieceUI.GetComponent<SpriteRenderer>().sprite = gamePieces[int.Parse(piece.ID)];

                //go.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

                go.gameObject.transform.SetParent(gamePieceGrid.transform);

                var toggle = go.GetComponent<Toggle>();
                ToggleGroup tg = gamePieceGrid.GetComponent<ToggleGroup>();
                toggle.group = tg;

                if (string.Equals(piece.ID, gamePieceId))
                {
                    gamePieceUI.ActivateSelector();
                }
            }
        }
    }
}