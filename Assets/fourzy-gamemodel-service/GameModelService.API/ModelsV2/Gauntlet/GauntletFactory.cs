using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public static class GauntletFactory
    {

        //public FourzyGame(Player Player1, int GauntletLevel, Area CurrentArea = Area.NONE, int DifficultModifier = -1, GameOptions Options = null)

        public static GameState Create(Player Human, int GauntletLevel, GauntletStatus Status = null, Area CurrentArea = Area.NONE, int DifficultModfier = -1, GameOptions Options = null, string SeedString = "")
        {
            if (SeedString == "") SeedString = Guid.NewGuid().ToString();
            RandomTools Random = new RandomTools(SeedString);

            if (Options == null) Options = new GameOptions();

            GameState State = null;

            if (CurrentArea == Area.NONE)
            {
                while (CurrentArea == Area.NONE
                    || CurrentArea == Area.ARENA
                    || CurrentArea == Area.CASTLE) CurrentArea = Random.RandomArea();
            }

            switch (CurrentArea)
            {
                case Area.TRAINING_GARDEN:
                    State = TrainingGarden(Human, GauntletLevel, DifficultModfier, Options, SeedString);
                    break;
                case Area.ENCHANTED_FOREST:
                    State = EnchantedForest(Human, GauntletLevel, DifficultModfier, Options, SeedString);
                    break;
                case Area.ICE_PALACE:
                    State = IcePalace(Human, GauntletLevel, DifficultModfier, Options, SeedString);
                    break;
                case Area.SANDY_ISLAND:
                    State = SandyIsland(Human, GauntletLevel, DifficultModfier, Options, SeedString);
                    break;
            }

            return State;
        }

        private static GameState TrainingGarden(Player Human, int GauntletLevel, int DifficultyModifier, GameOptions Options, string SeedString)
        {
            //A random AI Player. We'll eventually
            RandomTools Random = new RandomTools(SeedString);
            Player Opponent = new Player(2, Constants.GenerateName());

            int FirstPlayerId = 1;
            string Recipe = "";
            //This is temporary. We'll pass in a gauntlet configuration in a future version.
            switch (GauntletLevel)
            {
                case 0:
                    Opponent.Profile = AIPlayerFactory.RandomProfile(AIDifficulty.Pushover);
                    Recipe = Random.RandomItem(
                        new List<string>() { "Empty1", "Empty2", "Empty3", "Empty4",
                            "Center", "Simple1", "Simple2", "Simple3", "StickyBlob",
                            "OneFullLineOfGoop", "OneFullLineOfFruit"});

                    break;
                case 1:
                    Opponent.Profile = AIPlayerFactory.RandomProfile(AIDifficulty.Easy);

                    Recipe = Random.RandomItem(
                         new List<string>() { "Orchard", "FruitBlob", "RiverOfFruit", "StickyRiver3",
                             "TheZero", "WideCross", "CenterFruit",
                              "TwoLinesOfFruit"});

                    break;

                case 2:
                    Opponent.Profile = AIPlayerFactory.RandomProfile(AIDifficulty.Medium);

                    Recipe = Random.RandomItem(
                         new List<string>() { "AllysGarden", "ArrowFours", "BellasGarden",
                             "BlockerFours", "CenterFourWithArrows", "BellasGarden", "WideCross", "CenterTarget" });

                    break;

                case 3:
                    Opponent.Profile = AIPlayerFactory.RandomProfile(AIDifficulty.Hard);
                    FirstPlayerId = 1;

                    Recipe = Random.RandomItem(
                         new List<string>() { "DoubleFruitRiver", "EdgeBumps", "EdgeSpikes",
                             "DiagArrows", "CenterFruit", "Diagonals", "StickyTiles", "StickyTiles2" });

                    break;

                case 4:
                    Opponent.Profile = AIPlayerFactory.RandomProfile(AIDifficulty.Doctor);
                    FirstPlayerId = 2;

                    Recipe = Random.RandomItem(
                         new List<string>() { "EdgeBumps", "CheckeredDirection", "LargeCheckers", "HalfSpikes", "Swirly", "SymmetricArrows" });

                    break;

                default:
                    Opponent.Profile = AIPlayerFactory.RandomProfile(AIDifficulty.Hard);

                    break;
            }

            if (Options == null) Options = new GameOptions();

            GameBoard Board = BoardFactory.CreateRandomBoard(Options, Human, Opponent, Area.TRAINING_GARDEN, Recipe);

            GameState State = new GameState(Board, Options, FirstPlayerId);

            //Remove this eventually but helpful for testing.
            Opponent.DisplayName = Opponent.Profile.ToString();


            State.Players.Add(1, Human);
            State.Players.Add(2, Opponent);

            return State;
        }

        private static GameState EnchantedForest(Player Human, int GauntletLevel, int DifficultyModifier, GameOptions Options, string SeedString)
        {
            return null;
        }

        private static GameState SandyIsland(Player Human, int GauntletLevel, int DifficultyModifier, GameOptions Options, string SeedString)
        {
            return null;
        }

        private static GameState IcePalace(Player Human, int GauntletLevel, int DifficultyModifier, GameOptions Options, string SeedString)
        {
            return null;
        }


    }
}
