using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class MadBomberBoss : IBoss
    {
        public string Name { get { return "Dr Madd"; } }
        public List<IBossPower> Powers { get; }
        const int MinStartingTrees = 2;
        const int MaxStartingTrees = 6;

        public MadBomberBoss()
        {
            this.Powers = new List<IBossPower>();
            this.Powers.Add(new PlantBombPower());
        }

        public List<IMove> GetPossibleActivations(GameState State)
        {
            List<IMove> Activations = new List<IMove>();
            foreach (IBossPower p in Powers)
            {
                if (p.IsAvailable(State))
                    Activations.AddRange(p.GetPossibleActivations(State));
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

        //Use for special conditions.

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
