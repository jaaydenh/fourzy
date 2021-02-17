using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ArrowPinWheelFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public TokenType Token { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.ARROW_FOUR_SIDES; } }
        public Rotation Rotation { get; set; }
        public int ArmLength { get; set; }
        public bool CenterOn { get; set; }

        public ArrowPinWheelFeature(Rotation Rotation, BoardLocation Insert, int ArmLength = 3, bool CenterOn = true)
        {
            this.Insert = Insert;
            this.Rotation = Rotation;
            this.ArmLength = ArmLength;
            this.CenterOn = CenterOn;
            this.Name = "Arrow PinWheel" + Rotation.ToString();
            this.Type = IngredientType.SMALLFEATURE;
            this.Token = TokenType.ARROW;
            this.Width = Width;
            this.Height = Height;
        }

        public ArrowPinWheelFeature()
        {
            this.Insert = new BoardLocation(0,0);
            this.Rotation =  Rotation.CLOCKWISE;
            this.ArmLength = 3;
            this.CenterOn = true;
            this.Name = "Arrow PinWheel" + Rotation.ToString();
            this.Type = IngredientType.SMALLFEATURE;
            this.Token = TokenType.ARROW;
            this.Width = Width;
            this.Height = Height;
        }


        //Not perfect.  The insert location might not match the dot convention.
        public void Build(GameBoard Board)
        {

            if (Insert.Row == 0 && Insert.Column == 0)
            {
                Insert = Board.Random.RandomLocation(new BoardLocation(2, 2), Board.Rows - 6, Board.Columns - 6);
            }

            int count = 0;
            foreach (BoardLocation l in new BoardLocation(Insert.Row+1,Insert.Column).Look(Board, Direction.UP))
            {
                if (count > 0 || CenterOn)
                   if (Rotation == Rotation.CLOCKWISE)
                       Board.AddToken(new ArrowToken(Direction.RIGHT), l);
                   else
                        Board.AddToken(new ArrowToken(Direction.LEFT), l);
                if (++count >= ArmLength) break;
            }

            count = 0;
            foreach (BoardLocation l in new BoardLocation(Insert.Row, Insert.Column).Look(Board, Direction.RIGHT))
            {
                if (count > 0 || CenterOn)
                    if (Rotation == Rotation.CLOCKWISE)
                        Board.AddToken(new ArrowToken(Direction.DOWN), l);
                    else
                        Board.AddToken(new ArrowToken(Direction.UP), l);
                if (++count >= ArmLength) break;
            }

            count = 0;
            foreach (BoardLocation l in new BoardLocation(Insert.Row, Insert.Column+1).Look(Board, Direction.DOWN))
            {
                if (count > 0 || CenterOn)
                    if (Rotation == Rotation.CLOCKWISE)
                        Board.AddToken(new ArrowToken(Direction.LEFT), l);
                    else
                        Board.AddToken(new ArrowToken(Direction.RIGHT), l);
                if (++count >= ArmLength) break;
            }

            count = 0;
            foreach (BoardLocation l in new BoardLocation(Insert.Row+1, Insert.Column+1).Look(Board, Direction.LEFT))
            {
                if (count > 0 || CenterOn)
                    if (Rotation == Rotation.CLOCKWISE)
                        Board.AddToken(new ArrowToken(Direction.UP), l);
                    else
                        Board.AddToken(new ArrowToken(Direction.DOWN), l);
                if (++count >= ArmLength) break;
            }

        }
    }
}
