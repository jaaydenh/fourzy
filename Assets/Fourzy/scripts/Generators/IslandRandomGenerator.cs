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
                { "circle_bomb", 50},
                { "clear", 20 },
                { "sticky", 2 },
                { "ice", 0 },
                { "ghost", 1},
                { "pit", 10 },
                { "left_arrow", 1},
                { "right_arrow", 1},
                { "up_arrow", 1},
                { "down_arrow", 1},
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
                { "sticky", 5},
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
             { "fullbigcheckers", 20},
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
                { "sticky", 2},
                { "fruit", 2 },
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
             { "o_shape", 100 },
             { "l_shape", 5 },
             { "u_shape", 5 },
             { "checkers", 0 },
             { "cross", 30},
             { "diagcross", 10},
             { "drops", 50},
             { "steps", 0},
             { "dotsquare", 20},
             { "letter", 0},
             { "smiley", 10},
             { "corners", 20},
             { "cornerbrackets", 50},
             { "riverofarrows", 0},
             { "cycle", 0},
             { "faceoff", 0},
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
                { "up_arrow", 2 },
                { "down_arrow", 2 },
                { "left_arrow", 2 },
                { "right_arrow", 2 },
                { "pit", 2 },
                { "sticky", 5},
                { "ice", 0},
                { "water", 10 },
                { "fruit", 5 },
                { "left_turn", 0 },
                { "right_turn", 0 },
                { "circle_bomb", 1 },
                { "empty", 20 }
            };
        }
    }
}

