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
        TokenType Token {get; }
                
        void Build(GameBoard Board);
    }
}
