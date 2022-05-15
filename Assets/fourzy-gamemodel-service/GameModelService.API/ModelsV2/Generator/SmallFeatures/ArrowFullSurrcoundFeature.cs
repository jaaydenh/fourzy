using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ArrowFullSurroundFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public List<TokenType> Tokens { get; }


        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.ARROW_FOUR_SIDES; } }
        public Rotation Rotation { get; set; }

        public ArrowFullSurroundFeature(BoardLocation Insert, Rotation Rotation)
        {
            this.Insert = Insert;
            this.Rotation = Rotation;
            this.Name = "Arrow Four Sides " + Rotation.ToString();
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.ARROW };

            this.Width = Width;
            this.Height = Height;
        }

        public ArrowFullSurroundFeature(Rotation Rotation)
        {
            this.Insert = new BoardLocation(0,0);
            this.Rotation = Rotation;
            this.Name = "Arrow Four Sides " + Rotation.ToString();
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenType.ARROW };

            this.Width = Width;
            this.Height = Height;
        }


        //Not perfect.  The insert location might not match the dot convention.
        public void Build(GameBoard Board)
        {

            BoardLocation execute = new BoardLocation(0, 0);

            if (Insert.Row == 0 && Insert.Column == 0)
            {
                execute = Board.Random.RandomLocation(new BoardLocation(3, 3), 2, 2);
            }
            else
            {
                execute = Insert;
            }

            if (Rotation == Rotation.CLOCKWISE)
            {
                Board.AddToken(new ArrowToken(Direction.DOWN), execute.Neighbor(CompassDirection.E));
                Board.AddToken(new ArrowToken(Direction.LEFT), execute.Neighbor(CompassDirection.SE));
                Board.AddToken(new ArrowToken(Direction.LEFT), execute.Neighbor(CompassDirection.S));
                Board.AddToken(new ArrowToken(Direction.UP), execute.Neighbor(CompassDirection.SW));
                Board.AddToken(new ArrowToken(Direction.UP), execute.Neighbor(CompassDirection.W));
                Board.AddToken(new ArrowToken(Direction.RIGHT), execute.Neighbor(CompassDirection.NW));
                Board.AddToken(new ArrowToken(Direction.RIGHT), execute.Neighbor(CompassDirection.N));
                Board.AddToken(new ArrowToken(Direction.DOWN), execute.Neighbor(CompassDirection.NE));
            }
            else
            {
                Board.AddToken(new ArrowToken(Direction.UP), execute.Neighbor(CompassDirection.E));
                Board.AddToken(new ArrowToken(Direction.UP), execute.Neighbor(CompassDirection.SE));
                Board.AddToken(new ArrowToken(Direction.RIGHT), execute.Neighbor(CompassDirection.S));
                Board.AddToken(new ArrowToken(Direction.RIGHT), execute.Neighbor(CompassDirection.SW));
                Board.AddToken(new ArrowToken(Direction.DOWN), execute.Neighbor(CompassDirection.W));
                Board.AddToken(new ArrowToken(Direction.DOWN), execute.Neighbor(CompassDirection.NW));
                Board.AddToken(new ArrowToken(Direction.LEFT), execute.Neighbor(CompassDirection.N));
                Board.AddToken(new ArrowToken(Direction.LEFT), execute.Neighbor(CompassDirection.NE));
            }

        }
    }
}
