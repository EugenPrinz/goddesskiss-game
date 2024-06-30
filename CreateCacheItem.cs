using Cache;
using UnityEngine;

public class CreateCacheItem : MonoBehaviour, ICacheItem
{
	public string cacheName;

	public string itemName;

	public Transform targetRoot;

	public string animationName;

	public MountPointsSub mountPoints;

	public ETimeGroupType timeGroupType = ETimeGroupType.EtcGroup;

	[HideInInspector]
	public UnitRenderer actor;

	[HideInInspector]
	public GameObject targetItem;

	[HideInInspector]
	public AbstractCache cache;

	protected bool bStart;

	protected Animation animation;

	public int CacheID { get; set; }

	public GameObject CacheObj => targetItem;

	private void Start()
	{
		Load();
		bStart = true;
	}

	private void OnEnable()
	{
		if (bStart)
		{
			Load();
		}
	}

	public virtual void Release()
	{
		if (targetItem != null)
		{
			if (cache != null)
			{
				cache.Release(this);
			}
			else
			{
				Object.DestroyImmediate(targetItem);
			}
			targetItem = null;
		}
		if (mountPoints != null)
		{
			mountPoints.mountPoints = null;
		}
	}

	protected virtual void Create()
	{
		cache = CacheManager.instance.GetCache(cacheName);
		if (cache == null)
		{
			return;
		}
		if (actor != null)
		{
			if (CacheManager.instance.UnitCache == cache)
			{
				UnitRenderer unitRenderer = CacheManager.instance.UnitCache.Create(itemName, targetRoot);
				unitRenderer.SetModelType(actor.modelType);
				targetItem = unitRenderer.gameObject;
			}
			else if (CacheManager.instance.EtcEffectCache == cache)
			{
				if (actor.modelType > 0)
				{
					CacheElement element = cache.GetElement($"{itemName}@{actor.modelType}");
					if (element != null)
					{
						targetItem = cache.Create(element, targetRoot);
					}
				}
			}
			else if (!(CacheManager.instance.SpineCache == cache))
			{
			}
		}
		if (targetItem == null)
		{
			CacheElement element2 = cache.GetElement(itemName);
			if (element2 == null)
			{
				return;
			}
			targetItem = cache.Create(element2, targetRoot);
			if (targetItem == null)
			{
				return;
			}
			CacheID = element2.ID;
		}
		targetItem.name = itemName;
		targetItem.layer = base.gameObject.layer;
		MountPoints component = targetItem.GetComponent<MountPoints>();
		if (component != null)
		{
			if (mountPoints == null)
			{
				mountPoints = GetComponent<MountPointsSub>();
			}
			if (mountPoints != null)
			{
				mountPoints.mountPoints = component;
			}
		}
		TimedGameObject component2 = targetItem.GetComponent<TimedGameObject>();
		if (component2 != null)
		{
			component2.groupType = timeGroupType;
		}
		if (!string.IsNullOrEmpty(animationName))
		{
			animation = targetItem.GetComponent<Animation>();
			if (animation != null)
			{
				animation.cullingType = AnimationCullingType.AlwaysAnimate;
				animation.Play(animationName);
			}
		}
	}

	protected virtual void Reset()
	{
		if (animation != null && !string.IsNullOrEmpty(animationName))
		{
			animation.Rewind();
			animation.Play(animationName);
		}
	}

	public virtual void Load()
	{
		if (targetItem != null && targetItem.name != itemName)
		{
			Release();
		}
		if (targetItem == null)
		{
			Create();
		}
		else
		{
			Reset();
		}
	}
}
