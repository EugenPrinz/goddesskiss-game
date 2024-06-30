using System;
using System.Collections;
using Cache;
using Shared.Battle;
using Shared.Regulation;
using UnityEngine;

public class FireEffect : MonoBehaviour, ICacheItem
{
	[Serializable]
	public class TargetUnit
	{
		[HideInInspector]
		public UnitRenderer unitRenderer;

		public EffectPhaseRenderer effectPhaseRenderer;

		public void Set(UnitRenderer actor, UnitRenderer target)
		{
			unitRenderer = target;
			if (effectPhaseRenderer != null)
			{
				effectPhaseRenderer.Set(actor, target);
			}
		}
	}

	[Serializable]
	public class Effect
	{
		[Serializable]
		public class Option
		{
			public bool timeSleepDuringFire = true;

			public bool fireRender = true;

			[Range(0f, 10000f)]
			public int fireDelayTime;

			public bool hitRender = true;

			[Range(0f, 10000f)]
			public int hitDelayTime;

			[Range(0f, 5f)]
			public int depth;

			[Range(0f, 5f)]
			public int minimumRenderDepth;
		}

		[Range(0f, 20000f)]
		public int duration;

		[Range(-1f, 20000f)]
		public int eventTime = -1;

		public Option option;

		public GameObject root;

		public TargetUnit actor;

		public TargetUnit target;

		[NonSerialized]
		public bool enable;

		[NonSerialized]
		public int elapsedTime;

		[NonSerialized]
		public bool finishEventTime;
	}

	public delegate void OnFireDelegate();

	public TimedGameObject timeGameObject;

	public OnFireDelegate OnFire;

	[HideInInspector]
	public bool enableEffect;

	[HideInInspector]
	public bool canInterfereSkill;

	[HideInInspector]
	public Option option;

	public Effect on;

	public Effect off;

	[HideInInspector]
	public FireEvent fireEvent;

	[HideInInspector]
	public string actionEffSound;

	[HideInInspector]
	public EActionEffWithFireType actionEffWithFireType;

	[HideInInspector]
	public string actionSound;

	[HideInInspector]
	public int actionSoundDelay;

	protected TimeGroupController timeGroup;

	protected bool enableUpdate;

	protected int elapsedTime;

	protected CacheWithPool _fireEffCache;

	protected bool beFire;

	protected int _timeStack;

	public int CacheID { get; set; }

	public GameObject CacheObj => base.gameObject;

	public int hitDelayTime
	{
		get
		{
			if (enableEffect)
			{
				return on.option.hitDelayTime;
			}
			return off.option.hitDelayTime;
		}
	}

	public int fireDelayTime
	{
		get
		{
			if (enableEffect)
			{
				return on.option.fireDelayTime;
			}
			return off.option.fireDelayTime;
		}
	}

	public int renderDepth
	{
		get
		{
			if (enableEffect)
			{
				return on.option.minimumRenderDepth;
			}
			return off.option.minimumRenderDepth;
		}
	}

	public bool timeSleepDuringFire
	{
		get
		{
			if (enableEffect)
			{
				return on.option.timeSleepDuringFire;
			}
			return off.option.timeSleepDuringFire;
		}
	}

	private void Awake()
	{
		timeGameObject.ControllerGroupID = 4;
		timeGameObject.groupType = ETimeGroupType.SkillActorGroup;
		_fireEffCache = CacheManager.instance.FireEffectCache;
		if (TimeControllerManager.HasInstance)
		{
			timeGroup = TimeControllerManager.instance.SkillActor;
		}
		if (on.eventTime == -1)
		{
			on.eventTime = on.duration;
		}
	}

	public void Set(UnitRenderer actor, UnitRenderer target, bool effect)
	{
		on.actor.Set(actor, actor);
		on.target.Set(actor, target);
		off.actor.Set(actor, actor);
		off.target.Set(actor, target);
		beFire = false;
		enableEffect = effect;
	}

	private void OnEnable()
	{
		OnFire = null;
		elapsedTime = 0;
		enableUpdate = false;
		on.root.SetActive(value: false);
		off.root.SetActive(value: false);
		on.elapsedTime = 0;
		on.finishEventTime = false;
		off.elapsedTime = 0;
		off.finishEventTime = false;
		if (timeGroup != null)
		{
			StartCoroutine(Play());
		}
	}

	private IEnumerator Play()
	{
		yield return null;
		if (enableEffect)
		{
			if (!string.IsNullOrEmpty(actionEffSound) && actionEffSound != "0")
			{
				SoundManager.PlaySFX(actionEffSound);
			}
			on.enable = true;
			if (on.duration > 0)
			{
				on.root.SetActive(value: true);
			}
			else
			{
				on.root.SetActive(value: false);
			}
			off.enable = false;
			UISetter.SetActive(off.root, active: false);
		}
		else
		{
			on.enable = false;
			UISetter.SetActive(on.root, active: false);
			off.enable = true;
			if (off.duration > 0)
			{
				if (off.option.depth >= renderDepth)
				{
					off.root.SetActive(value: true);
				}
			}
			else
			{
				off.root.SetActive(value: false);
			}
		}
		if (off.eventTime == -1)
		{
			off.eventTime = fireEvent.time;
		}
		off.actor.unitRenderer.timedGameObject.ChangeTimeGroup(ETimeGroupType.SkillActorGroup);
		if (!canInterfereSkill && timeSleepDuringFire)
		{
			TimeControllerManager.instance.Battle.PauseGroupTime();
		}
		_timeStack = 0;
		enableUpdate = true;
	}

	private void Update()
	{
		if (!enableUpdate)
		{
			return;
		}
		_timeStack += (int)(1000f * timeGroup.TimedDeltaTime());
		while (_timeStack >= 66)
		{
			_timeStack -= 66;
			bool flag = false;
			if (elapsedTime < fireDelayTime + fireEvent.time + 66)
			{
				flag = true;
				if (Simulator.HasTimeEvent(fireDelayTime, elapsedTime))
				{
					float fNewSpeed = 100f * (float)fireEvent.time / (float)off.eventTime;
					off.actor.unitRenderer.timedGameObject.SetTimeSpeed(fNewSpeed);
					if (OnFire != null)
					{
						OnFire();
					}
				}
				if ((!enableEffect || actionEffWithFireType == EActionEffWithFireType.None) && Simulator.HasTimeEvent(fireDelayTime + actionSoundDelay, elapsedTime) && !off.actor.unitRenderer.IsDead && !string.IsNullOrEmpty(actionSound) && actionSound != "0")
				{
					SoundManager.PlaySFX(actionSound);
				}
				if (Simulator.HasTimeEvent(fireDelayTime + fireEvent.time, elapsedTime))
				{
					if (!canInterfereSkill && timeSleepDuringFire)
					{
						TimeControllerManager.instance.Battle.ResumeGroupTime();
					}
					off.actor.unitRenderer.timedGameObject.SetTimeSpeed(100f, update: false);
					off.actor.unitRenderer.timedGameObject.ChangeTimeGroup(ETimeGroupType.EtcGroup);
				}
			}
			int num = 0;
			int num2 = 66;
			elapsedTime += num2;
			if (on.enable)
			{
				flag = true;
				on.elapsedTime += num2;
				if (!on.finishEventTime)
				{
					num = Mathf.Max(0, on.elapsedTime - on.eventTime);
					if (num > 0)
					{
						on.finishEventTime = true;
						num2 = 0;
						off.enable = true;
						off.elapsedTime = num;
						off.finishEventTime = false;
						if (off.duration > 0 && off.option.depth >= renderDepth)
						{
							off.root.SetActive(value: true);
						}
					}
				}
				if (Mathf.Max(0, on.elapsedTime - on.duration) > 0)
				{
					on.root.SetActive(value: false);
					if (on.finishEventTime)
					{
						on.enable = false;
					}
				}
			}
			if (off.enable)
			{
				flag = true;
				off.elapsedTime += num2;
				if (!off.finishEventTime && Mathf.Max(0, off.elapsedTime - off.eventTime) > 0)
				{
					off.finishEventTime = true;
				}
				if (Mathf.Max(0, off.elapsedTime - off.duration) > 0)
				{
					off.root.SetActive(value: false);
					if (off.finishEventTime)
					{
						off.enable = false;
					}
				}
			}
			if (!flag)
			{
				if (_fireEffCache == null)
				{
					UnityEngine.Object.DestroyImmediate(base.gameObject);
				}
				else
				{
					_fireEffCache.Release(this);
				}
				break;
			}
		}
	}
}
