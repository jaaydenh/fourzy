using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{

    /// <summary>
    ///   LifeGameEffect will execute game of life rules on all tokens of a particular type.
    ///     Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.
    ///     Any live cell with two or three live neighbours lives on to the next generation.
    ///     Any live cell with more than three live neighbours dies, as if by overpopulation.
    //      Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
    /// </summary>
    public class RainGameEffect : IGameEffect
    {
        //Interface
        public string Name { get; }
        public GameEffectType Type { get; }
        public GameEffectTiming Timing { get; }
        public GameState Parent { get; set; }

        //Customization
        int RainSeverity { get; set; }

        public RainGameEffect(int RainSeverity, GameState Parent = null)
        {
            this.Name = "Rain Game Effect";
            this.Type = GameEffectType.RAIN;
            this.Timing = GameEffectTiming.END_OF_TURN;
            this.Parent = Parent;
            this.RainSeverity = RainSeverity;

            Initialize();
        }

        public RainGameEffect(string Notation)
        {
            this.Name = "Rain Game Effect";
            this.Type = GameEffectType.RAIN;
            this.Timing = GameEffectTiming.END_OF_TURN;
            this.RainSeverity = RainSeverity;

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
        public void EndOfTurn(int PlayerId)
        {
            switch (RainSeverity)
            {
                case 1:
                    break;
                case 2:
                    break;
            }

            Parent.Board.RecordGameAction(new GameActionGameEffect(this));

            //Impact of Rain
            //  -- % chance 'trickle' water token in a new space.
            //  -- % chance convert 'trickle' into puddle
            //  -- % chance convert 'puddle into water
            //  -- % water overflow into an adjacent space
            //  -- impact of water on other tokens. 
            //      may put out a fire for instance.

            //List<BoardLocation> TokenLocations = Parent.Board.FindTokenLocations(this.ShiftToken);
            //foreach (BoardLocation start in TokenLocations)
            //{
            //    BoardLocation end = start.Neighbor(ShiftDirection).Wrap(Parent.Board);

            //    IToken t = TokenFactory.Create(ShiftToken);
            //    t.Space = Parent.Board.ContentsAt(end);
            //    Parent.Board.ContentsAt(end).AddToken(t);
            //    Parent.Board.ContentsAt(start).RemoveTokens(ShiftToken);

            //    Parent.RecordGameAction(new GameActionTokenMovement(t, TransitionType.GAME_EFFECT, start, end));
            //}
        }

        public void PieceBumpsIntoLocation(MovingPiece Piece, BoardLocation Location)
        {

        }

        public void PieceStopsOnSpace(MovingPiece Piece)
        {

        }

        public void StartOfTurn(int PlayerId)
        {

        }
    }
}
