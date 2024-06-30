using System.Collections.Generic;
using UnityEngine;

public class TimePauseChecker : MonoBehaviour
{
	public TimeGroupController timeGroupController;

	public List<TimeGroupController> pauseTargetGroup;

	private void Awake()
	{
		timeGroupController.onAddObject = delegate
		{
			if (timeGroupController.objectCnt > 0)
			{
				for (int j = 0; j < pauseTargetGroup.Count; j++)
				{
					pauseTargetGroup[j].PauseGroupTime();
				}
			}
		};
		timeGroupController.onRemoveObject = delegate
		{
			if (timeGroupController.objectCnt <= 0)
			{
				for (int i = 0; i < pauseTargetGroup.Count; i++)
				{
					pauseTargetGroup[i].ResumeGroupTime();
				}
			}
		};
	}
}
