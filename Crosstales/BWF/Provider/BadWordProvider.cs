using System.Collections.Generic;
using System.Text.RegularExpressions;
using Crosstales.BWF.Model;
using Crosstales.BWF.Util;

namespace Crosstales.BWF.Provider
{
	public abstract class BadWordProvider : BaseProvider
	{
		protected List<BadWords> badwords = new List<BadWords>();

		private const string exactRegexStart = "(?<![\\w\\d])";

		private const string exactRegexEnd = "s?(?![\\w\\d])";

		private const string fuzzyRegexStart = "\\b\\w*";

		private const string fuzzyRegexEnd = "\\w*\\b";

		private Dictionary<string, Regex> exactBadwordsRegex = new Dictionary<string, Regex>();

		private Dictionary<string, Regex> fuzzyBadwordsRegex = new Dictionary<string, Regex>();

		private Dictionary<string, List<Regex>> debugExactBadwordsRegex = new Dictionary<string, List<Regex>>();

		private Dictionary<string, List<Regex>> debugFuzzyBadwordsRegex = new Dictionary<string, List<Regex>>();

		public Dictionary<string, Regex> ExactBadwordsRegex
		{
			get
			{
				return exactBadwordsRegex;
			}
			protected set
			{
				exactBadwordsRegex = value;
			}
		}

		public Dictionary<string, Regex> FuzzyBadwordsRegex
		{
			get
			{
				return fuzzyBadwordsRegex;
			}
			protected set
			{
				fuzzyBadwordsRegex = value;
			}
		}

		public Dictionary<string, List<Regex>> DebugExactBadwordsRegex
		{
			get
			{
				return debugExactBadwordsRegex;
			}
			protected set
			{
				debugExactBadwordsRegex = value;
			}
		}

		public Dictionary<string, List<Regex>> DebugFuzzyBadwordsRegex
		{
			get
			{
				return debugFuzzyBadwordsRegex;
			}
			protected set
			{
				debugFuzzyBadwordsRegex = value;
			}
		}

		public override void Load()
		{
			if (ClearOnLoad)
			{
				badwords.Clear();
			}
		}

		protected override void init()
		{
			ExactBadwordsRegex.Clear();
			FuzzyBadwordsRegex.Clear();
			if (Constants.DEBUG_BADWORDS)
			{
			}
			foreach (BadWords badword in badwords)
			{
				if (Constants.DEBUG_BADWORDS)
				{
					List<Regex> list = new List<Regex>(badword.BadWordList.Count);
					List<Regex> list2 = new List<Regex>(badword.BadWordList.Count);
					foreach (string badWord in badword.BadWordList)
					{
						list.Add(new Regex("(?<![\\w\\d])" + badWord + "s?(?![\\w\\d])", RegexOption1 | RegexOption2 | RegexOption3 | RegexOption4 | RegexOption5));
						list2.Add(new Regex("\\b\\w*" + badWord + "\\w*\\b", RegexOption1 | RegexOption2 | RegexOption3 | RegexOption4 | RegexOption5));
					}
					if (!DebugExactBadwordsRegex.ContainsKey(badword.Source.Name))
					{
						DebugExactBadwordsRegex.Add(badword.Source.Name, list);
					}
					if (!DebugExactBadwordsRegex.ContainsKey(badword.Source.Name))
					{
						DebugExactBadwordsRegex.Add(badword.Source.Name, list2);
					}
				}
				else
				{
					if (!ExactBadwordsRegex.ContainsKey(badword.Source.Name))
					{
						ExactBadwordsRegex.Add(badword.Source.Name, new Regex("(?<![\\w\\d])(" + string.Join("|", badword.BadWordList.ToArray()) + ")s?(?![\\w\\d])", RegexOption1 | RegexOption2 | RegexOption3 | RegexOption4 | RegexOption5));
					}
					if (!FuzzyBadwordsRegex.ContainsKey(badword.Source.Name))
					{
						FuzzyBadwordsRegex.Add(badword.Source.Name, new Regex("\\b\\w*(" + string.Join("|", badword.BadWordList.ToArray()) + ")\\w*\\b", RegexOption1 | RegexOption2 | RegexOption3 | RegexOption4 | RegexOption5));
					}
				}
				if (!Constants.DEBUG_BADWORDS)
				{
				}
			}
			base.isReady = true;
		}
	}
}
