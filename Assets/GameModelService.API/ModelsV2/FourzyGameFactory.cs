using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FourzyGameModel.Model
{
    public static class FourzyGameFactory
    {

        //PLAYER vs PLAYER games.

        // Select a Area and create a random board.
        // Active Player Id is set to -1 as the first player is not known when the game is created
        public static FourzyGame CreateFourzyGame(Area Area, Player Player1, Player Player2, int FirstPlayerId = 1, GameOptions Options = null, BoardGenerationPreferences BoardPreferences = null)
        {
            if (Options == null) Options = new GameOptions();

            GameBoard Board = BoardFactory.CreateGameBoard(Area, Options, BoardPreferences);
            return new FourzyGame();
        }

        // Pass in a Board
        public static FourzyGame CreateFourzyGame(GameBoard Board, Player Player1, Player Player2, int FirstPlayerId = 1, GameOptions Options = null)
        {
            if (Options == null) Options = new GameOptions();

            GameState State = new GameState(Board, Options, FirstPlayerId);
            State.Players.Add(1, Player1);
            State.Players.Add(2, Player2);
            State.ActivePlayerId = FirstPlayerId;

            return new FourzyGame(State);
        }


        //AI GAMES

        //Create a ai match with a random area board.
        public static FourzyGame CreateAIGameWithGeneratedBoard(Area Area, AIProfile Profile, Player Human, Player AI = null, int FirstPlayerId = 1, GameOptions Options = null, BoardGenerationPreferences Preferences = null)
        {
            if (Options == null) Options = new GameOptions();
            if (Preferences == null) Preferences = new BoardGenerationPreferences();

            GameBoard Board = BoardFactory.CreateGameBoard(Area, Options, Preferences);
            GameState State = new GameState(Board, Options, FirstPlayerId);
            State.Players.Add(1, Human);

            if (AI == null)
                AI = new Player(2, Profile.ToString(), Profile);
            State.Players.Add(2, AI);

            State.ActivePlayerId = FirstPlayerId;

            FourzyGame Game = new FourzyGame(State);
            Game.GameType = GameType.AI;
            return Game;
        }

        public static FourzyGame CreateAIGameWithCustomBoard(GameBoard Board, AIProfile Profile, Player Human, Player AI = null, int FirstPlayerId = 1, GameOptions Options = null)
        {
            if (Options == null) Options = new GameOptions();

            GameState State = new GameState(Board, Options, FirstPlayerId);

            State.Players.Add(1, Human);
            if (AI == null)
                AI = new Player(2, Profile.ToString(), Profile);
            State.Players.Add(2, AI);

            FourzyGame Game = new FourzyGame(State);
            Game.GameType = GameType.AI;
            return Game;

        }

        //THE GAUNTLET

        public static FourzyGame CreateGauntletGame(Player Human, int GauntletLevel, Area CurrentArea = Area.NONE, int DifficultModifier = -1, int membersCount = 999, GameOptions Options = null, string SeedString = "")
        {
            if (SeedString.Length == 0) SeedString = Guid.NewGuid().ToString();

            //GameState State = BossGameFactory.CreateBossGame(CurrentArea, BossType.DirectionMaster, new Player(1,"Player"), Options);
            GameState State = GauntletFactory.Create(Human, GauntletLevel, membersCount, CurrentArea, DifficultModifier, Options, SeedString);

            FourzyGame Game = new FourzyGame(State);
            Game.GameType = GameType.AI;
            return Game;
        }

        //Create a boss match with a specific board.
        public static FourzyGame CreateBossGame(GameBoardDefinition Board, BossType Boss, Player Human, GameOptions Options = null)
        {
            //We should standardize this function so that it GameBoard instead of GameBoard Defintion
            GameState State = BossGameFactory.CreateBossGame(Board, Boss, Human, Options);

            FourzyGame Game = new FourzyGame(State);
            Game.GameType = GameType.AI;
            return Game;
        }

        //Create a boss match in an selected Area.
        public static FourzyGame CreateBossGame(Area Area, BossType Boss, Player Human, GameOptions Options = null, BoardGenerationPreferences Preferences = null)
        {
            GameState State = BossGameFactory.CreateBossGame(Area, Boss, Human, Options, Preferences);
 
            FourzyGame Game = new FourzyGame(State);
            Game.GameType = GameType.AI;
            return Game;
        }
    
    }   
}
