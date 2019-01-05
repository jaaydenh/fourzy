using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    public class Reward 
    {
        public enum RewardType
        {
            Coins,
            CollectedGamePiece
        }

        public RewardType Type = RewardType.Coins;
        public int NumberOfCoins = 0;
        public GamePieceData CollectedGamePiece;
    }
}
