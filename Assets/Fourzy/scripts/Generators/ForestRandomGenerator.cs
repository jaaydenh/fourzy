using Fourzy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestRandomGenerator : BoardGenerator {

    public string Name = "Forest";
    public int MinNoise = 4;
    public int MaxNoise = 8;
    public override Dictionary<string, int> NoiseTokens
    {
        get
        {
            return new Dictionary<string, int>()
            {
                { "circle_bomb", 0},
                { "clear", 10 },
                { "sticky", 10 },
                { "ice", 0 },
                { "ghost", 10},
                { "pit", 20 },
                { "left_arrow", 5},
                { "right_arrow", 5},
                { "up_arrow", 5},
                { "down_arrow", 5},
                { "blocker", 5},
                { "water", 10},
                { "left_turn", 0 },
                { "right_turn", 0 },
                { "bumper", 0 }
            };
        }
    }
    public override Dictionary<string, int> TerrainTypes
    {
        get
        {
            return new Dictionary<string, int>()
            {
                { "empty", 15},
                { "sticky", 5},
                { "water", 10 }
            };
        }
    }

    public int MinLargeFeatures = 1;
    public int MaxLargeFeatures = 2;
    public override Dictionary<string, int> LargeFeatureTypes
    {
        get
        {
            return new Dictionary<string, int>()
            {
             { "bigsteps", 5},
             { "lotsofdrops", 40},
             { "hlines", 0},
             { "vlines", 0},
             { "quads", 0},
             { "diagcross", 0},
             { "mostlyfull", 0 },
             { "centersquare", 0 },
             { "blockedge", 20},
             { "vdoubleline", 0},
             { "hdoubleline", 0},
             { "doublecross", 0},
             { "fullcheckers", 0},
             { "diaglines", 0},
             { "blockalledges", 5},
             { "blocktwo", 5},
             { "blockcorner", 10 },
             { "centerplus", 0},
             { "fatdiagonal", 0},
             { "largering", 20},
             { "fullbigcheckers", 0},
             { "crosshair", 0},
             { "diagonalwaves", 0},
             { "donuts", 0},
             { "crookedlines", 0},
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
                { "sticky", 5},
                { "fruit", 5 },
                { "water", 10 }
            };
        }
    }

    public int MinFeatures = 1;
    public int MaxFeatures = 3;
    public override Dictionary<string, int> FeatureTypes
    {
        get
        {
            return new Dictionary<string, int>()
            {
             { "line", 200},
             { "rectangle", 50},
             { "o_shape", 100 },
             { "l_shape", 50 },
             { "u_shape", 10 },
             { "checkers", 50 },
             { "cross", 200},
             { "diagcross", 200},
             { "drops", 50},
             { "steps", 0},
             { "dotsquare", 0},
             { "letter", 0},
             { "smiley", 0},
             { "corners", 100},
             { "cornerbrackets", 100},
             { "riverofarrows", 0},
             { "cycle", 0},
             { "faceoff", 0}
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
                { "pit", 5 },
                { "sticky", 10},
                { "ice", 0},
                { "water", 10 },
                { "fruit", 5 },
                { "left_turn", 0 },
                { "right_turn", 0 },
                { "circle_bomb", 0 }
            };
        }
    }
}

