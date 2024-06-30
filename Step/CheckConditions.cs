using System.Collections.Generic;
using UnityEngine;

namespace Step
{
	public class CheckConditions : AbstractStepCondition
	{
		public List<AbstractStepCondition> conditions;

		public override bool Enter()
		{
			bool flag = true;
			for (int i = 0; i < conditions.Count; i++)
			{
				if (!conditions[i].Enter())
				{
					return false;
				}
				if (!conditions[i].IsFinish)
				{
					flag = false;
				}
			}
			if (flag)
			{
				_isFinish = true;
				return true;
			}
			return base.Enter();
		}

		public override bool Exit()
		{
			for (int i = 0; i < conditions.Count; i++)
			{
				if (!(conditions[i] == null))
				{
					conditions[i].Exit();
				}
			}
			conditions.Clear();
			OnExit();
			Object.DestroyImmediate(base.gameObject);
			return true;
		}

		public override bool Validate()
		{
			for (int i = 0; i < conditions.Count; i++)
			{
				if (!conditions[i].Validate())
				{
					return false;
				}
			}
			return true;
		}
	}
}
