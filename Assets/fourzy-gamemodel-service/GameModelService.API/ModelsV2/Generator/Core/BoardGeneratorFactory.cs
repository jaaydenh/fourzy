using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public static class BoardGeneratorFactory
    {
        public static BoardGenerator CreateGenerator(Area TargetArea, string SeedString= "", GameOptions Options = null, BoardGenerationPreferences Preferences = null)
        {
            switch (TargetArea)
            {
                case Area.TRAINING_GARDEN:
                    return new BeginnerGardenRandomGenerator(SeedString, Options, Preferences);

                case Area.ENCHANTED_FOREST:
                    return new ForestRandomGenerator(SeedString, Options, Preferences);

                case Area.ICE_PALACE:
                    return new IcePalaceRandomGenerator(SeedString, Options, Preferences);

                case Area.SANDY_ISLAND:
                    return new IslandRandomGenerator(SeedString, Options, Preferences);

            }
            return null;
        }
    }
}
