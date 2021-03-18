using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class PopUpBlockerToken : IToken
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

        public char TokenCharacter { get; set; }

        public string Notation
        {
            get
            {
                return TokenConstants.PopUpBlocker.ToString()
                       + (Raised ? 1:0).ToString()
                       + Frequency.ToString()
                       + Countdown.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.PopUpBlocker.ToString(),
                                            (Raised ? 1:0).ToString(),
                                            Frequency.ToString(),
                                            Countdown.ToString()};
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.BLOCKER;
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
                return false;
            }
        }

        public int Frequency { get; set; }
        public int Countdown { get; set; }
        public bool Raised { get; set; }
        public bool DestroyPiece { get; set; }

        public PopUpBlockerToken(bool Raised = true, int Frequency = TokenConstants.PopUpBlockerFrequency)
        {

            StandardTokenInit();

            this.Type = TokenType.POPUP_BLOCKER;
            this.TokenCharacter = TokenConstants.PopUpBlocker;
            this.Countdown = Frequency;
            this.Frequency = Frequency;
            this.Raised = Raised;
            this.DestroyPiece = false;

            if (Raised)
            {
                this.pieceCanEnter = false;
                this.pieceCanEndMoveOn = false;
            }
        }

        public PopUpBlockerToken(string Notation)
        {

            StandardTokenInit();
            this.Type = TokenType.POPUP_BLOCKER;

            if (Notation.Length > 1)
            {
                this.Raised = Notation[1] == '1';
            }

            if (Notation.Length > 2)
            {
                this.Frequency = int.Parse(Notation[2].ToString());
            }

            if (Notation.Length > 3)
            {
                this.Countdown = int.Parse(Notation[3].ToString());
            }

            if (Raised)
            {
                this.pieceCanEnter = false;
                this.pieceCanEndMoveOn = false;
            }
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
        public Direction GetDirection(MovingPiece Piece)
        {
            return Direction.NONE;
        }

        public void TurnGears()
        {
            Countdown--;
            if (Countdown == 0)
            {
                if (Raised) Lower();
                else Raise();
                Countdown = Frequency;
            }
        }
        public void Raise()
        {
            if (Space.ContainsPiece )
            {
                if (DestroyPiece)
                {
                    Space.Parent.RecordGameAction(new GameActionDestroyed(Space.ActivePiece, Space.Location, DestroyType.CRUSHED_BY_BLOCKER));
                    Space.Pieces.Clear();
                } else
                {
                    //unless 'destroy' mode is activated, popup blocker will remain down.
                    return;
                }
            }

            Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.BLOCKER_RAISE, this, this));

            this.pieceCanEnter = false;
            this.pieceCanEndMoveOn = false;
            Raised = true;
        }

        public void Lower()
        {
            Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.BLOCKER_LOWER, this, this));

            this.pieceCanEnter = true;
            this.pieceCanEndMoveOn = true;
            Raised = false;
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
            //Piece.Direction = Orientation;
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
