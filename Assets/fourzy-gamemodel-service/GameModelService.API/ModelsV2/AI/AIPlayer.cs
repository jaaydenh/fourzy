using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public interface AIPlayer
    {
        PlayerTurn GetTurn();
    }

    public static class AIPlayerFactory
    {
        public static AIPlayer Create(GameState State, AIProfile Profile)
        {
            switch (Profile)
            {

                //These AI are limited to make it easier for the player.
                case AIProfile.BeginnerAI:
                    return new BeginnerAI(State);
                case AIProfile.EasyAI:
                    return new EasyAI(State);
                case AIProfile.BlindBot:
                    return new BlindBotAI(State);
                case AIProfile.BadBot:
                    return new BadBotAI(State);


                //This bot will make a good move, but will not win unless last resort.
                case AIProfile.ExtenderBotAI:
                    return new ExtenderBotAI(State);

                //The following AIs have a preference on direction.

                case AIProfile.UpBot:
                    return new UpBotAI(State);
                case AIProfile.DownAI:
                    return new DownBotAI(State);
                case AIProfile.RightAI:
                    return new RightBotAI(State);
                case AIProfile.LeftAI:
                    return new LeftBotAI(State);
                case AIProfile.VerticalBot:
                    return new VerticalBotAI(State);
                case AIProfile.HorizontalBot:
                    return new HorizontalBotAI(State);

                //This AI tries to change up the direction every turn.

                case AIProfile.RotatorBot:
                    return new RotatorBotAI(State);

                //These AI are a bit easier since they will sometimes skip a turn.

                case AIProfile.PassBot:
                    return new PassBotAI(State);
                case AIProfile.PanicBot:
                    return new PanicBotAI(State);
                case AIProfile.WaitBot:
                    return new WaitBotAI(State);

                //The position bot values center position over potential lines of four.  
                case AIProfile.PositionBot:
                    return new PositionBot(State);

                //The Basic AI.  Fast and fairly good, but not perfect.
                case AIProfile.SimpleAI:
                    return new SimpleAI(State);

                //Basic AI but scores using opponents position for more accurate move.
                case AIProfile.ScoreBot:
                    return new ScoreBotAI(State);
                case AIProfile.DoctorBot:
                    return new DoctorBotAI(State);

                //Used for Puzzles. Always returns the same move
                case AIProfile.PuzzleAI:
                    return new PuzzleAI(State);

            }
            return null;
        }

        public static AIPlayer Create(GameState State, AIDifficulty Difficulty)
        {
            int Personality = 0;
            switch (Difficulty)
            {
                case AIDifficulty.Pushover:
                    return new WaitBotAI(State);

                    Personality = State.Random.RandomInteger(0, 3);
                    switch (Personality)
                    {
                        case 0:
                            return new ExtenderBotAI(State);
                        case 1:
                            return new PanicBotAI(State);
                        case 2:
                            return new WaitBotAI(State);
                        case 3:
                            return new BadBotAI(State);
                    }
                    break;

                case AIDifficulty.Easy:
                    return new BlindBotAI(State);

                    Personality = State.Random.RandomInteger(0, 6);
                    switch (Personality)
                    {
                        case 0:
                            return new LeftBotAI(State);
                        case 1:
                            return new RightBotAI(State);
                        case 2:
                            return new UpBotAI(State);
                        case 3:
                            return new DownBotAI(State);
                        case 4:
                            return new VerticalBotAI(State);
                        case 5:
                            return new HorizontalBotAI(State);
                        case 6:
                            return new RotatorBotAI(State);
                        case 7:
                            return new BlindBotAI(State);

                    }
                    break;

                case AIDifficulty.Medium:
                    return new PositionBot(State);

                    Personality = State.Random.RandomInteger(0, 2);
                    switch (Personality)
                    {
                        case 0:
                            return new BeginnerAI(State);
                        case 1:
                            return new EasyAI(State);

                        case 2:
                            return new PositionBot(State);
                    }
                    break;

                case AIDifficulty.Hard:
                    return new SimpleAI(State);

                case AIDifficulty.Doctor:
                    return new ScoreBotAI(State);
            }
            return null;
        }

        public static AIProfile RandomProfile(AIDifficulty Difficulty)
        {
            RandomTools Random = new RandomTools(Guid.NewGuid().ToString());

            int Personality = 0;
            switch (Difficulty)
            {
                case AIDifficulty.Pushover:
                    return AIProfile.BadBot;
                    Personality = Random.RandomInteger(0, 2);
                    switch (Personality)
                    {
                        case 0:
                            return AIProfile.ExtenderBotAI;
                        case 1:
                            return AIProfile.PanicBot;
                        case 2:
                            return AIProfile.WaitBot;
                    }
                    break;

                case AIDifficulty.Easy:
                    return AIProfile.BlindBot;

                    Personality = Random.RandomInteger(0, 6);
                    switch (Personality)
                    {
                        case 0:
                            return AIProfile.LeftAI;
                        case 1:
                            return AIProfile.RightAI;
                        case 2:
                            return AIProfile.UpBot;
                        case 3:
                            return AIProfile.DownAI;
                        case 4:
                            return AIProfile.VerticalBot;
                        case 5:
                            return AIProfile.HorizontalBot;
                        case 6:
                            return AIProfile.RotatorBot;
                    }
                    break;

                case AIDifficulty.Medium:
                    return AIProfile.PositionBot;

                    Personality = Random.RandomInteger(0, 2);
                    switch (Personality)
                    {
                        case 0:
                            return AIProfile.BeginnerAI;
                        case 1:
                            return AIProfile.EasyAI;
                        case 2:
                            return AIProfile.PositionBot;
                    }
                    break;

                case AIDifficulty.Hard:
                    Personality = Random.RandomInteger(0, 1);
                    return AIProfile.SimpleAI;
                    switch (Personality)
                    {
                        case 0:
                            return AIProfile.SimpleAI;
                        case 1:
                            return AIProfile.ScoreBot;
                        case 2:
                            return AIProfile.DoctorBot;
                    }
                    break;

                case AIDifficulty.Doctor:
                    return AIProfile.DoctorBot;
            }
            return AIProfile.PassBot;
        }


        public static string GenerateAIPlayerName(AIDifficulty Difficulty, int length = -1)
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


