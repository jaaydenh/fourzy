//modded @vadym udod

using UnityEngine;

namespace Fourzy
{
    public class Position
    {
        public static Position zero
        {
            get
            {
                return new Position(0, 0);
            }
        }

        public Vector2Int value;

        public int row
        {
            get { return value.y; }
            set { this.value.y = value; }
        }

        public int column
        {
            get { return value.x; }
            set { this.value.x = value; }
        }

        public Position(int column, int row)
        {
            value = new Vector2Int(column, row);
        }

        public bool IsTopRow()
        {
            return (value.y == 0 && value.x >= 0 && value.x < Constants.numColumns);
        }

        public bool IsBottomRow()
        {
            return (value.y == Constants.numRows - 1 && value.x >= 0 && value.x < Constants.numColumns);
        }

        public bool IsLeftColumn()
        {
            return (value.x == 0 && value.y >= 0 && value.y < Constants.numRows);
        }

        public bool IsRightColumn()
        {
            return (value.x == Constants.numColumns - 1 && value.y >= 0 && value.y < Constants.numRows);
        }

        public static float Distance(Position one, Position two)
        {
            return Vector2Int.Distance(one.value, two.value);
        }
    }
}