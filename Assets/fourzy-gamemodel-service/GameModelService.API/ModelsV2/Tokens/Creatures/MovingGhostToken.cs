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
                                            + Frequency.ToString();
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
                                            Frequency.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.CREATURE;
            }
        }

        public MovingGhostToken(Direction Orientation = Direction.NONE, MoveMethod MoveType = MoveMethod.CLOCKWISE, int Frequency = 1)
        {
            StandardTokenInit();

            this.Type = TokenType.MOVING_GHOST;
            this.MoveType = MoveType;
            this.Orientation = Orientation;
            this.Countdown = 0;
            this.Frequency = Frequency;

            this.pieceCanEnter = true;
            this.pieceMustStopOn = false;
            this.pieceCanEndMoveOn = false;
            this.adjustMomentum = 0;
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
                Frequency= int.Parse(Notation[4].ToString());
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
            if (Orientation == Direction.NONE) Orientation = Space.Random.RandomDirection();
            int MaxLooks = 4;
            if (MoveType == MoveMethod.HORIZONTAL_PACE || MoveType == MoveMethod.VERTICAL_PACE)
                MaxLooks = 2;

            for (int i=0; i<MaxLooks; i++)
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
                Space.Parent.ContentsAt(Space.Location.Neighbor(Orientation)).AddToken(new MovingGhostToken(Orientation, MoveType, Frequency));
                Space.Parent.RecordGameAction(new GameActionTokenMovement(this, TransitionType.GHOST_MOVE, Space.Location, Target.Location));
                Space.RemoveTokens(TokenType.MOVING_GHOST);
                return true;
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

            if (MoveType == MoveMethod.COUNTERCLOCKWISE) 
                Space.Parent.RecordGameAction(new GameActionTokenRotation(this,TransitionType.GHOST_TURN, Rotation.COUNTER_CLOCKWISE,startOrientation,Orientation));
            else
                Space.Parent.RecordGameAction(new GameActionTokenRotation(this, TransitionType.GHOST_TURN, Rotation.CLOCKWISE, startOrientation,Orientation));
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
            if (Countdown++ % Frequency == 0)
            {
                Countdown = 0;
                Haunt();
            }
        }
    }
}
