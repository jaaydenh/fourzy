﻿using System.Collections.Generic;

namespace FourzyGameModel.Model
{

    //A current token will force movement along a direction at the start of every turn.

    public class CurrentToken : IToken
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

        public char TokenCharacter
        {
            get
            {
                switch (this.Orientation)
                {
                    case Direction.UP:
                        return '\u2191';
                    case Direction.DOWN:
                        return '\u2193';
                    case Direction.LEFT:
                        return '\u2190';
                    case Direction.RIGHT:
                        return '\u2192';
                }
                return ' ';
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Ice.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Ice.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.TERRAIN;
            }
        }


        public CurrentToken(Direction Orientation)
        {
            StandardTokenInit();

            this.Orientation = Orientation;
            this.Type = TokenType.CURRENT;
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

        public void Flow()
        {
            //Try to move the token once space?
            //We really need the piece movement logic here... 
            //Can we trigger a piece move?

            BoardSpace Target = Space.Parent.ContentsAt(Space.Location.Neighbor(Orientation));
            //if piece is allowed + no pushing, just move the piece on.
            //if pushing is required, we'll need to deal with this?  
            
            //how do we ensure a piece is only moved once, if all spaces are triggered in a loop, it's possible, flow will occur once, and then again?

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
            Flow();
        }
    }
}
