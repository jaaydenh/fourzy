//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using System.Collections.Generic;
using UnityEngine;
using StackableDecorator;
using System.Linq;
using UnityEditor;
using FourzyGameModel.Model;

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
            public AIProfile profile;
            public BossType bossProfile = BossType.None;
            public Sprite background;
            public bool enabled;

            private GamePiecesDataHolder gamePiecesHolder;
            private string _gamePieceID = "-1";
            private string message;

            public string Message()
            {
#if UNITY_EDITOR
                if (!gamePiecesHolder) gamePiecesHolder = AssetDatabase.LoadAssetAtPath<GamePiecesDataHolder>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("DefaultGamePiecesDataHolder")[0]));

                if (_gamePieceID != gamePieceID)
                {
                    _gamePieceID = gamePieceID;

                    GamePieceData gamepieceData = gamePiecesHolder.GetGamePieceData(gamePieceID);

                    if (gamepieceData != null)
                        message = $"Piece name {gamepieceData.name}";
                    else
                        message = "Wrong ID";
                }
#endif
                return message;
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
