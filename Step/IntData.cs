using System;

namespace Step
{
	public class IntData : AbstractVariable
	{
		[Serializable]
		public class VariableData
		{
			public int value;
		}

		public VariableData data;

		public int value
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
			if (val is IntData)
			{
				data = ((IntData)val).data;
				return true;
			}
			return false;
		}

		public override bool Copy(AbstractVariable val)
		{
			if (val is IntData)
			{
				value = ((IntData)val).value;
				return true;
			}
			return false;
		}
	}
}
