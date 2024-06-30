using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRenderer : MonoBehaviour
{
	public const string PrefabFolderPath = "Assets/Prefabs/Projectiles";

	public int id;

	private ProjectileMotionPhase[] _phases;

	private int destroyCount;

	private static readonly HashSet<Type> _TimedObjectTypeSet = new HashSet<Type>
	{
		typeof(Animation),
		typeof(Animator),
		typeof(ParticleSystem),
		typeof(Rigidbody),
		typeof(Rigidbody2D),
		typeof(AudioSource)
	};

	private void Update()
	{
		if (_phases == null)
		{
			_phases = GetComponentsInChildren<ProjectileMotionPhase>(includeInactive: true);
			if (_phases.Length != 2)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
		}
		bool flag = false;
		int num = 0;
		SWP_TimedGameObject component = GetComponent<SWP_TimedGameObject>();
		int num2 = (int)(1000f * ((!(component != null)) ? Time.deltaTime : component.TimedDeltaTime()));
		for (int i = 0; i < _phases.Length; i++)
		{
			ProjectileMotionPhase projectileMotionPhase = _phases[i];
			if (projectileMotionPhase == null || !projectileMotionPhase.bEnabel)
			{
				continue;
			}
			int num3 = projectileMotionPhase.elapsedTime;
			if (num3 == 0)
			{
				num3 = -1;
			}
			if (projectileMotionPhase.gameObject.activeSelf)
			{
				_UpdateMotionPhase(projectileMotionPhase, num3);
			}
			flag = true;
			projectileMotionPhase.elapsedTime += num + num2;
			if (!projectileMotionPhase.bFinishEventTime)
			{
				num = Mathf.Max(0, projectileMotionPhase.elapsedTime - projectileMotionPhase.eventTime);
				if (num > 0)
				{
					projectileMotionPhase.bFinishEventTime = true;
					int num4 = i + 1;
					if (num4 < _phases.Length)
					{
						_phases[num4].bEnabel = true;
						_phases[num4].gameObject.SetActive(value: true);
					}
				}
			}
			num = Mathf.Max(0, projectileMotionPhase.elapsedTime - projectileMotionPhase.duration);
			if (num > 0)
			{
				projectileMotionPhase.gameObject.SetActive(value: false);
				if (projectileMotionPhase.bFinishEventTime)
				{
					_phases[i] = null;
				}
			}
		}
		if (!flag)
		{
			_phases = null;
			destroyCount++;
			UnityEngine.Object.Destroy(base.gameObject);
		}
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
				GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
				Transform parent = phase.transform.parent;
				SWP_TimedGameObject component = parent.GetComponent<SWP_TimedGameObject>();
				SWP_TimedGameObject sWP_TimedGameObject = gameObject.AddComponent<SWP_TimedGameObject>();
				List<UnityEngine.Object> list = new List<UnityEngine.Object>();
				_FindTimedObjects(gameObject.transform, list);
				sWP_TimedGameObject.ControllerGroupID = component.ControllerGroupID;
				sWP_TimedGameObject.SearchObjects = false;
				sWP_TimedGameObject.AssignedObjects = list.ToArray();
				Transform position = phase.extraEffects[j].position;
				if (position == null)
				{
					position = phase.transform;
				}
				gameObject.transform.position = position.position;
				if (gameObject.GetComponent<DestructionTimer>() == null)
				{
					UnityEngine.Object.Destroy(gameObject.gameObject, phase.extraEffects[j].destroyTime);
				}
			}
		}
	}

	public UnityEngine.Object[] FindTimedObjects()
	{
		List<UnityEngine.Object> list = new List<UnityEngine.Object>();
		_FindTimedObjects(base.transform, list);
		return list.ToArray();
	}

	private static void _FindTimedObjects(Transform t, List<UnityEngine.Object> dst)
	{
		Component[] components = t.GetComponents<Component>();
		Component[] array = components;
		foreach (Component component in array)
		{
			if (_TimedObjectTypeSet.Contains(component.GetType()))
			{
				dst.Add(component);
			}
		}
		for (int j = 0; j < t.childCount; j++)
		{
			_FindTimedObjects(t.GetChild(j), dst);
		}
	}
}
