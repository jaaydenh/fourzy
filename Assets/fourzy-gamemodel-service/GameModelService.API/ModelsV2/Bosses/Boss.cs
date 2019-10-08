using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public interface IBoss
    {
        string Name { get; }
        List <IBossPower> Powers { get; }
        bool UseCustomAI { get;  }

        bool StartGame(GameState State);

        //Use if Boss has a particular timing to use powers.
        bool TriggerPower(GameState State);

        //If a Boss has a special condition
        bool TriggerBossWin(GameState State);
        bool TriggerBossLoss(GameState State);
        
        List<IMove> GetPossibleActivations(GameState State, bool IsDesparate);
        PlayerTurn GetTurn(GameState State);
    }
}
