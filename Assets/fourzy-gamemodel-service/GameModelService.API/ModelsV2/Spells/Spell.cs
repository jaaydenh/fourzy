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
        void StartOfTurn(int PlayerId);
        string Export();
        bool RequiresLocation { get; set; }
    }
}
