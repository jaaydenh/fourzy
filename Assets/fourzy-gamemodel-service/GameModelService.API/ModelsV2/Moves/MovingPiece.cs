namespace FourzyGameModel.Model
{
    public class MovingPiece
    {
        public Piece Piece { get; set; }
        public BoardLocation Location { get; set; }
        public Direction Direction { get; set; }
        public int Momentum { get; set; }
        public float Friction { get; set; }
        public bool Pushed { get; set; }

        public string UniqueId
        {
            get { return Piece.UniqueId; }
        }

        public int PlayerId
        {
            get { return Piece.PlayerId; }
        }

        public string HerdId
        {
            get { return Piece.HerdId; }
        }

        public PieceType Type
        {
            get { return Piece.PieceType; }
        }

        #region "Constructors"
       

        public MovingPiece(SimpleMove Move, GameBoard Board)
        {
            Piece = Move.Piece;
            Direction = Move.Direction;
            Location = new BoardLocation(Move.Direction, Move.Location, Board.Rows, Board.Columns);
            Momentum = Board.Random.RandomInteger(Constants.MinimumMomentum, Constants.MaximumMomentum);
            this.Friction = 0;
            this.Pushed = false;
        }

        public MovingPiece(Piece Piece, BoardLocation Location, Direction Direction, int Momentum)
        {
            this.Piece = Piece;
            this.Direction = Direction;
            this.Momentum = Momentum;
            this.Friction = 0;
            this.Location = Location;
            this.Pushed = false;
        }

        public MovingPiece(MovingPiece PieceToCopy)
        {
            Piece = PieceToCopy.Piece;
            Direction = PieceToCopy.Direction;
            Momentum = PieceToCopy.Momentum;
            Friction = PieceToCopy.Friction;
            Location = PieceToCopy.Location;
            Pushed = PieceToCopy.Pushed;
        }

        #endregion

        public int ConditionCount(PieceConditionType Condition)
        {
            return Piece.ConditionCount(Condition);
        }

        public bool HasCondition(PieceConditionType Condition)
        {
            return Piece.HasCondition(Condition);
        }
        
        public void AddCondition(PieceConditionType Condition, int Count =1)
        {
            for (int i=0; i<Count;i++)
                Piece.Conditions.Add(Condition);
        }

        public void RemoveOneCondition(PieceConditionType Condition)
        {
            //int del_index = -1;

            //for (int i = 0; i < Piece.Conditions.Count; i++)
            //{
            //    if (Piece.Conditions[i] == Condition) { del_index = i; break; }
            //}

            //if (del_index > 0) Piece.Conditions.RemoveAt(del_index);
            int count = ConditionCount(Condition);
            Piece.Conditions.RemoveAll(x => x == Condition);
            //if more than one condition, add back the missing ones.
            if (count > 1) for (int i = 0; i < count - 1; i++) Piece.Conditions.Add(Condition);
        }

        public void RemoveCondition(PieceConditionType Condition)
        {
            //foreach (PieceConditionType c in Piece.Conditions)
            //    if (c.Equals(Condition)) { Piece.Conditions.Remove(c); }

            for (int i = 0; i < Piece.Conditions.Count; i++)
            {
                if (Piece.Conditions[i] == Condition) { Piece.Conditions.RemoveAt(i); }
            }

            Piece.Conditions.RemoveAll(x => x == Condition);

        }

    }
}
