using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class HoldSpellToken : IToken
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

        public int Countdown { get; set; }
        public int PlayerId { get; set; }

        public char TokenCharacter
        {
            get
            {
                return TokenConstants.Hold;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Hold.ToString() + "," + PlayerId + "," + Countdown;
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Hold.ToString() + ',' + PlayerId.ToString() + ',' + Countdown.ToString() };
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
                return false;
            }
        }

        public HoldSpellToken(int PlayerId, int Duration = SpellConstants.DefaultHoldDuration)
        {
            StandardTokenInit();
            this.PlayerId = PlayerId;
            this.Countdown = Duration;

            this.Type = TokenType.HOLD;
            this.pieceCanEnter = false;
            this.pieceCanEndMoveOn = false;
            this.adjustMomentum = 0;
        }

        public HoldSpellToken(string Notation)
        {
            StandardTokenInit();
            this.PlayerId = 0;
            this.Countdown = 4;
            string[] Parse = Notation.Split(',');
            if (Parse.Length > 1)
            {
                this.PlayerId = int.Parse(Parse[1]);
            }
            if (Parse.Length > 2)
            {
                this.Countdown = int.Parse(Parse[2]);
            }

            this.Type = TokenType.HOLD;
            this.pieceCanEnter = false;
            this.pieceCanEndMoveOn = false;
            this.adjustMomentum = 0;
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

        public void Fade()
        {
            if (Visible) Visible = false;
            Space.Parent.RecordGameAction(new GameActionTokenRemove(Space.Location, TransitionType.SPELL_FADE, this));
            Space.RemoveTokens(TokenType.HOLD);
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
            //if (PlayerId == this.PlayerId)
            //{
                Countdown--;
                if (Countdown == 0 && Visible) Fade();
            //}

            //If piece is removed for some effect, remove the hold as well.
            if (!Space.ContainsPiece) Fade();
        }
    }
}
