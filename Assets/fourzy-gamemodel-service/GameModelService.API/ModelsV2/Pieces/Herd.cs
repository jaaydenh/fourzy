using System.Collections.Generic;
using FourzyGameModel.Model.Herds;

namespace FourzyGameModel.Model
{
    public class Herd
    {
        public string Name { get; }
        public string Description { get; }

        public string HerdId; 
        public List<Creature> Members;
        public List<HerdTrick> AvailableTricks;

        public static string GetRandomName()
        {
            //PLACEHOLDER
            //create a Random Creature Name based on Herd.
            return "Blobby";
        }

        //Todo. Constant for default HerdCount.
        public Herd(string HerdId, int HerdCount = 100)
        {
            //Need to create HerdInfo
            this.Name = "PlaceHolder";
            this.Description = "PlaceHolder";
            this.HerdId = HerdId;
            this.Members = new List<Creature>();
            this.AvailableTricks = new List<HerdTrick>();
           
            if (HerdCount > 0)
            {
                for (int i=0; i<HerdCount; i++)
                {
                    Members.Add(new Creature(HerdId));
                }
            }
        }
        public Herd(Herd original) : this(original.HerdId, original.Members.Count) { }
    }
}
