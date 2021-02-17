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
    public class MovingWallGameEffect : IGameEffect
    {
        //Interface
        public string Name { get; }
        public GameEffectType Type { get; }
        public GameEffectTiming Timing { get; }
        public GameState Parent { get; set; }

        //Customization
        public TokenType WallToken { get; set; }
        public int Countdown { get; set; }
        public int Frequency { get; set; }
        public int CurrentLine { get; set; }
        public Direction MoveDirection { get; set; }

        public MovingWallGameEffect(TokenType WallToken, Direction MoveDirection, int ActivateFrequency = 3, GameState Parent = null)
        {
            this.Name = "Moving Wall Game Effect";
            this.Type = GameEffectType.MOVINGWALL;
            this.Timing = GameEffectTiming.END_OF_TURN;
            this.Parent = Parent;
            this.WallToken = WallToken;
            this.Frequency = ActivateFrequency;
            this.Countdown = ActivateFrequency;
            this.CurrentLine = -1;

            //TO DO.  Write a function to get direction based on starting location.
            this.MoveDirection = MoveDirection;

            Initialize();
        }

        public MovingWallGameEffect(string Notation)
        {
            this.Name = "Moving Wall Game Effect";
            this.Type = GameEffectType.MOVINGWALL;
            this.Timing = GameEffectTiming.END_OF_TURN;

            this.MoveDirection = Direction.RIGHT;
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
                Countdown = Frequency;
                Parent.Board.RecordGameAction(new GameActionGameEffect(this));

                List<BoardLocation> TokenLocations = new List<BoardLocation>() { };

                switch (MoveDirection)
                {
                    case Direction.UP:
                    case Direction.DOWN:
                        TokenLocations = Parent.Board.GetRow(CurrentLine);
                        break;

                    case Direction.LEFT:
                    case Direction.RIGHT:
                        TokenLocations = Parent.Board.GetColumn(CurrentLine);
                        break;
                }

                foreach (BoardLocation s in TokenLocations)
                {
                    if (Parent.Board.ContentsAt(s).ContainsTokenType(WallToken))
                    {
                        Parent.Board.ContentsAt(s).RemoveTokens(WallToken);
                    }
                }

                switch (MoveDirection)
                {
                    case Direction.UP:
                        CurrentLine--;
                        if (CurrentLine < 0) CurrentLine = Parent.Board.Rows - 1;
                            break;

                    case Direction.LEFT:
                        CurrentLine--;
                        if (CurrentLine < 0) CurrentLine = Parent.Board.Columns - 1;
                        break;

                    case Direction.RIGHT:
                        CurrentLine++;
                        if (CurrentLine == Parent.Board.Columns) CurrentLine = 0;
                        break;

                    case Direction.DOWN:
                        CurrentLine++;
                        if (CurrentLine == Parent.Board.Rows) CurrentLine = 0;
                        break;

                }

                switch (MoveDirection)
                {
                    case Direction.UP:
                    case Direction.DOWN:
                        TokenLocations = Parent.Board.GetRow(CurrentLine);
                        break;

                    case Direction.LEFT:
                    case Direction.RIGHT:
                        TokenLocations = Parent.Board.GetColumn(CurrentLine);
                        break;
                }

                foreach (BoardLocation s in TokenLocations)
                {
                    if (!Parent.Board.ContentsAt(s).ContainsTokenType(WallToken) 
                        && Parent.Board.ContentsAt(s).TokensAllowEndHere)
                    {
                        Parent.Board.ContentsAt(s).AddToken(TokenFactory.Create(WallToken));
                    }
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
