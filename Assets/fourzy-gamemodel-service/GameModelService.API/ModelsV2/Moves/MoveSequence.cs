using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class SimpleMoveSequence
    {
        public List<SimpleMove> Moves;
        public GameState State;

        public string UniqueId { get { return State.UniqueId;  } }

        public int MoveCount { get { return Moves.Count; } }

        public string MoveNotation {get {
                StringBuilder output = new StringBuilder();
                foreach (SimpleMove m in Moves)
                {
                    output.Append(m.Notation + ";");
                }
                return output.ToString();
            }
        }

        public SimpleMoveSequence(SimpleMoveSequence Copy)
        {
            this.Moves = new List<SimpleMove>();
            foreach (SimpleMove m in Copy.Moves)
            {
                this.Moves.Add(m);
            }
            
            this.State = new GameState(Copy.State);
        }

        public SimpleMoveSequence(GameState State)
        {
            this.Moves = new List<SimpleMove>();
            this.State = State;
        }

        public SimpleMoveSequence()
        {
            this.Moves = new List<SimpleMove>();
            this.State = null;
        }
    }
}
