using System.Collections.Generic;
using Cache;
using UnityEngine;

public class CacheItemPocket : MonoBehaviour
{
	public static CacheItemPocket instance;

	public List<GameObject> items;

	public int ItemCount => items.Count;

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Object.DestroyImmediate(this);
		}
		else
		{
			instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		items = new List<GameObject>();
	}

	private void OnDestroy()
	{
		instance = null;
		items = null;
	}

	public void Create(AbstractCache cache, string key)
	{
		GameObject gameObject = cache.Create(key, base.transform);
		if (gameObject == null)
		{
			if (cache is SoundPocketCache)
			{
			}
			return;
		}
		if (!(cache is SoundPocketCache))
		{
			gameObject.SetActive(value: false);
		}
		items.Add(gameObject);
	}

	public static void Release()
	{
		if (instance != null)
		{
			Object.DestroyImmediate(instance.gameObject);
		}
	}
}
