using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ArrowFourSidesFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public TokenType Token { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.ARROW_FOUR_SIDES; } }
        public Rotation Rotation { get; set; }

        public ArrowFourSidesFeature(BoardLocation Insert, Rotation Rotation)
        {
            this.Insert = Insert;
            this.Rotation = Rotation;
            this.Name = "Arrow Four Sides " + Rotation.ToString();
            this.Type = IngredientType.SMALLFEATURE;
            this.Token = Token;
            this.Width = Width;
            this.Height = Height;
        }

        //Not perfect.  The insert location might not match the dot convention.
        public void Build(GameBoard Board)
        {
            if (Rotation == Rotation.CLOCKWISE)
            {
                Board.AddToken(new ArrowToken(Direction.RIGHT), Insert.Neighbor(Direction.UP));
                Board.AddToken(new ArrowToken(Direction.DOWN), Insert.Neighbor(Direction.RIGHT));
                Board.AddToken(new ArrowToken(Direction.LEFT), Insert.Neighbor(Direction.DOWN));
                Board.AddToken(new ArrowToken(Direction.UP), Insert.Neighbor(Direction.LEFT));
            }
            else
            {
                Board.AddToken(new ArrowToken(Direction.LEFT), Insert.Neighbor(Direction.UP));
                Board.AddToken(new ArrowToken(Direction.UP), Insert.Neighbor(Direction.RIGHT));
                Board.AddToken(new ArrowToken(Direction.RIGHT), Insert.Neighbor(Direction.DOWN));
                Board.AddToken(new ArrowToken(Direction.DOWN), Insert.Neighbor(Direction.LEFT));
            }

        }
    }
}
