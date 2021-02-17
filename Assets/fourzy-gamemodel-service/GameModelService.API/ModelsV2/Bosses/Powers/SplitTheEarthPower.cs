using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class SplitTheEarthPower : IBossPower, IMove
    {
        public string Name { get { return "SplitTheEarth"; } }
        public BossPowerType PowerType { get { return BossPowerType.ArrowCharm; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }

        public string Notation { get { return Name; } }

        public int Location { get; set; }
        public Direction SplitDirection { get; set; }
        public List<BoardLocation> Locations { get; set; }
        
        public SplitTheEarthPower()
        {

        }

        public SplitTheEarthPower(Direction SplitDirection, int Location)
        {
            this.SplitDirection = SplitDirection;
            this.Location = Location;
        }

        public bool Activate(GameState State)
        {
            switch (SplitDirection)
            {
                case Direction.DOWN:
                    foreach (BoardLocation l in (new BoardLocation(0, Location)).Look(State.Board, Direction.DOWN))
                        Locations.Add(l);
                    break;
                case Direction.RIGHT:
                    foreach (BoardLocation l in (new BoardLocation(Location,0)).Look(State.Board, Direction.RIGHT))
                        Locations.Add(l);
                    break;
            }

            foreach (BoardLocation l in Locations)
            {
                State.Board.AddToken(new PitToken(), l, AddTokenMethod.ALWAYS, true);
            }
            BossAIHelper.GetBoss(State).UseSpecialAbility();

            return true;
        }

        public bool IsAvailable(GameState State, bool IsDesparate = false)
        {
            if (!IsDesparate) return false;
            if (BossAIHelper.GetBoss(State).SpecialAbilityCount > 0) return true;
            return false;
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
 
            List<IMove> Powers = new List<IMove>();

            for (int r = 1; r < State.Board.Rows - 1; r++) 
                Powers.Add(new SplitTheEarthPower(Direction.RIGHT, r));

            for (int c = 1; c < State.Board.Columns - 1; c++) 
                Powers.Add(new SplitTheEarthPower(Direction.DOWN, c));

            return Powers;

        }

    }
}