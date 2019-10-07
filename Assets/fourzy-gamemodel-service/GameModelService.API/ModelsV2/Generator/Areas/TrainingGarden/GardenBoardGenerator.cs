using System.Collections;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{

public class BeginnerGardenRandomGenerator : BoardGenerator
{

        public string Name = "Beginner Garden";
        public override Area Area { get { return Area.TRAINING_GARDEN; } }

    public BeginnerGardenRandomGenerator(string RequestedRecipeName = "")
    {
            this.RequestedRecipeName = RequestedRecipeName;
            this.Recipes = new Dictionary<BoardRecipe, int>();

            //Easy
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

        private BoardRecipe Empty1()
        {
            BoardRecipe Garden = new BoardRecipe("Empty1");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterOne, AddTokenMethod.ALWAYS, true));
            return Garden;
        }
        private BoardRecipe Empty2()
        {
            BoardRecipe Garden = new BoardRecipe("Empty2");
            Garden.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.CenterOne, AddTokenMethod.ALWAYS, true));
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.Four, AddTokenMethod.ONLY_TERRAIN, true));
            return Garden;
        }

        private BoardRecipe Empty3()
        {
            BoardRecipe Garden = new BoardRecipe("Empty3");
            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.CenterOne, AddTokenMethod.ALWAYS, true));
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.Four, AddTokenMethod.ONLY_TERRAIN, true));
            return Garden;
        }

        private BoardRecipe Empty4()
        {
            BoardRecipe Garden = new BoardRecipe("Empty4");
            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            Garden.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.SmallSymmetricBlockEdgePattern, AddTokenMethod.ALWAYS, true));
            return Garden;
        }

        private BoardRecipe Simple1()
        {
            BoardRecipe Garden = new BoardRecipe("Simple1");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.Four, AddTokenMethod.ALWAYS, true));
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.Four, AddTokenMethod.ALWAYS, true));
            return Garden;
        }

        private BoardRecipe Simple2()
        {
            BoardRecipe Garden = new BoardRecipe("Simple2");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS, true));
            Garden.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Garden;
        }

        private BoardRecipe Simple3()
        {
            BoardRecipe Garden = new BoardRecipe("Simple3");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.WideLine, AddTokenMethod.ALWAYS, true));
            Garden.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Garden;
        }

        private BoardRecipe OneLineOfGoop()
        {
            BoardRecipe Garden = new BoardRecipe("OneFullLineOfGoop");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.OneFullLine));

            return Garden;
        }

        private BoardRecipe OneLineOfFruit()
        {
            BoardRecipe Garden = new BoardRecipe("OneFullLineOfFruit");
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.OneFullLine));

            return Garden;
        }

        private BoardRecipe CenterTree()
        {
            BoardRecipe Garden = new BoardRecipe("CenterTree");
            Garden.Ingredients.Add(new Ingredient(new FruitTreeToken(), PatternType.CenterOne));
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.TwoRandom));
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.TwoRandom));
            return Garden;
        }

        private BoardRecipe CenterFour1()
        {
            BoardRecipe Garden = new BoardRecipe("CenterLineOfFour1");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterLineOfFour));
            return Garden;
        }

        private BoardRecipe CenterFour2()
        {
            BoardRecipe Garden = new BoardRecipe("CenterLineOfFour2");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.TwoCenterLinesOfFour));

            return Garden;
        }

        private BoardRecipe CenterFour3()
        {
            BoardRecipe Garden = new BoardRecipe("CenterLineOfFour3");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.ThreeCenterLinesOfFour));
            return Garden;
        }



        //MEDIUM

        private BoardRecipe TwoLinesOfFruit()
        {
            BoardRecipe Garden = new BoardRecipe("TwoLinesOfFruit");
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.OneFullLine));
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.OneFullLine));

            return Garden;
        }

        private BoardRecipe TheZero()
        {
            BoardRecipe Garden = new BoardRecipe("TheZero");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterRing, AddTokenMethod.ALWAYS, true));
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.OneFullLine, AddTokenMethod.ALWAYS, true));
            return Garden;
        }


        private BoardRecipe MiniCircles()
        {
            BoardRecipe Garden = new BoardRecipe("MiniCircles");
            Garden.Ingredients.Add(new TileFeature(new StickyToken(), 4, 4, "1110101111100100", -1, -1));
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.AlmostFull, AddTokenMethod.EMPTY, true));

            return Garden;
        }


        private BoardRecipe StickyTiles2()
        {
            BoardRecipe Garden = new BoardRecipe("StickyTiles2");
            Garden.Ingredients.Add(new TileFeature(new StickyToken(),-1,-1,"",-1,-1));

            return Garden;
        }


        private BoardRecipe StickyTiles()
        {
            BoardRecipe Garden = new BoardRecipe("StickyTiles");
            Garden.Ingredients.Add(new TileFeature(new StickyToken()));

            return Garden;
        }

        private BoardRecipe HalfStickyFruit()
        {
            BoardRecipe Garden = new BoardRecipe("HalfStickyFruit");
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.Half, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.Half, AddTokenMethod.EMPTY, true));

            return Garden;
        }

        private BoardRecipe HalfArrows()
        {
            BoardRecipe Garden = new BoardRecipe("HalfArrows");
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.Half, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.Half, AddTokenMethod.EMPTY, true));

            return Garden;
        }

        private BoardRecipe HalfSpikes()
        {
            BoardRecipe Garden = new BoardRecipe("HalfSpikes");
            Garden.Ingredients.Add(new ArrowCycleFeature(Rotation.CLOCKWISE, new BoardLocation(3, 3), 2, 2));
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.EdgeSpikeDuo, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.EdgeSpikeDuo, AddTokenMethod.EMPTY, true));
            //Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.Half, AddTokenMethod.EMPTY, true));

            return Garden;
        }

        private BoardRecipe EdgeSpikes()
        {
            BoardRecipe Garden = new BoardRecipe("EdgeSpikes");
            Garden.Ingredients.Add(new ArrowCycleFeature(Rotation.CLOCKWISE, new BoardLocation(3, 3), 2, 2));
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.EdgeSpikeDuo, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.EdgeSpikeDuo, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.Four, AddTokenMethod.EMPTY, true));

            return Garden;
        }

        private BoardRecipe EdgeBumps()
        {
            BoardRecipe Garden = new BoardRecipe("EdgeBumps");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.EdgeBumpDuo, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.EdgeBump, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.Four, AddTokenMethod.EMPTY, true));

            return Garden;
        }

        private BoardRecipe BlockerFour()
        {
            BoardRecipe Garden = new BoardRecipe("BlockerFours");
            Garden.Ingredients.Add(new Ingredient(new FruitTreeToken(), PatternType.Four, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.Four, AddTokenMethod.EMPTY, true));

            return Garden;
        }


        private BoardRecipe ArrowFours()
        {
            BoardRecipe Garden = new BoardRecipe("ArrowFours");
            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.Four, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.Four, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.Four, AddTokenMethod.EMPTY, true));

            return Garden;
        }

        private BoardRecipe ThreeFours()
        {
            BoardRecipe Garden = new BoardRecipe("ThreeFours");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.Four, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.Four, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.Four, AddTokenMethod.EMPTY, true));

            return Garden;
        }

        private BoardRecipe CenterTarget()
        {
            BoardRecipe Garden = new BoardRecipe("CenterTarget");
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.CenterFour, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.OuterRing, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.TwoRandom, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new SymmetricArrowsFeature());

            return Garden;
        }


        private BoardRecipe SymmArrows()
        {
            BoardRecipe Garden = new BoardRecipe("SymmetricArrows");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterFour, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new SymmetricArrowsFeature(true,2));

            return Garden;
        }

        private BoardRecipe CenterFourWithBlocking()
        {
            BoardRecipe Garden = new BoardRecipe("CenterFourWithBlocking");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterFour, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.SmallSymmetricBlockEdgePattern, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.Four, AddTokenMethod.EMPTY, true));

            return Garden;
        }

        private BoardRecipe CenterFourWithArrows()
        {
            BoardRecipe Garden = new BoardRecipe("CenterFourWithArrows");
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.CenterFour, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.Four, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new ArrowsBlockSideFeature());

            return Garden;
        }



        private BoardRecipe DiagArrows()
        {
            BoardRecipe Garden = new BoardRecipe("DiagArrows");
            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.CenterDiagonal, AddTokenMethod.ALWAYS, true));
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.ACouple, AddTokenMethod.EMPTY, true));

            return Garden;
        }

        private BoardRecipe FullLines()
        {
            BoardRecipe Garden = new BoardRecipe("FullLines");
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.FullLines));

            return Garden;
        }

        private BoardRecipe FruitBlob()
        {
            BoardRecipe Garden = new BoardRecipe("FruitBlob");
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.CenterBlob));

            return Garden;
        }

        private BoardRecipe StickyBlob()
        {
            BoardRecipe Garden = new BoardRecipe("StickyBlob");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterBlob));

            return Garden;
        }

        private BoardRecipe StickyBlobWithBlocker()
        {
            BoardRecipe Garden = new BoardRecipe("StickyBlobWithBlocker");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterBlob));
            Garden.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.CenterOne, AddTokenMethod.ALWAYS));

            return Garden;
        }
        
        private BoardRecipe Diagonals()
        {
            BoardRecipe Garden = new BoardRecipe("Diagonals");
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.DiagonalLines));

            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.Four, AddTokenMethod.EMPTY));
            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.Four, AddTokenMethod.EMPTY));

            return Garden;
        }


        private BoardRecipe LineOfArrows()
        {
            BoardRecipe Garden = new BoardRecipe("LineOfArrows");
            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.OneFullLine, AddTokenMethod.EMPTY));
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterSixteen, AddTokenMethod.EMPTY));

            return Garden;
        }

        private BoardRecipe CheckeredDirection()
        {
            BoardRecipe Garden = new BoardRecipe("CheckeredDirection");
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.AlmostFullCheckers, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.CenterSixteen, AddTokenMethod.EMPTY));

            return Garden;
        }

        private BoardRecipe GardenSpot()
        {
            BoardRecipe Garden = new BoardRecipe("GardenSpot");
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.CenterFour));
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.OuterCorners));
            Garden.Ingredients.Add(new ArrowCycleFeature(Rotation.NONE,new BoardLocation(2,2),4,4));

            return Garden;
        }


        private BoardRecipe ArrowCycles()
        {
            BoardRecipe Garden = new BoardRecipe("Cycles");
            Garden.Ingredients.Add(new ArrowCycleFeature());
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterDiagonal,AddTokenMethod.ALWAYS, true));
            return Garden;
        }

        private BoardRecipe SimpleArrows()
        {
            BoardRecipe Garden = new BoardRecipe("SimpleArrows");
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.CenterFour));
            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.UP), PatternType.DottedLine));
            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.DOWN), PatternType.DottedLine));
            return Garden;
        }


        private BoardRecipe CenterFruit()
        {
            BoardRecipe Garden = new BoardRecipe("CenterFruit");
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.CenterSixteen));
            Garden.Ingredients.Add(new ArrowsBlockSideFeature());
            return Garden;
        }

        private BoardRecipe LinesOfFruit()
        {
            BoardRecipe Garden = new BoardRecipe("LinesOfFruit");
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.FullLines));
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.FillGaps));
            return Garden;
        }

        private BoardRecipe LargeCheckers()
        {
            BoardRecipe Garden = new BoardRecipe("LargeCheckers");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.LargeCheckers));
            Garden.Ingredients.Add(new HighwayFeature(LineDirection.NONE, 0, 0, true, 1, true, AddTokenMethod.EMPTY, false));
            return Garden;
        }

        private BoardRecipe WideCross()
        {
            BoardRecipe Garden = new BoardRecipe("WideCross");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.DoubleCross));
            Garden.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.CentralCorners, AddTokenMethod.ALWAYS, true));
            return Garden;
        }

        private BoardRecipe CenterGarden()
        {
            BoardRecipe Garden = new BoardRecipe("Center");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterSixteen));
            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.TwoRandom, AddTokenMethod.ONLY_TERRAIN, true));

            return Garden;
        }

        private BoardRecipe SimpleGarden()
        {
            BoardRecipe Garden = new BoardRecipe("Simple");
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.ACouple));
            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.CenterOne));

            //Garden.Ingredients.Add(new HighwayFeature(LineDirection.NONE, 0, 0, false, 1, false, AddTokenMethod.EMPTY));

            return Garden;
        }

        private BoardRecipe SwirlyGarden()
        {
            BoardRecipe Garden = new BoardRecipe("Swirly");
            Garden.Ingredients.Add(new ArrowPinWheelFeature());
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.FullDots, AddTokenMethod.EMPTY, false));
            
            return Garden;
        }


        private BoardRecipe OrchardRecipe()
    {
            BoardRecipe Orchard = new BoardRecipe("Orchard");
            Orchard.Ingredients.Add(new ASmallGroveFeature());
            Orchard.Ingredients.Add(new TreesDropFruitFeature(2));

            return Orchard; 
    }


    private BoardRecipe BellasGardenRecipe1()
    {
            BoardRecipe BellasGarden = new BoardRecipe("BellasGarden");
            BellasGarden.Ingredients.Add(new Ingredient(new StickyToken(),PatternType.BlockAllEdges, AddTokenMethod.EMPTY, true));
            BellasGarden.Ingredients.Add(new Ingredient(new FruitToken(),PatternType.HorizontalLine, AddTokenMethod.EMPTY, true));
            BellasGarden.Ingredients.Add(new Ingredient(new FruitToken(),PatternType.VerticalLine, AddTokenMethod.EMPTY, true));
            return BellasGarden;
    }

        private BoardRecipe BellasGardenRecipe2()
        {
            BoardRecipe BellasGarden = new BoardRecipe("BellasGarden2");
            BellasGarden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.BlockAllEdges, AddTokenMethod.EMPTY, true));
            BellasGarden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterOne, AddTokenMethod.EMPTY, true));
            BellasGarden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.ACouple, AddTokenMethod.EMPTY, true));
            return BellasGarden;
        }

        private BoardRecipe BellasGardenRecipe3()
        {
            BoardRecipe BellasGarden = new BoardRecipe("BellasGarden3");
            BellasGarden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.BlockAllEdges, AddTokenMethod.EMPTY, true));
            BellasGarden.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.CenterOne, AddTokenMethod.EMPTY, true));
            BellasGarden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.ACouple, AddTokenMethod.EMPTY, true));
            return BellasGarden;
        }


        private BoardRecipe AllysGardenRecipe()
    {
            BoardRecipe Garden = new BoardRecipe("AllysGarden");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.AlmostFullCheckers, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new ArrowsBlockSideFeature());
            //InArrowFeature IAF = new InArrowFeature();
            //IAF.AddMethod = AddTokenMethod.EMPTY;
            //Garden.Ingredients.Add(IAF);
            //Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RIGHT), PatternType.FillGaps, AddTokenMethod.EMPTY, true));
            //Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.LEFT), PatternType.FillGaps, AddTokenMethod.EMPTY, true));

            return Garden;
    }

    private BoardRecipe PortiasGardenRecipe()
     {
         BoardRecipe Garden = new BoardRecipe("PortiasGarden");
         Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.SmallPinWheel));
         return Garden;
    }

    private BoardRecipe SpinGardenRecipe()
    {
            BoardRecipe Garden = new BoardRecipe("SpinGarden");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.SmallPinWheel));
            return Garden;
    }

        private BoardRecipe StickyRiver3()
        {
            BoardRecipe StickyRiver = new BoardRecipe("StickyRiver3");
            StickyRiver.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CrossBoardFourTurnPattern, AddTokenMethod.EMPTY, true));
            StickyRiver.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.MediumSymmetricBlockEdgePattern));
            return StickyRiver;
        }

        private BoardRecipe DoubleStickyRiver2()
        {
            BoardRecipe StickyRiver = new BoardRecipe("DoubleFruitRiver");
            StickyRiver.Ingredients.Add(new ASmallGroveFeature());
            StickyRiver.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.CrossBoardTwoTurnPattern, AddTokenMethod.EMPTY, true));
            StickyRiver.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.CrossBoardTwoTurnPattern, AddTokenMethod.EMPTY, true));
            return StickyRiver;
        }

        private BoardRecipe StickyRiver2()
        {
            BoardRecipe StickyRiver = new BoardRecipe("RiverOfFruit");
            StickyRiver.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.SmallSymmetricBlockEdgePattern));
            StickyRiver.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CrossBoardFourTurnPattern, AddTokenMethod.ALWAYS, true));
            return StickyRiver;
        }

        private BoardRecipe StickyRiverRecipe()
        {
            BoardRecipe StickyRiver = new BoardRecipe("RiverOfFruit");
            StickyRiver.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.FullDots, AddTokenMethod.EMPTY, true));
            StickyRiver.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CrossBoardFourTurnPattern, AddTokenMethod.ALWAYS, true));
            return StickyRiver;
        }
               
        private BoardRecipe Lucy()
        {
            BoardRecipe Garden = new BoardRecipe("Lucy");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.FullDots, AddTokenMethod.EMPTY, true));

            CrossArrowFeature f = new CrossArrowFeature();
            f.AddMethod= AddTokenMethod.EMPTY;
            f.ReplaceTokens = true;
            Garden.Ingredients.Add(f);            

            return Garden;
        }

        private BoardRecipe Lucy2()
        {
            BoardRecipe Garden = new BoardRecipe("Lucy2");
            Garden.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.FullDots, AddTokenMethod.EMPTY, true));
            Garden.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.CenterOne, AddTokenMethod.ALWAYS, true));
            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            
            return Garden;
        }





    }

}

