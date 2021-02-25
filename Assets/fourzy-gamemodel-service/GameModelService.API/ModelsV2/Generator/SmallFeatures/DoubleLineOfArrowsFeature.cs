namespace FourzyGameModel.Model
{
    public class DoubleLineOfArrowsFeature : ISmallFeature, IBoardIngredient
    {
        public string Name { get; }
        public IngredientType Type { get; }
        public TokenType Token { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public BoardLocation Insert { get; set; }
        public SmallFeatureType Feature { get { return SmallFeatureType.DOUBLELINE_OF_ARROWS; } }
        public Direction Direction { get; set; }

        public DoubleLineOfArrowsFeature(BoardLocation Insert, Direction Direction)
        {
            this.Insert = Insert;
            this.Name = "Double Line of Arrows";
            this.Type = IngredientType.SMALLFEATURE;
            this.Token = Token;
            this.Width = Width;
            this.Height = Height;
        }

        public void Build(GameBoard Board)
        {
            LineType Line = LineType.NONE;

            switch (Direction)
            {
                case Direction.UP:
                case Direction.DOWN:
                    Line = LineType.VERTICAL;
                    break;

                case Direction.LEFT:
                case Direction.RIGHT:
                    Line = LineType.HORIZONTAL;
                    break;
            }

            foreach ( BoardLocation l in new SolidFullLinePattern(Board, Insert, LineType.HORIZONTAL).Locations)
            {
                Board.AddToken(new ArrowToken(Direction), l);
            }
            
        }
    }
}
