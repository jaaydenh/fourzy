namespace FourzyGameModel.Model
{
    public class TokenRegion
    {
        public BoardLocation Origin { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public IToken Token {get; set; }

        public TokenRegion(IToken Token, BoardLocation Origin, int Width, int Height)
        {
            this.Token = Token;
            this.Origin = Origin;
            this.Width = Width;
            this.Height = Height;
        }
    }
}
