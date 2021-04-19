namespace FourzyGameModel.Model
{
    public class Creature
    {
        public string HerdId { get; set; }
        public string Name { get; set; } //Mr Frosty, Joe.  Maybe dynamically created. Maybe Player can name?

        //likely changing how champions will work.
        //public bool isChampion; // convert to a type?

        public Creature(string HerdId, string Name = "")
        {
                
            this.HerdId = HerdId;

            if (Name == "") Name = Constants.GenerateName(4);
            this.Name = Name;
        }
    }
}
