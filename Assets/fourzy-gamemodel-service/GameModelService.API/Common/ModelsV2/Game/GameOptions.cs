using System;
using System.Collections.Generic;
using System.Text;

namespace FourzyGameModel.Model
{
    public class GameOptions
    {
        public int Rows = Constants.DefaultRows;
        public int Columns = Constants.DefaultColumns;
        public bool MovesReduceHerd = false;

        //Maybe some other choices/settings related to game
        //string GameType = "";

        public GameOptions()
        {
        }

        public GameOptions(int Rows, int Columns)
        {
            this.Rows = Rows;
            this.Columns = Columns;
        }
    }
}
