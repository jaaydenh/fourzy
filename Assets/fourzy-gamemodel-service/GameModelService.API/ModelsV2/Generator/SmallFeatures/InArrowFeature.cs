using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class InArrowFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public TokenType Token { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.ARROWS_IN; } }
        public bool Solid { get; set; }
        public AddTokenMethod AddMethod {get; set; }
        public bool ReplaceTokens { get; set; }        

        public InArrowFeature(BoardLocation InsertLocation, bool Solid = false)
        {
            this.Insert = InsertLocation;
            this.Name = "In Arrow Pattern";
            this.Type = IngredientType.SMALLFEATURE;
            this.Token = TokenType.ARROW;
            this.Solid = Solid;
            this.AddMethod = AddTokenMethod.ONLY_TERRAIN;
            this.ReplaceTokens = false;
        }

        public InArrowFeature()
        {
            this.Insert = new BoardLocation(0,0);
            this.Name = "In Arrow Pattern";
            this.Type = IngredientType.SMALLFEATURE;
            this.Token = TokenType.ARROW;
            this.Solid = false;
            this.AddMethod = AddTokenMethod.ONLY_TERRAIN;
            this.ReplaceTokens = false;

        }

        public void Build(GameBoard Board)
        {
            if (Insert.Row == 0 && Insert.Column == 0)
            {
                Insert = Board.Random.RandomLocation(new BoardLocation(1, 1), Board.Rows - 2, Board.Columns - 2);
            }
            int count = 0;
            foreach (BoardLocation l in Insert.Look(Board,Direction.UP))
            {
                if (count++ % 2 == 0 || Solid)
                    Board.AddToken(new ArrowToken(Direction.DOWN), l, AddMethod, ReplaceTokens);
            }
            count = 0;
            foreach (BoardLocation l in Insert.Look(Board, Direction.DOWN))
            {
                if (count++ % 2 == 0 || Solid)
                    Board.AddToken(new ArrowToken(Direction.UP), l, AddMethod, ReplaceTokens);
            }
            count = 0;
            foreach (BoardLocation l in Insert.Look(Board, Direction.LEFT))
            {
                if (count++ % 2 == 0 || Solid)
                    Board.AddToken(new ArrowToken(Direction.RIGHT), l, AddMethod, ReplaceTokens);
            }
            count = 0;
            foreach (BoardLocation l in Insert.Look(Board, Direction.RIGHT))
            {
                if (count++ % 2 == 0 || Solid)

                    Board.AddToken(new ArrowToken(Direction.LEFT), l, AddMethod, ReplaceTokens);
            }


        }
    }
}
