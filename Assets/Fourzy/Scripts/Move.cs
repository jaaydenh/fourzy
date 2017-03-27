using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Fourzy {
    public class Move : IEquatable<Move> {

        public int location;
        public Position position;
        public Direction direction;

        public Move(Position position, Direction direction) {
            this.position = position;
            this.direction = direction;
        }

        public Move(int location, Direction direction) {
            this.location = location;
            this.direction = direction;

            Position pos = new Position(0,0);
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
            //Debug.Log("MOVE OVERRIDE EQUALS");
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (obj.GetType() != this.GetType()) return false;

            return this.Equals((Move)obj);
        }

        public bool Equals(Move other)
        {
            //Debug.Log("MOVE EQUALS");
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
    }
}
