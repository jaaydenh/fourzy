using Fourzy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePalaceRandomGenerator : BoardGenerator
{

    public string Name = "Ice Palace Generator";
    public int MinNoise = 8;
    public int MaxNoise = 12;
    public override Dictionary<string, int> NoiseTokens
    {
        get
        {
            return new Dictionary<string, int>()
            {
                { "circle_bomb", 0},
                { "clear", 5 },
                { "sticky", 1 },
                { "ice", 0 },
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

    public override string base_terrain { get { return "ice"; } }
    public override Dictionary<string, int> TerrainTypes
    {
        get
        {
            return new Dictionary<string, int>()
            {
                { "empty", 40},
                { "sticky", 1},
                { "water", 10 },
                { "ice", 40 }
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
             { "quads", 40},
             { "diagcross", 0},
             { "mostlyfull", 20 },
             { "centersquare", 20 },
             { "blockedge", 100},
             { "halfalledges", 0},
             { "blockalledges", 5},
             { "clearalledges", 0},
             { "vdoubleline", 0},
             { "hdoubleline", 0},
             { "doublecross", 0},
             { "fullcheckers", 20},
             { "diaglines", 0},
             { "blocktwo", 5},
             { "blockcorner", 10 },
             { "centerplus", 30},
             { "fatdiagonal", 20},
             { "largering", 20},
             { "fullbigcheckers", 50},
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
                { "fruit", 2 },
                { "water", 10 },
                { "ice", 20 },
                { "clear", 20 }
            };
        }
    }

    public int MinFeatures = 4;
    public int MaxFeatures = 6;
    public override Dictionary<string, int> FeatureTypes
    {
        get
        {
            return new Dictionary<string, int>()
            {
             { "line", 100},
             { "rectangle", 100},
             { "o_shape", 100 },
             { "l_shape", 5 },
             { "u_shape", 5 },
             { "checkers", 40 },
             { "cross", 30},
             { "diagcross", 10},
             { "drops", 50},
             { "steps", 20},
             { "dotsquare", 50},
             { "letter", 0},
             { "smiley", 10},
             { "corners", 20},
             { "cornerbrackets", 50},
             { "riverofarrows", 0},
             { "cycle", 0},
             { "faceoff", 0},
             { "clearalledges", 0 }
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
                { "sticky", 1},
                { "ice", 0},
                { "water", 10 },
                { "fruit", 2 },
                { "left_turn", 0 },
                { "right_turn", 0 },
                { "circle_bomb", 0 },
                { "clear", 20 },
                { "ghost", 5 }
            };
        }
    }
}

