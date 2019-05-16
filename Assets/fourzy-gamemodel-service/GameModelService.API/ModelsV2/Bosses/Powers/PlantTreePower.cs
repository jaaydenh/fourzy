using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class PlantTreePower : IBossPower, IMove
    {
        public string Name { get { return "Plant A Tree"; } }
        public BossPowerType PowerType { get { return BossPowerType.PlantTree; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }

        public string Notation { get { return Name; } }

        public BoardLocation GrowLocation { get; set; }

        public PlantTreePower()
        {
            this.GrowLocation = new BoardLocation(0, 0);
        }

        public PlantTreePower(BoardLocation Location)
        {
            this.GrowLocation = Location;
        }

        public bool Activate(GameState State)
        {
            if (State.Board.ContentsAt(GrowLocation).ContainsPiece) return false;
            if (!State.Board.ContentsAt(GrowLocation).TokensAllowEndHere) return false;

            State.Board.AddToken(new FruitTreeToken(), GrowLocation);
            State.Board.RecordGameAction(new GameActionBossPower(this));

            return true;
        }

        public bool IsAvailable(GameState State)
        {
            return true;
        }

        public List<IMove> GetPossibleActivations(GameState State)
        {
            List<IMove> Powers = new List<IMove>();
            foreach (BoardSpace s in State.Board.Contents)
            {
                if (s.ContainsPiece) continue;
                if (!s.TokensAllowEndHere) continue;

                Powers.Add(new PlantTreePower(s.Location));
            }

            return Powers;
        }

    }
}