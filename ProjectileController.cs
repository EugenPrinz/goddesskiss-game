using System.Collections.Generic;
using Cache;
using UnityEngine;

public class ProjectileController : MonoBehaviour, ICacheItem
{
	public const string CacheKey = "ProjectileController";

	public TimedGameObject timeGameObject;

	protected ProjectileCache _projectileCache;

	protected CacheWithPool _controllerCache;

	protected List<ProjectileMotionPhase> _phases;

	public int CacheID { get; set; }

	public GameObject CacheObj => base.gameObject;

	public int hitTime
	{
		get
		{
			if (_phases.Count != 2)
			{
				return 0;
			}
			return _phases[0].eventTime + _phases[0].eventDelayTime + _phases[1].eventTime + _phases[1].eventDelayTime;
		}
	}

	public int duration
	{
		get
		{
			if (_phases.Count != 2)
			{
				return 0;
			}
			return _phases[0].duration + _phases[0].eventDelayTime + _phases[1].duration + _phases[1].eventDelayTime;
		}
	}

	public new string name
	{
		get
		{
			return base.gameObject.name;
		}
		set
		{
			base.gameObject.name = $"Projectile-{value:D9}";
		}
	}

	private void Awake()
	{
		_phases = new List<ProjectileMotionPhase>();
		_projectileCache = CacheManager.instance.ProjectileCache;
		_controllerCache = CacheManager.instance.ControllerCache;
		if (timeGameObject == null)
		{
			timeGameObject = GetComponent<TimedGameObject>();
			if (timeGameObject == null)
			{
				timeGameObject = base.gameObject.AddComponent<TimedGameObject>();
				timeGameObject.groupType = ETimeGroupType.ProjectileGroup;
				timeGameObject.TimeGroupController = TimeControllerManager.instance.Projectile;
				timeGameObject.SearchObjects = false;
			}
		}
	}

	private void Release()
	{
		for (int i = 0; i < _phases.Count; i++)
		{
			_projectileCache.Release(_phases[i]);
		}
		_phases.Clear();
		_controllerCache.Release(this);
	}

	private void Update()
	{
		if (_phases.Count == 0 || _phases.Count > 2)
		{
			Release();
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = (int)(1000f * ((!(timeGameObject != null)) ? Time.deltaTime : timeGameObject.TimedDeltaTime()));
		if (_phases.Count > 0)
		{
			int num4 = 0;
			while (num4 < _phases.Count)
			{
				ProjectileMotionPhase projectileMotionPhase = _phases[num4];
				if (!projectileMotionPhase.bEnabel)
				{
					num4++;
					continue;
				}
				int num5 = projectileMotionPhase.elapsedTime;
				if (num5 == 0)
				{
					num5 = -1;
				}
				if (projectileMotionPhase.gameObject.activeSelf)
				{
					_UpdateMotionPhase(projectileMotionPhase, num5);
				}
				projectileMotionPhase.elapsedTime += num2 + num3;
				num2 = 0;
				if (!projectileMotionPhase.bFinishEventTime)
				{
					num = Mathf.Max(0, projectileMotionPhase.elapsedTime - (projectileMotionPhase.eventTime + projectileMotionPhase.eventDelayTime));
					if (num > 0)
					{
						projectileMotionPhase.bFinishEventTime = true;
						int num6 = num4 + 1;
						if (num6 < _phases.Count)
						{
							num2 = num;
							_phases[num6].bEnabel = true;
							_phases[num6].gameObject.SetActive(_phases[num6].bRender);
						}
					}
				}
				num = Mathf.Max(0, projectileMotionPhase.elapsedTime - projectileMotionPhase.duration);
				if (num > 0)
				{
					projectileMotionPhase.gameObject.SetActive(value: false);
					if (projectileMotionPhase.bFinishEventTime)
					{
						projectileMotionPhase.elapsedTime = 0;
						_projectileCache.Release(projectileMotionPhase);
						_phases.RemoveAt(num4);
						continue;
					}
				}
				num4++;
			}
		}
		else
		{
			Release();
		}
	}

	public ProjectileMotionPhase Create(string key, Vector3 position, bool bRender = true)
	{
		int num = _projectileCache.elements.FindIndex((CacheWithPoolElement x) => x.key == key);
		if (num < 0)
		{
			Release();
			return null;
		}
		return Create(num, position, bRender);
	}

	public ProjectileMotionPhase Create(int projectileId, Vector3 position, bool bRender = true)
	{
		ProjectileMotionPhase projectileMotionPhase = _projectileCache.Create(projectileId, base.transform);
		if (projectileMotionPhase == null)
		{
			Release();
			return null;
		}
		projectileMotionPhase.transform.position = position;
		if (_phases.Count == 0)
		{
			projectileMotionPhase.bEnabel = true;
			projectileMotionPhase.bRender = bRender;
			projectileMotionPhase.bFinishEventTime = false;
			projectileMotionPhase.gameObject.SetActive(bRender);
		}
		else
		{
			projectileMotionPhase.bEnabel = false;
			projectileMotionPhase.bRender = bRender;
			projectileMotionPhase.bFinishEventTime = false;
			projectileMotionPhase.gameObject.SetActive(value: false);
		}
		if (projectileMotionPhase.eventTime == -1)
		{
			projectileMotionPhase.eventTime = projectileMotionPhase.duration;
		}
		projectileMotionPhase.elapsedTime = 0;
		_phases.Add(projectileMotionPhase);
		return projectileMotionPhase;
	}

	private void _UpdateMotionPhase(ProjectileMotionPhase phase, float lastElapsedTime)
	{
		for (int i = 0; i < phase.activationEvents.Length; i++)
		{
			int time = phase.activationEvents[i].time;
			if ((float)time > lastElapsedTime && time <= phase.elapsedTime)
			{
				GameObject target = phase.activationEvents[i].target;
				if (target != null)
				{
					target.SetActive(phase.activationEvents[i].value);
				}
			}
		}
		for (int j = 0; j < phase.extraEffects.Length; j++)
		{
			int creationTime = phase.extraEffects[j].creationTime;
			if (!((float)creationTime > lastElapsedTime) || creationTime > phase.elapsedTime)
			{
				continue;
			}
			GameObject prefab = phase.extraEffects[j].prefab;
			if (prefab != null)
			{
				GameObject gameObject = Object.Instantiate(prefab);
				Transform parent = phase.transform.parent;
				SWP_TimedGameObject component = parent.GetComponent<SWP_TimedGameObject>();
				SWP_TimedGameObject sWP_TimedGameObject = gameObject.AddComponent<SWP_TimedGameObject>();
				List<Object> list = new List<Object>();
				sWP_TimedGameObject.ControllerGroupID = component.ControllerGroupID;
				sWP_TimedGameObject.SearchObjects = false;
				sWP_TimedGameObject.AssignedObjects = Utility.FindTimedObjects(gameObject.transform);
				Transform position = phase.extraEffects[j].position;
				if (position == null)
				{
					position = phase.transform;
				}
				gameObject.transform.position = position.position;
				if (gameObject.GetComponent<DestructionTimer>() == null)
				{
					Object.Destroy(gameObject.gameObject, phase.extraEffects[j].destroyTime);
				}
			}
		}
	}
}
