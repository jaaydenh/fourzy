using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public class Piece
    {

        #region "PieceData"

        [JsonProperty("playerId")]
        public int PlayerId { get; set; }

        [JsonProperty("herdId")]
        public int HerdId { get; set; }

        // Unique Id is to help with movement to make sure we grab the right piece when moving from one space to another
        // In the current model, 2 pieces may temporarily have the same location when a push occurs.
        public string UniqueId { get; set; }

        // Most of the time a piece will be a Fouzy player piece, but sometimes, it might be a champion with special properties
        // And sometimes it will be a non-player piece.
        [JsonProperty("pieceType")]
        public PieceType PieceType { get; set; }

        [JsonProperty("conditions")]
        public List<PieceConditionType> Conditions = new List<PieceConditionType>();

        // Fluff Only.  Every Fourzy will have a name.  Maybe we'll show these to the end user one day.
        // public string Name { get; set; }

        #endregion

        public string Notation
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(PlayerId);

                if (Conditions.Count > 0)
                {
                    sb.Append(':');
                    foreach (PieceConditionType c in Conditions)
                    {
                        sb.Append(PieceConstants.PieceCondition(c));
                    }
                }

                return sb.ToString();
            }
        }

        #region "Constructors"

        public Piece(int PlayerId)
        {
            this.PlayerId = PlayerId;
            this.HerdId = 0;
            this.PieceType = PieceType.PLAYER;
            this.UniqueId = Guid.NewGuid().ToString();
            //this.Name = Herd.GetRandomName();
        }

        [JsonConstructor]
        public Piece(int PlayerId, int HerdId)
        {
            this.PlayerId = PlayerId;
            this.HerdId = HerdId;
            this.PieceType = PieceType.PLAYER;
            this.UniqueId = Guid.NewGuid().ToString();
            //this.Name = Herd.GetRandomName();
        }

        public Piece(int PlayerId, int HerdId, PieceType Type, List<PieceConditionType> Conditions = null)
        {
            this.PlayerId = PlayerId;
            this.HerdId = HerdId;
            this.PieceType = Type;
            this.UniqueId = Guid.NewGuid().ToString();
    /*        this.Name = Herd.GetRandomName()*/;
            if (Conditions != null)
            {
                foreach (PieceConditionType c in Conditions)
                {
                    this.Conditions.Add(c);
                }
            }
        }

        public Piece(Piece Piece)
        {
            this.PlayerId = Piece.PlayerId;
            this.HerdId = Piece.HerdId;
            this.PieceType = Piece.PieceType;
            if (Piece.UniqueId == null)
                this.UniqueId = Guid.NewGuid().ToString();
            else
                this.UniqueId = Piece.UniqueId;
            foreach (PieceConditionType c in Piece.Conditions)
            {
                this.Conditions.Add(c);
            }
            //this.Name = Piece.Name;
        }

        public Piece(PieceType NonPlayerType)
        {
            this.PlayerId = 0;
            this.HerdId = 0;
            this.PieceType = NonPlayerType;
            this.UniqueId = Guid.NewGuid().ToString();
            //this.Name = Herd.GetRandomName();
        }

        public Piece(string Notation)
        {
            this.HerdId = 0;
            this.UniqueId = Guid.NewGuid().ToString();

            string[] notationArray = Notation.Split(':');
            if (notationArray.Length > 0)
            {
                this.PlayerId = int.Parse(notationArray[0]);
            }
            else
            {
                this.PlayerId = 0;
            }

            if (PlayerId > 0) { this.PieceType = PieceType.PLAYER; }

            if (notationArray.Length > 1)
            {
                foreach (char c in notationArray[1])
                {
                    Conditions.Add(PieceConstants.PieceCondition(c.ToString()));
                }
            }
        }

        #endregion

        #region "Commands"

        public void Die()
        {
            Conditions.Add(PieceConditionType.DEAD);
        }
        #endregion

        #region "Conditions"

        public int ConditionCount(PieceConditionType Condition)
        {
            return Conditions.Count(n => n == Condition);
        }

        public bool HasCondition(PieceConditionType Condition)
        {
            return Conditions.Count(n => n == Condition) > 0;
        }

        public void ClearCondition(PieceConditionType Condition)
        {
            Conditions.Remove(Condition);
        }

        #endregion

        #region "Elements"

        public void ApplyElement(ElementType Element, int Amount=0)
        {
            // Add logic here for water putting out fire, heat, fire melting cold, etc.
        }

        #endregion

        #region "Event Processing"

        public void StartOfTurn(int PlayerId)
        {

        }

        // Used for fragile pieces potentially, transferring poison, curses and other stuff.
        public void PieceBumpsIntoLocation(MovingPiece Piece, BoardLocation Location)
        {
        }

        public Direction PieceChoosesDirection(BoardLocation Location)
        {
            return Direction.NONE;
        }

        public void EndOfTurn(int PlayerId)
        {
            if (ConditionCount(PieceConditionType.FIERY) >= Constants.TurnsUntilFireConsumesPiece) Die();

            //Clear any conditions added during moving.
            ClearCondition(PieceConditionType.INERTIA);
            ClearCondition(PieceConditionType.WIND);
            ClearCondition(PieceConditionType.STRAIGHT);
        }

        #endregion "Event Processing"
    }
}
