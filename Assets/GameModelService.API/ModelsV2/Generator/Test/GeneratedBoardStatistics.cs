using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class GeneratedBoardStatistics
    {
        public int MaxComplexity { get; set; }
        public int MinComplexity { get; set; }
        public int MedianComplexity { get; set; }
        public int AverageComplexity { get; set; }
        public int StandardDeviation { get; set; }
        public int UniqueBoards { get; set; }
        public List<TokenType> TokenTypes { get; set; }

        public GeneratedBoardStatistics()
        {
            this.MaxComplexity = -1;
            this.MinComplexity = -1;
            this.MedianComplexity = -1;
            this.AverageComplexity = -1;
            this.UniqueBoards = -1;
            this.TokenTypes = new List<TokenType>() { };
        }

        public string toString()
        {
            return string.Format("min:{0},max:{1},med:{2},avg:{3},dev:{4},2devlow:{5},2devhigh:{6},unique:{7}", MinComplexity, MaxComplexity, MedianComplexity, AverageComplexity, StandardDeviation, AverageComplexity-2*StandardDeviation, AverageComplexity + 2 * StandardDeviation, UniqueBoards);
        }
    }
}
