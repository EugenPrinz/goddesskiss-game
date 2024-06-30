using System.Collections.Generic;
using UnityEngine;

namespace Cache
{
	public class UnitCache : AbstractCacheWithPool
	{
		public List<CacheWithPoolElement> elements;

		protected override CacheElement GetEditElement(string key)
		{
			return elements.Find((CacheWithPoolElement x) => x.key == key);
		}

		public new UnitRenderer Create(string key, Transform parent = null)
		{
			CacheElement element = GetElement(key);
			if (element != null)
			{
				GameObject gameObject = Create(element, Vector3.zero, Quaternion.identity, parent);
				if (gameObject != null)
				{
					UnitRenderer component = gameObject.GetComponent<UnitRenderer>();
					component.cacheID = element.ID;
					component.unitName = key;
					return component;
				}
			}
			return null;
		}

		public new UnitRenderer Create(string key, Vector3 position, Quaternion rotation, Transform parent = null)
		{
			CacheElement element = GetElement(key);
			if (element != null)
			{
				GameObject gameObject = Create(element, position, rotation, parent);
				if (gameObject != null)
				{
					UnitRenderer component = gameObject.GetComponent<UnitRenderer>();
					component.cacheID = element.ID;
					component.unitName = key;
					return component;
				}
			}
			return null;
		}

		public void Release(UnitRenderer unitRenderer)
		{
			Release(unitRenderer.cacheID, unitRenderer.gameObject);
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
			List<CacheWithPoolElement> list = new List<CacheWithPoolElement>();
			for (int i = 0; i < RemoteObjectManager.instance.regulation.unitDtbl.length; i++)
			{
				string url = RemoteObjectManager.instance.regulation.unitDtbl[i].resourceName + ".assetbundle";
				string key = RemoteObjectManager.instance.regulation.unitDtbl[i].prefabId;
				AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(url);
				if (assetBundle == null)
				{
					continue;
				}
				GameObject gameObject = assetBundle.mainAsset as GameObject;
				if (list.Find((CacheWithPoolElement element) => element.key == key) == null)
				{
					SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
					foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
					{
						skinnedMeshRenderer.sharedMaterial.shader = Shader.Find(skinnedMeshRenderer.sharedMaterial.shader.name);
					}
					MeshRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
					foreach (MeshRenderer meshRenderer in componentsInChildren2)
					{
						meshRenderer.sharedMaterial.shader = Shader.Find(meshRenderer.sharedMaterial.shader.name);
					}
					CacheWithPoolElement cacheWithPoolElement = new CacheWithPoolElement();
					cacheWithPoolElement.key = key;
					cacheWithPoolElement.sourcePrefab = gameObject;
					cacheWithPoolElement.resPath = string.Empty;
					cacheWithPoolElement.capacity = poolSetting.defaultSetting.capacity;
					list.Add(cacheWithPoolElement);
				}
			}
			elements.Clear();
			elements = list;
		}

		protected override CacheElement LoadAssetBundlePrefab(string key, CacheElement targetElement = null)
		{
			CacheWithPoolElement cacheWithPoolElement = (CacheWithPoolElement)targetElement;
			StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(key + ".assetbundle", ECacheType.Unit));
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
