using UnityEngine;

public class SWP_InternalTimedGameObject : MonoBehaviour
{
	[SerializeField]
	public SWP_TimeGroupController TimeGroupController;

	[SerializeField]
	public int ControllerGroupID = 1;

	[SerializeField]
	public bool SearchObjects = true;

	[SerializeField]
	public Object[] AssignedObjects;

	internal float fCurrentSpeedPercent = 100f;

	internal float fCurrentSpeedZeroToOne = 1f;

	protected float fPreviousSpeedPercentage = 100f;

	private void Start()
	{
		if (TimeGroupController != null)
		{
			SetSpeed(TimeGroupController.ControllerSpeedPercent);
		}
	}

	protected float GetNewSpeedFromPercentage(float _fInSpeed)
	{
		float num = ((fPreviousSpeedPercentage == 0f) ? _fInSpeed : (100f / fPreviousSpeedPercentage * _fInSpeed));
		return num / 100f * fCurrentSpeedPercent;
	}

	protected float GetNewSpeedFromPercentage(float _fOriginalSpeed, float _fInSpeed, bool _bReverse)
	{
		if (_bReverse && fCurrentSpeedPercent != 0f)
		{
			return _fOriginalSpeed * (100f / fCurrentSpeedPercent);
		}
		if (_bReverse)
		{
			return _fOriginalSpeed * 99999.99f;
		}
		return _fOriginalSpeed / 100f * fCurrentSpeedPercent;
	}

	protected virtual void ClearAssignedObjects()
	{
	}

	protected virtual void SetSpeedLooping(float _fNewSpeed, float _fCurrentSpeedPercent, float _fCurrentSpeedZeroToOne)
	{
	}

	protected virtual void SetSpeedAssigned(float _fNewSpeed, float _fCurrentSpeedPercent, float _fCurrentSpeedZeroToOne)
	{
	}

	protected virtual void SetSpeed(float _fNewSpeed)
	{
		fCurrentSpeedPercent = Mathf.Clamp(_fNewSpeed, 0f, 1000f);
		fCurrentSpeedZeroToOne = ((fCurrentSpeedPercent != 0f) ? (fCurrentSpeedPercent / 100f) : 0f);
		if (SearchObjects)
		{
			SetSpeedAssigned(_fNewSpeed, fCurrentSpeedPercent, fCurrentSpeedZeroToOne);
		}
		else
		{
			SetSpeedLooping(_fNewSpeed, fCurrentSpeedPercent, fCurrentSpeedZeroToOne);
		}
		if (fCurrentSpeedPercent != 0f)
		{
			fPreviousSpeedPercentage = fCurrentSpeedPercent;
		}
	}

	public float TimedDeltaTime()
	{
		if (fCurrentSpeedPercent != 0f)
		{
			return Time.deltaTime / (100f / fCurrentSpeedPercent);
		}
		return 0f;
	}

	private void OnGroupPauseBroadcast(SWP_InternalTimedClass _swpTimedClass)
	{
		if (_swpTimedClass.GroupID == ControllerGroupID)
		{
			SetSpeed(_swpTimedClass.NewSpeed);
		}
	}

	private void OnGroupSlowDownBroadcast(SWP_InternalTimedClass _swpTimedClass)
	{
		if (_swpTimedClass.GroupID == ControllerGroupID)
		{
			SetSpeed(_swpTimedClass.NewSpeed);
		}
	}

	private void OnGroupSpeedUpBroadcast(SWP_InternalTimedClass _swpTimedClass)
	{
		if (_swpTimedClass.GroupID == ControllerGroupID)
		{
			SetSpeed(_swpTimedClass.NewSpeed);
		}
	}

	private void OnGroupResumeBroadcast(SWP_InternalTimedClass _swpTimedClass)
	{
		if (_swpTimedClass.GroupID == ControllerGroupID)
		{
			SetSpeed(_swpTimedClass.NewSpeed);
		}
	}

	private void OnGlobalPauseBroadcast()
	{
	}

	private void OnGlobalResumeBroadcast()
	{
	}
}
