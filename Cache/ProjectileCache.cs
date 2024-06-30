using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Cache
{
	public class ProjectileCache : AbstractCacheWithPool
	{
		public List<CacheWithPoolElement> elements;

		public ProjectileMotionPhase Create(int idx, Transform parent = null)
		{
			CacheElement cacheElement = elements[idx];
			if (cacheElement != null)
			{
				GameObject gameObject = Create(cacheElement.key, Vector3.zero, Quaternion.identity, parent);
				if (gameObject != null)
				{
					ProjectileMotionPhase component = gameObject.GetComponent<ProjectileMotionPhase>();
					component.cacheID = cacheElement.ID;
					component.bRender = true;
					return component;
				}
			}
			return null;
		}

		public void Release(ProjectileMotionPhase projectilePhase)
		{
			Release(projectilePhase.cacheID, projectilePhase.gameObject);
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
		}

		protected override void LoadElementFromAssetBundle()
		{
			List<CacheWithPoolElement> list = new List<CacheWithPoolElement>();
			for (int i = 0; i < RemoteObjectManager.instance.regulation.projectileMotionPhaseDtbl.length; i++)
			{
				string key = RemoteObjectManager.instance.regulation.projectileMotionPhaseDtbl[i].GetKey();
				if (RemoteObjectManager.instance.regulation.projectileMotionPhaseDtbl[i].isHeader)
				{
					CacheWithPoolElement cacheWithPoolElement = new CacheWithPoolElement();
					cacheWithPoolElement.key = key;
					cacheWithPoolElement.sourcePrefab = null;
					cacheWithPoolElement.resPath = string.Empty;
					cacheWithPoolElement.capacity = 0;
					list.Add(cacheWithPoolElement);
					continue;
				}
				string[] array = key.Split('/');
				string url = array[array.Length - 1] + ".assetbundle";
				ProjectileMotionPhase projectileMotionPhase = null;
				AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(url);
				if (assetBundle != null)
				{
					GameObject gameObject = assetBundle.mainAsset as GameObject;
					projectileMotionPhase = gameObject.GetComponent<ProjectileMotionPhase>();
				}
				CacheWithPoolElement cacheWithPoolElement2 = new CacheWithPoolElement();
				cacheWithPoolElement2.key = key;
				cacheWithPoolElement2.sourcePrefab = projectileMotionPhase.gameObject;
				cacheWithPoolElement2.resPath = string.Empty;
				cacheWithPoolElement2.capacity = poolSetting.defaultSetting.capacity;
				list.Add(cacheWithPoolElement2);
			}
			elements.Clear();
			elements = list;
		}

		protected override CacheElement LoadAssetBundlePrefab(string key, CacheElement targetElement = null)
		{
			CacheWithPoolElement cacheWithPoolElement = (CacheWithPoolElement)targetElement;
			string fileName = Path.GetFileName(key);
			StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(fileName + ".assetbundle", ECacheType.Projectile));
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(fileName + ".assetbundle");
			if (assetBundle != null)
			{
				GameObject sourcePrefab = assetBundle.LoadAsset(fileName + ".prefab") as GameObject;
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
