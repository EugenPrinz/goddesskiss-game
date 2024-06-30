using System.Collections;
using System.Collections.Generic;
using Cache;
using Shared.Battle;
using Shared.Regulation;
using UnityEngine;

public class CutInDataSet : MonoBehaviour, ICacheItem
{
	public delegate void Delegate();

	public E_TARGET_SIDE side;

	[Range(-1f, 10000f)]
	public int fireTimeTick = -1;

	public List<CutInPlayer> datas;

	[HideInInspector]
	public bool isCommander;

	[HideInInspector]
	public int commanderDri;

	[HideInInspector]
	public Unit unit;

	[HideInInspector]
	public Skill skill;

	[HideInInspector]
	public UnitRenderer owner;

	[HideInInspector]
	public UnitRenderer enemy;

	[HideInInspector]
	public TimedGameObject ownerTimeObject;

	[HideInInspector]
	public FireEvent fireEvent;

	[HideInInspector]
	public float timeScale;

	public Delegate _Enter;

	public Delegate _Exit;

	protected int playCnt;

	protected float elapsedTime;

	protected float firstFireEventTime;

	protected bool bAfterFire;

	public int CacheID { get; set; }

	public GameObject CacheObj => base.gameObject;

	public string Key => base.name;

	private IEnumerator Play()
	{
		yield return null;
		Manager<CutInController>.GetInstance().Play(this);
	}

	private void OnEnable()
	{
		StartCoroutine(Play());
	}

	public void Load()
	{
		datas = new List<CutInPlayer>(base.transform.GetComponentsInChildren<CutInPlayer>());
	}

	public void Release()
	{
		CacheManager.instance.CutInEffectCache.Release(this);
	}

	private void Update()
	{
	}

	public void StartData()
	{
		if (datas.Count <= 0)
		{
			Object.DestroyImmediate(this);
			return;
		}
		bAfterFire = false;
		elapsedTime = 0f;
		firstFireEventTime = 0f;
		timeScale = 1f;
		if (owner != null)
		{
			ownerTimeObject = owner.timedGameObject;
			firstFireEventTime = (float)(fireEvent.time - 66) * 0.001f;
			if (fireTimeTick > 0)
			{
				timeScale = (float)(fireEvent.time - 66) / (float)fireTimeTick;
				ownerTimeObject.SetTimeSpeed(timeScale * 100f, update: false);
			}
			ownerTimeObject.ChangeTimeGroup(ETimeGroupType.SkillActorGroup);
			TimeControllerManager.instance.Battle.PauseGroupTime();
			owner.SetRenderQueueForCutIn();
		}
		if (_Enter != null)
		{
			_Enter();
		}
		playCnt = 0;
		for (int i = 0; i < datas.Count; i++)
		{
			datas[i]._Enter = delegate
			{
				playCnt++;
			};
			datas[i]._Exit = delegate
			{
				playCnt--;
				if (playCnt <= 0)
				{
					if (ownerTimeObject != null && !bAfterFire)
					{
						ownerTimeObject.SetTimeSpeed(100f, update: false);
						ownerTimeObject.ChangeTimeGroup(ETimeGroupType.EtcGroup, update: false);
						TimeControllerManager.instance.Battle.ResumeGroupTime();
						bAfterFire = true;
					}
					if (owner != null && owner.gameObject.activeSelf)
					{
						owner.ResetRenderQueue();
					}
					if (_Exit != null)
					{
						_Exit();
					}
				}
			};
			datas[i].key = i.ToString();
			datas[i].StartData(this);
		}
	}
}
