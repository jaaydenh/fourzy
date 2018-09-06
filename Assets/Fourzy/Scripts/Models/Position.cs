using UnityEngine;

namespace Fourzy
{
    public class Position 
    {

        public int column;
        public int row;

        public Position(int column, int row) 
        {
            this.column = column;
            this.row = row;
        }
       
        public Vector3 ConvertToVec3()
        {
            float posX = topLeft.x + step.x * 0.5f + step.x * column;
            float posY = topLeft.y - step.y * 0.5f - step.y * row;

            return new Vector3(posX, posY);
        }

        public static Position Vec3ToPosition(Vector3 position)
        {
            int x = Mathf.FloorToInt((position.x - topLeft.x) / step.x);
            int y = Mathf.FloorToInt(-(position.y - topLeft.y) / step.y);

            return new Position(x, y);
        }

        public bool IsTopRow()
        {
            return (row == 0 && column >= 0 && column < Constants.numColumns);
        }

        public bool IsBottomRow()
        {
            return (row == Constants.numRows - 1 && column >= 0 && column < Constants.numColumns);
        }

        public bool IsLeftColumn()
        {
            return (column == 0 && row >= 0 && row < Constants.numRows);
        }

        public bool IsRightColumn()
        {
            return (column == Constants.numColumns - 1 && row >= 0 && row < Constants.numRows);
        }

        public static Vector3 topLeft;
        public static Vector3 step;
    }
}