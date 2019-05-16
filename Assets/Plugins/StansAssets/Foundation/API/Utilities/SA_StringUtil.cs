using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Foundation.Utility
{
	public static class SA_StringUtil
    {
		private static StringBuilder _stringBuilder = new StringBuilder();

		private const string SecondsFormat = "{0:n0}:{1:00}";

		public static string Concat (params object[] objects)
		{
			_stringBuilder.Length = 0;
			for (var i = 0; i < objects.Length; i++)
			{
				_stringBuilder.Append(objects[i]);
			}
			return _stringBuilder.ToString();
		}

		public static string Join (object separator, List<string> objects)
		{
			_stringBuilder.Length = 0;

			if (objects.Count > 0)
			{
				_stringBuilder.Append(objects[0]);
			}

			for (var i = 1; i < objects.Count; i++)
			{
				_stringBuilder.Append(separator);
				_stringBuilder.Append(objects[i]);
			}

			return _stringBuilder.ToString();
		}

		public static string Join (object separator, params object[] objects)
		{
			_stringBuilder.Length = 0;

			if (objects.Length > 0) {
				_stringBuilder.Append(objects[0]);
			}

			for (var i = 1; i < objects.Length; i++) {
				_stringBuilder.Append(separator);
				_stringBuilder.Append(objects[i]);
			}

			return _stringBuilder.ToString();
		}

		public static string Format (string format, params object[] objects)
		{
			_stringBuilder.Length = 0;
			_stringBuilder.AppendFormat(format, objects);
			return _stringBuilder.ToString();
		}

		public static string SecondsToString (float seconds) {
			return SecondsToString((int)Math.Truncate(seconds));
		}

		public static string SecondsToString(int seconds) {
			return Format(SecondsFormat, seconds / 60, seconds % 60);
		}
	}
}
