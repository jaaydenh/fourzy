using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public interface IBossPower
    {
        string Name { get; }
        BossPowerType PowerType{get; }

        bool Activate(GameState State);
        bool IsAvailable(GameState State, bool IsDeparate);

        List<IMove> GetPossibleActivations(GameState State, bool IsDesparate);
    }
}
