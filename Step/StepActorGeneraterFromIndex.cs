using System;
using System.Collections.Generic;
using UnityEngine;

namespace Step
{
	public class StepActorGeneraterFromIndex : AbstractStepUpdateAction
	{
		[Serializable]
		public class ElementStepData
		{
			public int num;

			public StepActor prefab;
		}

		[Serializable]
		public class ElementParamData
		{
			public string key;

			public AbstractVariable value;
		}

		public GameObjectData parnetData;

		public IntData indexData;

		public List<ElementStepData> datas;

		public List<ElementParamData> parameters;

		protected int curNum = -1;

		protected AbstractStep curStep;

		protected Dictionary<int, ElementStepData> _dicDatas;

		protected void RefreshElementDict()
		{
			_dicDatas = new Dictionary<int, ElementStepData>();
			for (int i = 0; i < datas.Count; i++)
			{
				_dicDatas.Add(datas[i].num, datas[i]);
			}
		}

		public override bool Enter()
		{
			if (indexData == null)
			{
				return false;
			}
			RefreshElementDict();
			return base.Enter();
		}

		protected override void OnEnter()
		{
			OnUpdate();
		}

		protected override bool CanUpdate()
		{
			if (curNum == indexData.value)
			{
				return false;
			}
			return base.CanUpdate();
		}

		protected override void OnUpdate()
		{
			curNum = indexData.value;
			if (!_dicDatas.ContainsKey(indexData.value))
			{
				return;
			}
			ElementStepData elementStepData = _dicDatas[indexData.value];
			if (elementStepData == null)
			{
				return;
			}
			StepActor stepActor = UnityEngine.Object.Instantiate(elementStepData.prefab);
			if (stepActor == null)
			{
				return;
			}
			if (parameters != null && parameters.Count > 0)
			{
				for (int i = 0; i < parameters.Count; i++)
				{
					if (!stepActor.SetParameter(parameters[i].key, parameters[i].value))
					{
					}
				}
			}
			if (parnetData != null)
			{
				stepActor.transform.parent = parnetData.value.transform;
			}
			stepActor.transform.position = base.transform.position;
			stepActor.transform.localPosition = stepActor.transform.localPosition + elementStepData.prefab.transform.localPosition;
			stepActor.transform.localScale = elementStepData.prefab.transform.localScale;
			stepActor.transform.localRotation = elementStepData.prefab.transform.localRotation;
			stepActor.enabled = true;
		}
	}
}
