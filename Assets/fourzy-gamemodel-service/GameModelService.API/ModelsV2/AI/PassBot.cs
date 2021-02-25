using System.Linq;

namespace FourzyGameModel.Model
{
    public class PassBotAI : AIPlayer
    {
        private GameState EvalState { get; set; }

        public PassBotAI(GameState State)
        {
            this.EvalState = State;
        }

        public PlayerTurn GetTurn()
        {
            PlayerTurn Turn = new PlayerTurn(EvalState.ActivePlayerId, new PassMove() );
            return Turn;
        }
    }
}
