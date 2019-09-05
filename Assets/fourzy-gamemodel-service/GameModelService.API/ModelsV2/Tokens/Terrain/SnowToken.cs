using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class SnowToken : IToken
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

        //How Deep is the Snow?? Might have some deep snow??
        public int Depth { get; set; }

        public char TokenCharacter
        {
            get
            {
                return TokenConstants.Snow;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Snow.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Snow.ToString() };
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
                return TokenConstants.COMPLEXITY_MEDIUM;
            }
        }

        public SnowToken(int Depth = 1)
        {
            StandardTokenInit();
            this.Depth = Depth;                
            this.Type = TokenType.SNOW;
            this.isMoveable = true;
            this.addFriction = 50;
        }

        public SnowToken(string Notation)
        {
            StandardTokenInit();
            this.Type = TokenType.SNOW;
            this.isMoveable = true;
            this.addFriction = 50;

            if (Notation.Length > 1)
            {
                this.Depth = int.Parse(Notation[1].ToString());
            }
            else
            {
                this.Depth = 1;
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

        public void Slide()
        {
            Depth--;
            if (Depth ==0)
            {
                Visible = false;
                Delete = true;
                pieceMustStopOn = false;
                addFriction = 0;
                Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.SNOW_ICE, this, new IceToken()));

                //Do we want to add and remove tokens, or just change properties??
                Space.AddToken(new IceToken());
            }

        }

        public void Melt()
        {
            Visible = false;
            Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.SNOW_MELT, this, new WaterToken()));

            //Do we want to add and remove tokens, or just change properties??
            Space.AddToken(new WaterToken());
            Space.RemoveTokens(TokenType.SNOW);
        }

        public void ApplyElement(ElementType Element)
        {
            switch (Element) {
                case ElementType.FIRE:
                    Melt();
                    break;
            }

        }

        public Direction GetDirection(MovingPiece Piece)
        {
            return Piece.Direction;
        }


        public void EndOfTurn(int TurnCount)
        {
        }

        public void PieceEntersBoard(MovingPiece Piece)
        {
        }

        public void PieceEntersSpace(MovingPiece Piece)
        {
            if (Piece.Location.Equals(Space.Location)) Slide();
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

        public void StartOfTurn(int TurnCount)
        {
        }
    }
}
