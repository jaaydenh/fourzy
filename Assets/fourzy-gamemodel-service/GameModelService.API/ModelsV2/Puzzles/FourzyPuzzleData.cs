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
        public int StartingMagic { get; set; }
        public List<SpellId> AvailableSpells {get; set;}

        public FourzyPuzzleData()
        {

        }

        public FourzyPuzzleData(GameState State, PuzzleTestResults TestResults)
        {
            GameBoardData board = State.Board.SerializeData();
            GameBoardDefinition definition = new GameBoardDefinition();
            definition.Area = State.Board.Area;
            definition.BoardName = State.Board.Area.ToString() + " Win In " + TestResults.VictoryDepth.ToString();
            definition.EnabledRealtime = false;
            definition.Rows = board.Rows;
            definition.Columns = board.Columns;
            definition.ID = State.UniqueId;
            definition.Enabled = true;
            definition.BoardSpaceData = board.BoardSpaceData;
            definition.InitialMoves = new List<SimpleMove>();
            this.SolutionStateData = TestResults.VictoryStates[0].Board.SerializeData();
            foreach (PlayerTurn t in TestResults.Solution)
                this.Solution.Add(t);
            this.Enabled = true;
            this.GoalType = PuzzleGoalType.WIN;
            this.ID = Guid.NewGuid().ToString();
            this.GameBoard = definition;
            this.MoveLimit = TestResults.VictoryDepth;
            this.Complexity = PuzzleTestTools.PuzzleComplexity(State.Board, TestResults.VictoryDepth);
            if (this.AvailableSpells == null) this.AvailableSpells = new List<SpellId>();
            if (this.AvailableSpells.Count == 0) this.StartingMagic = 0;
            else this.StartingMagic = 100;
        }


    }
}
