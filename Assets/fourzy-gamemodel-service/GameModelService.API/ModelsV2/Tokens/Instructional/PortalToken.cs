using System.Collections.Generic;
using System.Linq;

namespace FourzyGameModel.Model
{
    public class PortalToken : IToken
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

        public TokenColor Color { get; set; }

        public char TokenCharacter
        {
            get
            {
                return TokenConstants.Portal;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Portal.ToString()
                       +TokenConstants.NotateColor(Color);
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Portal.ToString(),
                                            TokenConstants.NotateColor(Color)};
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.EFFECT;
            }
        }

        public PortalToken(TokenColor Color)
        {
            StandardTokenInit();

            this.Type = TokenType.PORTAL;
            this.Color = Color;
            this.pieceCanEnter = true;
            this.pieceMustStopOn = false;
            this.pieceCanEndMoveOn = true;
            this.changePieceDirection = true;
        }

        public PortalToken(string Notation)
        {
            if (Notation.Length > 1)
            {
                this.Color= TokenConstants.GetColor(Notation[1].ToString());
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

        public void Implode()
        {
            Space.Parent.RecordGameAction(new GameActionDestroyed(Space.ActivePiece, Space.Location, DestroyType.PORTAL));
            Space.Pieces.Clear();
            Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.PORTAL_IMPLODES, this, null));
            Space.RemoveTokens(TokenType.PORTAL);
        }

        public Direction GetDirection(MovingPiece Piece)
        {
            return Direction.NONE;
        }

        public void ApplyElement(ElementType Element)
        {
        }


        public void EndOfTurn(int TurnCount)
        {
        }

        public void PieceEntersBoard(MovingPiece Piece)
        {

        }

        public void PieceEntersSpace(MovingPiece Piece)
        {
            //Move Piece to new location
            //If one or more matching portal exists, move the piece to one of those locations and continue move

            if (!Piece.Location.Equals(Space.Location)) return;

            List<IToken> Portals = Space.Parent.FindTokens(TokenType.PORTAL).Where(p => ((PortalToken)p).Color == this.Color).ToList();

            //if no matching portal exists, then destory this portal and the piece with it. 
            if (Portals.Count == 1)
            {
                Implode();
                return;
            }
            
            //if movement type is teleport, this means piece arrived from another teleporter.
            //just let rest of move process.

            //there is one 
            if (Portals.Count == 2)
            {
                foreach (PortalToken p in Portals)
                {
                    if (!p.Space.Location.Equals(Space.Location))
                    {
                        Piece Teleported = Space.RemovePieceFrom();
                        Space.Parent.RecordGameAction(new GameActionMove(Piece, Space.Location, p.Space.Location));
                        if (Space.Parent.ContentsAt(p.Space.Location).CanMoveInto(Piece, Piece.Direction))
                        {
                            Space.Parent.RecordGameAction(new GameActionEffect(Space.Location, TransitionType.TELEPORT_FROM));
                            Space.Parent.RecordGameAction(new GameActionEffect(Space.Location, TransitionType.TELEPORT_TO));
                            Space.Parent.ContentsAt(p.Space.Location).MovePieceOn(Piece);
                        }
                        else
                            Implode();
                        break;
                    }
                }
                return;
            }

            //TODO.  NEED TO HANDLE SOME RANDOMNESS.
            //  FOR NOW, JUST MOVE TO THE FIRST ONE IN THE LIST
            //  NEED TO UPDATE THIS LOGIC
            //there is one 
            if (Portals.Count > 2)
            {
                foreach (PortalToken p in Portals)
                {
                    if (!p.Space.Location.Equals(Space.Location))
                    {
                        Piece Teleported = Space.RemovePieceFrom();

                        Space.Parent.RecordGameAction(new GameActionMove(Piece, Space.Location, p.Space.Location));
                        Space.Parent.ContentsAt(p.Space.Location).MovePieceOn(Piece);
                        break;
                    }
                }
            }

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
