using System.Collections.Generic;
using UnityEngine;

namespace Step
{
	public class StepPhase : AbstractStepContainer
	{
		public List<AbstractStep> steps;

		protected int lastActivedStepIdx;

		protected List<AbstractStep> activedSteps;

		protected AbstractStep priorityStep;

		public void ChangeStep()
		{
			for (int i = lastActivedStepIdx + 1; i < steps.Count; i++)
			{
				AbstractStep abstractStep = steps[i];
				abstractStep.enabled = true;
				abstractStep.StepNum = i;
				if (!abstractStep.Enter())
				{
					_enable = false;
					break;
				}
				lastActivedStepIdx = i;
				if (abstractStep.IsFinish)
				{
					steps[abstractStep.StepNum] = null;
					abstractStep.Exit();
				}
				else if (abstractStep.IsEveryFrameUpdate)
				{
					if (abstractStep.IsPriority)
					{
						priorityStep = abstractStep;
						break;
					}
					activedSteps.Add(abstractStep);
					if (abstractStep.IsLock)
					{
						break;
					}
				}
			}
		}

		public override bool Enter()
		{
			activedSteps = new List<AbstractStep>();
			bool flag = true;
			for (int i = 0; i < steps.Count; i++)
			{
				if (!flag)
				{
					steps[i].enabled = false;
					continue;
				}
				AbstractStep abstractStep = steps[i];
				abstractStep.enabled = true;
				abstractStep.StepNum = i;
				if (!abstractStep.Enter())
				{
					_enable = false;
					return false;
				}
				lastActivedStepIdx = i;
				if (abstractStep.IsFinish)
				{
					steps[abstractStep.StepNum] = null;
					abstractStep.Exit();
				}
				else if (abstractStep.IsEveryFrameUpdate)
				{
					if (abstractStep.IsPriority)
					{
						priorityStep = abstractStep;
						flag = false;
						continue;
					}
					activedSteps.Add(abstractStep);
					if (abstractStep.IsLock)
					{
						flag = false;
					}
				}
				else
				{
					flag = false;
				}
			}
			return base.Enter();
		}

		public override bool Exit()
		{
			if (priorityStep != null)
			{
				priorityStep.Exit();
				priorityStep = null;
			}
			for (int i = 0; i < activedSteps.Count; i++)
			{
				activedSteps[i].Exit();
			}
			activedSteps.Clear();
			OnExit();
			Object.DestroyImmediate(base.gameObject);
			return true;
		}

		protected override void OnUpdate()
		{
			if (priorityStep != null)
			{
				priorityStep.UpdateStep();
				if (priorityStep.IsFinish)
				{
					int stepNum = priorityStep.StepNum;
					steps[stepNum] = null;
					priorityStep.Exit();
					priorityStep = null;
					if (stepNum == lastActivedStepIdx)
					{
						ChangeStep();
					}
				}
				return;
			}
			if (activedSteps.Count <= 0)
			{
				_isFinish = true;
				return;
			}
			int num = 0;
			while (num < activedSteps.Count)
			{
				AbstractStep abstractStep = activedSteps[num];
				abstractStep.UpdateStep();
				if (abstractStep.IsFinish)
				{
					int stepNum2 = abstractStep.StepNum;
					steps[stepNum2] = null;
					abstractStep.Exit();
					activedSteps.RemoveAt(num);
					if (stepNum2 == lastActivedStepIdx)
					{
						ChangeStep();
						break;
					}
				}
				else
				{
					num++;
				}
			}
		}
	}
}
