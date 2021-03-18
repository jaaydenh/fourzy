using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class SymmetricArrowsFeature : IBoardIngredient
    {

        bool Inward { get; set; }
        int BlockedPerSide { get; set; }
        string BlockPattern { get; set; }

        public string Name { get { return "Symmetric Arrows"; } }
        public IngredientType Type { get { return IngredientType.SYMMETRICARROWS; } }
        public List<TokenType> Tokens { get; }


        public SymmetricArrowsFeature(bool inward = true, int BlockedPerSide = 1, string BlockPattern = "")
        {
            this.Inward = inward;
            this.BlockedPerSide = BlockedPerSide;
            this.BlockPattern = BlockPattern;
            this.Tokens = new List<TokenType>() { TokenType.ARROW };
        }

        public void Build(GameBoard Board)
        {
                    
            if (BlockPattern.Length == 0)
            {
                int Count = 0;
                int Blocked = Board.Random.RandomInteger(0, Math.Min(Board.Rows, Board.Columns) - 1);
                for (int i = 0; i < Math.Min(Board.Rows, Board.Columns); i++)
                {
                    if (i == Blocked && Count < BlockedPerSide )
                    {
                        BlockPattern += "1";
                        Count++;
                        Blocked = Board.Random.RandomInteger(i + 1, Math.Min(Board.Rows, Board.Columns));
                        continue;
                    }
                    BlockPattern += "0";
                }
            }
            
            //top
            for (int c = 0; c < Board.Columns; c++) if (BlockPattern[c] == '1') Board.AddToken(new ArrowToken(Direction.DOWN), new BoardLocation(0, c));
            //bottom
            for (int c = 0; c < Board.Columns; c++) if (BlockPattern[c] == '1') Board.AddToken(new ArrowToken(Direction.UP), new BoardLocation(Board.Rows - 1, Board.Columns - c - 1));
            //left
            for (int r = 0; r < Board.Rows; r++) if (BlockPattern[Board.Rows - r - 1] == '1') Board.AddToken(new ArrowToken(Direction.RIGHT), new BoardLocation(r, 0));
            //right
            for (int r = 0; r < Board.Rows; r++) if (BlockPattern[Board.Rows - r -1] == '1') Board.AddToken(new ArrowToken(Direction.LEFT), new BoardLocation(Board.Rows - r - 1, Board.Columns - 1));

        }
    }
}
