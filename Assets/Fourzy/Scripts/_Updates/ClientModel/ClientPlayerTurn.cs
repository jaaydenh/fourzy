//@vadym udod

using FourzyGameModel.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Fourzy._Updates.ClientModel
{
    [Serializable]
    public class ClientPlayerTurn : PlayerTurn
    {
        [JsonProperty("AITurn")]
        public bool AITurn = false;
        [JsonProperty("createdOnThisDevice")]
        public bool createdOnThisDevice = false;
        [JsonProperty("playerTimerLeft")]
        public float playerTimerLeft;

        public ClientPlayerTurn(SimpleMove Move) : base(Move)
        {
        }

        [JsonConstructor]
        public ClientPlayerTurn(List<IMove> Moves) : base(Moves)
        {
        }

        public ClientPlayerTurn(GameState State, string Notation) : base(State, Notation)
        {
        }

        public ClientPlayerTurn(int PlayerId, IMove Move) : base(PlayerId, Move)
        {
        }

        public ClientPlayerTurn(SimpleMove Move, IMove Power) : base(Move, Power)
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