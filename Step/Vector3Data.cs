using System;
using UnityEngine;

namespace Step
{
	public class Vector3Data : AbstractVariable
	{
		[Serializable]
		public class VariableData
		{
			public Vector3 value;
		}

		public VariableData data;

		public Vector3 value
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
			if (val is Vector3Data)
			{
				data = ((Vector3Data)val).data;
				return true;
			}
			return false;
		}

		public override bool Copy(AbstractVariable val)
		{
			if (val is Vector3Data)
			{
				value = ((Vector3Data)val).value;
				return true;
			}
			return false;
		}
	}
}
