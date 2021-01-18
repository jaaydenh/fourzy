using System.Collections.Generic;
using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public class PlayerTurnResult
    {
        [JsonProperty("gameState")]
        public GameState GameState { get; set; }

        [JsonProperty("activity")]
        public List<GameAction> Activity { get; set; }

        [JsonProperty("turn")]
        public PlayerTurn Turn { get; set; }

        public PlayerTurnResult()
        {
            GameState = null;
            this.Turn = null;
            this.Activity = new List<GameAction>();
        }

        public PlayerTurnResult(GameState GameState, List<GameAction> Activity)
        {
            this.GameState = GameState;
            this.Activity = Activity;
            this.Turn = null;
        }

        public PlayerTurnResult(GameState GameState, List<GameAction> Activity, PlayerTurn Turn)
        {
            this.GameState = GameState;
            this.Activity = Activity;
            this.Turn = Turn;
        }
    }
}
