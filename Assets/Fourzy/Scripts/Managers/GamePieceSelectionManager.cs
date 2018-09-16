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

        public void LoadGamePieces(string gamePieceId, bool isEnabledPieceSelector = true)
        {
            gamePieceData = TokenBoardLoader.instance.GetAllGamePieces();

            this.UpdateGamePieceSelectionView(gamePieceId, isEnabledPieceSelector);
            this.UpdatePrefabsWithGamePieceData();
        }

        private void UpdateGamePieceSelectionView(string gamePieceId, bool isEnabledPieceSelector)
        {
            for (int i = 0; i < gamePieceData.Length; i++)
            {
                var piece = gamePieceData[i];
                GameObject go = Instantiate(gamePiecePrefab) as GameObject;
                GamePieceUI gamePieceUI = go.GetComponent<GamePieceUI>();
                gamePieceUI.id = piece.ID;
                gamePieceUI.name = piece.Name;
                gamePieceUI.gamePieceImage.sprite = gamePieces[int.Parse(piece.ID)];
                gamePieceUI.isEnabledPieceSelector = isEnabledPieceSelector;

                go.transform.SetParent(gamePieceGrid.transform, false);

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

        private void UpdatePrefabsWithGamePieceData()
        {
            for (int i = 0; i < gamePiecePrefabs.Count; i++)
            {
                var piece = gamePieceData[i];
                gamePiecePrefabs[i].View.OutlineColor = piece.OutlineColorWrapper;
                gamePiecePrefabs[i].SecondaryColor = piece.SecondaryColorWrapper;
            }
        }

        public Sprite GetGamePieceSprite(int gamePieceId) 
        {
            return gamePieces[gamePieceId];
        }

        public string GetGamePieceName(int gamePieceId)
        {
            for (int i = 0; i < gamePieceData.Length; i++)
            {
                if (gamePieceData[i].ID == gamePieceId.ToString())
                {
                    return gamePieceData[i].Name;
                }
            }
            return "Error";
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