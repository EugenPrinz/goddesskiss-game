namespace Step
{
	public class IsActiveObject : AbstractStepCondition
	{
		public GameObjectData target;

		public bool equals;

		public override bool Enter()
		{
			if (target == null || target.value == null)
			{
				return false;
			}
			return base.Enter();
		}

		public override bool Validate()
		{
			return target.value.activeSelf == equals;
		}
	}
}
