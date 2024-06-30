using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Crosstales.BWF.Filter
{
	public class CapitalizationFilter : BaseFilter
	{
		private int characterNumber;

		public Regex RegularExpression { get; private set; }

		public int CharacterNumber
		{
			get
			{
				return characterNumber;
			}
			set
			{
				if (value < 1)
				{
					characterNumber = 1;
				}
				else
				{
					characterNumber = value;
				}
				RegularExpression = new Regex("\\b\\w*[A-ZÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖØÙÚÛÜÝ]{" + characterNumber + ",}\\w*\\b", RegexOptions.CultureInvariant);
			}
		}

		public override bool isReady => true;

		public CapitalizationFilter(int capitalizationCharsNumber, string markPrefix, string markPostfix)
		{
			CharacterNumber = capitalizationCharsNumber;
			MarkPrefix = markPrefix;
			MarkPostfix = markPostfix;
		}

		public override bool Contains(string testString, params string[] sources)
		{
			bool result = false;
			if (string.IsNullOrEmpty(testString))
			{
				logContains();
			}
			else
			{
				result = RegularExpression.Match(testString).Success;
			}
			return result;
		}

		public override List<string> GetAll(string testString, params string[] sources)
		{
			List<string> list = new List<string>();
			if (string.IsNullOrEmpty(testString))
			{
				logGetAll();
			}
			else
			{
				MatchCollection matchCollection = RegularExpression.Matches(testString);
				foreach (Match item in matchCollection)
				{
					foreach (Capture capture in item.Captures)
					{
						if (!list.Contains(capture.Value))
						{
							list.Add(capture.Value);
						}
					}
				}
			}
			return (from x in list.Distinct()
				orderby x
				select x).ToList();
		}

		public override string ReplaceAll(string testString, params string[] sources)
		{
			string text = testString;
			if (string.IsNullOrEmpty(testString))
			{
				logReplaceAll();
				text = string.Empty;
			}
			else
			{
				MatchCollection matchCollection = RegularExpression.Matches(testString);
				foreach (Match item in matchCollection)
				{
					foreach (Capture capture in item.Captures)
					{
						text = text.Replace(capture.Value, capture.Value.ToLowerInvariant());
					}
				}
			}
			return text;
		}

		public override string Replace(string text, List<string> badWords)
		{
			string text2 = text;
			if (string.IsNullOrEmpty(text))
			{
				logReplace();
				text2 = string.Empty;
			}
			else if (badWords != null && badWords.Count != 0)
			{
				foreach (string badWord in badWords)
				{
					text2 = text2.Replace(badWord, badWord.ToLowerInvariant());
				}
			}
			return text2;
		}
	}
}
