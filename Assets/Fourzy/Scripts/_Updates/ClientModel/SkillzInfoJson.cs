
namespace Fourzy._Updates.ClientModel
{
	internal static class SkillzInfoJson
	{
		public static string BuildForSyncGame(int gameId, bool getPlayers = false)
		{
			return
				"{ "
				+ $"{GetMatchBody(gameId, getPlayers)}, "
				+ CustomServerConnectionInfo(gameId.ToString())
				+ "}";
		}

		private static string GetMatchBody(int gameId, bool getPlayers = false)
		{
			string result =
				"\"matchDescription\": \"A mocked Skillz match in Unity\", "
				+ "\"entryCash\": 0, "
				+ "\"entryPoints\": 2, "
				+ $"\"id\": {gameId}, "
				+ "\"templateId\": 1061, "
				+ "\"name\": \"Warmup!\", "
				+ "\"isCash\": false, "
				+ "\"isSynchronous\": false, ";

			if (getPlayers)
			{
				result +=
					"\"players\": ["
					+ $"{GetPlayerBody("0", false)} ,"
					+ GetPlayerBody("1", true)
					+ "]";
			}
			else
			{
				result += "\"players\": [{}]";
			}

			return result;
		}

		private static string CustomServerConnectionInfo(string matchId)
		{
			return "\"connectionInfo\":"
                    + "{"
					+ $"\"matchId\": {matchId}, "
					+ $"\"serverIP\": \"\", "
					+ $"\"serverPort\": \"\", "
					+ $"\"matchToken\": \"\", "
					+ $"\"isBotMatch\": \"\""
					+ "}";
		}

		private static string GetPlayerBody(string id, bool current)
		{
			return
				"{ "
				+ $"\"userId\": \"{id}\", "
				+ $"\"userName\": \"{id}\", "
				+ $"\"avatarUrl\": \"\", "
				+ $"\"flagUrl\": \"\", "
				+ $"\"isCurrentPlayer\": \"{current}\", "
				+ $"\"playerMatchId\": \"\""
				+ "}";

		}
	}
}