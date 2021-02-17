using System;

namespace FourzyGameModel.Model
{
    public static class MoveFactory
    {
          //change a string like "U2" to Move(Up, 2)
          public static SimpleMove CreateSimpleMoveFromNotation(Piece Piece, string MoveNotation)
          {
            int Location = -1;
            Direction Direction = Direction.NONE;
         
            switch (MoveNotation.Substring(0, 1))
            {
                case "U":
                    Direction = Direction.UP;
                    break;
                case "D":
                    Direction = Direction.DOWN;
                    break;
                case "L":
                    Direction = Direction.LEFT;
                    break;
                case "R":
                    Direction = Direction.RIGHT;
                    break;
                 default:
                    throw new Exception("Could Not Parse Move Notation");
            }
            Location = int.Parse(MoveNotation.Substring(1, 2));

            return new SimpleMove(Piece, Direction, Location);
          }

    }
}
