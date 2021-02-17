using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class GlassBlockToken : IToken
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

        public bool Broken { get; set; }

        public char TokenCharacter { get; set; }

        public string Notation
        {
            get
            {
                return TokenConstants.GlassBlock.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.GlassBlock.ToString() };
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

        public GlassBlockToken()
        {
            //StandardTokenProperties
            StandardTokenInit();

            //Show the Properties that matter.
            this.Broken = false;
            this.Visible = true;
            this.Type = TokenType.GLASS_BLOCK;
            this.TokenCharacter = 'O';
            this.pieceCanEnter = false;
            this.pieceCanEndMoveOn = false;
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

        //If bumped an ice block will transition from a block of ice into a flat sheet of ice.
        public void Crumble()
        {
            Broken = true;
            Visible = false;
            pieceCanEnter = true;
            pieceCanEndMoveOn = true;
            Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.BLOCK_ICE, this, new IceToken()));

            //Do we want to add and remove tokens, or just change properties??
            Space.AddToken(new IceToken());
            Space.RemoveTokens(TokenType.ICE_BLOCK);
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
            //Piece.Direction = Orientation;
        }

        public void PieceLeavesSpace(MovingPiece Piece)
        {
        }

        public void PieceBumpsIntoLocation(MovingPiece Piece, BoardLocation Location)
        {
            if (!Broken && Location.Equals(Space.Location))
            {
                Crumble();
            }
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
