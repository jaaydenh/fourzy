using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class Ingredient : IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public List<TokenType> Tokens { get; }


        public PatternType Pattern { get; set; }
        public IToken TokenTemplate { get; set; }
        public bool InsertOnlyIfEmpty { get; set; }
        public bool ReplaceTokens { get; set; }
        public AddTokenMethod AddMethod { get; set; }


        public Ingredient(IToken TokenTemplate, PatternType Pattern, AddTokenMethod AddMethod= AddTokenMethod.ONLY_TERRAIN, bool ReplaceTokens = false)
        {
            this.Name = "Generic Pattern";
            this.Type = IngredientType.SMALLFEATURE;
            this.Tokens = new List<TokenType>() { TokenTemplate.Type };
            this.TokenTemplate = TokenTemplate;
            this.Pattern = Pattern;
            this.InsertOnlyIfEmpty = InsertOnlyIfEmpty;
            this.ReplaceTokens = ReplaceTokens;
            this.AddMethod = AddMethod;
        }

        public void Build(GameBoard Board)
        {
            if (TokenTemplate.Orientation == Direction.RANDOM && TokenTemplate.changePieceDirection) TokenTemplate.Orientation = Board.Random.RandomDirection();
            Board.AddToken(TokenTemplate, PatternFactory.GetLocations(Pattern,Board), AddMethod, ReplaceTokens);
        }
    }
}
