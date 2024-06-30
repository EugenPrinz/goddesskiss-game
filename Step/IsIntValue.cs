namespace Step
{
	public class IsIntValue : AbstractStepCondition
	{
		public enum E_COMPARE_TYPE
		{
			LESS,
			GREATER,
			NONE
		}

		public IntData data;

		public int value;

		public E_COMPARE_TYPE compareType;

		public bool equals;

		public override bool Enter()
		{
			if (data == null)
			{
				return false;
			}
			return base.Enter();
		}

		public override bool Validate()
		{
			if (equals && data.value == value)
			{
				return true;
			}
			switch (compareType)
			{
			case E_COMPARE_TYPE.LESS:
				if (data.value < value)
				{
					return true;
				}
				break;
			case E_COMPARE_TYPE.GREATER:
				if (data.value > value)
				{
					_isFinish = true;
					return true;
				}
				break;
			}
			return false;
		}
	}
}
