namespace Step
{
	public class IsBoolValue : AbstractStepCondition
	{
		public BoolData boolData;

		public bool equals;

		public override bool Enter()
		{
			if (boolData == null)
			{
				return false;
			}
			return base.Enter();
		}

		public override bool Validate()
		{
			return boolData.value == equals;
		}
	}
}
