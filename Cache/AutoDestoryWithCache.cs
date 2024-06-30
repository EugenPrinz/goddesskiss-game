using System.Collections;
using UnityEngine;

namespace Cache
{
	public class AutoDestoryWithCache : MonoBehaviour
	{
		public CacheItem cacheItem;

		public string cacheName;

		[Range(0.5f, 10f)]
		public float lifeTime = 5f;

		protected AbstractCache _cache;

		private IEnumerator Release()
		{
			yield return new WaitForSeconds(lifeTime);
			if (!cacheItem)
			{
				Object.DestroyImmediate(base.gameObject);
			}
			else if (string.IsNullOrEmpty(cacheName))
			{
				Object.DestroyImmediate(base.gameObject);
			}
			else if (_cache == null)
			{
				Object.DestroyImmediate(base.gameObject);
			}
			else
			{
				_cache.Release(cacheItem);
			}
		}

		private void Awake()
		{
			_cache = CacheManager.instance.GetCache(cacheName);
		}

		private void OnEnable()
		{
			StartCoroutine(Release());
		}
	}
}
