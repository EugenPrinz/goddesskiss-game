using System.Collections.Generic;
using UnityEngine;

namespace Cache
{
	public class CacheWithPool : AbstractCacheWithPool
	{
		public List<CacheWithPoolElement> elements;

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

		protected override CacheElement GetEditElement(string key)
		{
			return elements.Find((CacheWithPoolElement x) => x.key == key);
		}

		protected override void LoadElementFromAssetBundle()
		{
		}

		protected override CacheElement LoadAssetBundlePrefab(string key, CacheElement targetElement = null)
		{
			CacheWithPoolElement cacheWithPoolElement = (CacheWithPoolElement)targetElement;
			StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(key + ".assetbundle"));
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(key + ".assetbundle");
			if (assetBundle != null)
			{
				GameObject sourcePrefab = assetBundle.LoadAsset(key + ".prefab") as GameObject;
				if (cacheWithPoolElement != null)
				{
					cacheWithPoolElement.sourcePrefab = sourcePrefab;
				}
				else
				{
					cacheWithPoolElement = new CacheWithPoolElement();
					cacheWithPoolElement.key = key;
					cacheWithPoolElement.sourcePrefab = sourcePrefab;
					cacheWithPoolElement.resPath = string.Empty;
					cacheWithPoolElement.capacity = poolSetting.defaultSetting.capacity;
					AddCacheElement(cacheWithPoolElement);
				}
			}
			return cacheWithPoolElement;
		}
	}
}
