//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using System.Collections.Generic;
using UnityEngine;
using StackableDecorator;
using UnityEditor;
using System;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultGamePiecesDataHolder", menuName = "Create GamePieces Data Holder")]
    public class GamePiecesDataHolder : ScriptableObject
    {
        [List]
        [HelpBox("#Message", messageType = StackableDecorator.MessageType.None, below = false)]
        public GamePiecePrefabDataCollection gamePieces;

        public Dictionary<int, GamePiecePrefabData> gamePiecesFastAccess { get; set; }
        public Dictionary<int, GamePiecePrefabData> enabledGamePiecesFastAccess { get; set; }

        public void Initialize()
        {
            gamePiecesFastAccess = new Dictionary<int, GamePiecePrefabData>();
            enabledGamePiecesFastAccess = new Dictionary<int, GamePiecePrefabData>();

            foreach (GamePiecePrefabData gamePiece in gamePieces.list)
            {
                if (!PlayerPrefsWrapper.HaveGamePieceRecord(gamePiece.data))
                {
                    PlayerPrefsWrapper.GamePieceUpdatePiecesCount(gamePiece.data);
                    PlayerPrefsWrapper.GamePieceUpdateChampionsCount(gamePiece.data);
                }
                else
                    gamePiece.data.Initialize();

                gamePiecesFastAccess.Add(gamePiece.data.ID, gamePiece);

                if (gamePiece.data.enabled)
                    enabledGamePiecesFastAccess.Add(gamePiece.data.ID, gamePiece);
            }
        }

        public GamePiecePrefabData GetGamePiecePrefabData(int gamePieceID)
        {
            if (gamePiecesFastAccess.ContainsKey(gamePieceID))
                return gamePiecesFastAccess[gamePieceID];
            else
                return gamePieces.list[0];
        }

        public GamePieceData GetGamePieceData(int gamePieceID)
        {
            if (Application.isPlaying)
            {
                if (!gamePiecesFastAccess.ContainsKey(gamePieceID))
                    return null;

                return gamePiecesFastAccess[gamePieceID].data;
            }
            else
                return gamePieces.list.Find(gamePiece => gamePiece.data.ID == gamePieceID).data;
        }

        public GamePieceData GetGamePieceData(GamePieceView gamePiece)
        {
            GamePiecePrefabData prefabData = gamePieces.list.Find(_prefabData => _prefabData.player1Prefab.key == gamePiece.key || _prefabData.player2Prefab.key == gamePiece.key);

            if (prefabData == null) return null;

            return prefabData.data;
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
        [HideInInspector]
        public string _name;
        [HideInInspector]
        public string _key;

        [ShowIf("#Check")]
        [StackableField]
        public GamePieceData data;
        public GamePieceView player1Prefab;
        [StackableField]
        [Buttons(titles = "Reset Keys", actions = "Reset", below = true)]
        public GamePieceView player2Prefab;

        public bool Check()
        {
            if (string.IsNullOrEmpty(_key))
                _key = Guid.NewGuid().ToString();

            if (player1Prefab && player1Prefab.key != _key) player1Prefab.key = _key;
            if (player2Prefab && player2Prefab.key != _key) player2Prefab.key = _key;

            _name = data.name + ": " + (player1Prefab ? player1Prefab.name : "No Prefab");

            return true;
        }

        public void Reset()
        {
            _key = Guid.NewGuid().ToString();

            if (player1Prefab) player1Prefab.key = _key;
            if (player2Prefab) player2Prefab.key = _key;
        }
    }

    [System.Serializable]
    public class GamePiecePrefabDataCollection
    {
        public List<GamePiecePrefabData> list;
    }
}
