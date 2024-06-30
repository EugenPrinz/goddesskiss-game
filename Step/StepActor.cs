using System;
using System.Collections.Generic;
using UnityEngine;

namespace Step
{
	public class StepActor : MonoBehaviour
	{
		[Serializable]
		public class ElementParamData
		{
			public string key;

			public AbstractVariable value;
		}

		public delegate void Delegate();

		public AbstractStep step;

		public List<ElementParamData> parameters;

		protected Dictionary<string, ElementParamData> _dicDatas;

		public event Delegate OnFinish;

		protected void RefreshElementDict()
		{
			_dicDatas = new Dictionary<string, ElementParamData>();
			for (int i = 0; i < parameters.Count; i++)
			{
				_dicDatas.Add(parameters[i].key, parameters[i]);
			}
		}

		public bool SetParameter(string key, AbstractVariable param)
		{
			if (_dicDatas == null)
			{
				RefreshElementDict();
			}
			if (!_dicDatas.ContainsKey(key))
			{
				return false;
			}
			AbstractVariable value = _dicDatas[key].value;
			if (value == null)
			{
				return false;
			}
			return value.Set(param);
		}

		private void Start()
		{
			if (step == null)
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
				return;
			}
			if (!step.Enter())
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
			if (step.IsFinish)
			{
				if (this.OnFinish != null)
				{
					this.OnFinish();
				}
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
		}

		private void Update()
		{
			step.UpdateStep();
			if (step.IsFinish)
			{
				step.Exit();
				if (!(step is AbstractStepContainer))
				{
					UnityEngine.Object.DestroyImmediate(base.gameObject);
				}
				step = null;
				if (this.OnFinish != null)
				{
					this.OnFinish();
				}
			}
		}
	}
}
