using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class PuzzleHint
    {
        PuzzleHintType Type { get; set; }

        string Text { get; set; }
        int Cost { get; set; }
        PlayerTurn Turn { get; set; }
        int TurnCount { get; set; }

        public PuzzleHint(string Text, int TurnCount=0, int Cost=0)
        {
            this.Type = PuzzleHintType.CLUE;
            this.Cost = Cost;
            this.Text = Text;
            this.TurnCount = TurnCount;
            this.Turn = null; 
        }

        public PuzzleHint(PlayerTurn Turn, int TurnCount = 0, int Cost = 0)
        {
            this.Type = PuzzleHintType.CLUE;
            this.Cost = Cost;
            this.Text = "";
            this.TurnCount = TurnCount;
            this.Turn = Turn;
        }
    }
}
