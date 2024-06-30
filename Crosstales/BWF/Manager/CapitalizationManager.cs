using System.Collections.Generic;
using Crosstales.BWF.Filter;
using Crosstales.BWF.Util;
using UnityEngine;

namespace Crosstales.BWF.Manager
{
	[DisallowMultipleComponent]
	[HelpURL("http://www.crosstales.com/en/assets/badwordfilter/api/class_crosstales_1_1_bad_word_1_1_capitalization_manager.html")]
	public class CapitalizationManager : BaseManager
	{
		[Header("Settings")]
		[Tooltip("Defines the number of allowed capital letters in a row. (default: 3).")]
		public int CapitalizationCharsNumber = 3;

		private static bool initalized;

		private static CapitalizationFilter filter;

		private static CapitalizationManager manager;

		private static bool loggedFilterIsNull;

		private static bool loggedOnlyOneInstance;

		private const string clazz = "CapitalizationManager";

		public static CapitalizationFilter Filter => filter;

		public static bool isReady => filter.isReady;

		public void OnEnable()
		{
			if (Helper.isEditorMode || !initalized)
			{
				manager = this;
				Load();
				if (!Helper.isEditorMode && Constants.DONT_DESTROY_ON_LOAD)
				{
					Object.DontDestroyOnLoad(base.transform.root.gameObject);
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

		public void OnValidate()
		{
			if (CapitalizationCharsNumber < 2)
			{
				CapitalizationCharsNumber = 2;
			}
		}

		public static void Load()
		{
			if (manager != null)
			{
				filter = new CapitalizationFilter(manager.CapitalizationCharsNumber, manager.MarkPrefix, manager.MarkPostfix);
			}
		}

		public static bool Contains(string testString)
		{
			bool result = false;
			if (filter != null)
			{
				result = filter.Contains(testString);
			}
			else
			{
				logFilterIsNull("CapitalizationManager");
			}
			return result;
		}

		public static void ContainsMT(out bool result, string testString)
		{
			result = Contains(testString);
		}

		public static List<string> GetAll(string testString)
		{
			List<string> result = new List<string>();
			if (filter != null)
			{
				result = filter.GetAll(testString);
			}
			else
			{
				logFilterIsNull("CapitalizationManager");
			}
			return result;
		}

		public static void GetAllMT(out List<string> result, string testString)
		{
			result = GetAll(testString);
		}

		public static string ReplaceAll(string testString)
		{
			string result = testString;
			if (filter != null)
			{
				result = filter.ReplaceAll(testString);
			}
			else
			{
				logFilterIsNull("CapitalizationManager");
			}
			return result;
		}

		public static void ReplaceAllMT(out string result, string testString)
		{
			result = ReplaceAll(testString);
		}

		public static string Replace(string text, List<string> capitalWords)
		{
			string result = text;
			if (filter != null)
			{
				result = filter.Replace(text, capitalWords);
			}
			else
			{
				logFilterIsNull("CapitalizationManager");
			}
			return result;
		}

		public static string Mark(string text, List<string> capitalWords, string prefix = "<b><color=red>", string postfix = "</color></b>")
		{
			string result = text;
			if (filter != null)
			{
				result = filter.Mark(text, capitalWords, prefix, postfix);
			}
			else
			{
				logFilterIsNull("CapitalizationManager");
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
				logFilterIsNull("CapitalizationManager");
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
