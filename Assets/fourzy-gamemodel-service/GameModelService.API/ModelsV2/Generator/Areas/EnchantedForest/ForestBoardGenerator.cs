using System.Collections;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{

    public class ForestRandomGenerator : BoardGenerator
    {

        public string Name = "Enchanted Forest";
        public override Area Area { get { return Area.ENCHANTED_FOREST; } }
        //Recipes contains a list of potential Recipes for Building out the Garden.

        public ForestRandomGenerator(string RequestedRecipeName = "", bool AllowRandom = true)
        {
            this.RequestedRecipeName = RequestedRecipeName;
            this.AllowRandom = AllowRandom;

            this.Recipes = new Dictionary<BoardRecipe, int>();
            this.Recipes.Add(SpookyGrove(), 10);
            this.Recipes.Add(Spooked(), 10);
            this.Recipes.Add(DangerPath(), 5);
            this.Recipes.Add(ForestPath(), 10);
            this.Recipes.Add(NearTheLake(), 10);
            this.Recipes.Add(Swamp(), 10);
            this.Recipes.Add(WideRiver(), 10);
            this.Recipes.Add(DontFall(), 10);
            this.Recipes.Add(DontFall2(), 20);
            //this.Recipes.Add(SurpriseFall(), 10);
            this.Recipes.Add(CenterFall(), 10);
            this.Recipes.Add(EdgeBumps(), 10);
            this.Recipes.Add(SurroundedByGhosts(), 10);
            this.Recipes.Add(CenterVortex(), 10);
            this.Recipes.Add(CenterVortexWithPits(), 10);
            this.Recipes.Add(CenterVortexWithSticky(), 10);
            this.Recipes.Add(CenterVortexWithGhosts(), 10);
            this.Recipes.Add(DiagonalOfDeath(), 10);
            this.Recipes.Add(DiagonalOfDeathLeftRight(), 10);
            this.Recipes.Add(WaterBlob(), 10);
            this.Recipes.Add(InArrows(), 10);
            this.Recipes.Add(SpookyRiver(), 20);
            this.Recipes.Add(River1(), 20);
            this.Recipes.Add(River2(), 20);
            this.Recipes.Add(River3(), 50);

            foreach(BoardRecipe r in this.Recipes.Keys)
            {
                r.Ingredients.Add(new PossibilityOfGhostsFeature(25,4));
            }

            //this.Recipes.Add(RiverLand(), 10);
        }

        private BoardRecipe SpookyRiver()
        {
            BoardRecipe Forest = new BoardRecipe("SpookyRiver");
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CrossBoardTwoTurnPattern, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.ThreeRandom, AddTokenMethod.ALWAYS, true));

            return Forest;
        }


        private BoardRecipe River1()
        {
            BoardRecipe Forest = new BoardRecipe("River1");
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CrossBoardOneTurn, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.OneRandom, AddTokenMethod.ALWAYS, true));

            return Forest;
        }

        private BoardRecipe River2()
        {
            BoardRecipe Forest = new BoardRecipe("River2");
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CrossBoardTwoTurnPattern, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.OneRandom, AddTokenMethod.ALWAYS, true));

            return Forest;
        }

        private BoardRecipe River3()
        {
            BoardRecipe Forest = new BoardRecipe("River3");
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.OneRandom, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.OneRandom, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.OneRandom, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.OneRandom, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CrossBoardFourTurnPattern, AddTokenMethod.ALWAYS, true));

            return Forest;
        }



        private BoardRecipe InArrows()
        {
            BoardRecipe Forest = new BoardRecipe("InArrows");
            Forest.Ingredients.Add(new InArrowFeature());
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.OuterCorners, AddTokenMethod.ALWAYS, true));

            return Forest;
        }

        private BoardRecipe WaterBlob()
        {
            BoardRecipe Forest = new BoardRecipe("WaterBlob");
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CenterBlob, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.MediumSymmetricBlockEdgePattern, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));


            return Forest;
        }

        private BoardRecipe DiagonalOfDeathLeftRight()
        {
            BoardRecipe Forest = new BoardRecipe("DiagonalOfDeathLR");
            Forest.Ingredients.Add(new ArrowsBlockSideFeature(Direction.LEFT, RelativeDirection.IN));
            Forest.Ingredients.Add(new ArrowsBlockSideFeature(Direction.RIGHT, RelativeDirection.IN));
            Forest.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.CenterDiagonal, AddTokenMethod.ALWAYS, true));

            return Forest;
        }

        private BoardRecipe DiagonalOfDeath()
        {
            BoardRecipe Forest = new BoardRecipe("DiagonalOfDeath");
            Forest.Ingredients.Add(new SymmetricArrowsFeature(true, 2));
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.CenterDiagonal, AddTokenMethod.ALWAYS, true));

            return Forest;
        }


        private BoardRecipe CenterVortexWithPits()
        {
            BoardRecipe Forest = new BoardRecipe("CenterVortexWithPits");
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.LEFT), PatternType.BubbleEdgeRight));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RIGHT), PatternType.BubbleEdgeLeft));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.UP), PatternType.BubbleEdgeDown));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.DOWN), PatternType.BubbleEdgeUp));
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.ThreeRandom, AddTokenMethod.ALWAYS, true));

            return Forest;
        }

        private BoardRecipe CenterVortexWithSticky()
        {
            BoardRecipe Forest = new BoardRecipe("CenterVortexWithSticky");
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.LEFT), PatternType.BubbleEdgeRight));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RIGHT), PatternType.BubbleEdgeLeft));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.UP), PatternType.BubbleEdgeDown));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.DOWN), PatternType.BubbleEdgeUp));
            Forest.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.ACouple, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS, true));

            return Forest;
        }



        private BoardRecipe CenterVortexWithGhosts()
        {
            BoardRecipe Forest = new BoardRecipe("CenterVortexWithGhosts");
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.LEFT), PatternType.BubbleEdgeRight));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RIGHT), PatternType.BubbleEdgeLeft));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.UP), PatternType.BubbleEdgeDown));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.DOWN), PatternType.BubbleEdgeUp));
            Forest.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.MediumSymmetricBlockEdgePattern, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.ACouple, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new ArrowCycleFeature(Rotation.CLOCKWISE, new BoardLocation(3, 3), 2, 2));
            Forest.ContainsRandom = true;
            return Forest;
        }

        private BoardRecipe CenterVortex()
        {
            BoardRecipe Forest = new BoardRecipe("CenterVortex");
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.LEFT), PatternType.BubbleEdgeRight));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RIGHT), PatternType.BubbleEdgeLeft));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.UP), PatternType.BubbleEdgeDown));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.DOWN), PatternType.BubbleEdgeUp));
            Forest.Ingredients.Add(new ArrowCycleFeature(Rotation.CLOCKWISE, new BoardLocation(3, 3), 2, 2));
            Forest.ContainsRandom = true;
            return Forest;
        }


        private BoardRecipe SurroundedByGhosts()
        {
            BoardRecipe Forest = new BoardRecipe("Surrounded");
            Forest.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.MediumSymmetricBlockEdgePattern));
            Forest.Ingredients.Add(new ArrowCycleFeature(Rotation.CLOCKWISE, new BoardLocation(0, 0), 2, 2));
            Forest.ContainsRandom = true;
            return Forest;
        }


        private BoardRecipe EdgeBumps()
        {
            BoardRecipe Forest = new BoardRecipe("EdgeBumps");
            Forest.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.EdgeBump));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.DottedLine));
            
            return Forest;
        }

        private BoardRecipe CenterFall()
        {
            BoardRecipe Forest = new BoardRecipe("CenterFall");
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CenterSixteen));
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.OneFullLine, AddTokenMethod.EMPTY));

            return Forest;
        }

        private BoardRecipe SurpriseFall()
        {
            BoardRecipe Forest = new BoardRecipe("SurpriseFall");
            Forest.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterSixteen));
            //Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.ThreeRandom));

            return Forest;
        }


        private BoardRecipe DontFall2()
        {
            BoardRecipe Forest = new BoardRecipe("DontFall2");
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.BlockEdgesTwoSpace));
            Forest.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.BlockEdgesTwoSpace));
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.CenterFour));

            return Forest;
        }

        private BoardRecipe DontFall()
        {
            BoardRecipe Forest = new BoardRecipe("DontFall");
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.LargeSymmetricBlockEdgePattern));
            Forest.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterSixteen));

            return Forest;
        }


        private BoardRecipe WideRiver()
        {
            BoardRecipe Forest = new BoardRecipe("WideRiver");
            Forest.Ingredients.Add(new ASmallGroveFeature(TokenType.BLOCKER));
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.WideLine));
            Forest.Ingredients.Add(new ACoupleGhostsFeature(4));
            return Forest;
        }

        private BoardRecipe Swamp()
        {
            BoardRecipe Forest = new BoardRecipe("Swamp");
            Forest.Ingredients.Add(new ASmallGroveFeature(TokenType.WATER));
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.FillGaps));
            Forest.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.FillGaps));
            return Forest;
        }


        private BoardRecipe NearTheLake()
        {
            BoardRecipe Forest = new BoardRecipe("Lake");
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.BubbleEdge));
            Forest.Ingredients.Add(new ASmallGroveFeature(TokenType.BLOCKER));
            return Forest;
        }

        private BoardRecipe DangerPath()
        {
            BoardRecipe Forest = new BoardRecipe("DangerPath");
            Forest.Ingredients.Add(new HighwayFeature(LineDirection.NONE, 0, 0, false, 1, false));
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.FillGaps));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom));

            return Forest;
        }


        private BoardRecipe ForestPath()
        {
            BoardRecipe Forest = new BoardRecipe("ForestPath");
            Forest.Ingredients.Add(new ASmallGroveFeature(TokenType.BLOCKER));
            Forest.Ingredients.Add(new CrossArrowFeature());

            return Forest;
        }


        private BoardRecipe SpookyGrove()
        {
            BoardRecipe Forest = new BoardRecipe("SpookyGrove");
            Forest.Ingredients.Add(new ASmallGroveFeature());
            Forest.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.SmallSymmetricBlockEdgePattern));
            //Forest.Ingredients.Add(new HighwayFeature());

            return Forest;
        }

        private BoardRecipe Spooked()
        {
            BoardRecipe Spooked = new BoardRecipe("Spooked");
            Spooked.Ingredients.Add(new ACoupleGhostsFeature(4));

            return Spooked;
        }

        private BoardRecipe RiverLand()
        {
            BoardRecipe Riverland = new BoardRecipe("RiverLand");
            Riverland.Ingredients.Add(new ASmallGroveFeature());
            Riverland.Ingredients.Add(new ACoupleGhostsFeature(2));
            Riverland.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CrossTheBoard, 0, false));
            return Riverland;
        }

    }

}

