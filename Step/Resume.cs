namespace Step
{
	public class Resume : AbstractStepEvent
	{
		public Suspend suspendStep;

		public override bool IsLock => false;

		public override bool IsEveryFrameUpdate => false;

		public override bool Enter()
		{
			if (suspendStep == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			suspendStep.IsFinish = true;
		}
	}
}
