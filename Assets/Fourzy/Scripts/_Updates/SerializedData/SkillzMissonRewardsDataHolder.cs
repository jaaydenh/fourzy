using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultSkillzMissionRewardsDataHolder", menuName = "Create SkillzMissionRewards Data Holder")]
    public class SkillzMissonRewardsDataHolder : ScriptableObject
    {
#if UNITY_EDITOR
        public static GamePiecesDataHolder Current { get; private set; }
#endif

        [SerializeField]
        [ValidateInput("PieceDataEmpty", "Specify GamePiecesDataHolder object.")]
        private GamePiecesDataHolder piecesDataHolder;

        [SerializeField]
        [ShowIf("piecesDataHolder")]
        private List<SkillzGamepieceGroup> gamepieceGroups;

        public List<SkillzGamepieceGroup> Groups => gamepieceGroups;

#if UNITY_EDITOR
        private bool PieceDataEmpty(GamePiecesDataHolder dataHolder)
        {
            Current = dataHolder;

            return dataHolder != null;
        }
#endif
    }

    [Serializable]
    public class SkillzGamepieceGroup
    {
        public string categoryId;
        [ValueDropdown("GetPiecesIds", IsUniqueList = true, ExcludeExistingValuesInList = true)]
        public List<string> pieces;

        // Requirements.
        public bool cashGames;
        public int gameCount;

#if UNITY_EDITOR
        private IEnumerable GetPiecesIds()
        {
            if (!SkillzMissonRewardsDataHolder.Current)
            {
                return new string[0];
            }

            return SkillzMissonRewardsDataHolder.Current.gamePieces.Select(pieceData => new ValueDropdownItem(pieceData.name, pieceData.Id));
        }
#endif
    }
}
