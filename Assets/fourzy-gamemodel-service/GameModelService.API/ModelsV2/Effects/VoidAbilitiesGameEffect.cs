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
    public class VoidAbilitiesGameEffect : IGameEffect
    {
        //Interface
        public string Name { get; }
        public GameEffectType Type { get; }
        public GameEffectTiming Timing { get; }
        public GameState Parent { get; set; }

        public int Duration { get; set; }


        public VoidAbilitiesGameEffect(int Duration, GameState Parent = null)
        {
            this.Name = "Void";
            this.Type = GameEffectType.VOID;
            this.Timing = GameEffectTiming.PASSIVE;
            this.Parent = Parent;

            this.Duration = Duration;

            Initialize();
        }

        public VoidAbilitiesGameEffect(string Notation)
        {
            this.Name = "Void";
            this.Type = GameEffectType.VOID;
            this.Timing = GameEffectTiming.PASSIVE;

            Initialize();
        }

        private void Initialize()
        {

        }

        public void Fade()
        {
            this.Parent.GameEffects.Remove(this);
        }

        public string Export()
        {
            return TokenConstants.Sticky.ToString();
        }

        //Events
        public void EndOfTurn(int PlayerId)
        {
            if (Duration > 0) Duration--;
            if (Duration == 0) Fade();
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
