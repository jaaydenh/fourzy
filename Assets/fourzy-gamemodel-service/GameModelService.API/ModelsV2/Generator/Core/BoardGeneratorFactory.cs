using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public static class BoardGeneratorFactory
    {
        public static BoardGenerator CreateGenerator(Area TargetArea, GameOptions Options = null, BoardGenerationPreferences Preferences = null)
        {
            switch (TargetArea)
            {
                case Area.TRAINING_GARDEN:
                    return new BeginnerGardenRandomGenerator(Options, Preferences);

                case Area.ENCHANTED_FOREST:
                    return new ForestRandomGenerator(Options, Preferences);

                case Area.ICE_PALACE:
                    return new IcePalaceRandomGenerator(Options, Preferences);

                case Area.SANDY_ISLAND:
                    return new IslandRandomGenerator(Options, Preferences);

            }
            return null;
        }
    }
}
