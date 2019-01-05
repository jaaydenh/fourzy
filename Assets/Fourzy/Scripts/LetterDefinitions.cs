using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fourzy
{
    public static class LetterDefinitions
    {
        public static int[,] GetRandomLetterDefinition()
        {
            int id = UnityEngine.Random.Range(1, 26);
            switch (id)
            {
                case 1: return LetterADefinition;
                case 2: return LetterBDefinition;
                case 3: return LetterCDefinition;
                case 4: return LetterDDefinition;
                case 5: return LetterEDefinition;
                case 6: return LetterFDefinition;
                case 7: return LetterGDefinition;
                case 8: return LetterHDefinition;
                case 9: return LetterIDefinition;
                case 10: return LetterJDefinition;
                case 11: return LetterKDefinition;
                case 12: return LetterLDefinition;
                case 13: return LetterMDefinition;
                case 14: return LetterNDefinition;
                case 15: return LetterODefinition;
                case 16: return LetterPDefinition;
                case 17: return LetterQDefinition;
                case 18: return LetterRDefinition;
                case 19: return LetterSDefinition;
                case 20: return LetterTDefinition;
                case 21: return LetterUDefinition;
                case 22: return LetterVDefinition;
                case 23: return LetterWDefinition;
                case 24: return LetterXDefinition;
                case 25: return LetterYDefinition;
                case 26: return LetterZDefinition;

            }

            return LetterADefinition;
        }

        public static int[,] LetterADefinition = new int[,]
        {
                {1,1,1,1,1},
                {1,0,0,0,1},
                {1,1,1,1,1},
                {1,0,0,0,1},
                {1,0,0,0,1}
        };

        public static int[,] LetterBDefinition = new int[,]
        {
                {1,1,1,1,0},
                {1,0,0,0,1},
                {1,1,1,1,0},
                {1,0,0,0,1},
                {1,1,1,1,0}
        };

        public static int[,] LetterCDefinition = new int[,]
        {
                {1,1,1,1,1},
                {1,0,0,0,0},
                {1,0,0,0,0},
                {1,0,0,0,0},
                {1,1,1,1,1}
        };

        public static int[,] LetterDDefinition = new int[,]
        {
                {1,1,1,1,0},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,1,1,1,0}
        };

        public static int[,] LetterEDefinition = new int[,]
        {
                {1,1,1,1,1},
                {1,0,0,0,0},
                {1,1,1,1,0},
                {1,0,0,0,0},
                {1,1,1,1,1}
        };

        public static int[,] LetterFDefinition = new int[,]
        {
                {1,1,1,1,1},
                {1,0,0,0,0},
                {1,1,1,1,0},
                {1,0,0,0,0},
                {1,0,0,0,0}
        };

        public static int[,] LetterGDefinition = new int[,]
        {
                {1,1,1,1,0},
                {1,0,0,0,0},
                {1,0,0,1,1},
                {1,0,0,0,1},
                {1,1,1,1,1}
        };

        public static int[,] LetterHDefinition = new int[,]
        {
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,1,1,1,1},
                {1,0,0,0,1},
                {1,0,0,0,1}
        };

        public static int[,] LetterIDefinition = new int[,]
        {
                {0,1,1,1,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,1,1,1,0}
        };

        public static int[,] LetterJDefinition = new int[,]
        {
                {0,0,0,0,1},
                {0,0,0,0,1},
                {0,0,0,0,1},
                {1,0,0,0,1},
                {1,1,1,1,1}
        };

        public static int[,] LetterKDefinition = new int[,]
        {
                {1,0,0,0,1},
                {1,0,0,1,0},
                {1,1,1,0,0},
                {1,0,0,1,0},
                {1,0,0,0,1}
        };

        public static int[,] LetterLDefinition = new int[,]
        {
                {1,0,0,0,0},
                {1,0,0,0,0},
                {1,0,0,0,0},
                {1,0,0,0,0},
                {1,1,1,1,1}
        };

        public static int[,] LetterMDefinition = new int[,]
        {
                {1,1,1,1,1},
                {1,0,1,0,1},
                {1,0,1,0,1},
                {1,0,0,0,1},
                {1,0,0,0,1}
        };

        public static int[,] LetterNDefinition = new int[,]
        {
                {1,0,0,0,1},
                {1,1,0,0,1},
                {1,0,1,0,1},
                {1,0,0,1,1},
                {1,0,0,0,1}
        };

        public static int[,] LetterODefinition = new int[,]
        {
                {1,1,1,1,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,1,1,1,1}
        };

        public static int[,] LetterPDefinition = new int[,]
        {
                {1,1,1,1,1},
                {1,0,0,0,1},
                {1,1,1,1,1},
                {1,0,0,0,0},
                {1,0,0,0,0}
        };

        public static int[,] LetterQDefinition = new int[,]
        {
                {0,1,1,1,0},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,0,0,1,1},
                {0,1,1,1,1}
        };

        public static int[,] LetterRDefinition = new int[,]
        {
                {1,1,1,1,0},
                {1,0,0,0,1},
                {1,1,1,1,0},
                {1,0,0,0,1},
                {1,0,0,0,1}
        };

        public static int[,] LetterSDefinition = new int[,]
        {
                {1,1,1,1,1},
                {1,0,0,0,0},
                {1,1,1,1,1},
                {0,0,0,0,1},
                {1,1,1,1,1}
        };

        public static int[,] LetterTDefinition = new int[,]
        {
                {1,1,1,1,1},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0}
        };

        public static int[,] LetterUDefinition = new int[,]
        {
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,1,1,1,1}
        };

        public static int[,] LetterVDefinition = new int[,]
        {
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {0,1,0,1,0},
                {0,0,1,0,0}
        };

        public static int[,] LetterWDefinition = new int[,]
        {
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,0,1,0,1},
                {1,0,1,0,1},
                {0,1,1,1,0}
        };
        public static int[,] LetterXDefinition = new int[,]
        {
                {1,0,0,0,1},
                {0,1,0,1,0},
                {0,0,1,0,0},
                {0,1,0,1,0},
                {1,0,0,0,1}
        };

        public static int[,] LetterYDefinition = new int[,]
        {
                {1,0,0,0,1},
                {1,1,0,1,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0}
        };


        public static int[,] LetterZDefinition = new int[,]
        {
                {1,1,1,1,1},
                {0,0,0,1,0},
                {0,0,1,0,0},
                {0,1,0,0,0},
                {1,1,1,1,1}
        };

    }
}
