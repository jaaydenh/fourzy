using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public interface IBoardIngredient
    {
        string Name { get; }
        IngredientType Type { get; }
        List <TokenType> Tokens {get; }
                
        void Build(GameBoard Board);
    }
}
