﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public abstract class BoardGenerator
    {
        public RandomTools Random { get; set; }
        public List<TokenType> AllowedTokens {get; set;}

        public string Name = "Base Board Generator";


        public virtual int MinNoise { get { return 0; } }
        public virtual int MaxNoise { get { return 10; } }
        public virtual Dictionary<string, int> NoiseTokens { get; set; }
        public virtual string base_terrain { get { return "empty"; } }
        public virtual Dictionary<string, int> TerrainTypes { get; set; }
     
        public static Dictionary<string, int> LineTypes = new Dictionary<string, int>()
            {
             { "vertical", 34},
             { "horizontal", 34 },
             { "diagonal", 35}
        };

        public virtual string RequestedRecipeName { get; set; }
        public virtual Dictionary<BoardRecipe, int> Recipes { get; set; }
        public virtual Area Area { get { return Area.NONE; } }

        public virtual void Initialize()
        {

        }

        public virtual GameBoard GenerateBoard(int Rows, int Columns, string SeedString = "")
        {
            GameBoard NewBoard = null;
            int count = 0;
            NewBoard = BuildBoard(Rows, Columns, SeedString);

            while (!BoardGeneratorTester.TestGeneratedBoard(NewBoard) && count++ < 100)
                NewBoard = BuildBoard(Rows, Columns, SeedString);
            
            return NewBoard;
        }

        public GameBoard BuildBoard(int Rows, int Columns, string SeedString)
        {
            GameBoard NewBoard = BoardFactory.CreateDefaultBoard(Rows, Columns);
            NewBoard.Random = new RandomTools(SeedString);
            BoardRecipe CurrentRecipe = null;

            if (Recipes == null) return null;
            if (Recipes.Count == 0) return null;

            if (RequestedRecipeName.Length > 0)
            {
                foreach (BoardRecipe r in Recipes.Keys)
                {
                    if (r.Name == RequestedRecipeName) CurrentRecipe = r;
                }
            }

            if (CurrentRecipe == null) CurrentRecipe = NewBoard.Random.RandomWeightedRecipe(Recipes);

            foreach (IBoardIngredient Ingredient in CurrentRecipe.Ingredients)
            {
                NewBoard = ApplyIngredient(NewBoard, Ingredient);
            }

            NewBoard.Area = Area;
            return NewBoard;
        }


            public virtual GameBoard ApplyIngredient(GameBoard Board, IBoardIngredient Ingredient)
            {
                Ingredient.Build(Board);
                return Board;
            }

        }
    }
