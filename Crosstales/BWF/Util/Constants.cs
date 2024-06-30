using System;

namespace Crosstales.BWF.Util
{
	public static class Constants
	{
		public const string ASSET_NAME = "BWF PRO";

		public const string ASSET_VERSION = "2.7.2";

		public const int ASSET_BUILD = 272;

		public static readonly DateTime ASSET_CREATED = new DateTime(2015, 1, 3);

		public static readonly DateTime ASSET_CHANGED = new DateTime(2016, 10, 21);

		public const string ASSET_AUTHOR = "crosstales LLC";

		public const string ASSET_AUTHOR_URL = "http://www.crosstales.com";

		public const string ASSET_URL = "https://www.assetstore.unity3d.com/en/#!/content/26255";

		public const string ASSET_UPDATE_CHECK_URL = "http://www.crosstales.com/media/assets/bwf_versions.txt";

		public const string ASSET_CONTACT = "bwf@crosstales.com";

		public static readonly Guid ASSET_UID = new Guid("b11eebc0-525a-4d58-b33d-c0a9a728f3a9");

		public const string ASSET_MANUAL_URL = "http://www.crosstales.com/en/assets/badwordfilter/BadWordFilter-doc.pdf";

		public const string ASSET_API_URL = "http://goo.gl/QkE2sN";

		public const string ASSET_FORUM_URL = "http://goo.gl/Mj9XpS";

		public const string ASSET_CT_URL = "http://www.crosstales.com/en/assets/badwordfilter/";

		public const string MANAGER_SCENE_OBJECT_NAME = "BWF";

		private const string KEY_PREFIX = "BWF_CFG_";

		public const string KEY_ASSET_PATH = "BWF_CFG_ASSET_PATH";

		public const string KEY_DEBUG = "BWF_CFG_DEBUG";

		public const string KEY_DEBUG_BADWORDS = "BWF_CFG_DEBUG_BADWORDS";

		public const string KEY_DEBUG_DOMAINS = "BWF_CFG_DEBUG_DOMAINS";

		public const string KEY_UPDATE_CHECK = "BWF_CFG_UPDATE_CHECK";

		public const string KEY_UPDATE_OPEN_UAS = "BWF_CFG_UPDATE_OPEN_UAS";

		public const string KEY_PREFAB_AUTOLOAD = "BWF_CFG_PREFAB_AUTOLOAD";

		public const string KEY_UPDATE_DATE = "BWF_CFG_UPDATE_DATE";

		public const string DEFAULT_ASSET_PATH = "/crosstales/BadWordFilter/";

		public const bool DEFAULT_DEBUG = false;

		public const bool DEFAULT_DEBUG_BADWORDS = false;

		public const bool DEFAULT_DEBUG_DOMAINS = false;

		public const bool DEFAULT_UPDATE_CHECK = true;

		public const bool DEFAULT_UPDATE_OPEN_UAS = false;

		public const bool DEFAULT_DONT_DESTROY_ON_LOAD = true;

		public const bool DEFAULT_PREFAB_AUTOLOAD = false;

		public static string ASSET_PATH = "/crosstales/BadWordFilter/";

		public static bool DEBUG = false;

		public static bool DEBUG_BADWORDS = false;

		public static bool DEBUG_DOMAINS = false;

		public static bool UPDATE_CHECK = true;

		public static bool UPDATE_OPEN_UAS = false;

		public static bool DONT_DESTROY_ON_LOAD = true;

		public static bool PREFAB_AUTOLOAD = false;

		public static string PREFAB_SUBPATH = "Prefabs/";

		public static string TEXT_TOSTRING_START = " {";

		public static string TEXT_TOSTRING_END = "}";

		public static string TEXT_TOSTRING_DELIMITER = "', ";

		public static string TEXT_TOSTRING_DELIMITER_END = "'";

		public static string PREFAB_PATH => ASSET_PATH + PREFAB_SUBPATH;

		public static void Reset()
		{
			ASSET_PATH = "/crosstales/BadWordFilter/";
			DEBUG = false;
			DEBUG_BADWORDS = false;
			DEBUG_DOMAINS = false;
			UPDATE_CHECK = true;
			UPDATE_OPEN_UAS = false;
			DONT_DESTROY_ON_LOAD = true;
			PREFAB_AUTOLOAD = false;
		}
	}
}
