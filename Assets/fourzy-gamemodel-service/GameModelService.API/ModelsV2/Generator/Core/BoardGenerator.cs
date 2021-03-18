using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public abstract class BoardGenerator
    {
        public abstract Area Area { get; } 
        public abstract string Name {  get;  }

        //Each generator will have complexities in a defined range. 
        public abstract int MinComplexity { get; }
        public abstract int MaxComplexity { get; }

        //These are used to determine which recipes to use.
        protected GameOptions Options;
        protected BoardGenerationPreferences Preferences;

        //Concept of noise no longer being used at this level.

        //public virtual int MinNoise { get { return 0; } }
        //public virtual int MaxNoise { get { return 10; } }
        //public virtual Dictionary<string, int> NoiseTokens { get; set; }
        //public virtual string BaseTerrain { get { return "empty"; } }

        ////Need to review this section. Going to comment out for now.
        //public virtual Dictionary<string, int> TerrainTypes { get; set; }
        //public static Dictionary<string, int> LineTypes = new Dictionary<string, int>()
        //    {
        //     { "vertical", 34},
        //     { "horizontal", 34 },
        //     { "diagonal", 35}
        //};
        //public List<TokenType> AllowedTokens { get; set; }


        //REVIEW
        //Should be converted to Get Function.  Allow Random is a flag to determine whether we want to include random elements in the creation
        public bool AllowRandom { get { return Preferences.IncludesRandomTokens; } }

        //A random object is created with a seed to generate a board
        public RandomTools Random { get; set; }
        public string SeedString { get; set; }

        //A list of the recipes used by the generator and weight for probability.
        public Dictionary<BoardRecipe, int> Recipes { get; set; }
    
        public virtual void Initialize()
        {

        }

        //public virtual GameBoard GenerateBoard(int Rows, int Columns, string SeedString = "", int DesiredMinComplexity = -1, int DesiredMaxComplexity = -1)
              
        public virtual GameBoard GenerateBoard()
        {
            GameBoard NewBoard = null;
            int count = 0;
            NewBoard = BuildBoard();
            BoardGeneratorTester Tester = new BoardGeneratorTester();

            while (!Tester.TestGeneratedBoard(NewBoard) && count++ < BoardGeneratorConstants.MAX_GENERATOR_ATTEMPTS)
                NewBoard = BuildBoard();

            return NewBoard;
        }

        public virtual GameBoard BuildBoard()
        {
            GameBoard NewBoard = BoardFactory.CreateDefaultBoard(Options.Rows, Options.Columns);
            NewBoard.Random = new RandomTools(SeedString);
            BoardRecipe CurrentRecipe = null;

            if (Recipes == null) return null;
            if (Recipes.Count == 0) return null;

            string RequestedRecipeName = "";
            if (RequestedRecipeName.Length == 0 && Preferences.RequestedRecipe.Length > 0) RequestedRecipeName = Preferences.RequestedRecipe;
            if (RequestedRecipeName.Length > 0)
            {
                foreach (BoardRecipe r in Recipes.Keys)
                {
                    if (r.Name == RequestedRecipeName) CurrentRecipe = r;
                }
            }

            //No recipe found. Let's pick one.
            //Use information from Board Preferences.
            if (CurrentRecipe == null)
            {
                //1. Eliminate bad recipes.
                foreach(BoardRecipe r in Recipes.Keys)
                {
                    if (
                        (Preferences.TargetComplexityLow > 0 && r.ComplexityLowThreshold > 0 && r.ComplexityLowThreshold > Preferences.TargetComplexityHigh)
                        || (Preferences.TargetComplexityHigh > 0 && r.ComplexityHighThreshold > 0 && r.ComplexityHighThreshold < Preferences.TargetComplexityLow)
                        || (!Preferences.IncludesRandomTokens && r.ContainsRandom)
                        ) { 
                        Recipes.Remove(r); 
                        continue; }

                    if (Preferences.AllowedTokens.Count >0)
                    foreach (TokenType t in r.Tokens)
                    {
                        if (!Preferences.AllowedTokens.Contains(t)) { Recipes.Remove(r); continue; }
                    }

                    if (Preferences.ForbiddenTokens.Count > 0)
                        foreach (TokenType t in r.Tokens)
                        {
                            if (Preferences.ForbiddenTokens.Contains(t)) { Recipes.Remove(r); continue; }
                        }
                }
   
                CurrentRecipe = NewBoard.Random.RandomWeightedRecipe(Recipes);
            }

            foreach (IBoardIngredient Ingredient in CurrentRecipe.Ingredients)
            {
                try
                {
                    NewBoard = ApplyIngredient(NewBoard, Ingredient);
                }
                catch (Exception ex){
                    string TestMessage = ex.Message;
                }
            }

            NewBoard.Area = Area;
            return NewBoard;
        }

        public virtual List<BoardRecipe> GetRecipesWithTokenType(TokenType Type)
        {
            List<BoardRecipe> RecipeList = new List<BoardRecipe>();
            foreach (BoardRecipe r in Recipes.Keys)
            {
                if (r.Tokens.Contains(Type)) RecipeList.Add(r);
            }

            return RecipeList;
        }

        public virtual List<BoardRecipe> GetRecipesInComplexityRange(int Low, int High)
        {
            List<BoardRecipe> RecipeList = new List<BoardRecipe>();
            foreach (BoardRecipe r in Recipes.Keys)
            {
                if (
                      (Low > 0 && r.ComplexityLowThreshold > 0 && r.ComplexityLowThreshold > High)
                      || (High > 0 && r.ComplexityHighThreshold > 0 && r.ComplexityHighThreshold < Low)
                      ) continue;
                
                RecipeList.Add(r);
            }

            return RecipeList;
        }

        public virtual List<BoardRecipe> GetRecipes(List<TokenType> AllowedTokens)
        {
            List<BoardRecipe> RecipeList = new List<BoardRecipe>();

            foreach (BoardRecipe r in Recipes.Keys)
            {
                bool allowed = true;
                foreach (TokenType t in r.Tokens)
                {
                    if (!AllowedTokens.Contains(t)) { allowed = false;  }
                }
                if (allowed) RecipeList.Add(r);
            }

            return RecipeList;
        }

        public virtual List<BoardRecipe> GetRecipes(BoardGenerationPreferences Preferences)
        {
            List<BoardRecipe> RecipeList = new List<BoardRecipe>();

            foreach (BoardRecipe r in Recipes.Keys)
            {
                if (
                    (Preferences.TargetComplexityLow > 0 && r.ComplexityLowThreshold > 0 && r.ComplexityLowThreshold > Preferences.TargetComplexityHigh)
                    || (Preferences.TargetComplexityHigh > 0 && r.ComplexityHighThreshold > 0 && r.ComplexityHighThreshold < Preferences.TargetComplexityLow)
                    || (!Preferences.IncludesRandomTokens && r.ContainsRandom)
                    )
                {
                    continue;
                }

                if (Preferences.AllowedTokens.Count > 0)
                    foreach (TokenType t in r.Tokens)
                    {
                        if (!Preferences.AllowedTokens.Contains(t)) {  continue; }
                    }

                if (Preferences.ForbiddenTokens.Count > 0)
                    foreach (TokenType t in r.Tokens)
                    {
                        if (Preferences.ForbiddenTokens.Contains(t)) {  continue; }
                    }

                RecipeList.Add(r);
            }

            return RecipeList;
        }

        public virtual GameBoard ApplyIngredient(GameBoard Board, IBoardIngredient Ingredient)
            {
                Ingredient.Build(Board);
                return Board;
            }

        }
    }
