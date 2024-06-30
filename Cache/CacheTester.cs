using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cache
{
	public class CacheTester : MonoBehaviour
	{
		[Serializable]
		public class CachedObj
		{
			public int id;

			public GameObject obj;
		}

		public string cacheName;

		public AbstractCache cache;

		public string cacheKey;

		public List<CachedObj> createdObjects;

		public void Create()
		{
			if (!string.IsNullOrEmpty(cacheName))
			{
				cache = CacheManager.instance.GetCache(cacheName);
			}
			if (cache is UnitCache)
			{
				UnitRenderer unitRenderer = ((UnitCache)cache).Create(cacheKey, base.transform);
				if (unitRenderer != null)
				{
					CachedObj cachedObj = new CachedObj();
					cachedObj.id = unitRenderer.cacheID;
					cachedObj.obj = unitRenderer.gameObject;
					createdObjects.Add(cachedObj);
				}
			}
			else
			{
				CacheElement element = cache.GetElement(cacheKey);
				if (element != null)
				{
					CachedObj cachedObj2 = new CachedObj();
					cachedObj2.id = element.ID;
					cachedObj2.obj = cache.Create(element, base.transform);
					createdObjects.Add(cachedObj2);
				}
			}
		}

		public void Release()
		{
			if (createdObjects.Count > 0)
			{
				CachedObj cachedObj = createdObjects[0];
				createdObjects.RemoveAt(0);
				cache.Release(cachedObj.id, cachedObj.obj);
			}
		}
	}
}
