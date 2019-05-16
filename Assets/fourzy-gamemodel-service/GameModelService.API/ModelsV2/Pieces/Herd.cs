using System.Collections.Generic;
using FourzyGameModel.Model.Herds;

namespace FourzyGameModel.Model
{
    public class Herd
    {
        public string Name { get; }
        public string Description { get; }

        public int HerdId; 
        public List<Creature> Members;
        public List<HerdTrick> AvailableTricks;

        public static string GetRandomName()
        {
            //PLACEHOLDER
            //create a Random Creature Name based on Herd.
            return "Blobby";
        }

    }
}
