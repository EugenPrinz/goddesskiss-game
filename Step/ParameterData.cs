using System;
using System.Collections.Generic;
using UnityEngine;

namespace Step
{
	public class ParameterData : MonoBehaviour
	{
		[Serializable]
		public class ElementData
		{
			public string key;

			public AbstractVariable value;
		}

		public List<ElementData> datas;

		protected Dictionary<string, ElementData> _dicDatas;

		protected void RefreshElementDict()
		{
			_dicDatas = new Dictionary<string, ElementData>();
			for (int i = 0; i < datas.Count; i++)
			{
				_dicDatas.Add(datas[i].key, datas[i]);
			}
		}

		public bool Set(string key, AbstractVariable param)
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
	}
}
