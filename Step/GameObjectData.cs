using System;
using UnityEngine;

namespace Step
{
	public class GameObjectData : AbstractVariable
	{
		[Serializable]
		public class VariableData
		{
			public GameObject value;
		}

		public VariableData data;

		public GameObject value
		{
			get
			{
				return data.value;
			}
			set
			{
				data.value = value;
			}
		}

		public override bool Set(AbstractVariable val)
		{
			if (val is GameObjectData)
			{
				data = ((GameObjectData)val).data;
				return true;
			}
			return false;
		}

		public override bool Copy(AbstractVariable val)
		{
			if (val is GameObjectData)
			{
				value = ((GameObjectData)val).value;
				return true;
			}
			return false;
		}
	}
}
