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
    public class SideSpikeGameEffect : IGameEffect
    {
        //Interface
        public string Name { get; }
        public GameEffectType Type { get; }
        public GameEffectTiming Timing { get; }
        public GameState Parent { get; set; }

        //Customization
        public TokenType SpikeToken { get; set; }
        public TokenType PreSpikeToken { get; set; }
        public int Countdown { get; set; }
        public int Frequency { get; set; }
        public BoardLocation CurrentLocation { get; set; }
        public Direction SpikeDirection { get; set; }


        public SideSpikeGameEffect(TokenType SpikeToken, TokenType PreSpikeToken, BoardLocation InitialLocation,
            int ActivateFrequency = GameEffectConstants.DefaultFrequency, GameState Parent = null)
        {
            this.Name = "SideSpike Game Effect";
            this.Type = GameEffectType.SIDESPIKE;
            this.Timing = GameEffectTiming.END_OF_TURN;
            this.Parent = Parent;
            this.SpikeToken = SpikeToken;
            this.CurrentLocation = InitialLocation;
            this.Frequency = ActivateFrequency;
            this.Countdown = ActivateFrequency;

            //TO DO.  Write a function to get direction based on starting location.
            this.SpikeDirection = Direction.UP;

            Initialize();
        }

        public SideSpikeGameEffect(string Notation)
        {
            this.Name = "SpikeSpike Game Effect";
            this.Type = GameEffectType.SIDESPIKE;
            this.Timing = GameEffectTiming.END_OF_TURN;

            this.SpikeToken = SpikeToken;
            this.Frequency = 3;
            this.Countdown = 3;

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
            // Advance Countdown.
            // If trigger:
            //Review the current location.
            //If there is no tokens, as at start of game, place a leading token.
            //If there is a leading token, convert to real token. Place the next leading token.

            Countdown--;
            if (Countdown == 0)
            {
                if (!Parent.Board.ContentsAt(CurrentLocation).ContainsTokenType(PreSpikeToken))
                {
                    if (Parent.Board.ContentsAt(CurrentLocation).TokensAllowEnter)
                        Parent.Board.ContentsAt(CurrentLocation).AddToken(TokenFactory.Create(PreSpikeToken));
                    else
                        Countdown = -1;
                }
                else
                {
                    Parent.Board.RecordGameAction(new GameActionGameEffect(this));

                    Parent.Board.ContentsAt(CurrentLocation).RemoveTokens(PreSpikeToken);
                    Parent.Board.ContentsAt(CurrentLocation).AddToken(TokenFactory.Create(SpikeToken));

                    CurrentLocation = CurrentLocation.Neighbor(SpikeDirection);
                    //If done, end effect.
                    if (!CurrentLocation.OnBoard(Parent.Board) 
                        || !Parent.Board.ContentsAt(CurrentLocation).TokensAllowEndHere)
                            { Frequency = 0; Countdown = -1; }
                    else
                    {
                        Parent.Board.ContentsAt(CurrentLocation).AddToken(TokenFactory.Create(PreSpikeToken));
                    }
                }

                Countdown = Frequency;
            }

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
