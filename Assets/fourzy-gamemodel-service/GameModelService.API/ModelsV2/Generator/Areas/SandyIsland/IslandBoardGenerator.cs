using System.Collections;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{

    public class IslandRandomGenerator : BoardGenerator
    {

        public string Name = "Sandy Island";
        public override Area Area { get { return Area.SANDY_ISLAND; } }
        //Recipes contains a list of potential Recipes for Building out the Garden.

        public IslandRandomGenerator(string RequestedRecipeName = "")
        {
            this.RequestedRecipeName = RequestedRecipeName;
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
        }

        private BoardRecipe SandSpook2()
        {
            BoardRecipe Shore = new BoardRecipe("SandSpook2");
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterBlob, AddTokenMethod.ALWAYS, true));
            Shore.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.ThreeRandom, AddTokenMethod.ONLY_TERRAIN, true));
            return Shore;
        }

        private BoardRecipe SandSpook1()
        {
            BoardRecipe Shore = new BoardRecipe("SandSpook1");
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterSixteen, AddTokenMethod.ALWAYS, true));
            Shore.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.ThreeRandom, AddTokenMethod.ONLY_TERRAIN, true));
            return Shore;
        }


        private BoardRecipe LandBridge2()
        {
            BoardRecipe Shore = new BoardRecipe("LandBridge2");
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new PitToken(), PatternType.BubbleEdgeLeft, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new PitToken(), PatternType.BubbleEdgeRight, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterSixteen, AddTokenMethod.ONLY_TERRAIN, true));
            return Shore;
        }


        private BoardRecipe CenterBlob()
        {
            BoardRecipe Shore = new BoardRecipe("CenterBlob");
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterBlob));
            Shore.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Shore;
        }

        private BoardRecipe CenterSand3()
        {
            BoardRecipe Shore = new BoardRecipe("CenterSand3");
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.MediumSymmetricBlockEdgePattern, AddTokenMethod.ALWAYS, true));
            return Shore;
        }
        
        private BoardRecipe CenterSand2()
        {
            BoardRecipe Shore = new BoardRecipe("CenterSand2");
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterSixteen));
            Shore.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.PartialBlockEdges, AddTokenMethod.ALWAYS, true));
            return Shore;
        }

        private BoardRecipe CenterSand()
        {
            BoardRecipe Shore = new BoardRecipe("CenterSand");
            Shore.Ingredients.Add(new Ingredient(new SandToken(),PatternType.CenterSixteen));
            Shore.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS,true));
            Shore.Ingredients.Add(new ArrowsBlockSideFeature());
            return Shore;
        }


        private BoardRecipe SymmSand()
        {
            BoardRecipe Shore = new BoardRecipe("SymmSand");
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new SwirlingArrowsFeature());
            return Shore;
        }

        private BoardRecipe UnstableSand()
        {
            BoardRecipe Shore = new BoardRecipe("UnstableSand");
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new PitToken(), PatternType.ACouple, AddTokenMethod.ONLY_TERRAIN, true));
            return Shore;
        }
        
        private BoardRecipe SandHole()
        {
            BoardRecipe Shore = new BoardRecipe("SandHole");
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new InArrowFeature());
            return Shore;
        }

        private BoardRecipe SandWalk()
        {
            BoardRecipe Shore = new BoardRecipe("SandWalk");
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new HighwayFeature());
            return Shore;
        }

        private BoardRecipe SandSpiral()
        {
            BoardRecipe Shore = new BoardRecipe("SandSpiral");
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new ArrowPinWheelFeature());
            return Shore;
        }


        private BoardRecipe SandStorm()
        {
            BoardRecipe Shore = new BoardRecipe("SandStorm");
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new ArrowStormFeature());
            return Shore;
        }

        private BoardRecipe CannonGrave()
        {
            BoardRecipe Shore = new BoardRecipe("CannonGrave");
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.Four, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new CircleBombToken(), PatternType.Four, AddTokenMethod.ONLY_TERRAIN, false));
            Shore.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.DottedLine, AddTokenMethod.ONLY_TERRAIN, false));
            return Shore;
        }

        private BoardRecipe FourThings()
        {
            BoardRecipe Shore = new BoardRecipe("FourThings");
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
            BoardRecipe Shore = new BoardRecipe("FourSand");
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
            BoardRecipe Shore = new BoardRecipe("Worship");
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.CentralCorners, AddTokenMethod.ONLY_TERRAIN, false));
            Shore.Ingredients.Add(new ACoupleGhostsFeature(2));
            return Shore;
        }
               
        private BoardRecipe Ghosts()
        {
            BoardRecipe Shore = new BoardRecipe("Ghosts");
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterBlob));
            //Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new ACoupleGhostsFeature(2));
            Shore.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.SmallSymmetricBlockEdgePattern, AddTokenMethod.ALWAYS,true));

            return Shore;
        }

        private BoardRecipe ShoreRiver()
        {
            BoardRecipe Shore = new BoardRecipe("River");
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CrossTheBoard, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.BubbleEdge, AddTokenMethod.ONLY_TERRAIN, true));
            return Shore;
        }


        private BoardRecipe ShoreParadise()
        {
            BoardRecipe Shore = new BoardRecipe("Paradise");
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.BlockEdge, AddTokenMethod.ALWAYS,true));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.Dots, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new HighwayFeature());
            return Shore;
        }

        private BoardRecipe Island()
        {
            BoardRecipe Shore = new BoardRecipe("Island");
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new CircleBombToken(), PatternType.OneRandom, AddTokenMethod.ONLY_TERRAIN, false));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.BlockAllEdges, AddTokenMethod.ALWAYS, true));
            return Shore;
        }


        private BoardRecipe Shore()
        {
            BoardRecipe Shore = new BoardRecipe("Shore");
            Shore.Ingredients.Add(new Ingredient(new SandToken(), PatternType.CenterBlob, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.BubbleEdge, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.OneRandom, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new CircleBombToken(), PatternType.TwoRandom));
            return Shore;
        }

        private BoardRecipe LandBridge()
        {
            BoardRecipe Shore = new BoardRecipe("LandBridge");
            Shore.Ingredients.Add(new TerrainIngredient(new SandToken()));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.BubbleEdgeLeft, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.BubbleEdgeRight, AddTokenMethod.ONLY_TERRAIN, true));
            Shore.Ingredients.Add(new Ingredient(new CircleBombToken(), PatternType.TwoRandom, AddTokenMethod.ONLY_TERRAIN, true));
            return Shore;
        }




    }

}

