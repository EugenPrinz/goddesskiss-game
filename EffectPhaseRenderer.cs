using System.Collections.Generic;
using UnityEngine;

public class EffectPhaseRenderer : MonoBehaviour
{
	public enum EType
	{
		Actor,
		Target
	}

	public TimedGameObject timeGameObject;

	public MountPointsSub mount;

	public List<ProjectileMotionPhase> phases;

	public List<CreateCacheItem> cacheItems;

	[HideInInspector]
	public EType type;

	protected bool init;

	protected bool isAlive;

	protected Vector3 offsetPosition;

	private void Awake()
	{
		offsetPosition = base.transform.localPosition;
	}

	private void Start()
	{
		init = true;
		OnEnable();
	}

	public void Set(UnitRenderer actor, UnitRenderer target)
	{
		if (!init)
		{
			offsetPosition = base.transform.localPosition;
		}
		base.transform.position = target.transform.position;
		base.transform.localPosition += offsetPosition;
		if (mount != null)
		{
			mount.mountPoints = target.mountPosition;
		}
		for (int i = 0; i < phases.Count; i++)
		{
			if ((type == EType.Actor && target.drawSide == SplitScreenDrawSide.Right) || (type == EType.Target && target.drawSide == SplitScreenDrawSide.Left))
			{
				if (phases[i].anchor == ProjectileMotionPhase.EAnchor.UPDATE_TROOP_CAMERA)
				{
					phases[i].bInvers = true;
					phases[i].transform.localScale = new Vector3(-1f, 1f, 1f);
				}
				else
				{
					phases[i].transform.localScale = new Vector3(1f, 1f, 1f);
				}
			}
			else
			{
				phases[i].transform.localScale = new Vector3(1f, 1f, 1f);
			}
			phases[i].Set(actor, target);
		}
		for (int j = 0; j < cacheItems.Count; j++)
		{
			cacheItems[j].actor = actor;
		}
		base.gameObject.SetActive(value: true);
	}

	private void OnEnable()
	{
		if (!init)
		{
			return;
		}
		for (int i = 0; i < phases.Count; i++)
		{
			ProjectileMotionPhase projectileMotionPhase = phases[i];
			if (i == 0)
			{
				projectileMotionPhase.bEnabel = true;
				projectileMotionPhase.bFinishEventTime = false;
				projectileMotionPhase.gameObject.SetActive(value: true);
			}
			else
			{
				projectileMotionPhase.bEnabel = false;
				projectileMotionPhase.bFinishEventTime = false;
				projectileMotionPhase.gameObject.SetActive(value: false);
			}
		}
	}

	private void Update()
	{
		if (!init)
		{
			return;
		}
		isAlive = false;
		int num = (int)(1000f * timeGameObject.TimedDeltaTime());
		int num2 = 0;
		if (phases.Count > 0)
		{
			for (int i = 0; i < phases.Count; i++)
			{
				ProjectileMotionPhase projectileMotionPhase = phases[i];
				if (!projectileMotionPhase.bEnabel)
				{
					continue;
				}
				isAlive = true;
				int num3 = projectileMotionPhase.elapsedTime;
				if (num3 == 0)
				{
					num3 = -1;
				}
				if (projectileMotionPhase.gameObject.activeSelf)
				{
					_UpdateMotionPhase(projectileMotionPhase, num3);
				}
				projectileMotionPhase.elapsedTime += num2 + num;
				if (!projectileMotionPhase.bFinishEventTime)
				{
					num2 = Mathf.Max(0, projectileMotionPhase.elapsedTime - projectileMotionPhase.eventTime);
					if (num2 > 0)
					{
						projectileMotionPhase.bFinishEventTime = true;
						int num4 = i + 1;
						if (num4 < phases.Count)
						{
							phases[num4].bEnabel = true;
							phases[num4].gameObject.SetActive(value: true);
						}
					}
				}
				if (Mathf.Max(0, projectileMotionPhase.elapsedTime - projectileMotionPhase.duration) > 0)
				{
					projectileMotionPhase.gameObject.SetActive(value: false);
					if (projectileMotionPhase.bFinishEventTime)
					{
						projectileMotionPhase.bEnabel = false;
						projectileMotionPhase.elapsedTime = 0;
					}
				}
			}
		}
		if (!isAlive)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void AddEffectPhase(ProjectileMotionPhase phase)
	{
		phases.Add(phase);
	}

	private void _UpdateMotionPhase(ProjectileMotionPhase phase, float lastElapsedTime)
	{
	}
}
