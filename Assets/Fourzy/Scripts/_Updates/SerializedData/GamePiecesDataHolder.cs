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

        public GamePieceData random => gamePiecesFastAccess.Values.Random();

        public void Initialize()
        {
            gamePiecesFastAccess = new Dictionary<string, GamePieceData>();

            foreach (GamePieceData gamePieceData in gamePieces)
            {
                if (gamePieceData.enabled)
                {
                    // Skillz mod.
                    gamePieceData.PiecesToUnlock = 1;

                    gamePiecesFastAccess.Add(gamePieceData.Id, gamePieceData);
                }
            }
        }

        public GamePieceData GetGamePieceData(string gamePieceId)
        {
            if (Application.isPlaying)
            {
                if (!gamePiecesFastAccess.ContainsKey(gamePieceId)) return null;

                return gamePiecesFastAccess[gamePieceId];
            }
            else
            {
                return gamePieces.Find(gamePiece => gamePiece.Id == gamePieceId);
            }
        }

        public GamePieceData GetGamePieceData(GamePieceView gamePiece)
        {
            return gamePieces.Find(_prefabData =>
                gamePiece.name.Contains(_prefabData.player1Prefab.name) ||
                gamePiece.name.Contains(_prefabData.player2Prefab.name));
        }

        public void ResetPieces()
        {
            foreach (GamePieceData data in gamePieces)
            {
                data.Pieces = 0;
                data.Magic = 0;
                //default value
                data.PiecesToUnlock = 30;
            }
        }

        public static GamePieceData _GetGamePieceData(GamePieceView gamePiece)
        {
            if (Application.isPlaying)
            {
                return GameContentManager.Instance.piecesDataHolder.GetGamePieceData(gamePiece);
            }
#if UNITY_EDITOR
            else
            {
                return AssetDatabase.LoadAssetAtPath<GamePiecesDataHolder>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("DefaultGamePiecesDataHolder")[0])).GetGamePieceData(gamePiece);
            }
#else
            return null;
#endif
        }
    }
}
