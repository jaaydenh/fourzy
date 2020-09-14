using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public static class TokenFactory
    {
        public static IToken Create(string TokenNotation)
        {
            //switch (TokenNotation.Substring(0, 1))
            switch (TokenNotation[0])
            {
                case TokenConstants.Arrow:
                    return new ArrowToken(TokenNotation);
                case TokenConstants.ArrowOnce:
                    return new ArrowOnceToken(TokenNotation);
                case TokenConstants.Blocker:
                    return new BlockerToken();
                case TokenConstants.Bounce:
                    return new BounceToken();
                case TokenConstants.CircleBomb:
                    return new CircleBombToken(TokenNotation);
                case TokenConstants.Cold:
                    return new ColdToken();
                case TokenConstants.CrossBomb:
                    return new CrossBombToken(TokenNotation);
                case TokenConstants.Darkness:
                    return new DarknessToken(TokenNotation);
                case TokenConstants.Fire:
                    return new FireToken();
                case TokenConstants.Flowers:
                    return new FlowersToken(TokenNotation);
                case TokenConstants.Fountain:
                    return new FountainToken();
                case TokenConstants.FourWayArrow:
                    return new FourWayArrowToken();
                case TokenConstants.Fruit:
                    return new FruitToken();
                case TokenConstants.FruitTree:
                    return new FruitTreeToken();
                case TokenConstants.Ghost:
                    return new GhostToken();
                case TokenConstants.GlassBlock:
                    return new GlassBlockToken();
                case TokenConstants.Gold:
                    return new GoldToken(TokenNotation);
                case TokenConstants.Guard:
                    return new GuardToken(TokenNotation);
                case TokenConstants.Grass:
                    return new GrassToken(TokenNotation);
                case TokenConstants.Hex:
                    return new HexSpellToken(TokenNotation);
                case TokenConstants.HiddenBomb:
                    return new HiddenBombToken(TokenNotation);
                case TokenConstants.Hold:
                    return new HoldSpellToken(TokenNotation);
                case TokenConstants.Ice:
                    return new IceToken();
                case TokenConstants.IceBlock:
                    return new IceBlockToken();
                case TokenConstants.Lava:
                    return new LavaToken();
                case TokenConstants.LeftTurn:
                    return new LeftTurnToken(TokenNotation);
                case TokenConstants.Lure:
                    return new LureToken(TokenNotation);
                case TokenConstants.MoveBlocker:
                    return new MoveBlockerToken();
                case TokenConstants.MovingCloud:
                    return new MovingCloudToken(TokenNotation);
                case TokenConstants.MovingGhost:
                    return new MovingGhostToken(TokenNotation);
                case TokenConstants.MovingSun:
                    return new MovingSunToken(TokenNotation);
                case TokenConstants.Mud:
                    return new MudToken();
                case TokenConstants.Pit:
                    return new PitToken();
                case TokenConstants.PopUpBlocker:
                    return new PopUpBlockerToken(TokenNotation);
                case TokenConstants.Portal:
                    return new PortalToken(TokenNotation);
                case TokenConstants.Puddle:
                    return new PuddleToken();
                case TokenConstants.Rainbow:
                    return new RainbowToken();
                case TokenConstants.RandomFourWay:
                    return new FourWayRandomArrowToken();
                case TokenConstants.RandomLeftRight:
                    return new RandomLeftRightArrowToken();
                case TokenConstants.RandomUpDown:
                    return new RandomUpDownArrowToken();
                case TokenConstants.RightTurn:
                    return new RightTurnToken(TokenNotation);
                case TokenConstants.RotatingArrow:
                    return new RotatingArrowToken(TokenNotation);
                case TokenConstants.Sand:
                    return new SandToken();
                case TokenConstants.Shark:
                    return new SharkToken(TokenNotation);
                case TokenConstants.Snow:
                    return new SnowToken(TokenNotation);
                case TokenConstants.Snowman:
                    return new SnowManToken();
                case TokenConstants.Spider:
                    return new SpiderToken();
                case TokenConstants.SpiderWeb:
                    return new SpiderWebToken();
                case TokenConstants.Sticky:
                    return new StickyToken();
                case TokenConstants.Switch:
                    return new SwitchToken(TokenNotation);
                case TokenConstants.ToggleArrow:
                    return new ToggleArrowToken(TokenNotation);
                case TokenConstants.TrapDoor:
                    return new TrapDoorToken(TokenNotation);
                case TokenConstants.Quicksand:
                    return new QuickSandToken();
                case TokenConstants.Volcano:
                    return new VolcanoToken();
                case TokenConstants.Wall:
                    return new WallToken(TokenNotation);
                case TokenConstants.Water:
                    return new WaterToken();
                case TokenConstants.Wind:
                    return new WindToken(TokenNotation);
                case TokenConstants.Wisp:
                    return new WispToken(TokenNotation);
                case TokenConstants.Zone:
                    return new ZoneToken(TokenNotation);
            }

            return null; 
        }

        public static IToken Create(TokenType Token)
        {
            //switch (TokenNotation.Substring(0, 1))
            switch (Token)
            {
                case TokenType.BLOCKER:
                    return new BlockerToken();
                case TokenType.CIRCLE_BOMB:
                    return new CircleBombToken();
                case TokenType.CROSS_BOMB:
                    return new CrossBombToken();
                case TokenType.FRUIT:
                    return new FruitToken();
                case TokenType.FRUIT_TREE:
                    return new FruitTreeToken();
                case TokenType.FOURWAY_ARROW:
                    return new FourWayArrowToken();
                case TokenType.GHOST:
                    return new GhostToken();
                case TokenType.ICE:
                    return new IceToken();
                case TokenType.PIT:
                    return new PitToken();
                case TokenType.SAND:
                    return new SandToken();
                case TokenType.SPIDER:
                    return new SpiderToken();
                case TokenType.WEB:
                    return new SpiderWebToken();
                case TokenType.STICKY:
                    return new StickyToken();
                case TokenType.WATER:
                    return new WaterToken();
            }

            return null;
        }

        ////We may not need both a create and create terrain.
        //public static IToken CreateTerrain(TokenType Token)
        //{
        //    switch (Token)
        //    {
        //        case TokenType.STICKY:
        //            return new StickyToken();
        //        case TokenType.SAND:
        //            return new SandToken();
        //        case TokenType.WATER:
        //            return new WaterToken();
        //        case TokenType.ICE:
        //            return new IceToken();
        //        case TokenType.SNOW:
        //            return new SnowToken();
        //        case TokenType.GRASS:
        //            return new SnowToken();
        //        case TokenType.THIN_ICE:
        //            return new ThinIceToken();
        //    }

        //    return null;
        //}
                     

        //public static TokenType IdentifyToken(string TokenNotation)
        //{
        //    //switch (TokenNotation.Substring(0, 1))
        //    switch (TokenNotation[0])
        //    {
        //        case TokenConstants.Arrow:
        //            return TokenType.ARROW;
        //        case TokenConstants.Blocker:
        //            return TokenType.BLOCKER;
        //        case TokenConstants.Bounce:
        //            return TokenType.BOUNCE;
        //        case TokenConstants.CircleBomb:
        //            return TokenType.CIRCLE_BOMB;
        //        case TokenConstants.Cold:
        //            return TokenType.COLD;
        //        case TokenConstants.CrossBomb:
        //            return TokenType.CROSS_BOMB;
        //        case TokenConstants.Fire:
        //            return TokenType.FIRE;
        //        case TokenConstants.Flowers:
        //            return TokenType.FLOWERS;
        //        case TokenConstants.Ice:
        //            return TokenType.ICE;
        //        case TokenConstants.IceBlock:
        //            return TokenType.ICE_BLOCK;
        //        case TokenConstants.Sticky:
        //            return TokenType.STICKY;
        //    }

        //    return TokenType.NONE;
        //}

    }
}
