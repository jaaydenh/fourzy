using System.Collections;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{

public class BeginnerGardenRandomGenerator : BoardGenerator
{
        public override string Name { get { return "Beginner Garden"; } }
        public override Area Area { get { return Area.TRAINING_GARDEN; } }
        public override int MinComplexity { get { return 250; } }
        public override int MaxComplexity { get { return 900; } }

        public BeginnerGardenRandomGenerator(GameOptions Options = null, BoardGenerationPreferences Preferences = null)
    {
            if (Options == null) Options = new GameOptions();
            this.Options = Options;

            if (Preferences == null) Preferences = new BoardGenerationPreferences();
            this.Preferences = Preferences;

            this.SeedString = Preferences.RecipeSeed;
            this.Recipes = new Dictionary<BoardRecipe, int>();

            //Easy
            this.Recipes.Add(SimpleArrow1(), 2);
            this.Recipes.Add(SimpleArrow2(), 2);
            this.Recipes.Add(SimpleArrow3(), 2);
            this.Recipes.Add(SimpleArrow4(), 2);
            this.Recipes.Add(SimpleArrow5(), 2);
            this.Recipes.Add(SimpleArrow6(), 2);
            this.Recipes.Add(Simple1(), 2);
            this.Recipes.Add(Simple2(), 2);
            this.Recipes.Add(Simple3(), 2);
            this.Recipes.Add(SimpleGarden(), 10);
            this.Recipes.Add(Empty1(), 2);
            this.Recipes.Add(Empty2(), 2);
            this.Recipes.Add(Empty3(), 2);
            this.Recipes.Add(Empty4(), 2);
            this.Recipes.Add(CenterFourWithArrows(), 10);
            this.Recipes.Add(CenterFourWithBlocking(), 10);
            this.Recipes.Add(CenterGarden(), 10);
            this.Recipes.Add(CenterTree(), 10);
            this.Recipes.Add(CenterFour1(), 10);
            this.Recipes.Add(CenterFour2(), 10);
            this.Recipes.Add(CenterFour3(), 10);

            this.Recipes.Add(OneLineOfFruit(), 10);
            this.Recipes.Add(OneLineOfGoop(), 10);
            this.Recipes.Add(FruitBlob(), 10);
            this.Recipes.Add(StickyBlob(), 10);
            this.Recipes.Add(StickyBlobWithBlocker(), 10);
            this.Recipes.Add(OrchardSimple1(), 2);
            this.Recipes.Add(OrchardSimple2(), 2);
            this.Recipes.Add(OrchardSimple3(), 2);

            //Medium
            this.Recipes.Add(TheZero(), 10);
            this.Recipes.Add(SimpleArrows(), 10);
            this.Recipes.Add(TwoLinesOfFruit(), 10);
            this.Recipes.Add(FullLines(), 10);
            this.Recipes.Add(OrchardRecipe(), 10);
            this.Recipes.Add(BellasGardenRecipe1(), 10);
            this.Recipes.Add(BellasGardenRecipe2(), 10);
            this.Recipes.Add(BellasGardenRecipe3(), 10);

            this.Recipes.Add(AllysGardenRecipe(), 10);
            this.Recipes.Add(PortiasGardenRecipe(), 10);
            this.Recipes.Add(Lucy(), 10);
            this.Recipes.Add(SpinGardenRecipe(), 10);
            this.Recipes.Add(SwirlyGarden(), 10);
            this.Recipes.Add(WideCross(), 10);
            this.Recipes.Add(LargeCheckers(), 10);
            this.Recipes.Add(LinesOfFruit(), 10);
            this.Recipes.Add(CenterFruit(), 10);
            this.Recipes.Add(CheckeredDirection(), 10);
            this.Recipes.Add(GardenSpot(), 10);
            this.Recipes.Add(ArrowCycles(), 10);
            this.Recipes.Add(LineOfArrows(), 10);
            this.Recipes.Add(Diagonals(), 10);
            this.Recipes.Add(DiagArrows(), 10);
            this.Recipes.Add(SymmArrows(), 10);
            this.Recipes.Add(CenterTarget(), 10);
            this.Recipes.Add(ThreeFours(), 10);
            this.Recipes.Add(ArrowFours(), 10);
            this.Recipes.Add(BlockerFour(), 10);
            this.Recipes.Add(EdgeBumps(), 10);
            this.Recipes.Add(EdgeSpikes(), 10);
            this.Recipes.Add(HalfSpikes(), 10);
            this.Recipes.Add(HalfArrows(), 10);
            this.Recipes.Add(HalfStickyFruit(), 10);
            this.Recipes.Add(StickyTiles(), 10);
            this.Recipes.Add(StickyTiles2(), 20);
            this.Recipes.Add(MiniCircles(), 10);
            this.Recipes.Add(StickyRiverRecipe(), 30);
            this.Recipes.Add(StickyRiver3(), 30);
            this.Recipes.Add(StickyRiver2(), 30);
            this.Recipes.Add(DoubleStickyRiver2(), 30);
        }

        //SIMPLE
        private BoardRecipe SimpleArrow1()
        {
            BoardRecipe Garden = new BoardRecipe("SimpleArrow1",268,276);
            Garden.AddIngredient(new ArrowWingFeature());
            Garden.AddIngredient(new ArrowWingFeature());

            return Garden;
        }

        private BoardRecipe SimpleArrow2()
        {
            BoardRecipe Garden = new BoardRecipe("SimpleArrow2",266,268);
            Garden.AddIngredient(new DoubleArrowFeature());
            Garden.AddIngredient(new DoubleArrowFeature());
            return Garden;
        }

        private BoardRecipe SimpleArrow3()
        {
            BoardRecipe Garden = new BoardRecipe("SimpleArrow3",268,272);
            Garden.AddIngredient(new ArrowTurnFeature());
            Garden.AddIngredient(new ArrowTurnFeature());
            return Garden;
        }

        private BoardRecipe SimpleArrow4()
        {
            BoardRecipe Garden = new BoardRecipe("SimpleArrow4",268,272);
            Garden.AddIngredient(new ArrowTurnFeature());
            Garden.AddIngredient(new ArrowTurnFeature());
            return Garden;
        }

        private BoardRecipe SimpleArrow5()
        {
            BoardRecipe Garden = new BoardRecipe("SimpleArrow5",274,278);
            Garden.AddIngredient(new ArrowDoubleTurnFeature());
            return Garden;
        }

        private BoardRecipe SimpleArrow6()
        {
            BoardRecipe Garden = new BoardRecipe("SimpleArrow6",274,278);
            Garden.AddIngredient(new ArrowSingularityFeature());
            Garden.AddIngredient(new ArrowWingFeature());

            return Garden;
        }

        private BoardRecipe Empty1()
        {
            BoardRecipe Garden = new BoardRecipe("Empty1",336, 336);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.CenterOne, AddTokenMethod.ALWAYS, true));
            return Garden;
        }
        private BoardRecipe Empty2()
        {
            BoardRecipe Garden = new BoardRecipe("Empty2",362,362);
            Garden.AddIngredient(new Ingredient(new BlockerToken(), PatternType.CenterOne, AddTokenMethod.ALWAYS, true));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.Four, AddTokenMethod.ONLY_TERRAIN, true));
            return Garden;
        }

        private BoardRecipe Empty3()
        {
            BoardRecipe Garden = new BoardRecipe("Empty3",482,490);
            Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.CenterOne, AddTokenMethod.ALWAYS, true));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.Four, AddTokenMethod.ONLY_TERRAIN, true));
            return Garden;
        }

        private BoardRecipe Empty4()
        {
            BoardRecipe Garden = new BoardRecipe("Empty4",266,276);
            Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            Garden.AddIngredient(new Ingredient(new BlockerToken(), PatternType.SmallSymmetricBlockEdgePattern, AddTokenMethod.ALWAYS, true));
            return Garden;
        }

        private BoardRecipe Simple1()
        {
            BoardRecipe Garden = new BoardRecipe("Simple1",360,392);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.Four, AddTokenMethod.ALWAYS, true));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.Four, AddTokenMethod.ALWAYS, true));
            return Garden;
        }

        private BoardRecipe Simple2()
        {
            BoardRecipe Garden = new BoardRecipe("Simple2",356,364);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS, true));
            Garden.AddIngredient(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Garden;
        }

        private BoardRecipe Simple3()
        {
            BoardRecipe Garden = new BoardRecipe("Simple3",556,588);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.WideLine, AddTokenMethod.ALWAYS, true));
            Garden.AddIngredient(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Garden;
        }

        private BoardRecipe OneLineOfGoop()
        {
            BoardRecipe Garden = new BoardRecipe("OneFullLineOfGoop",136,392);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.OneFullLine));

            return Garden;
        }

        private BoardRecipe OneLineOfFruit()
        {
            BoardRecipe Garden = new BoardRecipe("OneFullLineOfFruit",136,352);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.OneFullLine));

            return Garden;
        }

        private BoardRecipe CenterTree()
        {
            BoardRecipe Garden = new BoardRecipe("CenterTree",597,600);
            Garden.AddIngredient(new Ingredient(new FruitTreeToken(), PatternType.CenterOne));
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.TwoRandom));
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.TwoRandom));
            return Garden;
        }

        private BoardRecipe CenterFour1()
        {
            BoardRecipe Garden = new BoardRecipe("CenterLineOfFour1",360,360);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.CenterLineOfFour));
            return Garden;
        }

        private BoardRecipe CenterFour2()
        {
            BoardRecipe Garden = new BoardRecipe("CenterLineOfFour2",360,392);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.TwoCenterLinesOfFour));

            return Garden;
        }

        private BoardRecipe CenterFour3()
        {
            BoardRecipe Garden = new BoardRecipe("CenterLineOfFour3",384,424);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.ThreeCenterLinesOfFour));
            return Garden;
        }



        //MEDIUM

        private BoardRecipe TwoLinesOfFruit()
        {
            BoardRecipe Garden = new BoardRecipe("TwoLinesOfFruit",136,376);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.OneFullLine));
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.OneFullLine));

            return Garden;
        }

        private BoardRecipe TheZero()
        {
            BoardRecipe Garden = new BoardRecipe("TheZero",424,488);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.CenterRing, AddTokenMethod.ALWAYS, true));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.OneFullLine, AddTokenMethod.ALWAYS, true));
            return Garden;
        }


        private BoardRecipe MiniCircles()
        {
            BoardRecipe Garden = new BoardRecipe("MiniCircles",865,865);
            Garden.AddIngredient(new TileFeature(new StickyToken(), 4, 4, "1110101111100100", -1, -1));
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.AlmostFull, AddTokenMethod.EMPTY, true));

            return Garden;
        }


        private BoardRecipe StickyTiles2()
        {
            BoardRecipe Garden = new BoardRecipe("StickyTiles2",392,616);
            Garden.AddIngredient(new TileFeature(new StickyToken(),-1,-1,"",-1,-1));

            return Garden;
        }


        private BoardRecipe StickyTiles()
        {
            BoardRecipe Garden = new BoardRecipe("StickyTiles",456,632);
            Garden.AddIngredient(new TileFeature(new StickyToken()));

            return Garden;
        }

        private BoardRecipe HalfStickyFruit()
        {
            BoardRecipe Garden = new BoardRecipe("HalfStickyFruit",136,850);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.Half, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.Half, AddTokenMethod.EMPTY, true));

            return Garden;
        }

        private BoardRecipe HalfArrows()
        {
            BoardRecipe Garden = new BoardRecipe("HalfArrows",136,606);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.Half, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.Half, AddTokenMethod.EMPTY, true));

            return Garden;
        }

        private BoardRecipe HalfSpikes()
        {
            BoardRecipe Garden = new BoardRecipe("HalfSpikes",528,560);
            Garden.AddIngredient(new ArrowCycleFeature(Rotation.CLOCKWISE, new BoardLocation(3, 3), 2, 2));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.EdgeSpikeDuo, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.EdgeSpikeDuo, AddTokenMethod.EMPTY, true));
            //Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.Half, AddTokenMethod.EMPTY, true));
            Garden.ContainsRandom = true;

            return Garden;
        }

        private BoardRecipe EdgeSpikes()
        {
            BoardRecipe Garden = new BoardRecipe("EdgeSpikes",717,734);
            Garden.AddIngredient(new ArrowCycleFeature(Rotation.CLOCKWISE, new BoardLocation(3, 3), 2, 2));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.EdgeSpikeDuo, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.EdgeSpikeDuo, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.Four, AddTokenMethod.EMPTY, true));
            Garden.ContainsRandom = true;

            return Garden;
        }

        private BoardRecipe EdgeBumps()
        {
            BoardRecipe Garden = new BoardRecipe("EdgeBumps",580,604);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.EdgeBumpDuo, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new BlockerToken(), PatternType.EdgeBump, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.Four, AddTokenMethod.EMPTY, true));

            return Garden;
        }

        private BoardRecipe BlockerFour()
        {
            BoardRecipe Garden = new BoardRecipe("BlockerFours",408,612);
            Garden.AddIngredient(new Ingredient(new FruitTreeToken(), PatternType.Four, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.Four, AddTokenMethod.EMPTY, true));

            return Garden;
        }


        private BoardRecipe ArrowFours()
        {
            BoardRecipe Garden = new BoardRecipe("ArrowFours",272,484);
            Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.Four, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.Four, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.Four, AddTokenMethod.EMPTY, true));

            return Garden;
        }

        private BoardRecipe ThreeFours()
        {
            BoardRecipe Garden = new BoardRecipe("ThreeFours",360,576);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.Four, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.Four, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.Four, AddTokenMethod.EMPTY, true));

            return Garden;
        }

        private BoardRecipe CenterTarget()
        {
            BoardRecipe Garden = new BoardRecipe("CenterTarget",692,844);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.CenterFour, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.OuterRing, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.TwoRandom, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new SymmetricArrowsFeature());

            return Garden;
        }


        private BoardRecipe SymmArrows()
        {
            BoardRecipe Garden = new BoardRecipe("SymmetricArrows",360,504);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.CenterFour, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new SymmetricArrowsFeature(true,2));

            return Garden;
        }

        private BoardRecipe CenterFourWithBlocking()
        {
            BoardRecipe Garden = new BoardRecipe("CenterFourWithBlocking", 561,572);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.CenterFour, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new BlockerToken(), PatternType.SmallSymmetricBlockEdgePattern, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.Four, AddTokenMethod.EMPTY, true));

            return Garden;
        }

        private BoardRecipe CenterFourWithArrows()
        {
            BoardRecipe Garden = new BoardRecipe("CenterFourWithArrows", 349,492);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.CenterFour, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.Four, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new ArrowsBlockSideFeature());

            return Garden;
        }



        private BoardRecipe DiagArrows()
        {
            BoardRecipe Garden = new BoardRecipe("DiagArrows",480,486);
            Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.CenterDiagonal, AddTokenMethod.ALWAYS, true));
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.ACouple, AddTokenMethod.EMPTY, true));

            return Garden;
        }

        private BoardRecipe FullLines()
        {
            BoardRecipe Garden = new BoardRecipe("FullLines",418,418);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.FullLines));

            return Garden;
        }

        private BoardRecipe FruitBlob()
        {
            BoardRecipe Garden = new BoardRecipe("FruitBlob",388,424);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.CenterBlob));

            return Garden;
        }

        private BoardRecipe StickyBlob()
        {
            BoardRecipe Garden = new BoardRecipe("StickyBlob",472,584);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.CenterBlob));

            return Garden;
        }

        private BoardRecipe StickyBlobWithBlocker()
        {
            BoardRecipe Garden = new BoardRecipe("StickyBlobWithBlocker",490,586);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.CenterBlob));
            Garden.AddIngredient(new Ingredient(new BlockerToken(), PatternType.CenterOne, AddTokenMethod.ALWAYS));

            return Garden;
        }
        
        private BoardRecipe Diagonals()
        {
            BoardRecipe Garden = new BoardRecipe("Diagonals",358,520);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.DiagonalLines));

            Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.Four, AddTokenMethod.EMPTY));
            Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.Four, AddTokenMethod.EMPTY));

            return Garden;
        }


        private BoardRecipe LineOfArrows()
        {
            BoardRecipe Garden = new BoardRecipe("LineOfArrows",456,600);
            Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.OneFullLine, AddTokenMethod.EMPTY));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.CenterSixteen, AddTokenMethod.EMPTY));

            return Garden;
        }

        private BoardRecipe CheckeredDirection()
        {
            BoardRecipe Garden = new BoardRecipe("CheckeredDirection",526,526);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.AlmostFullCheckers, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.CenterSixteen, AddTokenMethod.EMPTY));

            return Garden;
        }

        private BoardRecipe GardenSpot()
        {
            BoardRecipe Garden = new BoardRecipe("GardenSpot", 764, 764);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.CenterFour));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.OuterCorners));
            Garden.AddIngredient(new ArrowCycleFeature(Rotation.NONE,new BoardLocation(2,2),4,4));

            Garden.ContainsRandom = true;
            return Garden;
        }


        private BoardRecipe ArrowCycles()
        {
            BoardRecipe Garden = new BoardRecipe("Cycles",508,512);
            Garden.AddIngredient(new ArrowCycleFeature());
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.CenterDiagonal,AddTokenMethod.ALWAYS, true));
            Garden.ContainsRandom = true;

            return Garden;
        }

        private BoardRecipe SimpleArrows()
        {
            BoardRecipe Garden = new BoardRecipe("SimpleArrows",340,484);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.CenterFour));
            Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.UP), PatternType.DottedLine));
            Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.DOWN), PatternType.DottedLine));
            return Garden;
        }


        private BoardRecipe CenterFruit()
        {
            BoardRecipe Garden = new BoardRecipe("CenterFruit",376,516);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.CenterSixteen));
            Garden.AddIngredient(new ArrowsBlockSideFeature());
            return Garden;
        }

        private BoardRecipe LinesOfFruit()
        {
            BoardRecipe Garden = new BoardRecipe("LinesOfFruit",418,642);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.FullLines));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.FillGaps));
            return Garden;
        }

        private BoardRecipe LargeCheckers()
        {
            BoardRecipe Garden = new BoardRecipe("LargeCheckers", 688,720);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.LargeCheckers));
            Garden.AddIngredient(new HighwayFeature(LineDirection.NONE, 0, 0, true, 1, true, AddTokenMethod.EMPTY, false));
            return Garden;
        }

        private BoardRecipe WideCross()
        {
            BoardRecipe Garden = new BoardRecipe("WideCross",526,560);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.DoubleCross));
            Garden.AddIngredient(new Ingredient(new BlockerToken(), PatternType.CentralCorners, AddTokenMethod.ALWAYS, true));
            return Garden;
        }

        private BoardRecipe CenterGarden()
        {
            BoardRecipe Garden = new BoardRecipe("Center",456,592);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.CenterSixteen));
            Garden.AddIngredient(new SymmetricArrowsFeature(true,1));
            //Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.TwoRandom, AddTokenMethod.ONLY_TERRAIN, true));

            return Garden;
        }

        private BoardRecipe SimpleGarden()
        {
            BoardRecipe Garden = new BoardRecipe("Simple",343,476);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.ACouple));
            Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.CenterOne));

            //Garden.AddIngredient(new HighwayFeature(LineDirection.NONE, 0, 0, false, 1, false, AddTokenMethod.EMPTY));

            return Garden;
        }

        private BoardRecipe SwirlyGarden()
        {
            BoardRecipe Garden = new BoardRecipe("Swirly",576,576);
            Garden.AddIngredient(new ArrowPinWheelFeature());
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.FullDots, AddTokenMethod.EMPTY, false));
            
            return Garden;
        }

        private BoardRecipe OrchardSimple1()
        {
            BoardRecipe Garden = new BoardRecipe("OrchardSimple1",396,396);
            Garden.AddIngredient(new Ingredient(new FruitTreeToken(), PatternType.CenterOne, AddTokenMethod.EMPTY, false));
            return Garden;
        }

        private BoardRecipe OrchardSimple2()
        {
            BoardRecipe Garden = new BoardRecipe("OrchardSimple2",396,400);
            Garden.AddIngredient(new Ingredient(new FruitTreeToken(), PatternType.TwoRandom, AddTokenMethod.EMPTY, false));
            return Garden;
        }

        private BoardRecipe OrchardSimple3()
        {
            BoardRecipe Garden = new BoardRecipe("OrchardSimple3",136,408);
            Garden.AddIngredient(new Ingredient(new FruitTreeToken(), PatternType.DottedLine, AddTokenMethod.EMPTY, false));
            return Garden;
        }

        private BoardRecipe OrchardRecipe()
        {
            BoardRecipe Garden = new BoardRecipe("Orchard",613,872);
            Garden.AddIngredient(new ASmallGroveFeature());
            Garden.AddIngredient(new TreesDropFruitFeature(2));

            return Garden; 
    }


    private BoardRecipe BellasGardenRecipe1()
    {
            BoardRecipe Garden = new BoardRecipe("BellasGarden",620,745);
            Garden.AddIngredient(new Ingredient(new StickyToken(),PatternType.BlockAllEdges, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new FruitToken(),PatternType.HorizontalLine, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new FruitToken(),PatternType.VerticalLine, AddTokenMethod.EMPTY, true));
            return Garden;
    }

        private BoardRecipe BellasGardenRecipe2()
        {
            BoardRecipe Garden = new BoardRecipe("BellasGarden2",536,576);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.BlockAllEdges, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.CenterOne, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.ACouple, AddTokenMethod.EMPTY, true));
            return Garden;
        }

        private BoardRecipe BellasGardenRecipe3()
        {
            BoardRecipe Garden = new BoardRecipe("BellasGarden3",654,662);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.BlockAllEdges, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new BlockerToken(), PatternType.CenterOne, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.ACouple, AddTokenMethod.EMPTY, true));
            return Garden;
        }


        private BoardRecipe AllysGardenRecipe()
    {
            BoardRecipe Garden = new BoardRecipe("AllysGarden",472,612);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.AlmostFullCheckers, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new ArrowsBlockSideFeature());
            //InArrowFeature IAF = new InArrowFeature();
            //IAF.AddMethod = AddTokenMethod.EMPTY;
            //Garden.AddIngredient(IAF);
            //Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.RIGHT), PatternType.FillGaps, AddTokenMethod.EMPTY, true));
            //Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.LEFT), PatternType.FillGaps, AddTokenMethod.EMPTY, true));

            return Garden;
    }

    private BoardRecipe PortiasGardenRecipe()
     {
         BoardRecipe Garden = new BoardRecipe("PortiasGarden",364,400);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.SmallPinWheel));
         return Garden;
    }

    private BoardRecipe SpinGardenRecipe()
    {
            BoardRecipe Garden = new BoardRecipe("SpinGarden",426,520);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.SmallPinWheel));
            return Garden;
    }

        private BoardRecipe StickyRiver3()
        {
            BoardRecipe StickyRiver = new BoardRecipe("StickyRiver3",416,536);
            StickyRiver.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CrossBoardFourTurnPattern, AddTokenMethod.EMPTY, true));
            StickyRiver.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.MediumSymmetricBlockEdgePattern));
            return StickyRiver;
        }

        private BoardRecipe DoubleStickyRiver2()
        {
            BoardRecipe Garden = new BoardRecipe("DoubleFruitRiver",628,660);
            Garden.AddIngredient(new ASmallGroveFeature());
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.CrossBoardTwoTurnPattern, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.CrossBoardTwoTurnPattern, AddTokenMethod.EMPTY, true));
            return Garden;
        }

        private BoardRecipe StickyRiver2()
        {
            BoardRecipe Garden = new BoardRecipe("StickyRiver2");
            Garden.AddIngredient(new Ingredient(new BlockerToken(), PatternType.SmallSymmetricBlockEdgePattern));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.CrossBoardFourTurnPattern, AddTokenMethod.ALWAYS, true));
            return Garden;
        }

        private BoardRecipe StickyRiverRecipe()
        {
            BoardRecipe Garden = new BoardRecipe("StickyRiver",408,472);
            Garden.AddIngredient(new Ingredient(new FruitToken(), PatternType.FullDots, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.CrossBoardFourTurnPattern, AddTokenMethod.ALWAYS, true));
            return Garden;
        }
               
        private BoardRecipe Lucy()
        {
            BoardRecipe Garden = new BoardRecipe("Lucy",582,592);
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.FullDots, AddTokenMethod.EMPTY, true));

            CrossArrowFeature f = new CrossArrowFeature();
            f.AddMethod= AddTokenMethod.EMPTY;
            f.ReplaceTokens = true;
            Garden.AddIngredient(f);            

            return Garden;
        }

        private BoardRecipe Lucy2()
        {
            BoardRecipe Garden = new BoardRecipe("Lucy2");
            Garden.AddIngredient(new Ingredient(new StickyToken(), PatternType.FullDots, AddTokenMethod.EMPTY, true));
            Garden.AddIngredient(new Ingredient(new BlockerToken(), PatternType.CenterOne, AddTokenMethod.ALWAYS, true));
            Garden.AddIngredient(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            
            return Garden;
        }





    }

}

