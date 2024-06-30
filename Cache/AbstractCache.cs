using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cache
{
	[Serializable]
	public abstract class AbstractCache : MonoBehaviour
	{
		public CacheSetting setting;

		protected Dictionary<string, CacheElement> _dicElements;

		protected bool init;

		protected List<CacheElement> _usedElements;

		public string CacheName => base.name;

		public string GroupName => setting.groupName;

		public void AddCacheElement(CacheElement elem)
		{
			_dicElements.Add(elem.key, elem);
		}

		public void RemoveCacheElement(string key)
		{
			_dicElements.Remove(key);
		}

		public virtual void CleanUp()
		{
			if (!setting.dontDestroyOnLoad && _usedElements != null)
			{
				while (_usedElements.Count > 0)
				{
					_usedElements[0].sourcePrefab = null;
					_usedElements.RemoveAt(0);
				}
			}
		}

		protected virtual CacheElement GetEditElement(string key)
		{
			return null;
		}

		protected virtual GameObject LoadFromRes(string path)
		{
			return (GameObject)Resources.Load(path, typeof(GameObject));
		}

		protected virtual T LoadFromRes<T>(string path) where T : UnityEngine.Object
		{
			return (T)Resources.Load(path, typeof(T));
		}

		public virtual void SetElement(CacheElement elem)
		{
			if (_dicElements == null)
			{
				RefreshElementDict();
			}
			if (!_dicElements.ContainsKey(elem.key))
			{
				_dicElements.Add(elem.key, elem);
				if (elem.sourcePrefab != null)
				{
					_usedElements.Add(elem);
				}
			}
		}

		public virtual void SetElement(string key, GameObject sourcePrefab)
		{
			CacheElement cacheElement = new CacheElement();
			cacheElement.key = key;
			cacheElement.resPath = null;
			cacheElement.sourcePrefab = sourcePrefab;
			SetElement(cacheElement);
		}

		public virtual bool HasElement(string key)
		{
			if (_dicElements == null)
			{
				RefreshElementDict();
			}
			if (_dicElements.ContainsKey(key) && _dicElements[key].sourcePrefab != null)
			{
				return true;
			}
			return false;
		}

		public virtual CacheElement GetElement(string key)
		{
			CacheElement cacheElement = null;
			if (_dicElements == null)
			{
				RefreshElementDict();
			}
			if (!_dicElements.ContainsKey(key))
			{
				cacheElement = LoadAssetBundlePrefab(key);
				if (cacheElement == null)
				{
					return null;
				}
				if (cacheElement.sourcePrefab == null)
				{
					return null;
				}
				_usedElements.Add(cacheElement);
				return cacheElement;
			}
			cacheElement = _dicElements[key];
			if (cacheElement.sourcePrefab == null)
			{
				LoadAssetBundlePrefab(key, cacheElement);
				if (cacheElement.sourcePrefab == null)
				{
					if (string.IsNullOrEmpty(cacheElement.resPath))
					{
						return null;
					}
					cacheElement.sourcePrefab = LoadFromRes(cacheElement.resPath);
				}
				if (cacheElement.sourcePrefab == null)
				{
					return null;
				}
				_usedElements.Add(cacheElement);
			}
			return cacheElement;
		}

		public virtual GameObject Create(CacheElement elem, Vector3 position, Quaternion rotation, Transform parent = null)
		{
			return null;
		}

		public virtual GameObject Create(string key, Vector3 position, Quaternion rotation, Transform parent = null)
		{
			CacheElement element = GetElement(key);
			if (element == null)
			{
				return null;
			}
			return Create(element, position, rotation, parent);
		}

		public T Create<T>(string key, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
		{
			GameObject gameObject = Create(key, position, rotation, parent);
			return (!(gameObject != null)) ? ((T)null) : gameObject.GetComponent<T>();
		}

		public T Create<T>(string key, Transform parent = null) where T : Component
		{
			return Create<T>(key, Vector3.zero, Quaternion.identity, parent);
		}

		public GameObject Create(string key, Transform parent = null)
		{
			return Create(key, Vector3.zero, Quaternion.identity, parent);
		}

		public GameObject Create(CacheElement elem, Transform parent = null)
		{
			return Create(elem, Vector3.zero, Quaternion.identity, parent);
		}

		public T Create<T>(CacheElement elem, Transform parent = null) where T : Component
		{
			GameObject gameObject = Create(elem, Vector3.zero, Quaternion.identity, parent);
			return (!(gameObject != null)) ? ((T)null) : gameObject.GetComponent<T>();
		}

		public virtual void Release(int id, GameObject obj)
		{
			UnityEngine.Object.DestroyImmediate(obj);
		}

		public void Release(ICacheItem cacheItem)
		{
			Release(cacheItem.CacheID, cacheItem.CacheObj);
		}

		public void Release(string key, GameObject obj)
		{
			if (_dicElements == null)
			{
				UnityEngine.Object.DestroyImmediate(obj);
				return;
			}
			if (!_dicElements.ContainsKey(key))
			{
				UnityEngine.Object.DestroyImmediate(obj);
				return;
			}
			CacheElement cacheElement = _dicElements[key];
			if (cacheElement.ID == 0)
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
			else
			{
				Release(cacheElement.ID, obj);
			}
		}

		protected abstract void RefreshElementDict();

		protected abstract void LoadElementFromAssetBundle();

		protected abstract CacheElement LoadAssetBundlePrefab(string key, CacheElement targetElement = null);

		public virtual void Awake()
		{
			CacheManager.instance.SetCache(this);
		}

		public virtual void Start()
		{
			Init();
		}

		public virtual void Init()
		{
			if (!init)
			{
				if (setting.loadAssetbundleOnStart)
				{
					LoadElementFromAssetBundle();
				}
				if (setting.refreshCacheOnStart)
				{
					RefreshElementDict();
				}
				if (setting.dontDestroyOnLoad)
				{
					UnityEngine.Object.DontDestroyOnLoad(this);
				}
				init = true;
			}
		}
	}
}
