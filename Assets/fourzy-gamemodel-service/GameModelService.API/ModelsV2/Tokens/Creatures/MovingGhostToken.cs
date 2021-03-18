using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class MovingGhostToken : IToken
    {
        public int Layer { get; set; }
        public Direction Orientation { get; set; }
        public BoardSpace Space { get; set; }
        public bool Visible { get; set; }
        public bool Delete { get; set; }

        public bool pieceCanEnter { get; set; }
        public bool pieceMustStopOn { get; set; }
        public bool pieceCanEndMoveOn { get; set; }
        public bool isMoveable { get; set; }
        public int addFriction { get; set; }
        public int adjustMomentum { get; set; }
        public int setMomentum { get; set; }
        public bool changePieceDirection { get; set; }
        public TokenType Type { get; set; }
        public MoveMethod MoveType { get; set; }
        public int Frequency { get; set; }
        public int Countdown { get; set; }
        public bool Tired { get; set; }
        public TokenColor Color { get; set; }

        public char TokenCharacter
        {
            get
            {
                return TokenConstants.MovingGhost;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.MovingGhost.ToString()
                                            + TokenConstants.DirectionString(Orientation)
                                            + TokenConstants.MoveString(MoveType)
                                            + Countdown.ToString()
                                            + Frequency.ToString()
                                            + (Tired ? 1 : 0).ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.MovingGhost.ToString(),
                                            TokenConstants.DirectionString(Orientation),
                                            TokenConstants.MoveString(MoveType),
                                            Countdown.ToString(),
                                            Frequency.ToString(),
                                            (Tired ? 1 : 0).ToString()};
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.CREATURE;
            }
        }

        public bool HasDynamicFeature
        {
            get
            {
                return true;
            }
        }

        public int Complexity
        {
            get
            {
                return TokenConstants.COMPLEXITY_HARD;
            }
        }

        public bool DisruptsWin
        {
            get
            {
                return true;
            }
        }

        public MovingGhostToken(TokenColor Color, Direction Orientation)
        {
            StandardTokenInit();

            this.Type = TokenType.MOVING_GHOST;
            this.Orientation = Orientation;
            this.Countdown = 0;

            switch (Color)
            {
                case TokenColor.RED:
                    switch (Orientation)
                    {
                        case Direction.LEFT:
                        case Direction.RIGHT:
                            this.MoveType = MoveMethod.HORIZONTAL_PACE;
                            break;
                        case Direction.UP:
                        case Direction.DOWN:
                            this.MoveType = MoveMethod.VERTICAL_PACE;
                            break;
                    }
                    this.Frequency = 1;
                    break;

                case TokenColor.BLUE:
                    this.MoveType = MoveMethod.WRAPAROUND;
                    this.Frequency = 1;
                    break;

                case TokenColor.YELLOW:
                    this.MoveType = MoveMethod.CLOCKWISE;
                    this.Frequency = 1;
                    break;

                case TokenColor.GREEN:
                    this.MoveType = MoveMethod.COUNTERCLOCKWISE;
                    this.Frequency = 1;
                    break;

                case TokenColor.ORANGE:
                    this.MoveType = MoveMethod.RING_CLOCKWISE;
                    this.Frequency = 1;
                    break;

                case TokenColor.BROWN:
                    this.MoveType = MoveMethod.RING_COUNTERCLOCKWISE;
                    this.Frequency = 1;
                    break;
            }

            this.Tired = true;
            this.pieceCanEnter = true;
            this.pieceMustStopOn = false;
            this.pieceCanEndMoveOn = false;
            this.adjustMomentum = 0;
        }

        public MovingGhostToken(Direction Orientation = Direction.NONE, MoveMethod MoveType = MoveMethod.RANDOM, int Frequency = 1, bool Tired = true)
        {
            StandardTokenInit();

            this.Type = TokenType.MOVING_GHOST;
            this.MoveType = MoveType;
            this.Orientation = Orientation;
            this.Countdown = Frequency;
            this.Frequency = Frequency;
            this.Tired = Tired;

            this.pieceCanEnter = true;
            this.pieceMustStopOn = false;
            this.pieceCanEndMoveOn = false;
            this.adjustMomentum = 0;

            //Not sure if I like this here. 
            if (MoveType == MoveMethod.RANDOM)
            {
                MoveType = new RandomTools().RandomMoveMethod();
            }

            switch (MoveType)
            {
                case MoveMethod.COUNTERCLOCKWISE:
                    Color = TokenColor.GREEN;
                    break;
                case MoveMethod.CLOCKWISE:
                    Color = TokenColor.YELLOW;
                    break;
                case MoveMethod.HORIZONTAL_PACE:
                case MoveMethod.VERTICAL_PACE:
                    Color = TokenColor.RED;
                    break;
                case MoveMethod.WRAPAROUND:
                    Color = TokenColor.BLUE;
                    break;
                case MoveMethod.RING_COUNTERCLOCKWISE:
                    Color = TokenColor.BROWN;
                    break;
                case MoveMethod.RING_CLOCKWISE:
                    Color = TokenColor.ORANGE;
                    break;
            }


        }

        public MovingGhostToken(string Notation)
        {
            StandardTokenInit();

            this.Type = TokenType.MOVING_GHOST;
            this.pieceCanEnter = true;
            this.pieceMustStopOn = false;
            this.pieceCanEndMoveOn = false;
            this.adjustMomentum = 0;
            this.Countdown = 0;
            this.Frequency = 1;

            if (Notation.Length > 1)
            {
                this.Orientation = TokenConstants.IdentifyDirection(Notation[1].ToString());
            }
            else
            {
                this.Orientation = Direction.DOWN;
            }

            if (Notation.Length > 2)
            {
                this.MoveType = TokenConstants.IdentifyMoveMethod(Notation[2].ToString());
            }
            else
            {
                this.MoveType = MoveMethod.CLOCKWISE;
            }

            if (Notation.Length > 3)
            {
                Countdown = int.Parse(Notation[3].ToString());
            }

            if (Notation.Length > 4)
            {
                Frequency = int.Parse(Notation[4].ToString());
            }

            if (Notation.Length > 5)
            {
                Tired = (Notation[5].ToString() == "1" ? true : false);
            }
            else
            {
                Tired = true;
            }

            switch (MoveType)
            {
                case MoveMethod.COUNTERCLOCKWISE:
                    Color = TokenColor.GREEN;
                    break;
                case MoveMethod.CLOCKWISE:
                    Color = TokenColor.YELLOW;
                    break;
                case MoveMethod.HORIZONTAL_PACE:
                case MoveMethod.VERTICAL_PACE:
                    Color = TokenColor.RED;
                    break;
                case MoveMethod.WRAPAROUND:
                    Color = TokenColor.BLUE;
                    break;
                case MoveMethod.RING_COUNTERCLOCKWISE:
                    Color = TokenColor.BROWN;
                    break;
                case MoveMethod.RING_CLOCKWISE:
                    Color = TokenColor.ORANGE;
                    break;
            }
        }

        public void StandardTokenInit()
        {
            this.Layer = -1;
            this.Visible = true;
            this.Delete = false;
            this.pieceCanEnter = true;
            this.pieceMustStopOn = false;
            this.pieceCanEndMoveOn = true;
            this.isMoveable = false;
            this.addFriction = 0;
            this.adjustMomentum = -1;
            this.setMomentum = -1;
            this.changePieceDirection = false;
            this.Space = null;
            this.Orientation = Direction.NONE;
        }

        public void Haunt()
        {
            if (Tired) return;
            if (Space.ContainsHex) return;
            Tired = true;
            if (Orientation == Direction.NONE) Orientation = Space.Random.RandomDirection();
            int MaxLooks = 4;


            //SPECIAL CODE FOR RING MOVE.
            if (MoveType == MoveMethod.RING_CLOCKWISE || MoveType == MoveMethod.RING_COUNTERCLOCKWISE)
            {
                List<BoardLocation> Targets = new List<BoardLocation>() { };
                if (MoveType == MoveMethod.RING_CLOCKWISE)
                    Targets = Space.Location.Ring(Space.Parent, Rotation.CLOCKWISE);
                if (MoveType == MoveMethod.RING_COUNTERCLOCKWISE)
                    Targets = Space.Location.Ring(Space.Parent, Rotation.COUNTER_CLOCKWISE);

                List<BoardLocation> token_movements = new List<BoardLocation>();
                bool Found = false;
                BoardSpace Target = null;
                foreach (BoardLocation l in Targets)
                {
                    Target = Space.Parent.ContentsAt(l);
                    token_movements.Add(l);
                    if (Target.ContainsPiece) continue;
                    if (!Target.TokensAllowEndHere && !Target.ContainsTokenType(TokenType.MOVING_GHOST)) continue;
                    if (Target.ContainsHex) continue;
                    Found = true;
                    break;
                }
                if (Found)
                {
                    MovingGhostToken t = new MovingGhostToken(Orientation, MoveType, Frequency, true);
                    BoardLocation last = Space.Location;

                    //Record each step.  If going around a corner, the client won't know how to handle the move without these hints.
                    foreach (BoardLocation l in token_movements)
                    {
                        MovingGhostToken n = new MovingGhostToken(Orientation, MoveType, Frequency, true);

                        //Space.Parent.RecordGameAction(new GameActionTokenMovement(n, TransitionType.GHOST_MOVE, last, l));
                        last = l;
                    }
                    MovingGhostToken newt = new MovingGhostToken(Orientation, MoveType, Frequency, true);
                    Space.Parent.RecordGameAction(new GameActionTokenMovement(newt, TransitionType.GHOST_MOVE, Space.Location, last));

                    Target.AddToken(t);
                    t.Space = Target;
                    Space.RemoveOneToken(TokenType.MOVING_GHOST);
                }
                return;
            }



            if (MoveType == MoveMethod.HORIZONTAL_PACE || MoveType == MoveMethod.VERTICAL_PACE)
                MaxLooks = 2;
            if (MoveType == MoveMethod.WRAPAROUND) MaxLooks = 1;

            for (int i = 0; i < MaxLooks; i++)
            {
                if (Move()) break;
                else Turn();
            }

        }

        public bool Move()
        {
            List<BoardLocation> PossibleTargets = Space.Location.Look(Space.Parent, Orientation);

            bool Found = false;
            BoardSpace Target = null;

            foreach (BoardLocation l in PossibleTargets)
            {
                Target = Space.Parent.ContentsAt(l);
                if (Target.ContainsPiece) continue;
                if (!Target.TokensAllowEndHere) continue;
                if (Target.ContainsHex) continue;
                Found = true;
                break;
            }

            if (Found)
            {
                MovingGhostToken t = new MovingGhostToken(Orientation, MoveType, Frequency, true);
                Target.AddToken(t);
                t.Space = Target;
                Space.Parent.RecordGameAction(new GameActionTokenMovement(t, TransitionType.GHOST_MOVE, Space.Location, Target.Location));
                Space.RemoveTokens(TokenType.MOVING_GHOST);
                return true;
            }
            //Try for wrapping.
            else if (MoveType == MoveMethod.WRAPAROUND)
            {
                switch (Orientation)
                {
                    case Direction.LEFT:
                        PossibleTargets = (new BoardLocation(Space.Location.Row, Space.Parent.Rows - 1)).Look(Space.Parent, Direction.LEFT);
                        break;
                    case Direction.RIGHT:
                        PossibleTargets = (new BoardLocation(Space.Location.Row, 0)).Look(Space.Parent, Direction.RIGHT);
                        break;
                    case Direction.UP:
                        PossibleTargets = (new BoardLocation(Space.Parent.Columns - 1, Space.Location.Column)).Look(Space.Parent, Direction.UP);
                        break;
                    case Direction.DOWN:
                        PossibleTargets = (new BoardLocation(0, Space.Location.Column)).Look(Space.Parent, Direction.DOWN);
                        break;
                }
                foreach (BoardLocation l in PossibleTargets)
                {
                    Target = Space.Parent.ContentsAt(l);
                    if (Target.ContainsPiece) continue;
                    if (!Target.TokensAllowEndHere) continue;
                    if (Target.ContainsHex) continue;
                    Found = true;
                    break;
                }
                if (Found)
                {
                    MovingGhostToken t = new MovingGhostToken(Orientation, MoveType, Frequency, true);
                    Target.AddToken(t);
                    t.Space = Target;
                    Space.Parent.RecordGameAction(new GameActionTokenMovement(t, TransitionType.GHOST_WRAPAROUND, Space.Location, Target.Location));
                    Space.RemoveTokens(TokenType.MOVING_GHOST);
                    return true;
                }
            }

            return false;
        }


        public void Turn()
        {
            Direction startOrientation = Orientation;
            switch (MoveType)
            {
                case MoveMethod.CLOCKWISE:
                    Orientation = BoardLocation.Clockwise(Orientation);
                    break;
                case MoveMethod.COUNTERCLOCKWISE:
                    Orientation = BoardLocation.CounterClockwise(Orientation);
                    break;
                case MoveMethod.HORIZONTAL_PACE:
                    Orientation = BoardLocation.Reverse(Orientation);
                    break;
                case MoveMethod.VERTICAL_PACE:
                    Orientation = BoardLocation.Reverse(Orientation);
                    break;
            }

            //if (MoveType == MoveMethod.COUNTERCLOCKWISE) 
            //    Space.Parent.RecordGameAction(new GameActionTokenRotation(this,TransitionType.GHOST_TURN, Rotation.COUNTER_CLOCKWISE,startOrientation,Orientation));
            //else
            //Space.Parent.RecordGameAction(new GameActionTokenRotation(this, TransitionType.GHOST_TURN, Rotation.CLOCKWISE, startOrientation,Orientation));
        }

        public Direction GetDirection(MovingPiece Piece)
        {
            return Direction.NONE;
        }

        public void ApplyElement(ElementType Element)
        {

        }

        public void EndOfTurn(int PlayerId)
        {
            Tired = false;
        }

        public void PieceEntersBoard(MovingPiece Piece)
        {

        }

        public void PieceEntersSpace(MovingPiece Piece)
        {

        }

        public void PieceLeavesSpace(MovingPiece Piece)
        {
        }

        public void PieceBumpsIntoLocation(MovingPiece Piece, BoardLocation Location)
        {
        }

        public void PieceStopsOnSpace(MovingPiece Piece)
        {
        }

        public string Print()
        {
            return "";
        }

        public void StartOfTurn(int PlayerId)
        {
            if (Tired) return;
            if (Countdown++ % Frequency == 0)
            {
                Countdown = 0;
                Haunt();
            }
        }
    }
}
