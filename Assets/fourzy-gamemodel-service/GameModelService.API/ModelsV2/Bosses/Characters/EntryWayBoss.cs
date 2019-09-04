using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class EntryWayBoss : IBoss
    {
        public string Name { get { return "Entry Way Blocker"; } }
        public List<IBossPower> Powers { get; }
        const int MinStartingArrows = 6;   //too few arrows and boss is too easy
        const int MaxStartingArrows = 12;  //too many arrows and boss has too many combinations to think about.

        public EntryWayBoss()
        {
            this.Powers = new List<IBossPower>();
            this.Powers.Add(new BlockMovePower());
            this.Powers.Add(new BlockASidePower());
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Activations = new List<IMove>();
            foreach (IBossPower p in Powers)
            {
                if (p.IsAvailable(State, IsDesparate))
                    Activations.AddRange(p.GetPossibleActivations(State, IsDesparate));
            }
            return Activations;
        }

        public bool StartGame(GameState State)
        {
            return true;
        }

        public bool TriggerPower(GameState State)
        {
            return true;
        }
        
        public bool TriggerBossWin(GameState State)
        {
            return false;
        }

        public bool TriggerBossLoss(GameState State)
        {
            return false;
        }

    }
}
