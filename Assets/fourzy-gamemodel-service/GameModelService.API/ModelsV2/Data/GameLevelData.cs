using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class GameLevelData
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
        public int StartingMagic { get; set; }
        public List<SpellId> AvailableSpells { get; set; }

        public GameLevelData()
        {

        }

        public GameLevelData(GameBoardDefinition Board, List<SpellId> Spells, int StartingPlayer = 1, int StartingMagic=100)
        {
            GameBoardDefinition definition = Board;
            this.SolutionStateData = null;
            this.Solution = new List<PlayerTurn>();
            this.Enabled = true;
            this.GoalType = PuzzleGoalType.WIN;
            this.ID = Guid.NewGuid().ToString();
            this.GameBoard = definition;
            this.MoveLimit = -1;
            this.Complexity = -1;
            this.FirstTurn = StartingPlayer;
            this.AvailableSpells = new List<SpellId>();
            if (Spells.Count > 0)
            {
                foreach (SpellId s in Spells) AvailableSpells.Add(s);
            }
            if (this.AvailableSpells.Count == 0) this.StartingMagic = 0;
            else this.StartingMagic = StartingMagic;
        }


    }
}
