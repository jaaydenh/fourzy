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

        public static float Distance(Position one, Position two)
        {
            return UnityEngine.Mathf.Sqrt((two.row - one.row) * (two.row - one.row) + (two.column - one.column) * (two.column - one.column));
        }
    }
}