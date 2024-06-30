using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Crosstales.BWF.Model;
using Crosstales.BWF.Provider;
using Crosstales.BWF.Util;

namespace Crosstales.BWF.Filter
{
	public class DomainFilter : BaseFilter
	{
		public string ReplaceCharacters;

		private List<DomainProvider> domainProvider = new List<DomainProvider>();

		private readonly List<DomainProvider> tempDomainProvider;

		private readonly Dictionary<string, Regex> domainsRegex = new Dictionary<string, Regex>();

		private readonly Dictionary<string, List<Regex>> debugDomainsRegex = new Dictionary<string, List<Regex>>();

		private bool ready;

		private bool readyFirstime;

		public List<DomainProvider> DomainProvider
		{
			get
			{
				return domainProvider;
			}
			set
			{
				domainProvider = value;
				if (domainProvider == null || domainProvider.Count <= 0)
				{
					return;
				}
				foreach (DomainProvider item in domainProvider)
				{
					if (item != null)
					{
						if (Constants.DEBUG_DOMAINS)
						{
							debugDomainsRegex.CTAddRange(item.DebugDomainsRegex);
						}
						else
						{
							domainsRegex.CTAddRange(item.DomainsRegex);
						}
					}
					else if (Helper.isEditorMode)
					{
					}
				}
			}
		}

		public override bool isReady
		{
			get
			{
				bool flag = true;
				if (!ready)
				{
					if (tempDomainProvider != null)
					{
						foreach (DomainProvider item in tempDomainProvider)
						{
							if (item != null && !item.isReady)
							{
								flag = false;
								break;
							}
						}
					}
					if (!readyFirstime && flag)
					{
						DomainProvider = tempDomainProvider;
						if (DomainProvider != null)
						{
							foreach (DomainProvider item2 in DomainProvider)
							{
								if (!(item2 != null))
								{
									continue;
								}
								Source[] array = item2.Sources;
								foreach (Source source in array)
								{
									if (!sources.ContainsKey(source.Name))
									{
										sources.Add(source.Name, source);
									}
								}
							}
						}
						readyFirstime = true;
					}
				}
				ready = flag;
				return flag;
			}
		}

		public DomainFilter(List<DomainProvider> domainProvider, string replaceCharacters, string markPrefix, string markPostfix)
		{
			tempDomainProvider = domainProvider;
			ReplaceCharacters = replaceCharacters;
			MarkPrefix = markPrefix;
			MarkPostfix = markPostfix;
		}

		public override bool Contains(string testString, params string[] sources)
		{
			bool result = false;
			if (isReady)
			{
				if (string.IsNullOrEmpty(testString))
				{
					logContains();
				}
				else if (Constants.DEBUG_DOMAINS)
				{
					if (sources == null || sources.Length == 0)
					{
						foreach (List<Regex> value3 in debugDomainsRegex.Values)
						{
							foreach (Regex item in value3)
							{
								Match match = item.Match(testString);
								if (match.Success)
								{
									result = true;
									break;
								}
							}
						}
					}
					else
					{
						foreach (string text in sources)
						{
							if (debugDomainsRegex.TryGetValue(text, out var value))
							{
								foreach (Regex item2 in value)
								{
									Match match2 = item2.Match(testString);
									if (match2.Success)
									{
										result = true;
										break;
									}
								}
							}
							else
							{
								logResourceNotFound(text);
							}
						}
					}
				}
				else if (sources == null || sources.Length == 0)
				{
					foreach (Regex value4 in domainsRegex.Values)
					{
						if (value4.Match(testString).Success)
						{
							result = true;
							break;
						}
					}
				}
				else
				{
					foreach (string text2 in sources)
					{
						if (domainsRegex.TryGetValue(text2, out var value2))
						{
							Match match3 = value2.Match(testString);
							if (match3.Success)
							{
								result = true;
								break;
							}
						}
						else
						{
							logResourceNotFound(text2);
						}
					}
				}
			}
			else
			{
				logFilterNotReady();
			}
			return result;
		}

		public override List<string> GetAll(string testString, params string[] sources)
		{
			List<string> list = new List<string>();
			if (isReady)
			{
				if (string.IsNullOrEmpty(testString))
				{
					logGetAll();
				}
				else if (Constants.DEBUG_DOMAINS)
				{
					if (sources == null || sources.Length == 0)
					{
						foreach (List<Regex> value3 in debugDomainsRegex.Values)
						{
							foreach (Regex item in value3)
							{
								MatchCollection matchCollection = item.Matches(testString);
								foreach (Match item2 in matchCollection)
								{
									foreach (Capture capture5 in item2.Captures)
									{
										if (!list.Contains(capture5.Value))
										{
											list.Add(capture5.Value);
										}
									}
								}
							}
						}
					}
					else
					{
						foreach (string text in sources)
						{
							if (debugDomainsRegex.TryGetValue(text, out var value))
							{
								foreach (Regex item3 in value)
								{
									MatchCollection matchCollection2 = item3.Matches(testString);
									foreach (Match item4 in matchCollection2)
									{
										foreach (Capture capture6 in item4.Captures)
										{
											if (!list.Contains(capture6.Value))
											{
												list.Add(capture6.Value);
											}
										}
									}
								}
							}
							else
							{
								logResourceNotFound(text);
							}
						}
					}
				}
				else if (sources == null || sources.Length == 0)
				{
					foreach (Regex value4 in domainsRegex.Values)
					{
						MatchCollection matchCollection3 = value4.Matches(testString);
						foreach (Match item5 in matchCollection3)
						{
							foreach (Capture capture7 in item5.Captures)
							{
								if (!list.Contains(capture7.Value))
								{
									list.Add(capture7.Value);
								}
							}
						}
					}
				}
				else
				{
					foreach (string text2 in sources)
					{
						if (domainsRegex.TryGetValue(text2, out var value2))
						{
							MatchCollection matchCollection4 = value2.Matches(testString);
							foreach (Match item6 in matchCollection4)
							{
								foreach (Capture capture8 in item6.Captures)
								{
									if (!list.Contains(capture8.Value))
									{
										list.Add(capture8.Value);
									}
								}
							}
						}
						else
						{
							logResourceNotFound(text2);
						}
					}
				}
			}
			else
			{
				logFilterNotReady();
			}
			return (from x in list.Distinct()
				orderby x
				select x).ToList();
		}

		public override string ReplaceAll(string testString, params string[] sources)
		{
			string text = testString;
			if (isReady)
			{
				if (string.IsNullOrEmpty(testString))
				{
					logReplaceAll();
					text = string.Empty;
				}
				else if (Constants.DEBUG_DOMAINS)
				{
					if (sources == null || sources.Length == 0)
					{
						foreach (List<Regex> value3 in debugDomainsRegex.Values)
						{
							foreach (Regex item in value3)
							{
								MatchCollection matchCollection = item.Matches(testString);
								foreach (Match item2 in matchCollection)
								{
									foreach (Capture capture5 in item2.Captures)
									{
										text = text.Replace(capture5.Value, Helper.CreateReplaceString(ReplaceCharacters, capture5.Value.Length));
									}
								}
							}
						}
					}
					else
					{
						foreach (string text2 in sources)
						{
							if (debugDomainsRegex.TryGetValue(text2, out var value))
							{
								foreach (Regex item3 in value)
								{
									MatchCollection matchCollection2 = item3.Matches(testString);
									foreach (Match item4 in matchCollection2)
									{
										foreach (Capture capture6 in item4.Captures)
										{
											text = text.Replace(capture6.Value, Helper.CreateReplaceString(ReplaceCharacters, capture6.Value.Length));
										}
									}
								}
							}
							else
							{
								logResourceNotFound(text2);
							}
						}
					}
				}
				else if (sources == null || sources.Length == 0)
				{
					foreach (Regex value4 in domainsRegex.Values)
					{
						MatchCollection matchCollection3 = value4.Matches(testString);
						foreach (Match item5 in matchCollection3)
						{
							foreach (Capture capture7 in item5.Captures)
							{
								text = text.Replace(capture7.Value, Helper.CreateReplaceString(ReplaceCharacters, capture7.Value.Length));
							}
						}
					}
				}
				else
				{
					foreach (string text3 in sources)
					{
						if (domainsRegex.TryGetValue(text3, out var value2))
						{
							MatchCollection matchCollection4 = value2.Matches(testString);
							foreach (Match item6 in matchCollection4)
							{
								foreach (Capture capture8 in item6.Captures)
								{
									text = text.Replace(capture8.Value, Helper.CreateReplaceString(ReplaceCharacters, capture8.Value.Length));
								}
							}
						}
						else
						{
							logResourceNotFound(text3);
						}
					}
				}
			}
			else
			{
				logFilterNotReady();
			}
			return text;
		}

		public override string Replace(string text, List<string> domains)
		{
			string text2 = text;
			if (string.IsNullOrEmpty(text))
			{
				logReplace();
				text2 = string.Empty;
			}
			else if (domains != null && domains.Count != 0)
			{
				foreach (string domain in domains)
				{
					text2 = text2.Replace(domain, Helper.CreateReplaceString(ReplaceCharacters, domain.Length));
				}
			}
			return text2;
		}
	}
}
