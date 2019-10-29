using System;
using System.Collections.Generic;
using System.Text;
using FourzyGameModel.Common;
using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public class PlayerTurn
    {
        [JsonProperty("playerId")]
        public int PlayerId { get; set; }

        [JsonProperty("playerString")]
        public string PlayerString { get; set; }
        
        [JsonProperty("moves")]
        [JsonConverter(typeof(MoveConverter))]
        public List<IMove> Moves { get; set; }

        public string Notation { get
            {
                StringBuilder note = new StringBuilder();
                foreach (IMove m in Moves)
                {
                    note.Append(m.Notation);
                }
                return note.ToString();
            }
        }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("uniqueId")]
        public string UniqueId { get; private set; }





        public PlayerTurn(SimpleMove Move)
        {
            if (Move != null)
            {
                PlayerId = Move.Piece.PlayerId;
                Moves = new List<IMove>() { Move };
            }
            this.UniqueId = Guid.NewGuid().ToString();
        }

        public PlayerTurn(int PlayerId, IMove Move)
        {
            this.PlayerId = PlayerId;
            Moves = new List<IMove>() { Move };
            this.UniqueId = Guid.NewGuid().ToString();
        }

        public PlayerTurn(int Player, Direction Direction, int Location )
        {
            PlayerId = (int)Player;
            Moves = new List<IMove>() { new SimpleMove(new Piece(Player, Player, PieceType.PLAYER),Direction,Location)};
            this.UniqueId = Guid.NewGuid().ToString();
        }
        
        //Hex will be a common Element.
        public PlayerTurn(int PlayerId, Direction Direction, int Location, BoardLocation HexLocation)
        {
            this.PlayerId = PlayerId;
            Moves = new List<IMove>() { new SimpleMove(new Piece(PlayerId, PlayerId, PieceType.PLAYER), Direction, Location) };
            Moves.Add(new HexSpell(PlayerId, HexLocation));
            this.UniqueId = Guid.NewGuid().ToString();
        }

        public PlayerTurn(SimpleMove Move, IMove Power)
        {
            Moves = new List<IMove>() {};
            PlayerId = Move.Piece.PlayerId;
            this.UniqueId = Guid.NewGuid().ToString();

            if (Move != null)
            {
                Moves.Add(Move);
            }

            if (Power != null)
            {
                Moves.Add(Power);
            }

        }


        [JsonConstructor]
        public PlayerTurn(List<IMove> Moves)
        {
            //PlayerId = (int)Player;
            this.Moves = new List<IMove>() { };
            this.Moves.AddRange(Moves);
            this.UniqueId = Guid.NewGuid().ToString();
        }

        //[JsonConstructor]
        //public PlayerTurn(string PlayerString, List<IMove> Moves)
        //{
        //    this.PlayerString = PlayerString;
        //    this.PlayerId = 0;
        //    this.Moves = new List<IMove>() { };
        //    this.Moves.AddRange(Moves);
        //    this.UniqueId = Guid.NewGuid().ToString();
        //}

        public SimpleMove GetMove()
        {
            return Moves.Find(move => move.MoveType == MoveType.SIMPLE) as SimpleMove;
        }
    }
}
