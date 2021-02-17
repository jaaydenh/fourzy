namespace FourzyGameModel.Model
{
    public enum Area
    {
        NONE = 1,
        TRAINING_GARDEN = 2,
        ENCHANTED_FOREST = 4,
        SANDY_ISLAND = 8,
        ICE_PALACE = 16,
        CASTLE = 32,
        ARENA = 64
    }
    public static class AreaConstants
    {
        public static Area DefaultArea = Area.TRAINING_GARDEN;

        public const int TRAINING_GARDEN_INITIAL_MOVE_TIME = 30;
        public const int TRAINING_GARDEN_ADDITIONAL_MOVE_TIME = 10;
    }
}
