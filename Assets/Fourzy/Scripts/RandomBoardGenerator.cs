using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    public static class RandomBoardGenerator
    {
        public static TokenBoard GenerateBoard()
        {
            int comp = UnityEngine.Random.Range(0, 100);
            TokenBoard newBoard = null;
            string composition_name = GetRandomMember(BoardComposition.CompositionTypes);

            switch (composition_name)
            {
                case "uniform":
                    newBoard = GenerateUniformBoard();
                    break;

                case "centric":
                    newBoard = GenerateCentricBoard();
                    break;

                case "quads":
                    newBoard = GenerateQuads();
                    break;

                case "halfud":
                    newBoard = GenerateHalfUpDownBoard();
                    break;

                case "halflr":
                    newBoard = GenerateHalfRightLeftBoard();
                    break;

                case "hlines":
                    newBoard = GenerateHorizontalLinesBoard();
                    break;

                case "vlines":
                    newBoard = GenerateVerticalLinesBoard();
                    break;

            }

            AddFeatures(ref newBoard.tokenBoard, 1, 4);
            AddNoise(ref newBoard.tokenBoard, 5, 12);
            SetCorners(ref newBoard.tokenBoard);
            CheckArrows(ref newBoard.tokenBoard);
            CheckPits(ref newBoard.tokenBoard);

            newBoard.RefreshTokenBoard();

            return newBoard;
        }

        public static void AddNoise(ref int[,] board, int MinNoise, int MaxNoise)
        {
            int percentage = 1;
            int count = 0;

            while (count < MinNoise)
            {
                for (int r = 0; r < Constants.numRows; r++)
                {
                    for (int c = 0; c < Constants.numColumns; c++)
                    {
                        int hit = UnityEngine.Random.Range(0, 100);
                        if (hit <= percentage)
                        {
                            string tokenName = GetRandomMember(BoardNoise.TokenTypes);
                            int tokenId = ConvertTokenNameToTokenId(tokenName);
                            board[r, c] = tokenId;
                            count++;
                        }

                        if (count >= MaxNoise) return;
                    }
                }
            }

        }

        //private static int GenerateRandomFeature(int DefaultTokenId, int FeatureTokenId, int percentage)
        //{
        //    int rnd_percent = UnityEngine.Random.Range(0, 100);
        //    if (rnd_percent <= percentage) return FeatureTokenId;
        //    return DefaultTokenId;
        //}

        #region "Generate Board Compositions"
        #endregion

        public static TokenBoard GenerateUniformBoard()
        {
            int[,] tokenData = new int[Constants.numRows, Constants.numColumns];

            string default_terrain = RandomBaseTerrain();
            string overlay_terrain = RandomTerrain();
            string density = RandomDensity();

            for (int r = 0; r < Constants.numRows; r++)
            {
                for (int c = 0; c < Constants.numColumns; c++)
                {
                    tokenData[r, c] = GenerateRandomTerrain(default_terrain, overlay_terrain, density);
                }
            }

            TokenBoard newBoard = new TokenBoard(tokenData, "SomeId", "Random Board", null, null, true);
            return newBoard;
        }

        public static TokenBoard GenerateHorizontalLinesBoard()
        {
            int[,] tokenData = new int[Constants.numRows, Constants.numColumns];

            for (int r = 0; r < Constants.numRows; r++)
            {
                string default_terrain = RandomBaseTerrain();
                string overlay_terrain = RandomTerrain();
                string density = RandomDensity();

                for (int c = 0; c < Constants.numColumns; c++)
                {
                    tokenData[r, c] = GenerateRandomTerrain(default_terrain, overlay_terrain, density);
                }
            }

            SetCorners(ref tokenData);

            TokenBoard newBoard = new TokenBoard(tokenData, "SomeId", "Random Horizontal Lines Board", null, null, true);
            return newBoard;
        }

        private static int DistanceFromEdge(int r, int c)
        {
            return Math.Min(Math.Min(r, Constants.numRows - r - 1), Math.Min(c, Constants.numColumns - c - 1));
        }


        public static TokenBoard GenerateCentricBoard()
        {
            string default_terrain = RandomBaseTerrain();
            string[] overlay = new string[Constants.numRows];
            string[] density = new string[Constants.numRows];
            int[,] tokenData = new int[Constants.numRows, Constants.numColumns];

            for (int i = 0; i < Constants.numRows; i++)
            {
                overlay[i] = RandomTerrain();
                density[i] = RandomDensity();
            }

            for (int r = 0; r < Constants.numRows; r++)
            {
                for (int c = 0; c < Constants.numColumns; c++)
                {
                    int distance = DistanceFromEdge(r, c);
                    tokenData[r, c] = GenerateRandomTerrain(default_terrain, overlay[distance], density[distance]);
                }
            }

            TokenBoard newBoard = new TokenBoard(tokenData, "SomeId", "Centric Board", null, null, true);
            return newBoard;
        }

        public static TokenBoard GenerateVerticalLinesBoard()
        {
            int[,] tokenData = new int[Constants.numRows, Constants.numColumns];

            for (int c = 0; c < Constants.numColumns; c++)
            {
                string default_terrain = RandomBaseTerrain();
                string overlay_terrain = RandomTerrain();
                string density = RandomDensity();

                for (int r = 0; r < Constants.numRows; r++)
                {
                    tokenData[r, c] = GenerateRandomTerrain(default_terrain, overlay_terrain, density);
                }
            }

            SetCorners(ref tokenData);

            TokenBoard newBoard = new TokenBoard(tokenData, "SomeId", "Random Vertical Lines Board", null, null, true);
            return newBoard;
        }


        public static TokenBoard GenerateHalfUpDownBoard()
        {
            int[,] tokenData = new int[Constants.numRows, Constants.numColumns];

            string default_terrain = RandomTerrain();
            string overlay_terrain = RandomTerrain();
            string density = RandomDensity();

            for (int r = 0; r < Constants.numRows / 2; r++)
            {
                for (int c = 0; c < Constants.numColumns; c++)
                {
                    tokenData[r, c] = GenerateRandomTerrain(default_terrain, overlay_terrain, density);
                }
            }

            default_terrain = RandomBaseTerrain();
            overlay_terrain = RandomTerrain();
            density = RandomDensity();

            for (int r = Constants.numRows / 2; r < Constants.numRows; r++)
            {
                for (int c = 0; c < Constants.numColumns; c++)
                {
                    tokenData[r, c] = GenerateRandomTerrain(default_terrain, overlay_terrain, density);
                }
            }

            SetCorners(ref tokenData);

            TokenBoard newBoard = new TokenBoard(tokenData, "SomeId", "Random Half Up/Down", null, null, true);
            return newBoard;
        }

        public static TokenBoard GenerateHalfRightLeftBoard()
        {
            int[,] tokenData = new int[Constants.numRows, Constants.numColumns];

            string default_terrain = RandomBaseTerrain();
            string overlay_terrain = RandomTerrain();
            string density = RandomDensity();

            for (int r = 0; r < Constants.numRows; r++)
            {
                for (int c = 0; c < Constants.numColumns / 2; c++)
                {
                    tokenData[r, c] = GenerateRandomTerrain(default_terrain, overlay_terrain, density);
                }
            }

            default_terrain = RandomBaseTerrain();
            overlay_terrain = RandomTerrain();
            density = RandomDensity();

            for (int r = 0; r < Constants.numRows; r++)
            {
                for (int c = Constants.numColumns / 2; c < Constants.numColumns; c++)
                {
                    tokenData[r, c] = GenerateRandomTerrain(default_terrain, overlay_terrain, density);
                }
            }

            //SetCorners(ref tokenData);

            TokenBoard newBoard = new TokenBoard(tokenData, "SomeId", "Random Half Left/Right Board", null, null, true);
            return newBoard;
        }

        public static TokenBoard GenerateQuads()
        {
            int[,] tokenData = new int[Constants.numRows, Constants.numColumns];

            string default_terrain = RandomTerrain();
            string overlay_terrain = RandomTerrain();
            string density = RandomDensity();

            for (int r = 0; r < Constants.numRows / 2; r++)
            {
                for (int c = 0; c < Constants.numColumns / 2; c++)
                {
                    tokenData[r, c] = GenerateRandomTerrain(default_terrain, overlay_terrain, density);
                }
            }

            default_terrain = RandomTerrain();
            overlay_terrain = RandomTerrain();
            density = RandomDensity();

            for (int r = 0; r < Constants.numRows / 2; r++)
            {
                for (int c = Constants.numColumns / 2; c < Constants.numColumns; c++)
                {
                    tokenData[r, c] = GenerateRandomTerrain(default_terrain, overlay_terrain, density);
                }
            }

            default_terrain = RandomTerrain();
            overlay_terrain = RandomTerrain();
            density = RandomDensity();

            for (int r = Constants.numRows / 2; r < Constants.numRows; r++)
            {
                for (int c = 0; c < Constants.numColumns / 2; c++)
                {
                    tokenData[r, c] = GenerateRandomTerrain(default_terrain, overlay_terrain, density);
                }
            }

            default_terrain = RandomTerrain();
            overlay_terrain = RandomTerrain();
            density = RandomDensity();

            for (int r = Constants.numRows / 2; r < Constants.numRows; r++)
            {
                for (int c = Constants.numColumns / 2; c < Constants.numColumns; c++)
                {
                    tokenData[r, c] = GenerateRandomTerrain(default_terrain, overlay_terrain, density);
                }
            }



            TokenBoard newBoard = new TokenBoard(tokenData, "SomeId", "Random Quadrants", null, null, true);
            return newBoard;
        }

        private static void SetCorners(ref int[,] TokenData)
        {
            TokenData[0, 0] = (int)Token.BLOCKER;
            TokenData[0, Constants.numColumns - 1] = (int)Token.BLOCKER;
            TokenData[Constants.numRows - 1, 0] = (int)Token.BLOCKER;
            TokenData[Constants.numRows - 1, Constants.numColumns - 1] = (int)Token.BLOCKER;
        }

        private static void CheckPits(ref int[,] TokenData) {
            for (int r = 1; r < Constants.numRows - 1; r++)
            {
                if (TokenData[r, 0] == (int)Token.PIT) {
                    TokenData[r, 0] = (int)Token.EMPTY;
                }
                if (TokenData[r, Constants.numColumns - 1] == (int)Token.PIT)
                {
                    TokenData[r, Constants.numColumns - 1] = (int)Token.EMPTY;
                }
            }

            for (int c = 1; c < Constants.numColumns - 1; c++)
            {
                if (TokenData[0, c] == (int)Token.PIT)
                {
                    TokenData[0, c] = (int)Token.EMPTY;
                }
                if (TokenData[Constants.numRows - 1, c] == (int)Token.PIT)
                {
                    TokenData[Constants.numRows - 1, c] = (int)Token.EMPTY;
                }
            }
        }

        private static void CheckArrows(ref int[,] TokenData)
        {
            //check each row for left right arrows.
            for (int r = 0; r < Constants.numRows; r++)
            {
                bool found_left = false;
                bool found_right = false;
                for (int c = 0; c < Constants.numColumns; c++)
                {

                    switch (TokenData[r, c])
                    {
                        case (int)Token.LEFT_ARROW:
                            if (found_right) { TokenData[r, c] = (int)Token.RIGHT_ARROW; }
                            else found_left = true;

                            break;
                        case (int)Token.RIGHT_ARROW:
                            if (found_left) { TokenData[r, c] = (int)Token.LEFT_ARROW; }
                            else found_right = true;

                            break;
                    }

                }
            }

            //check each col for up/down arrows.
            for (int c = 0; c < Constants.numColumns; c++)
            {
                bool found_up = false;
                bool found_down = false;
                for (int r = 0; r < Constants.numRows; r++)
                {

                    switch (TokenData[r, c])
                    {
                        case (int)Token.UP_ARROW:
                            if (found_down) { TokenData[r, c] = (int)Token.DOWN_ARROW; }
                            else found_up = true;

                            break;
                        case (int)Token.DOWN_ARROW:
                            if (found_up) { TokenData[r, c] = (int)Token.UP_ARROW; }
                            else found_down = true;

                            break;
                    }
                }
            }


        }

        private static void AddFeatures(ref int[,] TokenData, int MinNumberOfFeatures, int MaxNumberOfFeatures)
        {
            int feature_count = UnityEngine.Random.Range(MinNumberOfFeatures, MaxNumberOfFeatures);
            for (int i = 0; i < feature_count; i++)
            {
                AddRandomFeature(ref TokenData);
            }
        }

        private static void AddRandomFeature(ref int[,] TokenData)
        {
            string feature = GetRandomMember(BoardFeatures.FeatureTypes);
            string defaultFeatureTokenName = GetRandomMember(BoardFeatures.TokenTypes);
            int defaultTokenId = ConvertTokenNameToTokenId(defaultFeatureTokenName);
            int insert_column = 0;
            int insert_row = 0;

            switch (feature)
            {
                case "line":
                    string linetype = GetRandomMember(BoardFeatures.LineTypes);

                    int line_length = 0;

                    switch (linetype)
                    {
                        case "vertical":
                            line_length = UnityEngine.Random.Range(3, Constants.numRows);
                            insert_column = UnityEngine.Random.Range(0, Constants.numColumns);
                            insert_row = UnityEngine.Random.Range(0, Constants.numRows - line_length);

                            for (int i = 0; i < line_length; i++)
                            {
                                TokenData[insert_row + i, insert_column] = defaultTokenId;
                            }

                            break;
                        case "horizontal":
                            line_length = UnityEngine.Random.Range(3, Constants.numColumns);

                            insert_column = UnityEngine.Random.Range(0, Constants.numColumns - line_length);
                            insert_row = UnityEngine.Random.Range(0, Constants.numRows);

                            for (int i = 0; i < line_length; i++)
                            {
                                TokenData[insert_row, insert_column + i] = defaultTokenId;
                            }

                            break;
                        case "diagonal":
                            line_length = UnityEngine.Random.Range(3, System.Math.Min(Constants.numRows, Constants.numColumns));

                            insert_column = UnityEngine.Random.Range(0, Constants.numColumns - line_length);
                            insert_row = UnityEngine.Random.Range(0, Constants.numRows - line_length);

                            for (int i = 0; i < line_length; i++)
                            {
                                TokenData[insert_row + i, insert_column + i] = defaultTokenId;
                            }

                            break;
                    }

                    break;
                case "rectangle":

                    int rect_height = UnityEngine.Random.Range(2, Constants.numRows / 2);
                    int rect_width = UnityEngine.Random.Range(2, Constants.numColumns / 2);

                    insert_column = UnityEngine.Random.Range(0, Constants.numColumns - rect_width);
                    insert_row = UnityEngine.Random.Range(0, Constants.numRows - rect_height);

                    for (int r = 0; r < rect_height; r++)
                    {
                        for (int c = 0; c < rect_width; c++)
                        {
                            TokenData[insert_row + r, insert_column + c] = defaultTokenId;
                        }
                    }

                    break;
                case "o_shape":
                    break;
                case "l_shape":
                    break;
                case "u_shape":
                    break;
                case "riverofarrows":
                    break;
                case "blockedge":
                    break;
            }

        }

        private static string GetRandomMember(Dictionary<string, int> WeightedValues)
        {
            int rnd_value = UnityEngine.Random.Range(0, 100);
            string value = "";

            foreach (string s in WeightedValues.Keys)
            {
                rnd_value -= WeightedValues[s];
                if (rnd_value <= 0)
                {
                    return s;
                }
            }
            return value;
        }

        private static int ConvertTokenNameToTokenId(string TokenName)
        {
            switch (TokenName)
            {
                case "clear":
                case "empty":
                    return 0;
                case "up_arrow":
                    return 1;
                case "down_arrow":
                    return 2;
                case "left_arrow":
                    return 3;
                case "right_arrow":
                    return 4;
                case "sticky":
                    return 5;
                case "blocker":
                    return 6;
                case "ghost":
                    return 7;
                case "ice":
                    return 8;
                case "pit":
                    return 9;
            }
            return 0;
        }

        private static string RandomTerrain()
        {
            int rnd_terrain = UnityEngine.Random.Range(0, 100);
            string terrain = "";

            foreach (string s in BoardComposition.TerrainTypes.Keys)
            {
                rnd_terrain -= BoardComposition.TerrainTypes[s];
                if (rnd_terrain <= 0)
                {
                    return s;
                }
            }
            return terrain;
        }

        private static string RandomBaseTerrain()
        {
            int rnd_terrain = UnityEngine.Random.Range(0, 100);
            string terrain = "";

            foreach (string s in BoardComposition.BaseTerrainTypes.Keys)
            {
                rnd_terrain -= BoardComposition.BaseTerrainTypes[s];
                if (rnd_terrain <= 0)
                {
                    return s;
                }
            }
            return terrain;
        }

        private static string RandomDensity()
        {
            int rnd_density = UnityEngine.Random.Range(0, 100);
            foreach (string s in BoardComposition.DensityTypes.Keys)
            {
                rnd_density -= BoardComposition.DensityTypes[s];
                if (rnd_density <= 0)
                {
                    return s;
                }
            }
            return "";
        }

        private static int GenerateRandomTerrain(string base_terrain, string overlay_terrain, string density)
        {
            int base_terrain_code = 0;
            switch (base_terrain)
            {
                case "sticky":
                    base_terrain_code = 5;
                    break;
                case "ice":
                    base_terrain_code = 8;
                    break;
            }


            int overlay_terrain_code = 0;
            switch (overlay_terrain)
            {
                case "sticky":
                    overlay_terrain_code = 5;
                    break;
                case "ice":
                    overlay_terrain_code = 8;
                    break;
            }

            int density_percentage = 0;
            switch (density)
            {
                case "light":
                    density_percentage = 10;
                    break;
                case "medium":
                    density_percentage = 50;
                    break;
                case "heavy":
                    density_percentage = 75;
                    break;
                case "filled":
                    density_percentage = 100;
                    break;

            }

            int density_poke = UnityEngine.Random.Range(0, 100);
            if (density_poke <= density_percentage) return overlay_terrain_code;
            return base_terrain_code;
        }



        static class BoardComposition
        {
            public static Dictionary<string, int> CompositionTypes = new Dictionary<string, int>()
            {
             { "uniform", 25},
             { "centric", 20},
             { "quads", 25},
             { "halfud", 5},
             { "halflr",5},
             { "vlines", 10},
             { "hlines", 10}
            };

            public static Dictionary<string, int> TerrainTypes = new Dictionary<string, int>()
            {
             { "none", 40},
             { "sticky", 40 },
             { "ice", 20}
            };

            public static Dictionary<string, int> BaseTerrainTypes = new Dictionary<string, int>()
            {
             { "none", 70},
             { "sticky", 5 },
             { "ice", 25}
            };

            public static Dictionary<string, int> DensityTypes = new Dictionary<string, int>()
            {
             { "filled", 10},
             { "light", 30 },
             { "medium", 20},
             { "heavy", 10}
            };
        }

        static class BoardFeatures
        {
            public static Dictionary<string, int> FeatureTypes = new Dictionary<string, int>()
            {
             { "line", 50},
             { "rectangle", 50},
             { "o_shape", 30 },
             { "l_shape", 30 },
             { "u_shape", 20},
             { "block_the_edge", 20},
             { "riverofarrows", 10}
            };

            public static Dictionary<string, int> LineTypes = new Dictionary<string, int>()
            {
             { "vertical", 35},
             { "horizontal", 35 },
             { "diagonal", 30}
            };

            public static Dictionary<string, int> TokenTypes = new Dictionary<string, int>()
            {
             { "ghost", 15},
             { "pit", 10 },
             { "up_arrow", 15},
             { "down_arrow", 15},
             { "left_arrow", 15},
             { "right_arrow", 15},
             { "blocker", 15}
            };
        }

        static class BoardNoise
        {
            public static Dictionary<string, int> TokenTypes = new Dictionary<string, int>()
            {
             { "clear", 20 },
             { "sticy", 10 },
             { "ice", 10 },
             { "ghost", 20},
             { "pit", 10 },
             { "left_arrow", 5},
             { "right_arrow", 5},
             { "up_arrow", 5},
             { "down_arrow", 5},
             { "blocker", 10}
            };
        }


    }
}
