using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ArrowCycleFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public List<TokenType> Tokens { get; }

        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.ARROW_FOUR_SIDES; } }
        public Rotation Rotation { get; set; }

        public ArrowCycleFeature(Rotation Rotation, BoardLocation Insert, int Width=4, int Height=4)
        {
            this.Insert = Insert;
            this.Rotation = Rotation;
            this.Name = "Arrow Cycle " + Rotation.ToString();
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.ARROW };
            this.Width = Width;
            this.Height = Height;
        }

        public ArrowCycleFeature()
        {
            this.Insert = new BoardLocation(0,0);
            this.Rotation = Rotation.NONE;
            this.Name = "Arrow Cycle " + Rotation.ToString();
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.ARROW };
            this.Width = 2;
            this.Height = 2;
        }


        //Not perfect.  The insert location might not match the dot convention.
        public void Build(GameBoard Board)
        {
            if (Insert.Row == 0 && Insert.Column == 0) Insert = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows - 2-Height, Board.Columns - 2-Width);
            if (Rotation == Rotation.NONE) Rotation = Board.Random.RandomRotation();
            
            if (Rotation == Rotation.CLOCKWISE)
            {
                int direction_index = 0;
                foreach (BoardLocation l in new CornerPattern(Board, Insert, Width, Height).Locations)
                {
                    switch (direction_index)
                    {
                        case 0:
                            Board.AddToken(new ArrowToken(Direction.RIGHT), l, AddTokenMethod.IF_NO_TOKEN_MATCH, true);
                            break;
                        case 1:
                            Board.AddToken(new ArrowToken(Direction.UP), l, AddTokenMethod.IF_NO_TOKEN_MATCH, true);
                            break;
                        case 2:
                            Board.AddToken(new ArrowToken(Direction.DOWN), l, AddTokenMethod.IF_NO_TOKEN_MATCH, true);
                            break;
                        case 3:
                            Board.AddToken(new ArrowToken(Direction.LEFT), l, AddTokenMethod.IF_NO_TOKEN_MATCH, true);
                            break;
                    }
                    direction_index++;
                }
            }
            else
            {
                int direction_index = 0;
                foreach (BoardLocation l in new CornerPattern(Board, Insert, Width, Height).Locations)
                {
                    switch (direction_index)
                    {
                        case 0:
                            Board.AddToken(new ArrowToken(Direction.DOWN), l, AddTokenMethod.IF_NO_TOKEN_MATCH, true);
                            break;
                        case 1:
                            Board.AddToken(new ArrowToken(Direction.RIGHT), l, AddTokenMethod.IF_NO_TOKEN_MATCH, true);
                            break;
                        case 2:
                            Board.AddToken(new ArrowToken(Direction.LEFT), l, AddTokenMethod.IF_NO_TOKEN_MATCH, true);
                            break;
                        case 3:
                            Board.AddToken(new ArrowToken(Direction.UP), l, AddTokenMethod.IF_NO_TOKEN_MATCH, true);
                            break;
                    }
                    direction_index++;
                }
            }

        }
    }
}
