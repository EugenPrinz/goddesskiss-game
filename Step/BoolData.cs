using System;

namespace Step
{
	public class BoolData : AbstractVariable
	{
		[Serializable]
		public class VariableData
		{
			public bool value;
		}

		public VariableData data;

		public bool value
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
			if (val is BoolData)
			{
				data = ((BoolData)val).data;
				return true;
			}
			return false;
		}

		public override bool Copy(AbstractVariable val)
		{
			if (val is BoolData)
			{
				value = ((BoolData)val).value;
				return true;
			}
			return false;
		}
	}
}
