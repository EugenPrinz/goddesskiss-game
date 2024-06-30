using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cache
{
	public class CacheManager : AbstractCacheWithoutPool
	{
		[Serializable]
		public class ManagerSetting
		{
			public bool LoadCacheOnStart = true;

			public int minimumMemSize = 1200;
		}

		protected static CacheManager _instance;

		public ManagerSetting managerSetting;

		public List<CacheElement> elements;

		protected Dictionary<string, AbstractCache> _caches;

		private CacheWithPool _controllerCache;

		private ProjectileCache _projectileCache;

		private UnitCache _unitCache;

		private TerrainCache _terrainCache;

		private CacheWithPool _statusEffectCache;

		private CacheWithPool _fireEffectCache;

		private CacheWithPool _etcEffectCache;

		private CacheWithPool _cutInEffectCache;

		private SpineCache _spineCache;

		private SoundPocketCache _soundPocketCache;

		private SoundCache _soundCache;

		private CacheWithPool _uiCache;

		public static CacheManager instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = UnityEngine.Object.FindObjectOfType<CacheManager>();
				}
				return _instance;
			}
		}

		public CacheWithPool ControllerCache
		{
			get
			{
				if (_controllerCache == null)
				{
					_controllerCache = (CacheWithPool)GetCache("ControllerCache");
				}
				return _controllerCache;
			}
		}

		public ProjectileCache ProjectileCache
		{
			get
			{
				if (_projectileCache == null)
				{
					_projectileCache = (ProjectileCache)GetCache("ProjectileCache");
				}
				return _projectileCache;
			}
		}

		public UnitCache UnitCache
		{
			get
			{
				if (_unitCache == null)
				{
					_unitCache = (UnitCache)GetCache("UnitCache");
				}
				return _unitCache;
			}
		}

		public TerrainCache TerrainCache
		{
			get
			{
				if (_terrainCache == null)
				{
					_terrainCache = (TerrainCache)GetCache("TerrainCache");
				}
				return _terrainCache;
			}
		}

		public CacheWithPool StatusEffectCache
		{
			get
			{
				if (_statusEffectCache == null)
				{
					_statusEffectCache = (CacheWithPool)GetCache("StatusEffectCache");
				}
				return _statusEffectCache;
			}
		}

		public CacheWithPool FireEffectCache
		{
			get
			{
				if (_fireEffectCache == null)
				{
					_fireEffectCache = (CacheWithPool)GetCache("FireEffectCache");
				}
				return _fireEffectCache;
			}
		}

		public CacheWithPool EtcEffectCache
		{
			get
			{
				if (_etcEffectCache == null)
				{
					_etcEffectCache = (CacheWithPool)GetCache("EtcEffectCache");
				}
				return _etcEffectCache;
			}
		}

		public CacheWithPool CutInEffectCache
		{
			get
			{
				if (_cutInEffectCache == null)
				{
					_cutInEffectCache = (CacheWithPool)GetCache("CutInEffectCache");
				}
				return _cutInEffectCache;
			}
		}

		public SpineCache SpineCache
		{
			get
			{
				if (_spineCache == null)
				{
					_spineCache = (SpineCache)GetCache("SpineCache");
				}
				return _spineCache;
			}
		}

		public SoundPocketCache SoundPocketCache
		{
			get
			{
				if (_soundPocketCache == null)
				{
					_soundPocketCache = (SoundPocketCache)GetCache("SoundPocketCache");
				}
				return _soundPocketCache;
			}
		}

		public SoundCache SoundCache
		{
			get
			{
				if (_soundCache == null)
				{
					_soundCache = (SoundCache)GetCache("SoundCache");
				}
				return _soundCache;
			}
		}

		public CacheWithPool UiCache
		{
			get
			{
				if (_uiCache == null)
				{
					_uiCache = (CacheWithPool)GetCache("UICache");
				}
				return _uiCache;
			}
		}

		public void RemoveSoundCache()
		{
			if (!(_soundCache == null) && _caches.ContainsKey(_soundCache.name))
			{
				_caches.Remove(_soundCache.name);
				_soundCache.CleanUp();
				UnityEngine.Object.DestroyImmediate(_soundCache.gameObject);
			}
		}

		private new void Awake()
		{
			if (_caches == null)
			{
				_caches = new Dictionary<string, AbstractCache>();
			}
			if (_instance != null && _instance != this)
			{
				UnityEngine.Object.DestroyImmediate(this);
			}
			else
			{
				_instance = this;
			}
		}

		public override void Init()
		{
			base.Init();
			setting.enableCache = false;
			if (SystemInfo.systemMemorySize < managerSetting.minimumMemSize)
			{
				setting.enableCache = false;
			}
			if (!managerSetting.LoadCacheOnStart)
			{
				return;
			}
			for (int i = 0; i < elements.Count; i++)
			{
				CacheElement cacheElement = elements[i];
				if (_caches.ContainsKey(cacheElement.key))
				{
					continue;
				}
				AbstractCache abstractCache = Create<AbstractCache>(cacheElement.key);
				if (!(abstractCache != null))
				{
					continue;
				}
				abstractCache.name = cacheElement.sourcePrefab.name;
				abstractCache.transform.parent = base.transform;
				if (!string.IsNullOrEmpty(abstractCache.GroupName))
				{
					Transform transform = base.transform.Find(abstractCache.GroupName);
					if (transform == null)
					{
						GameObject gameObject = new GameObject();
						gameObject.name = abstractCache.GroupName;
						gameObject.transform.parent = base.transform;
						transform = gameObject.transform;
					}
					abstractCache.transform.parent = transform;
				}
				abstractCache.Init();
				_caches.Add(abstractCache.name, abstractCache);
			}
		}

		public void SetCache(AbstractCache cache)
		{
			if (_caches == null)
			{
				_caches = new Dictionary<string, AbstractCache>();
			}
			else if (_caches.ContainsKey(cache.CacheName))
			{
				return;
			}
			_caches.Add(cache.CacheName, cache);
		}

		public AbstractCache GetCache(string cacheName)
		{
			if (_caches == null)
			{
				return null;
			}
			if (_caches.ContainsKey(cacheName))
			{
				return _caches[cacheName];
			}
			AbstractCache abstractCache = Create<AbstractCache>(cacheName, base.transform);
			if (abstractCache != null)
			{
				abstractCache.name = cacheName;
				abstractCache.transform.parent = base.transform;
				if (!string.IsNullOrEmpty(abstractCache.GroupName))
				{
					Transform transform = base.transform.Find(abstractCache.GroupName);
					if (transform == null)
					{
						GameObject gameObject = new GameObject();
						gameObject.name = abstractCache.GroupName;
						gameObject.transform.parent = base.transform;
						transform = gameObject.transform;
					}
					abstractCache.transform.parent = transform;
				}
				abstractCache.setting.enableCache = setting.enableCache;
				abstractCache.Init();
				_caches.Add(abstractCache.name, abstractCache);
			}
			return abstractCache;
		}

		protected override void RefreshElementDict()
		{
			_usedElements = new List<CacheElement>();
			_dicElements = new Dictionary<string, CacheElement>();
			for (int i = 0; i < elements.Count; i++)
			{
				_dicElements.Add(elements[i].key, elements[i]);
				if (elements[i].sourcePrefab != null)
				{
					_usedElements.Add(elements[i]);
				}
			}
			elements.Clear();
		}

		protected override void LoadElementFromAssetBundle()
		{
		}

		protected override CacheElement LoadAssetBundlePrefab(string key, CacheElement targetElement = null)
		{
			CacheElement cacheElement = targetElement;
			StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(key + ".assetbundle", ECacheType.Unit));
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(key + ".assetbundle");
			if (assetBundle != null)
			{
				GameObject sourcePrefab = assetBundle.mainAsset as GameObject;
				if (cacheElement != null)
				{
					cacheElement.sourcePrefab = sourcePrefab;
				}
				else
				{
					cacheElement = new CacheElement();
					cacheElement.key = key;
					cacheElement.sourcePrefab = sourcePrefab;
					cacheElement.resPath = string.Empty;
					AddCacheElement(cacheElement);
				}
			}
			return cacheElement;
		}

		public void StartUp()
		{
			CacheItemPocket.Release();
		}

		public override void CleanUp()
		{
			if (_caches != null)
			{
				Dictionary<string, AbstractCache>.Enumerator enumerator = _caches.GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.Value.CleanUp();
				}
			}
		}
	}
}
