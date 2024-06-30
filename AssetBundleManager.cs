using System;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleManager
{
	private class AssetReference : IDisposable
	{
		private int count;

		public AssetBundle bundle;

		public bool isBgm;

		public AssetReference(AssetBundle bundle)
		{
			this.bundle = bundle;
			count = 0;
		}

		public int Increase()
		{
			return ++count;
		}

		public int Decrease()
		{
			count--;
			if (count <= 0)
			{
				Dispose();
			}
			return count;
		}

		public void Dispose()
		{
			if (bundle != null)
			{
				bundle.Unload(unloadAllLoadedObjects: false);
				bundle = null;
			}
		}
	}

	private static Dictionary<string, AssetReference> bundles = new Dictionary<string, AssetReference>();

	public static void AddAssetBundle(string url, AssetBundle bundle, ECacheType type = ECacheType.None)
	{
		if (bundles.ContainsKey(url))
		{
			bundles[url].Increase();
		}
		else
		{
			AssetReference assetReference = new AssetReference(bundle);
			assetReference.isBgm = type == ECacheType.Bgm;
			bundles.Add(url, assetReference);
		}
		string text = url.Replace(".assetbundle", string.Empty);
		if (type != ECacheType.SoundPocket)
		{
		}
	}

	public static void AddAssetBundle(WWW www, ECacheType type = ECacheType.None)
	{
		string[] array = www.url.Split('/');
		AddAssetBundle(array[array.Length - 1], www.assetBundle, type);
	}

	public static void AddAssetBundleDown(string key, AssetBundle bundle, ECacheType type = ECacheType.None)
	{
		AddAssetBundle(key, bundle, type);
	}

	public static void DeleteAssetBundle(string url)
	{
		if (bundles.ContainsKey(url) && bundles[url].Decrease() <= 0)
		{
			bundles.Remove(url);
		}
	}

	public static void DeleteAssetBundle(WWW www)
	{
		DeleteAssetBundle(www.url);
	}

	public static void DeleteAssetBundleAll()
	{
		SoundManager.StopMusicImmediately();
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, AssetReference> bundle in bundles)
		{
			list.Add(bundle.Key);
		}
		for (int i = 0; i < list.Count; i++)
		{
			DeleteAssetBundle(list[i]);
		}
	}

	public static void DeleteAssetBundleAllWithoutBgm()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, AssetReference> bundle in bundles)
		{
			if (!bundle.Value.isBgm)
			{
				list.Add(bundle.Key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			DeleteAssetBundle(list[i]);
		}
	}

	public static AssetBundle GetAssetBundle(string url)
	{
		if (bundles.ContainsKey(url))
		{
			return bundles[url].bundle;
		}
		return null;
	}

	public static GameObject GetObjectFromAssetBundle(string url)
	{
		if (bundles.ContainsKey(url))
		{
			return bundles[url].bundle.LoadAsset(url.Replace("assetbundle", "prefab")) as GameObject;
		}
		return null;
	}

	public static bool HasAssetBundle(string url)
	{
		if (bundles.ContainsKey(url))
		{
			return true;
		}
		return false;
	}

	public static int GetObjectTottal()
	{
		return bundles.Count;
	}
}
