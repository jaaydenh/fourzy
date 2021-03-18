using System;
using System.Collections.Generic;
using System.Text;

namespace FourzyGameModel.Model
{
    public class CharacterNameFactory
    {
        private static bool isLoaded = false;
        private static RandomTools RandomObject = null;

        //One way to enhance the generator might be to implement some language tools, like open and closed syllables
        //public static Dictionary<string, int> OpenSyllables = new Dictionary<string, int>() { };
        //public static Dictionary<string, int> ClosedSyllables = new Dictionary<string, int>() { };


        //For now, we can use common syllables from well know magicians and mash them up.
        public static Dictionary<string, int> FirstSyllables = new Dictionary<string, int>() { };
        public static Dictionary<string, int> MidSyllables = new Dictionary<string, int>() { };
        public static Dictionary<string, int> LastSyllables = new Dictionary<string, int>() { };

        //Not implemented is a way to estimate the strength of word. 
        // This can be used in situations when we want to present the strength of the bot.

        //public static List<string> WeakWords = new List<string>()
        //{ "Fragile", "Feeble", "Frail", "Hesitant", "Powerless", "Shaky", "Slug", "Uncertain",  "Unsteady", "Weak", "Wobbly",
        //  "Delicate", "Faint", "Flimsy", "Rickety", "Rotten", "Spent", "Unsound", "Wasted", "Wavering", "Undependable", "Torpid", "Tender",
        //  "Supine", "Forceless"};

        //public static List<string> StupidWords = new List<string>()
        //{ "Dull", "Fool", "Futile", "Laughable", "Ludicrous", "Naive", "Sensless", "Trivial", "Dazed", "Rash", "Brainless", "Deficient", "Dolt",
        //  "Gullible", "Dopey", "Dim", "Inane", "Quack", "Egghead", "Blind", "Weak-minded"};

        //public static List<string> MediocreWords = new List<string>()
        //{ "Decent", "Middling", "Second-Rate", "So-So", "Uninspired", "Conventional", "Fair", "Hundrum", "Pedestrian", "Tolerable", "Vanilla", "Standard"};

        //public static List<string> SmartWords = new List<string>()
        //{ "Nerdy", "Smart", "Geek", "Savant", "Trained", "Nurtured", "Erudite", "Versed", "Vital", "Key"};

        //public static List<string> DoctorWords = new List<string>()
        //{ "Expert", "Professor", "Scientific", "Principal", "Sage", "Civilized", "Informed", "Knowledgeaable", "Versed", "Literary", "Strategic", "Decisive", "Imperative", "Fellow"};


        //Also not ready for titles.

        //public static List<string> CharacterTitles = new List<string>()
        //{ "Herder", "Wizard", "Strategist", "Magician", "Tenderer", "ologist", "Thinkerer", "SpellMasher", "General", "Commander", "Keeper", "Mage", "Sorcerer", "Warlock", "LightBringer",
        //  "Chooser", "Pointer", "Pusher", "Tamer", "Channeler", "Summoner", "Controller", "Teacher", "Instructor", "Whisperer", "ThoughtGiver", "MoveMaker", "Sender", "Shepard"};

        //public static List<string> StagePrefix = new List<string>()
        //{ "Amazing", "Great", "Marvelous","Fantabulous", "Famous", "Powerful" };

        //It might be fun to sprinkle in a few more earthly names.  I picked out a few, but didn't add them yet.

        //public static List<string> GirlNameIdeas = new List<string>()
        //{ "Bea", "Eve", "Fai", "Hope", "Greer", "Jade", "Rain",
        //  "Sage", "Skye", "Snow", "Rue", "Lark", "Vale", "Wyn", "Starr",
        //  "Alice", "Cora", "Eden", "Isla", "Maya", "Stella",
        //  "Avre", "Lav", "Sierra", "Trinity", "Sue"
        //};

        //public static List<string> BoyNameIdeas = new List<string>()
        //{ "Ash", "Max", "Park", "Evan", "Hunter", "Levi", "Owen", "Tyler", "Apollo", "Dak", "Xavier", "Serg", "Solo", "Bob"};


        //Prefixes
        //These will be at the start of a name.

        public static List<string> GeneralPrefixes = new List<string>()
        { "Think", "General", "Mage", "Great", "Wiz", "Spell", "Magic", "Mind"};

        public static List<string> FourzyObjectWords = new List<string>()
        { "Fourzy", "Forest", "Island", "Desert","Garden", "Palace","Fountain","Herd", "Boss", "Portal", "Way", "Quad", "Square", "Box", "Quatro", "Four", "Row", "Column", "Col", "Goop", "Arrow", "Ghost", "Gravity", "Slide", "Push", "Bump", "Wall", "Line", "Diag", "Vert", "Fruit", "Direction","Block", "Pit", "Lure", "Way", "Up", "Down", "Left", "Right"};

        public static List<string> TheNumberFour = new List<string>()
        { "four", "quad", "quat", "vier", "fir", "fjor", "patru", "erba", "ina", "afar", "hudu" };
        
        public static List<string> Animals = new List<string>()
        { "Bird", "Falcon", "Bat", "Tiger", "Lion", "Squirrel", "Rabbit", "Spider", "Hawk", "Monkey", "Snake", "Bison", "Wolf", "Owl", "Panther", "Dragon", "Griffin", "Sparrow", "Pigeon", "Penguin", "Bear", "Cat"};

        public static List<string> PowerNamesBefore = new List<string>()
        { "Polar", "Zig", "Zap", "Day", "Night", "Time", "Spell", "Fire", "Lava", "Power", "Mana", "Rain", "Goop","Corner","River","Storm","Ice","Whisper","Angel","Sand", "Snow", "Sky", "Light", "Luck", "Arrow", "Spirit", "Ghost", "Mind", "Water","Earth","Quake", "Quick", "Air", "Mind", "Swirl", "Spiral"};

        public static List<string> Substance = new List<string>()
        { "copper", "silver", "zinc", "gold", "steel", "wood", "earth", "stone", "iron", "diamond", "lead", "paper", "time", "luck", "sand"};

        public static List<string> PlacePrefix = new List<string>()
        { "forest", "island", "garden", "palace", "portal", "world", "pit", "moon" };

        //Can potentially be used either way

        public static List<string> AnimalPart = new List<string>()
        { "Feathers", "Claw", "Jaw", "Eye", "Head", "Foot", "Wing", "Tooth", "Heart", "Soul"};
        
        public static List<string> BodyPart = new List<string>()
        { "Hand", "Head", "Eye", "Finger", "Chin", "Stare", "Arm", "Leg", "Smile", "Fist", "Toe", "Tongue", "Jaw", "Bone", "Face", "Heart", "Soul", "Palm"};
        
        public static List<string> SolidObjects = new List<string>()
        { "spoon", "sword", "staff", "shield", "stone", "paper", "ring", "spear", "blade", "saw", "rod", "sink", "spin", "pack", "belt", "edge" };

        //Suffixes
        //Used at the end of a name.

        public static List<string> GeneralSuffixes = new List<string>()
        { "asaurus", "ician", "ologist", "erer", "lock", "erion", "inkerer", "brain", "inator" };
                        
        public static List<string> PlaceSuffix = new List<string>()
        { "field", "bridge", "pit", "pile", "tower", "keep", "castle", "town", "mor", "stone", "point", "hood", "haven", "hold","bank","brook","bend", "shire", "garden" };

        public static List<string> PowerNamesAfter = new List<string>()
        { "wind", "stone", "storm", "cloud", "moon", "beam", "zap", "sight"};

        public static List<string> Manipulate = new List<string>()
        { "seer","teller","sayer","slayer","finder","teacher","thinker","singer", "slinger", "bender", "bringer", "storm", "store", "runner", "tamer", "crusher", "crasher", "warder", "roller", "presser", "flinger", "walker", "breaker", "builder", "caller", "pusher", "horder"};

        //An Idea for the future is to create longer names with some adjectives. Needs more thought/
        //public static List<string> FullNameFormulas = new List<string>()
        //{ "[WizardName] The [WizardAdjective]",
        //  "The [Superlative] [WizardName]",
        //  "[TwoSyllable] [LastName]"
        //  "[TwoSyllable] [Of] [PlaceName]"
        //};


        public static Dictionary<string, int> WizardNameFormulas = new Dictionary<string, int>() { };

        private static void LoadSyllables()
        {
            if (!isLoaded)
            {
                FirstSyllables.Clear();
                MidSyllables.Clear();
                LastSyllables.Clear();

                FirstSyllables.Add("sar", 20);
                FirstSyllables.Add("mer", 20);
                FirstSyllables.Add("raist", 20);
                FirstSyllables.Add("rince", 20);
                FirstSyllables.Add("gand", 20);
                FirstSyllables.Add("el", 20);
                FirstSyllables.Add("garg", 10);
                FirstSyllables.Add("glin", 10);
                FirstSyllables.Add("yo", 10);
                FirstSyllables.Add("al", 10);
                FirstSyllables.Add("mor", 10);
                FirstSyllables.Add("rand", 10);
                FirstSyllables.Add("har", 10);
                FirstSyllables.Add("chun", 10);
                FirstSyllables.Add("bel", 10);
                FirstSyllables.Add("sir", 10);
                FirstSyllables.Add("zat", 10);
                FirstSyllables.Add("pal", 10);
                FirstSyllables.Add("jaf", 10);
                FirstSyllables.Add("drac", 10);
                FirstSyllables.Add("luc", 10);
                FirstSyllables.Add("sha", 10);
                FirstSyllables.Add("crisp", 10);
                FirstSyllables.Add("wil", 10);
                FirstSyllables.Add("cir", 10);
                FirstSyllables.Add("cass", 10);
                FirstSyllables.Add("dur", 10);
                FirstSyllables.Add("shin", 10);
                FirstSyllables.Add("am", 10);
                FirstSyllables.Add("val", 10);
                FirstSyllables.Add("wig", 10);
                FirstSyllables.Add("thoth", 10);
                FirstSyllables.Add("is", 10);
                FirstSyllables.Add("sev", 10);
                FirstSyllables.Add("dres", 10);
                FirstSyllables.Add("pres", 10);
                FirstSyllables.Add("trog", 10);
                FirstSyllables.Add("cor", 10);
                FirstSyllables.Add("ang", 10);


                MidSyllables.Add("a", 10);
                MidSyllables.Add("o", 10);
                MidSyllables.Add("u", 10);
                MidSyllables.Add("i", 10);
                MidSyllables.Add("e", 10);
                MidSyllables.Add("y", 2);

                LastSyllables.Add("a", 10);

                LastSyllables.Add("bus", 10);
                LastSyllables.Add("book", 10);
                LastSyllables.Add("bor", 10);

                LastSyllables.Add("den", 10);
                LastSyllables.Add("da", 10);
                LastSyllables.Add("dar", 10);
                LastSyllables.Add("dore", 10);
                LastSyllables.Add("dor", 10);
                LastSyllables.Add("dalf", 30);
                LastSyllables.Add("dorf", 10);

                LastSyllables.Add("esto", 10);
                LastSyllables.Add("ero", 10);

                LastSyllables.Add("gana", 10);

                LastSyllables.Add("ina", 10);
                LastSyllables.Add("ine", 10);
                LastSyllables.Add("igo", 10);

                LastSyllables.Add("lin", 10);
                LastSyllables.Add("lock", 10);
                LastSyllables.Add("kell", 10);

                LastSyllables.Add("mon", 10);
                LastSyllables.Add("mal", 10);
                LastSyllables.Add("mort", 10);

                LastSyllables.Add("o", 10);
                LastSyllables.Add("ow", 10);
                LastSyllables.Add("ov", 10);

                LastSyllables.Add("ric", 10);
                LastSyllables.Add("rond", 10);
                LastSyllables.Add("rath", 10);

                LastSyllables.Add("sa", 20);

                LastSyllables.Add("tar", 10);
                LastSyllables.Add("tor", 10);
                LastSyllables.Add("tana", 10);
                LastSyllables.Add("to", 10);

                LastSyllables.Add("us", 10);
                LastSyllables.Add("y", 10);
                LastSyllables.Add("zam", 10);
            }
        }

        private static void LoadWizardFormulas()
        {
            WizardNameFormulas.Clear();
            WizardNameFormulas.Add("[First][Last]", 50);
            WizardNameFormulas.Add("[First][Mid][Last]", 25);
            WizardNameFormulas.Add("[GeneralPrefix][BodyPart]", 10);
            WizardNameFormulas.Add("[GeneralPrefix][AnimalPart]", 10);
            WizardNameFormulas.Add("[GeneralPrefix][Mid][Last]", 10);
            WizardNameFormulas.Add("[Animal][AnimalPart]", 10);
            WizardNameFormulas.Add("[Animal][BodyPart]", 10);
            WizardNameFormulas.Add("[Animal][Object]", 10);
            WizardNameFormulas.Add("[Animal][PowerSuffix]", 10);
            WizardNameFormulas.Add("[Animal][Mid][Last]", 10);
            WizardNameFormulas.Add("[FourzyWord][GeneralSuffix]", 10);
            WizardNameFormulas.Add("[FourzyWord][PlaceSuffix]", 10);
            WizardNameFormulas.Add("[FourzyWord][Manipulate]", 10);
            WizardNameFormulas.Add("[FourzyWord][PowerSuffix]", 10);
            WizardNameFormulas.Add("[FourzyWord][AnimalPart]", 10);
            WizardNameFormulas.Add("[FourzyWord][BodyPart]", 10);
            WizardNameFormulas.Add("[FourzyWord][Object]", 10);
            WizardNameFormulas.Add("[FourzyWord][Mid][Last]", 10);
            WizardNameFormulas.Add("[Four][PowerSuffix]", 10);
            WizardNameFormulas.Add("[Four][GeneralSuffix]", 10);
            WizardNameFormulas.Add("[Four][Object]", 10);
            WizardNameFormulas.Add("[Four][Animal]", 10);
            WizardNameFormulas.Add("[Four][BodyPart]", 10);
            WizardNameFormulas.Add("[Four][Mid][Last]", 10);
            WizardNameFormulas.Add("[First][Four]", 10);
            WizardNameFormulas.Add("[PlacePrefix][GeneralSuffix]", 10);
            WizardNameFormulas.Add("[PlacePrefix][Manipulate]", 10);
            WizardNameFormulas.Add("[PlacePrefix][Animal]", 10);
            WizardNameFormulas.Add("[PlacePrefix][AnimalPart]", 10);
            WizardNameFormulas.Add("[PlacePrefix][PowerSuffix]", 10);
            WizardNameFormulas.Add("[PowerPrefix][BodyPart]", 10);
            WizardNameFormulas.Add("[PowerPrefix][Animal]", 10);
            WizardNameFormulas.Add("[PowerPrefix][PowerSuffix]", 10);
            WizardNameFormulas.Add("[PowerPrefix][Manipulate]", 10);
            WizardNameFormulas.Add("[PowerPrefix][PlaceSuffix]", 10);
            WizardNameFormulas.Add("[PowerPrefix][Object]", 10);
            WizardNameFormulas.Add("[PowerPrefix][GeneralSuffix]", 10);
            WizardNameFormulas.Add("[PowerPrefix][Mid][Last]", 10);
            WizardNameFormulas.Add("[Substance][PlaceSuffix]", 10);
            WizardNameFormulas.Add("[Substance][Manipulate]", 10);
            WizardNameFormulas.Add("[Substance][Animal]", 10);
            WizardNameFormulas.Add("[Substance][BodyPart]", 10);
            WizardNameFormulas.Add("[Substance][Last]", 10);
            WizardNameFormulas.Add("[Object][PowerSuffix]", 10);
        }
    
        private static string GenerateNameFromRandomCharacters(int length = -1)
        {
            const string vowels = "aeiou";
            const string consonants = "bcdfghjklmnpqrstvwxyz";

            var rnd = new Random();
            var name = new StringBuilder();
            if (length < 0) length = rnd.Next(3, 8);

            length = length % 2 == 0 ? length : length + 1;

            for (var i = 0; i < length / 2; i++)
            {
                name
                    .Append(vowels[rnd.Next(vowels.Length)])
                    .Append(consonants[rnd.Next(consonants.Length)]);
            }
            name[0] = char.ToUpper(name[0]);

            return name.ToString();
        }

        private static void Initialize()
        {
            if (!isLoaded)
            {
                LoadSyllables();
                LoadWizardFormulas();
                RandomObject = new RandomTools();
                isLoaded = true;
            }
        }

        private static string GenerateTwoSyllableName()
        {
            Initialize();

            string name = RandomObject.RandomWeightedItem(FirstSyllables) + RandomObject.RandomWeightedItem(LastSyllables);

            return Char.ToUpper(name[0]) + name.ToLower().Substring(1);
        }

        private static string GenerateThreeSyllableName()
        {
            LoadSyllables();
            RandomTools r = new RandomTools();

            string name = r.RandomWeightedItem(FirstSyllables) + r.RandomWeightedItem(MidSyllables) + r.RandomWeightedItem(LastSyllables);

            return name;
        }

        //Eventually We can refine this to have some specific traits for various bots.
        public static string GenerateBotName(AIDifficulty Difficulty, string Recipe = "")
        {
            Initialize();

            return GenerateWizardName();
        }
        public static string GeneratePlayerName(int MaxLength = 10)
        {
            Initialize();

            int MaxTries = 100;
            string name = GenerateWizardName();

            while (name.Length > MaxLength && MaxTries-->0) name = GenerateWizardName();

            return name;
        }

        public static string GenerateWizardName(string Recipe = "")
        {
            Initialize();

            string name = "";

            if (Recipe.Length == 0)
                Recipe = RandomObject.RandomWeightedItem(WizardNameFormulas);

            name = Recipe;

            //It's possible there could be some optimizations here, but should be fine for now.

            if (Recipe.Contains("[First]")) name = name.Replace("[First]", RandomObject.RandomWeightedItem(FirstSyllables));
            if (Recipe.Contains("[Mid]")) name = name.Replace("[Mid]", RandomObject.RandomWeightedItem(MidSyllables));
            if (Recipe.Contains("[Last]")) name = name.Replace("[Last]", RandomObject.RandomWeightedItem(LastSyllables));
            if (Recipe.Contains("[GeneralPrefix]")) name = name.Replace("[GeneralPrefix]", RandomObject.RandomItem(GeneralPrefixes));
            if (Recipe.Contains("[FourzyWord]")) name = name.Replace("[FourzyWord]", RandomObject.RandomItem(FourzyObjectWords));
            if (Recipe.Contains("[Four]")) name = name.Replace("[Four]", RandomObject.RandomItem(TheNumberFour));
            if (Recipe.Contains("[Animal]")) name = name.Replace("[Animal]", RandomObject.RandomItem(Animals));
            if (Recipe.Contains("[AnimalPart]")) name = name.Replace("[AnimalPart]", RandomObject.RandomItem(AnimalPart));
            if (Recipe.Contains("[BodyPart]")) name = name.Replace("[BodyPart]", RandomObject.RandomItem(BodyPart));
            if (Recipe.Contains("[PowerPrefix]")) name = name.Replace("[PowerPrefix]", RandomObject.RandomItem(PowerNamesBefore));
            if (Recipe.Contains("[PowerSuffix]")) name = name.Replace("[PowerSuffix]", RandomObject.RandomItem(PowerNamesAfter));
            if (Recipe.Contains("[Manipulate]")) name = name.Replace("[Manipulate]", RandomObject.RandomItem(Manipulate));
            if (Recipe.Contains("[Object]")) name = name.Replace("[Object]", RandomObject.RandomItem(SolidObjects));
            if (Recipe.Contains("[PlaceSuffix]")) name = name.Replace("[PlaceSuffix]", RandomObject.RandomItem(PlaceSuffix));
            if (Recipe.Contains("[PlacePrefix]")) name = name.Replace("[PlacePrefix]", RandomObject.RandomItem(PlacePrefix));
            if (Recipe.Contains("[GeneralSuffix]")) name = name.Replace("[GeneralSuffix]", RandomObject.RandomItem(GeneralSuffixes));
            if (Recipe.Contains("[Substance]")) name = name.Replace("[Substance]", RandomObject.RandomItem(Substance));

            //Just in case
            if (name.Length == 0) name = GenerateThreeSyllableName();
            if (name.Length > 0) name = char.ToUpper(name[0]) + name.ToLower().Substring(1);

            return name;

        }


        //public static string GenerateFullName(string Recipe = "")
        //{
        //    RandomTools r = new RandomTools();
        //    string name = "";

        //    if (Recipe.Length == 0)
        //        Recipe = r.RandomItem(FullNameFormulas);

        //    switch (Recipe)
        //    {
        //        case "[WizardName] The [WizardAdjective]":
        //            name = GenerateWizardName();
        //            //name = GenerateWizardName() + " The " + GenerateWizardAdjective();
        //            break;
        //        case "The [Superlative] [WizardName]":
        //            name = "The " + r.RandomItem(StagePrefix) + " " + GenerateWizardName();
        //            break;
        //        case "[TwoSyllable] [LastName]":
        //            name = GenerateTwoSyllableName() + " " + GenerateWizardName();
        //            break;
        //    }


        //    return name;
        //}

        //public static string GenerateWizardAdjective(AIDifficulty Difficulty = AIDifficulty.Hard)
        //{
        //    RandomTools r = new RandomTools();
        //    string name = "";

        //    switch (Difficulty)
        //    {
        //        case AIDifficulty.Pushover:
        //            name = r.RandomItem(WeakWords);
        //            break;
        //        case AIDifficulty.Easy:
        //            name = r.RandomItem(WeakWords);
        //            break;
        //        case AIDifficulty.Medium:
        //            name = r.RandomItem(MediocreWords);
        //            break;
        //        case AIDifficulty.Hard:
        //            name = r.RandomItem(SmartWords);
        //            break;
        //        case AIDifficulty.Doctor:
        //            name = r.RandomItem(DoctorWords);
        //            break;
        //    }

        //    return name;
        //}







    }
}
