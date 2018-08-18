using UnityEngine;

namespace Fourzy
{
    public class Position {

        public int column;
        public int row;

        public Position(int column, int row) {
            this.column = column;
            this.row = row;
        }
       
        public Vector3 ConvertToVec3()
        {
            float x = (column + .1f) * .972f;
            float y = (row * -1 + .05f) * .96f;
            return new Vector3(x, y);
        }
    }
}