using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    //A Recipe will be a list of instruction to build a particular board.
    // It may have some variations
    public class BoardRecipe
    {
        public string Name { get; set; }
        public List<IBoardIngredient> Ingredients { get; set; }
        public List<TokenType> Tokens { get; set; }
        public PatternComplexity Complexity { get; set; }

        public BoardRecipe(string Name, PatternComplexity Complexity = PatternComplexity.None)
        {
            this.Name = Name;
            this.Ingredients = new List<IBoardIngredient>();
            this.Complexity = Complexity;
            this.Tokens = new List<TokenType>();
        }

        public void Build(GameBoard Board)
        {
            foreach (IBoardIngredient i in Ingredients)
            {
                i.Build(Board);
            }
        }

    }
}
