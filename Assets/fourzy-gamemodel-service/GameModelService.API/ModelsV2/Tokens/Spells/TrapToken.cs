using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class TrapToken : IToken
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

        public int PlayerId { get; set; }
        public TokenType TrapType { get; set; }
        public bool Triggered { get; set; }
        
        public char TokenCharacter
        {
            get
            {
                return TokenConstants.Trap;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Trap.ToString() + "," + PlayerId + "," + TrapType.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Trap.ToString() + ',' + PlayerId.ToString() + ',' + TrapType.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.SPELL;
            }
        }

        public bool HasDynamicFeature
        {
            get
            {
                return false;
            }
        }

        public int Complexity
        {
            get
            {
                return TokenConstants.COMPLEXITY_NORMAL;
            }
        }

        public bool DisruptsWin
        {
            get
            {
                return true;
            }
        }

        public TrapToken(int PlayerId, TokenType TrapType)
        {
            StandardTokenInit();
            this.PlayerId = PlayerId;
            this.TrapType= TrapType;
            this.Triggered = false;

            this.Type = TokenType.TRAP;
        }

        public TrapToken(string Notation)
        {
            StandardTokenInit();
            this.PlayerId = 0;
          
            string[] Parse = Notation.Split(',');
            if (Parse.Length > 1)
            {
                this.PlayerId = int.Parse(Parse[1]);
            }
            if (Parse.Length > 2)
            {
                //this.Countdown = int.Parse(Parse[2]);
            }

            this.Type = TokenType.TRAP;
            this.Triggered = false;
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

        public void Reveal()
        {
            Triggered = true;
            Visible = false;
            IToken t = TokenFactory.Create(TrapType);
            Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.REVEAL_TRAP, this,t));

            //Do we want to add and remove tokens, or just change properties??
            Space.AddToken(t);
            Space.RemoveTokens(TokenType.TRAP);
        }
        
        public Direction GetDirection(MovingPiece Piece)
        {
            return Piece.Direction;
        }

        public void ApplyElement(ElementType Element)
        {
            switch (Element)
            {
                case ElementType.FIRE:
                    break;
                case ElementType.COLD:
                    break;
                case ElementType.WATER:
                    break;
                case ElementType.HEAT:
                    break;
            }
        }

        public void EndOfTurn(int PlayerId)
        {
        }

        public void PieceEntersBoard(MovingPiece Piece)
        {
        }

        public void PieceEntersSpace(MovingPiece Piece)
        {
            if (!Triggered && Piece.Location.Equals(Space.Location)) Reveal();
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
        }
    }
}
