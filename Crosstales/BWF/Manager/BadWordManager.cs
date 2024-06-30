using System.Collections.Generic;
using System.Text.RegularExpressions;
using Crosstales.BWF.Filter;
using Crosstales.BWF.Model;
using Crosstales.BWF.Provider;
using Crosstales.BWF.Util;
using UnityEngine;

namespace Crosstales.BWF.Manager
{
	[DisallowMultipleComponent]
	[HelpURL("http://www.crosstales.com/en/assets/badwordfilter/api/class_crosstales_1_1_bad_word_1_1_bad_word_manager.html")]
	public class BadWordManager : BaseManager
	{
		[Header("Bad Word Provider")]
		[Tooltip("List of all left-to-right providers.")]
		public List<BadWordProvider> BadWordProviderLTR;

		[Tooltip("List of all right-to-left providers.")]
		public List<BadWordProvider> BadWordProviderRTL;

		[Header("Settings")]
		[Tooltip("Replace characters for bad words (default: *).")]
		public string ReplaceChars = "*";

		[Tooltip("Defines how exact the match will be. Without fuzziness, only exact matches are detected. Important: “Fuzzy” is much more performance consuming – so be careful (default: off).")]
		public bool Fuzzy;

		private GameObject root;

		private static bool initalized;

		private static BadWordFilter filter;

		private static BadWordManager manager;

		private static bool loggedFilterIsNull;

		private static bool loggedOnlyOneInstance;

		private const string clazz = "BadWordManager";

		public static BadWordFilter Filter => filter;

		public static bool isReady
		{
			get
			{
				bool result = false;
				if (filter != null)
				{
					result = filter.isReady;
				}
				else
				{
					logFilterIsNull("BadWordManager");
				}
				return result;
			}
		}

		public static List<Source> Sources
		{
			get
			{
				List<Source> result = new List<Source>();
				if (filter != null)
				{
					result = filter.Sources;
				}
				else
				{
					logFilterIsNull("BadWordManager");
				}
				return result;
			}
		}

		public void OnEnable()
		{
			if (Helper.isEditorMode || !initalized)
			{
				manager = this;
				Load();
				root = base.transform.root.gameObject;
				if (!Helper.isEditorMode && Constants.DONT_DESTROY_ON_LOAD)
				{
					Object.DontDestroyOnLoad(root);
					initalized = true;
				}
			}
			else if (!Helper.isEditorMode && Constants.DONT_DESTROY_ON_LOAD)
			{
				if (!loggedOnlyOneInstance)
				{
					loggedOnlyOneInstance = true;
				}
				Object.Destroy(base.transform.root.gameObject, 0.2f);
			}
		}

		public static void Load()
		{
			if (!(manager != null))
			{
				return;
			}
			filter = new BadWordFilter(manager.BadWordProviderLTR, manager.BadWordProviderRTL, manager.ReplaceChars, manager.Fuzzy, manager.MarkPrefix, manager.MarkPostfix);
			if (!Application.isPlaying || RemoteObjectManager.instance.localUser.badWords == null)
			{
				return;
			}
			Dictionary<string, List<string>>.Enumerator enumerator = RemoteObjectManager.instance.localUser.badWords.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Value.Count > 0)
				{
					string[] value = enumerator.Current.Value.ToArray();
					string text = string.Join("|", value);
					string text2 = "(?<![\\w\\d])";
					string text3 = "s?(?![\\w\\d])";
					string text4 = "\\b\\w*";
					string text5 = "\\w*\\b";
					Regex regex = new Regex(text2 + "(" + text + ")" + text3, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
					Regex regex2 = new Regex(text4 + "(" + text + ")" + text5, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
					if (enumerator.Current.Key == "ko")
					{
						filter.AddExactBadWord(enumerator.Current.Key, regex2);
					}
					else
					{
						filter.AddExactBadWord(enumerator.Current.Key, regex);
					}
					filter.AddFuzzyBadWord(enumerator.Current.Key, regex2);
				}
			}
			RemoteObjectManager.instance.localUser.badWords = null;
		}

		public static bool Contains(string testString, params string[] sources)
		{
			bool result = false;
			if (filter != null)
			{
				result = filter.Contains(testString, sources);
			}
			else
			{
				logFilterIsNull("BadWordManager");
			}
			return result;
		}

		public static void ContainsMT(out bool result, string testString, params string[] sources)
		{
			result = Contains(testString, sources);
		}

		public static List<string> GetAll(string testString, params string[] sources)
		{
			List<string> result = new List<string>();
			if (filter != null)
			{
				result = filter.GetAll(testString, sources);
			}
			else
			{
				logFilterIsNull("BadWordManager");
			}
			return result;
		}

		public static void GetAllMT(out List<string> result, string testString, params string[] sources)
		{
			result = GetAll(testString, sources);
		}

		public static string ReplaceAll(string testString, params string[] sources)
		{
			string result = testString;
			if (filter != null)
			{
				result = filter.ReplaceAll(testString, sources);
			}
			else
			{
				logFilterIsNull("BadWordManager");
			}
			return result;
		}

		public static void ReplaceAllMT(out string result, string testString, params string[] sources)
		{
			result = ReplaceAll(testString, sources);
		}

		public static string Replace(string text, List<string> badWords)
		{
			string result = text;
			if (filter != null)
			{
				result = filter.Replace(text, badWords);
			}
			else
			{
				logFilterIsNull("BadWordManager");
			}
			return result;
		}

		public static string Mark(string text, List<string> badWords, string prefix = "<b><color=red>", string postfix = "</color></b>")
		{
			string result = text;
			if (filter != null)
			{
				result = filter.Mark(text, badWords, prefix, postfix);
			}
			else
			{
				logFilterIsNull("BadWordManager");
			}
			return result;
		}

		public static string Unmark(string text, string prefix = "<b><color=red>", string postfix = "</color></b>")
		{
			string result = text;
			if (filter != null)
			{
				result = filter.Unmark(text, prefix, postfix);
			}
			else
			{
				logFilterIsNull("BadWordManager");
			}
			return result;
		}

		private static void logFilterIsNull(string clazz)
		{
			if (!loggedFilterIsNull)
			{
				loggedFilterIsNull = true;
			}
		}
	}
}
