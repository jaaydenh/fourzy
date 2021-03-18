using System.Collections;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{

    public class IcePalaceRandomGenerator : BoardGenerator
    {

        //public string Name = "Ice Palace";
        public override Area Area { get { return Area.ICE_PALACE; } }
        //Recipes contains a list of potential Recipes for Building out the Garden.
        public override string Name { get { return "Ice Palace"; } }
        public override int MinComplexity { get { return 450; } }
        public override int MaxComplexity { get { return 1650; } }

        public IcePalaceRandomGenerator(string SeedString = "", GameOptions Options = null, BoardGenerationPreferences Preferences = null)
        {
            if (Options == null) Options = new GameOptions();
            this.Options = Options;

            if (Preferences == null) Preferences = new BoardGenerationPreferences();
            this.Preferences = Preferences;

            this.SeedString = SeedString;
        
            this.Recipes = new Dictionary<BoardRecipe, int>();
            this.Recipes.Add(ThroneRoomRecipe(), 10);
            this.Recipes.Add(IceRoomRecipe(), 10);
            this.Recipes.Add(StarRoom(), 10);
            this.Recipes.Add(SwirlingIce(), 10);
            this.Recipes.Add(PillarsOfIce(), 10);
            this.Recipes.Add(IceCheckers(), 10);
            this.Recipes.Add(SnowCheckers(), 10);
            this.Recipes.Add(TheDevide(), 10);
            this.Recipes.Add(FrozenLake(), 10);
            this.Recipes.Add(IceShore(), 10);
            this.Recipes.Add(IceDots(), 10);
            this.Recipes.Add(DividedLand(), 100);
            this.Recipes.Add(IceHall(), 10);
            this.Recipes.Add(CrossRoad(), 10);
            this.Recipes.Add(SnowKeep(), 10);
            this.Recipes.Add(SnowCross(), 10);
            this.Recipes.Add(SnowQuads(), 10);
            this.Recipes.Add(IceWine(), 10);
            this.Recipes.Add(ABitOfSnow(), 10);
            this.Recipes.Add(BrokenLake(), 10);
            this.Recipes.Add(ThinIce(), 10);

            this.Recipes.Add(SnowStorm(), 20);
            this.Recipes.Add(SnowStorm2(), 20);
            this.Recipes.Add(SnowStorm3(), 20);
            this.Recipes.Add(SnowStorm4(), 20);
            this.Recipes.Add(GhostsGo(), 10);
            this.Recipes.Add(CenterPillar(), 20);
            this.Recipes.Add(CenterPillar2(), 20);
            this.Recipes.Add(CenterPillar3(), 20);

            this.Recipes.Add(IceRiver1(), 40);
            this.Recipes.Add(IceRiver2(), 40);
            this.Recipes.Add(IceRiver3(), 40);
            this.Recipes.Add(IceRiver4(), 40);
            this.Recipes.Add(IceRiver5(), 40);
            
            foreach (BoardRecipe r in this.Recipes.Keys)
            {
                r.Ingredients.Add(new PossibilityOfGhostsFeature(10, 2));
            }
        }


        private BoardRecipe ABitOfSnow()
        {
            BoardRecipe Room = new BoardRecipe("ABitOfSnow", 668, 1528);
            Room.Ingredients.Add(new Ingredient(new SnowToken(), PatternType.CenterBlob, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Room;
        }

        private BoardRecipe BrokenLake()
        {
            BoardRecipe Room = new BoardRecipe("BrokenLake", 681, 1621);
            Room.Ingredients.Add(new TerrainIngredient(new IceToken()));
            Room.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CenterBlob, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Room;
        }

        private BoardRecipe CenterPillar()
        {
            BoardRecipe Room = new BoardRecipe("CenterPillar", 641, 1497);
            Room.Ingredients.Add(new TileFeature(new IceToken(), 4, 4, "", 10, 12));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS, true));
            return Room;
        }

        private BoardRecipe CenterPillar2()
        {
            BoardRecipe Room = new BoardRecipe("CenterPillar2", 729, 1589);
            Room.Ingredients.Add(new TerrainIngredient(new SnowToken()));
            Room.Ingredients.Add(new TileFeature(new IceToken(), 3, 3, "", 4, 6));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.SmallSymmetricBlockEdgePattern, AddTokenMethod.ALWAYS, true));
            return Room;
        }

        private BoardRecipe CenterPillar3()
        {
            BoardRecipe Room = new BoardRecipe("CenterPillar3", 680, 1548);
            Room.Ingredients.Add(new TileFeature(new IceToken(), 3, 3, "", 4, 6));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.OuterRing, AddTokenMethod.ALWAYS, true));
            return Room;
        }


        private BoardRecipe CrossRoad()
        {
            BoardRecipe Room = new BoardRecipe("CrossRoad", 702, 1486);
            Room.Ingredients.Add(new TerrainIngredient(new SnowToken()));
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.DoubleCross, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.CentralCorners, AddTokenMethod.ALWAYS, true));
            return Room;
        }

        private BoardRecipe DividedLand()
        {
            BoardRecipe Room = new BoardRecipe("DividedLand", 745, 1573);
            Room.Ingredients.Add(new DivideFeature(new IceToken(), new SnowToken()));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.OneFullLine, AddTokenMethod.ALWAYS, true));
            //Room.Ingredients.Add(new HighwayFeature());
            return Room;
        }

        private BoardRecipe FrozenLake()
        {
            BoardRecipe Room = new BoardRecipe("FrozenLakeShore", 655, 1543);
            Room.Ingredients.Add(new TerrainIngredient(new SnowToken()));
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.CenterSixteen, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));

            return Room;
        }
        private BoardRecipe GhostsGo()
        {
            BoardRecipe Room = new BoardRecipe("GhostsGo", 666, 1618);
            Room.Ingredients.Add(new TileFeature(new IceToken(), 3, 3, "", 7, 7));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new MovingGhostToken(Direction.UP, MoveMethod.HORIZONTAL_PACE, 1), PatternType.Four, AddTokenMethod.ONLY_TERRAIN, false));
            return Room;
        }

        private BoardRecipe IceCheckers()
        {
            BoardRecipe Room = new BoardRecipe("IceCheckers", 587, 1671);
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.LargeCheckers, AddTokenMethod.ONLY_TERRAIN, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.DottedLine, AddTokenMethod.ONLY_TERRAIN, true));

            return Room;
        }
        private BoardRecipe IceDots()
        {
            BoardRecipe Room = new BoardRecipe("IceDots", 630, 1538);
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.FullDots, AddTokenMethod.ONLY_TERRAIN, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.ACouple, AddTokenMethod.ONLY_TERRAIN, true));
            Room.Ingredients.Add(new ZipperFeature());

            return Room;
        }

        private BoardRecipe IceHall()
        {
            BoardRecipe Room = new BoardRecipe("IceHall", 595, 1623);
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.WideLine, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.DottedCross, AddTokenMethod.ALWAYS, true));

            return Room;
        }


 
        private BoardRecipe IceRiver1()
        {
            BoardRecipe Room = new BoardRecipe("IceRiver1", 693, 1549);
            Room.Ingredients.Add(new TerrainIngredient(new IceToken()));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.SmallSymmetricBlockEdgePattern, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CrossBoardFourTurnPattern, AddTokenMethod.ALWAYS, true));
            return Room;
        }

        private BoardRecipe IceRiver2()
        {
            BoardRecipe Room = new BoardRecipe("IceRiver2", 629, 1565);
            Room.Ingredients.Add(new TerrainIngredient(new IceToken()));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.SmallSymmetricBlockEdgePattern, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CrossBoardFourTurnPattern, AddTokenMethod.ALWAYS, true));
            return Room;
        }

        private BoardRecipe IceRiver3()
        {
            BoardRecipe Room = new BoardRecipe("IceRiver3", 552, 1676);
            Room.Ingredients.Add(new TerrainIngredient(new IceToken()));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.SmallSymmetricBlockEdgePattern, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.CrossBoardFourTurnPattern, AddTokenMethod.ALWAYS, true));
            return Room;
        }

        private BoardRecipe IceRiver4()
        {
            BoardRecipe Room = new BoardRecipe("IceRiver4", 772, 1526);
            Room.Ingredients.Add(new TileFeature(new SnowToken(), 3, 3, "", 7, 7));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.CrossBoardFourTurnPattern, AddTokenMethod.ALWAYS, true));
            return Room;
        }
        private BoardRecipe IceRiver5()
        {
            BoardRecipe Room = new BoardRecipe("IceRiver5",667,1513);
            Room.Ingredients.Add(new Ingredient(new SnowToken(), PatternType.OuterRing, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new SnowToken(), PatternType.CenterBlob, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.CrossBoardFourTurnPattern, AddTokenMethod.ALWAYS, true));
            return Room;
        }

        private BoardRecipe IceRoomRecipe()
        {
            BoardRecipe Room = new BoardRecipe("IceRoom", 526, 1598);
            string Pattern = "";
            Room.Ingredients.Add(new TerrainPinwheelFeature(TokenType.ICE, Pattern, 8, 10));

            return Room;
        }

        private BoardRecipe IceShore()
        {
            BoardRecipe Room = new BoardRecipe("IceShore", 650, 1606);
            Room.Ingredients.Add(new TerrainIngredient(new SnowToken()));
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.BubbleEdge, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));

            return Room;
        }
        private BoardRecipe IceWine()
        {
            BoardRecipe Room = new BoardRecipe("IceWine", 697, 1605);
            Room.Ingredients.Add(new TerrainIngredient(new IceToken()));
            Room.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.CenterBlob, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Room;
        }

 

        private BoardRecipe PillarsOfIce()
        {
            BoardRecipe Room = new BoardRecipe("PillarsOfIce", 688, 1540);
            Room.Ingredients.Add(new TerrainIngredient(new IceToken()));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.ACouple, AddTokenMethod.ONLY_TERRAIN, true));

            return Room;
        }

        private BoardRecipe SnowCheckers()
        {
            BoardRecipe Room = new BoardRecipe("SnowCheckers",635,1495);
            Room.Ingredients.Add(new Ingredient(new SnowToken(), PatternType.LargeCheckers, AddTokenMethod.ONLY_TERRAIN, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.ACouple, AddTokenMethod.ONLY_TERRAIN, true));

            return Room;
        }

        private BoardRecipe SnowCross()
        {
            BoardRecipe Room = new BoardRecipe("SnowCross", 655, 1555);
            Room.Ingredients.Add(new TerrainIngredient(new SnowToken()));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.HorizontalLine, AddTokenMethod.ONLY_TERRAIN, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.VerticalLine, AddTokenMethod.ONLY_TERRAIN, true));
            return Room;
        }


        private BoardRecipe SnowKeep()
        {
            BoardRecipe Room = new BoardRecipe("SnowKeep",1131,1371);
            Room.Ingredients.Add(new TerrainIngredient(new SnowToken()));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.OuterRing, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS, true));
            return Room;
        }

        private BoardRecipe SnowQuads()
        {
            BoardRecipe Room = new BoardRecipe("SnowQuads", 738,1518);
            Room.Ingredients.Add(new Ingredient(new SnowToken(), PatternType.CrossQuads, AddTokenMethod.EMPTY, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.HorizontalLine, AddTokenMethod.EMPTY, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.HorizontalLine, AddTokenMethod.EMPTY, true));
            return Room;
        }


        private BoardRecipe SnowStorm()
        {
            BoardRecipe Room = new BoardRecipe("SnowStorm", 694, 1574);
            Room.Ingredients.Add(new TerrainIngredient(new IceToken()));
            Room.Ingredients.Add(new TileFeature(new SnowToken(), 3, 3, "", 5, 6));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Room;
        }


        private BoardRecipe SnowStorm2()
        {
            BoardRecipe Room = new BoardRecipe("SnowStorm2", 705, 1613);
            Room.Ingredients.Add(new TileFeature(new IceToken(), 3, 3, "", 6, 7));
            Room.Ingredients.Add(new TileFeature(new SnowToken(), 3, 3, "", 4, 5));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Room;
        }
        private BoardRecipe SnowStorm3()
        {
            BoardRecipe Room = new BoardRecipe("SnowStorm3", 614, 1550);
            Room.Ingredients.Add(new TileFeature(new IceToken(), 3, 3, "", 5, 6));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Room;
        }

        private BoardRecipe SnowStorm4()
        {
            BoardRecipe Room = new BoardRecipe("SnowStorm4", 737, 1541);
            Room.Ingredients.Add(new TerrainIngredient(new IceToken()));
            Room.Ingredients.Add(new TileFeature(new SnowToken(), 3, 3, "", 5, 6));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Room;
        }


        private BoardRecipe StarRoom()
        {
            BoardRecipe Room = new BoardRecipe("StarRoom", 592, 1560);
            Room.Ingredients.Add(new TerrainIngredient(new IceToken()));
            Room.Ingredients.Add(new Ingredient(new StickyToken(), PatternType.FullStarPattern, AddTokenMethod.ONLY_TERRAIN, true));
            return Room;
        }

 

        private BoardRecipe SwirlingIce()
        {
            BoardRecipe Room = new BoardRecipe("SwirlingIce",752,1524);
            Room.Ingredients.Add(new TerrainIngredient(new IceToken()));
            Room.Ingredients.Add(new SwirlingArrowsFeature(Rotation.CLOCKWISE,"",2,4));

            return Room;
        }
        private BoardRecipe TheDevide()
        {
            BoardRecipe Room = new BoardRecipe("TheDivide", 694, 1566);
            Room.Ingredients.Add(new TerrainIngredient(new SnowToken()));
            Room.Ingredients.Add(new RiftArrowFeature());
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.ACouple, AddTokenMethod.ONLY_TERRAIN, true));

            return Room;
        }
        private BoardRecipe ThinIce()
        {
            BoardRecipe Room = new BoardRecipe("ThinIce", 666, 1582);
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.CenterBlob, AddTokenMethod.ALWAYS));
            Room.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ONLY_TERRAIN, true));
            Room.Ingredients.Add(new Ingredient(new PitToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS));
            return Room;
        }

        private BoardRecipe ThroneRoomRecipe()
        {
            BoardRecipe ThroneRoom = new BoardRecipe("ThroneRoom",742,1482);
            ThroneRoom.Ingredients.Add(new TerrainIngredient(new IceToken()));
            ThroneRoom.Ingredients.Add(new SymmetricIcePillarsFeature());
 
            return ThroneRoom;
        }

       
             

    }

}

