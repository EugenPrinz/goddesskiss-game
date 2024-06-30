namespace Step
{
	public class SetVariableData : AbstractStepAction
	{
		public AbstractVariable target;

		public AbstractVariable value;

		public override bool Enter()
		{
			if (value == null || target == null)
			{
				return false;
			}
			if (!target.Copy(value))
			{
				return false;
			}
			return base.Enter();
		}
	}
}
