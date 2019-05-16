using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class GameArea
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int InitialPlayerTime { get; set; }
        public int AdditionalTimePerMove { get; set; }

        //Specific Rules or Conditions Specific to the Region
        public List<GameEffect> Effects { get; set; }
    }
}
