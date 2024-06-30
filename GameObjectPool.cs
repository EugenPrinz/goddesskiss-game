using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : MonoBehaviour
{
	public Transform cacheRoot;

	public GameObject sourcePrefab;

	[Range(0f, 10000f)]
	public int capacity = 5;

	public List<GameObject> _pool;

	private bool _init;

	public int CachedCnt => _pool.Count;

	private void Start()
	{
		Init();
	}

	public virtual void Init()
	{
		if (!_init)
		{
			_pool = new List<GameObject>();
			_init = true;
		}
	}

	public GameObject Create(Transform parent = null)
	{
		if (!_init)
		{
			Init();
		}
		GameObject gameObject;
		if (CachedCnt > 0)
		{
			gameObject = _pool[0];
			_pool.RemoveAt(0);
		}
		else
		{
			gameObject = Object.Instantiate(sourcePrefab);
		}
		if (parent != null)
		{
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = sourcePrefab.transform.localPosition;
			gameObject.transform.localScale = sourcePrefab.transform.localScale;
		}
		else
		{
			gameObject.transform.parent = base.transform;
		}
		gameObject.SetActive(value: true);
		return gameObject;
	}

	public T Create<T>(Transform parent = null)
	{
		GameObject gameObject = Create(parent);
		if (gameObject == null)
		{
			return default(T);
		}
		return gameObject.GetComponent<T>();
	}

	public void Release(GameObject obj)
	{
		if (CachedCnt >= capacity)
		{
			Object.DestroyImmediate(obj);
			return;
		}
		_pool.Add(obj);
		obj.SetActive(value: false);
		if (cacheRoot != null)
		{
			obj.transform.parent = cacheRoot;
		}
		else
		{
			obj.transform.parent = base.transform;
		}
	}
}
