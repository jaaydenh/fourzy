using System.Collections.Generic;
using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public interface IToken
    {
        int Layer { get; set; }
        Direction Orientation { get; set; }
        bool Visible { get; set; }
        bool Delete { get; set; }
        TokenClassification Classification { get; }
        int Complexity { get;  }

        [JsonIgnore]
        BoardSpace Space { get; set; }

        bool pieceCanEnter { get; set; }
        bool pieceMustStopOn { get; set; }
        bool pieceCanEndMoveOn { get; set; }
        bool isMoveable { get; set; }

        int addFriction { get; set; }

        //Adjust momentum will increase or decrease value 
        //Set Momentum will set to a specific value.
        int adjustMomentum { get; set; }
        int setMomentum { get; set; }
        bool changePieceDirection { get; set; }
        bool HasDynamicFeature { get; }
        TokenType Type { get; set; }

        Direction GetDirection(MovingPiece Piece);

        //bool destroyTokenOnEnd { get; set; }
        //float chanceDestroyPieceOnEnd { get; set; }
        //bool useCurrentDirection { get; set; }

        void ApplyElement(ElementType Element);
        //quicksand, piece conditions, rotating stuff.
        void StartOfTurn(int PlayerId);

        //?
        void PieceEntersBoard(MovingPiece Piece);

        //? Additional Stuff that happens?  
        //Should we group what happens here, with what happens on neighboring space?        
        void PieceEntersSpace(MovingPiece Piece);

        //We need to have a trigger here, but should this only go to the location where something was bumped??
        // or do we need this to go to all space.  ie. If any tree is bumped, something happens?
        void PieceBumpsIntoLocation(MovingPiece Piece, BoardLocation Location);

        void PieceStopsOnSpace(MovingPiece Piece);
        void PieceLeavesSpace(MovingPiece Piece);

        //Stuff like pits, trap doors, bombs
        void EndOfTurn(int PlayerId);

        //for displaying a board.
        char TokenCharacter { get; }
        string Notation { get; }

        string Print();

    }
}
