using System;
using UnityEngine;

namespace Cache
{
	[Serializable]
	public class CacheWithPoolElement : CacheElement
	{
		[Range(0f, 100f)]
		public int capacity;

		public static CacheWithPoolElement Create(string key, GameObject prefab, int capacity, string resPath = null)
		{
			CacheWithPoolElement cacheWithPoolElement = new CacheWithPoolElement();
			cacheWithPoolElement.key = key;
			cacheWithPoolElement.sourcePrefab = prefab;
			cacheWithPoolElement.capacity = capacity;
			cacheWithPoolElement.resPath = resPath;
			return cacheWithPoolElement;
		}
	}
}
