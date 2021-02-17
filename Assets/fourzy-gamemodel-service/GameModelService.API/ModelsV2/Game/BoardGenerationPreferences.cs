using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{ 
    public class BoardGenerationPreferences
    {
        public int DesiredComplexityPercentage = -1;
        public List<TokenType> AllowedTokens = new List<TokenType>() { };
        public List<TokenType> ForbiddenTokens = new List<TokenType>() { };
        public bool IncludesRandomTokens = true;
        public bool IncludesDynamicTokens = true;
    }
}
