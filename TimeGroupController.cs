using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TimeGroupController : SWP_TimeGroupController
{
	public delegate void ObjectDelegate();

	public ETimeGroupType groupType;

	public TimeGroupController parentController;

	public List<TimeGroupController> childControllers;

	[Range(0f, 300f)]
	public float localSpeed = 100f;

	public bool pause;

	public ObjectDelegate onAddObject;

	public ObjectDelegate onRemoveObject;

	protected Dictionary<int, TimedGameObject> _timeObjects;

	public int objectCnt => _timeObjects.Count;

	private void Awake()
	{
		GroupID = (int)groupType;
		if (_timeObjects == null)
		{
			_timeObjects = new Dictionary<int, TimedGameObject>();
		}
	}

	public void AddTimeObject(TimedGameObject timeObject)
	{
		if (_timeObjects == null)
		{
			_timeObjects = new Dictionary<int, TimedGameObject>();
		}
		_timeObjects.Add(timeObject.GetInstanceID(), timeObject);
		if (onAddObject != null)
		{
			onAddObject();
		}
	}

	public void RemoveTimeObject(TimedGameObject timeObject)
	{
		_timeObjects.Remove(timeObject.GetInstanceID());
		if (onRemoveObject != null)
		{
			onRemoveObject();
		}
	}

	protected override void BroadcastEvents(string _EventName, object _PassedObject)
	{
		if (_timeObjects != null)
		{
			Dictionary<int, TimedGameObject>.Enumerator enumerator = _timeObjects.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.Value.SendMessage(_EventName, _PassedObject, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public override void PauseGroupTime()
	{
		if (!pause)
		{
			pause = true;
			SetControllerSpeed(0f);
			SWP_InternalTimedClass passedObject = new SWP_InternalTimedClass((int)groupType, ControllerSpeedPercent);
			BroadcastEvents("OnGroupPauseBroadcast", passedObject);
			for (int i = 0; i < childControllers.Count; i++)
			{
				childControllers[i].SpeedDown(0f);
			}
		}
	}

	public override void ResumeGroupTime()
	{
		if (!pause)
		{
			return;
		}
		pause = false;
		float num = localSpeed;
		if (parentController != null)
		{
			if (parentController.pause)
			{
				return;
			}
			num *= parentController.ControllerSpeedZeroToOne;
		}
		SetControllerSpeed(num);
		SWP_InternalTimedClass passedObject = new SWP_InternalTimedClass((int)groupType, ControllerSpeedPercent);
		BroadcastEvents("OnGroupResumeBroadcast", passedObject);
		for (int i = 0; i < childControllers.Count; i++)
		{
			childControllers[i].SpeedUp(childControllers[i].localSpeed);
		}
	}

	protected void SpeedUp(float _NewTime)
	{
		if (!pause)
		{
			float num = _NewTime;
			if (parentController != null)
			{
				num *= parentController.ControllerSpeedZeroToOne;
			}
			SetControllerSpeed(num);
			SWP_InternalTimedClass passedObject = new SWP_InternalTimedClass((int)groupType, ControllerSpeedPercent);
			BroadcastEvents("OnGroupSpeedUpBroadcast", passedObject);
			for (int i = 0; i < childControllers.Count; i++)
			{
				childControllers[i].SpeedUp(childControllers[i].localSpeed);
			}
		}
	}

	protected void SpeedDown(float _NewTime)
	{
		if (!pause)
		{
			float num = _NewTime;
			if (parentController != null)
			{
				num *= parentController.ControllerSpeedZeroToOne;
			}
			SetControllerSpeed(num);
			SWP_InternalTimedClass passedObject = new SWP_InternalTimedClass((int)groupType, ControllerSpeedPercent);
			BroadcastEvents("OnGroupSlowDownBroadcast", passedObject);
			for (int i = 0; i < childControllers.Count; i++)
			{
				childControllers[i].SpeedDown(childControllers[i].localSpeed);
			}
		}
	}

	public override void SpeedUpGroupTime(float _NewTime)
	{
		if (localSpeed != _NewTime)
		{
			localSpeed = _NewTime;
			SpeedUp(localSpeed);
		}
	}

	public override void SlowDownGroupTime(float _NewTime)
	{
		if (localSpeed != _NewTime)
		{
			localSpeed = _NewTime;
			SpeedDown(localSpeed);
		}
	}

	public void SetSpeed(float _NewTime)
	{
		if (_NewTime > localSpeed)
		{
			SpeedUpGroupTime(_NewTime);
		}
		else
		{
			SlowDownGroupTime(_NewTime);
		}
	}
}
