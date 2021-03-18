using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{ 
    public static class PieceConstants
    {
        public static string PieceCondition(PieceConditionType Type)
        {
            switch (Type)
            {
                case PieceConditionType.CURSED:
                    return "C";
                case PieceConditionType.DEAD:
                    return "D";
                case PieceConditionType.FALLING:
                    return "L";
                case PieceConditionType.FIERY:
                    return "F";
                case PieceConditionType.FLYING:
                    return "Y";
                case PieceConditionType.FROZEN:
                    return "Z";
                case PieceConditionType.GIANT:
                    return "G";
                case PieceConditionType.POISONED:
                    return "P";
                case PieceConditionType.SCARED:
                    return "S";
                case PieceConditionType.SHRUNKEN:
                    return "R";
                case PieceConditionType.SINKING:
                    return "N";
                case PieceConditionType.STUCK:
                    return "K";
                case PieceConditionType.WIND:
                    return "W";
                default:
                    return "";
            }
        }

        public static PieceConditionType PieceCondition(string PieceConditionString)
        {
            switch (PieceConditionString)
            {
                case "C":
                    return PieceConditionType.CURSED;
                case "D":
                    return PieceConditionType.DEAD;
                case "L":
                    return PieceConditionType.FALLING;
                case "F":
                    return PieceConditionType.FIERY;
                case "Y":
                    return PieceConditionType.FLYING;
                case "Z":
                    return PieceConditionType.FROZEN;
                case "G":
                    return PieceConditionType.GIANT;
                case "P":
                    return PieceConditionType.POISONED;
                case "S":
                    return PieceConditionType.SCARED;
                case "R":
                    return PieceConditionType.SHRUNKEN;
                case "N":  
                    return PieceConditionType.SINKING;
                case "K":
                    return PieceConditionType.STUCK;
                case "W":
                    return PieceConditionType.WIND;
                default:
                    return PieceConditionType.NONE;

            }
        }
    }
}
