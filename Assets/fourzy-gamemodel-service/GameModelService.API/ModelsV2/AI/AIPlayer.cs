﻿using System;
using System.Text;

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
                case AIProfile.OrthoBot:
                    return new OrthoBotAI(State);


                //This bot will make a good move, but will not win unless last resort.
                case AIProfile.ExtenderBotAI:
                    return new ExtenderBotAI(State);
                //This bot will make a good move, but will not win unless last resort.
                case AIProfile.UnevenBotAI:
                    return new UnevenBotAI(State);

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

                //This bot tinkers with Heuristics on only score it's own pieces

                case AIProfile.MeBot:
                    return new MeBotAI(State);

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
                case AIProfile.SmartBot:
                    return new SmartBotAI(State);

                //Basic AI but scores using opponents position for more accurate move.
                case AIProfile.ScoreBot:
                    return new ScoreBotAI(State);

                case AIProfile.AggressiveAI:
                    return new AggressiveAI(State);
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
            //int Personality = 0;

            AIProfile Profile = RandomProfile(Difficulty);
            return Create(State, Profile);
        }

        public static AIProfile RandomProfile(AIDifficulty Difficulty)
        {
            RandomTools Random = new RandomTools(Guid.NewGuid().ToString());

            int Personality = 0;
            switch (Difficulty)
            {
                case AIDifficulty.Pushover:
                    Personality = Random.RandomInteger(0, 0);
                    switch (Personality)
                    {
                        case 0:
                            return AIProfile.BadBot;
                        case 1:
                            return AIProfile.WaitBot;
                    
                    }
                    break;

                case AIDifficulty.Easy:

                    Personality = Random.RandomInteger(0, 10);
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
                            return AIProfile.BlindBot;
                        case 7:
                            return AIProfile.EasyAI;
                        case 8:
                            return AIProfile.UnevenBotAI;
                        case 9:
                            return AIProfile.OrthoBot;

                    }
                    break;

                case AIDifficulty.Medium:
                  
                    Personality = Random.RandomInteger(0, 4);
                    switch (Personality)
                    {
                        case 0:
                            return AIProfile.BeginnerAI;
                        case 1:
                            return AIProfile.PositionBot;
                        case 2:
                            return AIProfile.MeBot;
                        case 3:
                            return AIProfile.ExtenderBotAI;
                        case 4:
                            return AIProfile.RotatorBot;
                    }
                    break;

                case AIDifficulty.Hard:
                    Personality = Random.RandomInteger(0, 1);
                    switch (Personality)
                    {
                        case 0:
                            return AIProfile.SimpleAI;
                        case 1:
                            return AIProfile.AggressiveAI;
                        case 2:
                            return AIProfile.SmartBot;
                            //case 2:
                            //    //return AIProfile.SmartBot;
                            //case 3:
                            //    //return AIProfile.ScoreBot;

                    }
                    break;

                case AIDifficulty.Doctor:
                    return AIProfile.SimpleAI;
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

