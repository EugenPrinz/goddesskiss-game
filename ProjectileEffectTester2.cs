using System;
using System.Collections;
using System.Collections.Generic;
using Cache;
using Shared.Regulation;
using UnityEngine;

[Serializable]
public class ProjectileEffectTester2 : MonoBehaviour
{
	public class DummyProjectile
	{
		public int firePhaseId;

		public int hitPhaseId;

		public int beHitId;

		public bool isMiss;

		public bool isCri;

		public bool isSplashe;

		public int hitDelayTime;

		public bool fireRender;

		public bool hitRender;
	}

	[Serializable]
	public class FirePoint
	{
		public string projectileMotionKey;

		public float rateCri;

		public float rateMiss;

		public bool isSplashe;

		public string statusKey;
	}

	[Serializable]
	public class KeyBinder
	{
		public KeyCode key;

		public string animationName;
	}

	public static readonly string EmptySlotString = "-";

	public static readonly int SlotCount = 9;

	[HideInInspector]
	public T03_ProjectileEffect sceneManager;

	[HideInInspector]
	public SWP_TimeGroupController timeGroupController;

	[HideInInspector]
	public string attackActionKey;

	[HideInInspector]
	public string actionSound;

	[HideInInspector]
	public int activedProjectileCnt;

	[HideInInspector]
	public string cutInKey;

	[HideInInspector]
	public FirePoint[] firePoints = new FirePoint[3];

	[HideInInspector]
	public bool isSplashe;

	[HideInInspector]
	public string statusKey;

	[HideInInspector]
	public int statusTime = 2;

	public bool enableEffect = true;

	public float gameSpeed = 1f;

	[HideInInspector]
	public List<string> attackUnitIdList;

	[HideInInspector]
	public List<string> attackUnitTypes;

	[HideInInspector]
	public List<string> defenseUnitIdList;

	[HideInInspector]
	public List<string> defenseUnitTypes;

	public List<string> animationList = new List<string> { "attack", "passive", "idle", "avoid", "behit", "destroy", "injury" };

	public List<KeyBinder> hotKeyList = new List<KeyBinder>
	{
		new KeyBinder
		{
			key = KeyCode.F,
			animationName = "attack"
		},
		new KeyBinder
		{
			key = KeyCode.D,
			animationName = "passive"
		}
	};

	[HideInInspector]
	public List<string> _attackUnitIdList = new List<string>();

	[HideInInspector]
	public List<string> _attackUnitTypes = new List<string>();

	[HideInInspector]
	public List<string> _defenseUnitIdList = new List<string>();

	[HideInInspector]
	public List<string> _defenseUnitTypes = new List<string>();

	[HideInInspector]
	public List<UnitRenderer> _attackUnitList = new List<UnitRenderer>();

	[HideInInspector]
	public List<UnitRenderer> _defenseUnitList = new List<UnitRenderer>();

	private bool _swap;

	private void Awake()
	{
		Application.targetFrameRate = 30;
	}

	private void Start()
	{
		Time.timeScale = gameSpeed;
		SoundManager.SetPitchSFX(gameSpeed);
		_BeginDummyMove();
	}

	private void Update()
	{
		hotKeyList.ForEach(delegate(KeyBinder elem)
		{
			if (Input.GetKeyDown(elem.key))
			{
				Fire(elem.animationName);
			}
		});
	}

	private void _BeginDummyMove()
	{
		_attackUnitList.ForEach(delegate(UnitRenderer unit)
		{
			if (!(unit == null))
			{
				unit.BeginDummyMove();
			}
		});
		_defenseUnitList.ForEach(delegate(UnitRenderer unit)
		{
			if (!(unit == null))
			{
				unit.BeginDummyMove();
			}
		});
	}

	public void RefreshUnitList()
	{
		if (_RefreshUnitList(attackUnitIdList, attackUnitTypes, ref _attackUnitIdList, ref _attackUnitTypes))
		{
			_RefreshUnit(ref _attackUnitList, _attackUnitIdList, _attackUnitTypes, sceneManager.lhsTroopAnchor, SplitScreenDrawSide.Left);
		}
		if (_RefreshUnitList(defenseUnitIdList, defenseUnitTypes, ref _defenseUnitIdList, ref _defenseUnitTypes))
		{
			_RefreshUnit(ref _defenseUnitList, _defenseUnitIdList, _defenseUnitTypes, sceneManager.rhsTroopAnchor, SplitScreenDrawSide.Right);
		}
	}

	private void _RefreshUnit(ref List<UnitRenderer> unitList, List<string> idList, List<string> typeIds, Transform anchorRoot, SplitScreenDrawSide side)
	{
		if (unitList == null || unitList.Count <= 0)
		{
			unitList = new List<UnitRenderer>();
		}
		else
		{
			unitList.ForEach(delegate(UnitRenderer unit)
			{
				if (unit != null)
				{
					UnityEngine.Object.DestroyImmediate(unit.gameObject);
				}
			});
			unitList.Clear();
		}
		UnitCache unitCache = CacheManager.instance.UnitCache;
		for (int i = 0; i < idList.Count; i++)
		{
			string text = idList[i];
			if (text == EmptySlotString)
			{
				unitList.Add(null);
				continue;
			}
			Transform transform = anchorRoot.Find(i.ToString("00"));
			UnitRenderer unitRenderer = unitCache.Create<UnitRenderer>(text);
			Transform transform2 = unitRenderer.gameObject.transform;
			transform2.parent = anchorRoot;
			transform2.localPosition = transform.localPosition;
			unitRenderer.unitName = text;
			unitRenderer.unitIdx = i + ((side == SplitScreenDrawSide.Right) ? 9 : 0);
			unitRenderer.drawSide = side;
			unitRenderer.gameObject.name = int.Parse(text.Substring("Unit-".Length)).ToString();
			unitList.Add(unitRenderer);
			int modelType = 0;
			if (typeIds[i] != EmptySlotString)
			{
				modelType = int.Parse(typeIds[i]);
			}
			unitRenderer.SetModelType(modelType);
		}
	}

	private bool _RefreshUnitList(List<string> srcUnitList, List<string> srcTypeList, ref List<string> destUnitList, ref List<string> destTypeList)
	{
		if (destUnitList == null)
		{
			destUnitList = new List<string>();
		}
		if (destTypeList == null)
		{
			destTypeList = new List<string>();
		}
		if (srcUnitList.Count != destUnitList.Count || srcTypeList.Count != destTypeList.Count)
		{
			destUnitList.Clear();
			destUnitList.AddRange(srcUnitList);
			destTypeList.Clear();
			destTypeList.AddRange(srcTypeList);
			return true;
		}
		bool result = false;
		for (int i = 0; i < srcUnitList.Count; i++)
		{
			if (srcUnitList[i] != destUnitList[i])
			{
				result = true;
				destUnitList[i] = srcUnitList[i];
			}
		}
		for (int j = 0; j < srcTypeList.Count; j++)
		{
			if (srcTypeList[j] != destTypeList[j])
			{
				result = true;
				destTypeList[j] = srcTypeList[j];
			}
		}
		return result;
	}

	public void Swap()
	{
		_swap = !_swap;
		List<string> list = new List<string>();
		list.AddRange(attackUnitIdList);
		attackUnitIdList.Clear();
		attackUnitIdList.AddRange(defenseUnitIdList);
		defenseUnitIdList.Clear();
		defenseUnitIdList.AddRange(list);
		RefreshUnitList();
		if (Application.isPlaying)
		{
			_BeginDummyMove();
		}
	}

	public void Fire(string animationName)
	{
		StopAllCoroutines();
		activedProjectileCnt = 0;
		Regulation regulation = sceneManager.regulation;
		DataTable<UnitMotionDataRow> unitMotionDtbl = regulation.unitMotionDtbl;
		List<UnitRenderer> defenderList = new List<UnitRenderer>();
		List<UnitRenderer> list = ((!_swap) ? _defenseUnitList : _attackUnitList);
		list.ForEach(delegate(UnitRenderer unit)
		{
			if (!(unit == null))
			{
				defenderList.Add(unit);
			}
		});
		list = ((!_swap) ? _attackUnitList : _defenseUnitList);
		list.ForEach(delegate(UnitRenderer unit)
		{
			if (!(unit == null))
			{
				StartCoroutine(_Fire(unit, defenderList, animationName));
			}
		});
	}

	private DummyProjectile _CreateProjectile(FirePoint firePoint)
	{
		if (firePoint == null || string.IsNullOrEmpty(firePoint.projectileMotionKey) || firePoint.projectileMotionKey == EmptySlotString)
		{
			return null;
		}
		Regulation regulation = sceneManager.regulation;
		DataTable<ProjectileMotionPhaseDataRow> projectileMotionPhaseDtbl = regulation.projectileMotionPhaseDtbl;
		DummyProjectile dummyProjectile = new DummyProjectile();
		string projectileMotionKey = firePoint.projectileMotionKey;
		string key = projectileMotionKey + "/FirePhase";
		int num = projectileMotionPhaseDtbl.FindIndex(key);
		if (num == -1)
		{
			return null;
		}
		ProjectileMotionPhaseDataRow projectileMotionPhaseDataRow = projectileMotionPhaseDtbl[num];
		num += UnityEngine.Random.Range(0, projectileMotionPhaseDataRow.patternCount) + 1;
		projectileMotionPhaseDataRow = projectileMotionPhaseDtbl[num];
		dummyProjectile.firePhaseId = num;
		bool flag = UnityEngine.Random.value < firePoint.rateMiss * 0.01f;
		string key2 = projectileMotionKey + ((!flag) ? "/HitPhase" : "/MissPhase");
		int num2 = projectileMotionPhaseDtbl.FindIndex(key2);
		if (num2 == -1)
		{
			return null;
		}
		ProjectileMotionPhaseDataRow projectileMotionPhaseDataRow2 = projectileMotionPhaseDtbl[num2];
		num2 += UnityEngine.Random.Range(0, projectileMotionPhaseDataRow2.patternCount) + 1;
		projectileMotionPhaseDataRow2 = projectileMotionPhaseDtbl[num2];
		dummyProjectile.hitPhaseId = num2;
		if (!flag)
		{
			string key3 = projectileMotionKey + "/BeHitPhase";
			int num3 = projectileMotionPhaseDtbl.FindIndex(key3);
			if (num3 >= 0)
			{
				ProjectileMotionPhaseDataRow projectileMotionPhaseDataRow3 = projectileMotionPhaseDtbl[num3];
				num3 += UnityEngine.Random.Range(0, projectileMotionPhaseDataRow3.patternCount) + 1;
			}
			dummyProjectile.beHitId = num3;
			bool isCri = UnityEngine.Random.value < firePoint.rateCri * 0.01f;
			dummyProjectile.isCri = isCri;
		}
		dummyProjectile.isMiss = flag;
		dummyProjectile.fireRender = true;
		dummyProjectile.hitRender = true;
		dummyProjectile.isSplashe = firePoint.isSplashe;
		return dummyProjectile;
	}

	private IEnumerator _Fire(UnitRenderer attacker, List<UnitRenderer> targetList, string animationName)
	{
		yield return new WaitForFixedUpdate();
		Regulation reg = sceneManager.regulation;
		DataTable<UnitMotionDataRow> motionTable = reg.unitMotionDtbl;
		DataTable<ProjectileMotionPhaseDataRow> projectileMotionTbl = reg.projectileMotionPhaseDtbl;
		string motionKey = attacker.gameObject.name + "/" + animationName;
		if (!motionTable.ContainsKey(motionKey))
		{
			yield break;
		}
		DummyProjectile[] projectiles = new DummyProjectile[3];
		for (int i = 0; i < projectiles.Length; i++)
		{
			projectiles[i] = _CreateProjectile(firePoints[i]);
		}
		UnitMotionDataRow motionData = motionTable[motionKey];
		IList<FireEvent> fireEvents = motionData.fireEvents;
		GameSetting.instance.effect = enableEffect;
		FireEffect fireEffect = null;
		UnitRenderer targetUnit = targetList[UnityEngine.Random.Range(0, targetList.Count)];
		if (!IsEmptySlot(attackActionKey))
		{
			fireEffect = CacheManager.instance.FireEffectCache.Create<FireEffect>(attackActionKey);
			if (fireEffect != null)
			{
				fireEffect.fireEvent = motionData.fireEvents[0];
				if (!string.IsNullOrEmpty(actionSound))
				{
					fireEffect.actionEffSound = actionSound;
				}
				fireEffect.Set(attacker, targetUnit, GameSetting.instance.effect);
				for (int j = 0; j < projectiles.Length; j++)
				{
					if (projectiles[j] != null)
					{
						projectiles[j].hitDelayTime = fireEffect.hitDelayTime;
						projectiles[j].fireRender = ((!fireEffect.enableEffect) ? fireEffect.off.option.fireRender : fireEffect.on.option.fireRender);
						projectiles[j].hitRender = ((!fireEffect.enableEffect) ? fireEffect.off.option.hitRender : fireEffect.on.option.hitRender);
					}
				}
			}
		}
		int fireDelay = 0;
		if (fireEffect != null)
		{
			fireDelay = fireEffect.fireDelayTime;
		}
		int tm3 = 0;
		int dt2 = 0;
		while (true)
		{
			dt2 += (int)(1000f * TimeControllerManager.instance.SkillActor.TimedDeltaTime());
			tm3 = dt2 - fireDelay;
			if (tm3 > 0)
			{
				break;
			}
			yield return new WaitForFixedUpdate();
		}
		bool hasReturnMotion = false;
		Animation ani = attacker.GetComponent<Animation>();
		if ((bool)ani.GetClip(animationName + "_return"))
		{
			hasReturnMotion = true;
		}
		attacker.PlayAnimation(animationName, hasReturnMotion);
		int launchDelay = 0;
		if (fireEvents.Count > 0)
		{
			launchDelay = fireEvents[0].time;
			if (fireEffect != null && fireEffect.off.eventTime >= 0)
			{
				launchDelay = fireEffect.off.eventTime;
			}
		}
		dt2 = tm3;
		while (true)
		{
			dt2 += (int)(1000f * TimeControllerManager.instance.SkillActor.TimedDeltaTime());
			tm3 = dt2 - launchDelay;
			if (tm3 > 0)
			{
				break;
			}
			yield return new WaitForFixedUpdate();
		}
		for (int k = 0; k < fireEvents.Count; k++)
		{
			FireEvent fireEvent = fireEvents[k];
			if (fireEvent == null)
			{
				continue;
			}
			DummyProjectile dummyProjectile = projectiles[fireEvent.firePointTypeIndex];
			if (dummyProjectile == null)
			{
				continue;
			}
			if (dummyProjectile.isSplashe)
			{
				for (int l = 0; l < targetList.Count; l++)
				{
					StartCoroutine(_LaunchProjectile(fireEvent.time - fireEvents[0].time - tm3, attacker, fireEvent.firePointBonePath, targetList[l].transform.position, targetList[l], dummyProjectile));
				}
				continue;
			}
			if (k > 0)
			{
				targetUnit = targetList[UnityEngine.Random.Range(0, targetList.Count)];
			}
			StartCoroutine(_LaunchProjectile(fireEvent.time - fireEvents[0].time - tm3, attacker, fireEvent.firePointBonePath, targetUnit.transform.position, targetUnit, dummyProjectile));
		}
		yield return new WaitForFixedUpdate();
		if (hasReturnMotion)
		{
			while (activedProjectileCnt > 0)
			{
				yield return new WaitForFixedUpdate();
			}
			attacker.PlayAnimation(animationName + "_return");
		}
	}

	private IEnumerator _LaunchProjectile(int delayMS, UnitRenderer attacker, string launchBonePath, Vector3 hitPos, UnitRenderer beHitUnit, DummyProjectile projectile)
	{
		activedProjectileCnt++;
		int tm = 0;
		int dt = 0;
		while (true)
		{
			dt += (int)(1000f * TimeControllerManager.instance.Battle.TimedDeltaTime());
			tm = dt - delayMS;
			if (tm > 0)
			{
				break;
			}
			yield return new WaitForFixedUpdate();
		}
		Vector3 firePosition = attacker.GetBone(launchBonePath).position;
		ProjectileController projectileCtl = CacheManager.instance.ControllerCache.Create<ProjectileController>("ProjectileController");
		if (!(projectileCtl != null))
		{
			yield break;
		}
		ProjectileMotionPhase fire = projectileCtl.Create(projectile.firePhaseId, firePosition, projectile.fireRender);
		fire.Set(attacker, attacker);
		fire.SetEventDealy(projectile.hitDelayTime);
		ProjectileMotionPhase hit = projectileCtl.Create(projectile.hitPhaseId, hitPos, projectile.hitRender);
		hit.Set(attacker, beHitUnit);
		int duration = projectileCtl.duration;
		int hitTime = projectileCtl.hitTime;
		yield return new WaitForSeconds((float)projectileCtl.hitTime * 0.001f);
		if (!projectile.isMiss)
		{
			beHitUnit.PlayAnimation("behit");
		}
		else
		{
			beHitUnit.PlayAnimation("avoid");
		}
		if (projectile.beHitId > 0)
		{
			ProjectileController projectileController = CacheManager.instance.ControllerCache.Create<ProjectileController>("ProjectileController");
			if (projectileController != null)
			{
				projectileController.name = "1";
				ProjectileMotionPhase projectileMotionPhase = projectileController.Create(projectile.beHitId, hitPos);
				projectileMotionPhase.Set(attacker, beHitUnit);
			}
		}
		if (!IsEmptySlot(statusKey))
		{
			StartCoroutine(_CreateStatus(statusKey, hitPos));
		}
		yield return new WaitForSeconds((float)(duration - hitTime) * 0.001f);
		activedProjectileCnt--;
	}

	private IEnumerator _CreateStatus(string key, Vector3 hitPos)
	{
		StatusEffectController statusCtl = CacheManager.instance.ControllerCache.Create<StatusEffectController>("StatusEffectController");
		if (statusCtl != null)
		{
			statusCtl.Create(statusKey, hitPos);
		}
		yield return new WaitForSeconds(statusTime);
		statusCtl.Release();
	}

	public bool IsEmptySlot(string id)
	{
		return string.IsNullOrEmpty(id) || id == EmptySlotString;
	}
}
