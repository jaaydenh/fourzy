﻿using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class FruitToken : IToken
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
                return TokenConstants.Fruit;
            }
        }

        public string Notation
        {
            get
            {
                return TokenConstants.Fruit.ToString();
            }
        }

        public List<string> ExportNotation
        {
            get
            {
                return new List<string>() { TokenConstants.Fruit.ToString() };
            }
        }

        public TokenClassification Classification
        {
            get
            {
                return TokenClassification.ITEM;
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

        public bool Squashed { get; set; }

        public FruitToken()
        {
            StandardTokenInit();
            this.Type = TokenType.FRUIT;
            this.Squashed = false;
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


        public void Squash()
        {
            Squashed = true;
            Visible = false;
            pieceMustStopOn = true;
            addFriction = 100;
            Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.FRUIT_SQUASH, this, new StickyToken()));

            //Do we want to add and remove tokens, or just change properties??
            Space.AddToken(new StickyToken());
            Space.RemoveTokens(TokenType.FRUIT);

        }

        public void Burn()
        {
            Visible = false;
            Squashed = true;
            Delete = true;
        }

        public void PlantTree()
        {
            Squashed = true;
            Visible = false;
            pieceMustStopOn = true;
            addFriction = 100;
            pieceCanEnter = false;
            Space.Parent.RecordGameAction(new GameActionTokenTransition(Space.Location, TransitionType.FRUIT_TREE, this, new FruitTreeToken()));

            //Do we want to add and remove tokens, or just change properties??
            Space.AddToken(new FruitTreeToken());
            Space.RemoveTokens(TokenType.FRUIT);
        }


        public Direction GetDirection(MovingPiece Piece)
        {
            return Orientation;
        }

        public void ApplyElement(ElementType Element)
        {
            switch (Element)
            {
                case ElementType.FIRE:
                    Delete = true;
                    break;
                case ElementType.WATER:
                    PlantTree();
                    Delete = true;
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
            if (!Squashed && Piece.Location.Equals(Space.Location)) Squash();
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