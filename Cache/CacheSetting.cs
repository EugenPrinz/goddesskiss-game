using System;
using UnityEngine;

namespace Cache
{
	[Serializable]
	public class CacheSetting
	{
		[Serializable]
		public class CacheCullingOption
		{
			[Range(0f, 100f)]
			public int cullAbove = 50;

			[Range(0f, 100f)]
			public int cullRetain = 30;

			public int cullDelay = 30;

			public int cullMaxPerPass = 5;
		}

		public string groupName;

		public bool refreshCacheOnStart;

		public bool loadAssetbundleOnStart;

		public bool dontDestroyOnLoad;

		[HideInInspector]
		public bool enableCache;
	}
}
