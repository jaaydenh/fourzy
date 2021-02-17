using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ArrowWingFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public TokenType Token { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.ARROW_WING; } }
        public DiagonalType LineDirection { get; set; }
        public Direction ArrowDirection { get; set; }

        public AddTokenMethod AddMethod { get; set; }
        public bool ReplaceTokens { get; set; }

        public ArrowWingFeature(DiagonalType LineDirection = DiagonalType.NONE, Direction ArrowDirection = Direction.RANDOM, AddTokenMethod AddMethod = AddTokenMethod.ONLY_TERRAIN, bool ReplaceTokens = false)
        {
            this.LineDirection = LineDirection;
            this.ArrowDirection = ArrowDirection;
            this.Name = "Double Arrow" + " " + LineDirection.ToString();
            this.Type = IngredientType.SMALLFEATURE;
            this.Token = TokenType.ARROW;

            this.AddMethod = AddMethod;
            this.ReplaceTokens = ReplaceTokens;
            this.Width = -1;
            this.Height = -1;
        }


        public void Build(GameBoard Board)
        {
            if (LineDirection == DiagonalType.NONE)
            {
                int R = Board.Random.RandomInteger(0, 1);
                if (R==0) LineDirection = DiagonalType.HIGHLEFT;
                else LineDirection = DiagonalType.HIGHRIGHT;
            }

            if (ArrowDirection == Direction.RANDOM) ArrowDirection = Board.Random.RandomDirection();
            Insert = Board.Random.RandomLocation(new BoardLocation(2, 2), Board.Rows - 4, Board.Columns - 4);
            int count = 3;
            for (int i = 0; i < count; i++)
            {
                BoardLocation l = new BoardLocation(Insert);
                switch (LineDirection)
                {
                    case DiagonalType.HIGHLEFT:
                        l.Row += i;
                        l.Column += i;
                        break;
                    case DiagonalType.HIGHRIGHT:
                        l.Row += i;
                        l.Column -= i;
                        break;
                 }

                Board.AddToken(new ArrowToken(ArrowDirection), l);
            }
        }
    }
}
