using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cache
{
	public abstract class AbstractCacheWithPool : AbstractCache
	{
		public PoolSetting poolSetting;

		[SerializeField]
		protected int cachedCount;

		[SerializeField]
		protected bool cullingActive;

		protected Dictionary<int, CachePool> _cachePool;

		public PoolSetting.CullingSetting cullingSetting => poolSetting.cullingSetting;

		public override void Awake()
		{
			cachedCount = 0;
			cullingActive = false;
			_cachePool = new Dictionary<int, CachePool>();
			base.Awake();
		}

		protected virtual IEnumerator Culling()
		{
			CachePool pool2 = null;
			List<CachePool> pools = new List<CachePool>();
			Dictionary<int, CachePool>.Enumerator itr = _cachePool.GetEnumerator();
			while (itr.MoveNext())
			{
				pool2 = itr.Current.Value;
				pool2.usedCount = 0;
				if (pool2.cachedCount > 0)
				{
					pools.Add(itr.Current.Value);
				}
			}
			pools.Sort(delegate(CachePool a, CachePool b)
			{
				int result = a.usedCount - b.usedCount;
				a.usedCount = 0;
				return result;
			});
			while (cachedCount > cullingSetting.cullRetain)
			{
				pool2 = null;
				while (pools.Count > 0)
				{
					pool2 = pools[0];
					if (pool2.cachedCount > 0)
					{
						break;
					}
					pool2 = null;
					pools.RemoveAt(0);
				}
				if (pool2 == null)
				{
					break;
				}
				for (int i = 0; i < cullingSetting.cullMaxPerPass; i++)
				{
					if (cachedCount <= cullingSetting.cullRetain)
					{
						break;
					}
					if (pool2.cachedCount <= 0)
					{
						pool2 = null;
						pools.RemoveAt(0);
						while (pools.Count > 0)
						{
							pool2 = pools[0];
							if (pool2.cachedCount > 0)
							{
								break;
							}
							pool2 = null;
							pools.RemoveAt(0);
						}
						if (pool2 == null)
						{
							break;
						}
					}
					pool2.ObjectRemove(1);
					cachedCount--;
				}
				yield return new WaitForSeconds(cullingSetting.cullDelay);
			}
			cullingActive = false;
			yield return null;
		}

		public override void Release(int id, GameObject obj)
		{
			if (!cullingActive && cullingSetting.canCulling && cachedCount > cullingSetting.cullAbove)
			{
				cullingActive = true;
				StartCoroutine(Culling());
			}
			if (cachedCount >= poolSetting.capacity)
			{
				Object.DestroyImmediate(obj);
				return;
			}
			if (!_cachePool.ContainsKey(id))
			{
				Object.DestroyImmediate(obj);
				return;
			}
			CachePool cachePool = _cachePool[id];
			int num = cachePool.cachedCount;
			cachePool.ObjectDestroy(obj);
			if (num != cachePool.cachedCount)
			{
				cachedCount++;
			}
		}

		public void Release(ICacheItem cacheItem, GameObject obj)
		{
			Release(cacheItem.CacheID, obj);
		}

		public override GameObject Create(CacheElement elem, Vector3 position, Quaternion rotation, Transform parent = null)
		{
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			CacheWithPoolElement cacheWithPoolElement = (CacheWithPoolElement)elem;
			if (!setting.enableCache)
			{
				gameObject2 = elem.sourcePrefab;
				gameObject = Object.Instantiate(gameObject2);
				Transform transform = gameObject.transform;
				transform.SetParent(parent, worldPositionStays: false);
				transform.localPosition = gameObject2.transform.localPosition + position;
				transform.localRotation = gameObject2.transform.localRotation * rotation;
				transform.localScale = gameObject2.transform.localScale;
				if (poolSetting.useCacheItem)
				{
					ICacheItem cacheItem = gameObject.GetComponent<ICacheItem>();
					if (cacheItem == null)
					{
						cacheItem = gameObject.AddComponent<CacheItem>();
					}
					if (cacheItem != null)
					{
						cacheItem.CacheID = elem.ID;
					}
				}
				elem.sourcePrefab = null;
				gameObject.SetActive(value: true);
			}
			else
			{
				CachePool cachePool = null;
				if (_cachePool.ContainsKey(cacheWithPoolElement.ID))
				{
					cachePool = _cachePool[cacheWithPoolElement.ID];
				}
				if (cachePool == null)
				{
					if (cacheWithPoolElement.sourcePrefab == null)
					{
						return null;
					}
					cachePool = new CachePool(cacheWithPoolElement.sourcePrefab, base.transform, cacheWithPoolElement.capacity);
					cachePool.parentOnCache = poolSetting.parentOnCache;
					_cachePool.Add(cachePool.ID, cachePool);
				}
				int num = cachePool.cachedCount;
				gameObject = cachePool.ObjectInstantiate(position, rotation, parent, poolSetting.useCacheItem);
				if (num != cachePool.cachedCount)
				{
					cachedCount--;
				}
			}
			return gameObject;
		}

		public override void CleanUp()
		{
			if (setting.dontDestroyOnLoad)
			{
				return;
			}
			if (cullingActive)
			{
				StopAllCoroutines();
			}
			if (_usedElements != null)
			{
				while (_usedElements.Count > 0)
				{
					_usedElements[0].sourcePrefab = null;
					_usedElements.RemoveAt(0);
				}
			}
			Dictionary<int, CachePool>.Enumerator enumerator = _cachePool.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CachePool value = enumerator.Current.Value;
				value.ClearCache();
			}
			_cachePool = new Dictionary<int, CachePool>();
		}
	}
}
