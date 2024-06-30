using System.Collections.Generic;
using System.Linq;
using Crosstales.BWF.Filter;
using Crosstales.BWF.Manager;
using Crosstales.BWF.Model;
using Crosstales.BWF.Util;
using UnityEngine;

namespace Crosstales.BWF
{
	[ExecuteInEditMode]
	public class BWFManager : MonoBehaviour
	{
		private GameObject root;

		public static bool isReady => BadWordManager.isReady && DomainManager.isReady && CapitalizationManager.isReady && PunctuationManager.isReady;

		public void OnEnable()
		{
			root = base.transform.root.gameObject;
		}

		public void Update()
		{
			if (Helper.isEditorMode && root != null)
			{
				root.name = "BWF";
			}
		}

		public static void Load(ManagerMask mask = ManagerMask.All)
		{
			if ((mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All)
			{
				BadWordManager.Load();
			}
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All)
			{
				DomainManager.Load();
			}
			if ((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization || (mask & ManagerMask.All) == ManagerMask.All)
			{
				CapitalizationManager.Load();
			}
			if ((mask & ManagerMask.Punctuation) == ManagerMask.Punctuation || (mask & ManagerMask.All) == ManagerMask.All)
			{
				PunctuationManager.Load();
			}
		}

		public static BaseFilter Filter(ManagerMask mask = ManagerMask.BadWord)
		{
			BaseFilter filter = BadWordManager.Filter;
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain)
			{
				filter = DomainManager.Filter;
			}
			if ((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization)
			{
				filter = CapitalizationManager.Filter;
			}
			if ((mask & ManagerMask.Punctuation) == ManagerMask.Punctuation)
			{
				filter = PunctuationManager.Filter;
			}
			return filter;
		}

		public static List<Source> Sources(ManagerMask mask = ManagerMask.All)
		{
			List<Source> list = new List<Source>(30);
			if ((mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(BadWordManager.Sources);
			}
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(DomainManager.Sources);
			}
			return (from x in list.Distinct()
				orderby x.Name
				select x).ToList();
		}

		public static bool Contains(string testString, ManagerMask mask = ManagerMask.All, params string[] sources)
		{
			return (((mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All) && BadWordManager.Contains(testString, sources)) || (((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All) && DomainManager.Contains(testString, sources)) || (((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization || (mask & ManagerMask.All) == ManagerMask.All) && CapitalizationManager.Contains(testString)) || (((mask & ManagerMask.Punctuation) == ManagerMask.Punctuation || (mask & ManagerMask.All) == ManagerMask.All) && PunctuationManager.Contains(testString));
		}

		public static void ContainsMT(out bool result, string testString, ManagerMask mask = ManagerMask.All, params string[] sources)
		{
			result = (((mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All) && BadWordManager.Contains(testString, sources)) || (((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All) && DomainManager.Contains(testString, sources)) || (((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization || (mask & ManagerMask.All) == ManagerMask.All) && CapitalizationManager.Contains(testString)) || (((mask & ManagerMask.Punctuation) == ManagerMask.Punctuation || (mask & ManagerMask.All) == ManagerMask.All) && PunctuationManager.Contains(testString));
		}

		public static List<string> GetAll(string testString, ManagerMask mask = ManagerMask.All, params string[] sources)
		{
			List<string> list = new List<string>();
			if ((mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(BadWordManager.GetAll(testString, sources));
			}
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(DomainManager.GetAll(testString, sources));
			}
			if ((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(CapitalizationManager.GetAll(testString));
			}
			if ((mask & ManagerMask.Punctuation) == ManagerMask.Punctuation || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(PunctuationManager.GetAll(testString));
			}
			return (from x in list.Distinct()
				orderby x
				select x).ToList();
		}

		public static void GetAllMT(out List<string> result, string testString, ManagerMask mask = ManagerMask.All, params string[] sources)
		{
			List<string> list = new List<string>();
			if ((mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(BadWordManager.GetAll(testString, sources));
			}
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(DomainManager.GetAll(testString, sources));
			}
			if ((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(CapitalizationManager.GetAll(testString));
			}
			if ((mask & ManagerMask.Punctuation) == ManagerMask.Punctuation || (mask & ManagerMask.All) == ManagerMask.All)
			{
				list.AddRange(PunctuationManager.GetAll(testString));
			}
			result = (from x in list.Distinct()
				orderby x
				select x).ToList();
		}

		public static string ReplaceAll(string testString, ManagerMask mask = ManagerMask.All, params string[] sources)
		{
			string text = testString ?? string.Empty;
			if ((mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All)
			{
				text = BadWordManager.ReplaceAll(text, sources);
			}
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All)
			{
				text = DomainManager.ReplaceAll(text, sources);
			}
			if ((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization || (mask & ManagerMask.All) == ManagerMask.All)
			{
				text = CapitalizationManager.ReplaceAll(text);
			}
			if ((mask & ManagerMask.Punctuation) == ManagerMask.Punctuation || (mask & ManagerMask.All) == ManagerMask.All)
			{
				text = PunctuationManager.ReplaceAll(text);
			}
			return text;
		}

		public static void ReplaceAllMT(out string result, string testString, ManagerMask mask = ManagerMask.All, params string[] sources)
		{
			result = testString ?? string.Empty;
			if ((mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All)
			{
				result = BadWordManager.ReplaceAll(result, sources);
			}
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All)
			{
				result = DomainManager.ReplaceAll(result, sources);
			}
			if ((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization || (mask & ManagerMask.All) == ManagerMask.All)
			{
				result = CapitalizationManager.ReplaceAll(result);
			}
			if ((mask & ManagerMask.Punctuation) == ManagerMask.Punctuation || (mask & ManagerMask.All) == ManagerMask.All)
			{
				result = PunctuationManager.ReplaceAll(result);
			}
		}

		public static string Replace(string text, List<string> unwantedWords, ManagerMask mask = ManagerMask.All)
		{
			string text2 = text ?? string.Empty;
			if ((mask & ManagerMask.BadWord) == ManagerMask.BadWord || (mask & ManagerMask.All) == ManagerMask.All)
			{
				text2 = BadWordManager.Replace(text2, unwantedWords);
			}
			if ((mask & ManagerMask.Domain) == ManagerMask.Domain || (mask & ManagerMask.All) == ManagerMask.All)
			{
				text2 = DomainManager.Replace(text2, unwantedWords);
			}
			if ((mask & ManagerMask.Capitalization) == ManagerMask.Capitalization || (mask & ManagerMask.All) == ManagerMask.All)
			{
				text2 = CapitalizationManager.Replace(text2, unwantedWords);
			}
			if ((mask & ManagerMask.Punctuation) == ManagerMask.Punctuation || (mask & ManagerMask.All) == ManagerMask.All)
			{
				text2 = PunctuationManager.Replace(text2, unwantedWords);
			}
			return text2;
		}

		public static string Mark(string text, List<string> unwantedWords, string prefix = "<b><color=red>", string postfix = "</color></b>")
		{
			string text2 = text ?? string.Empty;
			return BadWordManager.Mark(text2, unwantedWords, prefix, postfix);
		}

		public static string Unmark(string text, string prefix = "<b><color=red>", string postfix = "</color></b>")
		{
			string text2 = text ?? string.Empty;
			return BadWordManager.Unmark(text2, prefix, postfix);
		}
	}
}
