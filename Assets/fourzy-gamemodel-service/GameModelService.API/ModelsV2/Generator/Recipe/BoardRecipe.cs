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



        //covert this to code instead of maintaining list.
        public List<TokenType> Tokens { get
            {
                List<TokenType> tokens = new List<TokenType>();
                foreach (IBoardIngredient i in Ingredients)
                {
                    foreach (TokenType t in i.Tokens)
                    if (!tokens.Contains(t)) tokens.Add(t);
                }
                return tokens;
            }
        }

        //public PatternComplexity Complexity { get; set; }
        // Change to upper and lower bounds.  This will be arbritary, but will trigger use of recipe

        public int ComplexityHighThreshold { get; set; }
        public int ComplexityLowThreshold { get; set; }

        //It might be worth converting this to determine based on incredients
        //For now, we may need to flag them manually.
        public bool ContainsRandom { get; set; }

        public BoardRecipe(string Name, int ComplexityLowThreshold=-1, int ComplexityHighThreshold =-1)
        {
            this.Name = Name;
            this.Ingredients = new List<IBoardIngredient>();
            //this.Tokens = new List<TokenType>();
            this.ContainsRandom = false;
            this.ComplexityHighThreshold = ComplexityHighThreshold;
            this.ComplexityLowThreshold = ComplexityLowThreshold;
        }

        public void Build(GameBoard Board)
        {
            foreach (IBoardIngredient i in Ingredients)
            {
                i.Build(Board);
            }
        }

        public void AddIngredient(IBoardIngredient NewIngredient)
        {
            Ingredients.Add(NewIngredient);
            //Tokens.Add(NewIngredient.Token);
        }
    }
}
