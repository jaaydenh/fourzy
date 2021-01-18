using System;
using System.Collections.Generic;
using System.Text;

namespace FourzyGameModel.Model
{
    public interface IGameEffect
    {
        string Name { get;  }
        GameEffectType Type { get;  }
        GameEffectTiming Timing { get; }
        GameState Parent { get; set; }

        string Export();

        //Events;
        void StartOfTurn(int PlayerId);
        void EndOfTurn(int PlayerId);
        void PieceStopsOnSpace(MovingPiece Piece);
        void PieceBumpsIntoLocation(MovingPiece Piece, BoardLocation Location);
    }
}

