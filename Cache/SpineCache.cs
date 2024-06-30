using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace Cache
{
	public class SpineCache : AbstractCacheWithPool
	{
		public List<CacheWithPoolElement> elements;

		protected override CacheElement GetEditElement(string key)
		{
			return elements.Find((CacheWithPoolElement x) => x.key == key);
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
			for (int i = 0; i < RemoteObjectManager.instance.localUser.assetbundleSpineKey.Count; i++)
			{
				LoadAssetBundlePrefab(RemoteObjectManager.instance.localUser.assetbundleSpineKey[i]);
			}
		}

		protected override CacheElement LoadAssetBundlePrefab(string key, CacheElement targetElement = null)
		{
			CacheWithPoolElement cacheWithPoolElement = (CacheWithPoolElement)targetElement;
			StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(key + ".assetbundle", ECacheType.Spine));
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

		private void ResetSpine(GameObject spineObj)
		{
			string text = string.Empty;
			bool flag = false;
			MeshRenderer[] componentsInChildren = spineObj.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				text = "Custom/Spine/UnlitTransparent Colored/One-OneMinusAlpha";
			}
			SkeletonAnimation component = spineObj.GetComponent<SkeletonAnimation>();
			AtlasAsset atlasAsset = ScriptableObject.CreateInstance<AtlasAsset>();
			atlasAsset.atlasFile = component.skeletonDataAsset.atlasAssets[0].atlasFile;
			Material[] array = new Material[component.GetComponent<MeshRenderer>().sharedMaterials.Length];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = new Material(Shader.Find(text));
				array[j].mainTexture = component.GetComponent<MeshRenderer>().sharedMaterials[j].mainTexture;
			}
			atlasAsset.materials = array;
			SkeletonDataAsset skeletonDataAsset = Object.Instantiate(spineObj.GetComponent<SkeletonAnimation>().skeletonDataAsset);
			skeletonDataAsset.atlasAssets[0] = atlasAsset;
			component.skeletonDataAsset = skeletonDataAsset;
			if (!(component.skeletonDataAsset != null))
			{
				return;
			}
			if (flag)
			{
				MeshRenderer[] componentsInChildren2 = spineObj.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
				foreach (MeshRenderer meshRenderer2 in componentsInChildren2)
				{
				}
			}
			else if (component.skeletonDataAsset.atlasAssets[0] != null)
			{
				component.skeletonDataAsset.atlasAssets[0].Reset();
			}
			component.skeletonDataAsset.Reset();
		}
	}
}
