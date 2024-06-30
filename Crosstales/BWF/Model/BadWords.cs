using System;
using System.Collections.Generic;
using System.Text;
using Crosstales.BWF.Util;

namespace Crosstales.BWF.Model
{
	[Serializable]
	public class BadWords
	{
		public Source Source;

		public List<string> BadWordList;

		public BadWords(Source source, List<string> badWordList)
		{
			Source = source;
			BadWordList = badWordList;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GetType().Name);
			stringBuilder.Append(Constants.TEXT_TOSTRING_START);
			stringBuilder.Append("Source='");
			stringBuilder.Append(Source);
			stringBuilder.Append(Constants.TEXT_TOSTRING_DELIMITER);
			stringBuilder.Append("BadWordList='");
			stringBuilder.Append(BadWordList);
			stringBuilder.Append(Constants.TEXT_TOSTRING_DELIMITER_END);
			stringBuilder.Append(Constants.TEXT_TOSTRING_END);
			return stringBuilder.ToString();
		}
	}
}
