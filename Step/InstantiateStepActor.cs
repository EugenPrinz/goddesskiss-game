using System;
using System.Collections.Generic;
using UnityEngine;

namespace Step
{
	public class InstantiateStepActor : AbstractStepAction
	{
		[Serializable]
		public class ElementParamData
		{
			public string key;

			public AbstractVariable value;
		}

		public GameObjectData parnetData;

		public StepActor prefabStep;

		public List<ElementParamData> parameters;

		public override bool Enter()
		{
			if (prefabStep == null)
			{
				return false;
			}
			if (parnetData != null && parnetData.value == null)
			{
				return false;
			}
			StepActor stepActor = UnityEngine.Object.Instantiate(prefabStep);
			if (stepActor == null)
			{
				return false;
			}
			if (parameters != null && parameters.Count > 0)
			{
				for (int i = 0; i < parameters.Count; i++)
				{
					if (!stepActor.SetParameter(parameters[i].key, parameters[i].value))
					{
						return false;
					}
				}
			}
			if (parnetData != null)
			{
				stepActor.transform.parent = parnetData.value.transform;
			}
			stepActor.transform.position = base.transform.position;
			stepActor.transform.localPosition = stepActor.transform.localPosition + prefabStep.transform.localPosition;
			stepActor.transform.localScale = prefabStep.transform.localScale;
			stepActor.transform.localRotation = prefabStep.transform.localRotation;
			stepActor.enabled = true;
			return base.Enter();
		}
	}
}
