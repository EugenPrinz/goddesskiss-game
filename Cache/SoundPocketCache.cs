using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cache
{
	public class SoundPocketCache : AbstractCacheWithoutPool
	{
		public List<CacheElement> elements;

		public IEnumerator Load(SoundPocketSfxData pocket)
		{
			if (SoundManager.Instance.currentPockets.Contains(pocket.pocketName))
			{
				yield break;
			}
			for (int i = 0; i < pocket.clipNames.Count; i++)
			{
				if (!CacheManager.instance.SoundCache.Create(pocket.clipNames[i], isBgm: false, pocket.autoPrepoolAmount, pocket.autoBaseVolume, pocket.autoVolumeVariation, pocket.autoPitchVariation))
				{
				}
				yield return null;
			}
			SoundManager.Instance.currentPockets.Add(pocket.pocketName);
		}

		public IEnumerator Load(SoundBgmPocketData pocket, string level = null)
		{
			SoundConnection sc = pocket.GetConnection();
			if (sc != null)
			{
				CacheManager.instance.SoundCache.PlayConnection(sc, level);
			}
			yield return null;
		}

		public IEnumerator LoadSfxPocket(string key)
		{
			CacheElement elem = GetElement(key);
			if (elem != null)
			{
				SoundPocketSfx soundPocket = elem.sourcePrefab.GetComponent<SoundPocketSfx>();
				if (soundPocket != null)
				{
					yield return StartCoroutine(Load(soundPocket.data));
				}
			}
		}

		public IEnumerator LoadBgmPocket(string key, string level = null)
		{
			CacheElement elem = GetElement(key);
			if (elem != null)
			{
				SoundPocketBgm soundPocket = elem.sourcePrefab.GetComponent<SoundPocketBgm>();
				if (soundPocket != null)
				{
					yield return StartCoroutine(Load(soundPocket.data, level));
				}
			}
		}

		public override GameObject Create(CacheElement elem, Vector3 position, Quaternion rotation, Transform parent = null)
		{
			SoundPocketBgm component = elem.sourcePrefab.GetComponent<SoundPocketBgm>();
			if (component != null)
			{
				StartCoroutine(Load(component.data));
				return null;
			}
			SoundPocketSfx component2 = elem.sourcePrefab.GetComponent<SoundPocketSfx>();
			if (component2 != null)
			{
				StartCoroutine(Load(component2.data));
				return null;
			}
			return null;
		}

		public override void CleanUp()
		{
			StopAllCoroutines();
			base.CleanUp();
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
			StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(key + ".assetbundle", ECacheType.SoundPocket));
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
