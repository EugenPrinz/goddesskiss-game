using System.Collections.Generic;
using UnityEngine;

public class TimeControllerManager : MonoBehaviour
{
	protected static TimeControllerManager _instance;

	public List<TimeGroupController> controllers;

	protected Dictionary<ETimeGroupType, TimeGroupController> _controllers;

	private TimeGroupController _gameMain;

	private TimeGroupController _battle;

	private TimeGroupController _cutin;

	private TimeGroupController _skillActor;

	private TimeGroupController _projectile;

	private TimeGroupController _etc;

	public static TimeControllerManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType<TimeControllerManager>();
			}
			return _instance;
		}
	}

	public static bool HasInstance => _instance != null;

	public TimeGroupController GameMain
	{
		get
		{
			if (_gameMain == null)
			{
				_gameMain = GetController(ETimeGroupType.GameMainGroup);
			}
			return _gameMain;
		}
	}

	public TimeGroupController Battle
	{
		get
		{
			if (_battle == null)
			{
				_battle = GetController(ETimeGroupType.BattleGroup);
			}
			return _battle;
		}
	}

	public TimeGroupController CutIn
	{
		get
		{
			if (_cutin == null)
			{
				_cutin = GetController(ETimeGroupType.CutInGroup);
			}
			return _cutin;
		}
	}

	public TimeGroupController SkillActor
	{
		get
		{
			if (_skillActor == null)
			{
				_skillActor = GetController(ETimeGroupType.SkillActorGroup);
			}
			return _skillActor;
		}
	}

	public TimeGroupController Projectile
	{
		get
		{
			if (_projectile == null)
			{
				_projectile = GetController(ETimeGroupType.ProjectileGroup);
			}
			return _projectile;
		}
	}

	public TimeGroupController ETC
	{
		get
		{
			if (_etc == null)
			{
				_etc = GetController(ETimeGroupType.EtcGroup);
			}
			return _etc;
		}
	}

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Object.DestroyImmediate(this);
		}
		else
		{
			_instance = this;
		}
		RefreshElementDict();
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	public TimeGroupController GetController(ETimeGroupType groupType)
	{
		if (_controllers == null)
		{
			RefreshElementDict();
		}
		if (!_controllers.ContainsKey(groupType))
		{
			return null;
		}
		return _controllers[groupType];
	}

	protected void RefreshElementDict()
	{
		_controllers = new Dictionary<ETimeGroupType, TimeGroupController>();
		for (int i = 0; i < controllers.Count; i++)
		{
			_controllers.Add(controllers[i].groupType, controllers[i]);
		}
	}
}
