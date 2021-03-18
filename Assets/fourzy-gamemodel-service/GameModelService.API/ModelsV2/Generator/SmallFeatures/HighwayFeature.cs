using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class HighwayFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public List<TokenType> Tokens { get; }


        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.ARROW_HIGHWAY; } }
        public LineDirection LineDirection { get; set; }
        public int Separation { get; set; }
        public int InsertLocation { get; set; }
        public bool Staggered { get; set; }
        public int NumberLanesPerDirection { get; set; }
        public bool TwoWay { get; set; }
        public AddTokenMethod AddMethod {get; set; }
        public bool ReplaceTokens { get; set; }

        public HighwayFeature(LineDirection LineDirection = LineDirection.NONE, int Separation = 0, int InsertLocation = 0, bool Staggered = true, int NumberLanesPerDirection = 1, bool TwoWay = true, AddTokenMethod AddMethod = AddTokenMethod.ONLY_TERRAIN, bool ReplaceTokens = false)
        {
            this.InsertLocation = InsertLocation;
            this.Insert = new BoardLocation(InsertLocation, InsertLocation);
            this.LineDirection = LineDirection;
            this.Staggered = Staggered;
            this.NumberLanesPerDirection = NumberLanesPerDirection;
            this.Separation = Separation;
            this.Name = "HighwaySeparation " + Separation + " " + LineDirection.ToString();
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.ARROW };
            this.TwoWay = TwoWay;
            this.AddMethod = AddMethod;
            this.ReplaceTokens = ReplaceTokens;
            //this.Width = Width;
            //this.Height = Height;
        }


        public void Build(GameBoard Board)
        {
            if (InsertLocation == 0) InsertLocation = Board.Random.RandomInteger(2, Board.Rows - 3);
            if (LineDirection == LineDirection.NONE) LineDirection = (LineDirection)Board.Random.RandomInteger(0, 1);

            //A random chance to have the first lane a particular direction.
            bool SwitchOrder = Board.Random.Chance(50);

            switch (LineDirection)
            {
             
                case LineDirection.HORIZONTAL:
                    Board.AddToken(new ArrowToken(SwitchOrder?Direction.LEFT:Direction.RIGHT), new DottedLinePattern(Board, new BoardLocation(InsertLocation, 0), LineType.HORIZONTAL).Locations, AddMethod, ReplaceTokens);
                    if (TwoWay)
                        Board.AddToken(new ArrowToken(SwitchOrder ? Direction.RIGHT: Direction.LEFT), new DottedLinePattern(Board, new BoardLocation(InsertLocation + 1 + Separation, this.Staggered?1:0), LineType.HORIZONTAL).Locations, AddMethod, ReplaceTokens);

                    if (NumberLanesPerDirection >= 2)
                    {
                        Board.AddToken(new ArrowToken(SwitchOrder ? Direction.LEFT : Direction.RIGHT), new DottedLinePattern(Board, new BoardLocation(InsertLocation-1, 0), LineType.HORIZONTAL).Locations, AddMethod, ReplaceTokens);
                        if (TwoWay)
                            Board.AddToken(new ArrowToken(SwitchOrder ? Direction.RIGHT : Direction.LEFT), new DottedLinePattern(Board, new BoardLocation(InsertLocation + 2 + Separation, this.Staggered ? 1 : 0), LineType.HORIZONTAL).Locations, AddMethod, ReplaceTokens);
                    }

                    break;

                case LineDirection.VERTICAL:
                    Board.AddToken(new ArrowToken(SwitchOrder ? Direction.UP: Direction.DOWN), new DottedLinePattern(Board, new BoardLocation(0, InsertLocation), LineType.VERTICAL).Locations, AddMethod, ReplaceTokens);
                    if (TwoWay)
                        Board.AddToken(new ArrowToken(SwitchOrder ? Direction.DOWN: Direction.UP), new DottedLinePattern(Board, new BoardLocation(this.Staggered ? 1 : 0, InsertLocation + 1 + Separation), LineType.VERTICAL).Locations, AddMethod, ReplaceTokens);

                    if (NumberLanesPerDirection >= 2)
                    {
                        Board.AddToken(new ArrowToken(SwitchOrder ? Direction.UP : Direction.DOWN), new DottedLinePattern(Board, new BoardLocation(0, InsertLocation-1), LineType.VERTICAL).Locations, AddMethod, ReplaceTokens);
                        if (TwoWay)
                            Board.AddToken(new ArrowToken(SwitchOrder ? Direction.DOWN : Direction.UP), new DottedLinePattern(Board, new BoardLocation(this.Staggered ? 1 : 0, InsertLocation + 2 + Separation), LineType.VERTICAL).Locations, AddMethod, ReplaceTokens);
                    }

                    break;
            }
        }
    }
}
