using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public static class BossFactory
    {
        public static IBoss Create(BossType Type)
        {

            switch (Type) {
                case BossType.DirectionMaster:
                   return new DirectionMasterBoss();
                case BossType.EntryWay:
                    return new EntryWayBoss();

            }

            return null; 
        }
    }
}
