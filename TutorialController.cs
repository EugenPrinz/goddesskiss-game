using Step;
using UnityEngine;

public class TutorialController : Manager<TutorialController>
{
	public bool enable = true;

	public string PrefabHome;

	[HideInInspector]
	public int curStep;

	[HideInInspector]
	public StepActor activedTutorial;

	public StepActor Create(int step)
	{
		if (!enable)
		{
			return null;
		}
		if (activedTutorial != null)
		{
			return null;
		}
		string path = $"{PrefabHome}/{step.ToString()}";
		StepActor stepActor = Resources.Load<StepActor>(path);
		if (stepActor == null)
		{
			return null;
		}
		curStep = step;
		activedTutorial = Object.Instantiate(stepActor);
		activedTutorial.step.StepNum = curStep;
		activedTutorial.transform.parent = base.transform;
		activedTutorial.OnFinish += delegate
		{
			activedTutorial = null;
		};
		return activedTutorial;
	}
}
