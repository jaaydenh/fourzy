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
    public class RotatingSideWallGameEffect : IGameEffect
    {
        //Interface
        public string Name { get; }
        public GameEffectType Type { get; }
        public GameEffectTiming Timing { get; }
        public GameState Parent { get; set; }

        //Customization
        public TokenType WallToken { get; set; }
        public Rotation RotateDirection { get; set; }
        public Direction CurrentDirection { get; set; }
        public int Countdown { get; set; }
        public int Frequency { get; set; }

        public RotatingSideWallGameEffect(TokenType WallToken, Rotation RotateDirection, Direction InitialDirection = Direction.LEFT, 
                    int ActivationFrequency = GameEffectConstants.DefaultFrequency, GameState Parent = null)
        {
            this.Name = "Rotating Side Wall Game Effect";
            this.Type = GameEffectType.ROTATING_SIDE_WALL;
            this.Timing = GameEffectTiming.END_OF_TURN;
            this.Parent = Parent;
            this.WallToken = WallToken;
            this.Countdown = ActivationFrequency;
            this.Frequency = ActivationFrequency;
            this.RotateDirection = RotateDirection;
            this.CurrentDirection = InitialDirection;

            Initialize();
        }

        public RotatingSideWallGameEffect(string Notation)
        {
            this.Name = "Shift Tokens Game Effect";
            this.Type = GameEffectType.ROTATING_SIDE_WALL;
            this.Timing = GameEffectTiming.END_OF_TURN;

            this.WallToken = TokenFactory.Create(Notation).Type;
            //TODO
            this.RotateDirection = Rotation.CLOCKWISE;

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
            // Remove all matching tokens from the edge.
            // Rotate 90
            // Add tokens to the next edge
            //    If the token does not allows pieces end
            //      do not place on a piece
            //    otherwise, add on any spaces.  Replace any existing terrain.

            if (Countdown < 0) return;
            if (Countdown > 0) Countdown--;
            if (Countdown > 0)
            {
                return;
            }
            Countdown = Frequency; 

            List<BoardLocation> TokenLocations = Parent.Board.Edge(CurrentDirection);
            foreach (BoardLocation s in TokenLocations)
            {
                if (Parent.Board.ContentsAt(s).ContainsTokenType(WallToken))
                {
                    Parent.Board.ContentsAt(s).RemoveTokens(WallToken);
                }
            }

            CurrentDirection = BoardLocation.Rotate(CurrentDirection, RotateDirection);

            TokenLocations = Parent.Board.Edge(CurrentDirection);
            foreach (BoardLocation s in TokenLocations)
            {
                IToken Token = TokenFactory.Create(WallToken);
                if (!Token.pieceCanEndMoveOn)
                    if (Parent.Board.ContentsAt(s).ContainsPiece) continue;

                if (Parent.Board.ContentsAt(s).ContainsTerrain)
                {
                    Parent.Board.ContentsAt(s).RemoveTerrain();
                }

                Parent.Board.ContentsAt(s).AddToken(Token);
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
