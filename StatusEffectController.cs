using System.Collections.Generic;
using Cache;
using UnityEngine;

public class StatusEffectController : MonoBehaviour, ICacheItem
{
	public class StatusEffect
	{
		public int CacheID;

		public GameObject CacheObj;
	}

	public const string CacheKey = "StatusEffectController";

	public TimedGameObject timeGameObject;

	protected CacheWithPool _statusEffCache;

	protected CacheWithPool _controllerCache;

	protected List<StatusEffect> _cachedEffects;

	public int CacheID { get; set; }

	public GameObject CacheObj => base.gameObject;

	public new string name
	{
		get
		{
			return base.gameObject.name;
		}
		set
		{
			base.gameObject.name = $"StatusEffect-{value:D9}";
		}
	}

	private void Awake()
	{
		_cachedEffects = new List<StatusEffect>();
		_statusEffCache = CacheManager.instance.StatusEffectCache;
		_controllerCache = CacheManager.instance.ControllerCache;
		if (timeGameObject == null)
		{
			timeGameObject = GetComponent<TimedGameObject>();
			if (timeGameObject == null)
			{
				timeGameObject = base.gameObject.AddComponent<TimedGameObject>();
				timeGameObject.SearchObjects = false;
				timeGameObject.groupType = ETimeGroupType.EtcGroup;
				timeGameObject.TimeGroupController = TimeControllerManager.instance.ETC;
			}
		}
	}

	public void Release()
	{
		for (int i = 0; i < _cachedEffects.Count; i++)
		{
			_statusEffCache.Release(_cachedEffects[i].CacheID, _cachedEffects[i].CacheObj);
		}
		_cachedEffects.Clear();
		_controllerCache.Release(this);
	}

	public bool Create(string cachedKey, Vector3 position)
	{
		CacheElement element = _statusEffCache.GetElement(cachedKey);
		if (element == null)
		{
			Release();
			return false;
		}
		GameObject gameObject = _statusEffCache.Create(element, position, Quaternion.identity, base.transform);
		if (gameObject == null)
		{
			Release();
			return false;
		}
		StatusEffect statusEffect = new StatusEffect();
		statusEffect.CacheID = element.ID;
		statusEffect.CacheObj = gameObject;
		_cachedEffects.Add(statusEffect);
		return true;
	}
}
