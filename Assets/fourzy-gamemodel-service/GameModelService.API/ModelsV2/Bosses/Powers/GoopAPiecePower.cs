using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class GoopAPiecePower : IBossPower, IMove
    {
        public string Name { get { return "Gooped"; } }
        public BossPowerType PowerType { get { return BossPowerType.GoopAPiece; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }

        public string Notation { get { return Name; } }

        public BoardLocation GoopLocation { get; set; }

        public GoopAPiecePower()
        {
            this.GoopLocation = new BoardLocation(0, 0);
        }

        public GoopAPiecePower(BoardLocation Location)
        {
            this.GoopLocation = Location;
        }

        public bool Activate(GameState State)
        {
            return true;
        }

        public bool IsAvailable(GameState State, bool IsDesparate = false)
        {
            return true;
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Powers = new List<IMove>();

            foreach (BoardLocation l in State.Board.FindPieceLocations(State.Opponent(State.ActivePlayerId)))
            {
                if (!State.Board.ContentsAt(l).TokensAllowPushing) Powers.Add(new GoopAPiecePower(l));
            }

            return Powers;
        }

    }
}