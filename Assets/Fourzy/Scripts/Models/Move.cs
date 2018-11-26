using System;

namespace Fourzy
{
    public class Move : IEquatable<Move>
    {
        public int location;
        public Position position;
        public Direction direction;
        public PlayerEnum player;
        public long timeStamp;

        public Move(Position position, Direction direction)
        {
            this.position = position;
            this.direction = direction;

            switch (direction)
            {
                case Direction.UP:
                    location = position.column;
                    break;
                case Direction.DOWN:
                    location = position.column;
                    break;
                case Direction.LEFT:
                    location = position.row;
                    break;
                case Direction.RIGHT:
                    location = position.row;
                    break;
                default:
                    break;
            }
        }

        public Move(Position position, Direction direction, PlayerEnum player)
        {
            this.position = position;
            this.direction = direction;
            this.player = player;

            switch (direction)
            {
                case Direction.UP:
                    location = position.column;
                    break;
                case Direction.DOWN:
                    location = position.column;
                    break;
                case Direction.LEFT:
                    location = position.row;
                    break;
                case Direction.RIGHT:
                    location = position.row;
                    break;
                default:
                    break;
            }
        }

        public Move(int location, Direction direction, PlayerEnum player)
        {
            this.player = player;
            InitMove(location, direction);
        }

        public Move(int location, Direction direction)
        {
            InitMove(location, direction);
        }

        public void InitMove(int location, Direction direction)
        {
            this.location = location;
            this.direction = direction;

            Position pos = new Position(0, 0);
            switch (direction)
            {
                case Direction.UP:
                    pos.column = location;
                    pos.row = Constants.numRows;
                    break;
                case Direction.DOWN:
                    pos.column = location;
                    pos.row = -1;
                    break;
                case Direction.LEFT:
                    pos.column = Constants.numColumns;
                    pos.row = location;
                    break;
                case Direction.RIGHT:
                    pos.column = -1;
                    pos.row = location;
                    break;
                default:
                    break;
            }

            this.position = pos;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (obj.GetType() != this.GetType()) return false;

            return this.Equals((Move)obj);
        }

        public bool Equals(Move other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.position.column.Equals(other.position.column) &&
                this.position.row.Equals(other.position.row) &&
                this.direction.Equals(other.direction);
        }

        public override int GetHashCode()
        {
            // Constant because equals tests mutable member.
            // This will give poor hash performance, but will prevent bugs.
            return 0;
        }

        public Move GetNextPosition()
        {
            Position nextPosition = new Position(0, 0);

            switch (direction)
            {
                case Direction.UP:
                    nextPosition.column = position.column;
                    nextPosition.row = position.row - 1;
                    break;
                case Direction.DOWN:
                    nextPosition.column = position.column;
                    nextPosition.row = position.row + 1;
                    break;
                case Direction.LEFT:
                    nextPosition.column = position.column - 1;
                    nextPosition.row = position.row;
                    break;
                case Direction.RIGHT:
                    nextPosition.column = position.column + 1;
                    nextPosition.row = position.row;
                    break;
                default:
                    break;
            }

            Move move = new Move(nextPosition, direction, player);
            return move;
        }
    }
}
