using System;
using UnityEngine;

namespace Cache
{
	[Serializable]
	public class CacheElement
	{
		public string key;

		public string resPath;

		public GameObject sourcePrefab;

		protected int _Id;

		public int ID
		{
			get
			{
				if (_Id == 0 && sourcePrefab != null)
				{
					_Id = sourcePrefab.GetInstanceID();
				}
				return _Id;
			}
		}

		public static CacheElement Create(string key, GameObject prefab, string resPath = null)
		{
			CacheElement cacheElement = new CacheElement();
			cacheElement.key = key;
			cacheElement.sourcePrefab = prefab;
			cacheElement.resPath = resPath;
			return cacheElement;
		}
	}
}
