using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class MagicWaterToken : IToken
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
                return TokenConstants.MagicWater;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.MagicWater.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.MagicWater.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.TERRAIN;
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

        public MagicWaterToken(int PlayerId, int Duration = SpellConstants.DefaultMagicWaterDuration)
        {
            StandardTokenInit();
            this.PlayerId = PlayerId;
            this.Countdown = Duration;

            this.Type = TokenType.MAGICWATER;
            this.pieceMustStopOn = true;
            this.isMoveable = true;
            this.addFriction = 100;
            this.adjustMomentum = -2;
            this.setMomentum = 1;
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
            return Piece.Direction;
        }

        public void Fade()
        {
            if (Visible) Visible = false;
            Space.Parent.RecordGameAction(new GameActionTokenRemove(Space.Location, TransitionType.SPELL_FADE, this));
            Space.RemoveTokens(TokenType.MAGICWATER);
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

        public void EndOfTurn(int TurnCount)
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
            if (Piece.Location.Equals(Space.Location))
            {
                Piece.RemoveCondition(PieceConditionType.INERTIA);
                Piece.AddCondition(PieceConditionType.INERTIA);
            }
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

        public void StartOfTurn(int TurnCount)
        {
            //if (PlayerId == this.PlayerId)
            //{
                Countdown--;
                if (Countdown == 0 && Visible) Fade();
            //}

        }
    }
}
