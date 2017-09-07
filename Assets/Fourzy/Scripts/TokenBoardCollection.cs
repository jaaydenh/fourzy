using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace Fourzy
{
	public class TokenBoardCollection : MonoBehaviour
	{
		private TokenBoardInfo[] allTokenBoards;

		public TokenBoardInfo LoadAllTokenBoards()
		{
			// XmlSerializer serializer = new XmlSerializer(typeof(TokenBoardInfo[]));
			// using (StreamReader streamReader = new StreamReader("TokenBoards.xml"))
			// {
			// 	allTokenBoards = (TokenBoardInfo[])serializer.Deserialize(streamReader);
			// }

			var asset = Resources.Load<TextAsset>("TokenBoards.json");
			var info = JsonUtility.FromJson<TokenBoardInfo[]>(asset.text);

			var tokenBoard = allTokenBoards
				.Where(t => t.Enabled == true)
				.OrderBy(t => UnityEngine.Random.Range(0, int.MaxValue))
				.FirstOrDefault();

			return tokenBoard;
		}
		
		public TokenBoardInfo GetRandomTokenBoard()
		{
			//ResetQuestionsIfAllHaveBeenAsked();

			var tokenBoard = allTokenBoards
				.Where(t => t.Enabled == true)
				.OrderBy(t => UnityEngine.Random.Range(0, int.MaxValue))
				.FirstOrDefault();

			return tokenBoard;
		}

		// private void ResetQuestionsIfAllHaveBeenAsked()
		// {
		// 	if (allQuestions.Any(t => t.Asked == false) == false)
		// 	{
		// 		ResetQuestions();
		// 	}
		// }

		// private void ResetQuestions()
		// {
		// 	foreach (var question in allTokenBoards)
		// 		question.Asked = false;
		// }

		/// <summary>
		/// This method is used to generate a starting sample xml file if none exists
		/// </summary>
		// private void WriteSampleQuestionsToXml()
		// {
		// 	allQuestions = new QuizQuestion[] {
		// 		new QuizQuestion { Question = "If it's noon in Boston, what time is it in New York",
		// 			Answers = new string[] { "1PM", "2PM", "Noon", "11AM" }, CorrectAnswer = 2 },
		// 		new QuizQuestion { Question = "What type of animal was Babe in the film of the same name",
		// 			Answers = new string[] { "Donkey", "Spider", "Dog", "Pig" }, CorrectAnswer = 3 },
		// 	};

		// 	XmlSerializer serializer = new XmlSerializer(typeof(QuizQuestion[]));
		// 	using (StreamWriter streamWriter = new StreamWriter("Questions.xml"))
		// 	{
		// 		serializer.Serialize(streamWriter, allQuestions);
		// 	}
		// }
	}
}