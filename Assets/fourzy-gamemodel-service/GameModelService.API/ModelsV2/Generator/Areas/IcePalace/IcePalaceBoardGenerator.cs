using System.Collections;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{

    public class IcePalaceRandomGenerator : BoardGenerator
    {

        public string Name = "Ice Palace";
        public override Area Area { get { return Area.ICE_PALACE; } }
        //Recipes contains a list of potential Recipes for Building out the Garden.
    
        public IcePalaceRandomGenerator(string RequestedRecipeName = "")
        {
            this.RequestedRecipeName = RequestedRecipeName;
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
        }

        private BoardRecipe ThinIce()
        {
            BoardRecipe Room = new BoardRecipe("ThinIce");
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.CenterBlob, AddTokenMethod.ALWAYS));
            Room.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ONLY_TERRAIN, true));
            Room.Ingredients.Add(new Ingredient(new PitToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS));
            return Room;
        }

        private BoardRecipe BrokenLake()
        {
            BoardRecipe Room = new BoardRecipe("BrokenLake");
            Room.Ingredients.Add(new TerrainIngredient(new IceToken()));
            Room.Ingredients.Add(new Ingredient(new WaterToken(), PatternType.CenterBlob, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Room;
        }

        private BoardRecipe ABitOfSnow()
        {
            BoardRecipe Room = new BoardRecipe("ABitOfSnow");
            Room.Ingredients.Add(new Ingredient(new SnowToken(), PatternType.CenterBlob, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Room;
        }

        private BoardRecipe IceWine()
        {
            BoardRecipe Room = new BoardRecipe("IceWine");
            Room.Ingredients.Add(new TerrainIngredient(new IceToken()));
            Room.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.CenterBlob, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));
            return Room;
        }

        private BoardRecipe SnowQuads()
        {
            BoardRecipe Room = new BoardRecipe("SnowQuads");
            Room.Ingredients.Add(new Ingredient(new SnowToken(), PatternType.CrossQuads, AddTokenMethod.EMPTY, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.HorizontalLine, AddTokenMethod.EMPTY, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.HorizontalLine, AddTokenMethod.EMPTY, true));
            return Room;
        }

        private BoardRecipe SnowCross()
        {
            BoardRecipe Room = new BoardRecipe("CrossRoad");
            Room.Ingredients.Add(new TerrainIngredient(new SnowToken()));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.HorizontalLine, AddTokenMethod.ONLY_TERRAIN, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.VerticalLine, AddTokenMethod.ONLY_TERRAIN, true));
            return Room;
        }

        private BoardRecipe SnowKeep()
        {
            BoardRecipe Room = new BoardRecipe("SnowKeep");
            Room.Ingredients.Add(new TerrainIngredient(new SnowToken()));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.OuterRing, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new FruitToken(), PatternType.CenterFour, AddTokenMethod.ALWAYS, true));
            return Room;
        }


        private BoardRecipe CrossRoad()
        {
            BoardRecipe Room = new BoardRecipe("CrossRoad");
            Room.Ingredients.Add(new TerrainIngredient(new SnowToken()));
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.DoubleCross, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.CentralCorners, AddTokenMethod.ALWAYS, true));
            return Room;
        }

        private BoardRecipe DividedLand()
        {
            BoardRecipe Room = new BoardRecipe("DividedLand");
            Room.Ingredients.Add(new DivideFeature(new IceToken(), new SnowToken()));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.OneFullLine, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new HighwayFeature());
            return Room;
        }

        private BoardRecipe FrozenLake()
        {
            BoardRecipe Room = new BoardRecipe("FrozenLakeShore");
            Room.Ingredients.Add(new TerrainIngredient(new SnowToken()));
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.CenterSixteen, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));

            return Room;
        }

        private BoardRecipe IceHall()
        {
            BoardRecipe Room = new BoardRecipe("IceHall");
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.WideLine, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.DottedCross, AddTokenMethod.ALWAYS, true));

            return Room;
        }


        private BoardRecipe IceShore()
        {
            BoardRecipe Room = new BoardRecipe("IceShore");
            Room.Ingredients.Add(new TerrainIngredient(new SnowToken()));
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.BubbleEdge, AddTokenMethod.ALWAYS, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.TwoRandom, AddTokenMethod.ALWAYS, true));

            return Room;
        }

        private BoardRecipe IceDots()
        {
            BoardRecipe Room = new BoardRecipe("IceDots");
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.FullDots, AddTokenMethod.ONLY_TERRAIN, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.ACouple, AddTokenMethod.ONLY_TERRAIN, true));
            Room.Ingredients.Add(new ZipperFeature());

            return Room;
        }

        private BoardRecipe IceCheckers()
        {
            BoardRecipe Room = new BoardRecipe("IceCheckers");
            Room.Ingredients.Add(new Ingredient(new IceToken(), PatternType.LargeCheckers, AddTokenMethod.ONLY_TERRAIN, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.DottedLine, AddTokenMethod.ONLY_TERRAIN, true));

            return Room;
        }

        private BoardRecipe SnowCheckers()
        {
            BoardRecipe Room = new BoardRecipe("SnowCheckers");
            Room.Ingredients.Add(new Ingredient(new SnowToken(), PatternType.LargeCheckers, AddTokenMethod.ONLY_TERRAIN, true));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.ACouple, AddTokenMethod.ONLY_TERRAIN, true));

            return Room;
        }

        private BoardRecipe TheDevide()
        {
            BoardRecipe Room = new BoardRecipe("TheDivide");
            Room.Ingredients.Add(new TerrainIngredient(new SnowToken()));
            Room.Ingredients.Add(new RiftArrowFeature());
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(), PatternType.ACouple, AddTokenMethod.ONLY_TERRAIN, true));

            return Room;
        }


        private BoardRecipe PillarsOfIce()
        {
            BoardRecipe Room = new BoardRecipe("PillarsOfIce");
            Room.Ingredients.Add(new TerrainIngredient(new IceToken()));
            Room.Ingredients.Add(new Ingredient(new IceBlockToken(),PatternType.ACouple,AddTokenMethod.ONLY_TERRAIN,true));

            return Room;
        }

        private BoardRecipe SwirlingIce()
        {
            BoardRecipe Room = new BoardRecipe("SwirlingIce");
            Room.Ingredients.Add(new TerrainIngredient(new IceToken()));
            Room.Ingredients.Add(new SwirlingArrowsFeature(Rotation.CLOCKWISE,"",2,4));

            return Room;
        }

        private BoardRecipe ThroneRoomRecipe()
        {
            BoardRecipe ThroneRoom = new BoardRecipe("ThroneRoom");
            ThroneRoom.Ingredients.Add(new TerrainIngredient(new IceToken()));
            ThroneRoom.Ingredients.Add(new SymmetricIcePillarsFeature());
 
            return ThroneRoom;
        }

        private BoardRecipe StarRoom()
        {
            BoardRecipe Room = new BoardRecipe("StarRoom");
            Room.Ingredients.Add(new TerrainIngredient(new IceToken()));
            Room.Ingredients.Add(new Ingredient(new StickyToken(),PatternType.FullStarPattern, AddTokenMethod.ONLY_TERRAIN,true));
            return Room;
        }

        private BoardRecipe IceRoomRecipe()
        {
            BoardRecipe Room = new BoardRecipe("IceRoom");
            string Pattern = "";
            Room.Ingredients.Add(new TerrainPinwheelFeature(TokenType.ICE, Pattern, 8,10));

            return Room;
        }
              

    }

}

