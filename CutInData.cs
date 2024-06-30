using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CutInData : MonoBehaviour
{
	public delegate void Delegate();

	public CutInController cutInController;

	public Animation animation;

	public E_TARGET_SIDE side;

	public List<AbstractCutInObject> datas;

	[HideInInspector]
	public string key;

	[HideInInspector]
	public CutInPlayer cutInPlayer;

	public Delegate _Enter;

	public Delegate _Exit;

	protected int setupPlayTimeTick = -1;

	protected float durationTime;

	protected float elapsedTime;

	public virtual bool IsEnd => (elapsedTime >= durationTime) ? true : false;

	public void Load()
	{
		animation = GetComponent<Animation>();
		if (animation == null)
		{
		}
		datas = new List<AbstractCutInObject>(base.transform.GetComponentsInChildren<AbstractCutInObject>());
		cutInController = GameObject.Find("CutInController").GetComponent<CutInController>();
		for (int i = 0; i < datas.Count; i++)
		{
			datas[i].cutInData = this;
			datas[i].cutInController = cutInController;
		}
	}

	private void Update()
	{
		elapsedTime += Time.deltaTime;
		if (!Application.isPlaying)
		{
			UpdateStatus();
		}
	}

	public void StartData(CutInPlayer cutInPlayer)
	{
		int playCnt = 0;
		this.cutInPlayer = cutInPlayer;
		side = cutInPlayer.Side;
		TimeSet(cutInPlayer.playTimeTick, cutInPlayer.durationTimeTick);
		if (_Enter != null)
		{
			_Enter();
		}
		cutInController = Manager<CutInController>.GetInstance();
		for (int i = 0; i < datas.Count; i++)
		{
			datas[i]._Enter = delegate
			{
				playCnt++;
			};
			datas[i]._Exit = delegate
			{
				playCnt--;
				if (playCnt <= 0 && _Exit != null)
				{
					_Exit();
				}
			};
			datas[i].StartData(this);
		}
	}

	protected void TimeSet(int playTimeTick, int durationTimeTick)
	{
		elapsedTime = 0f;
		setupPlayTimeTick = -1;
		durationTime = -1f;
		if (playTimeTick > 0)
		{
			setupPlayTimeTick = playTimeTick;
		}
		if (durationTimeTick > 0)
		{
			durationTime = (float)durationTimeTick / 1000f;
		}
		if (animation != null)
		{
			animation.Rewind();
		}
		UpdateTimeData();
	}

	protected void UpdateTimeData()
	{
		if (!(animation != null))
		{
			return;
		}
		float num = 0f;
		foreach (AnimationState item in animation)
		{
			num = item.length;
			if (setupPlayTimeTick > 0)
			{
				float num2 = Mathf.Max(0f, item.length - item.time);
				item.speed = num2 * 1000f / (float)setupPlayTimeTick;
				num = (float)setupPlayTimeTick / 1000f;
			}
			if (durationTime == -1f)
			{
				durationTime = num;
			}
		}
	}

	protected void UpdateStatus()
	{
		for (int i = 0; i < datas.Count; i++)
		{
			if (datas[i].CanEditModeUpdate())
			{
				datas[i].StartEdit();
				datas[i].UpdateStatus();
			}
		}
	}
}
