//using System.Collections;
//using System.Collections.Generic;

//namespace FourzyGameModel.Model
//{

//    public class GenericRandomGenerator : BoardGenerator
//    {
        
//        public string Name = "Somewhere in FourzyLand";
//        public GenericRandomGenerator(List<TokenType> AllowedTokens)
//        {

//        }
        
//        public override Dictionary<string, int> NoiseTokens
//        {
//            get
//            {
//                return new Dictionary<string, int>()
//            {
//                { "circle_bomb", 0},
//                { "clear", 20 },
//                { "sticky", 10 },
//                { "ice", 0 },
//                { "ghost", 0},
//                { "pit", 0 },
//                { "left_arrow", 10},
//                { "right_arrow", 10},
//                { "up_arrow", 10},
//                { "down_arrow", 10},
//                { "blocker", 10},
//                { "water", 0},
//                { "left_turn", 0 },
//                { "right_turn", 0 },
//                { "bumper", 0 }
//            };
//            }
//        }
//        public override Dictionary<string, int> TerrainTypes
//        {
//            get
//            {
//                return new Dictionary<string, int>()
//            {
//            { "empty", 15},
//            { "sticky", 1},
//            };
//            }
//        }

//        public int MinLargeFeatures = 0;
//        public int MaxLargeFeatures = 1;
//        public override Dictionary<string, int> LargeFeatureTypes
//        {
//            get
//            {
//                return new Dictionary<string, int>()
//            {
//             { "bigsteps", 0},
//             { "lotsofdrops", 0},
//             { "hlines", 100},
//             { "vlines", 100},
//             { "quads", 0},
//             { "diagcross", 50},
//             { "mostlyfull", 0 },
//             { "centersquare", 0 },
//             { "blockedge", 50},
//             { "vdoubleline", 2},
//             { "hdoubleline", 2},
//             { "doublecross", 0},
//             { "fullcheckers", 50},
//             { "diaglines", 0},
//             { "blockalledges", 0},
//             { "blocktwo", 0},
//             { "blockcorner", 0 },
//             { "centerplus", 0},
//             { "fatdiagonal", 0},
//             { "largering", 0},
//             { "fullbigcheckers", 0},
//             { "crosshair", 0},
//             { "diagonalwaves", 0},
//             { "donuts", 0},
//             { "crookedlines", 0},
//             { "wheel", 0}
//            };
//            }
//        }
//        public override Dictionary<string, int> LargeFeatureTokens
//        {
//            get
//            {
//                return new Dictionary<string, int>()
//            {
//                { "sticky", 5},
//                { "fruit", 10 }
//            };
//            }
//        }

//        public int MinFeatures = 1;
//        public int MaxFeatures = 1;
//        public override Dictionary<string, int> FeatureTypes
//        {
//            get
//            {
//                return new Dictionary<string, int>()
//            {
//             { "line", 200},
//             { "rectangle", 50},
//             { "o_shape", 100 },
//             { "l_shape", 50 },
//             { "u_shape", 10 },
//             { "checkers", 50 },
//             { "cross", 200},
//             { "diagcross", 200},
//             { "drops", 50},
//             { "steps", 0},
//             { "dotsquare", 0},
//             { "letter", 100},
//             { "smiley", 50},
//             { "corners", 100},
//             { "cornerbrackets", 100},
//             { "riverofarrows", 0},
//             { "cycle", 0},
//             { "faceoff", 0}
//            };
//            }
//        }
//        public override Dictionary<string, int> FeatureTokens
//        {
//            get
//            {
//                return new Dictionary<string, int>()
//            {
//                { "up_arrow", 2 },
//                { "down_arrow", 2 },
//                { "left_arrow", 2 },
//                { "right_arrow", 2 },
//                { "pit", 0 },
//                { "sticky", 20},
//                { "ice", 0},
//                { "water", 0},
//                { "fruit", 40 },
//                { "left_turn", 0 },
//                { "right_turn", 0 },
//                { "circle_bomb", 0 },
//            };
//            }
//        }
//    }

//}

