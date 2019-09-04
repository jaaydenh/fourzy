using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class PlantBombPower : IBossPower, IMove
    {
        public string Name { get { return "Plant A Tree"; } }
        public BossPowerType PowerType { get { return BossPowerType.PlantTree; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }
        public const int MaxBombs = 2;

        public string Notation { get { return Name; } }

        public BoardLocation BombLocation { get; set; }

        public PlantBombPower()
        {
            this.BombLocation = new BoardLocation(0, 0);
        }

        public PlantBombPower(BoardLocation Location)
        {
            this.BombLocation = Location;
        }

        public bool Activate(GameState State)
        {
            if (State.Board.ContentsAt(BombLocation).ContainsPiece) return false;
            if (!State.Board.ContentsAt(BombLocation).TokensAllowEndHere) return false;
            if (State.Board.ContentsAt(BombLocation).ContainsTokenType(TokenType.CIRCLE_BOMB)) return false;

            State.Board.AddToken(new CircleBombToken(), BombLocation);
            State.Board.RecordGameAction(new GameActionBossPower(this));

            return true;
        }

        public bool IsAvailable(GameState State, bool IsDesparate = false)
        {
            if (!IsDesparate) return false;
            if (State.Board.FindTokens(TokenType.CIRCLE_BOMB).Count > MaxBombs) return false;
            return true;
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Powers = new List<IMove>();
            foreach (BoardSpace s in State.Board.Contents)
            {
                if (s.ContainsPiece) continue;
                if (!s.TokensAllowEndHere) continue;
                if (s.ContainsTokenType(TokenType.CIRCLE_BOMB)) continue;

                Powers.Add(new PlantTreePower(s.Location));
            }

            return Powers;
        }

    }
}