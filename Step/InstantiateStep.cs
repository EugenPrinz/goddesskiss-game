using System;
using System.Collections.Generic;
using UnityEngine;

namespace Step
{
	public class InstantiateStep : AbstractStepAction
	{
		[Serializable]
		public class ElementData
		{
			public string key;

			public AbstractVariable value;
		}

		public bool isPriority;

		public bool isLock = true;

		public GameObjectData parnetData;

		public AbstractStep prefabStep;

		public GameObjectData ret;

		public List<ElementData> parameter;

		protected AbstractStep createdStep;

		public override bool IsPriority
		{
			get
			{
				return isPriority;
			}
			set
			{
				isPriority = value;
			}
		}

		public override bool IsLock
		{
			get
			{
				return isLock;
			}
			set
			{
				isLock = value;
			}
		}

		public override bool IsEveryFrameUpdate => true;

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
			createdStep = UnityEngine.Object.Instantiate(prefabStep);
			if (createdStep == null)
			{
				return false;
			}
			if (parameter != null && parameter.Count > 0)
			{
				ParameterData component = createdStep.GetComponent<ParameterData>();
				if (component != null)
				{
					for (int i = 0; i < parameter.Count; i++)
					{
						if (!component.Set(parameter[i].key, parameter[i].value))
						{
							return false;
						}
					}
				}
			}
			if (parnetData != null)
			{
				createdStep.transform.parent = parnetData.value.transform;
			}
			else
			{
				createdStep.transform.parent = base.transform;
			}
			createdStep.transform.position = base.transform.position;
			createdStep.transform.localPosition = createdStep.transform.localPosition + prefabStep.transform.localPosition;
			createdStep.transform.localScale = prefabStep.transform.localScale;
			createdStep.transform.localRotation = prefabStep.transform.localRotation;
			createdStep.enabled = true;
			if (!createdStep.Enter())
			{
				return false;
			}
			if (ret != null)
			{
				ret.value = createdStep.gameObject;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			if (createdStep.IsFinish)
			{
				_isFinish = true;
			}
		}

		public override bool Exit()
		{
			if (createdStep != null)
			{
				createdStep.Exit();
				if (!(createdStep is AbstractStepContainer))
				{
					UnityEngine.Object.DestroyImmediate(createdStep.gameObject);
				}
				createdStep = null;
				if (ret != null)
				{
					ret.value = null;
				}
			}
			OnExit();
			return true;
		}

		protected override void OnUpdate()
		{
			if (createdStep != null)
			{
				createdStep.UpdateStep();
				if (createdStep.IsFinish)
				{
					_isFinish = true;
				}
			}
		}
	}
}
