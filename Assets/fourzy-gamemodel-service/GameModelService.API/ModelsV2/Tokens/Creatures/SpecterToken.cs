﻿using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class SpecterToken : IToken
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
                return TokenConstants.Specter;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Specter.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Specter.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.CREATURE;
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

        public SpecterToken()
        {
            StandardTokenInit();

            this.Type = TokenType.SPECTER;
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
            Space.Parent.RecordGameAction(new GameActionTokenRemove(Space.Location, TransitionType.SPELL_FADE, this));
            Space.RemoveTokens(TokenType.SPECTER);
        }

        public Direction GetDirection(MovingPiece Piece)
        {
            return Piece.Direction;
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
            if (Piece.Location.Equals(Space.Location)) Fade();
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
