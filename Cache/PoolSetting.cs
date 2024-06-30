using System;
using UnityEngine;

namespace Cache
{
	[Serializable]
	public class PoolSetting
	{
		[Serializable]
		public class DefaultSetting
		{
			[Range(0f, 100f)]
			public int capacity = 5;
		}

		[Serializable]
		public class CullingSetting
		{
			public bool canCulling;

			[Range(0f, 100f)]
			public int cullAbove = 50;

			[Range(0f, 100f)]
			public int cullRetain = 30;

			public int cullDelay = 30;

			public int cullMaxPerPass = 5;
		}

		public DefaultSetting defaultSetting;

		public CullingSetting cullingSetting;

		[Range(0f, 100f)]
		public int capacity = 100;

		public bool useCacheItem;

		public bool parentOnCache = true;
	}
}
