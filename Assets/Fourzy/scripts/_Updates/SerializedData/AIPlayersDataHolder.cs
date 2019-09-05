//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using System.Collections.Generic;
using UnityEngine;
using StackableDecorator;
using System.Linq;
using UnityEditor;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultAIPlayersHolder", menuName = "Create AI Players Data Holder")]
    public class AIPlayersDataHolder : ScriptableObject
    {
        public static AIPlayerData SELECTED;

        [List]
        public AIPlayersCollection aiPlayers;

        public List<AIPlayerData> enabledAIPlayers => aiPlayers.list.Where(aiPlayer => aiPlayer.enabled).ToList();

        [System.Serializable]
        public class AIPlayersCollection
        {
            public List<AIPlayerData> list;
        }

        [System.Serializable]
        public class AIPlayerData
        {
            public string name;
            [StackableField]
            [HelpBox(message: "#Message", messageType = StackableDecorator.MessageType.None, below = true)]
            public string gamePieceID;
            public Sprite background;
            public bool enabled;

            public string Message()
            {
#if UNITY_EDITOR
                string[] guids = AssetDatabase.FindAssets("DefaultGamePiecesDataHolder");
                GamePiecesDataHolder gamePiecesHolder = AssetDatabase.LoadAssetAtPath<GamePiecesDataHolder>(AssetDatabase.GUIDToAssetPath(guids[0]));

                GamePieceData gamepieceData = gamePiecesHolder.GetGamePieceData(gamePieceID);

                if (gamepieceData != null)
                    return $"Piece name {gamepieceData.name}";
                else
                    return "Wrong ID";
#else
                return "";
#endif
            }

            //conditions list
        }

        [System.Serializable]
        public class PlayerAIUnlockConditionsList
        {

        }

        [System.Serializable]
        public class PlayerAIUnlockCondition
        {

        }
    }
}
