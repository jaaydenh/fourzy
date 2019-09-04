//@vadym udod

using Fourzy._Updates.Mechanics.Rewards;
using System.Collections.Generic;
using UnityEngine;
using static Fourzy._Updates.Serialized.PuzzlePacksDataHolder;

namespace Fourzy._Updates.ClientModel
{
    public interface IPuzzlePack
    {
        string __name { get; }
        PackType __packType { get; }
        string __packID { get; }
        string __herdID { get; }
        string __profileName { get; }
        //Color __aiColor { get; }
        //Color __playerColor { get; }

        List<ClientPuzzleData> puzzlesData { get; set; }
        List<ClientPuzzleData> enabledPuzzlesData { get; }
        List<ClientPuzzleData> rewardPuzzles { get; }
        List<RewardsManager.Reward> allRewards { get; set; }

    }
}