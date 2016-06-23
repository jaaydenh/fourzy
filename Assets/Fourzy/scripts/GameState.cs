using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;

namespace ConnectFour
{
	// Encapsulates all information that represents the state of our game, so that
	// this information can be saved and loaded.
	public class GameState
	{
		// Information about an opponent who is playing on the same device as the same user
		// (for playing the game by physically handing the device back and forth).
		private const string LocalOpponentId = "locopp";
		private const string LocalOpponentName = "Local Opponent";

		// A game state simply contains the user's info, info about all of the user's friends,
		// and all match states the user is involved with.
		public PlayerInfo Self;
		public Dictionary<string, PlayerInfo> Friends { get; private set; }
		public Dictionary<string, MatchState> MatchStates { get; private set; }

		private GameState()
		{
			throw new InvalidOperationException();
		}

		public GameState(PlayerInfo self)
		{
			Self = self;
			Friends = new Dictionary<string, PlayerInfo>();
			MatchStates = new Dictionary<string, MatchState>();
		}

		public GameState(PlayerInfo self, Dictionary<string, PlayerInfo> friends, Dictionary<string, MatchState> matches)
		{
			Self = self;
			Friends = friends;
			MatchStates = matches;
		}

		public MatchState getWithIdentifier(string id)
		{
			if (MatchStates.ContainsKey(id))
			{
				return MatchStates[id];
			}
			return null;
		}

		public void AddLocalOpponent()
		{
			Friends.Add(GameState.LocalOpponentId, new GameState.PlayerInfo(LocalOpponentId, LocalOpponentName));
		}

		// Info about a stayer, currently just their id and name. The annotations show how to
		// store this data in DynamoDB.
		[DynamoDBTable("FourzyPlayers")]
		public class PlayerInfo
		{
			[DynamoDBHashKey]
			public string Id { get; private set; }
			[DynamoDBProperty]
			public string Name { get; private set; }

			public PlayerInfo(string identifier, string name)
			{
				this.Id = identifier;
				this.Name = name;
			}
			public PlayerInfo()
			{
				this.Id = LocalOpponentId;
				this.Name = LocalOpponentName;
			}

			public bool IsLocalOpponent()
			{
				return Id == LocalOpponentId;
			}
		}

		// A single match of fourzy.
		public class MatchState
		{
			public PlayerInfo Opponent { get; private set; }
			public bool SelfIsBlue { get; private set; }
			public BoardState BoardState { get; set; }
			public string Identifier { get; private set; }

			public MatchState(PlayerInfo opponent, string boardNotation, string moves, bool selfIsBlue, string identifier)
			{
				this.Opponent = opponent;
				if (boardNotation == null || moves == null)
				{
					this.BoardState = null;
				}
				else
				{
					this.BoardState = new BoardState(boardNotation, moves);
				}
				this.SelfIsBlue = selfIsBlue;
				this.Identifier = identifier;
			}

			public MatchState(PlayerInfo opponent, BoardState boardState, bool selfIsBlue, string identifier)
			{
				this.Opponent = opponent;
				this.BoardState = boardState;
				this.SelfIsBlue = selfIsBlue;
				this.Identifier = identifier;
			}

			public bool IsSelfTurn()
			{
				return (BoardState.TurnColor == BoardState.FourzyPieceColor.Blue) == SelfIsBlue;
			}
		}
	}
}