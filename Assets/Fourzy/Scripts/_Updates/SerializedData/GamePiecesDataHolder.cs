//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "GamePiecesDataHolder", menuName = "Create GamePieces data holder")]
    public class GamePiecesDataHolder : ScriptableObject
    {
        public GamePiecePrefabData[] gamePieces;

        public Dictionary<int, GamePiecePrefabData> gamePiecesFastAccess { get; set; }
        public Dictionary<int, GamePiecePrefabData> enabledGamePiecesFastAccess { get; set; }

        public void Init()
        {
            gamePiecesFastAccess = new Dictionary<int, GamePiecePrefabData>();
            enabledGamePiecesFastAccess = new Dictionary<int, GamePiecePrefabData>();

            foreach (GamePiecePrefabData gamePiece in gamePieces)
            {
                gamePiecesFastAccess.Add(gamePiece.data.ID, gamePiece);

                if (gamePiece.data.Enabled)
                    enabledGamePiecesFastAccess.Add(gamePiece.data.ID, gamePiece);

                foreach (GamePiece prefabVariant in gamePiece.prefabs)
                    prefabVariant.pieceData = gamePiece.data;
            }
        }

        public GamePiecePrefabData GetGamePiecePrefabData(int ID)
        {
            if (gamePiecesFastAccess.ContainsKey(ID))
                return gamePiecesFastAccess[ID];
            else
                return gamePieces[0];
        }

        public string GetGamePieceName(int gamePieceID)
        {
            if (!gamePiecesFastAccess.ContainsKey(gamePieceID))
                return string.Empty;

            return gamePiecesFastAccess[gamePieceID].data.Name;
        }
    }

    [System.Serializable]
    public class GamePiecePrefabData
    {
        public GamePieceData data;
        public GamePiece[] prefabs;
    }
}
