using Fourzy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleRandomGenerator : BoardGenerator
{

    public string Name = "Castle Generator";
    public int MinNoise = 0;
    public int MaxNoise = 4;
    public override Dictionary<string, int> NoiseTokens
    {
        get
        {
            return new Dictionary<string, int>()
            {
                { "circle_bomb", 0},
                { "clear", 20 },
                { "sticky", 1 },
                { "ice", 40 },
                { "ghost", 1},
                { "pit", 0 },
                { "left_arrow", 1},
                { "right_arrow", 1},
                { "up_arrow", 1},
                { "down_arrow", 1},
                { "blocker", 5},
                { "water", 10},
                { "left_turn", 0 },
                { "right_turn", 0 },
                { "bumper", 0 },

            };
        }
    }

    public override string base_terrain { get { return "empty"; } }
    public override Dictionary<string, int> TerrainTypes
    {
        get
        {
            return new Dictionary<string, int>()
            {
                { "empty", 40},
                { "sticky", 1}
            };
        }
    }

    public int MinLargeFeatures = 0;
    public int MaxLargeFeatures = 0;
    public override Dictionary<string, int> LargeFeatureTypes
    {
        get
        {
            return new Dictionary<string, int>()
            {
             { "bigsteps", 0},
             { "lotsofdrops", 40},
             { "hlines", 0},
             { "vlines", 0},
             { "quads", 40},
             { "diagcross", 0},
             { "mostlyfull", 20 },
             { "centersquare", 20 },
             { "blockedge", 20},
             { "vdoubleline", 0},
             { "hdoubleline", 0},
             { "doublecross", 0},
             { "fullcheckers", 20},
             { "diaglines", 0},
             { "blockalledges", 10},
             { "blocktwo", 5},
             { "blockcorner", 10 },
             { "centerplus", 30},
             { "fatdiagonal", 20},
             { "largering", 20},
             { "fullbigcheckers", 20},
             { "crosshair", 0},
             { "diagonalwaves", 20},
             { "donuts", 20},
             { "crookedlines", 20},
             { "wheel", 20}
            };
        }
    }
    public override Dictionary<string, int> LargeFeatureTokens
    {
        get
        {
            return new Dictionary<string, int>()
            {
                { "sticky", 1},
                { "water", 10 },
                { "clear", 20 }
            };
        }
    }

    public int MinFeatures = 2;
    public int MaxFeatures = 6;
    public override Dictionary<string, int> FeatureTypes
    {
        get
        {
            return new Dictionary<string, int>()
            {
             { "line", 50},
             { "rectangle", 50},
             { "o_shape", 100 },
             { "l_shape", 0 },
             { "u_shape", 0 },
             { "checkers", 200 },
             { "cross", 100},
             { "diagcross", 100},
             { "drops", 0},
             { "steps", 0},
             { "dotsquare", 100},
             { "letter", 0},
             { "smiley", 0},
             { "corners", 100},
             { "cornerbrackets", 100},
             { "riverofarrows", 0},
             { "cycle", 0},
             { "faceoff", 0},
             { "clearalledges", 20 }
          };
        }
    }
    public override Dictionary<string, int> FeatureTokens
    {
        get
        {
            return new Dictionary<string, int>()
            {
                { "up_arrow", 2 },
                { "down_arrow", 2 },
                { "left_arrow", 2 },
                { "right_arrow", 2 },
                { "pit", 2 },
                { "sticky", 10},
                { "ice", 0},
                { "water", 0 },
                { "fruit", 1 },
                { "left_turn", 0 },
                { "right_turn", 0 },
                { "circle_bomb", 0 },
                { "clear", 2 },
                { "ghost", 10 }
            };
        }
    }
}

