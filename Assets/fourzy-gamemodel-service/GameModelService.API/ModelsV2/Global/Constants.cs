using System;
using System.Collections.Generic;
using System.Text;

namespace FourzyGameModel.Model
{
    public static class Constants
    {
        public const int DefaultRows = 8;
        public const int DefaultColumns = 8;
        public const char ColDelimiter = ',';
        public const char RowDelimiter = '|';
        public const char DataDelimiter = ':';
        public const int DefaultStartingSeconds = 120;
        public const int TurnsUntilSinkInQuicksand = 4;
        public const int TurnsUntilFireConsumesPiece = 4;
        public const int PlayerStartingMagic = 100;
        public const int DefaultFlowerMagic = 10;
        public const int MinimumMomentum = 30;
        public const int MaximumMomentum = 40;

        public static List<Direction> Directions
        {
            get
            {
                List<Direction> dlist = new List<Direction>();

                foreach (Direction d in Enum.GetValues(typeof(Direction)))
                {
                    if (d != Direction.NONE && d != Direction.RANDOM && d != Direction.TELEPORT)
                        dlist.Add(d);
                }
                return dlist;
            }
        }

        public static string GenerateName(int length = -1)
        {
            const string vowels = "aeiou";
            const string consonants = "bcdfghjklmnpqrstvwxyz";

            var rnd = new Random();
            var name = new StringBuilder();
            if (length < 0) length = rnd.Next(3, 8);

                length = length % 2 == 0 ? length : length + 1;

            for (var i = 0; i < length / 2; i++)
            {
                name
                    .Append(vowels[rnd.Next(vowels.Length)])
                    .Append(consonants[rnd.Next(consonants.Length)]);
            }
            name[0] = char.ToUpper(name[0]);
            return name.ToString();
        }
    }
}
