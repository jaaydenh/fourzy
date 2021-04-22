using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{ 
    public class BoardGenerationPreferences
    {
        //Set one of the following.
        //public int DesiredComplexityPercentage = -1;
        public int TargetComplexityLow = -1;
        public int TargetComplexityHigh = -1;

        //It might be useful to put requested recipe here, instead of a constructor
        public string RequestedRecipe = "";
        public string RecipeSeed = "";

        public List<TokenType> AllowedTokens = new List<TokenType>() { };
        public List<TokenType> ForbiddenTokens = new List<TokenType>() { };
        public List<TokenType> RequiredTokens = new List<TokenType>() { };

        public bool IncludesRandomTokens = true;
        public bool IncludesDynamicTokens = true;
        public BoardGenerationPreferences(Area TargetArea = Area.NONE, int Percentage = -1)
        {
            if (Percentage >= 0)
            {
                Tuple<int, int> range = BoardFactory.NormalizeComplexity(TargetArea, Percentage);
                this.TargetComplexityLow = range.Item1;
                this.TargetComplexityHigh = range.Item2;

            }
        }
    }
}
