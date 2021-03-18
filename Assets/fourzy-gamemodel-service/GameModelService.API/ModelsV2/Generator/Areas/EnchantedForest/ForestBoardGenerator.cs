using System.Collections;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{

    public class ForestRandomGenerator : BoardGenerator
    {
        public override Area Area { get { return Area.ENCHANTED_FOREST; } }
        public override string Name { get { return "Enchanted Forest"; } }
        public override int MinComplexity { get { return 270; } }
        public override int MaxComplexity { get { return 1200; } }

        public ForestRandomGenerator(string SeedString = "", GameOptions Options=null, BoardGenerationPreferences Preferences=null)
        {
            if (Options == null) Options = new GameOptions();
            this.Options = Options;

            if (Preferences == null) Preferences = new BoardGenerationPreferences();
            this.Preferences = Preferences;

            this.SeedString= SeedString;
             
            this.Recipes = new Dictionary<BoardRecipe, int>();
            this.Recipes.Add(SimplePuddle(), 10);
            this.Recipes.Add(SimplePits(), 10);
            this.Recipes.Add(PitEdge(), 10);
            this.Recipes.Add(SpookyGrove(), 10);
            this.Recipes.Add(Spooked(), 10);
            this.Recipes.Add(DangerPath(), 5);
            this.Recipes.Add(ForestPath(), 10);
            this.Recipes.Add(NearTheLake(), 10);
            this.Recipes.Add(Swamp(), 10);
            this.Recipes.Add(WideRiver(), 10);
            this.Recipes.Add(DontFall(), 10);
            this.Recipes.Add(DontFall2(), 20);
            this.Recipes.Add(DontFall3(), 20);
            this.Recipes.Add(DontFall4(), 20);
            this.Recipes.Add(CenterFall(), 10);
            this.Recipes.Add(EdgeBumps(), 10);
            this.Recipes.Add(SurroundedByGhosts(), 10);
            
            this.Recipes.Add(DiagonalOfDeath(), 10);
            this.Recipes.Add(DiagonalOfDeathLeftRight(), 10);
            this.Recipes.Add(WaterBlob(), 10);
            this.Recipes.Add(InArrows(), 10);
            this.Recipes.Add(SpookyRiver(), 20);
            this.Recipes.Add(River1(), 20);
            this.Recipes.Add(River2(), 20);
            this.Recipes.Add(River3(), 50);
            this.Recipes.Add(GhostRing1(), 50);
            this.Recipes.Add(GhostRing2(), 50);
            this.Recipes.Add(SpookyRing1(), 50);
            this.Recipes.Add(SpookyRing2(), 50);
           
            this.Recipes.Add(SimplePuddle2(), 50);
            this.Recipes.Add(SimplePuddle3(), 50);
            this.Recipes.Add(FourWayBack(), 50);
            this.Recipes.Add(CenterVortex(), 10);

            //Unusuable Boards.

            //Players aren't expecting pieces to fall.  Would work if there was a warning.
            //this.Recipes.Add(SurpriseFall(), 10);

            //this.Recipes.Add(SpookyRing3(), 50);
            //this.Recipes.Add(CenterVortexWithPits(), 10);
            //this.Recipes.Add(CenterVortexWithSticky(), 10);
            //this.Recipes.Add(CenterVortexWithGhosts(), 10);

            //foreach(BoardRecipe r in this.Recipes.Keys)
            //{
            //    r.Ingredients.Add(new PossibilityOfGhostsFeature(25,4));
            //}

            //this.Recipes.Add(RiverLand(), 10);
        }

        private BoardRecipe GhostRing1()
        {
            BoardRecipe Forest = new BoardRecipe("GhostRing1", 184, 1080);
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.OneRandom, AddTokenMethod.EMPTY));
            Forest.Ingredients.Add(new Ingredient(new MovingGhostToken(Direction.UP,MoveMethod.RING_CLOCKWISE), PatternType.FourConnectedRingTwo, AddTokenMethod.EMPTY));

            return Forest;
        }
        private BoardRecipe GhostRing2()
        {
            BoardRecipe Forest = new BoardRecipe("GhostRing2", 184, 1080);
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.OneRandomRingThree, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new MovingGhostToken(Direction.UP, MoveMethod.RING_CLOCKWISE), PatternType.FourSymmetricRingTwo, AddTokenMethod.EMPTY));
            Forest.Ingredients.Add(new Ingredient(new MovingGhostToken(Direction.UP, MoveMethod.RING_CLOCKWISE), PatternType.FourSymmetricRingOne, AddTokenMethod.EMPTY));

            return Forest;
        }

        private BoardRecipe CenterFall()
        {
            BoardRecipe Forest = new BoardRecipe("CenterFall", 184,1080);
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CenterSixteen));
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.OneFullLine, AddTokenMethod.EMPTY));

            return Forest;
        }

        private BoardRecipe CenterVortex()
        {
            BoardRecipe Forest = new BoardRecipe("CenterVortex", 0, 368);

            //All of these arrows are too noisy for now. May look better when we have upgraded graphics.
            //Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.LEFT), PatternType.BubbleEdgeRight));
            //Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RIGHT), PatternType.BubbleEdgeLeft));
            //Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.UP), PatternType.BubbleEdgeDown));
            //Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.DOWN), PatternType.BubbleEdgeUp));
            Forest.Ingredients.Add(new ArrowCycleFeature(Rotation.CLOCKWISE, new BoardLocation(3, 3), 2, 2));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));

            Forest.ContainsRandom = true;
            return Forest;
        }
        private BoardRecipe CenterVortexWithGhosts()
        {
            BoardRecipe Forest = new BoardRecipe("CenterVortexWithGhosts", 577, 577);
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

        private BoardRecipe CenterVortexWithPits()
        {
            BoardRecipe Forest = new BoardRecipe("CenterVortexWithPits", 494, 494);
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.LEFT), PatternType.BubbleEdgeRight));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RIGHT), PatternType.BubbleEdgeLeft));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.UP), PatternType.BubbleEdgeDown));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.DOWN), PatternType.BubbleEdgeUp));
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.ThreeRandom, AddTokenMethod.ALWAYS, true));

            return Forest;
        }

        private BoardRecipe CenterVortexWithSticky()
        {
            BoardRecipe Forest = new BoardRecipe("CenterVortexWithSticky", 1048, 1048);
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.LEFT), PatternType.BubbleEdgeRight));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RIGHT), PatternType.BubbleEdgeLeft));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.UP), PatternType.BubbleEdgeDown));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.DOWN), PatternType.BubbleEdgeUp));
            Forest.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.ACouple, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS, true));

            return Forest;
        }


        private BoardRecipe FourWayBack()
        {
            BoardRecipe Forest = new BoardRecipe("FourWayBack", 1048, 1048);
            Forest.Ingredients.Add(new FourWayBackFeature(new BoardLocation(0,0), 2));

            return Forest;
        }



        private BoardRecipe SimplePuddle()
        {
            BoardRecipe Forest = new BoardRecipe("SimplePuddle", 0, 400);
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));

            return Forest;
        }

        private BoardRecipe SimplePuddle2()
        {
            BoardRecipe Forest = new BoardRecipe("SimplePuddle2", 0, 400);
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.FourSymmetricRingOne, AddTokenMethod.ALWAYS, true));

            return Forest;
        }

        private BoardRecipe SimplePuddle3()
        {
            BoardRecipe Forest = new BoardRecipe("SimplePuddle3", 0, 400);
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.FourSymmetricRingOne, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.FourSymmetricRingTwo, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.TwoRandomRingThree, AddTokenMethod.ALWAYS, true));

            return Forest;
        }


        private BoardRecipe SimplePits()
        {
            BoardRecipe Forest = new BoardRecipe("SimplePits", 0, 400);
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));

            return Forest;
        }

        private BoardRecipe PitEdge()
        {
            BoardRecipe Forest = new BoardRecipe("PitEdge", 200, 400);
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.EdgeBump, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));

            return Forest;
        }

        private BoardRecipe SpookyRiver()
        {
            BoardRecipe Forest = new BoardRecipe("SpookyRiver",625,625);
            Forest.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.ThreeRandom, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CrossBoardTwoTurnPattern, AddTokenMethod.ALWAYS, true));

            return Forest;
        }

        private BoardRecipe River1()
        {
            BoardRecipe Forest = new BoardRecipe("River1",982,982);
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.OneRandomRingOne, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.OneRandomRingTwo, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.OneRandomRingThree, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.OneRandom, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CrossBoardOneTurn, AddTokenMethod.ALWAYS, true));

            return Forest;
        }

        private BoardRecipe River2()
        {
            BoardRecipe Forest = new BoardRecipe("River2",558,558);
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.TwoRandomRingTwo, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.OneRandomRingThree, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CrossBoardTwoTurnPattern, AddTokenMethod.ALWAYS, true));

            return Forest;
        }

        private BoardRecipe River3()
        {
            BoardRecipe Forest = new BoardRecipe("River3",548,548);
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.OneRandomRingOne, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.OneRandomRingOne, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandomRingTwo, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CrossBoardFourTurnPattern, AddTokenMethod.ALWAYS, true));

            return Forest;
        }
        private BoardRecipe InArrows()
        {
            BoardRecipe Forest = new BoardRecipe("InArrows",564,564);
            Forest.Ingredients.Add(new InArrowFeature());
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.OuterCorners, AddTokenMethod.ALWAYS, true));

            return Forest;
        }

        private BoardRecipe WaterBlob()
        {
            BoardRecipe Forest = new BoardRecipe("WaterBlob",516,516);
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CenterBlob, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.MediumSymmetricBlockEdgePattern, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));


            return Forest;
        }

        private BoardRecipe DiagonalOfDeathLeftRight()
        {
            BoardRecipe Forest = new BoardRecipe("DiagonalOfDeathLR",1068,1068);
            Forest.Ingredients.Add(new ArrowsBlockSideFeature(Direction.LEFT, RelativeDirection.IN));
            Forest.Ingredients.Add(new ArrowsBlockSideFeature(Direction.RIGHT, RelativeDirection.IN));
            Forest.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.CenterDiagonal, AddTokenMethod.ALWAYS, true));

            return Forest;
        }

        private BoardRecipe DiagonalOfDeath()
        {
            BoardRecipe Forest = new BoardRecipe("DiagonalOfDeath",412,412);
            Forest.Ingredients.Add(new SymmetricArrowsFeature(true, 2));
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.CenterDiagonal, AddTokenMethod.ALWAYS, true));

            return Forest;
        }




        private BoardRecipe SurroundedByGhosts()
        {
            BoardRecipe Forest = new BoardRecipe("Surrounded",476,476);
            Forest.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.MediumSymmetricBlockEdgePattern));
            Forest.Ingredients.Add(new ArrowCycleFeature(Rotation.CLOCKWISE, new BoardLocation(0, 0), 2, 2));
            Forest.ContainsRandom = true;
            return Forest;
        }


        private BoardRecipe EdgeBumps()
        {
            BoardRecipe Forest = new BoardRecipe("EdgeBumps",916,916);
            Forest.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.EdgeBumpDuo));
            //Forest.Ingredients.Add(new Ingredient(new ArrowToken(Direction.RANDOM), PatternType.DottedLine));
            
            return Forest;
        }



        //DO NOT USE.  CONFUSING TO PLAYERS
        //private BoardRecipe SurpriseFall()
        //{
        //    BoardRecipe Forest = new BoardRecipe("SurpriseFall");
        //    Forest.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterSixteen));
        //    //Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.ThreeRandom));

        //    return Forest;
        //}

        private BoardRecipe DontFall4()
        {
            BoardRecipe Forest = new BoardRecipe("DontFall4", 911, 911);
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.SmallSymmetricBlockEdgePattern));
            Forest.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterFour));

            return Forest;
        }

        private BoardRecipe DontFall3()
        {
            BoardRecipe Forest = new BoardRecipe("DontFall3", 911, 911);
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.LargeSymmetricBlockEdgePattern));

            return Forest;
        }
        private BoardRecipe DontFall2()
        {
            BoardRecipe Forest = new BoardRecipe("DontFall2",911,911);
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.BlockEdgesTwoSpace));
            Forest.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.BlockEdgesTwoSpace));
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.CenterFour));

            return Forest;
        }

        private BoardRecipe DontFall()
        {
            BoardRecipe Forest = new BoardRecipe("DontFall",592,592);
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.LargeSymmetricBlockEdgePattern));
            Forest.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.CenterSixteen));

            return Forest;
        }


        private BoardRecipe WideRiver()
        {
            BoardRecipe Forest = new BoardRecipe("WideRiver",1012,1012);
            //Forest.Ingredients.Add(new ASmallGroveFeature(TokenType.BLOCKER));
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.WideLine));
            Forest.Ingredients.Add(new ACoupleGhostsFeature(4));
            return Forest;
        }

        private BoardRecipe Swamp()
        {
            BoardRecipe Forest = new BoardRecipe("Swamp",1196,1196);
            Forest.Ingredients.Add(new ASmallGroveFeature(TokenType.WATER));
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.FillGaps));
            Forest.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.FillGaps));
            return Forest;
        }


        private BoardRecipe NearTheLake()
        {
            BoardRecipe Forest = new BoardRecipe("Lake",680,680);
            Forest.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.BubbleEdge));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoSymmetricRingTwo));
            //Forest.Ingredients.Add(new ASmallGroveFeature(TokenType.BLOCKER));
            return Forest;
        }

        private BoardRecipe DangerPath()
        {
            BoardRecipe Forest = new BoardRecipe("DangerPath",412,412);
            Forest.Ingredients.Add(new HighwayFeature(LineDirection.NONE, 0, 0, false, 1, false));
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.FillGaps));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.TwoRandom));

            return Forest;
        }


        private BoardRecipe ForestPath()
        {
            BoardRecipe Forest = new BoardRecipe("ForestPath",544,544);
            //Forest.Ingredients.Add(new ASmallGroveFeature(TokenType.BLOCKER));
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.FourSymmetricRingOne));
            Forest.Ingredients.Add(new CrossArrowFeature());

            return Forest;
        }


        private BoardRecipe SpookyGrove()
        {
            BoardRecipe Forest = new BoardRecipe("SpookyGrove",604,604);
            //Forest.Ingredients.Add(new ASmallGroveFeature());
            Forest.Ingredients.Add(new Ingredient(new BlockerToken(), PatternType.OneRandomRingThree));
            Forest.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.SmallSymmetricBlockEdgePattern));
            //Forest.Ingredients.Add(new HighwayFeature());

            return Forest;
        }

        private BoardRecipe SpookyRing1()
        {
            BoardRecipe Forest = new BoardRecipe("SpookyRing1", 604, 604);
            Forest.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.AlternateRingTwo));
        
            return Forest;
        }
        private BoardRecipe SpookyRing2()
        {
            BoardRecipe Forest = new BoardRecipe("SpookyRing2", 604, 604);
            Forest.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.FourSymmetricRingTwo));

            return Forest;
        }

        private BoardRecipe SpookyRing3()
        {
            BoardRecipe Forest = new BoardRecipe("SpookyRing3", 604, 604);
            Forest.Ingredients.Add(new Ingredient(new PitToken(), PatternType.OneRandomRingThree));
            Forest.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.FourConnectedRingTwo));
            Forest.Ingredients.Add(new Ingredient(new GhostToken(), PatternType.FourConnectedRingOne));

            return Forest;
        }

        private BoardRecipe Spooked()
        {
            BoardRecipe Spooked = new BoardRecipe("Spooked",780,780);
            Spooked.Ingredients.Add(new ACoupleGhostsFeature(4));

            return Spooked;
        }

        //private BoardRecipe RiverLand()
        //{
        //    BoardRecipe Riverland = new BoardRecipe("RiverLand");
        //    Riverland.Ingredients.Add(new ASmallGroveFeature());
        //    Riverland.Ingredients.Add(new ACoupleGhostsFeature(2));
        //    Riverland.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CrossTheBoard, 0, false));
        //    return Riverland;
        //}

    }

}

