using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{

    public class DetectWinGameEffect : IGameEffect
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

        public DetectWinGameEffect(GameState Parent = null)
        {
            this.Name = "Detect Win Game Effect";
            this.Type = GameEffectType.DETECT_WIN;
            this.Timing = GameEffectTiming.START_OF_TURN;
            this.Parent = Parent;
            this.LifeToken = LifeToken;

            Initialize();
        }

        public DetectWinGameEffect(string Notation)
        {
            this.Name = "Detect Win Game Effect";
            this.Type = GameEffectType.DETECT_WIN;
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
            TurnEvaluator TE = new TurnEvaluator(Parent);
            if (TE.PossibleToWin(PlayerId))
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
