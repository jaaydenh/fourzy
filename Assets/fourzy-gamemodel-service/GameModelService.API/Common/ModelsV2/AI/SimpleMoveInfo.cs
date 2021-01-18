namespace FourzyGameModel.Model
{
    public class SimpleMoveInfo
    {
        public SimpleMove Move { get; set; }
        public GameState State { get; set; }
        public BoardLocation Alive { get; set; }
    }

}