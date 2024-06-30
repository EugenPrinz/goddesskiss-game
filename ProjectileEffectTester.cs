using System;
using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class ProjectileEffectTester : MonoBehaviour
{
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

	public List<ProjectileMotionPhase> firePhaseList;

	public List<ProjectileMotionPhase> hitPhaseList;

	public List<ProjectileMotionPhase> missPhaseList;

	[Range(0f, 100f)]
	public float missRate;

	[HideInInspector]
	public List<string> attackUnitIdList;

	[HideInInspector]
	public List<string> defenseUnitIdList;

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
	public List<string> _defenseUnitIdList = new List<string>();

	[HideInInspector]
	public List<UnitRenderer> _attackUnitList = new List<UnitRenderer>();

	[HideInInspector]
	public List<UnitRenderer> _defenseUnitList = new List<UnitRenderer>();

	private bool _swap;

	private void Start()
	{
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
		if (_RefreshUnitList(attackUnitIdList, ref _attackUnitIdList))
		{
			_RefreshUnit(ref _attackUnitList, _attackUnitIdList, sceneManager.lhsTroopAnchor);
		}
		if (_RefreshUnitList(defenseUnitIdList, ref _defenseUnitIdList))
		{
			_RefreshUnit(ref _defenseUnitList, _defenseUnitIdList, sceneManager.rhsTroopAnchor);
		}
	}

	private void _RefreshUnit(ref List<UnitRenderer> unitList, List<string> idList, Transform anchorRoot)
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
		UnitRendererCache unitRendererCache = UnityEngine.Object.FindObjectOfType<UnitRendererCache>();
		for (int i = 0; i < idList.Count; i++)
		{
			string text = idList[i];
			if (text == EmptySlotString)
			{
				unitList.Add(null);
				continue;
			}
			Transform transform = anchorRoot.Find(i.ToString("00"));
			UnitRenderer unitRenderer = unitRendererCache.Create(text);
			Transform transform2 = unitRenderer.gameObject.transform;
			transform2.parent = anchorRoot;
			transform2.localPosition = transform.localPosition;
			unitRenderer.gameObject.name = int.Parse(text.Substring("Unit-".Length)).ToString();
			unitList.Add(unitRenderer);
		}
	}

	private bool _RefreshUnitList(List<string> src, ref List<string> dest)
	{
		if (dest == null)
		{
			dest = new List<string>();
		}
		if (src.Count != dest.Count)
		{
			dest.Clear();
			dest.AddRange(src);
			return true;
		}
		bool result = false;
		for (int i = 0; i < src.Count; i++)
		{
			if (src[i] != dest[i])
			{
				result = true;
				dest[i] = src[i];
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
				unit.PlayAnimation(animationName);
				StartCoroutine(_Fire(unit, defenderList, animationName));
			}
		});
	}

	private IEnumerator _Fire(UnitRenderer attacker, List<UnitRenderer> targetList, string animationName)
	{
		Regulation regulation = sceneManager.regulation;
		DataTable<UnitMotionDataRow> unitMotionDtbl = regulation.unitMotionDtbl;
		string key = attacker.gameObject.name + "/" + animationName;
		if (!unitMotionDtbl.ContainsKey(key))
		{
			yield break;
		}
		UnitMotionDataRow unitMotionDataRow = unitMotionDtbl[key];
		IList<FireEvent> fireEvents = unitMotionDataRow.fireEvents;
		for (int i = 0; i < fireEvents.Count; i++)
		{
			FireEvent fireEvent = fireEvents[i];
			if (fireEvent != null)
			{
				UnitRenderer unitRenderer = targetList[UnityEngine.Random.Range(0, targetList.Count)];
				StartCoroutine(_LaunchProjectile(fireEvent.time, attacker, fireEvent.firePointBonePath, unitRenderer.transform.position, unitRenderer));
			}
		}
	}

	private IEnumerator _LaunchProjectile(int delayMS, UnitRenderer attacker, string launchBonePath, Vector3 hitPos, UnitRenderer beHitUnit)
	{
		yield return new WaitForSeconds((float)delayMS * 0.001f);
		GameObject go = new GameObject();
		ProjectileRenderer pr = go.AddComponent<ProjectileRenderer>();
		bool isMiss = UnityEngine.Random.value < missRate * 0.01f;
		ProjectileMotionPhase firePhase = _CreateProjectilePhase(firePhaseList);
		ProjectileMotionPhase hitPhase = _CreateProjectilePhase((!isMiss) ? hitPhaseList : missPhaseList);
		pr.name = firePhase.gameObject.name + " / " + hitPhase.gameObject.name;
		firePhase.transform.parent = pr.transform;
		hitPhase.transform.parent = pr.transform;
		firePhase.transform.position = attacker.GetBone(launchBonePath).position;
		hitPhase.transform.position = hitPos;
		firePhase.gameObject.SetActive(value: true);
		hitPhase.gameObject.SetActive(value: false);
		yield return new WaitForSeconds((float)(firePhase.duration + hitPhase.eventTime) * 0.001f);
		if (!isMiss)
		{
			beHitUnit.PlayAnimation("behit");
		}
		else
		{
			beHitUnit.PlayAnimation("avoid");
		}
		SWP_TimedGameObject tgo2 = pr.GetComponent<SWP_TimedGameObject>();
		if (tgo2 == null)
		{
			tgo2 = go.gameObject.AddComponent<SWP_TimedGameObject>();
			tgo2.ControllerGroupID = timeGroupController.GroupID;
			tgo2.TimeGroupController = timeGroupController;
			tgo2.AssignedObjects = pr.FindTimedObjects();
			tgo2.SearchObjects = true;
		}
	}

	private ProjectileMotionPhase _CreateProjectilePhase(List<ProjectileMotionPhase> prefabList)
	{
		ProjectileMotionPhase original = prefabList[UnityEngine.Random.Range(0, prefabList.Count)];
		ProjectileMotionPhase projectileMotionPhase = UnityEngine.Object.Instantiate(original);
		return projectileMotionPhase.GetComponent<ProjectileMotionPhase>();
	}

	public bool IsEmptySlot(string id)
	{
		return string.IsNullOrEmpty(id) || id == EmptySlotString;
	}
}
