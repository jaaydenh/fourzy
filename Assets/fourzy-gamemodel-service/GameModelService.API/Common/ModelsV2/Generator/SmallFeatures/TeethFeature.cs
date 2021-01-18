using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class TeethFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public TokenType Token { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.ARROW_FOUR_SIDES; } }
        public LineDirection LineDirection { get; set; }
        public int Separation { get; set; }
        public int InsertLocation { get; set; }

        public TeethFeature(LineDirection LineDirection = LineDirection.NONE, int Separation = 0, int InsertLocation = 0)
        {
            this.InsertLocation = InsertLocation;
            this.Insert = new BoardLocation(InsertLocation, InsertLocation);
            this.LineDirection = LineDirection;
            this.Separation = Separation;
            this.Name = "Zipper Separation " + Separation + " " + LineDirection.ToString();
            this.Type = IngredientType.SMALLFEATURE;
            this.Token = TokenType.ARROW;
            //this.Width = Width;
            //this.Height = Height;
        }

        //Not perfect.  The insert location might not match the dot convention.
        public void Build(GameBoard Board)
        {
            if (InsertLocation == 0) InsertLocation = Board.Random.RandomInteger(1, Board.Rows - 3);
            if (LineDirection == LineDirection.NONE) LineDirection = (LineDirection)Board.Random.RandomInteger(0, 1);

            switch (LineDirection)
            {
                case LineDirection.HORIZONTAL:
                    Board.AddToken(new ArrowToken(Direction.DOWN), new SolidFullLinePattern(Board, new BoardLocation(InsertLocation, 0), LineType.HORIZONTAL).Locations);
                    Board.AddToken(new ArrowToken(Direction.UP), new SolidFullLinePattern(Board, new BoardLocation(InsertLocation+1+Separation, 0 ), LineType.HORIZONTAL).Locations);
                    break;

                case LineDirection.VERTICAL:
                    Board.AddToken(new ArrowToken(Direction.RIGHT), new SolidFullLinePattern(Board, new BoardLocation(0, InsertLocation), LineType.VERTICAL).Locations);
                    Board.AddToken(new ArrowToken(Direction.LEFT), new SolidFullLinePattern(Board, new BoardLocation(0, InsertLocation+1+Separation), LineType.VERTICAL).Locations);
                    break;
            }
        }
    }
}
