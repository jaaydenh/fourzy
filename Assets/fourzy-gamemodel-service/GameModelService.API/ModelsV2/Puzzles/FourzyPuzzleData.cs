using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class FourzyPuzzleData
    {
        public string ID;
        public bool Enabled;
        public GameBoardDefinition GameBoard;
        public AIProfile Profile;
        public BossType Boss;
        public int FirstTurn;
        public List<PlayerTurn> Solution = new List<PlayerTurn>();
        public GameBoardData SolutionStateData { get; set; }
        public PuzzleGoalType GoalType { get; set; }
        public int MoveLimit { get; set; }
        public int Complexity { get; set; }

        //What Spells?
        //        public SpellId[] availableSpells;

    }
}
