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

        public static Vector3 topLeft;
        public static Vector3 step;
    }
}