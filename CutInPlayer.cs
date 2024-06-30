using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutInPlayer : MonoBehaviour
{
	public enum E_TYPE
	{
		OWNER,
		ENEMY
	}

	public delegate void Delegate();

	public E_TYPE type;

	[Range(0f, 10000f)]
	public int delay;

	[Range(-1f, 10000f)]
	public int playTimeTick = -1;

	[Range(-1f, 10000f)]
	public int durationTimeTick = -1;

	public bool needResetData;

	public List<CutInData> datas;

	[HideInInspector]
	public string key;

	[HideInInspector]
	public UnitRenderer owner;

	[HideInInspector]
	public UnitRenderer enemy;

	[HideInInspector]
	public CutInDataSet cutInDataSet;

	public Delegate _Enter;

	public Delegate _Exit;

	protected CutInData data;

	protected E_TARGET_SIDE side;

	protected ObjectPool<string, CutInData> pool = new ObjectPool<string, CutInData>();

	protected int playCnt;

	public virtual bool CanPlay
	{
		get
		{
			if (data == null)
			{
				return false;
			}
			if (data.IsEnd)
			{
				return false;
			}
			return true;
		}
	}

	public virtual E_TARGET_SIDE Side
	{
		get
		{
			return side;
		}
		set
		{
			side = value;
			if (type == E_TYPE.ENEMY)
			{
				if (side == E_TARGET_SIDE.LEFT)
				{
					side = E_TARGET_SIDE.RIGHT;
				}
				else
				{
					side = E_TARGET_SIDE.LEFT;
				}
			}
		}
	}

	public virtual CutInData Data => data;

	public void StartData(CutInDataSet cutInDataSet)
	{
		if (datas.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < datas.Count; i++)
		{
			if (datas[i] == null)
			{
				return;
			}
		}
		if (playTimeTick != 0 && durationTimeTick != 0)
		{
			this.cutInDataSet = cutInDataSet;
			playCnt = 0;
			Side = cutInDataSet.side;
			owner = cutInDataSet.owner;
			enemy = cutInDataSet.enemy;
			if (cutInDataSet.skill.SkillDataRow.targetType != ESkillTargetType.Friend || type != E_TYPE.ENEMY)
			{
				StartCoroutine("Play");
			}
		}
	}

	public void OnPlay()
	{
		playCnt++;
	}

	public void OnStop()
	{
		playCnt--;
		if (playCnt <= 0 && _Exit != null)
		{
			_Exit();
		}
	}

	private IEnumerator Play()
	{
		if (_Enter != null)
		{
			_Enter();
		}
		yield return new WaitForSeconds((float)delay / 1000f);
		int rnd = Random.Range(0, datas.Count);
		string strRnd = rnd.ToString();
		data = pool.Pop(strRnd);
		if (data == null)
		{
			data = Object.Instantiate(datas[rnd]);
			data.key = strRnd;
		}
		data.transform.parent = base.transform;
		data._Exit = delegate
		{
			pool.Push(data.key, data);
			if (_Exit != null)
			{
				_Exit();
			}
		};
		data.StartData(this);
	}
}
