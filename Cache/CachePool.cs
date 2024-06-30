using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cache
{
	[Serializable]
	public class CachePool
	{
		[SerializeField]
		private GameObject sourcePrefab;

		public int capacity;

		public bool parentOnCache;

		private Stack<GameObject> cache;

		private Transform parentTransform;

		private Transform sourceTransform;

		protected int _usedCount;

		public int ID { get; private set; }

		public int cachedCount => cache.Count;

		public int usedCount
		{
			get
			{
				return _usedCount;
			}
			set
			{
				_usedCount = value;
			}
		}

		public CachePool(GameObject prefab, Transform rootTransform = null, int capacity = 0)
		{
			this.capacity = capacity;
			sourcePrefab = prefab;
			sourceTransform = sourcePrefab.transform;
			Init(rootTransform);
		}

		public bool Init(Transform rootTransform)
		{
			if (sourcePrefab != null)
			{
				cache = new Stack<GameObject>();
				parentTransform = rootTransform;
				ID = sourcePrefab.GetInstanceID();
				return true;
			}
			return false;
		}

		public void ClearCache()
		{
			while (cache.Count > 0)
			{
				UnityEngine.Object.DestroyImmediate(cache.Pop());
			}
		}

		public T ObjectInstantiate<T>(Transform parent = null, bool useCacheItem = false) where T : Component
		{
			return ObjectInstantiate<T>(Vector3.zero, Quaternion.identity, parent, useCacheItem);
		}

		public T ObjectInstantiate<T>(Vector3 position, Quaternion rotation, Transform parent = null, bool useCacheItem = false) where T : Component
		{
			GameObject gameObject = ObjectInstantiate(position, rotation, parent, useCacheItem);
			return (!(gameObject != null)) ? ((T)null) : gameObject.GetComponent<T>();
		}

		public GameObject ObjectInstantiate(Transform parent = null, bool useCacheItem = false)
		{
			return ObjectInstantiate(Vector3.zero, Quaternion.identity, parent, useCacheItem);
		}

		public void ObjectRemove(int count)
		{
			if (count > cache.Count)
			{
				count = cache.Count;
			}
			for (int i = 0; i < count; i++)
			{
				GameObject gameObject = cache.Pop();
				if (gameObject != null)
				{
					UnityEngine.Object.DestroyImmediate(gameObject);
				}
			}
		}

		public GameObject ObjectInstantiate(Vector3 position, Quaternion rotation, Transform parent = null, bool useCacheItem = false)
		{
			_usedCount++;
			GameObject gameObject;
			Transform transform;
			while (cache.Count > 0)
			{
				gameObject = cache.Pop();
				if (gameObject != null)
				{
					transform = gameObject.transform;
					transform.parent = parent;
					transform.localPosition = sourceTransform.localPosition + position;
					transform.localRotation = sourceTransform.localRotation * rotation;
					transform.localScale = sourceTransform.localScale;
					gameObject.SetActive(value: true);
					return gameObject;
				}
			}
			gameObject = UnityEngine.Object.Instantiate(sourcePrefab);
			transform = gameObject.transform;
			transform.parent = parent;
			transform.localPosition = sourceTransform.localPosition + position;
			transform.localRotation = sourceTransform.localRotation * rotation;
			transform.localScale = sourceTransform.localScale;
			if (useCacheItem)
			{
				ICacheItem cacheItem = gameObject.GetComponent<ICacheItem>();
				if (cacheItem == null)
				{
					cacheItem = gameObject.AddComponent<CacheItem>();
				}
				if (cacheItem != null)
				{
					cacheItem.CacheID = ID;
				}
			}
			return gameObject;
		}

		public void ObjectDestroy<T>(T obj) where T : Component
		{
			if (obj != null)
			{
				ObjectDestroy(obj.gameObject);
			}
		}

		public void ObjectDestroy(GameObject obj)
		{
			if (!(obj != null))
			{
				return;
			}
			_usedCount++;
			if (cache.Count < capacity)
			{
				obj.SetActive(value: false);
				cache.Push(obj);
				if (parentOnCache)
				{
					obj.transform.parent = parentTransform;
				}
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
		}
	}
}
