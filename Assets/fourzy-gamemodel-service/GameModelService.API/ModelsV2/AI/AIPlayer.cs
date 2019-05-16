using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public interface AIPlayer
    {
        PlayerTurn GetTurn();
    }
}
