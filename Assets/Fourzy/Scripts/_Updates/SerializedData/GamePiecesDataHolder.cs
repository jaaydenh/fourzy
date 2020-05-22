//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using System.Collections.Generic;
using UnityEngine;
using StackableDecorator;
using UnityEditor;
using System;
using Fourzy._Updates.Tools;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultGamePiecesDataHolder", menuName = "Create GamePieces Data Holder")]
    public class GamePiecesDataHolder : ScriptableObject
    {
        [List]
        [HelpBox("#Message", messageType = StackableDecorator.MessageType.None, below = false)]
        public GamePiecePrefabDataCollection gamePieces;

        public Dictionary<string, GamePiecePrefabData> gamePiecesFastAccess { get; set; }

        public Dictionary<string, GamePiecePrefabData> enabledGamePiecesFastAccess { get; set; }

        public GamePiecePrefabData random => enabledGamePiecesFastAccess.Values.Random();

        public void Initialize()
        {
            gamePiecesFastAccess = new Dictionary<string, GamePiecePrefabData>();
            enabledGamePiecesFastAccess = new Dictionary<string, GamePiecePrefabData>();

            foreach (GamePiecePrefabData gamePiece in gamePieces.list)
            {
                gamePiece.data.Initialize();

                gamePiecesFastAccess.Add(gamePiece.data.ID, gamePiece);

                if (gamePiece.data.enabled) enabledGamePiecesFastAccess.Add(gamePiece.data.ID, gamePiece);
            }
        }

        public GamePiecePrefabData GetGamePiecePrefabData(string gamePieceID)
        {
            if (gamePiecesFastAccess.ContainsKey(gamePieceID))
                return gamePiecesFastAccess[gamePieceID];
            else
                return gamePieces.list[0];
        }

        public GamePieceData GetGamePieceData(string gamePieceID)
        {
            if (Application.isPlaying)
            {
                if (!gamePiecesFastAccess.ContainsKey(gamePieceID)) return null;

                return gamePiecesFastAccess[gamePieceID].data;
            }
            else
            {
                GamePiecePrefabData gamePieceData = gamePieces.list.Find(gamePiece => gamePiece.data.ID == gamePieceID);

                if (gamePieceData != null) return gamePieceData.data;
                else return null;
            }
        }

        public GamePieceData GetGamePieceData(GamePieceView gamePiece)
        {
            GamePiecePrefabData prefabData = gamePieces.list.Find(_prefabData => 
                gamePiece.name.Contains(_prefabData.player1Prefab.name) ||
                gamePiece.name.Contains(_prefabData.player2Prefab.name));

            if (prefabData == null) return null;

            return prefabData.data;
        }

        public void UnlockAll()
        {
            foreach (GamePiecePrefabData gamePiecePrefabData in enabledGamePiecesFastAccess.Values)
                gamePiecePrefabData.data.Pieces = gamePiecePrefabData.data.piecesToUnlock;
        }

        public static void ClearGamepiecesData()
        {
            for (int index = 0; index < 50; index++) PlayerPrefsWrapper.GamePieceDeleteData(index + "");
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

        public string Message() => $"Gamepieces Count: {gamePieces.list.Count}";
    }

    [System.Serializable]
    public class GamePiecePrefabData
    {
        [StackableField, ShowIf("#ShowIf")]
        public string _name;

        public GamePieceData data;
        public GamePieceView player1Prefab;
        public GamePieceView player2Prefab;

        private bool ShowIf()
        {
            _name = data.name + (player1Prefab ? " " + player1Prefab.name : "");

            return false;
        }
    }

    [System.Serializable]
    public class GamePiecePrefabDataCollection
    {
        public List<GamePiecePrefabData> list;
    }
}
