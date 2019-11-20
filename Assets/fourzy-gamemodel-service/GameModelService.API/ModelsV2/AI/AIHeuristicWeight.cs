using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace FourzyGameModel.Model
{
    public class AIHeuristicWeight
    {
        public int PositionWeight { get; set; }
        public int FourWeight { get; set; }
        public int FiveWeight { get; set; }
        public int ThreatWeight { get; set; }
        public int SetupWeight { get; set; }
        public bool ConsiderDiagonals { get; set; }

        public bool PruneFiveSetup { get; set; }
        //public bool PruneCrossSetup { get; set; }

        public bool ConsiderDeadSpaces { get; set; }
        public bool ConsiderOpponentPieces { get; set; }

        public bool LookForSetups { get; set; }
        public bool AvoidSetups { get; set; }

        public bool LookForUnstoppable { get; set; }
        public bool AvoidUnstoppable { get; set; }

        public bool IsAggressive { get; set; }

        public AIHeuristicWeight()
        {
            Initialize();
        }

        public AIHeuristicWeight(int FourWeight, int FiveWeight, int PositionWeight)
        {
            Initialize();
            this.FourWeight = FourWeight;
            this.FiveWeight = FiveWeight;
            this.PositionWeight = PositionWeight;
        }

        public void Initialize()
        {
            PositionWeight = AIConstants.DefaultPositionWeight;
            FourWeight = AIConstants.DefaultFourWeight;
            FiveWeight = AIConstants.DefaultFiveWeight;
            ThreatWeight = AIConstants.DefaultThreatWeight;
            SetupWeight = AIConstants.DefaultSetupWeight;
            ConsiderDeadSpaces = true;
            ConsiderOpponentPieces = true;
            PruneFiveSetup = false;
            LookForSetups = false;
            AvoidSetups = false;
            LookForUnstoppable = false;
            AvoidUnstoppable = false;
            IsAggressive = false;
            ConsiderDiagonals = true;
        }

    }
}
