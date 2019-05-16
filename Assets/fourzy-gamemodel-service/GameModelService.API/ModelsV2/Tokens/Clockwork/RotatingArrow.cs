using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class RotatingArrowToken : IToken
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

        public Rotation RotationDirection { get; set; }
        public int Frequency { get; set; }
        private int CountDown { get; set; }

        public char TokenCharacter
        {
            get
            {
                switch (this.Orientation)
                {
                    case Direction.UP:
                        return TokenConstants.UpArrowTokenChararacter;
                    case Direction.DOWN:
                        return TokenConstants.DownArrowTokenChararacter;
                    case Direction.LEFT:
                        return TokenConstants.LeftArrowTokenChararacter;
                    case Direction.RIGHT:
                        return TokenConstants.RightArrowTokenChararacter;
                }
                return ' ';
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.RotatingArrow.ToString() 
                     + TokenConstants.DirectionString(Orientation) 
                     + TokenConstants.NotateRotation(RotationDirection)
                     + Frequency
                    +CountDown; 


            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.RotatingArrow.ToString(),
                                            TokenConstants.DirectionString(Orientation),
                                            TokenConstants.NotateRotation(RotationDirection),
                                            Frequency.ToString(),
                                            CountDown.ToString()};
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.INSTRUCTION;
            }
        }


        public RotatingArrowToken(Direction Orientation, Rotation Direction, int Frequency = 1)
        {
            StandardTokenInit();
            
            this.Orientation = Orientation;
            this.RotationDirection = Direction;
            this.Frequency = Frequency;

            this.Type = TokenType.ROTATING_ARROW;
            this.changePieceDirection = true;
            this.CountDown = Frequency;
        }

        public RotatingArrowToken(string Notation)
        {
            StandardTokenInit();

            if (Notation.Length > 1)
            {
                this.Orientation = TokenConstants.GetOrientation(Notation[1]);
            }


            if (Notation.Length > 2)
            {
                this.RotationDirection = TokenConstants.GetRotation(Notation[2]);
            }

            if (Notation.Length > 3)
            {
                this.Orientation = TokenConstants.GetOrientation(Notation[3]);
            }

            if (Notation.Length > 4)
            {
                this.Frequency = int.Parse(Notation[4].ToString());
            }
            else
            {
                this.Frequency = 1;
            }

            if (Notation.Length > 5)
            {
                this.CountDown = int.Parse(Notation[5].ToString());
            }
            else
            {
                this.CountDown = Frequency;
            }

            this.Type = TokenType.ROTATING_ARROW;
            this.changePieceDirection = true;
        }

        public void StandardTokenInit()
        {
            this.Layer = -1;
            this.Orientation = Direction.NONE;
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
        }

        public void TurnGears()
        {
            CountDown--;
            if (CountDown == 0)
            {
                Direction InitialOrientation = Orientation;
                Orientation = BoardLocation.Rotate(Orientation, RotationDirection);
                Space.Parent.RecordGameAction(new GameActionTokenRotation(this, TransitionType.ROTATE_ARROW, RotationDirection, InitialOrientation,Orientation));

                CountDown = Frequency;
            }

        }

        public Direction GetDirection(MovingPiece Piece)
        {
            return Orientation;
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
            TurnGears();
        }
    }
}
