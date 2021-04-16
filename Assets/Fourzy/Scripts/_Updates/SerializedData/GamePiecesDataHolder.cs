//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Tools;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultGamePiecesDataHolder", menuName = "Create GamePieces Data Holder")]
    public class GamePiecesDataHolder : ScriptableObject
    {
        [ListDrawerSettings(NumberOfItemsPerPage = 15, ListElementLabelName = "name")]
        public List<GamePieceData> gamePieces;

        public Dictionary<string, GamePieceData> gamePiecesFastAccess { get; set; }

        public Dictionary<string, GamePieceData> enabledGamePiecesFastAccess { get; set; }

        public GamePieceData random => enabledGamePiecesFastAccess.Values.Random();

        public void Initialize()
        {
            gamePiecesFastAccess = new Dictionary<string, GamePieceData>();
            enabledGamePiecesFastAccess = new Dictionary<string, GamePieceData>();

            foreach (GamePieceData gamePieceData in gamePieces)
            {
                if (gamePieceData.enabled)
                {
                    gamePieceData.Initialize();

                    gamePiecesFastAccess.Add(gamePieceData.Id, gamePieceData);

                    enabledGamePiecesFastAccess.Add(gamePieceData.Id, gamePieceData);
                }
            }
        }

        public GamePieceData GetGamePiecePrefabData(string gamePieceID)
        {
            if (gamePiecesFastAccess.ContainsKey(gamePieceID))
            {
                return gamePiecesFastAccess[gamePieceID];
            }
            else
            {
                return gamePieces[0];
            }
        }

        public GamePieceData GetGamePieceData(string gamePieceID)
        {
            if (Application.isPlaying)
            {
                if (!gamePiecesFastAccess.ContainsKey(gamePieceID)) return null;

                return gamePiecesFastAccess[gamePieceID];
            }
            else
            {
                return gamePieces.Find(gamePiece => gamePiece.Id == gamePieceID);
            }
        }

        public GamePieceData GetGamePieceData(GamePieceView gamePiece)
        {
            return gamePieces.Find(_prefabData =>
                gamePiece.name.Contains(_prefabData.player1Prefab.name) ||
                gamePiece.name.Contains(_prefabData.player2Prefab.name));
        }

        public void UnlockAll()
        {
            foreach (GamePieceData gamePiecePrefabData in enabledGamePiecesFastAccess.Values)
            {
                gamePiecePrefabData.Pieces = gamePiecePrefabData.piecesToUnlock;
            }
        }

        public static void ClearGamepiecesData()
        {
            for (int index = 0; index < 50; index++)
            {
                PlayerPrefsWrapper.GamePieceDeleteData(index + "");
            }
        }

        public static GamePieceData _GetGamePieceData(GamePieceView gamePiece)
        {
            if (Application.isPlaying)
                return GameContentManager.Instance.piecesDataHolder.GetGamePieceData(gamePiece);
#if UNITY_EDITOR
            else
                return AssetDatabase.LoadAssetAtPath<GamePiecesDataHolder>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("DefaultGamePiecesDataHolder")[0])).GetGamePieceData(gamePiece);
#else
            return null;
#endif
        }
    }
}
