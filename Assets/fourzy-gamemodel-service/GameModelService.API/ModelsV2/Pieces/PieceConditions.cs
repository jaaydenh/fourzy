namespace FourzyGameModel.Model
{
    //CURRENT IDEAS:

    //FROZEN: will shatter if bumped
    //FIERY: on fire, will burn up in x turns
    //CURSED: will disappear in x turns
    //POISONED: will disappear in x turns
    //GIANT: bigger. Harder to be pushed
    //FLYING: Ignores terrain
    //SCARED: Won't go near ghosts and maybe some other tokens.

    public enum PieceConditionType { DEAD, FROZEN, FIERY, CURSED, POISONED, SHRUNKEN, GIANT, FLYING, STUCK, SCARED, FALLING, SINKING, WIND, NONE, INERTIA,
        STRAIGHT,
        PUSHED_UP, PUSHED_DOWN, PUSHED_RIGHT, PUSHED_LEFT
    }
}
