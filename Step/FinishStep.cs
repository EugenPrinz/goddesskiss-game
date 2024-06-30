namespace Step
{
	public class FinishStep : AbstractStepAction
	{
		public AbstractStep step;

		public override bool Enter()
		{
			if (step == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			step.IsFinish = true;
		}
	}
}
