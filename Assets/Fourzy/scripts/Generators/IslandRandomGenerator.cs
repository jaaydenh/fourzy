using Fourzy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandRandomGenerator : BoardGenerator
{

    public string Name = "Island Generator";
    public int MinNoise = 8;
    public int MaxNoise = 12;
    public override Dictionary<string, int> NoiseTokens
    {
        get
        {
            return new Dictionary<string, int>()
            {
                { "circle_bomb", 100},
                { "clear", 5 },
                { "sticky", 1 },
                { "ice", 0 },
                { "ghost", 0},
                { "pit", 10 },
                { "left_arrow", 0},
                { "right_arrow", 0},
                { "up_arrow", 0},
                { "down_arrow", 0},
                { "blocker", 5},
                { "water", 10},
                { "left_turn", 0 },
                { "right_turn", 0 },
                { "bumper", 0 },
                { "sand", 0 }

            };
        }
    }

    public override string base_terrain { get { return "sand"; } }
    public override Dictionary<string, int> TerrainTypes
    {
        get
        {
            return new Dictionary<string, int>()
            {
                { "empty", 20},
                { "water", 10 },
                { "sand", 5 }
            };
        }
    }

    public int MinLargeFeatures = 3;
    public int MaxLargeFeatures = 7;
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
             { "quads", 0},
             { "diagcross", 0},
             { "mostlyfull", 0 },
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
             { "fullbigcheckers", 0},
             { "crosshair", 0},
             { "diagonalwaves", 20},
             { "donuts", 0},
             { "crookedlines", 20},
             { "wheel", 0}
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
                { "fruit", 1 },
                { "water", 10 },
                { "empty", 20 }
            };
        }
    }

    public int MinFeatures = 3;
    public int MaxFeatures = 5;
    public override Dictionary<string, int> FeatureTypes
    {
        get
        {
            return new Dictionary<string, int>()
            {
             { "line", 200},
             { "rectangle", 50},
             { "o_shape", 50 },
             { "l_shape", 2 },
             { "u_shape", 2 },
             { "checkers", 0 },
             { "cross", 30},
             { "diagcross", 10},
             { "drops", 10},
             { "steps", 0},
             { "dotsquare", 20},
             { "letter", 0},
             { "smiley", 0},
             { "corners", 20},
             { "cornerbrackets", 50},
             { "riverofarrows", 0},
             { "cycle", 1000},
             { "faceoff", 1000},
             { "clearalledges", 50 }
          };
        }
    }
    public override Dictionary<string, int> FeatureTokens
    {
        get
        {
            return new Dictionary<string, int>()
            {
                { "up_arrow", 0 },
                { "down_arrow", 0 },
                { "left_arrow", 0 },
                { "right_arrow", 0 },
                { "pit", 2 },
                { "sticky", 1},
                { "ice", 0},
                { "water", 10 },
                { "fruit", 5 },
                { "left_turn", 0 },
                { "right_turn", 0 },
                { "circle_bomb", 5 },
                { "empty", 10 }
            };
        }
    }
}

