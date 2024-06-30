using System.Collections.Generic;
using System.Text.RegularExpressions;
using Crosstales.BWF.Model;
using Crosstales.BWF.Util;

namespace Crosstales.BWF.Provider
{
	public abstract class DomainProvider : BaseProvider
	{
		protected List<Domains> domains = new List<Domains>();

		private const string domainRegexStart = "\\b{0,1}((ht|f)tp(s?)\\:\\/\\/)?[\\w\\-\\.\\@]*[\\.]";

		private const string domainRegexEnd = "(:\\d{1,5})?(\\/|\\b)";

		private Dictionary<string, Regex> domainsRegex = new Dictionary<string, Regex>();

		private Dictionary<string, List<Regex>> debugDomainsRegex = new Dictionary<string, List<Regex>>();

		public Dictionary<string, Regex> DomainsRegex
		{
			get
			{
				return domainsRegex;
			}
			protected set
			{
				domainsRegex = value;
			}
		}

		public Dictionary<string, List<Regex>> DebugDomainsRegex
		{
			get
			{
				return debugDomainsRegex;
			}
			protected set
			{
				debugDomainsRegex = value;
			}
		}

		public override void Load()
		{
			if (ClearOnLoad)
			{
				domains.Clear();
			}
		}

		protected override void init()
		{
			DomainsRegex.Clear();
			if (Constants.DEBUG_DOMAINS)
			{
			}
			foreach (Domains domain in domains)
			{
				if (Constants.DEBUG_DOMAINS)
				{
					List<Regex> list = new List<Regex>(domain.DomainList.Count);
					foreach (string domain2 in domain.DomainList)
					{
						list.Add(new Regex("\\b{0,1}((ht|f)tp(s?)\\:\\/\\/)?[\\w\\-\\.\\@]*[\\.]" + domain2 + "(:\\d{1,5})?(\\/|\\b)", RegexOption1 | RegexOption2 | RegexOption3 | RegexOption4 | RegexOption5));
					}
					if (!DebugDomainsRegex.ContainsKey(domain.Source.Name))
					{
						DebugDomainsRegex.Add(domain.Source.Name, list);
					}
				}
				else if (!DomainsRegex.ContainsKey(domain.Source.Name))
				{
					DomainsRegex.Add(domain.Source.Name, new Regex("\\b{0,1}((ht|f)tp(s?)\\:\\/\\/)?[\\w\\-\\.\\@]*[\\.](" + string.Join("|", domain.DomainList.ToArray()) + ")(:\\d{1,5})?(\\/|\\b)", RegexOption1 | RegexOption2 | RegexOption3 | RegexOption4 | RegexOption5));
				}
				if (!Constants.DEBUG_DOMAINS)
				{
				}
			}
			base.isReady = true;
		}
	}
}
