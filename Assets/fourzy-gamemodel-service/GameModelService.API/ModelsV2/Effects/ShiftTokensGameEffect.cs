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
    public class ShiftTokensGameEffect : IGameEffect
    {
        //Interface
        public string Name { get; }
        public GameEffectType Type { get; }
        public GameEffectTiming Timing { get; }
        public GameState Parent { get; set; }

        //Customization
        public TokenType ShiftToken { get; set; }
        public Direction ShiftDirection { get; set; }

        public int Countdown { get; set; }
        public int Frequency { get; set; }

        public ShiftTokensGameEffect(TokenType ShiftToken, Direction ShiftDirection, GameState Parent = null, int ActivateFrequency =3)
        {
            this.Name = "Life Game Effect";
            this.Type = GameEffectType.LIFE;
            this.Timing = GameEffectTiming.END_OF_TURN;
            this.Parent = Parent;
            this.ShiftToken = ShiftToken;
            this.ShiftDirection = ShiftDirection;
            this.Frequency = ActivateFrequency;
            this.Countdown = ActivateFrequency;

            Initialize();
        }

        public ShiftTokensGameEffect(string Notation)
        {
            this.Name = "Shift Tokens Game Effect";
            this.Type = GameEffectType.LIFE;
            this.Timing = GameEffectTiming.END_OF_TURN;

            this.ShiftToken = TokenFactory.Create(Notation).Type;
            //TODO
            this.ShiftDirection = Direction.NONE;

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
            List<BoardLocation> TokenLocations = Parent.Board.FindTokenLocations(this.ShiftToken);

            foreach (BoardLocation start in TokenLocations)
            {
                BoardLocation end = start.Neighbor(ShiftDirection).Wrap(Parent.Board);
                
                    IToken t = TokenFactory.Create(ShiftToken);
                    t.Space = Parent.Board.ContentsAt(end);
                    Parent.Board.ContentsAt(end).AddToken(t);
                    Parent.Board.ContentsAt(start).RemoveTokens(ShiftToken);
                
                    Parent.RecordGameAction(new GameActionTokenMovement(t, TransitionType.GAME_EFFECT, start,end));
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
