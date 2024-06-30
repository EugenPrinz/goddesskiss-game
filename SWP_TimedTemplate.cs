using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class SWP_TimedTemplate : SWP_InternalTimedGameObject
{
	private TrailRenderer trTrailRenderer;

	private float fOriginalSpeed;

	private void Awake()
	{
		if (SearchObjects)
		{
			trTrailRenderer = GetComponent<TrailRenderer>();
			fOriginalSpeed = trTrailRenderer.time;
		}
	}

	protected override void ClearAssignedObjects()
	{
		trTrailRenderer = null;
	}

	protected override void SetSpeedLooping(float _fNewSpeed, float _fCurrentSpeedPercent, float _fCurrentSpeedZeroToOne)
	{
		for (int i = 0; i < AssignedObjects.Length; i++)
		{
			if (AssignedObjects[i].GetType() == typeof(TrailRenderer))
			{
				trTrailRenderer = (TrailRenderer)AssignedObjects[i];
			}
			SetSpeedAssigned(_fNewSpeed, _fCurrentSpeedPercent, _fCurrentSpeedZeroToOne);
		}
		ClearAssignedObjects();
	}

	protected override void SetSpeedAssigned(float _fNewSpeed, float _fCurrentSpeedPercent, float _fCurrentSpeedZeroToOne)
	{
		if (trTrailRenderer != null)
		{
			trTrailRenderer.time = GetNewSpeedFromPercentage(fOriginalSpeed, trTrailRenderer.time, _bReverse: true);
		}
	}
}
