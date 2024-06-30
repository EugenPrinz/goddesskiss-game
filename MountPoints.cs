using System;
using System.Collections.Generic;
using UnityEngine;

public class MountPoints : AbstractMountPoints
{
	[Serializable]
	public class MountElement
	{
		public string key;

		public Transform position;
	}

	public List<MountElement> datas;

	protected Dictionary<string, Transform> _dicDatas;

	protected virtual void RefreshElementDict()
	{
		_dicDatas = new Dictionary<string, Transform>();
		for (int i = 0; i < datas.Count; i++)
		{
			_dicDatas.Add(datas[i].key, datas[i].position);
		}
	}

	private void Start()
	{
		RefreshElementDict();
	}

	public override Transform GetPosition(string key)
	{
		if (_dicDatas == null)
		{
			RefreshElementDict();
		}
		if (!_dicDatas.ContainsKey(key))
		{
			return null;
		}
		return _dicDatas[key];
	}
}
