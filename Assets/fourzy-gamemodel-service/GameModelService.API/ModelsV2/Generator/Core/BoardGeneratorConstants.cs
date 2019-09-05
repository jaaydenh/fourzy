using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public static class BoardGeneratorConstants
    {
        public static List<TokenType> TerrainTokens
        {
            get
            {
                return new List<TokenType>() {
                TokenType.SNOW, TokenType.SAND, TokenType.GRASS, TokenType.ICE, TokenType.STICKY, TokenType.WATER };
            }
        }

        public const int MAX_GENERATOR_ATTEMPTS = 10;
    }

    public enum LineType { VERTICAL, HORIZONTAL, DIAGONAL, NONE};

    public enum AddTokenMethod {ALWAYS, EMPTY, NO_TERRAIN, ONLY_TERRAIN, IF_NO_TOKEN_MATCH,
        CORNERS
    }

    public enum LargeFeatureType { BIGSTEPS, LOTS_OF_DROPS, HLINES, VLINES, QUADS, DIAGCROSS, CENTER_SQUARE, BLOCK_EDGE, VDOUBLELINE, HDOUBLELINE, DOUBLECROSS, FULLCHECKERS, DIAGLINES, BLOCK_ALL_EDGES, BLOCK_TWO_EDGES, BLOCK_CORNERS, CENTER_PLUS, FAT_DIAGONAL, LARGE_RING, FULL_BIG_CHECKERS, CROSSHAIR, DIAGONAL_WAVES, DONUTS, CROOKED_LINES, WHEEL,
        HALF,
        ARROW_STORM,
        DIVIDE
    }

    public enum SmallFeatureType { LINE, RECTANGLE, O_SHAPE, L_SHAPE, U_SHAPE, CHECKERS, CROSS, DIAGCROSS, STEPS, LETTER, SMILEY, CORNERS, CORNER_BRACKETS, RIVER_OF_ARROWS, CYCLE, FACEOFF,
        STAGGERED, ARROW_FOUR_SIDES, DOTTEDLINE,
        DOUBLELINE_OF_ARROWS,
        TREES_DROP_FRUIT,
        A_COUPLE_OF_GHOSTS,
        HIGHWAY_ARROWS,
        ARROW_HIGHWAY,
        ARROWS_OUT,
        ARROWS_IN,
        ARROWS_SwIRLING,
        A_COUPLE_OF_ARROWS,
        MATCHING_ARROWS,
        RIFT_OF_ARROWS
    }

    public enum IngredientType { TERRAIN, LARGEFEATURE, SMALLFEATURE, NOISE,
        ENHANCEMENT,
        SYMMETRICARROWS
    }
   
}
