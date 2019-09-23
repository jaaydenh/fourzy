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
    public class LifeGameEffect : IGameEffect
    {
        //Interface
        public string Name { get; }
        public GameEffectType Type { get; }
        public GameEffectTiming Timing { get; }
        public GameState Parent { get; set; }

        //Customization
        public TokenType LifeToken { get; set; }

        public LifeGameEffect(TokenType LifeToken, GameState Parent = null)
        {
            this.Name = "Life Game Effect";
            this.Type = GameEffectType.LIFE;
            this.Timing = GameEffectTiming.END_OF_TURN;
            this.Parent = Parent;
            this.LifeToken = LifeToken;

            Initialize();
        }

        public LifeGameEffect(string Notation)
        {
            this.Name = "Life Game Effect";
            this.Type = GameEffectType.LIFE;
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
        public void EndOfTurn(int PlayerId)
        {
            List<BoardLocation> TokenLocations = Parent.Board.FindTokenLocations(this.LifeToken);
            List<BoardLocation> Death = new List<BoardLocation>();
            Dictionary<BoardLocation, int> Birth = new Dictionary<BoardLocation, int>();

            //1. Find Death through under or over population
            foreach (BoardLocation l in TokenLocations)
            {
                int life_count = 0;
                foreach (BoardLocation neighbor in l.GetAdjacentWithWrap(Parent.Board))
                {
                    if (TokenLocations.Contains(neighbor)) life_count++;
                    else
                    {
                        if (Birth.Keys.Contains(neighbor)) Birth[neighbor]++;
                        else Birth.Add(neighbor, 1);
                    }
                }
                if (life_count < 2 || life_count > 3) Death.Add(l);
            }

            //2. Find Birth
            foreach (BoardLocation l in Birth.Keys)
            {
                if (Birth[l] == 3)
                {
                    IToken t = TokenFactory.Create(LifeToken);
                    t.Space = Parent.Board.ContentsAt(l);
                    Parent.Board.ContentsAt(l).AddToken(t);

                    Parent.RecordGameAction(new GameActionTokenDrop(t, TransitionType.GAME_EFFECT, l, l));
                }
            }

            foreach (BoardLocation l in Death)
            {
                IToken t = Parent.Board.ContentsAt(l).FindTokens(LifeToken).First();
                if (t != null)
                {
                    Parent.RecordGameAction(new GameActionTokenRemove(l, TransitionType.GAME_EFFECT, t));
                    Parent.Board.ContentsAt(l).RemoveTokens(LifeToken);
                }
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
