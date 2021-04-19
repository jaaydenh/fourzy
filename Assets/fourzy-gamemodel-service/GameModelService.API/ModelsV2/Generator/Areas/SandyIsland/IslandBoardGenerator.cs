using System.Collections;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{

    public class IslandRandomGenerator : BoardGenerator
    {

        //public string Name { get; set; } 
        public override Area Area { get { return Area.SANDY_ISLAND; } }
        
        public override string Name { get { return "Sandy Island"; } }

        public override int MinComplexity { get { return 476; } }
        public override int MaxComplexity { get { return 1500; } }


        public IslandRandomGenerator(GameOptions Options = null, BoardGenerationPreferences Preferences = null)
        {
            if (Options == null) Options = new GameOptions();
            this.Options = Options;

            if (Preferences == null) Preferences = new BoardGenerationPreferences();
            this.Preferences = Preferences;

            this.SeedString = Preferences.RecipeSeed;

            this.Recipes = new Dictionary<BoardRecipe, int>();
            //this.Recipes.Add(ShoreRiver(), 10);
            this.Recipes.Add(ShoreParadise(), 10);
            this.Recipes.Add(Island(), 10);
            this.Recipes.Add(Shore(), 10);
            this.Recipes.Add(LandBridge(), 10);
            this.Recipes.Add(Ghosts(), 10);
            this.Recipes.Add(CannonGrave(), 10);
            this.Recipes.Add(FourThings(), 10);
            this.Recipes.Add(FourSand(), 10);
            this.Recipes.Add(SandStorm(), 10);
            this.Recipes.Add(SandHole(), 10);
            this.Recipes.Add(SandWalk(), 10);
            this.Recipes.Add(SandSpiral(), 10);
            this.Recipes.Add(UnstableSand(), 10);
            this.Recipes.Add(SymmSand(), 10);
            this.Recipes.Add(CenterSand(), 10);
            this.Recipes.Add(CenterSand2(), 10);
            this.Recipes.Add(CenterSand3(), 10);
            this.Recipes.Add(CenterBlob(), 10);
            this.Recipes.Add(LandBridge2(), 10);
            this.Recipes.Add(SandSpook1(), 10);
            this.Recipes.Add(SandSpook2(), 10);
            this.Recipes.Add(Worship(), 25);
            this.Recipes.Add(SandCircle(), 25);
            this.Recipes.Add(SandCircle2(), 25);
            this.Recipes.Add(SandArt(), 25);
            this.Recipes.Add(SandPath(), 25);
            this.Recipes.Add(SandCheckers(), 25);

            this.Recipes.Add(SandFourWay(), 25);
            this.Recipes.Add(SandPivot(), 25);
            this.Recipes.Add(SandPivot2(), 25);
            this.Recipes.Add(SandPivot3(), 25);
       

            foreach (BoardRecipe r in this.Recipes.Keys)
            {
                r.Ingredients.Add(new PossibilityOfGhostsFeature(10, 2));
            }

        }

        private BoardRecipe SandFourWay()
        {
            BoardRecipe Island = new BoardRecipe("SandFourWay",704,704);
            Island.Ingredients.Add(new TileFeature(new SandToken(), 3, 3, "", 5, 6));
            Island.Ingredients.Add(new Ingredient(new FourWayArrowToken(), PatternType.Four, AddTokenMethod.ALWAYS, true));
            return Island;
        }

        private BoardRecipe SandPivot3()
        {
            BoardRecipe Island = new BoardRecipe("SandPivot3",940,940);
            Island.Ingredients.Add(new Ingredient(new SandToken(), PatternType.OuterRing, AddTokenMethod.ALWAYS, true));
            Island.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterBlob, AddTokenMethod.ALWAYS, true));
            Island.Ingredients.Add(new Ingredient(new RotatingArrowToken(Direction.RIGHT, Rotation.CLOCKWISE, 3), PatternType.Four, AddTokenMethod.ALWAYS, true));
            return Island;
        }

        private BoardRecipe SandPivot2()
        {
            BoardRecipe Island = new BoardRecipe("SandPivot2",950,950);
            Island.Ingredients.Add(new Ingredient(new SandToken(), PatternType.OuterRing, AddTokenMethod.ALWAYS, true));
            Island.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterBlob, AddTokenMethod.ALWAYS, true));
            Island.Ingredients.Add(new Ingredient(new RotatingArrowToken(Direction.LEFT, Rotation.CLOCKWISE, 3), PatternType.CenterDiagonal, AddTokenMethod.ALWAYS, true));
            return Island;
        }

        private BoardRecipe SandPivot()
        {
            BoardRecipe Island = new BoardRecipe("SandPivot",836,836);
            Island.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterBlob, AddTokenMethod.ALWAYS, true));
            Island.Ingredients.Add(new Ingredient(new RotatingArrowToken(Direction.UP, Rotation.CLOCKWISE,3), PatternType.CenterFour, AddTokenMethod.ALWAYS, true));
            return Island;
        }

        private BoardRecipe SandArt()
        {
            BoardRecipe Island = new BoardRecipe("SandArt",476,476);
            Island.Ingredients.Add(new TileFeature(new SandToken(), -1, -1, "", -1, -1));
            Island.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Island;
        }

        private BoardRecipe SandPath()
        {
            BoardRecipe Island = new BoardRecipe("SandPath",588,588);
            Island.Ingredients.Add(new Ingredient(new SandToken(), PatternType.OuterRing, AddTokenMethod.ALWAYS, true));
            Island.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CrossBoardFourTurnPattern, AddTokenMethod.ALWAYS, true));
            Island.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CrossBoardFourTurnPattern, AddTokenMethod.ALWAYS, true));
            Island.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Island;
        }
        
        private BoardRecipe SandCheckers()
        {
            BoardRecipe Island = new BoardRecipe("SandCheckers",556,566);
            Island.Ingredients.Add(new Ingredient(new SandToken(), PatternType.LargeCheckers, AddTokenMethod.ALWAYS, true));
            Island.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Island;
        }

        private BoardRecipe SandCircle2()
        {
            BoardRecipe Island = new BoardRecipe("SandCircle2",732,732);
            Island.Ingredients.Add(new Ingredient(new SandToken(), PatternType.OuterRing, AddTokenMethod.ALWAYS, true));
            Island.Ingredients.Add(new Ingredient(new SandToken(), PatternType.WideCross, AddTokenMethod.ALWAYS, true));
            Island.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Island;
        }

        private BoardRecipe SandCircle()
        {
            BoardRecipe Island = new BoardRecipe("SandCircle",516,516);
            Island.Ingredients.Add(new Ingredient(new SandToken(), PatternType.OuterRing, AddTokenMethod.ALWAYS, true));
            Island.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS, true));
            Island.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Island;
        }

        private BoardRecipe SandSpook2()
        {
            BoardRecipe Shore = new BoardRecipe("SandSpook2",734,734);
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterBlob, AddTokenMethod.ALWAYS, true));
            Shore.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.ThreeRandom, AddTokenMethod.ONLY_TERRAIN, true));
            return Shore;
        }

        private BoardRecipe SandSpook1()
        {
            BoardRecipe Shore = new BoardRecipe("SandSpook1",649,649);
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterSixteen, AddTokenMethod.ALWAYS, true));
            Shore.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.ThreeRandom, AddTokenMethod.ONLY_TERRAIN, true));
            return Shore;
        }


        private BoardRecipe LandBridge2()
        {
            BoardRecipe Shore = new BoardRecipe("LandBridge2",792,792);
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new PitToken(), PatternType.BubbleEdgeLeft, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new PitToken(), PatternType.BubbleEdgeRight, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterSixteen, AddTokenMethod.ONLY_TERRAIN, true));
            return Shore;
        }


        private BoardRecipe CenterBlob()
        {
            BoardRecipe Shore = new BoardRecipe("CenterBlob",548,548);
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterBlob));
            Shore.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Shore;
        }

        private BoardRecipe CenterSand3()
        {
            BoardRecipe Shore = new BoardRecipe("CenterSand3",812,812);
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.MediumSymmetricBlockEdgePattern, AddTokenMethod.ALWAYS, true));
            return Shore;
        }
        
        private BoardRecipe CenterSand2()
        {
            BoardRecipe Shore = new BoardRecipe("CenterSand2",812,812);
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterSixteen));
            Shore.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.PartialBlockEdges, AddTokenMethod.ALWAYS, true));
            return Shore;
        }

        private BoardRecipe CenterSand()
        {
            BoardRecipe Shore = new BoardRecipe("CenterSand",584,584);
            Shore.Ingredients.Add(new Ingredient(new SandToken(),PatternType.CenterSixteen));
            Shore.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS,true));
            Shore.Ingredients.Add(new ArrowsBlockSideFeature());
            return Shore;
        }


        private BoardRecipe SymmSand()
        {
            BoardRecipe Shore = new BoardRecipe("SymmSand",1366,1366);
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new SwirlingArrowsFeature());
            return Shore;
        }

        private BoardRecipe UnstableSand()
        {
            BoardRecipe Shore = new BoardRecipe("UnstableSand",900,900);
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new PitToken(), PatternType.ACouple, AddTokenMethod.ONLY_TERRAIN, true));
            return Shore;
        }
        
        private BoardRecipe SandHole()
        {
            BoardRecipe Shore = new BoardRecipe("SandHole",952,952);
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new InArrowFeature());
            return Shore;
        }

        private BoardRecipe SandWalk()
        {
            BoardRecipe Shore = new BoardRecipe("SandWalk",952,952);
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new HighwayFeature());
            return Shore;
        }

        private BoardRecipe SandSpiral()
        {
            BoardRecipe Shore = new BoardRecipe("SandSpiral",1310,1310);
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new ArrowPinWheelFeature());
            return Shore;
        }


        private BoardRecipe SandStorm()
        {
            BoardRecipe Shore = new BoardRecipe("SandStorm",864,864);
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new ArrowStormFeature());
            return Shore;
        }

        private BoardRecipe CannonGrave()
        {
            BoardRecipe Shore = new BoardRecipe("CannonGrave",985,985);
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.Four, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new CircleBombToken(), PatternType.Four, AddTokenMethod.ONLY_TERRAIN, false));
            Shore.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.DottedLine, AddTokenMethod.ONLY_TERRAIN, false));
            return Shore;
        }

        private BoardRecipe FourThings()
        {
            BoardRecipe Shore = new BoardRecipe("FourThings",1460,1460);
            //Shore.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterFour, 0, false, false));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.BubbleEdgeUp));
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.BubbleEdgeDown));
            Shore.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.BubbleEdgeLeft));
            Shore.Ingredients.Add(new Ingredient(new IceToken(), PatternType.BubbleEdgeRight));
            Shore.Ingredients.Add(new Ingredient(new CircleBombToken(), PatternType.Four, AddTokenMethod.ONLY_TERRAIN, true));
            return Shore;
        }


        private BoardRecipe FourSand()
        {
            BoardRecipe Shore = new BoardRecipe("FourSand",1108,1108);
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS, false));
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.BubbleEdgeUp));
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.BubbleEdgeDown));
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.BubbleEdgeLeft));
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.BubbleEdgeRight));
            Shore.Ingredients.Add(new Ingredient(new CircleBombToken(), PatternType.Four, AddTokenMethod.ONLY_TERRAIN, true));
            return Shore;
        }


        private BoardRecipe Worship()
        {
            BoardRecipe Shore = new BoardRecipe("Worship",800,800);
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterBlob));
            Shore.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.CentralCorners, AddTokenMethod.ONLY_TERRAIN, false));
            Shore.Ingredients.Add(new ACoupleGhostsFeature(2));
            return Shore;
        }
               
        private BoardRecipe Ghosts()
        {
            BoardRecipe Shore = new BoardRecipe("Ghosts",766,766);
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterBlob));
            //Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new ACoupleGhostsFeature(2));
            Shore.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.SmallSymmetricBlockEdgePattern, AddTokenMethod.ALWAYS,true));

            return Shore;
        }

        private BoardRecipe ShoreRiver()
        {
            BoardRecipe Shore = new BoardRecipe("ShoreRiver");
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CrossTheBoard, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.BubbleEdge, AddTokenMethod.ONLY_TERRAIN, true));
            return Shore;
        }


        private BoardRecipe ShoreParadise()
        {
            BoardRecipe Shore = new BoardRecipe("Paradise",1144,1144);
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.BlockEdge, AddTokenMethod.ALWAYS,true));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.Dots, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new HighwayFeature());
            return Shore;
        }

        private BoardRecipe Island()
        {
            BoardRecipe Shore = new BoardRecipe("Island",1189,1189);
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new CircleBombToken(), PatternType.OneRandom, AddTokenMethod.ONLY_TERRAIN, false));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.BlockAllEdges, AddTokenMethod.ALWAYS, true));
            return Shore;
        }


        private BoardRecipe Shore()
        {
            BoardRecipe Shore = new BoardRecipe("Shore",960,960);
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterBlob, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.BubbleEdge, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.OneRandom, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new CircleBombToken(), PatternType.TwoRandom));
            return Shore;
        }

        private BoardRecipe LandBridge()
        {
            BoardRecipe Shore = new BoardRecipe("LandBridge",1182,1182);
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.BubbleEdgeLeft, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.BubbleEdgeRight, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new CircleBombToken(), PatternType.TwoRandom, AddTokenMethod.ONLY_TERRAIN, true));
            return Shore;
        }




    }

}

