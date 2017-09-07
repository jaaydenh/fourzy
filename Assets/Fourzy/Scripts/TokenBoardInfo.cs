//using System.Xml.Serialization;

namespace Fourzy
{
	[System.Serializable]
	public class TokenBoardInfo
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public bool Enabled { get; set; }
		public int[] TokenData { get; set; }
		
	}
}