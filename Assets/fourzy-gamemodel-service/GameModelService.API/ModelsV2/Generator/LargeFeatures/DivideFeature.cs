using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class DivideFeature : IBoardIngredient
    {
        public LargeFeatureType Feature { get { return LargeFeatureType.DIVIDE; } }
        public string Name { get { return "Divide"; } }
        public IngredientType Type { get { return IngredientType.LARGEFEATURE; } }
        public int DivideRatio { get; set; }
        public LineType DivideType { get; set; }
        public IToken TokenA { get; set; }
        public IToken TokenB { get; set; }

        public DivideFeature(IToken TokenA, IToken TokenB, LineType DivideType = LineType.NONE, int DivideRatio = -1)
        {
            this.DivideRatio = DivideRatio;
            this.DivideType = DivideType;
            this.TokenA = TokenA;
            this.TokenB = TokenB;
        }

        public void Build(GameBoard Board)
        {
            if (DivideRatio < 0) DivideRatio = Board.Random.RandomInteger(1, Math.Min(Board.Rows, Board.Columns));
            if (DivideType == LineType.NONE) DivideType = (LineType)Board.Random.RandomInteger(0, 1);

            for (int r = 0; r < Board.Rows; r++)
            {
                for (int c = 0; c < Board.Columns; c++)
                {
                    switch (DivideType)
                    {
                        case LineType.HORIZONTAL:

                            if (c > DivideRatio)
                                Board.AddToken(TokenA, new BoardLocation(r, c), AddTokenMethod.ALWAYS, true);
                            else
                                Board.AddToken(TokenB, new BoardLocation(r, c), AddTokenMethod.ALWAYS, true);

                            break;
                        case LineType.VERTICAL:

                            if (r > DivideRatio)
                                Board.AddToken(TokenA, new BoardLocation(r, c), AddTokenMethod.ALWAYS, true);
                            else
                                Board.AddToken(TokenB, new BoardLocation(r, c), AddTokenMethod.ALWAYS, true);

                            break;
                    }

                }
            }


        }
    }
}
