using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public interface ISpell 
    {
        int PlayerId { get; }
        string Name { get;  }
        SpellType SpellType { get;  }
        SpellId SpellId { get;  }
        int Cost { get; set; }
        bool Cast(GameState State);
        bool RequiresLocation { get; set; }
        List<BoardLocation> GetValidSpellLocations(GameBoard Board);
    }
}
