using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{

    public class DetectLossGameEffect : IGameEffect
    {
        //Interface
        public string Name { get; }
        public GameEffectType Type { get; }
        public GameEffectTiming Timing { get; }
        public GameState Parent { get; set; }

        //Customization
        public TokenType LifeToken { get; set; }
        public int Countdown { get; set; }
        public int Frequency { get; set; }

        public DetectLossGameEffect(GameState Parent = null)
        {
            this.Name = "Detect Loss Game Effect";
            this.Type = GameEffectType.DETECT_LOSS;
            this.Timing = GameEffectTiming.START_OF_TURN;
            this.Parent = Parent;
            this.LifeToken = LifeToken;

            Initialize();
        }

        public DetectLossGameEffect(string Notation)
        {
            this.Name = "Detect Win Game Effect";
            this.Type = GameEffectType.DETECT_LOSS;
            this.Timing = GameEffectTiming.END_OF_TURN;

            this.LifeToken = TokenFactory.Create(Notation).Type;

            Initialize();
        }

        private void Initialize()
        {

        }

        public string Export()
        {
            return TokenConstants.Sticky.ToString();
        }

        //Events
        public void StartOfTurn(int PlayerId)
        {
            AITurnEvaluator AITE = new AITurnEvaluator(Parent);
            if (AITE.IsAThreat())
                Parent.Board.RecordGameAction(new GameActionGameEffect(this));
        }

        public void EndOfTurn(int PlayerId)
        {

        }

        public void PieceBumpsIntoLocation(MovingPiece Piece, BoardLocation Location)
        {

        }

        public void PieceStopsOnSpace(MovingPiece Piece)
        {

        }


    }
}
