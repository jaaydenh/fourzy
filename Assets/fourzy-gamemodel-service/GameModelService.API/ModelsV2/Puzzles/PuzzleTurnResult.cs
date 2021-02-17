//using System.Collections.Generic;
//using Newtonsoft.Json;

//namespace FourzyGameModel.Model
//{
//    public class PuzzleTurnResult
//    {
//        [JsonProperty("afterPlayerMove")]
//        public PlayerTurnResult AfterPlayerMove { get; set; }
//        [JsonProperty("afterPuzzleMove")]
//        public AITurnResult AfterPuzzleMove { get; set; }

//        public PuzzleTurnResult()
//        {
//            this.AfterPlayerMove = new PlayerTurnResult();
//            this.AfterPuzzleMove = new AITurnResult();

//        }

//        public PuzzleTurnResult(PlayerTurnResult AfterPlayerMove, AITurnResult AfterPuzzleMove)
//        {
//            this.AfterPlayerMove = AfterPlayerMove;
//            this.AfterPuzzleMove = AfterPuzzleMove;
//        }
//    }
//}
