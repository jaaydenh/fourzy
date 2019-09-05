//@vadym udod

using System.Collections.Generic;
using FourzyGameModel.Model;
using UnityEngine;

namespace Fourzy._Updates.ClientModel
{
    public class ClientPlayerTurn : PlayerTurn
    {
        public bool AITurn = false;

        public ClientPlayerTurn(SimpleMove Move) : base(Move)
        {
        }

        public ClientPlayerTurn(List<IMove> Moves) : base(Moves)
        {
        }

        public ClientPlayerTurn(int PlayerId, IMove Move) : base(PlayerId, Move)
        {
        }

        public ClientPlayerTurn(int Player, Direction Direction, int Location) : base(Player, Direction, Location)
        {
        }

        public ClientPlayerTurn(int PlayerId, Direction Direction, int Location, BoardLocation HexLocation) : base(PlayerId, Direction, Location, HexLocation)
        {
        }
    }
}