using System.Collections.Generic;
using UnityEngine;

namespace Cache
{
	public class TerrainCache : AbstractCacheWithoutPool
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

		public override GameObject Create(CacheElement elem, Vector3 position, Quaternion rotation, Transform parent = null)
		{
			GameObject sourcePrefab = elem.sourcePrefab;
			if (!setting.enableCache)
			{
				elem.sourcePrefab = null;
			}
			Transform transform = sourcePrefab.transform;
			int index = Random.Range(0, transform.childCount);
			GameObject original = transform.GetChild(index).gameObject;
			GameObject gameObject = Object.Instantiate(original);
			Transform transform2 = gameObject.transform;
			transform2.SetParent(parent, worldPositionStays: false);
			transform2.localPosition = position;
			transform2.localRotation = rotation;
			gameObject.SetActive(value: true);
			return gameObject;
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
