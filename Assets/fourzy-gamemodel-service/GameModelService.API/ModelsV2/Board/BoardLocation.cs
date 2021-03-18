using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public struct BoardLocation : IEquatable<BoardLocation>
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public bool OddRow { get { return Row % 2 == 1; } }
        public bool EvenRow { get { return Row % 2 == 0; } }
        public bool OddColumn { get { return Column % 2 == 1; } }
        public bool EvenColumn { get { return Column % 2 == 0; } }
       
        #region "Initializer'
        public BoardLocation(BoardLocation Location)
        {
            this.Row = Location.Row;
            this.Column = Location.Column;
        }

        public BoardLocation(int Row, int Column)
        {
            this.Row = Row;
            this.Column = Column;
        }

        public BoardLocation(SimpleMove Move, GameBoard Board)
        {
            switch (Move.Direction)
            {
                case Direction.UP:
                    this.Row = Board.Rows;
                    this.Column = Move.Location;
                    break;
                case Direction.DOWN:
                    this.Row = -1;
                    this.Column = Move.Location;
                    break;
                case Direction.LEFT:
                    this.Row = Move.Location;
                    this.Column = Board.Columns;
                    break;
                case Direction.RIGHT:
                    this.Row = Board.Columns;
                    this.Column = -1;
                    break;
                default:
                    this.Row = 0;
                    this.Column = 0;
                    break;
            }
        }

        public BoardLocation(Direction direction, int location, int NumRows, int NumCols)
        {
            switch (direction)
            {
                case Direction.UP:
                    this.Row = NumRows -1;
                    this.Column = location;
                    break;
                case Direction.DOWN:
                    this.Row = 0;
                    this.Column = location;
                    break;
                case Direction.LEFT:
                    this.Row = location;
                    this.Column = NumCols -1;
                    break;
                case Direction.RIGHT:
                    this.Row = location;
                    this.Column = 0;
                    break;
                default:
                    this.Row = 0;
                    this.Column = 0;
                    break;
            }
        }
        #endregion
        
        #region "Equality"

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            if (((BoardLocation)obj).Row == this.Row && ((BoardLocation)obj).Column == this.Column) return true;
            return false;
        }

        public bool Equals(BoardLocation other)
        {
            if (other.Row == this.Row && other.Column == this.Column) return true;
            return false;
        }

        public override int GetHashCode()
        {
            //I don't have a reference to the board and how many rows and columns
            //Don't want to use a constant so we can have dynamic sized boards eventually.
            //I don't anticipate a board reaching 100, so this should return a unique value.
            return Row * 100 + Column ;
        }

        public static BoardLocation operator - (BoardLocation left, BoardLocation right)
        {
            return new BoardLocation(left.Row - right.Row, left.Column - right.Column);
        }

        public static BoardLocation operator + (BoardLocation left, BoardLocation right)
        {
            return new BoardLocation(left.Row + right.Row, left.Column + right.Column);
        }
        #endregion


        public string Print()
        {
            return string.Format("r:{0}, c:{1}", Row, Column); 
        }

        [JsonIgnore]
        public BoardLocation Left { get { return Neighbor(Direction.LEFT); } }
        [JsonIgnore]
        public BoardLocation Right { get { return Neighbor(Direction.RIGHT); } }
        [JsonIgnore]
        public BoardLocation Up { get { return Neighbor(Direction.UP); } }
        [JsonIgnore]
        public BoardLocation Down { get { return Neighbor(Direction.DOWN); } }

        public BoardLocation Neighbor(Direction Direction, int Distance = 1)
        {
            BoardLocation l = new BoardLocation(Row, Column);
            switch (Direction)
            {
                case Direction.UP:
                    l = new BoardLocation(Row - Distance, Column);
                    break;
                case Direction.DOWN:
                    l = new BoardLocation(Row + Distance, Column);
                    break;
                case Direction.LEFT:
                    l = new BoardLocation(Row, Column - Distance);
                    break;
                case Direction.RIGHT:
                    l = new BoardLocation(Row, Column + Distance);
                    break;
            }
            return l;
        }

        public BoardLocation Wrap(GameBoard Board)
        {
                if (Row < 0) Row = Board.Rows - 1;
                if (Row >= Board.Rows) Row = 0;
                if (Column < 0) Column = Board.Columns - 1;
                if (Column >= Board.Columns) Column = 0;

                return new BoardLocation(Row, Column);
        }

        public List<BoardLocation> Ring(GameBoard Board, Rotation Direction)
        {
            List<BoardLocation> Ring = new List<BoardLocation>() { };

            BoardLocation eval = new BoardLocation(this);

            int colval = -1;
            if (eval.Column < Board.Columns / 2)
                colval = eval.Column;
            else colval = Board.Columns - eval.Column -1;

            int rowval = -1;
            if (eval.Row < Board.Rows / 2)
                rowval = eval.Row ;
            else rowval = Board.Rows - eval.Row -1;

            int ringval = Math.Min(rowval, colval);

            int leftcol = ringval;
            int rightcol = Board.Columns - ringval -1;
            int toprow = ringval;
            int botrow = Board.Rows - ringval -1;

            do
            {
                if (Direction == Rotation.CLOCKWISE)
                {
                    if (eval.Column == leftcol)
                        if (eval.Row == toprow) eval.Column++;
                        else eval.Row--;
                    else if (eval.Column == rightcol)
                        if (eval.Row != botrow) eval.Row++;
                        else eval.Column--;
                    else if (eval.Row == toprow) eval.Column++;
                    else if (eval.Row == botrow) eval.Column--;
                }
                else
                {
                    if (eval.Column == leftcol)
                        if (eval.Row == botrow) eval.Column++;
                        else eval.Row++;
                    else if (eval.Column == rightcol)
                        if (eval.Row == toprow) eval.Column--;
                        else eval.Row--;
                    else if (eval.Row == toprow) eval.Column--;
                    else if (eval.Row == botrow) eval.Column++;
                }
                Ring.Add(new BoardLocation(eval));

            } while(!eval.Equals(this));

            return Ring;
        }


        public BoardLocation Neighbor(CompassDirection Direction, int Distance = 1)
        {
            switch (Direction)
            {
                case CompassDirection.N:
                    return new BoardLocation(Row - Distance, Column);
                case CompassDirection.NE:
                    return new BoardLocation(Row - Distance, Column + Distance);
                case CompassDirection.E:
                    return new BoardLocation(Row, Column + Distance);
                case CompassDirection.SE:
                    return new BoardLocation(Row + Distance, Column + Distance);
                case CompassDirection.S:
                    return new BoardLocation(Row + Distance, Column);
                case CompassDirection.SW:
                    return new BoardLocation(Row + Distance, Column - Distance);
                case CompassDirection.W:
                    return new BoardLocation(Row, Column - Distance);
                case CompassDirection.NW:
                    return new BoardLocation(Row - Distance, Column - Distance);
            }
            return new BoardLocation(Row, Column);
        }


        public List<BoardLocation> GetOrthogonals(GameBoard Board)
        {
            List<BoardLocation> Locations = new List<BoardLocation>();
            if (Neighbor(Direction.UP).OnBoard(Board)) Locations.Add(Neighbor(Direction.UP));
            if (Neighbor(Direction.DOWN).OnBoard(Board)) Locations.Add(Neighbor(Direction.DOWN));
            if (Neighbor(Direction.LEFT).OnBoard(Board)) Locations.Add(Neighbor(Direction.LEFT));
            if (Neighbor(Direction.RIGHT).OnBoard(Board)) Locations.Add(Neighbor(Direction.RIGHT));

            return Locations;
        }
        public List<BoardLocation> GetAdjacent(GameBoard Board)
        {
            List<BoardLocation> Locations = new List<BoardLocation>();
            for (int r=-1; r<=1; r++)
            {
                for (int c = -1; c <= 1; c++)
                {
                    if (r != 0 || c != 0)
                    {
                        BoardLocation l = new BoardLocation(Row + r, Column + c);
                        if (l.OnBoard(Board)) Locations.Add(l);
                    }
                }
            }
            return Locations;
        }

        public List<BoardLocation> GetAdjacentWithWrap(GameBoard Board)
        {
            List<BoardLocation> Locations = new List<BoardLocation>();
            for (int r = -1; r <= 1; r++)
            {
                for (int c = -1; c <= 1; c++)
                {
                    if (r != 0 || c != 0)
                    {
                        BoardLocation l = new BoardLocation(Row + r, Column + c);
                        if (l.Row < 0) l.Row = Board.Rows - 1;
                        if (l.Row >= Board.Rows) l.Row = 0;
                        if (l.Column < 0) l.Column = Board.Columns - 1;
                        if (l.Column >= Board.Columns) l.Column = 0;
                        Locations.Add(l);
                        //if (l.OnBoard(Board)) Locations.Add(l);
                    }
                }
            }
            return Locations;
        }

        public List<BoardLocation> GetRow(GameBoard Board)
        {
            List<BoardLocation> Locations = new List<BoardLocation>();
            for (int c = 0; c <= Board.Columns-1; c++)
            {
                BoardLocation l = new BoardLocation(Row, c);
                if (l.OnBoard(Board) && l.Column != Column) Locations.Add(l);
            }
                return Locations;
        }

        public List<BoardLocation> GetColumn(GameBoard Board)
        {
            List<BoardLocation> Locations = new List<BoardLocation>();
            for (int r = 0; r <= Board.Rows- 1; r++)
            {
                BoardLocation l = new BoardLocation(r, Column );
                if (l.OnBoard(Board) && l.Row != Row) Locations.Add(l);
            }
            return Locations;
        }

        //it's possible this could be more efficient, but this should be effective.
        public List<BoardLocation> GetDiagonal(GameBoard Board, DiagonalType Diagonal)
        {
            int DiagNumber = 0;
            switch (Diagonal)
            {
                case DiagonalType.HIGHLEFT:
                    DiagNumber = Row - Column;
                    break;
                case DiagonalType.HIGHRIGHT:
                    DiagNumber = Row + Column;
                    break;
            } 

            List<BoardLocation> Locations = new List<BoardLocation>();
            for (int r = 0; r <= Board.Rows - 1; r++)
            {
                for (int c = 0; c <= Board.Columns- 1; c++)
                {
                    int DiagCheck = 0;
                    switch (Diagonal)
                    {
                        case DiagonalType.HIGHLEFT:
                            DiagCheck = r - c;
                            break;
                        case DiagonalType.HIGHRIGHT:
                            DiagCheck = r + c;
                            break;
                    }
                    if (DiagCheck == DiagNumber && (r!=Row || c!=Column)) Locations.Add(new BoardLocation(r, c));
                }
            }
            return Locations;
        }



        public bool OnBoard(GameBoard Board)
        {
                if (Row >= 0 && Row < Board.Rows && Column >= 0 && Column < Board.Columns) return true;
                return false;
        }

        public bool IsCorner(GameBoard Board)
        {
            foreach (BoardLocation l in Board.Corners)
                if (l.Equals(this)) return true;
            return false;
        }
        
        public List<BoardLocation> Look(GameBoard Board,  Direction Direction)
        {
            List<BoardLocation> Locations = new List<BoardLocation>();
            BoardLocation l = new BoardLocation(this);

            l = l.Neighbor(Direction);
            while (l.OnBoard(Board))
            {
                Locations.Add(l);
                l = l.Neighbor(Direction);
            }

            return Locations;
        }

        public List<BoardLocation> GetLine(GameBoard Board, LineType Line)
        {
            //List<BoardLocation> Locations = new List<BoardLocation>();
            //BoardLocation l = new BoardLocation(this);

            switch (Line)
            {
                case LineType.HORIZONTAL:
                    return GetRow(Board);

                case LineType.VERTICAL:
                    return GetColumn(Board);

                case LineType.DIAGONAL:
                    return GetDiagonal(Board, DiagonalType.HIGHLEFT);
            }

  
            return new List<BoardLocation>() { };
        }


        public static Direction Reverse(Direction Orientation)
        {
            switch (Orientation)
            {
                case Direction.UP:
                    return Direction.DOWN;
                case Direction.DOWN:
                    return Direction.UP;
                case Direction.LEFT:
                    return Direction.RIGHT;
                case Direction.RIGHT:
                    return Direction.LEFT;
            }
            return Direction.NONE;
        }

        public static Direction Clockwise(Direction Orientation)
        {
            switch (Orientation)
            {
                case Direction.UP:
                    return Direction.RIGHT;
                case Direction.DOWN:
                    return Direction.LEFT;
                case Direction.LEFT:
                    return Direction.UP;
                case Direction.RIGHT:
                    return Direction.DOWN;
            }
            return Direction.NONE;
        }

        public static Direction CounterClockwise(Direction Orientation)
        {
            switch (Orientation)
            {
                case Direction.UP:
                    return Direction.LEFT;
                case Direction.DOWN:
                    return Direction.RIGHT;
                case Direction.LEFT:
                    return Direction.DOWN;
                case Direction.RIGHT:
                    return Direction.UP;
            }
            return Direction.NONE;
        }

        public static Direction Rotate(Direction Orientation, Rotation Rotation)
        {
            switch (Rotation)
            {
                case Rotation.CLOCKWISE:
                    return BoardLocation.Clockwise(Orientation);
                case Rotation.COUNTER_CLOCKWISE:
                    return BoardLocation.CounterClockwise(Orientation);
            }
            return Direction.NONE;
        }
    }
}
