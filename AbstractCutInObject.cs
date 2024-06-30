using System.Collections;
using UnityEngine;

public abstract class AbstractCutInObject : MonoBehaviour, ICutInObject
{
	public delegate void Delegate();

	public CutInData cutInData;

	public CutInController cutInController;

	public Delegate _Enter;

	public Delegate _Exit;

	protected float Lerp = 1f;

	public abstract string ID { get; }

	public virtual void StartData(CutInData cutInData)
	{
		this.cutInData = cutInData;
		cutInController = cutInData.cutInController;
		Manager<CutInController>.GetInstance().Play(this);
	}

	public virtual void Play()
	{
		if (_Enter != null)
		{
			_Enter();
		}
		Lerp = 0f;
		if (cutInData.cutInPlayer.needResetData)
		{
			Lerp = 1f;
		}
		StartCoroutine("CutInUpdate");
	}

	public virtual void Stop()
	{
		if (!(this == null))
		{
			StopCoroutine("CutInUpdate");
			cutInController.OnStop(this);
			if (_Exit != null)
			{
				_Exit();
			}
		}
	}

	public virtual IEnumerator CutInUpdate()
	{
		EnterStatus();
		while (!cutInData.IsEnd)
		{
			UpdateStatus();
			Lerp += Time.deltaTime;
			if (Lerp > 1f)
			{
				Lerp = 1f;
			}
			yield return null;
		}
		ExitStatus();
		yield return null;
	}

	public virtual bool CanEditModeUpdate()
	{
		return true;
	}

	public virtual void StartEdit()
	{
	}

	public virtual void EnterStatus()
	{
		if (Application.isPlaying)
		{
			cutInController.OnPlay(this);
		}
	}

	public virtual void UpdateStatus()
	{
	}

	public virtual void ExitStatus()
	{
		cutInController.OnStop(this);
		if (_Exit != null)
		{
			_Exit();
		}
	}
}
