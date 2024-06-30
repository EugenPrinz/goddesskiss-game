using System.Collections.Generic;
using UnityEngine;

namespace Cache
{
	public class CacheWithoutPool : AbstractCacheWithoutPool
	{
		public List<CacheElement> elements;

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
			return elements.Find((CacheElement x) => x.key == key);
		}

		protected override void LoadElementFromAssetBundle()
		{
		}

		protected override CacheElement LoadAssetBundlePrefab(string key, CacheElement targetElement = null)
		{
			CacheElement cacheElement = targetElement;
			StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(key + ".assetbundle"));
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(key + ".assetbundle");
			if (assetBundle != null)
			{
				GameObject sourcePrefab = assetBundle.LoadAsset(key + ".prefab") as GameObject;
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
	}
}
