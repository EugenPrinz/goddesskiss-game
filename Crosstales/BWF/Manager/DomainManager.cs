using System.Collections.Generic;
using Crosstales.BWF.Filter;
using Crosstales.BWF.Model;
using Crosstales.BWF.Provider;
using Crosstales.BWF.Util;
using UnityEngine;

namespace Crosstales.BWF.Manager
{
	[DisallowMultipleComponent]
	[HelpURL("http://www.crosstales.com/en/assets/badwordfilter/api/class_crosstales_1_1_bad_word_1_1_domain_manager.html")]
	public class DomainManager : BaseManager
	{
		[Header("Domain Provider")]
		[Tooltip("List of all domain providers.")]
		public List<DomainProvider> DomainProvider;

		[Header("Settings")]
		[Tooltip("Replace characters for domains (default: *).")]
		public string ReplaceChars = "*";

		private static bool initalized;

		private static DomainFilter filter;

		private static DomainManager manager;

		private static bool loggedFilterIsNull;

		private static bool loggedOnlyOneInstance;

		private const string clazz = "DomainManager";

		public static DomainFilter Filter => filter;

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
					logFilterIsNull("DomainManager");
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
					logFilterIsNull("DomainManager");
				}
				return result;
			}
		}

		public void OnEnable()
		{
			if (Helper.isEditorMode || (!initalized && Constants.DONT_DESTROY_ON_LOAD))
			{
				manager = this;
				Load();
				if (!Helper.isEditorMode && Constants.DONT_DESTROY_ON_LOAD)
				{
					Object.DontDestroyOnLoad(base.transform.root.gameObject);
					initalized = true;
				}
			}
			else if (!Helper.isEditorMode)
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
			if (manager != null)
			{
				filter = new DomainFilter(manager.DomainProvider, manager.ReplaceChars, manager.MarkPrefix, manager.MarkPostfix);
			}
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
				logFilterIsNull("DomainManager");
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
				logFilterIsNull("DomainManager");
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
				logFilterIsNull("DomainManager");
			}
			return result;
		}

		public static void ReplaceAllMT(out string result, string testString, params string[] sources)
		{
			result = ReplaceAll(testString, sources);
		}

		public static string Replace(string text, List<string> domains)
		{
			string result = text;
			if (filter != null)
			{
				result = filter.Replace(text, domains);
			}
			else
			{
				logFilterIsNull("DomainManager");
			}
			return result;
		}

		public static string Mark(string text, List<string> domains, string prefix = "<b><color=red>", string postfix = "</color></b>")
		{
			string result = text;
			if (filter != null)
			{
				result = filter.Mark(text, domains, prefix, postfix);
			}
			else
			{
				logFilterIsNull("DomainManager");
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
				logFilterIsNull("DomainManager");
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
