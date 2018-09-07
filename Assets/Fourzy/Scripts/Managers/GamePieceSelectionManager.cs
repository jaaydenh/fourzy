using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
 [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene, false)]
    public class GamePieceSelectionManager : UnitySingleton<GamePieceSelectionManager>
    {
        [SerializeField]
        private List<GamePiece> gamePiecePrefabs = new List<GamePiece>(); 

        public GameObject gamePieceGrid;
        public List<Sprite> gamePieces = new List<Sprite>();
        public GameObject gamePiecePrefab;
        GamePieceData[] gamePieceData;

        public string GetGamePieceName(int gamePieceId) {
            for (int i = 0; i < gamePieceData.Length; i++)
            {
                if (gamePieceData[i].ID == gamePieceId.ToString()) {
                    return gamePieceData[i].Name;
                }
            }
            return "Error";
        }

        public void LoadGamePieces(string gamePieceId, bool isEnabledPieceSelector = true)
        {
            gamePieceData = TokenBoardLoader.instance.GetAllGamePieces();
    
            //Debug.Log("LoadGamePieces gamePieceId: " + gamePieceId);
            foreach (var piece in gamePieceData)
            {
                GameObject go = Instantiate(gamePiecePrefab) as GameObject;
                GamePieceUI gamePieceUI = go.GetComponent<GamePieceUI>();
                gamePieceUI.id = piece.ID;
                gamePieceUI.name = piece.Name;
                gamePieceUI.gamePieceImage.sprite = gamePieces[int.Parse(piece.ID)];
                gamePieceUI.isEnabledPieceSelector = isEnabledPieceSelector;

                go.gameObject.transform.SetParent(gamePieceGrid.transform, false);

                if (isEnabledPieceSelector)
                {
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

        public Sprite GetGamePieceSprite(int gamePieceId) {
            //Debug.Log("GetGamePieceSprite: " + gamePieceId);
            return gamePieces[gamePieceId];
        }

        public GameObject GetGamePiecePrefab(int gamePieceId)
        {
            if (gamePieceId < gamePiecePrefabs.Count && gamePieceId >= 0)
            {
                return gamePiecePrefabs[gamePieceId].gameObject;
            }
            else
            {
                return gamePiecePrefabs[0].gameObject;
            }
        }
    }
}