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
            this.Recipes.Add(OrchardRecipe(), 10);
            this.Recipes.Add(BellasGardenRecipe(), 10);
            this.Recipes.Add(AllysGardenRecipe(), 10);
            this.Recipes.Add(PortiasGardenRecipe(), 10);
            this.Recipes.Add(Lucy(), 10);
            //this.Recipes.Add(StickyRiverRecipe(), 10);
            this.Recipes.Add(SpinGardenRecipe(), 10);
            this.Recipes.Add(SimpleGarden(), 10);
            this.Recipes.Add(SwirlyGarden(), 10);
            this.Recipes.Add(CenterGarden(), 10);
            this.Recipes.Add(WideCross(), 10);
            this.Recipes.Add(LargeCheckers(), 10);
            this.Recipes.Add(CenterTree(), 10);
            this.Recipes.Add(LinesOfFruit(), 10);
            this.Recipes.Add(CenterFruit(), 10);
            this.Recipes.Add(SimpleArrows(), 10);

            this.Recipes.Add(CheckeredDirection(), 10);
            this.Recipes.Add(GardenSpot(), 10);
            this.Recipes.Add(ArrowCycles(), 10);
            this.Recipes.Add(LineOfArrows(), 10);
            this.Recipes.Add(Diagonals(), 10);
            this.Recipes.Add(FullLines(), 10);
            this.Recipes.Add(DiagArrows(), 10);
            this.Recipes.Add(CenterFourWithArrows(), 10);
            this.Recipes.Add(CenterFourWithBlocking(), 10);
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
        }


        private BoardRecipe MiniCircles()
        {
            BoardRecipe Garden = new BoardRecipe("StickyTiles2");
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
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.Half, AddTokenMethod.EMPTY, true));

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
            BoardRecipe Garden = new BoardRecipe("CenterFourWithArrows");
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
            BoardRecipe Garden = new BoardRecipe("CenterFruit");
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

        private BoardRecipe CenterTree()
        {
            BoardRecipe Garden = new BoardRecipe("CenterTree");
            Garden.Ingredients.Add(new Ingredient(new FruitTreeToken(), PatternType.CenterFour));
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.TwoRandom));
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
            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RIGHT), PatternType.TwoRandom, AddTokenMethod.ONLY_TERRAIN, true));
            Garden.Ingredients.Add(new Ingredient(new ArrowToken(Direction.LEFT), PatternType.TwoRandom, AddTokenMethod.ONLY_TERRAIN, true));

            return Garden;
        }

        private BoardRecipe SimpleGarden()
        {
            BoardRecipe Garden = new BoardRecipe("Simple");
            Garden.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.ACouple));
            Garden.Ingredients.Add(new HighwayFeature(LineDirection.NONE, 0, 0, false, 1, false, AddTokenMethod.EMPTY));

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


    private BoardRecipe BellasGardenRecipe()
    {
            BoardRecipe BellasGarden = new BoardRecipe("BellasGarden");
            BellasGarden.Ingredients.Add(new Ingredient(new StickyToken(),PatternType.BlockAllEdges, AddTokenMethod.EMPTY, true));
            BellasGarden.Ingredients.Add(new Ingredient(new FruitToken(),PatternType.HorizontalLine, AddTokenMethod.EMPTY, true));
            BellasGarden.Ingredients.Add(new Ingredient(new FruitToken(),PatternType.VerticalLine, AddTokenMethod.EMPTY, true));
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

        private BoardRecipe StickyRiverRecipe()
        {
            BoardRecipe StickyRiver = new BoardRecipe("RiverOfFruit");
            StickyRiver.Ingredients.Add(new ASmallGroveFeature());
            StickyRiver.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CrossTheBoard, AddTokenMethod.EMPTY, true));
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

}

}

