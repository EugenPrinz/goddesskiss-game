using System;
using Cache;
using UnityEngine;

[RequireComponent(typeof(TimedGameObject))]
public class FlowAndDeactivate : MonoBehaviour
{
	public enum EDeactivateType
	{
		NONE,
		DISABLE,
		DESTROY,
		CACHE
	}

	public GameObject target;

	public float flowDelayMS;

	public float deactivateDelayMS = 3000f;

	public float speed = ConstValue.battleTerrainScrollSpeed;

	public string cacheName;

	public EDeactivateType deactivateType;

	public Action OnEventTime;

	private float _playTime;

	private TimedGameObject _tgo;

	protected AbstractCache _cache;

	protected int _cacheID = -1;

	public int cacheID => (_cacheID != -1) ? _cacheID : target.GetInstanceID();

	private void Awake()
	{
		if (!string.IsNullOrEmpty(cacheName))
		{
			_cache = CacheManager.instance.GetCache(cacheName);
		}
	}

	private void OnEnable()
	{
		_tgo = GetComponent<TimedGameObject>();
		_playTime = 0f;
	}

	private void Update()
	{
		float num = _tgo.TimedDeltaTime();
		_playTime += num;
		if (_playTime >= flowDelayMS * 0.001f)
		{
			target.transform.Translate(0f, 0f, speed * num);
		}
		if (!(_playTime >= deactivateDelayMS * 0.001f))
		{
			return;
		}
		base.enabled = false;
		if (OnEventTime != null)
		{
			OnEventTime();
		}
		switch (deactivateType)
		{
		case EDeactivateType.DESTROY:
			UnityEngine.Object.DestroyImmediate(target);
			break;
		case EDeactivateType.DISABLE:
			target.SetActive(value: false);
			break;
		case EDeactivateType.CACHE:
			if (_cache != null)
			{
				_cache.Release(cacheID, target);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(target);
			}
			break;
		}
	}
}
