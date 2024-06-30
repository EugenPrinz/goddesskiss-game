namespace Step
{
	public class StepData : AbstractVariable
	{
		public AbstractStep value;

		public override bool Set(AbstractVariable val)
		{
			return false;
		}

		public override bool Copy(AbstractVariable val)
		{
			return false;
		}
	}
}
