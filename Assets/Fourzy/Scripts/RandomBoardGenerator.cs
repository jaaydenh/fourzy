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

            AddFeatures(ref newBoard.tokenBoard, 3, 6);
            AddNoise(ref newBoard.tokenBoard, 3, 7);
            SetCorners(ref newBoard.tokenBoard);
            CheckArrows(ref newBoard.tokenBoard);
            CheckPits(ref newBoard.tokenBoard);
            CheckMaxBlockers(ref newBoard.tokenBoard, 6);

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

        private static void CheckMaxBlockers(ref int[,] TokenData, int MaxBlockers)
        {
            //count up any blockers not on the edge.
            List<Position> BlockerList = new List<Position>();
            for (int r = 1; r < Constants.numRows - 1; r++)
            {
                for (int c = 1; c < Constants.numColumns - 1; c++)
                {
                    if (IsBlockerToken(TokenData[r, c]))
                    {
                        BlockerList.Add(new Position(c, r));
                    }
                }
            }
            if (BlockerList.Count <= MaxBlockers) return;

            //Convert a random blocker to a random space.
            while (BlockerList.Count > MaxBlockers)
            {
                int choose_one = UnityEngine.Random.Range(0, BlockerList.Count - 1);
                Position pos = BlockerList[choose_one];
                TokenData[pos.row, pos.column] = (int)Token.EMPTY;
                BlockerList.RemoveAt(choose_one);
            }

        }

        private static void CheckPits(ref int[,] TokenData)
        {
            for (int r = 1; r < Constants.numRows - 1; r++)
            {
                if (TokenData[r, 0] == (int)Token.PIT)
                {
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
            string defaultFeatureTokenName = GetRandomMember(BoardFeatures.FeatureTokens[feature]);
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

                case "steps":

                    int steps_height = UnityEngine.Random.Range(2, Constants.numRows / 2);


                    insert_column = UnityEngine.Random.Range(0, Constants.numColumns - steps_height);
                    insert_row = UnityEngine.Random.Range(0, Constants.numRows - steps_height);

                    for (int r = 0; r < steps_height; r++)
                    {
                        for (int c = 0; c < steps_height; c++)
                        {
                            if (c <= r) TokenData[insert_row + r, insert_column + c] = defaultTokenId;
                        }
                    }

                    break;


                case "smiley":

                    int smile_spacing = UnityEngine.Random.Range(1, 2);
                    int smile_height = smile_spacing + 3;
                    int smile_width = smile_spacing + 4;

                    insert_column = UnityEngine.Random.Range(0, Constants.numColumns - smile_height);
                    insert_row = UnityEngine.Random.Range(0, Constants.numRows - smile_width);

                    TokenData[insert_row, insert_column + 1] = defaultTokenId;
                    TokenData[insert_row, (insert_column + smile_width - 1)] = defaultTokenId;
                    TokenData[(insert_row + smile_height - 1), insert_column] = defaultTokenId;
                    TokenData[(insert_row + smile_height - 1), (insert_column + smile_width)] = defaultTokenId;

                    for (int c = 0; c < smile_width; c++)
                    {
                        TokenData[insert_row + smile_height, insert_column + c] = defaultTokenId;
                    }

                    break;

                case "vdoubleline":

                    insert_column = UnityEngine.Random.Range(2, Constants.numColumns - 2);

                    for (int r = 0; r < Constants.numRows; r++)
                    {
                        TokenData[r, insert_column] = defaultTokenId;
                        TokenData[r, insert_column + 1] = defaultTokenId;
                    }

                    break;

                case "hdoubleline":

                    insert_row = UnityEngine.Random.Range(2, Constants.numRows - 2);

                    for (int c = 0; c < Constants.numColumns; c++)
                    {
                        TokenData[insert_row, c] = defaultTokenId;
                        TokenData[insert_row + 1, c] = defaultTokenId;
                    }
                    break;



                case "letter":

                    int letter_size = 5;
                    int[,] letter_definition = LetterDefinitions.GetRandomLetterDefinition();
                    insert_column = UnityEngine.Random.Range(0, Constants.numColumns - letter_size);
                    insert_row = UnityEngine.Random.Range(0, Constants.numRows - letter_size);

                    for (int r = 0; r < letter_size; r++)
                    {
                        for (int c = 0; c < letter_size; c++)
                        {
                            if (letter_definition[r, c] == 1) TokenData[insert_row + r, insert_column + c] = defaultTokenId;
                        }
                    }

                    break;

                case "drops":

                    int drops_height = UnityEngine.Random.Range(4, Constants.numRows - 2);
                    int drops_width = UnityEngine.Random.Range(4, Constants.numColumns - 2);

                    insert_column = UnityEngine.Random.Range(0, Constants.numColumns - drops_width);
                    insert_row = UnityEngine.Random.Range(0, Constants.numRows - drops_height);

                    int count = drops_height + drops_width;
                    while (count > 0)
                    {
                        for (int r = 0; r < drops_height; r++)
                        {
                            for (int c = 0; c < drops_width; c++)
                            {
                                if (UnityEngine.Random.Range(1, drops_height + drops_width) <= 1)
                                {
                                    TokenData[insert_row + r, insert_column + c] = defaultTokenId;
                                    count--;
                                }

                                if (count == 0) break;
                            }

                            if (count == 0) break;
                        }
                    }

                    break;

                case "dotsquare":

                    int dotsquare_height = UnityEngine.Random.Range(3, Constants.numRows - 2);
                    int dotsquare_width = UnityEngine.Random.Range(3, Constants.numColumns - 2);

                    insert_column = UnityEngine.Random.Range(0, Constants.numColumns - dotsquare_width);
                    insert_row = UnityEngine.Random.Range(0, Constants.numRows - dotsquare_height);

                    TokenData[insert_row, insert_column] = defaultTokenId;
                    TokenData[insert_row, insert_column + dotsquare_width] = defaultTokenId;
                    TokenData[insert_row + dotsquare_height, insert_column] = defaultTokenId;
                    TokenData[insert_row + dotsquare_height, insert_column + dotsquare_width] = defaultTokenId;

                    break;


                case "cornerbrackets":

                    int cornerbracket_spacing = UnityEngine.Random.Range(1, 2);
                    int cornerbracket_height = 4 + cornerbracket_spacing;
                    int cornerbracket_width = 4 + cornerbracket_spacing;

                    insert_column = UnityEngine.Random.Range(0, Constants.numColumns - cornerbracket_width);
                    insert_row = UnityEngine.Random.Range(0, Constants.numRows - cornerbracket_height);

                    TokenData[insert_row, insert_column] = defaultTokenId;
                    TokenData[insert_row + 1, insert_column] = defaultTokenId;
                    TokenData[insert_row, insert_column + 1] = defaultTokenId;

                    TokenData[insert_row, insert_column + cornerbracket_width] = defaultTokenId;
                    TokenData[insert_row, insert_column + cornerbracket_width - 1] = defaultTokenId;
                    TokenData[insert_row + 1, insert_column + cornerbracket_width] = defaultTokenId;

                    TokenData[insert_row + cornerbracket_height, insert_column] = defaultTokenId;
                    TokenData[insert_row + cornerbracket_height, insert_column + 1] = defaultTokenId;
                    TokenData[insert_row + cornerbracket_height - 1, insert_column] = defaultTokenId;

                    TokenData[insert_row + cornerbracket_height, insert_column + cornerbracket_width] = defaultTokenId;
                    TokenData[insert_row - 1 + cornerbracket_height, insert_column + cornerbracket_width] = defaultTokenId;
                    TokenData[insert_row + cornerbracket_height, insert_column + cornerbracket_width - 1] = defaultTokenId;


                    break;

                case "o_shape":
                    int o_height = UnityEngine.Random.Range(3, 6);
                    int o_width = UnityEngine.Random.Range(3, 6);

                    insert_column = UnityEngine.Random.Range(0, Constants.numColumns - o_width);
                    insert_row = UnityEngine.Random.Range(0, Constants.numRows - o_height);

                    for (int r = 0; r < o_height; r++)
                    {
                        for (int c = 0; c < o_width; c++)
                        {
                            if (c == 0 || r == 0 || c == o_width - 1 || r == o_height - 1) TokenData[insert_row + r, insert_column + c] = defaultTokenId;
                        }
                    }

                    break;
                case "l_shape":
                    int l_height = UnityEngine.Random.Range(2, 6);
                    int l_width = UnityEngine.Random.Range(2, 6);

                    insert_column = UnityEngine.Random.Range(0, Constants.numColumns - l_width);
                    insert_row = UnityEngine.Random.Range(0, Constants.numRows - l_height);

                    for (int r = 0; r < l_height; r++)
                    {
                        for (int c = 0; c < l_width; c++)
                        {
                            if (c == 0 || r == l_height - 1) TokenData[insert_row + r, insert_column + c] = defaultTokenId;
                        }
                    }
                    break;

                case "u_shape":

                    int u_height = UnityEngine.Random.Range(3, 5);
                    int u_width = UnityEngine.Random.Range(3, 5);

                    insert_column = UnityEngine.Random.Range(0, Constants.numColumns - u_width);
                    insert_row = UnityEngine.Random.Range(0, Constants.numRows - u_height);

                    for (int r = 0; r < u_height; r++)
                    {
                        for (int c = 0; c < u_width; c++)
                        {
                            if (c == 0 || c == u_width - 1 || r == u_height - 1) TokenData[insert_row + r, insert_column + c] = defaultTokenId;
                        }
                    }
                    break;
                case "checkers":

                    int check_height = UnityEngine.Random.Range(3, 6);
                    int check_width = UnityEngine.Random.Range(3, 6);

                    insert_column = UnityEngine.Random.Range(0, Constants.numColumns - check_width);
                    insert_row = UnityEngine.Random.Range(0, Constants.numRows - check_height);

                    for (int r = 0; r < check_height; r++)
                    {
                        for (int c = 0; c < check_width; c++)
                        {
                            if ((c + r) % 2 == 0) TokenData[insert_row + r, insert_column + c] = defaultTokenId;
                        }
                    }

                    break;

                case "cross":

                    int cross_width = UnityEngine.Random.Range(3, 6);
                    int cross_center = (int)(cross_width / 2);

                    insert_column = UnityEngine.Random.Range(0, Constants.numColumns - cross_width);
                    insert_row = UnityEngine.Random.Range(0, Constants.numRows - cross_width);

                    for (int r = 0; r < cross_width; r++)
                    {
                        for (int c = 0; c < cross_width; c++)
                        {
                            if (c == cross_center || r == cross_center) TokenData[insert_row + r, insert_column + c] = defaultTokenId;
                        }
                    }
                    break;

                case "corners":

                    for (int r = 0; r < Constants.numRows; r++)
                    {
                        for (int c = 0; c < Constants.numColumns; c++)
                        {
                            if ((r < 2 && c < 2) || (r > Constants.numRows - 3 && c > Constants.numColumns - 3) || (r > Constants.numRows - 3 && c < 2) || (r < 2 && c > Constants.numColumns - 3)) TokenData[insert_row + r, insert_column + c] = defaultTokenId;
                        }
                    }
                    break;


                case "diagcross":

                    int diagcross_width = UnityEngine.Random.Range(3, 6);
                    int diagcross_center = (int)(diagcross_width / 2);

                    insert_column = UnityEngine.Random.Range(0, Constants.numColumns - diagcross_width);
                    insert_row = UnityEngine.Random.Range(0, Constants.numRows - diagcross_width);

                    for (int r = 0; r < diagcross_width; r++)
                    {
                        for (int c = 0; c < diagcross_width; c++)
                        {
                            if (r == c || r + c == (diagcross_width - 1)) TokenData[insert_row + r, insert_column + c] = defaultTokenId;
                        }
                    }
                    break;

                case "diaglines":

                    for (int r = 0; r < Constants.numRows; r++)
                    {
                        for (int c = 0; c < Constants.numColumns; c++)
                        {
                            if ((r + c) % 3 == 1) TokenData[insert_row + r, insert_column + c] = defaultTokenId;
                        }
                    }
                    break;

                case "blockedge":

                    int direction = UnityEngine.Random.Range(0, 3);
                    switch (direction)
                    {
                        //top
                        case 0:
                            for (int c = 0; c < Constants.numColumns; c++)
                            {
                                TokenData[0, c] = defaultTokenId;
                            }
                            break;
                        //bottom
                        case 1:
                            for (int c = 0; c < Constants.numColumns; c++)
                            {
                                TokenData[Constants.numRows - 1, c] = defaultTokenId;
                            }

                            break;

                        //left
                        case 2:

                            for (int r = 0; r < Constants.numRows; r++)
                            {
                                TokenData[r, 0] = defaultTokenId;
                            }


                            break;

                        //right
                        case 3:
                            for (int r = 0; r < Constants.numRows; r++)
                            {
                                TokenData[r, Constants.numColumns - 1] = defaultTokenId;
                            }

                            break;
                    }
                    break;
            }

        }

        private static string GetRandomMember(Dictionary<string, int> WeightedValues)
        {
            int total_weight = 0;
            foreach (string s in WeightedValues.Keys)
            {
                total_weight += WeightedValues[s];
            }

            int rnd_value = UnityEngine.Random.Range(0, total_weight);
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

        private static bool IsBlockerToken(int EvalToken)
        {
            switch (EvalToken)
            {
                case (int)Token.BLOCKER:
                case (int)Token.BUMPER:
                case (int)Token.GHOST:
                    {
                        return true;
                    }
            }
            return false;
        }

        private static int ConvertTokenNameToTokenId(string TokenName)
        {
            switch (TokenName)
            {
                case "clear":
                case "empty":
                    return (int)Token.EMPTY;
                case "up_arrow":
                    return (int)Token.UP_ARROW;
                case "down_arrow":
                    return (int)Token.DOWN_ARROW;
                case "left_arrow":
                    return (int)Token.LEFT_ARROW;
                case "right_arrow":
                    return (int)Token.RIGHT_ARROW;
                case "sticky":
                    return (int)Token.STICKY;
                case "blocker":
                    return (int)Token.BLOCKER;
                case "ghost":
                    return (int)Token.GHOST;
                case "ice":
                    return (int)Token.ICE_SHEET;
                case "pit":
                    return (int)Token.PIT;
                case "sand":
                    return (int)Token.SAND;
                case "water":
                    return (int)Token.WATER;
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
                    base_terrain_code = (int)Token.STICKY;
                    break;
                case "ice":
                    base_terrain_code = (int)Token.ICE_SHEET;
                    break;
                case "sand":
                    base_terrain_code = (int)Token.SAND;
                    break;
                case "water":
                    base_terrain_code = (int)Token.WATER;
                    break;
            }


            int overlay_terrain_code = 0;
            switch (overlay_terrain)
            {
                case "sticky":
                    overlay_terrain_code = (int)Token.STICKY;
                    break;
                case "ice":
                    overlay_terrain_code = (int)Token.ICE_SHEET;
                    break;
                case "sand":
                    overlay_terrain_code = (int)Token.SAND;
                    break;
                case "water":
                    overlay_terrain_code = (int)Token.WATER;
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
             { "uniform", 30},
             { "centric", 30},
             { "quads", 10},
             { "halfud", 10},
             { "halflr",10},
             { "vlines", 5},
             { "hlines", 5}
            };

            public static Dictionary<string, int> TerrainTypes = new Dictionary<string, int>()
            {
             { "none", 20},
             { "sticky", 30 },
             { "ice", 25},
             { "sand", 25},
             { "water", 10}
            };

            public static Dictionary<string, int> BaseTerrainTypes = new Dictionary<string, int>()
            {
             { "none", 40},
             { "sticky", 10 },
             { "ice", 25},
             { "sand", 25}
            };

            public static Dictionary<string, int> DensityTypes = new Dictionary<string, int>()
            {
             { "filled", 45},
             { "light", 7 },
             { "medium", 3},
             { "heavy", 45}
            };
        }


        static class BoardFeatures
        {


            public static Dictionary<string, int> LineTypes = new Dictionary<string, int>()
            {
             { "vertical", 34},
             { "horizontal", 34 },
             { "diagonal", 35}
            };

            public static Dictionary<string, int> TokenTypes = new Dictionary<string, int>()
            {
             { "ghost", 15},
             { "pit", 15 },
             { "up_arrow", 10},
             { "down_arrow", 10},
             { "left_arrow", 10},
             { "right_arrow", 10},
             { "blocker", 15},
             { "sticky", 15}
            };

            public static Dictionary<string, int> LargeShapeTokenTypes = new Dictionary<string, int>()
            {
                { "pit", 15 },
                { "sticky", 15},
                { "ice", 15},
                { "sand", 15}
            };

            public static Dictionary<string, int> SmallShapeTokenTypes = new Dictionary<string, int>()
            {
                { "pit", 15 },
                { "sticky", 15},
                { "ice", 15},
                { "water", 15}
            };

            public static Dictionary<string, int> BlockEdge = new Dictionary<string, int>()
            {
                { "pit", 5 },
                { "blocker", 20},
                { "up_arrow", 5},
                { "down_arrow", 5},
                { "left_arrow", 5},
                { "right_arrow", 5}
            };

            public static Dictionary<string, int> CornerTokens = new Dictionary<string, int>()
            {
                { "sticky", 15},
                { "ice", 15}
            };


            public static Dictionary<string, int> FeatureTypes = new Dictionary<string, int>()
            {
             { "line", 200},
             { "rectangle", 50},
             { "o_shape", 50 },
             { "l_shape", 50 },
             { "u_shape", 10 },
             { "checkers", 50 },
             { "cross", 200},
             { "diagcross", 200},
             { "drops", 50},
             { "steps", 50},
             { "dotsquare", 300},
             { "letter", 50},
             { "smiley", 0}, //smiley is broken...  
             { "vdoubleline", 100},
             { "hdoubleline", 100},
             { "diaglines", 25},
             { "blockedge", 25},
             { "corners", 50},
             { "cornerbrackets", 100},
             { "riverofarrows", 0}, //not implemented
             { "cycle", 0}, //not implemented
             { "faceoff", 0} //not implemented
            };

            public static Dictionary<string, Dictionary<string, int>> FeatureTokens = new Dictionary<string, Dictionary<string, int>>() {
                { "line", TokenTypes },
                { "drops", SmallShapeTokenTypes },
                { "rectangle", TokenTypes },
                { "o_shape", SmallShapeTokenTypes },
                { "l_shape", TokenTypes },
                { "u_shape", TokenTypes  },
                { "checkers", TokenTypes },
                { "cross", SmallShapeTokenTypes },
                { "diagcross", TokenTypes },
                { "dotsquare", TokenTypes },
                { "smiley", TokenTypes },
                { "steps", LargeShapeTokenTypes },
                { "hdoubleline", LargeShapeTokenTypes },
                { "vdoubleline", LargeShapeTokenTypes },
                { "diaglines", LargeShapeTokenTypes },
                { "letter", TokenTypes },
                { "blockedge", TokenTypes },
                { "corners", CornerTokens},
                { "cornerbrackets", SmallShapeTokenTypes},
                { "riverofarrows", TokenTypes },
                { "cycle", TokenTypes },
                { "faceoff", TokenTypes }
            };


        }

        static class BoardNoise
        {
            public static Dictionary<string, int> TokenTypes = new Dictionary<string, int>()
            {
             { "clear", 20 },
             { "sticky", 10 },
             { "ice", 10 },
             { "ghost", 20},
             { "pit", 10 },
             { "left_arrow", 5},
             { "right_arrow", 5},
             { "up_arrow", 5},
             { "down_arrow", 5},
             { "blocker", 10},
             { "water", 10}
            };
        }

    }
}
