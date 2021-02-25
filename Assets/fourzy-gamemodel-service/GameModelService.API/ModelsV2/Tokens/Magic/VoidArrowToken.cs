﻿using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class VoidArrowToken : IToken
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
        public int MagicAdjustment { get; set; }

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

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Arrow.ToString(), TokenConstants.DirectionString(Orientation) };
            }
        }

        public VoidArrowToken(Direction Orientation, int MagicAdjustment = TokenConstants.DefaultVoidArrowLoss)
        {
            StandardTokenInit();

            this.MagicAdjustment = MagicAdjustment;
            this.Orientation = Orientation;
            this.Type = TokenType.ARROW;
            this.changePieceDirection = true;
        }

        public VoidArrowToken(string notation)
        {
            StandardTokenInit();
            if (notation.Length > 1)
            {
                this.Orientation = GetOrientation(notation[1]);
            }
            else
            {
                this.Orientation = Direction.DOWN;
            }

            this.Type = TokenType.ARROW;
            this.changePieceDirection = true;
        }

        private Direction GetOrientation(char orientation)
        {
            switch (orientation)
            {
                case TokenConstants.Up:
                    return Direction.UP;
                case TokenConstants.Down:
                    return Direction.DOWN;
                case TokenConstants.Left:
                    return Direction.LEFT;
                case TokenConstants.Right:
                    return Direction.RIGHT;
            }
            throw new Exception("Missing correct orientation character");
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

        public string Notation
        {
            get
            {
                return TokenConstants.Arrow + TokenConstants.DirectionString(Orientation);
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.INSTRUCTION;
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
                return TokenConstants.COMPLEXITY_BASIC;
            }
        }

        public bool DisruptsWin
        {
            get
            {
                return false;
            }
        }

        public Direction GetDirection(MovingPiece Piece)
        {
            return Orientation;
        }

        #region "Events"
        #endregion

        public void ApplyElement(ElementType Element)
        {

        }

        public void StartOfTurn(int PlayerId)
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


    }
}
